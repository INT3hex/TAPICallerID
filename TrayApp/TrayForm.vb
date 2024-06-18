Public Class TrayForm

    Public Sub New()
        InitializeComponent()
        Icon = My.Resources.PhoneCall
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CancelFormButton.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub CloseAppButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles CloseAppButton.Click
        Me.DialogResult = Windows.Forms.DialogResult.Abort
        Me.Close()
    End Sub

    Private Sub PopupForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cTray.Tray.Icon = My.Resources.PhoneCall
        sLastCaller.Text = App.sLastCaller
    End Sub

End Class