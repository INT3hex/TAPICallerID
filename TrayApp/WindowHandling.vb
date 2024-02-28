Module WindowHandling
    Public Declare Function SetForegroundWindow Lib "user32.dll" (ByVal hwnd As Integer) As Integer
    Public Declare Auto Function FindWindow Lib "user32.dll" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    Public Declare Function IsIconic Lib "user32.dll" (ByVal hwnd As Integer) As Boolean
    Public Declare Function ShowWindow Lib "user32.dll" (ByVal hwnd As Integer, ByVal nCmdShow As Integer) As Integer
    Public Declare Function GetClassName Lib "user32.dll" Alias "GetClassNameA" (ByVal hwnd As Integer, ByVal lpClassName As System.Text.StringBuilder, ByVal nMaxCount As Integer) As Integer

    Public Const SW_RESTORE As Integer = 9
    Public Const SW_SHOW As Integer = 5


    Function FocusWindow(ByVal strWindowCaption As String, ByVal strClassName As String) As Long
        Dim hWnd As Integer
        Dim rc As Integer

        DebugPrint("FindWindow Captiom: " & strWindowCaption)
        hWnd = FindWindow(vbNullString, strWindowCaption)

        If hWnd > 0 Then

            Dim strClassNameFound As New System.Text.StringBuilder(Chr(0), 100)
            rc = GetClassName(hWnd, strClassNameFound, 100)
            DebugPrint("FoundWindow (wndT2Class): " & strClassNameFound.ToString)

            'check partial ClassName
            If InStr(strClassNameFound.ToString, strClassName) Then

                SetForegroundWindow(hWnd)
                If IsIconic(hWnd) Then  'Restore if minimized
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

End Module
