<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditProperties
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
		Me.GridProperties = New System.Windows.Forms.DataGridView()
		Me.PropertyName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.PropertyDefaultValue = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.PropertyShow = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.TextSize = New System.Windows.Forms.TextBox()
		Me.BtnSave = New System.Windows.Forms.Button()
		Me.BtnCancel = New System.Windows.Forms.Button()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.ColorDialog = New System.Windows.Forms.ColorDialog()
		Me.ColorBox = New System.Windows.Forms.PictureBox()
		Me.ComboDecoration = New System.Windows.Forms.ComboBox()
		Me.Label3 = New System.Windows.Forms.Label()
		CType(Me.GridProperties, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.ColorBox, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'GridProperties
		'
		Me.GridProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.GridProperties.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PropertyName, Me.PropertyDefaultValue, Me.PropertyShow})
		Me.GridProperties.Location = New System.Drawing.Point(0, 0)
		Me.GridProperties.Name = "GridProperties"
		Me.GridProperties.Size = New System.Drawing.Size(400, 280)
		Me.GridProperties.TabIndex = 1
		'
		'PropertyName
		'
		Me.PropertyName.HeaderText = "Name"
		Me.PropertyName.Name = "PropertyName"
		'
		'PropertyDefaultValue
		'
		Me.PropertyDefaultValue.HeaderText = "Default value"
		Me.PropertyDefaultValue.Name = "PropertyDefaultValue"
		'
		'PropertyShow
		'
		Me.PropertyShow.HeaderText = "Display"
		Me.PropertyShow.Name = "PropertyShow"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(408, 3)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(27, 13)
		Me.Label2.TabIndex = 6
		Me.Label2.Text = "Size"
		'
		'TextSize
		'
		Me.TextSize.Location = New System.Drawing.Point(472, 0)
		Me.TextSize.Name = "TextSize"
		Me.TextSize.Size = New System.Drawing.Size(100, 20)
		Me.TextSize.TabIndex = 5
		'
		'BtnSave
		'
		Me.BtnSave.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.BtnSave.Location = New System.Drawing.Point(512, 256)
		Me.BtnSave.Name = "BtnSave"
		Me.BtnSave.Size = New System.Drawing.Size(75, 23)
		Me.BtnSave.TabIndex = 7
		Me.BtnSave.Text = "Ok"
		Me.BtnSave.UseVisualStyleBackColor = True
		'
		'BtnCancel
		'
		Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.BtnCancel.Location = New System.Drawing.Point(408, 256)
		Me.BtnCancel.Name = "BtnCancel"
		Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
		Me.BtnCancel.TabIndex = 8
		Me.BtnCancel.Text = "Cancel"
		Me.BtnCancel.UseVisualStyleBackColor = True
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(408, 32)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(31, 13)
		Me.Label1.TabIndex = 9
		Me.Label1.Text = "Color"
		'
		'ColorDialog
		'
		Me.ColorDialog.SolidColorOnly = True
		'
		'ColorBox
		'
		Me.ColorBox.BackColor = System.Drawing.Color.Black
		Me.ColorBox.Location = New System.Drawing.Point(472, 24)
		Me.ColorBox.Name = "ColorBox"
		Me.ColorBox.Size = New System.Drawing.Size(100, 24)
		Me.ColorBox.TabIndex = 10
		Me.ColorBox.TabStop = False
		'
		'ComboDecoration
		'
		Me.ComboDecoration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.ComboDecoration.FormattingEnabled = True
		Me.ComboDecoration.Location = New System.Drawing.Point(472, 55)
		Me.ComboDecoration.Name = "ComboDecoration"
		Me.ComboDecoration.Size = New System.Drawing.Size(100, 21)
		Me.ComboDecoration.TabIndex = 11
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(408, 63)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(59, 13)
		Me.Label3.TabIndex = 12
		Me.Label3.Text = "Decoration"
		'
		'EditProperties
		'
		Me.AcceptButton = Me.BtnSave
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.BtnCancel
		Me.ClientSize = New System.Drawing.Size(591, 284)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.ComboDecoration)
		Me.Controls.Add(Me.ColorBox)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.BtnCancel)
		Me.Controls.Add(Me.BtnSave)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.TextSize)
		Me.Controls.Add(Me.GridProperties)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.KeyPreview = True
		Me.MaximizeBox = False
		Me.Name = "EditProperties"
		Me.ShowIcon = False
		Me.Text = "EditProperties"
		CType(Me.GridProperties, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.ColorBox, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Friend WithEvents GridProperties As DataGridView
    Friend WithEvents PropertyName As DataGridViewTextBoxColumn
    Friend WithEvents PropertyDefaultValue As DataGridViewTextBoxColumn
    Friend WithEvents PropertyShow As DataGridViewCheckBoxColumn
    Friend WithEvents Label2 As Label
    Friend WithEvents TextSize As TextBox
    Friend WithEvents BtnSave As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents ColorDialog As ColorDialog
    Friend WithEvents ColorBox As PictureBox
    Friend WithEvents ComboDecoration As ComboBox
    Friend WithEvents Label3 As Label
End Class
