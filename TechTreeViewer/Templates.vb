Public Class Templates
    Private Sub FillListTemplates()
        Dim PrevItem = ListTemplates.SelectedItem
        ListTemplates.Items.Clear()
        For Each Template In Hl.TemplateStorage
            ListTemplates.Items.Add(Template)
        Next
        ListTemplates.SelectedItem = PrevItem
    End Sub
    Private Sub Templates_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Call FillListTemplates()
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles BtnAdd.Click
        Hl.TemplateStorage.Add()
        Call MainView.InvalidateFile()
        Call FillListTemplates()
    End Sub

    Private Sub BtnRemove_Click(sender As Object, e As EventArgs) Handles BtnRemove.Click
        If ListTemplates.SelectedIndex = -1 Then Exit Sub
        Call MainView.InvalidateFile()
        Hl.TemplateStorage.Remove(ListTemplates.SelectedItem)
        Call FillListTemplates()
    End Sub

    Private Sub ListTemplates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListTemplates.SelectedIndexChanged
        If ListTemplates.SelectedIndex = -1 Then Exit Sub
        Dim Template As CPropertyTemplate
        Template = ListTemplates.SelectedItem
        TextName.Text = Template.Name
        CheckAppliesToNodes.Checked = Template.AppliesToNodes
        CheckAppliesToConnections.Checked = Template.AppliesToConnections
        CheckDefaultTemplate.Checked = Template.DefaultTemplate
        TextSize.Text = Trim(Str(Template.Size))
        ColorBox.BackColor = Template.Color
        ComboDecoration.SelectedIndex = Template.Decoration

        GridProperties.Rows.Clear()
        For Each TemplateProperty In Template.Properties
            Dim Index = GridProperties.Rows.Add()
            GridProperties.Rows.Item(Index).Cells.Item(0).Value = TemplateProperty.Name
            GridProperties.Rows.Item(Index).Cells.Item(1).Value = TemplateProperty.DefaultValue
            GridProperties.Rows.Item(Index).Cells.Item(2).Value = TemplateProperty.Display
        Next
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        If ListTemplates.SelectedIndex = -1 Then Exit Sub
        Dim Template As CPropertyTemplate
        Template = ListTemplates.SelectedItem
        Template.Name = TextName.Text
        Template.AppliesToConnections = CheckAppliesToConnections.Checked
        Template.AppliesToNodes = CheckAppliesToNodes.Checked
        Template.DefaultTemplate = CheckDefaultTemplate.Checked
        Template.Size = Val(TextSize.Text)
        Template.Color = ColorBox.BackColor
        Template.Decoration = ComboDecoration.SelectedIndex
        Template.Properties.Clear()
        For Each Row As DataGridViewRow In GridProperties.Rows
            If Row.Cells.Item(0).Value Is Nothing Then Continue For
            Dim NewProperty As CPropertyTemplate.SProperty
            NewProperty.Name = Row.Cells.Item(0).Value
            NewProperty.DefaultValue = Row.Cells.Item(1).Value
            NewProperty.Display = Row.Cells.Item(2).Value
            Template.Properties.Add(NewProperty)
        Next

        Call MainView.InvalidateFile()
        Call FillListTemplates()
    End Sub

    Private Sub SaveToFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToFileToolStripMenuItem.Click
        Dim SaveFileDialog As New SaveFileDialog
        'SaveFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments

        SaveFileDialog.Filter = "Property Template Files (*.template)|*.template|All Files (*.*)|*.*"

        If (SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            Dim FileName As String = SaveFileDialog.FileName
            Dim File As System.IO.BinaryWriter
            File = New System.IO.BinaryWriter(System.IO.File.Open(FileName, IO.FileMode.OpenOrCreate))
            Hl.TemplateStorage.Save(File)
            File.Close()
        End If

    End Sub

    Private Sub LoadFromFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadFromFileToolStripMenuItem.Click
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.Filter = "Property Template Files (*.template)|*.template|All Files (*.*)|*.*"
        OpenFileDialog.Multiselect = False
        If (OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            Dim FileName As String
            FileName = OpenFileDialog.FileName
            Dim File As System.IO.BinaryReader
            File = New System.IO.BinaryReader(System.IO.File.Open(FileName, IO.FileMode.Open))
            Hl.TemplateStorage.Load(File)
            File.Close()
            Call FillListTemplates()
        End If
    End Sub

    Private Sub AddFromFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddFromFileToolStripMenuItem.Click
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.Filter = "Property Template Files (*.template)|*.template|All Files (*.*)|*.*"
        OpenFileDialog.Multiselect = True
        If (OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            For r = 0 To OpenFileDialog.FileNames.Length - 1
                Dim File = New System.IO.BinaryReader(System.IO.File.Open(OpenFileDialog.FileNames(r), IO.FileMode.Open))
                Hl.TemplateStorage.AddFromFile(File)
                File.Close()
            Next r
            Call FillListTemplates()
        End If
    End Sub

    Private Sub ColorBox_Click(sender As Object, e As EventArgs) Handles ColorBox.Click
        If ColorDialog.ShowDialog() = DialogResult.OK Then
            ColorBox.BackColor = ColorDialog.Color
        End If
    End Sub

    Private Sub Templates_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboDecoration.Items.Clear()
        For Each DecorationType In [Enum].GetNames(GetType(CGraph.EDecoration))
            ComboDecoration.Items.Add(DecorationType)
        Next DecorationType
    End Sub
End Class