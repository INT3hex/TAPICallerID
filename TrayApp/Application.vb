
Imports System.Globalization
Imports System.IO
Imports System.Reflection.Emit
Imports System.Text
Imports System.Text.RegularExpressions
Imports TrayApp.AppConfigClass


Public Module App

    Public WithEvents globalTAPI As namespace_tapi.TAPIClass
    Public globalListbox As New ListBox

    Public _appConfig As New AppConfig
    Public cTray As New TrayClass
    Public sLastCaller As String = ""
    Public aPhoneBook As New ArrayList
    Public oPBEntry As PHONEBOOKENTRY
    Public Structure PHONEBOOKENTRY
        Dim ID As String
        Dim Name As String
        Dim Numbers As String
    End Structure


    ' https://www.vb-paradise.de/index.php/Thread/117372-Telefonnummern-richtig-formatieren/
    ' CSV-Felder: Patientennummer, Vorname, Nachname, Telefon (Privat), Telefon (Mobil), Telefon (Arbeit), Telefon (Sonstiges)
    'Init CSV
    'Read from Configuration
    Dim sCsvFile As String = App._appConfig.GetProperty("sCsvFile", "")
    Public sCsvVorwahl As String = App._appConfig.GetProperty("sCsvVorwahl", "")
    Dim iCsvPosID As Integer = App._appConfig.GetProperty("iCsvPosPatientennummer", 0)
    Dim iCsvPosVorname As Integer = App._appConfig.GetProperty("iCsvPosVorname", 5)
    Dim iCsvPosName As Integer = App._appConfig.GetProperty("iCsvPosName", 6)
    Dim iCsvPosTel1 As Integer = App._appConfig.GetProperty("iCsvPosTel1", 28)
    Dim iCsvPosTel2 As Integer = App._appConfig.GetProperty("iCsvPosTel2", 29)
    Dim iCsvPosTel3 As Integer = App._appConfig.GetProperty("iCsvPosTel3", 30)
    Dim iCsvPosTel4 As Integer = App._appConfig.GetProperty("iCsvPosTel4", 31)
    Dim iHotKey As Integer = App._appConfig.GetProperty("iHotKey", 183)
    Dim iHotKeyModifier As Integer = App._appConfig.GetProperty("iHotKeyModifier", 0)

    Public Sub Main()
        ' Init TAPI
        DebugPrint("MAIN: TAPICallerID für T2med gestartet.")
        DebugPrint("MAIN: Initialisiere TAPI.")
        Dim localTAPI As New namespace_tapi.TAPIClass
        Dim sTapiID As String = localTAPI.Initialize()
        DebugPrint("TAPI: TAPI-ID: " & sTapiID)

        If Left(sTapiID, 31) <> "Initialisierung fehlgeschlagen:" Then
            globalTAPI = localTAPI
        Else
            'print error message
            MsgBox("TAPI " & sTapiID)
            globalTAPI = Nothing
            Application.Exit()
            End
        End If
        localTAPI = Nothing



        If sCsvFile <> "" Then
            DebugPrint("MAIN: CSV-Konfiguration gesetzt, lese Datei: " & sCsvFile)
            Using tfp = New Microsoft.VisualBasic.FileIO.TextFieldParser(sCsvFile, Encoding.Default)
                tfp.SetDelimiters(";")
                Dim fields = tfp.ReadFields
                DebugPrint("CSV: " & fields(iCsvPosID) & "," & fields(iCsvPosName) & "," & fields(iCsvPosVorname) & "," & fields(iCsvPosTel1) & "," & fields(iCsvPosTel2) & "," & fields(iCsvPosTel3) & "," & fields(iCsvPosTel4))
                While fields IsNot Nothing
                    oPBEntry.ID = fields(iCsvPosID)
                    oPBEntry.Name = fields(iCsvPosName) & ", " & fields(iCsvPosVorname)

                    'NormalizePhoneNumbers
                    'DebugPrint("Native    :" & fields(iCsvPosTel1) & " " & fields(iCsvPosTel2) & " " & fields(iCsvPosTel3) & " " & fields(iCsvPosTel4))
                    'DebugPrint("Normalized:" & NormalizePhoneNumber(fields(iCsvPosTel1)) & " " & NormalizePhoneNumber(fields(iCsvPosTel2)) & " " & NormalizePhoneNumber(fields(iCsvPosTel3)) & " " & NormalizePhoneNumber(fields(iCsvPosTel4)))
                    oPBEntry.Numbers = NormalizePhoneNumber(fields(iCsvPosTel1)) & " " & NormalizePhoneNumber(fields(iCsvPosTel2)) & " " & NormalizePhoneNumber(fields(iCsvPosTel3)) & " " & NormalizePhoneNumber(fields(iCsvPosTel4))

                    aPhoneBook.Add(oPBEntry)
                    fields = tfp.ReadFields
                End While
            End Using
        End If 'Csv


        'Turn visual styles back on
        Application.EnableVisualStyles()

        'Register global Hotkey
        ' z.B. asc("W"), HotKeyClass.KeyModifier.Alt = Alt+W
        '      183, 0 = VK_LAUNCH_APP2 (Calculator Key)
        ' see http://www.kbdedit.com/manual/low_level_vk_list.html or Google for virtual keys & modifier
        HotKeyClass.registerHotkey(MainForm, iHotKey, iHotKeyModifier)

        'Run the application using AppContext
        Application.Run(cTray)

    End Sub

    Public Sub DebugPrint(ByRef sDebugOutput As String)
        Debug.WriteLine(sDebugOutput)
    End Sub

    Public Function NormalizePhoneNumber(ByRef sNumber As String) As String
        sNumber = Regex.Replace(sNumber, "[^0-9+*]", "", RegexOptions.IgnoreCase)
        sNumber = sNumber.Replace("(", "")
        sNumber = sNumber.Replace(")", "")
        sNumber = sNumber.Replace(" ", "")
        Dim M As Match = Regex.Match(sNumber, "^\+([\S\s]*) ")
        Dim LandesVorwahl As String = M.Groups(1).Value
        sNumber = Regex.Replace(sNumber, "[^0-9+*]", "", RegexOptions.IgnoreCase)
        sNumber = Regex.Replace(sNumber, "^\+", "00", RegexOptions.IgnoreCase)
        sNumber = Regex.Replace(sNumber, LandesVorwahl & "\(.\)", LandesVorwahl, RegexOptions.IgnoreCase)
        If (Len(sNumber) > 1) And (Left(sNumber, 1) <> "0") Then sNumber = sCsvVorwahl & sNumber
        Return sNumber
    End Function

    Private Sub globalTAPI_IncomingCall(ByVal strCallerID As Object, ByVal strCallerIDName As Object) Handles globalTAPI.IncomingCall

        'Invoke(Sub()
        '           ListBox1.Items.Add(DateTime.Now.ToString() & strCallerIDName)
        '           ShowInactiveTopmost(Me)
        '    End Sub)

        DebugPrint("TAPI: IncomingCall")

        If Len(strCallerID) > 1 Then ' if CallerID is somehow available


            If TrayApp.App.globalListbox.Items.Count > 2 Then TrayApp.App.globalListbox.Items.RemoveAt(0)

            'following is just for debugging reasons
            If strCallerID = "**626" Then strCallerID = "0711620" : strCallerIDName = "~0711620"
            If strCallerIDName = "" Then strCallerIDName = strCallerID ' !!! SHOULD NEVER MATCH, strCallerIDName is always set

            ' check if CallerIDName has # pattern as PatientenID set by PBX, otherwise search in Phonebook
            If InStr(strCallerIDName, "#") = 0 Then
                DebugPrint("TAPI: CallerIDName enthält kein Kennzeichen für PatientenID")
                ' add info from CSV
                ' DebugPrint("Durchsuche Phonebookeinträge:" & aPhoneBook.Count)
                For Each item As PHONEBOOKENTRY In aPhoneBook
                    If InStr(item.Numbers, strCallerID) > 0 Then
                        DebugPrint("TAPI/CSV: Telefoneintrag für " & item.Name & "|" & item.ID & " gefunden.")
                        strCallerIDName = item.Name & " #" & item.ID
                        Exit For
                    End If
                Next
            End If

            DebugPrint("TAPI: Eingehender Anruf von CallerID:" & strCallerID & " CallerIDName:" & strCallerIDName)

            'add caller to tray
            TrayApp.App.globalListbox.Items.Add(DateTime.Now.ToString("HH:mm:ss") & " " & strCallerIDName)
            sLastCaller = strCallerIDName


            ' blink icon while receiving new call
            For i = 1 To 10
                Threading.Thread.Sleep(300)

                TrayForm.Icon = My.Resources.PhoneCall
                cTray.Tray.Icon = My.Resources.PhoneCall
                TrayForm.Refresh()

                Threading.Thread.Sleep(300)

                TrayForm.Icon = My.Resources.Phone
                cTray.Tray.Icon = My.Resources.Phone
                TrayForm.Refresh()
            Next i
        End If
    End Sub

End Module
