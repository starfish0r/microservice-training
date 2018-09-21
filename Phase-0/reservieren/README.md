#  Reservieren

Reserviert Bücher wenn dieese aktuell nicht ausgeliehen sind.

## Voraussetzungen

### Umgebungsvariablen
Durch einen Bug in der Library akka.net hocon ist es nicht möglich Umgebungsvariblen zu verwenden.
Als workaround werden die benötigten Umgebungsvariablen selbst in die Konfiguration eingefügt.
Dadurch müssen folgende Umgebungsvariablen zwingend gesetzt sein.

* CONTAINER_BOOTCAMP_EINBUCHEN_URL
* CONTAINER_BOOTCAMP_AUSLEIHEN_URL
* CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_USER
* CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_PASSWORD
* CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_HOST
* CONTAINER_BOOTCAMP_AKKA_PERSISTENCE_DBNAME

#### Visual Studio Mac 2017
Im Visual Studio Mac werden Umgebungsvariablen gesetzt unter:
```Projekt -> reservieren Optionen -> Ausführen -> Default```


## Event Source Client from [3ventic/EvtSource](https://github.com/3ventic/EvtSource)

Due a bug in the library on reconnect, use the source code directly in the project
and fix it. Also created a [pull request](https://github.com/3ventic/EvtSource/pull/3) for the fix. 
 