
Imports System.Reflection.Emit
Imports TrayApp.AppConfigClass


Public Module App

    Public WithEvents globalTAPI As namespace_tapi.TAPIClass
    Public globalListbox As New ListBox

    Public _appConfig As New AppConfig
    Public cTray As New TrayClass
    Public sLastCaller As String = ""


    Public Sub Main()
        ' Init TAPI
        DebugPrint("Initialisiere TAPI.")
        Dim localTAPI As New namespace_tapi.TAPIClass
        Dim sTapiID As String = localTAPI.Initialize()
        DebugPrint("TAPI-ID: " & sTapiID)

        If Left(sTapiID, 31) <> "Initialisierung fehlgeschlagen:" Then
            globalTAPI = localTAPI
        Else
            'print error message
            MsgBox("TAPI " & sTapiID)
            globalTAPI = Nothing
        End If
        localTAPI = Nothing
        'Turn visual styles back on
        Application.EnableVisualStyles()

        'Run the application using AppContext
        Application.Run(cTray)

    End Sub

    Public Sub DebugPrint(ByRef sDebugOutput As String)
        Debug.WriteLine(sDebugOutput)
    End Sub


    Private Sub globalTAPI_IncommingCall(ByVal strCallerID As Object, ByVal strCallerIDName As Object) Handles globalTAPI.IncommingCall

        'Invoke(Sub()
        '           ListBox1.Items.Add(DateTime.Now.ToString() & strCallerIDName)
        '           ShowInactiveTopmost(Me)
        '    End Sub)

        If TrayApp.App.globalListbox.Items.Count > 2 Then TrayApp.App.globalListbox.Items.RemoveAt(0)

        If strCallerIDName = "" Then strCallerIDName = strCallerID

        If InStr(strCallerIDName, "#") = 0 Then
            Debug.Print("TAPI: CallerIDName enthält kein Kennzeichen für PatientenID")
            ' CSV auslesen
        End If

        TrayApp.App.globalListbox.Items.Add(DateTime.Now.ToString("HH:mm:ss") & " " & strCallerIDName & " #123457846")
        sLastCaller = strCallerIDName

        Debug.Print("TAPI: Eingehender Anruf von " & strCallerID & " " & strCallerIDName)

        ' blink icon
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
    End Sub

End Module
