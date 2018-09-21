namespace reservieren
{
    using System;
    using System.IO;

    using Akka.Actor;
    using Akka.Configuration;
    using Akka.Persistence.Query;
    using Akka.Persistence.Query.Sql;
    using Akka.Streams;

    using Controllers;

    using Eventing;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Models;

    using Services;

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection pServices)
        {
            /*
             * Due a bug in akka hocon, setting of Enviroment variables has to be on our one.
             * Therefore this variables has to be set and it is not optional!.
             */

            string appConfig = File.ReadAllText("app.config");
            appConfig = appConfig.Replace("${CONTAINER_BOOTCAMP_EINBUCHEN_URL}",
                                          Environment.GetEnvironmentVariable("CONTAINER_BOOTCAMP_EINBUCHEN_URL"));
            appConfig = appConfig.Replace("${CONTAINER_BOOTCAMP_AUSLEIHEN_URL}",
                                          Environment.GetEnvironmentVariable("CONTAINER_BOOTCAMP_AUSLEIHEN_URL"));
            appConfig = appConfig.Replace("${CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_USER}",
                                          Environment.GetEnvironmentVariable(
                                                  "CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_USER"));
            appConfig = appConfig.Replace("${CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_PASSWORD}",
                                          Environment.GetEnvironmentVariable(
                                                  "CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_PASSWORD"));
            appConfig = appConfig.Replace("${CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_HOST}",
                                          Environment.GetEnvironmentVariable(
                                                  "CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_HOST"));
            appConfig = appConfig.Replace("${CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_DBNAME}",
                                          Environment.GetEnvironmentVariable(
                                                  "CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_DBNAME"));

            var config = ConfigurationFactory.ParseString(appConfig);
            var system = ActorSystem.Create("reservieren", config);
            var materializer = ActorMaterializer.Create(system);

            var readJournal = PersistenceQuery
                    .Get(system).ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");

            var booklibrarian = system.ActorOf(BookLibrarian.props, "book-librarian");
            var booklibrarianLookup = system.ActorOf(BookLibrarianLookup.props(readJournal), "book-librarian-lookup");
            var eventConnectionHolder = system.ActorOf(ConnectionHolder.props(), "event-connection-holder");
            
            pServices.AddTransient(typeof(IBookLibrarianActorRef),
                                  pServiceProvider => new BookLibrarianActorRef(booklibrarian));
            pServices.AddTransient(typeof(IBookLibrarianLookupActorRef),
                                  pServiceProvider => new BookLIbrarianLookupActorRef(booklibrarianLookup));
            pServices.AddTransient(typeof(SqlReadJournal), pServiceProvider => readJournal);
            pServices.AddTransient(typeof(ActorMaterializer), pServiceProvider => materializer);
            pServices.AddTransient(typeof(IEventConnectionHolderActorRef), pServiceProvider => new EventConnectionHolderActorRefActorRef(eventConnectionHolder));
            pServices.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder pApp, IHostingEnvironment pEnv)
        {
            if (pEnv.IsDevelopment())
            {
                pApp.UseDeveloperExceptionPage();
            }
            else
            {
                pApp.UseExceptionHandler("/Home/Error");
            }

            pApp.UseStaticFiles();

            pApp.UseMvc(pRoutes =>
                       {
                           pRoutes.MapRoute(
                                   "default",
                                   "{controller=Reservieren}/{action=Index}");
                       });
        }
    }
}
