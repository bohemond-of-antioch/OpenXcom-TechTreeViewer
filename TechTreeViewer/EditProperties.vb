Public Class EditProperties
    Private Entity As CGraph.CGraphElement

    Public Shared Sub DisplayDialog(ByRef Entity As CGraph.CGraphElement)
        For Each Window As Form In Application.OpenForms
            If TypeOf Window Is EditProperties Then
                If CType(Window, EditProperties).Entity Is Entity Then
                    Call Window.BringToFront()
                    Exit Sub
                End If
            End If
        Next
        Dim NewWindow As EditProperties
        NewWindow = New EditProperties
        NewWindow.Entity = Entity
        NewWindow.Show()
    End Sub

    Private Sub EditProperties_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If Entity.SupportsDisplaySize() Then
            TextSize.Text = Trim(Str(Entity.DisplayProperties.Size))
            TextSize.Enabled = True
        Else
            TextSize.Text = ""
            TextSize.Enabled = False
        End If
        If Entity.SupportsDisplayColor() Then
            ColorBox.Visible = True
            Label1.Visible = True
            ColorBox.BackColor = Entity.DisplayProperties.Color
        Else
            ColorBox.Visible = False
            Label1.Visible = False
        End If
        If Entity.SupportsDisplayDecoration() Then
            ComboDecoration.Visible = True
            Label3.Visible = True
            ComboDecoration.SelectedIndex = Entity.DisplayProperties.Decoration
        Else
            ComboDecoration.Visible = False
            Label3.Visible = False
        End If

        GridProperties.Rows.Clear()
        For Each EntityProperty In Entity.Properties
            Dim Index = GridProperties.Rows.Add()
            GridProperties.Rows.Item(Index).Cells.Item(0).Value = EntityProperty.Key
            GridProperties.Rows.Item(Index).Cells.Item(1).Value = EntityProperty.Value
            If Entity.SupportsDisplayProperties() Then
                GridProperties.Rows.Item(Index).Cells.Item(2).ReadOnly = False
                GridProperties.Rows.Item(Index).Cells.Item(2).Value = Entity.DisplayProperties.ShownProperties.Contains(EntityProperty.Key)
            Else
                GridProperties.Rows.Item(Index).Cells.Item(2).Value = False
                GridProperties.Rows.Item(Index).Cells.Item(2).ReadOnly = True
            End If
        Next
        GridProperties.AutoResizeColumns()

    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        If Entity.SupportsDisplaySize Then
            Entity.DisplayProperties.Size = Val(TextSize.Text)
        End If
        If Entity.SupportsDisplayColor Then
            Entity.DisplayProperties.Color = ColorBox.BackColor
        End If
        If Entity.SupportsDisplayDecoration Then
            Entity.DisplayProperties.Decoration = ComboDecoration.SelectedIndex
        End If

        Entity.Properties.Clear()
        If Entity.SupportsDisplayProperties() Then Entity.DisplayProperties.ShownProperties.Clear()
        For Each Row As DataGridViewRow In GridProperties.Rows
            If Row.Cells.Item(0).Value Is Nothing Then Continue For
            Entity.Properties.Add(New KeyValuePair(Of String, String)(Row.Cells.Item(0).Value, Row.Cells.Item(1).Value))
            If Entity.SupportsDisplayProperties() AndAlso Row.Cells.Item(2).Value = True AndAlso Not Entity.DisplayProperties.ShownProperties.Contains(Row.Cells.Item(0).Value) Then Entity.DisplayProperties.ShownProperties.Add(Row.Cells.Item(0).Value)
        Next

        Call MainView.InvalidateFile()

        Me.Close()
        For Each Window As Form In Application.OpenForms
            If TypeOf Window Is MainView Then
                Window.Refresh()
            End If
        Next

    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        Me.Close()
    End Sub

    Private Sub EditProperties_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.Escape Then Me.Close()
    End Sub

    Private Sub ColorBox_Click(sender As Object, e As EventArgs) Handles ColorBox.Click
        If ColorDialog.ShowDialog() = DialogResult.OK Then
            ColorBox.BackColor = ColorDialog.Color
        End If
    End Sub

    Private Sub EditProperties_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboDecoration.Items.Clear()
        For Each DecorationType In [Enum].GetNames(GetType(CGraph.EDecoration))
            ComboDecoration.Items.Add(DecorationType)
        Next DecorationType
    End Sub
End Class