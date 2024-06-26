Imports System.Globalization

Public Class MainForm

    Public Sub New()
        InitializeComponent()
        Icon = My.Resources.Phone
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = HotKeyClass.WM_HOTKEY Then
            HotKeyClass.handleHotKeyEvent(m.WParam)
        End If
        MyBase.WndProc(m)
    End Sub 'System wide hotkey event handling
End Class