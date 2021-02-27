<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Templates
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
		Me.ListTemplates = New System.Windows.Forms.ListBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.TextName = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.CheckAppliesToNodes = New System.Windows.Forms.CheckBox()
		Me.CheckAppliesToConnections = New System.Windows.Forms.CheckBox()
		Me.CheckDefaultTemplate = New System.Windows.Forms.CheckBox()
		Me.BtnAdd = New System.Windows.Forms.Button()
		Me.BtnRemove = New System.Windows.Forms.Button()
		Me.BtnSave = New System.Windows.Forms.Button()
		Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
		Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.LoadFromFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.AddFromFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.SaveToFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.ColorDialog = New System.Windows.Forms.ColorDialog()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.TextSize = New System.Windows.Forms.TextBox()
		Me.ColorBox = New System.Windows.Forms.PictureBox()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.ComboDecoration = New System.Windows.Forms.ComboBox()
		CType(Me.GridProperties, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.MenuStrip1.SuspendLayout()
		CType(Me.ColorBox, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'GridProperties
		'
		Me.GridProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.GridProperties.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PropertyName, Me.PropertyDefaultValue, Me.PropertyShow})
		Me.GridProperties.Location = New System.Drawing.Point(224, 152)
		Me.GridProperties.Name = "GridProperties"
		Me.GridProperties.Size = New System.Drawing.Size(400, 280)
		Me.GridProperties.TabIndex = 0
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
		'ListTemplates
		'
		Me.ListTemplates.FormattingEnabled = True
		Me.ListTemplates.Location = New System.Drawing.Point(8, 32)
		Me.ListTemplates.Name = "ListTemplates"
		Me.ListTemplates.Size = New System.Drawing.Size(200, 407)
		Me.ListTemplates.TabIndex = 1
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(224, 136)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(54, 13)
		Me.Label1.TabIndex = 2
		Me.Label1.Text = "Properties"
		'
		'TextName
		'
		Me.TextName.Location = New System.Drawing.Point(288, 32)
		Me.TextName.Name = "TextName"
		Me.TextName.Size = New System.Drawing.Size(100, 20)
		Me.TextName.TabIndex = 3
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(224, 35)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(35, 13)
		Me.Label2.TabIndex = 4
		Me.Label2.Text = "Name"
		'
		'CheckAppliesToNodes
		'
		Me.CheckAppliesToNodes.Location = New System.Drawing.Point(400, 48)
		Me.CheckAppliesToNodes.Name = "CheckAppliesToNodes"
		Me.CheckAppliesToNodes.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.CheckAppliesToNodes.Size = New System.Drawing.Size(152, 17)
		Me.CheckAppliesToNodes.TabIndex = 7
		Me.CheckAppliesToNodes.Text = "Applies to Nodes"
		Me.CheckAppliesToNodes.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.CheckAppliesToNodes.UseVisualStyleBackColor = True
		'
		'CheckAppliesToConnections
		'
		Me.CheckAppliesToConnections.Location = New System.Drawing.Point(400, 64)
		Me.CheckAppliesToConnections.Name = "CheckAppliesToConnections"
		Me.CheckAppliesToConnections.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.CheckAppliesToConnections.Size = New System.Drawing.Size(152, 17)
		Me.CheckAppliesToConnections.TabIndex = 8
		Me.CheckAppliesToConnections.Text = "Applies to Connections"
		Me.CheckAppliesToConnections.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.CheckAppliesToConnections.UseVisualStyleBackColor = True
		'
		'CheckDefaultTemplate
		'
		Me.CheckDefaultTemplate.Location = New System.Drawing.Point(400, 32)
		Me.CheckDefaultTemplate.Name = "CheckDefaultTemplate"
		Me.CheckDefaultTemplate.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.CheckDefaultTemplate.Size = New System.Drawing.Size(152, 17)
		Me.CheckDefaultTemplate.TabIndex = 9
		Me.CheckDefaultTemplate.Text = "Default template"
		Me.CheckDefaultTemplate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.CheckDefaultTemplate.UseVisualStyleBackColor = True
		'
		'BtnAdd
		'
		Me.BtnAdd.Location = New System.Drawing.Point(8, 448)
		Me.BtnAdd.Name = "BtnAdd"
		Me.BtnAdd.Size = New System.Drawing.Size(75, 23)
		Me.BtnAdd.TabIndex = 10
		Me.BtnAdd.Text = "Add"
		Me.BtnAdd.UseVisualStyleBackColor = True
		'
		'BtnRemove
		'
		Me.BtnRemove.Location = New System.Drawing.Point(136, 448)
		Me.BtnRemove.Name = "BtnRemove"
		Me.BtnRemove.Size = New System.Drawing.Size(75, 23)
		Me.BtnRemove.TabIndex = 11
		Me.BtnRemove.Text = "Remove"
		Me.BtnRemove.UseVisualStyleBackColor = True
		'
		'BtnSave
		'
		Me.BtnSave.Location = New System.Drawing.Point(552, 448)
		Me.BtnSave.Name = "BtnSave"
		Me.BtnSave.Size = New System.Drawing.Size(75, 23)
		Me.BtnSave.TabIndex = 12
		Me.BtnSave.Text = "Apply"
		Me.BtnSave.UseVisualStyleBackColor = True
		'
		'MenuStrip1
		'
		Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
		Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
		Me.MenuStrip1.Name = "MenuStrip1"
		Me.MenuStrip1.Size = New System.Drawing.Size(631, 24)
		Me.MenuStrip1.TabIndex = 13
		Me.MenuStrip1.Text = "MenuStrip1"
		'
		'FileToolStripMenuItem
		'
		Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoadFromFileToolStripMenuItem, Me.AddFromFileToolStripMenuItem, Me.SaveToFileToolStripMenuItem})
		Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
		Me.FileToolStripMenuItem.Size = New System.Drawing.Size(35, 20)
		Me.FileToolStripMenuItem.Text = "File"
		'
		'LoadFromFileToolStripMenuItem
		'
		Me.LoadFromFileToolStripMenuItem.Name = "LoadFromFileToolStripMenuItem"
		Me.LoadFromFileToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
		Me.LoadFromFileToolStripMenuItem.Text = "Load from file..."
		'
		'AddFromFileToolStripMenuItem
		'
		Me.AddFromFileToolStripMenuItem.Name = "AddFromFileToolStripMenuItem"
		Me.AddFromFileToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
		Me.AddFromFileToolStripMenuItem.Text = "Add from file..."
		'
		'SaveToFileToolStripMenuItem
		'
		Me.SaveToFileToolStripMenuItem.Name = "SaveToFileToolStripMenuItem"
		Me.SaveToFileToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
		Me.SaveToFileToolStripMenuItem.Text = "Save to file..."
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(224, 59)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(27, 13)
		Me.Label3.TabIndex = 15
		Me.Label3.Text = "Size"
		'
		'TextSize
		'
		Me.TextSize.Location = New System.Drawing.Point(288, 56)
		Me.TextSize.Name = "TextSize"
		Me.TextSize.Size = New System.Drawing.Size(100, 20)
		Me.TextSize.TabIndex = 14
		'
		'ColorBox
		'
		Me.ColorBox.BackColor = System.Drawing.Color.Black
		Me.ColorBox.Location = New System.Drawing.Point(288, 80)
		Me.ColorBox.Name = "ColorBox"
		Me.ColorBox.Size = New System.Drawing.Size(100, 24)
		Me.ColorBox.TabIndex = 17
		Me.ColorBox.TabStop = False
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(224, 85)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(31, 13)
		Me.Label4.TabIndex = 16
		Me.Label4.Text = "Color"
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(224, 112)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(59, 13)
		Me.Label5.TabIndex = 21
		Me.Label5.Text = "Decoration"
		'
		'ComboDecoration
		'
		Me.ComboDecoration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.ComboDecoration.FormattingEnabled = True
		Me.ComboDecoration.Location = New System.Drawing.Point(288, 108)
		Me.ComboDecoration.Name = "ComboDecoration"
		Me.ComboDecoration.Size = New System.Drawing.Size(100, 21)
		Me.ComboDecoration.TabIndex = 20
		'
		'Templates
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(631, 474)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.ComboDecoration)
		Me.Controls.Add(Me.ColorBox)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.TextSize)
		Me.Controls.Add(Me.BtnSave)
		Me.Controls.Add(Me.BtnRemove)
		Me.Controls.Add(Me.BtnAdd)
		Me.Controls.Add(Me.CheckDefaultTemplate)
		Me.Controls.Add(Me.CheckAppliesToConnections)
		Me.Controls.Add(Me.CheckAppliesToNodes)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.TextName)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.ListTemplates)
		Me.Controls.Add(Me.GridProperties)
		Me.Controls.Add(Me.MenuStrip1)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.MainMenuStrip = Me.MenuStrip1
		Me.MaximizeBox = False
		Me.Name = "Templates"
		Me.ShowIcon = False
		Me.Text = "Templates"
		CType(Me.GridProperties, System.ComponentModel.ISupportInitialize).EndInit()
		Me.MenuStrip1.ResumeLayout(False)
		Me.MenuStrip1.PerformLayout()
		CType(Me.ColorBox, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Friend WithEvents GridProperties As DataGridView
    Friend WithEvents PropertyName As DataGridViewTextBoxColumn
    Friend WithEvents PropertyDefaultValue As DataGridViewTextBoxColumn
    Friend WithEvents PropertyShow As DataGridViewCheckBoxColumn
    Friend WithEvents ListTemplates As ListBox
    Friend WithEvents Label1 As Label
    Friend WithEvents TextName As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents CheckAppliesToNodes As CheckBox
    Friend WithEvents CheckAppliesToConnections As CheckBox
    Friend WithEvents CheckDefaultTemplate As CheckBox
    Friend WithEvents BtnAdd As Button
    Friend WithEvents BtnRemove As Button
    Friend WithEvents BtnSave As Button
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LoadFromFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddFromFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveToFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ColorDialog As ColorDialog
    Friend WithEvents Label3 As Label
    Friend WithEvents TextSize As TextBox
    Friend WithEvents ColorBox As PictureBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents ComboDecoration As ComboBox
End Class
