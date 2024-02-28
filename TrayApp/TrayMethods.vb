Friend Module TrayMethods

    Private PF As TrayForm

    Public Sub ExitApplication()
        'Perform any clean-up here
        'Then exit the application
        Application.Exit()
    End Sub

    Public Sub ShowDialog()
        If PF IsNot Nothing AndAlso Not PF.IsDisposed Then Exit Sub

        Dim CloseApp As Boolean = False

        PF = New TrayForm
        ' PF.lbCalls = App.globalListbox
        PF.ShowDialog()
        CloseApp = (PF.DialogResult = DialogResult.Abort)
        PF = Nothing

        If CloseApp Then Application.Exit()
    End Sub

End Module
