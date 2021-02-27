Module Hl
    Friend Enum ESortMode
        Attractors
    End Enum
    Friend Enum ERecentItemAction
        Open
        ImportXComResearch
    End Enum
    Friend Structure SRecentItem
        Public Action As ERecentItemAction
        Public FileName As String
        Public Description As String
    End Structure

    Public Graph As CGraph
    Public TemplateStorage As CTemplateStorage
    Public CurrentGraphFileName As String
    Public ChangesSaved As Boolean
    Friend RecentItems As List(Of SRecentItem)

    Friend SortMode As ESortMode

    Public Sub LoadSettings()
        Dim ViewerSettings = New YamlFileParser("settings.yml").Parse()

        If ViewerSettings.HasMapping("sort") AndAlso ViewerSettings.GetMapping("sort").HasMapping("attractors") Then
            AttractorPullPower = Val(ViewerSettings.GetMapping("sort").GetMapping("attractors").GetMapping("pullPower", "2.5").GetValue())
            AttractorPullConstant = Val(ViewerSettings.GetMapping("sort").GetMapping("attractors").GetMapping("pullConstant", "0.001").GetValue())
            AttractorPushPower = Val(ViewerSettings.GetMapping("sort").GetMapping("attractors").GetMapping("pushPower", "3").GetValue())
            AttractorPushConstant = Val(ViewerSettings.GetMapping("sort").GetMapping("attractors").GetMapping("pushConstant", "20000000").GetValue())
            AttractorClusterPower = Val(ViewerSettings.GetMapping("sort").GetMapping("attractors").GetMapping("clusterPower", "1.1").GetValue())
            AttractorClusterConstant = Val(ViewerSettings.GetMapping("sort").GetMapping("attractors").GetMapping("clusterConstant", "0.0001").GetValue())
        End If

        RecentItems = New List(Of SRecentItem)
        LoadRecentItems()

        If ViewerSettings.HasMapping("colors") Then
            Dim ColorSettings As YamlNode
            ColorSettings = ViewerSettings.GetMapping("colors")
            If ColorSettings.HasMapping("graph") Then
                Dim GraphColors = ColorSettings.GetMapping("graph")
                If GraphColors.HasMapping("background") Then
                    MainView.BackColor = ParseColor(GraphColors.GetMapping("background").GetValue())
                End If
                If GraphColors.HasMapping("text") Then
                    MainView.NodeTextBrush = New SolidBrush(ParseColor(GraphColors.GetMapping("text").GetValue()))
                End If
            End If
        End If
    End Sub
    Private Sub LoadRecentItems()
        Dim FileReader As System.IO.TextReader
        Try
            FileReader = System.IO.File.OpenText("recent.ini")
        Catch ex As System.IO.FileNotFoundException
            Exit Sub
        End Try
        Do
            Dim SavedAction = FileReader.ReadLine()
            If SavedAction Is Nothing Then Exit Do
            Dim SavedFileName = FileReader.ReadLine()
            If SavedFileName Is Nothing Then Exit Do
            AddRecentItem(SavedFileName, System.Enum.Parse(GetType(ERecentItemAction), SavedAction), False)
        Loop
        FileReader.Close()
    End Sub
    Private Sub SaveRecentItems()
        Using writer As IO.StreamWriter = IO.File.CreateText("recent.ini")
            For Each Item In RecentItems
                writer.WriteLine(Item.Action.ToString())
                writer.WriteLine(Item.FileName)
            Next Item
        End Using
    End Sub

    Public Sub Initialize()
        Graph = New CGraph
        TemplateStorage = New CTemplateStorage
        CurrentGraphFileName = ""
        ChangesSaved = True
    End Sub
    Friend Sub AddRecentItem(FileName As String, Action As ERecentItemAction, Optional Save As Boolean = True)
        For Each Item In RecentItems
            If Item.Action = Action And Item.FileName = FileName Then Exit Sub
        Next Item
        Dim NewRecentItem As SRecentItem
        NewRecentItem = New SRecentItem
        NewRecentItem.Action = Action
        NewRecentItem.FileName = FileName
        Select Case Action
            Case ERecentItemAction.ImportXComResearch
                NewRecentItem.Description = "Import XComResearch " + FileName
            Case ERecentItemAction.Open
                NewRecentItem.Description = "Open Graph " + FileName
        End Select
        RecentItems.Add(NewRecentItem)
        If Save Then SaveRecentItems()
    End Sub

    Private Function RemovePrefixesFromString(Source As String, Prefixes As List(Of String)) As String
        RemovePrefixesFromString = Source
        For Each Prefix In Prefixes
            If Source.StartsWith(Prefix) Then
                RemovePrefixesFromString = Mid(Source, Len(Prefix) + 1)
                Exit For
            End If
        Next Prefix
    End Function

    Private XComResearchConnectionTemplates As Dictionary(Of String, CPropertyTemplate)
    Public Sub ImportXComResearch(Path As String)
        AddRecentItem(Path, ERecentItemAction.ImportXComResearch)
        Dim ViewerSettings = New YamlFileParser("settings.yml").Parse()

        Dim RemovePrefixes As List(Of String) = New List(Of String)
        Dim SectionColors As YamlNode = Nothing
        Dim ConnectionColors As YamlNode = Nothing
        If Not ViewerSettings Is Nothing Then
            If ViewerSettings.HasMapping("qualityOfLife") Then
                Dim QualityOfLife As YamlNode
                QualityOfLife = ViewerSettings.GetMapping("qualityOfLife")
                If QualityOfLife.HasMapping("removePrefixes") Then
                    Dim RemovePrefixesSetting As YamlNode
                    RemovePrefixesSetting = QualityOfLife.GetMapping("removePrefixes")
                    For f = 0 To RemovePrefixesSetting.ItemCount - 1
                        RemovePrefixes.Add(RemovePrefixesSetting.GetItem(f).GetValue())
                    Next f
                End If
            End If
            If ViewerSettings.HasMapping("colors") Then
                Dim ColorSettings As YamlNode
                ColorSettings = ViewerSettings.GetMapping("colors")
                If ColorSettings.HasMapping("sections") Then
                    SectionColors = ColorSettings.GetMapping("sections")
                End If
                If ColorSettings.HasMapping("connections") Then
                    ConnectionColors = ColorSettings.GetMapping("connections")
                End If
            End If
        End If


        Hl.Initialize()
        Dim GameConfiguration As YamlNode
        Try
            GameConfiguration = New YamlFileParser(Path + "\User\options.cfg").Parse()
        Catch ex As Exception
            MsgBox("Game folder not recognized. Make sure options.cfg exists in User\", MsgBoxStyle.Exclamation, "Error during game mods parsing")
            Exit Sub
        End Try
        Dim Mods = GameConfiguration.GetMapping("mods")
        Dim ModsToLoad As New List(Of String)
        For f = 0 To Mods.ItemCount - 1
            Dim ModEntry = Mods.GetItem(f)
            If ModEntry.GetMapping("active").GetValue().ToLower() = "true" Then
                ModsToLoad.Add(ModEntry.GetMapping("id").GetValue())
            End If
        Next
        Dim AllResearch = XComResearchImport.LoadResearch(Path, ModsToLoad)

        Dim CoordsRandomizer As Random = New Random

        XComResearchConnectionTemplates = New Dictionary(Of String, CPropertyTemplate)
        If Not ConnectionColors Is Nothing Then
            For Each ConnectionType In ConnectionColors.GetMappingKeys()
                Dim ConnectionTemplate As CPropertyTemplate
                ConnectionTemplate = New CPropertyTemplate
                ConnectionTemplate.Color = Color.Blue
                ConnectionTemplate.Size = 1
                ConnectionTemplate.Color = ParseColor(ConnectionColors.GetMapping(ConnectionType).GetValue())
                XComResearchConnectionTemplates.Add(ConnectionType, ConnectionTemplate)
            Next
        End If


        Dim NodeTemplate As CPropertyTemplate
        NodeTemplate = New CPropertyTemplate
        NodeTemplate.Size = 15
        NodeTemplate.Color = Color.Black
        Dim NameProperty As CPropertyTemplate.SProperty
        NameProperty.DefaultValue = ""
        NameProperty.Display = True
        NameProperty.Name = "Name"
        NodeTemplate.Properties.Add(NameProperty)
        Dim LastCreatedNode As CGraph.CNode = Nothing
        For Each Research In AllResearch
            LastCreatedNode = Graph.Add(NodeTemplate)
            LastCreatedNode.SetProperty("RawName", Research.Name)
            LastCreatedNode.SetProperty("Name", RemovePrefixesFromString(Research.Name, RemovePrefixes))
            LastCreatedNode.DisplayProperties.Position.X = CoordsRandomizer.NextDouble() * 500 - 250
            LastCreatedNode.DisplayProperties.Position.Y = CoordsRandomizer.NextDouble() * 500 - 250

            LastCreatedNode.SetProperty("ListOrder", Research.ListOrder)
            'LastCreatedNode.DisplayProperties.ShownProperties.Add("ListOrder")

            LastCreatedNode.SetProperty("RawSection", Research.Section)
            LastCreatedNode.SetProperty("Section", RemovePrefixesFromString(Research.Section, RemovePrefixes))
            'LastCreatedNode.DisplayProperties.ShownProperties.Add("Section")

            If Research.Planet <> "" Then
                LastCreatedNode.SetProperty("Planet", Research.Planet)
                'LastCreatedNode.DisplayProperties.ShownProperties.Add("Planet")
                Dim Result As Boolean
                Result = CreatePlanetNodeOrConnect(Research.Planet, LastCreatedNode)
            End If

            LastCreatedNode.SetProperty("Cost", Research.Cost)
            If Research.Cost > 0 Then LastCreatedNode.DisplayProperties.ShownProperties.Add("Cost")

            If Research.Item Then
                LastCreatedNode.SetProperty("NeedsItem", "Needs item")
                LastCreatedNode.DisplayProperties.ShownProperties.Add("NeedsItem")
            End If
        Next

        REM Now we do all node connections
        For Each Research In AllResearch
            For Each ConnectionType In XComResearchConnectionTemplates.Keys
                Dim Connections As List(Of String)
                Dim Reversed As Boolean
                Select Case ConnectionType
                    Case "lookup" : Connections = New List(Of String) From {Research.Lookup} : Reversed = False
                    Case "dependencies" : Connections = Research.Dependencies : Reversed = True
                    Case "requires" : Connections = Research.Requires : Reversed = True
                    Case "getOneFree" : Connections = Research.GetOneFree : Reversed = False
                    Case "getOneFreeProtected" : Connections = Research.GetOneFreeProtected : Reversed = False
                    Case "sequentialGetOneFree" : Connections = Research.SequentialGetOneFree : Reversed = False
                    Case "unlocks" : Connections = Research.Unlocks : Reversed = False
                End Select

                CreateConnections(Research, Connections, XComResearchConnectionTemplates(ConnectionType), Reversed)
            Next
        Next


        REM Now colorize

        For Each Node As CGraph.CNode In Graph
            Dim Connections = Graph.GetConnections(Node.Index)
            Dim IncomingConnections As Integer = 0
            For c = 0 To Connections.Length() - 1
                If Connections(c).NodeB = Node.Index Then
                    IncomingConnections += 1
                End If
            Next c

            If IncomingConnections = 0 Then
                Node.DisplayProperties.Decoration = CGraph.EDecoration.Square
            End If

            If Not SectionColors Is Nothing Then
                If Not Node.GetProperty("RawSection") Is Nothing AndAlso SectionColors.HasMapping(Node.GetProperty("RawSection")) Then
                    Node.DisplayProperties.Color = ParseColor(SectionColors.GetMapping(Node.GetProperty("RawSection")).GetValue())
                End If
            End If
        Next Node

        Dim AngleRandomizer As Random = New Random
        For f = 20 To 1 Step -1
            For Each Node As CGraph.CNode In Graph
                Dim Connections = Graph.GetConnections(Node.Index)
                Dim ConnectionCount As Integer = 0
                Dim LongestConnectionIndex As Integer
                Dim LongestConnectionNodeIndex As Integer
                Dim MaxConnectionDistance As Single = 0

                For c = 0 To Connections.Length() - 1
                    If Connections(c).NodeA = Node.Index Then
                        ConnectionCount += 1
                        Dim OtherNode = Connections(c).NodeB
                        Dim ConnectionDistance As Single = (Graph.GetNode(OtherNode).DisplayProperties.Position.X - Node.DisplayProperties.Position.X) * (Graph.GetNode(OtherNode).DisplayProperties.Position.X - Node.DisplayProperties.Position.X) + (Graph.GetNode(OtherNode).DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y) * (Graph.GetNode(OtherNode).DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y)
                        If ConnectionDistance > MaxConnectionDistance Then
                            LongestConnectionIndex = c
                            LongestConnectionNodeIndex = OtherNode
                            MaxConnectionDistance = ConnectionDistance
                        End If
                    End If
                    If Connections(c).NodeB = Node.Index Then
                        ConnectionCount += 1
                        Dim OtherNode = Connections(c).NodeA
                        Dim ConnectionDistance As Single = (Graph.GetNode(OtherNode).DisplayProperties.Position.X - Node.DisplayProperties.Position.X) * (Graph.GetNode(OtherNode).DisplayProperties.Position.X - Node.DisplayProperties.Position.X) + (Graph.GetNode(OtherNode).DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y) * (Graph.GetNode(OtherNode).DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y)
                        If ConnectionDistance > MaxConnectionDistance Then
                            LongestConnectionIndex = c
                            LongestConnectionNodeIndex = OtherNode
                            MaxConnectionDistance = ConnectionDistance
                        End If
                    End If
                Next c

                If ConnectionCount <= f Then
                    Dim AnchorPosition = Graph.Nodes(LongestConnectionNodeIndex).DisplayProperties.Position
                    Dim Angle As Single
                    Angle = Math.Atan2(Node.DisplayProperties.Position.Y - AnchorPosition.Y, Node.DisplayProperties.Position.X - AnchorPosition.X) + AngleRandomizer.NextDouble() * 0.1 - 0.05
                    Node.DisplayProperties.Position.X = AnchorPosition.X + Math.Cos(Angle) * 20
                    Node.DisplayProperties.Position.Y = AnchorPosition.Y + Math.Sin(Angle) * 20
                End If

            Next Node
        Next f

        Hl.SortMode = ESortMode.Attractors

        Dim InitialSortIterations As Integer = 100
        If ViewerSettings.HasMapping("sort") Then
            InitialSortIterations = Val(ViewerSettings.GetMapping("sort").GetMapping("initialIterations", Str(InitialSortIterations)).GetValue())
        End If
        LocalForceLimit = 500
        For f = 0 To InitialSortIterations
            ProcessSort()
        Next
        LocalForceLimit = 50
        MainView.SortTimer.Enabled = True
    End Sub

    Private Function ParseColor(ColorDescription As String) As Color
        Try
            ParseColor = ColorTranslator.FromHtml(ColorDescription)
        Catch ex As Exception
            ParseColor = Color.FromName(ColorDescription)
        End Try
    End Function

    Private Sub CreateConnections(Research As CXComResearch, Connections As List(Of String), Template As CPropertyTemplate, ReverseDirection As Boolean)
        For Each targetNodeName In Connections
            If targetNodeName = "" Then Continue For
            Dim OriginNode As CGraph.CNode
            Dim TargetNode As CGraph.CNode
            Dim NodeA, NodeB As Integer
            OriginNode = Nothing
            TargetNode = Nothing
            For Each Node As CGraph.CNode In Graph
                If Node.GetProperty("RawName") = Research.Name Then
                    OriginNode = Node
                End If
                If Node.GetProperty("RawName") = targetNodeName Then
                    TargetNode = Node
                End If
            Next Node
            If TargetNode Is Nothing Then GoTo ZaTo
            If ReverseDirection Then
                NodeA = TargetNode.Index
                NodeB = OriginNode.Index
            Else
                NodeA = OriginNode.Index
                NodeB = TargetNode.Index
            End If
            For c = 0 To UBound(Graph.Connections)
                If Graph.Connections(c).NodeA = NodeA And Graph.Connections(c).NodeB = NodeB Then GoTo ZaTo
            Next c
            Graph.Connect(NodeA, NodeB, Template)
ZaTo:
        Next
    End Sub

    Public Sub ImportTechResearchGraph(FileName As String)
        Dim CoordsRandomizer As Random = New Random
        Dim Reader As System.IO.TextReader
        Reader = System.IO.File.OpenText(FileName)

        Dim ConnectionTemplate As CPropertyTemplate
        ConnectionTemplate = New CPropertyTemplate
        ConnectionTemplate.Color = Color.Blue
        ConnectionTemplate.Size = 1
        Dim ConnectionTemplateRequirement As CPropertyTemplate
        ConnectionTemplateRequirement = New CPropertyTemplate
        ConnectionTemplateRequirement.Color = Color.Red
        ConnectionTemplateRequirement.Size = 1
        Dim ConnectionTemplateLookup As CPropertyTemplate
        ConnectionTemplateLookup = New CPropertyTemplate
        ConnectionTemplateLookup.Color = Color.LightCyan
        ConnectionTemplateLookup.Size = 1
        Dim ConnectionTemplateFree As CPropertyTemplate
        ConnectionTemplateFree = New CPropertyTemplate
        ConnectionTemplateFree.Color = Color.Green
        ConnectionTemplateFree.Size = 1
        Dim NodeTemplate As CPropertyTemplate
        NodeTemplate = New CPropertyTemplate
        NodeTemplate.Size = 15
        NodeTemplate.Color = Color.Black
        Dim NameProperty As CPropertyTemplate.SProperty
        NameProperty.DefaultValue = ""
        NameProperty.Display = True
        NameProperty.Name = "Name"
        NodeTemplate.Properties.Add(NameProperty)
        Dim WholeTree(-1) As String
        Dim Line As String
        Do
            Line = Reader.ReadLine()
            If Line Is Nothing Then Exit Do
            ReDim Preserve WholeTree(UBound(WholeTree) + 1)
            WholeTree(UBound(WholeTree)) = Line
        Loop
        Dim TreeChanged As Boolean
        Do
            TreeChanged = False
            Dim LastCreatedNode As CGraph.CNode = Nothing
            Dim NodeContext As CGraph.CNode = Nothing
            For f = 0 To UBound(WholeTree)
                Dim ParsedLine() As String
                Dim Command As String
                Dim Value As String
                ParsedLine = WholeTree(f).Split(":")
                Command = ParsedLine(0)
                Value = ParsedLine(1)
                Select Case Command
                    Case "RES"
                        Dim CreateNode As Boolean
                        CreateNode = True
                        For Each Node As CGraph.CNode In Graph
                            If Node.GetProperty("Name") = Value Then
                                NodeContext = Node
                                LastCreatedNode = Nothing
                                CreateNode = False
                                Exit For
                            End If
                        Next Node
                        If CreateNode Then
                            LastCreatedNode = Graph.Add(NodeTemplate)
                            LastCreatedNode.SetProperty("Name", Value)
                            LastCreatedNode.DisplayProperties.Position.X = CoordsRandomizer.Next(-2500, 2500)
                            LastCreatedNode.DisplayProperties.Position.Y = CoordsRandomizer.Next(-2500, 2500)
                            NodeContext = LastCreatedNode
                            TreeChanged = True
                        End If
                    Case "LIST_ORDER"
                        If Not LastCreatedNode Is Nothing Then
                            LastCreatedNode.SetProperty("ListOrder", Value)
                            'LastCreatedNode.DisplayProperties.ShownProperties.Add("ListOrder")
                        End If
                    Case "SECTION"
                        If Not LastCreatedNode Is Nothing Then
                            LastCreatedNode.SetProperty("Section", Value)
                            'LastCreatedNode.DisplayProperties.ShownProperties.Add("Section")
                        End If
                    Case "PLANET"
                        If Not LastCreatedNode Is Nothing Then
                            LastCreatedNode.SetProperty("Planet", Value)
                            'LastCreatedNode.DisplayProperties.ShownProperties.Add("Planet")
                            Dim Result As Boolean
                            Result = CreatePlanetNodeOrConnect(Value, LastCreatedNode)
                            If Not Result Then TreeChanged = True
                        End If
                    Case "COST"
                        If Not LastCreatedNode Is Nothing Then
                            LastCreatedNode.SetProperty("Cost", Value)
                            If Value > 0 Then LastCreatedNode.DisplayProperties.ShownProperties.Add("Cost")
                        End If
                    Case "ITEM"
                        If Not LastCreatedNode Is Nothing And Value = "true" Then
                            LastCreatedNode.SetProperty("NeedsItem", "Needs item")
                            LastCreatedNode.DisplayProperties.ShownProperties.Add("NeedsItem")
                        End If
                    Case "DEP", "REQ", "LOOKUP", "FREE"
                        If Not NodeContext Is Nothing Then
                            Dim ConnectNode As CGraph.CNode
                            Dim NodeA, NodeB As Integer
                            Dim Template As CPropertyTemplate = Nothing
                            ConnectNode = Nothing
                            For Each Node As CGraph.CNode In Graph
                                If Node.GetProperty("Name") = Value Then
                                    ConnectNode = Node
                                    Exit For
                                End If
                            Next Node

                            If Not ConnectNode Is Nothing Then
                                Select Case Command
                                    Case "DEP"
                                        NodeA = ConnectNode.Index
                                        NodeB = NodeContext.Index
                                        Template = ConnectionTemplate
                                    Case "REQ"
                                        NodeA = ConnectNode.Index
                                        NodeB = NodeContext.Index
                                        Template = ConnectionTemplateRequirement
                                    Case "LOOKUP"
                                        NodeA = NodeContext.Index
                                        NodeB = ConnectNode.Index
                                        Template = ConnectionTemplateLookup
                                    Case "FREE"
                                        NodeA = NodeContext.Index
                                        NodeB = ConnectNode.Index
                                        Template = ConnectionTemplateFree
                                End Select
                                For c = 0 To UBound(Graph.Connections)
                                    If Graph.Connections(c).NodeA = NodeA And Graph.Connections(c).NodeB = NodeB Then GoTo ZaTo
                                Next c
                                Graph.Connect(NodeA, NodeB, Template)
                                TreeChanged = True
ZaTo:
                            End If
                        End If
                End Select
            Next f
        Loop While TreeChanged = True

        For Each Node As CGraph.CNode In Graph
            Dim Connections = Graph.GetConnections(Node.Index)
            Dim IncomingConnections As Integer = 0
            For c = 0 To Connections.Length() - 1
                If Connections(c).NodeB = Node.Index Then
                    IncomingConnections += 1
                End If
            Next c

            If IncomingConnections = 0 Then
                Node.DisplayProperties.Decoration = CGraph.EDecoration.Square
            End If

            Select Case Node.GetProperty("Section")
                Case "NOT_AVAILABLE"
                    Node.DisplayProperties.Color = Color.Red
                Case "ALIEN_ARTIFACTS"
                    Node.DisplayProperties.Color = Color.CornflowerBlue
                Case "NATURAL_SCIENCE"
                    Node.DisplayProperties.Color = Color.ForestGreen
                Case "STR_CAPTAIN_LOG"
                    Node.DisplayProperties.Color = Color.Lavender
                Case "ALIEN_LIFE_FORMS"
                    Node.DisplayProperties.Color = Color.GreenYellow
                Case "ALIEN_RESEARCH_UC"
                    Node.DisplayProperties.Color = Color.Yellow
                Case "BASE_FACILITIES"
                    Node.DisplayProperties.Color = Color.Beige
                Case "BASIC_RESEARCH"
                    Node.DisplayProperties.Color = Color.FloralWhite
                Case "HEAVY_WEAPONS_PLATFORMS"
                    Node.DisplayProperties.Color = Color.Crimson
                Case "UFOS"
                    Node.DisplayProperties.Color = Color.SkyBlue
                Case "UFO_COMPONENTS"
                    Node.DisplayProperties.Color = Color.Olive
                Case "UNDEFINED"
                    Node.DisplayProperties.Color = Color.Gray
                Case "WEAPONS_AND_EQUIPMENT"
                    Node.DisplayProperties.Color = Color.Brown
                Case "XCOM_CRAFT_ARMAMENT"
                    Node.DisplayProperties.Color = Color.Purple
            End Select
        Next Node
        Reader.Close()
    End Sub
    Private Function CreatePlanetNodeOrConnect(Planet As String, TechNode As CGraph.CNode)
        CreatePlanetNodeOrConnect = True
        Dim ConnectNode As CGraph.CNode
        Dim NodeA, NodeB As Integer
        ConnectNode = Nothing
        If Planet = "" Then
            Exit Function
        End If
        For Each Node As CGraph.CNode In Graph
            If Node.GetProperty("PlanetName") = Planet Then
                ConnectNode = Node
                Exit For
            End If
        Next Node

        If ConnectNode Is Nothing Then
            Dim CoordsRandomizer As Random = New Random
            Dim NodeTemplate As CPropertyTemplate
            NodeTemplate = New CPropertyTemplate
            NodeTemplate.Size = 30
            NodeTemplate.Color = Color.Gray
            Dim NameProperty As CPropertyTemplate.SProperty
            NameProperty.DefaultValue = ""
            NameProperty.Display = True
            NameProperty.Name = "PlanetName"
            NodeTemplate.Properties.Add(NameProperty)

            ConnectNode = Graph.Add(NodeTemplate)
            ConnectNode.SetProperty("PlanetName", Planet)
            ConnectNode.DisplayProperties.Position.X = CoordsRandomizer.Next(-2500, 2500)
            ConnectNode.DisplayProperties.Position.Y = CoordsRandomizer.Next(-2500, 2500)
            CreatePlanetNodeOrConnect = False
        End If
        Dim ConnectionTemplate As CPropertyTemplate
        ConnectionTemplate = New CPropertyTemplate
        ConnectionTemplate.Color = Color.LightGray
        ConnectionTemplate.Size = 1
        Graph.Connect(ConnectNode.Index, TechNode.Index, ConnectionTemplate)
    End Function

    Public Function FindDistanceToSegment(Cursor As PointF, SegmentA As PointF, SegmentB As PointF) As Double
        Dim dx = SegmentB.X - SegmentA.X
        Dim dy = SegmentB.Y - SegmentA.Y
        If ((dx = 0) And (dy = 0)) Then
            ' It's a point not a line segment.
            dx = Cursor.X - SegmentA.X
            dy = Cursor.Y - SegmentA.Y
            Return Math.Sqrt(dx * dx + dy * dy)
        End If

        ' Calculate the t that minimizes the distance.
        Dim t = ((Cursor.X - SegmentA.X) * dx + (Cursor.Y - SegmentA.Y) * dy) / (dx * dx + dy * dy)

        ' See if this represents one of the segment's
        ' end points Or a point in the middle.
        If (t < 0) Then
            dx = Cursor.X - SegmentA.X
            dy = Cursor.Y - SegmentA.Y
        ElseIf (t > 1) Then
            dx = Cursor.X - SegmentB.X
            dy = Cursor.Y - SegmentB.Y
        Else
            Dim closest = New PointF(SegmentA.X + t * dx, SegmentA.Y + t * dy)
            dx = Cursor.X - closest.X
            dy = Cursor.Y - closest.Y
        End If
        Return Math.Sqrt(dx * dx + dy * dy)
    End Function

    Private AttractorPullPower = 2.5
    Private AttractorPullConstant = 0.001
    Private AttractorPushPower = 3
    Private AttractorPushConstant = 20000000
    Private AttractorClusterPower = 1.1
    Private AttractorClusterConstant = 0.0001
    Private LocalForceLimit = 50
    Private Sub SortAttractors()
        Dim Force() As PointF
        ReDim Force(Graph.Nodes.Count - 1)
        For Each Node As CGraph.CNode In Graph
            Force(Node.Index) = New PointF(0, 0)
            Dim Connections = Graph.GetConnections(Node.Index)
            For Each Connection In Connections
                If Connection.NodeA = Connection.NodeB Then Continue For
                Dim OtherNode As Integer
                If Connection.NodeA = Node.Index Then
                    OtherNode = Connection.NodeB
                Else
                    OtherNode = Connection.NodeA
                End If
                Dim RawDistance As Double = Math.Max(Math.Sqrt((Graph.GetNode(OtherNode).DisplayProperties.Position.X - Node.DisplayProperties.Position.X) * (Graph.GetNode(OtherNode).DisplayProperties.Position.X - Node.DisplayProperties.Position.X) + (Graph.GetNode(OtherNode).DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y) * (Graph.GetNode(OtherNode).DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y)), 0.1)
                Dim Distance As Double = Math.Max(1, RawDistance - Node.DisplayProperties.Size - Graph.GetNode(OtherNode).DisplayProperties.Size)
                Dim LocalForce As PointF
                Dim SortWeight As Double = 1
                If Not Connection.GetProperty("SortWeight") Is Nothing Then
                    Dim Weight As Double = Val(Connection.GetProperty("SortWeight"))
                    SortWeight = (Weight / 100.0)
                End If

                LocalForce.X = Math.Pow(Distance, AttractorPullPower * SortWeight) * AttractorPullConstant * ((Graph.GetNode(OtherNode).DisplayProperties.Position.X - Node.DisplayProperties.Position.X) / RawDistance)
                LocalForce.Y = Math.Pow(Distance, AttractorPullPower * SortWeight) * AttractorPullConstant * ((Graph.GetNode(OtherNode).DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y) / RawDistance)
                Dim LocalForceMagnitude = Math.Sqrt(LocalForce.X * LocalForce.X + LocalForce.Y * LocalForce.Y)


                If LocalForceMagnitude > LocalForceLimit Then
                    LocalForce.X = (LocalForce.X / LocalForceMagnitude) * LocalForceLimit
                    LocalForce.Y = (LocalForce.Y / LocalForceMagnitude) * LocalForceLimit
                End If
                Force(Node.Index).X += LocalForce.X
                Force(Node.Index).Y += LocalForce.Y
            Next Connection

            For Each RepulsiveNode As CGraph.CNode In Graph
                If RepulsiveNode.Index <> Node.Index Then
                    Dim RawDistance As Double = Math.Max(Math.Sqrt((RepulsiveNode.DisplayProperties.Position.X - Node.DisplayProperties.Position.X) * (RepulsiveNode.DisplayProperties.Position.X - Node.DisplayProperties.Position.X) + (RepulsiveNode.DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y) * (RepulsiveNode.DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y)), 0.1)
                    Dim Distance As Double = Math.Max(1, RawDistance - Node.DisplayProperties.Size - RepulsiveNode.DisplayProperties.Size)
                    Dim LocalForce As PointF
                    LocalForce.X = (AttractorPushConstant / Math.Pow(Distance, AttractorPushPower)) * ((RepulsiveNode.DisplayProperties.Position.X - Node.DisplayProperties.Position.X) / RawDistance)
                    LocalForce.Y = (AttractorPushConstant / Math.Pow(Distance, AttractorPushPower)) * ((RepulsiveNode.DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y) / RawDistance)
                    Dim LocalForceMagnitude = Math.Sqrt(LocalForce.X * LocalForce.X + LocalForce.Y * LocalForce.Y)
                    If LocalForceMagnitude > LocalForceLimit Then
                        LocalForce.X = (LocalForce.X / LocalForceMagnitude) * LocalForceLimit
                        LocalForce.Y = (LocalForce.Y / LocalForceMagnitude) * LocalForceLimit
                    End If
                    Force(Node.Index).X -= LocalForce.X
                    Force(Node.Index).Y -= LocalForce.Y
                End If
            Next

            For Each OtherClusterNode As CGraph.CNode In Graph
                If OtherClusterNode.Cluster <> Node.Cluster Then
                    Dim RawDistance As Double = Math.Max(Math.Sqrt((OtherClusterNode.DisplayProperties.Position.X - Node.DisplayProperties.Position.X) * (OtherClusterNode.DisplayProperties.Position.X - Node.DisplayProperties.Position.X) + (OtherClusterNode.DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y) * (OtherClusterNode.DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y)), 0.1)
                    Dim Distance As Double = Math.Max(1, RawDistance - Node.DisplayProperties.Size - OtherClusterNode.DisplayProperties.Size)
                    Dim LocalForce As PointF
                    LocalForce.X = (Math.Pow(Distance, AttractorClusterPower) * AttractorClusterConstant) * ((Node.DisplayProperties.Position.X - OtherClusterNode.DisplayProperties.Position.X) / RawDistance)
                    LocalForce.Y = (Math.Pow(Distance, AttractorClusterPower) * AttractorClusterConstant) * ((Node.DisplayProperties.Position.Y - OtherClusterNode.DisplayProperties.Position.Y) / RawDistance)
                    Dim LocalForceMagnitude = Math.Sqrt(LocalForce.X * LocalForce.X + LocalForce.Y * LocalForce.Y)
                    If LocalForceMagnitude > LocalForceLimit Then
                        LocalForce.X = (LocalForce.X / LocalForceMagnitude) * LocalForceLimit
                        LocalForce.Y = (LocalForce.Y / LocalForceMagnitude) * LocalForceLimit
                    End If
                    Force(Node.Index).X -= LocalForce.X
                    Force(Node.Index).Y -= LocalForce.Y
                End If
            Next
        Next Node
        For f = 0 To UBound(Force)
            Dim Node = Graph.GetNode(f)
            Node.DisplayProperties.Position.X += Force(f).X / Node.DisplayProperties.Size
            Node.DisplayProperties.Position.Y += Force(f).Y / Node.DisplayProperties.Size
        Next f
    End Sub

    Friend Sub ProcessSort()
        Select Case Hl.SortMode
            Case ESortMode.Attractors
                Call SortAttractors()

        End Select
    End Sub
End Module
