Imports GraphEditor
Imports TechTreeViewer

Public Class MainView
    Private Enum EOperationState
        Off
        Preparing
        Active
    End Enum
    Private Structure SUIControlData
        Dim GraphX As Integer, GraphY As Integer
        Dim DragX As Integer, DragY As Integer
        Dim IsScroll As Boolean
        Dim DragState As EOperationState
        Dim DragNode As CGraph.CNode
        Dim ScrollX As Integer, ScrollY As Integer
        Dim Scale As Double
        Dim ConnectState As EOperationState
        Dim ConnectNodeA As CGraph.CNode
        Friend LastUsedTemplate As CPropertyTemplate
        Friend KeyScrollY As Integer
        Friend KeyScrollX As Integer
        Friend InteractedObject As CGraph.CGraphElement
    End Structure


    Dim UI As SUIControlData
    Friend NodeTextBrush As Brush
    Friend NodeDarkenedTextBrush As Brush
    Friend DarkenedAlpha As Integer
    Shared CenteredStringFormat As StringFormat

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub MainView_Load(sender As Object, e As EventArgs) Handles Me.Load
        UI.Scale = 1
        DarkenedAlpha = 20
        ComboBoxSearchHighlight.SelectedIndex = 1
        NodeTextBrush = Brushes.Black
        NodeDarkenedTextBrush = New SolidBrush(Color.FromArgb(DarkenedAlpha, 0, 0, 0))
        Call Hl.Initialize()
        Call Hl.LoadSettings()
        CenteredStringFormat = New StringFormat()
        CenteredStringFormat.Alignment = StringAlignment.Center
        Call UpdateTitle()
    End Sub

    Private Sub MainView_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        UI.GraphX = (e.X - UI.ScrollX) / UI.Scale
        UI.GraphY = (e.Y - UI.ScrollY) / UI.Scale
        If e.Button = MouseButtons.Left Then
            Dim Node = FindNode(UI.GraphX, UI.GraphY)
            If Not Node Is Nothing AndAlso Node.DisplayProperties.Highlighted Then
                UI.DragState = EOperationState.Preparing
                UI.DragNode = Node
                UI.DragX = e.X
                UI.DragY = e.Y
            Else
                UI.IsScroll = True
                UI.DragX = e.X
                UI.DragY = e.Y
            End If
        End If
        If e.Button = MouseButtons.Middle Then
            UI.IsScroll = True
            UI.DragX = e.X
            UI.DragY = e.Y
        End If
    End Sub
    Private Sub MainView_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        UI.GraphX = (e.X - UI.ScrollX) / UI.Scale
        UI.GraphY = (e.Y - UI.ScrollY) / UI.Scale
        If UI.DragState = EOperationState.Preparing Or UI.DragState = EOperationState.Active Then
            Dim DeltaX As Integer = UI.DragX - e.X
            Dim DeltaY As Integer = UI.DragY - e.Y

            If DeltaX <> 0 Or DeltaY <> 0 Then
                UI.DragState = EOperationState.Active

                UI.DragNode.DisplayProperties.Position.X = (e.X - UI.ScrollX) / UI.Scale
                UI.DragNode.DisplayProperties.Position.Y = (e.Y - UI.ScrollY) / UI.Scale
            End If
            UI.DragX = e.X
            UI.DragY = e.Y

            Call InvalidateFile()
            Me.Refresh()
        End If
        If UI.IsScroll Then
            Dim DeltaX As Integer = UI.DragX - e.X
            Dim DeltaY As Integer = UI.DragY - e.Y

            UI.ScrollX -= DeltaX
            UI.ScrollY -= DeltaY

            UI.DragX = e.X
            UI.DragY = e.Y

            Me.Refresh()
        End If
        If UI.ConnectState = EOperationState.Active Then
            Me.Refresh()
        End If
    End Sub
    Private Sub MainView_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        UI.GraphX = (e.X - UI.ScrollX) / UI.Scale
        UI.GraphY = (e.Y - UI.ScrollY) / UI.Scale
        If e.Button = MouseButtons.Left And UI.DragState = EOperationState.Preparing Then
            UI.DragState = EOperationState.Off
            UI.DragNode = Nothing
        End If
        If UI.IsScroll Then
            UI.IsScroll = False
        ElseIf e.Button = MouseButtons.Right Then
            Dim Node = FindNode(UI.GraphX, UI.GraphY)
            If Node Is Nothing Then
                Dim Connection = FindConnection(UI.GraphX, UI.GraphY)
                If Connection Is Nothing Then
                    UI.InteractedObject = Nothing
                    MenuNewNode.Show(Me, e.X, e.Y)
                Else
                    UI.InteractedObject = Connection
                    MenuEditNode.Show(Me, e.X, e.Y)
                End If
            Else
                UI.InteractedObject = Node
                MenuEditNode.Show(Me, e.X, e.Y)
            End If
        ElseIf e.Button = MouseButtons.Left And UI.DragState = EOperationState.Active Then
            UI.DragState = EOperationState.Off
            UI.DragNode = Nothing
        ElseIf e.Button = MouseButtons.Left And UI.ConnectState = EOperationState.Preparing Then
            UI.ConnectState = EOperationState.Active
        ElseIf e.Button = MouseButtons.Left And UI.ConnectState = EOperationState.Active Then
            Dim Node = FindNode(UI.GraphX, UI.GraphY)
            If Not Node Is Nothing Then
                UI.ConnectState = EOperationState.Off
                Dim DefaultTemplate As CPropertyTemplate = Nothing
                For Each Template As CPropertyTemplate In Hl.TemplateStorage
                    If Template.AppliesToConnections AndAlso Template.DefaultTemplate Then
                        DefaultTemplate = Template
                        Exit For
                    End If
                Next Template

                Graph.Connect(UI.ConnectNodeA.Index, Node.Index, DefaultTemplate)
                Call InvalidateFile()
                Me.Refresh()
            End If
        End If

    End Sub

    Private Function FindConnection(X As Integer, Y As Integer) As CGraph.CConnection
        Dim MinDistance As Double
        Dim NearestConnection As CGraph.CConnection = Nothing

        Dim Cursor = New PointF(X, Y)

        For Each Connection In Graph.GetAllConnections
            Dim Distance As Double
            Dim SegmentA, SegmentB As Point
            Dim Angle As Single
            If Connection.NodeA = Connection.NodeB Then
                Dim Node = Graph.GetNode(Connection.NodeA)
                Dim CenterX = Node.DisplayProperties.Position.X - Node.DisplayProperties.Size * 0.75
                Dim CenterY = Node.DisplayProperties.Position.Y + Node.DisplayProperties.Size * 0.75
                Dim Radius = Node.DisplayProperties.Size * 0.75
                Distance = Math.Abs(Radius - Math.Sqrt((CenterX - Cursor.X) * (CenterX - Cursor.X) + (CenterY - Cursor.Y) * (CenterY - Cursor.Y)))

            Else
                CalculateConnectionPoints(Connection, SegmentA, SegmentB, Angle)
                Distance = FindDistanceToSegment(Cursor, SegmentA, SegmentB)
            End If
            If NearestConnection Is Nothing OrElse Distance < MinDistance Then
                MinDistance = Distance
                NearestConnection = Connection
            End If
        Next
        If MinDistance <= 8 Then Return NearestConnection
        Return Nothing
    End Function
    Private Sub CalculateConnectionPoints(ByRef Connection As CGraph.CConnection, ByRef SegmentA As Point, ByRef SegmentB As Point, ByRef Angle As Single)
        Dim NodeA, NodeB As CGraph.CNode
        Dim LineX1, LineY1, LineX2, LineY2 As Integer

        NodeA = Graph.GetNode(Connection.NodeA)
        NodeB = Graph.GetNode(Connection.NodeB)
        LineX1 = NodeA.DisplayProperties.Position.X
        LineY1 = NodeA.DisplayProperties.Position.Y
        Angle = Math.Atan2(NodeB.DisplayProperties.Position.Y - NodeA.DisplayProperties.Position.Y, NodeB.DisplayProperties.Position.X - NodeA.DisplayProperties.Position.X)
        LineX1 += Math.Cos(Angle) * NodeA.DisplayProperties.Size
        LineY1 += Math.Sin(Angle) * NodeA.DisplayProperties.Size

        LineX2 = NodeB.DisplayProperties.Position.X
        LineY2 = NodeB.DisplayProperties.Position.Y

        LineX2 -= Math.Cos(Angle) * NodeB.DisplayProperties.Size
        LineY2 -= Math.Sin(Angle) * NodeB.DisplayProperties.Size
        If Connection.TwoWay Then
            LineX1 += Math.Cos(Angle + Math.PI / 2) * 5
            LineY1 += Math.Sin(Angle + Math.PI / 2) * 5
            LineX2 += Math.Cos(Angle + Math.PI / 2) * 5
            LineY2 += Math.Sin(Angle + Math.PI / 2) * 5
        End If

        SegmentA.X = LineX1
        SegmentA.Y = LineY1
        SegmentB.X = LineX2
        SegmentB.Y = LineY2
    End Sub
    Private Sub MainView_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim Stopwatch As Stopwatch = New Stopwatch()
        e.Graphics.TranslateTransform(UI.ScrollX, UI.ScrollY)
        e.Graphics.ScaleTransform(UI.Scale, UI.Scale)
        Stopwatch.Start()
        For Each Connection As CGraph.CConnection In Graph.GetAllConnections
            Dim SegmentA, SegmentB As Point
            Dim Angle As Single
            Dim LinePen As Pen
            If Connection.DisplayProperties.Highlighted Then
                LinePen = New Pen(Connection.DisplayProperties.Color)
            Else
                LinePen = New Pen(Color.FromArgb(DarkenedAlpha, Connection.DisplayProperties.Color.R, Connection.DisplayProperties.Color.G, Connection.DisplayProperties.Color.B))
            End If
            If Connection.NodeA = Connection.NodeB Then
                Angle = Math.PI / 4
                Dim Node = Graph.GetNode(Connection.NodeA)

                SegmentA = New Point(Node.DisplayProperties.Position.X - Node.DisplayProperties.Size * 1.2 - 20, Node.DisplayProperties.Position.Y + Node.DisplayProperties.Size * 1.1 - 20)
                SegmentB = New Point(Node.DisplayProperties.Position.X - Node.DisplayProperties.Size * 1.2 + 20, Node.DisplayProperties.Position.Y + Node.DisplayProperties.Size * 1.1 + 20)

                e.Graphics.DrawArc(LinePen, CType(Node.DisplayProperties.Position.X - Node.DisplayProperties.Size * 1.5, Integer), Node.DisplayProperties.Position.Y, CType(Node.DisplayProperties.Size * 1.5, Integer), CType(Node.DisplayProperties.Size * 1.5, Integer), 20, 230)

                Dim IntersectionPoint = New Point(Node.DisplayProperties.Position.X - Node.DisplayProperties.Size * 1, Node.DisplayProperties.Position.Y + Node.DisplayProperties.Size * 0.027)

                e.Graphics.DrawLine(LinePen, IntersectionPoint.X, IntersectionPoint.Y, CType(IntersectionPoint.X - Math.Cos(-0.05) * 12, Integer), CType(IntersectionPoint.Y - Math.Sin(-0.05) * 12, Integer))
                e.Graphics.DrawLine(LinePen, IntersectionPoint.X, IntersectionPoint.Y, CType(IntersectionPoint.X - Math.Cos(-1.05) * 12, Integer), CType(IntersectionPoint.Y - Math.Sin(-1.05) * 12, Integer))
            Else
                CalculateConnectionPoints(Connection, SegmentA, SegmentB, Angle)

                e.Graphics.DrawLine(LinePen, SegmentB.X, SegmentB.Y, CType(SegmentB.X - Math.Cos(Angle - 0.5) * 12, Integer), CType(SegmentB.Y - Math.Sin(Angle - 0.5) * 12, Integer))
                If Not Connection.TwoWay Then
                    e.Graphics.DrawLine(LinePen, SegmentB.X, SegmentB.Y, CType(SegmentB.X - Math.Cos(Angle + 0.5) * 12, Integer), CType(SegmentB.Y - Math.Sin(Angle + 0.5) * 12, Integer))
                End If

                e.Graphics.DrawLine(LinePen, SegmentA.X, SegmentA.Y, SegmentB.X, SegmentB.Y)
            End If


            If Connection.DisplayProperties.ShownProperties.Count > 0 Then
                Dim LineMiddleX, LineMiddleY As Integer
                LineMiddleX = (SegmentA.X + SegmentB.X) / 2
                LineMiddleY = (SegmentA.Y + SegmentB.Y) / 2

                Dim PrevTransform = e.Graphics.Transform
                e.Graphics.TranslateTransform(LineMiddleX, LineMiddleY)
                e.Graphics.RotateTransform(Angle * 180 / Math.PI)
                e.Graphics.TranslateTransform(-LineMiddleX, -LineMiddleY)
                Dim DisplayedProperty As Integer
                DisplayedProperty = 0
                Dim TextBrush As Brush
                If Connection.DisplayProperties.Highlighted Then
                    TextBrush = NodeTextBrush
                Else
                    TextBrush = NodeDarkenedTextBrush
                End If
                For Each PropertyName As String In Connection.DisplayProperties.ShownProperties
                    For Each P In Connection.Properties
                        If P.Key = PropertyName Then
                            e.Graphics.DrawString(P.Value, Me.Font, TextBrush, New Rectangle(LineMiddleX - 250, LineMiddleY + 5 + DisplayedProperty * 15, 500, 50), CenteredStringFormat)
                            DisplayedProperty += 1
                        End If
                    Next
                Next

                e.Graphics.Transform = PrevTransform
            End If
        Next Connection
        Stopwatch.Stop()
        Debug.WriteLine("Paint connections processing time: " + Trim(Str(Stopwatch.ElapsedMilliseconds)) + "ms")
        Stopwatch.Restart()
        For Each Node As CGraph.CNode In Graph
            Dim Pen
            If Node.DisplayProperties.Highlighted Then
                Pen = New Pen(Node.DisplayProperties.Color)
            Else
                Pen = New Pen(Color.FromArgb(DarkenedAlpha, Node.DisplayProperties.Color.R, Node.DisplayProperties.Color.G, Node.DisplayProperties.Color.B))
            End If
            e.Graphics.DrawEllipse(Pen, CType(Node.DisplayProperties.Position.X - Node.DisplayProperties.Size, Integer), CType(Node.DisplayProperties.Position.Y - Node.DisplayProperties.Size, Integer), Node.DisplayProperties.Size * 2, Node.DisplayProperties.Size * 2)
            Select Case Node.DisplayProperties.Decoration
                Case CGraph.EDecoration.Square
                    REM a=(2*r)/sqr(2)
                    Dim A = 2 * Node.DisplayProperties.Size / 1.4142135623730949
                    e.Graphics.DrawRectangle(Pen, CType(Node.DisplayProperties.Position.X - A / 2, Integer), CType(Node.DisplayProperties.Position.Y - A / 2, Integer), CType(A, Integer), CType(A, Integer))
                Case CGraph.EDecoration.Triangle
                    REM v=r*3/2
                    REM v2=a2-(a/2)2 => 3a2/4 => a2=4v2/3
                    Dim V = Node.DisplayProperties.Size * 3 / 2
                    Dim A = Math.Sqrt(V * V * 4 / 3)
                    Dim V1x = CType(Node.DisplayProperties.Position.X - A / 2, Integer)
                    Dim V1y = CType(Node.DisplayProperties.Position.Y + V / 3, Integer)

                    Dim V2x = CType(Node.DisplayProperties.Position.X + A / 2, Integer)
                    Dim V2y = CType(Node.DisplayProperties.Position.Y + V / 3, Integer)

                    Dim V3x = CType(Node.DisplayProperties.Position.X, Integer)
                    Dim V3y = CType(Node.DisplayProperties.Position.Y - V / 3 * 2, Integer)

                    e.Graphics.DrawLine(Pen, V1x, V1y, V2x, V2y)
                    e.Graphics.DrawLine(Pen, V2x, V2y, V3x, V3y)
                    e.Graphics.DrawLine(Pen, V1x, V1y, V3x, V3y)
            End Select

            Dim DisplayedProperty As Integer
            DisplayedProperty = 0
            Dim TextBrush As Brush
            If Node.DisplayProperties.Highlighted Then
                TextBrush = NodeTextBrush
            Else
                TextBrush = NodeDarkenedTextBrush
            End If
            For Each PropertyName As String In Node.DisplayProperties.ShownProperties
                For Each P In Node.Properties
                    If P.Key = PropertyName Then
                        e.Graphics.DrawString(P.Value, Me.Font, TextBrush, New Rectangle(Node.DisplayProperties.Position.X - 250, Node.DisplayProperties.Position.Y + Node.DisplayProperties.Size + 5 + DisplayedProperty * 15, 500, 50), CenteredStringFormat)
                        DisplayedProperty += 1
                    End If
                Next
            Next
        Next Node
        Stopwatch.Stop()
        Debug.WriteLine("Paint nodes processing time: " + Trim(Str(Stopwatch.ElapsedMilliseconds)) + "ms")
        If UI.ConnectState = EOperationState.Active Then
            Dim LineX1, LineY1, LineX2, LineY2 As Integer
            LineX1 = UI.ConnectNodeA.DisplayProperties.Position.X
            LineY1 = UI.ConnectNodeA.DisplayProperties.Position.Y

            e.Graphics.DrawLine(Pens.BlueViolet, LineX1, LineY1, UI.GraphX, UI.GraphY)
        End If
    End Sub
    Private Sub CreateNode(X As Integer, Y As Integer, Optional ByRef UseTemplate As CPropertyTemplate = Nothing)
        If UseTemplate Is Nothing Then
            For Each Template As CPropertyTemplate In Hl.TemplateStorage
                If Template.AppliesToNodes AndAlso Template.DefaultTemplate Then
                    UseTemplate = Template
                    Exit For
                End If
            Next Template
        Else
        End If

        UI.LastUsedTemplate = UseTemplate
        Dim NewNode As CGraph.CNode = Graph.Add(UseTemplate)
        NewNode.DisplayProperties.Position.X = X
        NewNode.DisplayProperties.Position.Y = Y
        Call InvalidateFile()
    End Sub

    Private Sub MenuNewNodeCreate_Click(sender As Object, e As EventArgs) Handles MenuNewNodeCreate.Click
        CreateNode(UI.GraphX, UI.GraphY)
        Me.Refresh()
    End Sub

    Private Sub MainView_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Down Then
            UI.KeyScrollY = -1
        End If
        If e.KeyCode = Keys.Up Then
            UI.KeyScrollY = 1
        End If
        If e.KeyCode = Keys.Left Then
            UI.KeyScrollX = 1
        End If
        If e.KeyCode = Keys.Right Then
            UI.KeyScrollX = -1
        End If
    End Sub
    Private Sub ChangeZoom(Value As Double)
        Dim PreviousScale As Double
        PreviousScale = UI.Scale
        UI.Scale += Value
        If UI.Scale < 0.1 Then UI.Scale = 0.1
        UI.ScrollX -= (Me.Width / 2 - UI.ScrollX) / PreviousScale * (UI.Scale) - Me.Width / 2 + UI.ScrollX
        UI.ScrollY -= (Me.Height / 2 - UI.ScrollY) / PreviousScale * (UI.Scale) - Me.Height / 2 + UI.ScrollY
        Me.Refresh()
    End Sub
    Private Sub MainView_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.F3 And ModifierKeys = Keys.None Then
            SearchNext()
        End If
        If e.KeyCode = Keys.F3 And ModifierKeys = Keys.Shift Then
            SearchPrevious()
        End If
        If e.KeyCode = Keys.F And ModifierKeys = Keys.Control Then
            ToggleSearch()
        End If
        If e.KeyCode = Keys.Space Then
            SetHighlights(True)
            e.Handled = True
            Me.Refresh()
        End If
        If e.KeyCode = Keys.Down Then
            UI.KeyScrollY = 0
        End If
        If e.KeyCode = Keys.Up Then
            UI.KeyScrollY = 0
        End If
        If e.KeyCode = Keys.Left Then
            UI.KeyScrollX = 0
        End If
        If e.KeyCode = Keys.Right Then
            UI.KeyScrollX = 0
        End If
        If e.KeyCode = Keys.Escape Then
            If UI.ConnectState = EOperationState.Active Then
                UI.ConnectState = EOperationState.Off
                UI.ConnectNodeA = Nothing
                Me.Refresh()
            End If
        End If
        If e.KeyCode = Keys.Add Then
            ChangeZoom(0.1)
        End If
        If e.KeyCode = Keys.Subtract Then
            ChangeZoom(-0.1)
        End If
    End Sub

    Private Function FindNode(X As Integer, Y As Integer) As CGraph.CNode
        For Each Node As CGraph.CNode In Graph
            If (Node.DisplayProperties.Position.X - X) * (Node.DisplayProperties.Position.X - X) + (Node.DisplayProperties.Position.Y - Y) * (Node.DisplayProperties.Position.Y - Y) <= Node.DisplayProperties.Size * Node.DisplayProperties.Size Then
                Return Node
            End If
        Next Node
        Return Nothing
    End Function

    Private Sub MenuEditNodeRemove_Click(sender As Object, e As EventArgs) Handles MenuEditNodeRemove.Click
        If UI.InteractedObject Is Nothing Then Exit Sub
        If TypeOf UI.InteractedObject Is CGraph.CConnection Then
            Dim Connection As CGraph.CConnection = UI.InteractedObject
            If MsgBox("Are you sure you want to remove the CONNECTION?", MsgBoxStyle.YesNo, "Graph editor") = MsgBoxResult.Yes Then
                Graph.Disconnect(Connection)
                Call InvalidateFile()
                Me.Refresh()
            End If
        End If
        If TypeOf UI.InteractedObject Is CGraph.CNode Then
            If MsgBox("Are you sure you want to remove the NODE?", MsgBoxStyle.YesNo, "Graph editor") = MsgBoxResult.Yes Then
                Graph.Remove(UI.InteractedObject)
                Call InvalidateFile()
                Me.Refresh()
            End If
        End If
    End Sub

    Private Sub MainView_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        UI.GraphX = (e.X - UI.ScrollX) / UI.Scale
        UI.GraphY = (e.Y - UI.ScrollY) / UI.Scale
        If e.Button = MouseButtons.Left And ModifierKeys = Keys.Shift Then
            Dim Node = FindNode(UI.GraphX, UI.GraphY)
            If Not Node Is Nothing AndAlso Node.DisplayProperties.Highlighted Then
                HighlightNeighborhood(Node, 1)
            End If
        End If
    End Sub

    Private Sub MainView_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles Me.MouseDoubleClick
        UI.GraphX = (e.X - UI.ScrollX) / UI.Scale
        UI.GraphY = (e.Y - UI.ScrollY) / UI.Scale
        If e.Button = MouseButtons.Left Then
            Dim Node = FindNode(UI.GraphX, UI.GraphY)
            If Node Is Nothing Then
                CreateNode(UI.GraphX, UI.GraphY)
                Me.Refresh()
            Else
                UI.ConnectState = EOperationState.Preparing
                UI.ConnectNodeA = Node
                Call InvalidateFile()
                Me.Refresh()
            End If
        End If
    End Sub

    Public Sub InvalidateFile()
        Hl.ChangesSaved = False
        Call UpdateTitle()
    End Sub

    Private Sub ProgramMenuTemplates_Click(sender As Object, e As EventArgs) Handles ProgramMenuTemplates.Click
        Call (New Templates).Show()
    End Sub

    Private Sub MenuEditNodeProperties_Click(sender As Object, e As EventArgs) Handles MenuEditNodeProperties.Click
        If Not UI.InteractedObject Is Nothing Then
            EditProperties.DisplayDialog(UI.InteractedObject)
        End If
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        If CheckUnsavedFile() Then End
    End Sub

    Private Sub MenuNewNode_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles MenuNewNode.Opening
        Dim Index As Integer = 0
        MenuNewNodeCreateFromTemplate.DropDownItems.Clear()
        For Each Template As CPropertyTemplate In Hl.TemplateStorage
            If Template.AppliesToNodes Then
                Dim NewMenuTemplate As ToolStripMenuItem
                NewMenuTemplate = New ToolStripMenuItem(Template.Name, Nothing, AddressOf MenuNewNode_HandleFromTemplate)
                NewMenuTemplate.Tag = Trim(Str(Index))
                Index += 1
                MenuNewNodeCreateFromTemplate.DropDownItems.Add(NewMenuTemplate)
            End If
        Next Template
    End Sub

    Private Sub MenuNewNode_HandleFromTemplate(sender As Object, e As EventArgs)
        Dim Index As Integer = 0
        For Each Template As CPropertyTemplate In Hl.TemplateStorage
            If Template.AppliesToNodes Then
                If Index = Val(sender.tag) Then
                    CreateNode(UI.GraphX, UI.GraphY, Template)
                    Me.Refresh()
                    Exit Sub
                End If
                Index += 1
            End If
        Next Template
    End Sub

    Private Sub MenuNewNodeCreateFromLastTemplate_Click(sender As Object, e As EventArgs) Handles MenuNewNodeCreateFromLastTemplate.Click
        CreateNode(UI.GraphX, UI.GraphY, UI.LastUsedTemplate)
        Me.Refresh()
    End Sub

    Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripMenuItem.Click
        If CheckUnsavedFile() Then
            Hl.Initialize()
            Me.Refresh()
        End If
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click
        Dim SaveFileDialog As New SaveFileDialog
        'SaveFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments

        SaveFileDialog.Filter = "Graph Files (*.graph)|*.graph|All Files (*.*)|*.*"
        SaveFileDialog.FileName = Hl.CurrentGraphFileName

        If (SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            Dim FileName As String = SaveFileDialog.FileName
            Dim File As System.IO.BinaryWriter
            File = New System.IO.BinaryWriter(System.IO.File.Open(FileName, IO.FileMode.OpenOrCreate))
            Hl.Graph.Save(File)
            Hl.TemplateStorage.Save(File)
            Hl.AddRecentItem(FileName, ERecentItemAction.Open)
            File.Close()
            Hl.CurrentGraphFileName = FileName
            Hl.ChangesSaved = True
            Call UpdateTitle()
        End If

    End Sub

    Private Sub OpenFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenFileToolStripMenuItem.Click
        If CheckUnsavedFile() Then
            Dim OpenFileDialog As New OpenFileDialog
            OpenFileDialog.Filter = "Graph Files (*.graph)|*.graph|All Files (*.*)|*.*"
            OpenFileDialog.Multiselect = False
            If (OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                Dim FileName As String
                FileName = OpenFileDialog.FileName
                OpenGraphFile(FileName)
            End If
        End If
    End Sub
    Private Sub StartImportXComResearch(Path As String)
        Hl.ImportXComResearch(Path)
        Hl.ChangesSaved = False
        Call UpdateTitle()
        Call CenterAndZoomToGraph()
        Me.Refresh()
    End Sub
    Private Sub OpenGraphFile(FileName As String)
        Dim File As System.IO.BinaryReader = New System.IO.BinaryReader(System.IO.File.Open(FileName, IO.FileMode.Open))
        Hl.Initialize()
        Hl.AddRecentItem(FileName, ERecentItemAction.Open)
        Hl.Graph.Load(File)
        Hl.TemplateStorage.Load(File)
        File.Close()
        Hl.CurrentGraphFileName = FileName
        Hl.ChangesSaved = True
        Call UpdateTitle()
        Call CenterAndZoomToGraph()
        Me.Refresh()
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        If Hl.CurrentGraphFileName <> "" Then
            Dim File As System.IO.BinaryWriter
            File = New System.IO.BinaryWriter(System.IO.File.Open(Hl.CurrentGraphFileName, IO.FileMode.OpenOrCreate))
            Hl.Graph.Save(File)
            Hl.TemplateStorage.Save(File)
            Hl.AddRecentItem(Hl.CurrentGraphFileName, ERecentItemAction.Open)
            File.Close()
            Hl.ChangesSaved = True
            Call UpdateTitle()
        Else
            SaveAsToolStripMenuItem_Click(sender, e)
        End If
    End Sub

    Private Sub UpdateTitle()
        If Hl.CurrentGraphFileName = "" Then
            Me.Text = "New Graph"
        Else
            Me.Text = System.IO.Path.GetFileNameWithoutExtension(Hl.CurrentGraphFileName)
        End If
        If Not Hl.ChangesSaved Then Me.Text += "*"
    End Sub

    Private Sub ProgramMenuSortAttractors_Click(sender As Object, e As EventArgs) Handles ProgramMenuSortAttractors.Click
        Hl.SortMode = ESortMode.Attractors
        SortTimer.Enabled = True
    End Sub

    Private Sub StopSortToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopSortToolStripMenuItem.Click
        SortTimer.Enabled = False
    End Sub

    Private Sub SortTimer_Tick(sender As Object, e As EventArgs) Handles SortTimer.Tick
        Call Hl.ProcessSort()
        Me.Refresh()
    End Sub

    Private Sub MenuEditNode_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles MenuEditNode.Opening
        Dim Index As Integer = 0
        Dim Node = FindNode(UI.GraphX, UI.GraphY)
        Dim Connection = Nothing
        If Node Is Nothing Then
            Connection = FindConnection(UI.GraphX, UI.GraphY)
        End If
        MenuEditNodeApplyTemplate.DropDownItems.Clear()
        For Each Template As CPropertyTemplate In Hl.TemplateStorage
            If (Not Node Is Nothing And Template.AppliesToNodes) Or (Not Connection Is Nothing And Template.AppliesToConnections) Then
                Dim NewMenuTemplate As ToolStripMenuItem
                NewMenuTemplate = New ToolStripMenuItem(Template.Name, Nothing, AddressOf MenuEditNode_HandleApplyTemplate)
                NewMenuTemplate.Tag = Trim(Str(Index))
                Index += 1
                MenuEditNodeApplyTemplate.DropDownItems.Add(NewMenuTemplate)
            End If
        Next Template
    End Sub
    Friend Sub RebuildRecent()
        Dim Index As Integer = 0
        RecentToolStripMenuItem.DropDownItems.Clear()
        For Each Item As SRecentItem In RecentItems
            Dim NewMenuTemplate As ToolStripMenuItem
            NewMenuTemplate = New ToolStripMenuItem(Item.Description, Nothing, AddressOf RecentItem_Click)
            NewMenuTemplate.Tag = Trim(Str(Index))
            Index += 1
            RecentToolStripMenuItem.DropDownItems.Add(NewMenuTemplate)
        Next Item
    End Sub
    Private Sub RecentItem_Click(sender As Object, e As EventArgs)
        Dim Index As Integer = Val(sender.tag)
        Select Case RecentItems(Index).Action
            Case ERecentItemAction.ImportXComResearch
                If Not CheckUnsavedFile() Then Exit Sub
                Dim Path = RecentItems(Index).FileName
                StartImportXComResearch(Path)
            Case ERecentItemAction.Open
                If Not CheckUnsavedFile() Then Exit Sub
                Dim Path = RecentItems(Index).FileName
                OpenGraphFile(Path)
        End Select
    End Sub

    Private Sub MenuEditNode_HandleApplyTemplate(sender As Object, e As EventArgs)
        If UI.InteractedObject Is Nothing Then Exit Sub
        Dim Index As Integer = 0
        If TypeOf UI.InteractedObject Is CGraph.CConnection Then
            Dim UseTemplate As CPropertyTemplate = Nothing
            For Each Template As CPropertyTemplate In Hl.TemplateStorage
                If Template.AppliesToConnections Then
                    If Index = Val(sender.tag) Then
                        UseTemplate = Template
                        Exit For
                    End If
                    Index += 1
                End If
            Next Template
            If UseTemplate Is Nothing Then Exit Sub
            UI.InteractedObject.ApplyTemplate(UseTemplate)
            Call InvalidateFile()
            Me.Refresh()
        End If
        If TypeOf UI.InteractedObject Is CGraph.CNode Then
            Dim UseTemplate As CPropertyTemplate = Nothing
            For Each Template As CPropertyTemplate In Hl.TemplateStorage
                If Template.AppliesToNodes Then
                    If Index = Val(sender.tag) Then
                        UseTemplate = Template
                        Exit For
                    End If
                    Index += 1
                End If
            Next Template
            If UseTemplate Is Nothing Then Exit Sub
            UI.InteractedObject.ApplyTemplate(UseTemplate)
            Call InvalidateFile()
            Me.Refresh()
        End If
    End Sub

    Private Sub OpenXComResearchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenXComResearchToolStripMenuItem.Click
        Dim OpenFolderDialog As New FolderBrowserDialog
        If Not CheckUnsavedFile() Then Exit Sub
        If OpenFolderDialog.ShowDialog() = DialogResult.OK Then
            Dim Path = OpenFolderDialog.SelectedPath
            StartImportXComResearch(Path)
        End If
    End Sub

    Private Function CheckUnsavedFile() As Boolean
        If Not Hl.ChangesSaved Then
            Dim Result As MsgBoxResult
            Result = MsgBox("You have unsaved changes. Do you want to save the file before closing it?", MsgBoxStyle.YesNoCancel Or MsgBoxStyle.Question, "Unsaved changes")
            If Result = MsgBoxResult.Yes Then
                SaveToolStripMenuItem_Click(Me, New EventArgs)
            ElseIf Result = MsgBoxResult.Cancel Then
                Return False
            End If
        End If
        Return True
    End Function

    Private Sub MainView_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not CheckUnsavedFile() Then e.Cancel = True
    End Sub

    Private Sub FileToolStripMenuItem_DropDownOpened(sender As Object, e As EventArgs) Handles FileToolStripMenuItem.DropDownOpened
        RebuildRecent()
    End Sub

    Private Sub CenterAndZoomToGraph()
        Dim Center As PointF
        Center.X = 0
        Center.Y = 0
        For Each Node As CGraph.CNode In Graph
            Center.X += Node.DisplayProperties.Position.X
            Center.Y += Node.DisplayProperties.Position.Y
        Next Node
        If Graph.Nodes.Count > 0 Then
            Center.X = Center.X / Graph.Nodes.Count
            Center.Y = Center.Y / Graph.Nodes.Count
        End If
        UI.ScrollX = Me.Width / 2 - Center.X * UI.Scale
        UI.ScrollY = Me.Height / 2 - Center.Y * UI.Scale
    End Sub

    Private Sub KeyScrollTimer_Tick(sender As Object, e As EventArgs) Handles KeyScrollTimer.Tick
        If UI.KeyScrollX <> 0 Then
            UI.ScrollX += UI.KeyScrollX * 10
        End If
        If UI.KeyScrollY <> 0 Then
            UI.ScrollY += UI.KeyScrollY * 10
        End If
        If UI.KeyScrollX <> 0 Or UI.KeyScrollY <> 0 Then
            Me.Refresh()
        End If
    End Sub

    Private Sub MainView_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        If e.Delta <> 0 Then
            ChangeZoom(e.Delta / SystemInformation.MouseWheelScrollDelta * 0.1)
        End If
    End Sub

    Private Sub MenuEditHighlightNeigborhood1_Click(sender As Object, e As EventArgs) Handles MenuEditHighlightNeigborhood1.Click
        HighlightNeighborhood(UI.InteractedObject, 1)
    End Sub

    Private Sub MenuEditHighlightNeigborhood2_Click(sender As Object, e As EventArgs) Handles MenuEditHighlightNeigborhood2.Click
        HighlightNeighborhood(UI.InteractedObject, 2)
    End Sub

    Private Sub MenuEditHighlightNeigborhood3_Click(sender As Object, e As EventArgs) Handles MenuEditHighlightNeigborhood3.Click
        HighlightNeighborhood(UI.InteractedObject, 3)
    End Sub

    Private Sub MenuEditHighlightNeigborhood4_Click(sender As Object, e As EventArgs) Handles MenuEditHighlightNeigborhood4.Click
        HighlightNeighborhood(UI.InteractedObject, 4)
    End Sub

    Private Sub MenuEditHighlightNeigborhood5_Click(sender As Object, e As EventArgs) Handles MenuEditHighlightNeigborhood5.Click
        HighlightNeighborhood(UI.InteractedObject, 5)
    End Sub

    Private Sub HighlightNeighborhood(ByRef RootNode As CGraph.CNode, Depth As Integer)
        If RootNode Is Nothing OrElse Not TypeOf RootNode Is CGraph.CNode Then Exit Sub
        SetHighlights(False)
        HighlighNeighborhoodTraverse(RootNode, Depth)
        Me.Refresh()
    End Sub
    Private Sub HighlighNeighborhoodTraverse(ByRef Node As CGraph.CNode, Depth As Integer)
        Node.DisplayProperties.Highlighted = True
        If Depth = 0 Then Exit Sub
        For Each Connection In Graph.GetConnections(Node.Index)
            Connection.DisplayProperties.Highlighted = True
            If Node.Index = Connection.NodeA Then
                HighlighNeighborhoodTraverse(Graph.GetNode(Connection.NodeB), Depth - 1)
            Else
                HighlighNeighborhoodTraverse(Graph.GetNode(Connection.NodeA), Depth - 1)
            End If
        Next Connection
    End Sub

    Private Sub ClearHighlightToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearHighlightToolStripMenuItem.Click
        SetHighlights(True)
    End Sub

    Private Shared Sub SetHighlights(Value As Boolean)
        For Each Node As CGraph.CNode In Graph
            Node.DisplayProperties.Highlighted = Value
        Next Node
        For Each Connection As CGraph.CConnection In Graph.GetAllConnections
            Connection.DisplayProperties.Highlighted = Value
        Next Connection
    End Sub
    Private Sub ToggleSearch()
        GroupBoxSearch.Visible = Not GroupBoxSearch.Visible
        If (GroupBoxSearch.Visible) Then
            TextBoxSearch.Focus()
        Else
            Me.Focus()
        End If
    End Sub
    Private Sub SearchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchToolStripMenuItem.Click
        ToggleSearch()
    End Sub
    Private SearchFoundNodes As List(Of CGraph.CNode)
    Private SearchCurrentNode As CGraph.CNode
    Private Sub SeekNodes(Pattern As String)
        SearchFoundNodes = New List(Of CGraph.CNode)
        For Each Node As CGraph.CNode In Graph
            Dim NodeName As String = Node.GetProperty("Name")
            If Not NodeName Is Nothing AndAlso System.Text.RegularExpressions.Regex.IsMatch(NodeName, Pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) Then
                SearchFoundNodes.Add(Node)
            End If
        Next Node
        LabelSearchOccurences.Text = "Occurences: " + Trim(Str(SearchFoundNodes.Count))
        If SearchFoundNodes.Count = 0 Then
            LabelSearchMessage.Text = "No nodes match search criteria"
            LabelSearchMessage.ForeColor = Color.Red
            SearchCurrentNode = Nothing
        Else
            LabelSearchMessage.Text = ""
            SearchNextOrCurrent()
        End If
    End Sub
    Private Sub TextBoxSearch_TextChanged(sender As Object, e As EventArgs) Handles TextBoxSearch.TextChanged
        SeekNodes(TextBoxSearch.Text)
    End Sub

    Private Sub CenterAtNode(Node As CGraph.CNode)
        UI.ScrollX = Me.Width / 2 - Node.DisplayProperties.Position.X * UI.Scale
        UI.ScrollY = Me.Height / 2 - Node.DisplayProperties.Position.Y * UI.Scale
    End Sub

    Private Sub SearchNextOrCurrent()
        If SearchFoundNodes.Count = 0 Then Exit Sub
        If SearchFoundNodes.Contains(SearchCurrentNode) Then
            CenterAtNode(SearchCurrentNode)
        Else
            SearchNext()
        End If
    End Sub

    Private Sub SearchNext()
        If SearchFoundNodes.Count = 0 Then Exit Sub
        Dim CurrentIndex = SearchFoundNodes.IndexOf(SearchCurrentNode)
        If CurrentIndex = SearchFoundNodes.Count - 1 Then
            CurrentIndex = -1
            LabelSearchMessage.Text = "Passed the last node"
            LabelSearchMessage.ForeColor = Color.DarkGreen
        Else
            LabelSearchMessage.Text = ""
        End If
        SearchCurrentNode = SearchFoundNodes(CurrentIndex + 1)
        CenterAtNode(SearchCurrentNode)
        HighlightNeighborhood(SearchCurrentNode, ComboBoxSearchHighlight.SelectedIndex)
        SearchCurrentNode.DisplayProperties.Highlighted = True
    End Sub

    Private Sub SearchPrevious()
        If SearchFoundNodes.Count = 0 Then Exit Sub
        Dim CurrentIndex = SearchFoundNodes.IndexOf(SearchCurrentNode)
        If CurrentIndex = 0 Then
            CurrentIndex = SearchFoundNodes.Count
            LabelSearchMessage.Text = "Passed the first node"
            LabelSearchMessage.ForeColor = Color.DarkGreen
        Else
            LabelSearchMessage.Text = ""
        End If
        SearchCurrentNode = SearchFoundNodes(CurrentIndex - 1)
        CenterAtNode(SearchCurrentNode)
        HighlightNeighborhood(SearchCurrentNode, ComboBoxSearchHighlight.SelectedIndex)
        SearchCurrentNode.DisplayProperties.Highlighted = True
    End Sub

    Private Sub ButtonSearchNext_Click(sender As Object, e As EventArgs) Handles ButtonSearchNext.Click
        SearchNext()
    End Sub

    Private Sub ButtonSearchPrevious_Click(sender As Object, e As EventArgs) Handles ButtonSearchPrevious.Click
        SearchPrevious()
    End Sub
End Class
