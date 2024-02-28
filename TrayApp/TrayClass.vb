Imports System.Reflection
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Windows.Automation ' Verweis - UIAutomationTypes & UIAutomationClient
Imports System.Threading

Public Class TrayClass
    Inherits ApplicationContext

#Region " Storage "

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



    ' Read from configuration
    Public wndT2Class As String = App._appConfig.GetProperty("wndT2Class", "GlassWndClass-GlassWindowClass-")
    Public wndT2Caption As String = App._appConfig.GetProperty("wndT2Caption", "t2med")
    'Public wndT2SearchControl As String = App._appConfig.GetProperty("wndT2SearchControl", "ControlType.Image")


#End Region

#Region " Constructor "

    Public Sub New()
        'Initialize the menus
        mnuDisplayForm = New ToolStripMenuItem("Letzte Anrufe")
        mnuSep1 = New ToolStripSeparator()
        mnuExit = New ToolStripMenuItem("Programmende")
        MainMenu = New ContextMenuStrip
        MainMenu.Items.AddRange(New ToolStripItem() {mnuDisplayForm, mnuSep1, mnuExit})

        'Initialize the tray
        Tray = New NotifyIcon
        Tray.Icon = My.Resources.Phone
        Tray.ContextMenuStrip = MainMenu
        Tray.Text = "T2MedTAPI"

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
        'TrayForm.lbCalls = App.globalListbox
        'Debug.WriteLine("TrayForm.lbCalls:" & TrayForm.lbCalls.Items.Count)
        'Debug.WriteLine("TrayForm.lbCalls_text:" & TrayForm.lbCalls.GetItemText(1))
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

        Dim iPos As Integer = InStr(iClickedMnu.Text, "#")
        If iPos > 0 Then

            Dim sPatientID As String = Right(iClickedMnu.Text, Len(iClickedMnu.Text) - iPos)
            DebugPrint("Call T2MedSearch with PatientID: " & sPatientID)
            'Debug.WriteLine(T2medSearch(iClickedMnu.Text))
            T2medSearch(sPatientID)
        End If

    End Sub

    Private Sub Tray_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles Tray.DoubleClick
        'TrayApp.App.globalListbox.Items.Add(DateTime.Now.ToString() & "Test")
        ShowDialog()
    End Sub

    Private Sub Tray_MouseDown(sender As Object, e As MouseEventArgs) Handles Tray.MouseDown
        TrayForm.lbCalls = TrayApp.App.globalListbox
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
        mi.Invoke(Tray, Nothing)
    End Sub

    Private Function GenMnuItem(ByRef sCall As String) As ToolStripMenuItem
        If InStr(sCall, "o") Then
            img = TrayApp.My.Resources.PhoneCall.ToBitmap
            'Dim ico As Icon = TrayApp.My.Resources.PhoneCall
            'img = FromIconToBitmap(ico)
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

    Private Function T2medSearch(ByVal sPatient As String) As Integer

        Dim hT2med As Long = FocusWindow(wndT2Caption, wndT2Class)
        'Handle auf T2Med Client
        If hT2med > 0 Then

            'Enumerate T2Med Window-Controls
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
                    Console.WriteLine("Control 'Suche' gefunden. Typ:" & controlElement.Current.ControlType.ProgrammaticName & " " & controlElement.Current.AutomationId)
                    Dim nextElement As AutomationElement = TreeWalker.RawViewWalker.GetNextSibling(controlElement)
                    'Console.WriteLine("  Next ist:" & parentElement.Current.Name)
                    Console.WriteLine("  NextControl (sollte Image sein) ist:" & nextElement.Current.ControlType.ProgrammaticName & " " & nextElement.Current.AutomationId)

                    If nextElement.Current.ControlType.ProgrammaticName = "ControlType.Image" Then
                        ' Send Search
                        Console.WriteLine("NextControl gefunden. Suche Patient.")
                        Dim valuePattern As ValuePattern = TryCast(controlElement.GetCurrentPattern(ValuePattern.Pattern), ValuePattern)
                        If valuePattern IsNot Nothing Then
                            ' Suchtext in das Textfeld einfügen
                            valuePattern.SetValue(sPatient)
                            Thread.Sleep(1000)
                            Console.WriteLine("   Suchtext wurde erfolgreich in das Textfeld eingefügt.")
                            wndElement = AutomationElement.FromHandle(hT2med)
                            Console.WriteLine("Enumeriere erneut die Controls.")
                            Dim controlCollection As AutomationElementCollection = wndElement.FindAll(TreeScope.Children, imageCondition)
                            Console.WriteLine("  Anzahl ImageControls:" & controlCollection.Count)
                            Dim next2Element As AutomationElement = controlCollection(1)
                            Console.WriteLine("  next2Element:" & controlCollection(1).Current.AutomationId)
                            If next2Element.Current.ControlType.ProgrammaticName = "ControlType.Image" Then
                                Console.WriteLine("Element vermutlich korrekt gefunden. OK!")
                                next2Element.SetFocus()
                                SendKeys.Send("~")
                            End If
                        Else
                            Console.WriteLine("Das Steuerelement unterstützt kein ValuePattern.")
                            Return 2 ' Control not found
                        End If
                    Else
                        Console.WriteLine("Leider nicht das richtige Steuerelement gefunden.")
                        Return 2 ' Control not found
                    End If
                    Return 0 ' OK
                End If
            End If
            Return 2 ' Control not found
        Else
            Debug.WriteLine("T2Med Applikation nicht gefunden.")
            Return 1 ' T2Med not found
        End If
    End Function

End Class
