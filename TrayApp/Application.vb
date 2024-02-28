﻿
Imports System.Reflection.Emit
Imports TrayApp.AppConfigClass


Public Module App

    Public WithEvents globalTAPI As namespace_tapi.TAPIClass
    Public globalListbox As New ListBox

    Public _appConfig As New AppConfig
    Public cTray As New TrayClass

    Public Sub Main()
        ' Init TAPI
        DebugPrint("Initialisiere TAPI.")
        Dim localTAPI As New namespace_tapi.TAPIClass
        Dim strTapiProvider As String = localTAPI.Initialize()
        DebugPrint("TAPI: " & strTapiProvider)

        If strTapiProvider <> "Initializing failed" Then
            globalTAPI = localTAPI
        Else
            MsgBox("TAPI Initialisierung fehlgeschlagen.")
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
        '       End Sub)

        If TrayApp.App.globalListbox.Items.Count > 2 Then TrayApp.App.globalListbox.Items.RemoveAt(0)
        If strCallerIDName = "" Then strCallerIDName = strCallerID
        TrayApp.App.globalListbox.Items.Add(DateTime.Now.ToString("HH:mm:ss") & " " & strCallerIDName & " #123457846")
        Debug.Print("TAPI: Eingehender Anruf von " & strCallerID & " " & strCallerIDName)
    End Sub

End Module
