Imports System.IO
Imports TAPI3Lib

Module GDTHandling
    ' configuration definitions
    ' bFeatureSearchGDT in WindowHandling.vb defined
    Public sGDTFile As String = App._appConfig.GetProperty("sGDTFile", "C:\GDT\T2MDTAPI.GDT")

    Public sGDTData As String

    Public Function T2medSearchGDT(ByVal sPatient As String) As Integer
        ' create GDT-File with PatientenID (Feldkennung 3000)
        addGDT(sGDTData, "8000", "6301")        ' Satzart Stammdaten übermitteln
        addGDT(sGDTData, "8100", "?????")       ' Dateilänge initialisieren
        addGDT(sGDTData, "8315", "T2MED_PX")    ' GDT-ID Empfänger
        addGDT(sGDTData, "8316", "TAPITray")    ' GDT-ID Empfänger
        addGDT(sGDTData, "9218", "02.10")       ' GDT Version
        addGDT(sGDTData, "3000", sPatient)      ' PatientID
        addGDT(sGDTData, "8100", "")            ' Dateilänge berechnen/patchen

        ' MsgBox(sGDTData)
        DebugPrint("T2medSearch: Erzeuge GDT " & sGDTFile & vbCrLf & sGDTData)

        Try
            File.WriteAllText(sGDTFile, sGDTData)
        Catch ex As Exception
            DebugPrint("t2medSearch: got Execption writing GDT-File: " & ex.Message)
        End Try

    End Function

    Private Sub addGDT(ByRef sGDTDataSet As String, ByVal sSatzart As String, ByVal sContent As String)

        Select Case sSatzart
            Case "8000"
                ' Initialize GDTDataSet
                sGDTDataSet = ""
            Case "8100"
                If sContent = "" Then
                    ' Calculate GDTSize
                    sGDTDataSet = sGDTDataSet.Replace("?????", String.Format("{0:00000}", Len(sGDTDataSet)))
                    Return
                Else
                    ' Initialize GDTSize
                    sContent = "?????"
                End If
        End Select

        ' BDT/GDT-Dataset nnn####xxxxxxx<CR><LF> (nnn=Size, ####=Satzart, xxxxxxx=Content)
        sGDTDataSet = sGDTDataSet & String.Format("{0:000}", Len(sContent) + 3 + 4 + 2) & sSatzart & sContent & vbCrLf

    End Sub

End Module
