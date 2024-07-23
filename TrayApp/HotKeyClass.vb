Public Class HotKeyClass
    ' https://gist.github.com/kirsbo/3b01a1412311e7a1d565
#Region "Declarations - WinAPI, Hotkey constant and Modifier Enum"
    ''' <summary>
    ''' Declaration of winAPI function wrappers. The winAPI functions are used to register / unregister a hotkey
    ''' </summary>
    Private Declare Function RegisterHotKey Lib "user32" _
        (ByVal hwnd As IntPtr, ByVal id As Integer, ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer

    Private Declare Function UnregisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer) As Integer

    Public Const WM_HOTKEY As Integer = &H312

    Enum KeyModifier
        None = 0
        Alt = &H1
        Control = &H2
        Shift = &H4
        Winkey = &H8
    End Enum 'This enum is just to make it easier to call the registerHotKey function: The modifier integer codes are replaced by a friendly "Alt","Shift" etc.
#End Region


#Region "Hotkey registration, unregistration and handling"
    Public Shared Sub registerHotkey(ByRef sourceForm As Form, hknumber As Integer, ByVal triggerKey As Integer, ByVal modifier As KeyModifier)
        RegisterHotKey(sourceForm.Handle, hknumber, modifier, triggerKey)
    End Sub
    Public Shared Sub unregisterHotkeys(ByRef sourceForm As Form, hknumber As Integer)
        UnregisterHotKey(sourceForm.Handle, hknumber)  'Remember to call unregisterHotkeys() when closing your application.
    End Sub
    Public Shared Sub handleHotKeyEvent(ByVal hotkeyID As IntPtr)
        DebugPrint("HotKeyClass: Hotkey " & hotkeyID.ToString & " was pressed.... passing LastCallerID to T2med...")
        If hotkeyID = 1 Then T2medSearchID(App.sLastCaller) Else T2medSearch("HOTKEY")
    End Sub
#End Region
End Class
