Imports System.Reflection
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Windows.Automation ' Verweis - UIAutomationTypes & UIAutomationClient
Imports System.Threading

Public Class TrayClass
    Inherits ApplicationContext

#Region " Variables "

    Public WithEvents Tray As NotifyIcon
    Private WithEvents MainMenu As ContextMenuStrip
    Public WithEvents CallMenu As ContextMenuStrip
    Private WithEvents mnuDisplayForm As ToolStripMenuItem
    Private WithEvents mnuSep1 As ToolStripSeparator
    Private WithEvents mnuExit As ToolStripMenuItem
    Private WithEvents mnuNumber1 As ToolStripMenuItem
    Private WithEvents mnuNumber2 As ToolStripMenuItem
    Private WithEvents mnuNumber3 As ToolStripMenuItem
    Private WithEvents img As Image
    Private WithEvents mnu As ToolStripMenuItem
    Public bTrayPrevent As Boolean

    ' Read definitions from configuration
    Public bDirectSearchOnDblClick As Boolean = App._appConfig.GetProperty("bDirectSearchOnDblClick", True)


#End Region

#Region " Constructor "

    Public Sub New()
        'Initialize the menus
        mnuDisplayForm = New ToolStripMenuItem("Letzter Anruf")
        mnuSep1 = New ToolStripSeparator()
        mnuExit = New ToolStripMenuItem("Programmende")
        MainMenu = New ContextMenuStrip
        MainMenu.Items.AddRange(New ToolStripItem() {mnuDisplayForm, mnuSep1, mnuExit})

        'Initialize the tray
        Tray = New NotifyIcon
        Tray.Icon = My.Resources.Phone
        Tray.ContextMenuStrip = MainMenu
        Tray.Text = "T2TAPI"

        'Display
        Tray.Visible = True

        TrayApp.App.globalListbox.Items.Add("")
        TrayApp.App.globalListbox.Items.Add("")
        TrayApp.App.globalListbox.Items.Add("")

    End Sub

#End Region

#Region " Event handlers "

    Private Sub AppContext_ThreadExit(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles Me.ThreadExit
        'Guarantees that the icon will not linger.
        Tray.Visible = False
    End Sub

    Private Sub mnuDisplayForm_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles mnuDisplayForm.Click
        ShowDialog()
    End Sub

    Private Sub mnuExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles mnuExit.Click
        If Not IsNothing(globalTAPI) Then globalTAPI.ShutDown()
        ExitApplication()
    End Sub

    Private Sub mnuNumber_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles mnuNumber1.Click, mnuNumber2.Click, mnuNumber3.Click
        Dim iClickedMnu As ToolStripMenuItem
        iClickedMnu = CType(sender, ToolStripMenuItem)

        T2medSearchID(iClickedMnu.Text)

    End Sub

    Private Sub Tray_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles Tray.DoubleClick
        DebugPrint("TrayClass: Tray_DoubleClick-Event arrived")
        bTrayPrevent = True
        'TrayApp.App.globalListbox.Items.Add(DateTime.Now.ToString() & "Test")
        If bDirectSearchOnDblClick = False Then
            ShowDialog()
        Else
            T2medSearchID(App.sLastCaller)
        End If
        bTrayPrevent = True
    End Sub

    Private Sub Tray_MouseDown(sender As Object, e As MouseEventArgs) Handles Tray.MouseDown
        'TrayForm.lbCalls = TrayApp.App.globalListbox
        DebugPrint("TrayClass: Tray_MouseDown: " & e.Button)
        If e.Button = MouseButtons.Left Then
            'Build new Caller Menu
            'For i = 0 To TrayApp.App.globalListbox.Items.Count
            mnuNumber1 = GenMnuItem(TrayApp.App.globalListbox.Items.Item(0).ToString)
            mnuNumber2 = GenMnuItem(TrayApp.App.globalListbox.Items.Item(1).ToString)
            mnuNumber3 = GenMnuItem(TrayApp.App.globalListbox.Items.Item(2).ToString)


            CallMenu = New ContextMenuStrip
            CallMenu.Items.AddRange(New ToolStripItem() {mnuNumber3, mnuNumber2, mnuNumber1})
            Tray.ContextMenuStrip = CallMenu
        Else
            Tray.ContextMenuStrip = MainMenu
        End If

        Dim mi = GetType(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.NonPublic Or BindingFlags.Instance)
        If bTrayPrevent <> True Then
            mi.Invoke(Tray, Nothing)
        End If
        bTrayPrevent = False
    End Sub

    Private Function GenMnuItem(ByRef sCall As String) As ToolStripMenuItem
        'DebugPrint("GenMenuItem:" & sCall)
        If InStr(sCall, "#") Then
            img = TrayApp.My.Resources.PhoneCall.ToBitmap
        Else
            img = TrayApp.My.Resources.PhoneGrey.ToBitmap
        End If
        mnu = New ToolStripMenuItem(sCall, img)
        Return mnu
    End Function

    Public Function FromIconToBitmap(ByVal icon As Icon) As Bitmap
        Dim bmp As New Bitmap(128, 128) 'Bitmap(icon.Width, icon.Height)
        Using gp As Graphics = Graphics.FromImage(bmp)
            gp.Clear(Color.Transparent)
            gp.DrawIcon(icon, New Rectangle(0, 0, icon.Width, icon.Height))
        End Using
        Return bmp
    End Function
#End Region



End Class
