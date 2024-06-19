Imports System.Threading
Imports System.Windows.Automation

Module WindowHandling
    Public Declare Function SetForegroundWindow Lib "user32.dll" (ByVal hwnd As Integer) As Integer
    Public Declare Auto Function FindWindow Lib "user32.dll" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    Public Declare Function IsIconic Lib "user32.dll" (ByVal hwnd As Integer) As Boolean
    Public Declare Function ShowWindow Lib "user32.dll" (ByVal hwnd As Integer, ByVal nCmdShow As Integer) As Integer
    Public Declare Function GetClassName Lib "user32.dll" Alias "GetClassNameA" (ByVal hwnd As Integer, ByVal lpClassName As System.Text.StringBuilder, ByVal nMaxCount As Integer) As Integer

    Public Const SW_RESTORE As Integer = 9
    Public Const SW_SHOW As Integer = 5

    ' Read definitions from configuration
    Public wndT2Class As String = App._appConfig.GetProperty("wndT2Class", "GlassWndClass-GlassWindowClass-")
    Public wndT2Caption As String = App._appConfig.GetProperty("wndT2Caption", "t2med")
    'Public wndT2SearchControl As String = App._appConfig.GetProperty("wndT2SearchControl", "ControlType.Image")

    Function FocusWindow(ByVal strWindowCaption As String, ByVal strClassName As String) As Long
        Dim hWnd As Integer
        Dim rc As Integer

        'Find window
        DebugPrint("WindowHandling: FindWindow Caption: " & strWindowCaption)
        hWnd = FindWindow(vbNullString, strWindowCaption)

        If hWnd > 0 Then

            Dim strClassNameFound As New System.Text.StringBuilder(Chr(0), 100)
            rc = GetClassName(hWnd, strClassNameFound, 100)
            DebugPrint("WindowHandling: FoundWindow (wndT2Class): " & strClassNameFound.ToString)

            'check partial ClassName
            If InStr(strClassNameFound.ToString, strClassName) Then

                SetForegroundWindow(hWnd)
                If IsIconic(hWnd) Then  'Restore window if minimized
                    ShowWindow(hWnd, SW_RESTORE)
                Else
                    ShowWindow(hWnd, SW_SHOW)
                End If

                Return hWnd
            Else
                Return 0
            End If
        Else
            Return 0
        End If
    End Function

    Public Function T2medSearchID(ByVal sSearch As String) As Integer
        Dim iHash As Integer = InStr(sSearch, "#")
        If iHash > 0 Then
            Dim sPatientID As String = sSearch.Substring(iHash)
            DebugPrint("WindowHandling: Aufruf von T2MedSearch mit PatientID: " & sPatientID)
            Return T2medSearch(sPatientID)
        End If
        DebugPrint("T2medSearchID: Patientennummer (#) nicht angegeben:" & sSearch)
        Return 3 'no ID in searchstring
    End Function

    Public Function T2medSearch(ByVal sPatient As String) As Integer

        Dim hT2med As Long = FocusWindow(wndT2Caption, wndT2Class)
        'get handle for T2Med Client
        If hT2med > 0 Then

            'Enumerate T2Med window-controls
            Dim wndElement As AutomationElement = AutomationElement.FromHandle(hT2med)
            If wndElement IsNot Nothing Then

                ' Such-Control condition (fix: Edit & "Suche")
                Dim controlCondition As New AndCondition(
                    New PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit),
                    New PropertyCondition(ValuePattern.ValueProperty, "Suche"))

                ' Treffer-Control condition (fix: Image)
                Dim imageCondition As New PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Image)

                ' Finde Such-Control
                Dim controlElement As AutomationElement = wndElement.FindFirst(TreeScope.Descendants, controlCondition)

                If controlElement IsNot Nothing Then
                    DebugPrint("T2MedSearch: Control 'Suche' gefunden. Typ:" & controlElement.Current.ControlType.ProgrammaticName & " " & controlElement.Current.AutomationId)
                    Dim nextElement As AutomationElement = TreeWalker.RawViewWalker.GetNextSibling(controlElement)
                    'DebugPrint("T2MedSearch:  Next ist:" & parentElement.Current.Name)
                    DebugPrint("T2MedSearch:  NextControl (sollte Image sein) ist:" & nextElement.Current.ControlType.ProgrammaticName & " " & nextElement.Current.AutomationId)

                    If nextElement.Current.ControlType.ProgrammaticName = "ControlType.Image" Then
                        ' Send Search
                        DebugPrint("T2MedSearch: NextControl gefunden. Suche Patient.")
                        Dim valuePattern As ValuePattern = TryCast(controlElement.GetCurrentPattern(ValuePattern.Pattern), ValuePattern)
                        If valuePattern IsNot Nothing Then
                            ' Suchtext in das Textfeld einfügen
                            valuePattern.SetValue(sPatient)
                            Thread.Sleep(1000)
                            DebugPrint("T2MedSearch: Suchtext wurde erfolgreich in das Textfeld eingefügt.")
                            wndElement = AutomationElement.FromHandle(hT2med)
                            DebugPrint("T2MedSearch: Enumeriere erneut die Controls.")
                            Dim controlCollection As AutomationElementCollection = wndElement.FindAll(TreeScope.Children, imageCondition)
                            DebugPrint("T2MedSearch: Debug: Anzahl ImageControls:" & controlCollection.Count)
                            Dim next2Element As AutomationElement = controlCollection(1)
                            DebugPrint("T2MedSearch: Debug: next2Element:" & controlCollection(1).Current.AutomationId)
                            If next2Element.Current.ControlType.ProgrammaticName = "ControlType.Image" Then
                                DebugPrint("T2MedSearch: Element vermutlich korrekt gefunden. Send Enter|GO!")
                                next2Element.SetFocus()
                                SendKeys.Send("~")
                            End If
                        Else
                            DebugPrint("T2MedSearch: Oha! Das Steuerelement unterstützt kein ValuePattern.")
                            Return 2 ' Control not found
                        End If
                    Else
                        DebugPrint("T2MedSearch: Oha! Leider nicht das richtige Steuerelement gefunden.")
                        Return 2 ' Control not found
                    End If
                    Return 0 ' OK
                End If
            End If
            Return 2 ' Control not found
        Else
            DebugPrint("T2MedSearch: Oha! T2Med Applikation nicht gefunden.")
            Return 1 ' T2Med not found
        End If
    End Function

End Module
