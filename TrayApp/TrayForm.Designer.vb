<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TrayForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.CancelFormButton = New System.Windows.Forms.Button()
        Me.SearchAppButton = New System.Windows.Forms.Button()
        Me.sLastCaller = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'CancelFormButton
        '
        Me.CancelFormButton.Location = New System.Drawing.Point(29, 88)
        Me.CancelFormButton.Margin = New System.Windows.Forms.Padding(4)
        Me.CancelFormButton.Name = "CancelFormButton"
        Me.CancelFormButton.Size = New System.Drawing.Size(100, 28)
        Me.CancelFormButton.TabIndex = 1
        Me.CancelFormButton.Text = "Abbrechen"
        Me.CancelFormButton.UseVisualStyleBackColor = True
        '
        'SearchAppButton
        '
        Me.SearchAppButton.Location = New System.Drawing.Point(153, 88)
        Me.SearchAppButton.Margin = New System.Windows.Forms.Padding(4)
        Me.SearchAppButton.Name = "SearchAppButton"
        Me.SearchAppButton.Size = New System.Drawing.Size(157, 28)
        Me.SearchAppButton.TabIndex = 0
        Me.SearchAppButton.Text = "In T2med aufrufen"
        Me.SearchAppButton.UseVisualStyleBackColor = True
        '
        'sLastCaller
        '
        Me.sLastCaller.AutoSize = True
        Me.sLastCaller.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.sLastCaller.Location = New System.Drawing.Point(26, 30)
        Me.sLastCaller.Name = "sLastCaller"
        Me.sLastCaller.Size = New System.Drawing.Size(82, 18)
        Me.sLastCaller.TabIndex = 2
        Me.sLastCaller.Text = "sLastCaller"
        '
        'TrayForm
        '
        Me.AcceptButton = Me.SearchAppButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(356, 125)
        Me.Controls.Add(Me.sLastCaller)
        Me.Controls.Add(Me.SearchAppButton)
        Me.Controls.Add(Me.CancelFormButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "TrayForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "T2TAPI - letzter Anruf"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents SearchAppButton As System.Windows.Forms.Button
    Friend WithEvents CancelFormButton As System.Windows.Forms.Button
    Friend WithEvents sLastCaller As Label
End Class
