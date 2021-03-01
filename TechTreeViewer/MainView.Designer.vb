<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainView
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
        Me.components = New System.ComponentModel.Container()
        Me.MenuNewNode = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MenuNewNodeCreate = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuNewNodeCreateFromLastTemplate = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuNewNodeCreateFromTemplate = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuNewNodeTemplate = New System.Windows.Forms.ToolStripMenuItem()
        Me.ProgramMenu = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RecentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExampleFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.OpenFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ImportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenXComResearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ProgramMenuTemplates = New System.Windows.Forms.ToolStripMenuItem()
        Me.GraphToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SortToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ProgramMenuSortAttractors = New System.Windows.Forms.ToolStripMenuItem()
        Me.StopSortToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem6 = New System.Windows.Forms.ToolStripSeparator()
        Me.ClearHighlightToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditNode = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MenuEditNodeProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditNodeApplyTemplate = New System.Windows.Forms.ToolStripMenuItem()
        Me.Template1ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuEditNodeRemove = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuEditHighlightNeigborhood = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditHighlightNeigborhood1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditHighlightNeigborhood2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditHighlightNeigborhood3 = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditHighlightNeigborhood4 = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditHighlightNeigborhood5 = New System.Windows.Forms.ToolStripMenuItem()
        Me.SortTimer = New System.Windows.Forms.Timer(Me.components)
        Me.KeyScrollTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuNewNode.SuspendLayout()
        Me.ProgramMenu.SuspendLayout()
        Me.MenuEditNode.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuNewNode
        '
        Me.MenuNewNode.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuNewNodeCreate, Me.MenuNewNodeCreateFromLastTemplate, Me.MenuNewNodeCreateFromTemplate})
        Me.MenuNewNode.Name = "MenuNewNode"
        Me.MenuNewNode.Size = New System.Drawing.Size(218, 70)
        '
        'MenuNewNodeCreate
        '
        Me.MenuNewNodeCreate.Name = "MenuNewNodeCreate"
        Me.MenuNewNodeCreate.Size = New System.Drawing.Size(217, 22)
        Me.MenuNewNodeCreate.Text = "Create node"
        '
        'MenuNewNodeCreateFromLastTemplate
        '
        Me.MenuNewNodeCreateFromLastTemplate.Name = "MenuNewNodeCreateFromLastTemplate"
        Me.MenuNewNodeCreateFromLastTemplate.Size = New System.Drawing.Size(217, 22)
        Me.MenuNewNodeCreateFromLastTemplate.Text = "Create node from last template"
        '
        'MenuNewNodeCreateFromTemplate
        '
        Me.MenuNewNodeCreateFromTemplate.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuNewNodeTemplate})
        Me.MenuNewNodeCreateFromTemplate.Name = "MenuNewNodeCreateFromTemplate"
        Me.MenuNewNodeCreateFromTemplate.Size = New System.Drawing.Size(217, 22)
        Me.MenuNewNodeCreateFromTemplate.Text = "Create node from template"
        '
        'MenuNewNodeTemplate
        '
        Me.MenuNewNodeTemplate.Name = "MenuNewNodeTemplate"
        Me.MenuNewNodeTemplate.Size = New System.Drawing.Size(127, 22)
        Me.MenuNewNodeTemplate.Text = "Template 1"
        '
        'ProgramMenu
        '
        Me.ProgramMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.ProgramMenuTemplates, Me.GraphToolStripMenuItem})
        Me.ProgramMenu.Location = New System.Drawing.Point(0, 0)
        Me.ProgramMenu.Name = "ProgramMenu"
        Me.ProgramMenu.Size = New System.Drawing.Size(714, 24)
        Me.ProgramMenu.TabIndex = 1
        Me.ProgramMenu.Text = "ProgramMenu"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripMenuItem, Me.RecentToolStripMenuItem, Me.ToolStripMenuItem1, Me.OpenFileToolStripMenuItem, Me.SaveToolStripMenuItem, Me.SaveAsToolStripMenuItem, Me.ToolStripMenuItem3, Me.ImportToolStripMenuItem, Me.ToolStripMenuItem4, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(35, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'NewToolStripMenuItem
        '
        Me.NewToolStripMenuItem.Name = "NewToolStripMenuItem"
        Me.NewToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.NewToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.NewToolStripMenuItem.Text = "New"
        '
        'RecentToolStripMenuItem
        '
        Me.RecentToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExampleFileToolStripMenuItem})
        Me.RecentToolStripMenuItem.Name = "RecentToolStripMenuItem"
        Me.RecentToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.RecentToolStripMenuItem.Text = "Recent"
        '
        'ExampleFileToolStripMenuItem
        '
        Me.ExampleFileToolStripMenuItem.Name = "ExampleFileToolStripMenuItem"
        Me.ExampleFileToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.ExampleFileToolStripMenuItem.Text = "ExampleFile"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(161, 6)
        '
        'OpenFileToolStripMenuItem
        '
        Me.OpenFileToolStripMenuItem.Name = "OpenFileToolStripMenuItem"
        Me.OpenFileToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.OpenFileToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.OpenFileToolStripMenuItem.Text = "Open File..."
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.SaveToolStripMenuItem.Text = "Save"
        '
        'SaveAsToolStripMenuItem
        '
        Me.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem"
        Me.SaveAsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12
        Me.SaveAsToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.SaveAsToolStripMenuItem.Text = "Save as..."
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(161, 6)
        '
        'ImportToolStripMenuItem
        '
        Me.ImportToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenXComResearchToolStripMenuItem})
        Me.ImportToolStripMenuItem.Name = "ImportToolStripMenuItem"
        Me.ImportToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.ImportToolStripMenuItem.Text = "Import"
        '
        'OpenXComResearchToolStripMenuItem
        '
        Me.OpenXComResearchToolStripMenuItem.Name = "OpenXComResearchToolStripMenuItem"
        Me.OpenXComResearchToolStripMenuItem.Size = New System.Drawing.Size(172, 22)
        Me.OpenXComResearchToolStripMenuItem.Text = "OpenXCom research"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(161, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'ProgramMenuTemplates
        '
        Me.ProgramMenuTemplates.Name = "ProgramMenuTemplates"
        Me.ProgramMenuTemplates.Size = New System.Drawing.Size(68, 20)
        Me.ProgramMenuTemplates.Text = "Templates"
        '
        'GraphToolStripMenuItem
        '
        Me.GraphToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SortToolStripMenuItem, Me.StopSortToolStripMenuItem, Me.ToolStripMenuItem6, Me.ClearHighlightToolStripMenuItem, Me.SearchToolStripMenuItem})
        Me.GraphToolStripMenuItem.Name = "GraphToolStripMenuItem"
        Me.GraphToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
        Me.GraphToolStripMenuItem.Text = "Graph"
        '
        'SortToolStripMenuItem
        '
        Me.SortToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ProgramMenuSortAttractors})
        Me.SortToolStripMenuItem.Name = "SortToolStripMenuItem"
        Me.SortToolStripMenuItem.Size = New System.Drawing.Size(187, 22)
        Me.SortToolStripMenuItem.Text = "Sort"
        '
        'ProgramMenuSortAttractors
        '
        Me.ProgramMenuSortAttractors.Name = "ProgramMenuSortAttractors"
        Me.ProgramMenuSortAttractors.Size = New System.Drawing.Size(119, 22)
        Me.ProgramMenuSortAttractors.Text = "Attractors"
        '
        'StopSortToolStripMenuItem
        '
        Me.StopSortToolStripMenuItem.Name = "StopSortToolStripMenuItem"
        Me.StopSortToolStripMenuItem.Size = New System.Drawing.Size(187, 22)
        Me.StopSortToolStripMenuItem.Text = "Stop sort"
        '
        'ToolStripMenuItem6
        '
        Me.ToolStripMenuItem6.Name = "ToolStripMenuItem6"
        Me.ToolStripMenuItem6.Size = New System.Drawing.Size(184, 6)
        '
        'ClearHighlightToolStripMenuItem
        '
        Me.ClearHighlightToolStripMenuItem.Name = "ClearHighlightToolStripMenuItem"
        Me.ClearHighlightToolStripMenuItem.Size = New System.Drawing.Size(187, 22)
        Me.ClearHighlightToolStripMenuItem.Text = "Clear Highlights (Space)"
        '
        'MenuEditNode
        '
        Me.MenuEditNode.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuEditNodeProperties, Me.MenuEditNodeApplyTemplate, Me.ToolStripMenuItem2, Me.MenuEditNodeRemove, Me.ToolStripMenuItem5, Me.MenuEditHighlightNeigborhood})
        Me.MenuEditNode.Name = "MenuEditNode"
        Me.MenuEditNode.Size = New System.Drawing.Size(178, 104)
        '
        'MenuEditNodeProperties
        '
        Me.MenuEditNodeProperties.Name = "MenuEditNodeProperties"
        Me.MenuEditNodeProperties.Size = New System.Drawing.Size(177, 22)
        Me.MenuEditNodeProperties.Text = "Properties"
        '
        'MenuEditNodeApplyTemplate
        '
        Me.MenuEditNodeApplyTemplate.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Template1ToolStripMenuItem})
        Me.MenuEditNodeApplyTemplate.Name = "MenuEditNodeApplyTemplate"
        Me.MenuEditNodeApplyTemplate.Size = New System.Drawing.Size(177, 22)
        Me.MenuEditNodeApplyTemplate.Text = "Apply template"
        '
        'Template1ToolStripMenuItem
        '
        Me.Template1ToolStripMenuItem.Name = "Template1ToolStripMenuItem"
        Me.Template1ToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.Template1ToolStripMenuItem.Text = "Template 1"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(174, 6)
        '
        'MenuEditNodeRemove
        '
        Me.MenuEditNodeRemove.Name = "MenuEditNodeRemove"
        Me.MenuEditNodeRemove.Size = New System.Drawing.Size(177, 22)
        Me.MenuEditNodeRemove.Text = "Remove"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(174, 6)
        '
        'MenuEditHighlightNeigborhood
        '
        Me.MenuEditHighlightNeigborhood.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuEditHighlightNeigborhood1, Me.MenuEditHighlightNeigborhood2, Me.MenuEditHighlightNeigborhood3, Me.MenuEditHighlightNeigborhood4, Me.MenuEditHighlightNeigborhood5})
        Me.MenuEditHighlightNeigborhood.Name = "MenuEditHighlightNeigborhood"
        Me.MenuEditHighlightNeigborhood.Size = New System.Drawing.Size(177, 22)
        Me.MenuEditHighlightNeigborhood.Text = "Highlight neigborhood"
        '
        'MenuEditHighlightNeigborhood1
        '
        Me.MenuEditHighlightNeigborhood1.Name = "MenuEditHighlightNeigborhood1"
        Me.MenuEditHighlightNeigborhood1.Size = New System.Drawing.Size(135, 22)
        Me.MenuEditHighlightNeigborhood1.Text = "1 (Shift click)"
        '
        'MenuEditHighlightNeigborhood2
        '
        Me.MenuEditHighlightNeigborhood2.Name = "MenuEditHighlightNeigborhood2"
        Me.MenuEditHighlightNeigborhood2.Size = New System.Drawing.Size(135, 22)
        Me.MenuEditHighlightNeigborhood2.Text = "2"
        '
        'MenuEditHighlightNeigborhood3
        '
        Me.MenuEditHighlightNeigborhood3.Name = "MenuEditHighlightNeigborhood3"
        Me.MenuEditHighlightNeigborhood3.Size = New System.Drawing.Size(135, 22)
        Me.MenuEditHighlightNeigborhood3.Text = "3"
        '
        'MenuEditHighlightNeigborhood4
        '
        Me.MenuEditHighlightNeigborhood4.Name = "MenuEditHighlightNeigborhood4"
        Me.MenuEditHighlightNeigborhood4.Size = New System.Drawing.Size(135, 22)
        Me.MenuEditHighlightNeigborhood4.Text = "4"
        '
        'MenuEditHighlightNeigborhood5
        '
        Me.MenuEditHighlightNeigborhood5.Name = "MenuEditHighlightNeigborhood5"
        Me.MenuEditHighlightNeigborhood5.Size = New System.Drawing.Size(135, 22)
        Me.MenuEditHighlightNeigborhood5.Text = "5"
        '
        'SortTimer
        '
        Me.SortTimer.Interval = 20
        '
        'KeyScrollTimer
        '
        Me.KeyScrollTimer.Enabled = True
        Me.KeyScrollTimer.Interval = 10
        '
        'SearchToolStripMenuItem
        '
        Me.SearchToolStripMenuItem.Name = "SearchToolStripMenuItem"
        Me.SearchToolStripMenuItem.Size = New System.Drawing.Size(187, 22)
        Me.SearchToolStripMenuItem.Text = "Search (F3)"
        '
        'MainView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(714, 613)
        Me.Controls.Add(Me.ProgramMenu)
        Me.DoubleBuffered = True
        Me.MainMenuStrip = Me.ProgramMenu
        Me.Name = "MainView"
        Me.Text = "Graph"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.MenuNewNode.ResumeLayout(False)
        Me.ProgramMenu.ResumeLayout(False)
        Me.ProgramMenu.PerformLayout()
        Me.MenuEditNode.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuNewNode As ContextMenuStrip
    Friend WithEvents ProgramMenu As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents OpenFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveAsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MenuNewNodeCreate As ToolStripMenuItem
    Friend WithEvents MenuNewNodeCreateFromTemplate As ToolStripMenuItem
    Friend WithEvents MenuNewNodeTemplate As ToolStripMenuItem
    Friend WithEvents MenuNewNodeCreateFromLastTemplate As ToolStripMenuItem
    Friend WithEvents MenuEditNode As ContextMenuStrip
    Friend WithEvents MenuEditNodeProperties As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents MenuEditNodeRemove As ToolStripMenuItem
    Friend WithEvents ProgramMenuTemplates As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GraphToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SortToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ProgramMenuSortAttractors As ToolStripMenuItem
    Friend WithEvents SortTimer As Timer
    Friend WithEvents StopSortToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MenuEditNodeApplyTemplate As ToolStripMenuItem
    Friend WithEvents Template1ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ImportToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenXComResearchToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
    Friend WithEvents RecentToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExampleFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents KeyScrollTimer As Timer
    Friend WithEvents ToolStripMenuItem5 As ToolStripSeparator
    Friend WithEvents MenuEditHighlightNeigborhood As ToolStripMenuItem
    Friend WithEvents MenuEditHighlightNeigborhood1 As ToolStripMenuItem
    Friend WithEvents MenuEditHighlightNeigborhood2 As ToolStripMenuItem
    Friend WithEvents MenuEditHighlightNeigborhood3 As ToolStripMenuItem
    Friend WithEvents MenuEditHighlightNeigborhood4 As ToolStripMenuItem
    Friend WithEvents MenuEditHighlightNeigborhood5 As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem6 As ToolStripSeparator
    Friend WithEvents ClearHighlightToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SearchToolStripMenuItem As ToolStripMenuItem
End Class
