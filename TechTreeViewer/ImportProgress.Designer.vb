<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ImportProgress
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
        Me.ImportProgressBar = New System.Windows.Forms.ProgressBar()
        Me.LabelProgressInfo = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'ImportProgressBar
        '
        Me.ImportProgressBar.Location = New System.Drawing.Point(0, 0)
        Me.ImportProgressBar.Name = "ImportProgressBar"
        Me.ImportProgressBar.Size = New System.Drawing.Size(328, 32)
        Me.ImportProgressBar.TabIndex = 0
        '
        'LabelProgressInfo
        '
        Me.LabelProgressInfo.AutoSize = True
        Me.LabelProgressInfo.Location = New System.Drawing.Point(0, 40)
        Me.LabelProgressInfo.Name = "LabelProgressInfo"
        Me.LabelProgressInfo.Size = New System.Drawing.Size(92, 13)
        Me.LabelProgressInfo.TabIndex = 1
        Me.LabelProgressInfo.Text = "LabelProgressInfo"
        '
        'ImportProgress
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(329, 66)
        Me.ControlBox = False
        Me.Controls.Add(Me.LabelProgressInfo)
        Me.Controls.Add(Me.ImportProgressBar)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "ImportProgress"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "ImportProgress"
        Me.UseWaitCursor = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ImportProgressBar As ProgressBar
    Friend WithEvents LabelProgressInfo As Label
End Class
