# TAPICallerID (für T2med)
TAPICallerID ist eine kleine TrayApp, die über ein TAPI-Device ([s. Microsoft TAPI Übersicht](https://learn.microsoft.com/de-de/windows/win32/tapi/telephony-application-programming-interfaces)) eine CTI-Integration in die Praxissoftware T2med ermöglicht. (Eine Nutzung ist nur in Verbindung mit T2med sinnvoll - getestet bis Version 24.6.4)
Dabei werden eingehende Anrufe (z.B. von einer Fritzbox oder sonstigen Telefonanlage) in der TrayIcon-Leiste signalisiert und der zur Rufnummer passende Patienteneintrag kann mit einem Doppelklick direkt im Praxisverwaltungssystem aufgerufen werden.

![](https://github.com/INT3hex/TAPICallerID/blob/master/doc/TrayIcon.png)
- Die Suche des Patienteneintrags in der T2med-Anwendung erfolgt dabei über die interne T2med Patientennummer.
- Die Telefonnummern-Erkennung und Zuordnung zu Name bzw. Patientennummer erfolgt
  
  a) entweder direkt über den via TAPI übergebenen Anrufernamen (CallerID), sofern er in der Telefonanlage (z.B. Fritzbox) gepflegt ist. Hierfür muss im Feld Anrufername am Ende das Hash-Zeichen (#) gefolgt von der Patientennummer übergeben werden. (dieser Weg ist aus Datenschutzkonformitätsgründen empfehlenswert)

  b) oder separat über eine CSV-Export-Datei (erstellt via 'kombinierte Suche' in T2med) 

## Voraussetzung (TAPI-Device)
Als Schnittstelle zur Telefonanlage ist ein systemseitig installiertes TAPI-Device notwendig.
Hierfür kann z.B. der kostenlose TAPI-Provider **SIPTAPI** genutzt werden, aber auch weitere TAPI-Provider wie 3CX, TAPICall, PhoneSuite oder Auerswald können prinzipiell genutzt werden.

[siptapi v0.2.17](https://sourceforge.net/projects/siptapi/files/siptapi/) initiale Version

[siptapi v0.3.23](https://github.com/nic-at/siptapi) erweiterte Version

Für die Anbindung an eine Fritzbox nutzen wir SIPTAPI in der *Release SingleLine* 64bit-Version v0.3.23.
* SIPTAPI (siptapi_0.3.tsp) wird nach C:\Windows\System32 kopiert
* Über die Systemsteuerung "Telefon und Modem" wird der TAPI-Provider installiert und konfiguriert: ![](https://github.com/INT3hex/TAPICallerID/blob/master/doc/Systemsteuerung_Telefon%20und%20Modem.PNG)
* In SIPTAPI wird die Telefonanlage (SIP Domain), sowie der zu nutzende SIP-Account (SIP User / Password) konfiguriert. ![](https://github.com/INT3hex/TAPICallerID/blob/master/doc/SIPTAPI-Konfiguration.PNG)
* Der SIP-Account ist muss zum eingerichteten SIP/Telefoniegerät in der Telefonanlage (z.B. Fritzbox) passen. ![](https://github.com/INT3hex/TAPICallerID/blob/master/doc/Fritzbox_Telefonieger%C3%A4te.png)

**Hinweis**: 
SIPTAPI speichert die Konfigurationsinformationen im Registry-Key [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Telephony\SIPTAPI]. 

Das Passwort des SIP-Accounts wird dabei im Klartext gespeichert.


## Installation TAPICallerID
Die TrayApp (TrayApp.Exe) ist grundsätzlich eine 'portable App' und muss nicht installiert werden.
Beim ersten Start der Applikation wird eine Fehlermeldung erscheinen, dass keine TAPI-ID (= konfigurierter TAPI-Provider) gefunden wurde:
![](https://github.com/INT3hex/TAPICallerID/blob/master/doc/TrayApp_NoTAPI.PNG)

Die verfügbaren TAPI-Provider werden angezeigt (s. highlighted Zeile). Wenn kein TAPI-Provider angezeigt wird --> siehe [Voraussetzungen](https://github.com/INT3hex/TAPICallerID/new/master?filename=README.md#voraussetzung-tapi-device) . 

Beim Beenden der TrayApp wird im Verzeichnis der Applikation eine Konfigurationsdatei (TrayApp.exe.config) mit der Defaultkonfiguration erzeugt.
Die Konfigurationsdatei kann nun mit einem beliebigen Texteditor (Notepad) editiert werden.

* In der Konfigurationsdatei muss die beim ersten Start angezeigte ID des verfügbaren TAPI-Providers unter dem Schlüsselwort "sSIPAddr" eingetragen werden.
* Weiterhin kann eine CSV-Patientendatei mit den hinterlegten Telefonnummern im Schlüsselwort "sCsvFile" angegeben werden
* die Schlüsselwörtern "iCsvPos..." geben die Position der einzulesenden Attribute aus der CSV-Datei an. Im Default wird eine Standard-T2med-Export-Datei mit folgenden Exportdaten unterstützt:
```bash
Patientennummer;Anrede;Titel;Namensvorsatz;Namenszusatz;Vorname;Nachname;Geburtsdatum;Geburtsort;Geschlecht;Staatsangehörigkeit;Geburtsname;Verstorben;Sterbedatum;Straße;Hausnummer;Zusatz;PLZ;Ort;Ländercode;Hinweis;Postfach;"Postfach-PLZ";"Postfach-Ort";"Postfach-Ländercode";"E-Mail";"Bevorzugter Benachrichtigungsweg";"Benachrichtigung erlaubt";"Telefon (Privat)";"Telefon (Mobil)";"Telefon (Arbeit)";"Telefon (Sonstiges)";Hausarzt;Chroniker
```
allerdings kann auch jede beliebig andere CSV-Datei mit den entsprechenden Werten (Patientennummer, Vorname, Nachname, Telefon, ...) genutzt werden; dafür müssen die Positionen der Attribute dann entsprechend angepasst werden.
* Wird das Schlüsselwort "sCsvVorwahl" angegeben, dann werden beim Einlesen der CSV-Datei alle Telefoneinträge ohne explizite Ortsvorwahl z.B. 658492 durch die eingetragene ergänzt (--> 0711658492)

Beispielkonfiguration:
```bash
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <appSettings>
        <add key="bDirectSearchOnDblClick" value="True" />
        <add key="sCsvFile" value="C:\Daten\t2med_PatientenlisteExport.csv" />
        <add key="sCsvVorwahl" value="" />
        <add key="iCsvPosPatientennummer" value="0" />
        <add key="iCsvPosVorname" value="5" />
        <add key="iCsvPosName" value="6" />
        <add key="iCsvPosTel1" value="28" />
        <add key="iCsvPosTel2" value="29" />
        <add key="iCsvPosTel3" value="30" />
        <add key="iCsvPosTel4" value="31" />
        <add key="sSIPAddr" value="SIPTAPI 001: sip:TAPIuser@192.168.178.1" />
    </appSettings>
</configuration>
```
## Debuginformationen
Die TrayApp erstellt aktuell kein Logfile - jedoch können interne Debuginformationen (z.B. Fehlermeldungen, Debugausgaben, usw.) zur Laufzeit mittels [Sysinternals DebugView](https://learn.microsoft.com/de-de/sysinternals/downloads/debugview) angezeigt werden.

## Disclaimer
Für die Nutzung, Eignung, Fehlerfreiheit, Nützlichkeit etc. der Software wird keine Haftung übernommen. Die Anwendung und Anleitung wurde nach bestem Wissen erstellt. Der Quellcode kann eingesehen (und beliebig kopiert und verändert) werden. Das bereitgestellte Binärfile TrayApp.exe in v1.0 wurde bei Virustotal validiert. 
[Virustotal Analyse](https://www.virustotal.com/gui/file/123def58a400cafe24bfa4b602b2ea53966953ee6b36893eacdafb6517132049)
