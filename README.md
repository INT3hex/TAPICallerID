# TAPICallerID (für T2med)
TAPICallerID ist eine kleine TrayApp, die über ein TAPI-Device ([s. Microsoft TAPI Übersicht](https://learn.microsoft.com/de-de/windows/win32/tapi/telephony-application-programming-interfaces)) eine CTI-Integration in die Praxissoftware T2med ermöglicht. (Eine Nutzung ist nur in Verbindung mit T2med sinnvoll - getestet bis Version 24.8.0)
Dabei werden eingehende Anrufe (z.B. von einer Fritzbox oder sonstigen Telefonanlage) in der TrayIcon-Leiste signalisiert und der zur Rufnummer passende Patienteneintrag kann mit einem Doppelklick und **neu** mittels HotKey direkt im Praxisverwaltungssystem aufgerufen werden.

![](https://github.com/INT3hex/TAPICallerID/blob/master/doc/TrayIcon.png)
- Die Suche des Patienteneintrags in der T2med-Anwendung erfolgt dabei über die interne T2med Patientennummer.
- Die Telefonnummern-Erkennung und Zuordnung zu Name bzw. Patientennummer erfolgt
  
  a) entweder direkt über den via TAPI übergebenen Anrufernamen (CallerID), sofern er in der Telefonanlage (z.B. Fritzbox) gepflegt ist. Hierfür muss im Feld Anrufername am Ende das Hash-Zeichen (#) gefolgt von der Patientennummer übergeben werden. (dieser Weg ist aus Datenschutzkonformitätsgründen empfehlenswert)

  b) oder separat über eine CSV-Export-Datei (erstellt via 'kombinierte Suche' in T2med). Option *sCsvFile* mit fullqualified Pfad. 

  c) ab T2med Version 24.9.0 ist der Aufruf der Patientenkartei über eine Telefonnummer-Rückwärtssuche möglich. Die Nutzung wird ab TAPICallerID v1.1 unterstützt. In der Konfigurationsdatei wird dies über die Option *bFeatureSearchPhonenumber* gesteuert. Wenn weder die Patientennummer aus der Telefonanlage noch durch die CSV-Datei zugesteuert wird, erfolgt die Suche über die per TAPI übergebene Telefonnummer. Diese muss dann entsprechend in T2med gepflegt sein. (Da die Version 24.9.0 derzeit noch nicht produktiv ist, ist diese Funktion bislang ohne realen Funktionstest implementiert.) 

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

Das Passwort des SIP-Accounts wird dabei im Klartext gespeichert!


## Installation TAPICallerID
Die TrayApp (TrayApp.Exe) ist grundsätzlich eine 'portable App' und muss nicht installiert werden.
Ein Start der Anwendung bei Anmeldung (Stichwort *Autostart*) ist sinnvoll und wäre manuell zu konfigurieren.

Beim ersten Start der Applikation wird eine Fehlermeldung erscheinen, dass keine TAPI-ID (= konfigurierte TAPI-Line/Provider) gefunden wurde:
 ![](https://github.com/INT3hex/TAPICallerID/blob/master/doc/TrayApp_NoTAPI.PNG)
 
Die verfügbaren TAPI-Line/Provider werden in der Meldung angezeigt (s. highlighted Zeile). Wenn kein TAPI-Provider angezeigt wird --> siehe [Voraussetzungen](https://github.com/INT3hex/TAPICallerID/new/master?filename=README.md#voraussetzung-tapi-device) .
Verfügbare TAPI-Line/Provider können alternativ auch mittels Powershell im System abgefragt werden:
```powershell
# Create and Initialize TAPI Object
$tapi = New-Object -ComObject TAPI.TAPI.1
$tapi.initialize()
# Enumerate TAPI-Lines/Providers
foreach ($item in $tapi.Addresses) { Write-Host $item.AddressName }
```


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
        <add key="bFeatureSearchPhonenumber" value="True" />
        <add key="sCsvFile" value="C:\Daten\t2med_PatientenlisteExport.csv" />
        <add key="sCsvVorwahl" value="" />
        <add key="iCsvPosPatientennummer" value="0" />
        <add key="iCsvPosVorname" value="5" />
        <add key="iCsvPosName" value="6" />
        <add key="iCsvPosTel1" value="28" />
        <add key="iCsvPosTel2" value="29" />
        <add key="iCsvPosTel3" value="30" />
        <add key="iCsvPosTel4" value="31" />
        <add key="iHotKey" value="183" />
        <add key="iHotKeyModifier" value="0" />
    	<add key="iHotKey2" value="120" />
    	<add key="iHotKeyModifier2" value="0" />
        <add key="sSIPAddr" value="SIPTAPI 001: sip:TAPIuser@192.168.178.1" />
        <add key="wndT2Class" value="GlassWndClass-GlassWindowClass-" />
        <add key="wndT2Caption" value="t2med" />
	<add key="wndT2SearchControlHeight" value="53" />
	<!-- neue Funktionen ab v1.2: bWriteLogFile = True schreibt TrayApp.exe.log mit Debuginformationen --> 
	<add key="bWriteLogfile" value="True" />
	<!-- neue Funktionen ab v1.2: bFeatureSearchGDT = True nutzt optional die GDT-Schnittstelle um die Patientenakte zu öffnen -->
	<add key="bFeatureSearchGDT" value="True" />
	<add key="sGDTFile" value="C:\GDT\TAPIT2MED.GDT" />
    </appSettings>
</configuration>
```

## Funktionalität ##
* Die TrayApp verbindet sich beim Start mit einem (beliebigen) konfigurierten TAPI-Provider als Audio-Gerät ("VoIP-Telefon") und hört auf Anrufsignalisierung (TE_CALLNOTIFICATION). Dabei muss der TAPI-Provider die Anrufsignalisierung (mindestens CallerIDNumber) unterstützen. 
* Sofern in der Konfigurationsdatei eine konforme CSV mit Rufnummern-Patientennummern-Zuordnung angegeben ist, wird diese zum Programmstart einmalig eingelesen.
* Das Programm legt sich in die TrayIcon-Leiste und ist bewusst minimalinvasiv ausgestattet, d.h. es sollten keine Meldungen & PopUps erscheinen, um den Praxisbetrieb so wenig wie möglich zu stören. (Lediglich beim Start des Programms erfolgt einmalig eine Meldung, sofern kein konfigurierter TAPI-Provider gefunden wird.)

**Erweiterungen mit Version 1.1**
* Ab der aktualisierten Version v1.1 sind zusätzlich zur Bedienung über das TrayIcon zwei *HotKey-Funktionen* ergänzt.
  Durch Drücken einer Funktionstaste (initial der sog. VK_LAUNCH_APP2-Taste = Sondertaste zum Starten des Calculators) wird der aktuell eingehende Anruf, bzw. der zuletzt eingegangene Anruf an das Praxisverwaltungssystem übergeben und die Patientenkartei aufgerufen.
  Die Hotkey-Taste kann angepasst werden (siehe Konfigurationsdatei). Für die Konfigurationswerte ist [Microsoft Dokumentation](https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes), [NirSoft KeyboardStateView](https://www.nirsoft.net/utils/keyboard_state_view.html) oder Google behilflich...
  
  ![](https://github.com/INT3hex/TAPICallerID/blob/master/doc/TrayApp_HotKey.png)

Die zweite Hotkey-Taste (aktuell *F9*) springt lediglich in das Suchfeld von T2med.

**Erweiterungen mit Version 1.2**
* Für Debugzwecke/Debugsuche kann die Option *bWriteLogFile = True* gesetzt werden, dadurch wird im Applikationsverzeichnis eine Logdatei (TrayApp.exe.log) erzeugt
* Um direkt eine Patientenakte programmatisch aufzurufen, kann TrayApp mit Parameter -id #<PatientenID> aufgerufen werden.
```bash
TrayApp.exe -id #123456    --> springt zur Patientenakte 123456
```
* Da die Suche über die T2Med-GUI nicht optimal ist, kann nun *alternativ* die Patientenakte über die GDT-Schnittstelle geöffnet werden (bFeatureSearchGDT = True). Dadurch wird in einem Austauschverzeichnis eine einfache GDT-Datei (über *sGDTFile* konfigurierbar) geschrieben, welche lediglich die PatientenID überträgt und die Patientenakte öffnet.
  
  Vielen Dank nochmals an Manuel Mühling für den Tipp zu GDT und stavismed für das GDT-Beispiel.

  Die Konfiguration in T2Med hierzu sieht wie folgt aus:
![](https://github.com/INT3hex/TAPICallerID/blob/master/doc/GDT_Konfig1.JPG)
![](https://github.com/INT3hex/TAPICallerID/blob/master/doc/GDT_Konfig2.JPG)

## Debuginformationen
Für Debugzwecke/Debugsuche kann ab v1.2 die Option *bWriteLogFile = True* gesetzt werden, dadurch wird im Applikationsverzeichnis eine Logdatei (TrayApp.exe.log) erzeugt.
Diesselben Debuginformationen (z.B. Fehlermeldungen, Debugausgaben, usw.) können zudem zur Laufzeit mittels [Sysinternals DebugView](https://learn.microsoft.com/de-de/sysinternals/downloads/debugview) angezeigt werden.

Zur Diagnose der TAPI-Kommunikation ist [TapiCaps der Fa. ESTOS](https://support.estos.de/de/procall-enterprise/analyse-fuer-tapi-leitungen-trace-erzeugen-mit-tapicaps-exe) ein wertvolles Werkzeug.

## Disclaimer
Für die Nutzung, Eignung, Fehlerfreiheit, Nützlichkeit etc. der Software wird keine Haftung übernommen. Die Anwendung und Anleitung wurde nach bestem Wissen erstellt. Der Quellcode kann eingesehen (und beliebig kopiert und verändert) werden. Das bereitgestellte Binärfile TrayApp.exe in v1.2 wurde bei Virustotal validiert. 
[Virustotal Analyse](https://www.virustotal.com/gui/file/6cef17d83bce5ebe3f45107a7e5417cc5841870a4419079de7099a3f00eb074a)
