FROM microsoft/aspnetcore-build:1.0-2.0 AS build

COPY . /src
WORKDIR /src
RUN /bin/bash -c "dotnet restore ./reservieren.sln && dotnet publish ./reservieren.sln -c Release -o ./obj/Docker/publish"

FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build /src/reservieren/obj/Docker/publish .
ENTRYPOINT ["dotnet", "reservieren.dll"]