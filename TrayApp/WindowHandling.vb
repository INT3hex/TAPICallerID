Imports System.Threading
Imports System.Windows
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
    Public bFeatureSearchPhonenumber As Boolean = App._appConfig.GetProperty("bFeatureSearchPhonenumber", True)
    Public bFeatureSearchGDT As Boolean = App._appConfig.GetProperty("bFeatureSearchGDT", False)
    Public wndT2Class As String = App._appConfig.GetProperty("wndT2Class", "GlassWndClass-GlassWindowClass-")
    Public wndT2Caption As String = App._appConfig.GetProperty("wndT2Caption", "t2med")
    Public wndT2SearchControlHeight As Integer = App._appConfig.GetProperty("wndT2SearchControlHeight", 53)
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
        'sSearch = " #123457846" 'just for debugging
        Dim iHash As Integer = InStr(sSearch, "#")
        Dim sPatientSearch As String

        If iHash > 0 Then
            sPatientSearch = sSearch.Substring(iHash)
            DebugPrint("WindowHandling: Aufruf von T2MedSearch mit PatientID: " & sPatientSearch)
            Return T2medSearch(sPatientSearch)
        End If
        DebugPrint("T2medSearchID: Patientennummer (#) nicht angegeben:" & sSearch)

        If bFeatureSearchPhonenumber Then
            iHash = InStr(sSearch, "~")
            If iHash > 0 Then
                sPatientSearch = sSearch.Substring(iHash)
                DebugPrint("WindowHandling: NewFeatureEnabled - Aufruf von T2MedSearch mit Telefonnummer: " & sPatientSearch)
                Return T2medSearch(sPatientSearch)
            End If
        End If
        Return 3 'no ID in searchstring
    End Function

    Public Function T2medSearch(ByVal sPatient As String) As Integer
        If bFeatureSearchGDT Then
            ' Search via GDT-Entry
            DebugPrint("T2medSearch: GDT")
            Return T2medSearchGDT(sPatient)

        Else
            ' Search via GUI
            DebugPrint("T2medSearch: GUI")
            Return T2medSearchGUI(sPatient)
        End If

    End Function

    Public Function T2medSearchGUI(ByVal sPatient As String) As Integer

        Dim hT2med As Long = FocusWindow(wndT2Caption, wndT2Class)
        'get handle for T2Med Client
        If hT2med > 0 Then

            'Enumerate T2Med window-controls
            Dim wndElement As AutomationElement = AutomationElement.FromHandle(hT2med)
            If wndElement IsNot Nothing Then

                Dim controlCondition As New PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit)

                ' Finde Such-Control
                Dim elementCollection As AutomationElementCollection = wndElement.FindAll(TreeScope.Descendants, controlCondition)
                Dim prevElement As AutomationElement
                Dim nextElement As AutomationElement
                Dim valPattern As ValuePattern

                ' Iteriere durch alle gefundenen Elemente
                For Each element As AutomationElement In elementCollection

                    prevElement = TreeWalker.RawViewWalker.GetPreviousSibling(element)
                    nextElement = TreeWalker.RawViewWalker.GetNextSibling(element)
                    valPattern = DirectCast(element.GetCurrentPattern(ValuePattern.Pattern), ValuePattern)
                    DebugPrint("T2MedSearch: Umgebene Controls zu " & element.Current.AutomationId & " prüfen!")

                    ' Logik ist: das vorausgehende Element ist 'nothing' und das nachfolgende Element ist 'Button' oder 'Image'
                    If nextElement IsNot Nothing Then
                        If (valPattern IsNot Nothing) And
                            (prevElement Is Nothing) And
                            (nextElement.Current.ControlType.ProgrammaticName = "ControlType.Button" Or nextElement.Current.ControlType.ProgrammaticName = "ControlType.Image") Then
                            DebugPrint("T2MedSearch: Suchfeld höchstwahrscheinlich gefunden: " & element.Current.AutomationId & " - Suchtext wird in das Suchfeld eingefügt!")
                            ' HotKey 2 (aka F9) pressed
                            If sPatient = "HOTKEY" Then element.SetFocus() : Return 0

                            valPattern.SetValue(sPatient)
                            Thread.Sleep(1000)
                            nextElement.SetFocus()
                            SendKeys.SendWait("~")
                            Return 0 ' OK
                        End If
                    End If

                Next
                DebugPrint("T2MedSearch: Oha! Leider nicht das richtige Steuerelement gefunden.")
                Return 2 ' Control not found
            Else
                DebugPrint("T2MedSearch: Oha! T2Med Applikation nicht gefunden.")
                Return 1 ' T2Med not found
            End If
            DebugPrint("T2MedSearch: Oha! T2Med Applikation nicht gefunden.")
            Return 1 ' T2Med not found
        End If
    End Function

End Module
