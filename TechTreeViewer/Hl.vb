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
        Dim ViewerSettings
        Try
            ViewerSettings = New YamlFileParser("settings.yml").Parse()
        Catch ex As IO.FileNotFoundException
            MsgBox("Settings file not found. Make sure settings.yml is present in path.", MsgBoxStyle.Critical, "Can't continue")
            End
        End Try
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
                If GraphColors.HasMapping("darkenedAlpha") Then
                    MainView.DarkenedAlpha = Val(GraphColors.GetMapping("darkenedAlpha").GetValue())
                End If
                If GraphColors.HasMapping("background") Then
                    MainView.BackColor = ParseColor(GraphColors.GetMapping("background").GetValue())
                End If
                If GraphColors.HasMapping("text") Then
                    Dim TextColor = ParseColor(GraphColors.GetMapping("text").GetValue())
                    MainView.NodeTextBrush = New SolidBrush(TextColor)
                    MainView.NodeDarkenedTextBrush = New SolidBrush(Color.FromArgb(MainView.DarkenedAlpha, TextColor.R, TextColor.G, TextColor.B))
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
        For f = 0 To RecentItems.Count - 1
            Dim Item = RecentItems(f)
            If Item.Action = Action And Item.FileName = FileName Then
                If (f <> 0) Then
                    RecentItems.RemoveAt(f)
                    RecentItems.Insert(0, Item)
                End If
                If Save Then SaveRecentItems()
                Exit Sub
            End If
        Next f
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
        Dim UserFolderPath As String = Path + "\User"
        Try
            GameConfiguration = New YamlFileParser(UserFolderPath + "\options.cfg").Parse()
        Catch ex As Exception
            Dim WindowsOXCEPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\OpenXCom"
            Try
                GameConfiguration = New YamlFileParser(WindowsOXCEPath + "\options.cfg").Parse()
                UserFolderPath = WindowsOXCEPath
            Catch ex2 As Exception
                MsgBox("Game folder not recognized. Make sure options.cfg exists in User\ or in My Documents", MsgBoxStyle.Exclamation, "Error during game mods parsing")
                Exit Sub
            End Try
        End Try
        Dim Mods = GameConfiguration.GetMapping("mods")
        Dim ModsToLoad As New List(Of String)
        For f = 0 To Mods.ItemCount - 1
            Dim ModEntry = Mods.GetItem(f)
            If ModEntry.GetMapping("active").GetValue().ToLower() = "true" Then
                ModsToLoad.Add(ModEntry.GetMapping("id").GetValue())
            End If
        Next
        ImportProgress.Show(MainView)
        Dim AllResearch = XComResearchImport.LoadResearch(Path, UserFolderPath, ModsToLoad)

        Dim CoordsRandomizer As Random = New Random
        ImportProgress.SetProgressBar(AllResearch.Count, "Building graph nodes")

        XComResearchConnectionTemplates = New Dictionary(Of String, CPropertyTemplate)
        If Not ConnectionColors Is Nothing Then
            For Each ConnectionType In ConnectionColors.GetMappingKeys()
                Dim ConnectionTemplate As CPropertyTemplate
                ConnectionTemplate = New CPropertyTemplate
                ConnectionTemplate.Color = Color.Blue
                ConnectionTemplate.Size = 1
                ConnectionTemplate.Color = ParseColor(ConnectionColors.GetMapping(ConnectionType).GetValue())
                If ConnectionType = "unlocks" Then
                    Dim DefaultNameProperty = New CPropertyTemplate.SProperty()
                    DefaultNameProperty.Name = "Type"
                    DefaultNameProperty.DefaultValue = "U"
                    DefaultNameProperty.Display = True
                    ConnectionTemplate.Properties.Add(DefaultNameProperty)
                End If
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
            LastCreatedNode.DisplayProperties.Position.X = CoordsRandomizer.NextDouble() * AllResearch.Count * 2 - AllResearch.Count
            LastCreatedNode.DisplayProperties.Position.Y = CoordsRandomizer.NextDouble() * AllResearch.Count * 2 - AllResearch.Count

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
            ImportProgress.ImportProgressBar.PerformStep()
        Next Research

        REM Now we do all node connections
        ImportProgress.SetProgressBar(AllResearch.Count, "Building graph connections")
        For Each ConnectionType In XComResearchConnectionTemplates.Keys
            For Each Research In AllResearch
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
            Next Research
            ImportProgress.ImportProgressBar.PerformStep()
        Next ConnectionType


        REM Now colorize
        ImportProgress.SetProgressBar(Graph.Nodes.Count, "Adding colors")
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
            ImportProgress.ImportProgressBar.PerformStep()
        Next Node

        ImportProgress.SetProgressBar(Graph.Nodes.Count * 20, "Pre-sorting graph")
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

                ImportProgress.ImportProgressBar.PerformStep()
            Next Node
        Next f

        Graph.RecalculateClusters()
        Hl.SortMode = ESortMode.Attractors

        Dim InitialSortIterations As Integer = 100
        Dim AutostartSort As Boolean = True
        If ViewerSettings.HasMapping("sort") Then
            InitialSortIterations = Val(ViewerSettings.GetMapping("sort").GetMapping("initialIterations", Str(InitialSortIterations)).GetValue())
            AutostartSort = (ViewerSettings.GetMapping("sort").GetMapping("autostartRealtimeSort", "true").GetValue().ToLower() = "true")
        End If
        LocalForceLimit = 50
        ImportProgress.SetProgressBar(InitialSortIterations, "Sorting graph")
        For f = 0 To InitialSortIterations
            Debug.WriteLine("Sort iteration: " + Str(f))
            ProcessSort()
            ImportProgress.ImportProgressBar.PerformStep()
        Next
        LocalForceLimit = 50
        MainView.SortTimer.Enabled = AutostartSort
        ImportProgress.Close()
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
                If Graph.Connections(c).NodeA = NodeA And Graph.Connections(c).NodeB = NodeB Then
                    Graph.Connections(c).ApplyTemplateProperties(Template)
                    GoTo ZaTo
                End If
            Next c
            Graph.Connect(NodeA, NodeB, Template, False)
ZaTo:
        Next
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

    Private AttractorPullPower As Single = 2.5
    Private AttractorPullConstant As Single = 0.001
    Private AttractorPushPower As Single = 3
    Private AttractorPushConstant As Single = 20000000
    Private AttractorClusterPower As Single = 1.1
    Private AttractorClusterConstant As Single = 0.0001
    Private LocalForceLimit As Single = 50
    Private Sub SortAttractors()
        Dim Force() As PointF
        ReDim Force(Graph.Nodes.Count - 1)
#If DEBUG Then
        Dim Stopwatch As Stopwatch = Stopwatch.StartNew()
#End If
        Dim ConnectionsTime As Long = 0
        Dim RepulsiveTime As Long = 0
        Dim ClusterTime As Long = 0
        For f = 0 To Graph.Nodes.Count - 1
            Force(f) = New PointF(0, 0)
        Next f

        Parallel.ForEach(Of CGraph.CNode)(Graph.Nodes, Sub(Node As CGraph.CNode, LoopState As ParallelLoopState, Parameter As Long)
#If DEBUG Then
                                                           Dim ParallelStopwatch As Stopwatch
                                                           ParallelStopwatch = Stopwatch.StartNew()
#End If
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
                                                                   Dim Weight As Double = Math.Min(200, Val(Connection.GetProperty("SortWeight")))
                                                                   SortWeight = (Weight / 100.0)
                                                               End If
                                                               Dim ForceMagnitude = Math.Pow(Distance, AttractorPullPower * SortWeight) * AttractorPullConstant
                                                               LocalForce.X = ForceMagnitude * ((Graph.GetNode(OtherNode).DisplayProperties.Position.X - Node.DisplayProperties.Position.X) / RawDistance)
                                                               LocalForce.Y = ForceMagnitude * ((Graph.GetNode(OtherNode).DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y) / RawDistance)

                                                               If ForceMagnitude > LocalForceLimit Then
                                                                   LocalForce.X = (LocalForce.X / ForceMagnitude) * LocalForceLimit
                                                                   LocalForce.Y = (LocalForce.Y / ForceMagnitude) * LocalForceLimit
                                                               End If
                                                               SyncLock Force
                                                                   Force(Node.Index).X += LocalForce.X
                                                                   Force(Node.Index).Y += LocalForce.Y
                                                               End SyncLock
                                                           Next Connection
#If DEBUG Then
                                                           ParallelStopwatch.Stop()
                                                           System.Threading.Interlocked.Add(ConnectionsTime, ParallelStopwatch.ElapsedMilliseconds)

                                                           ParallelStopwatch = New Stopwatch()
                                                           ParallelStopwatch.Start()
#End If
                                                           Dim RepulsiveLocalForce As PointF
                                                           Dim RepulsiveNodeIndex As Integer
                                                           For RepulsiveNodeIndex = Node.Index + 1 To Graph.Nodes.Count - 1
                                                               Dim RepulsiveNode As CGraph.CNode = Graph.Nodes(RepulsiveNodeIndex)
                                                               Dim DeltaX = RepulsiveNode.DisplayProperties.Position.X - Node.DisplayProperties.Position.X
                                                               Dim DeltaY = RepulsiveNode.DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y
                                                               Dim RawDistanceSquared = DeltaX * DeltaX + DeltaY * DeltaY
                                                               If RawDistanceSquared <= 271 * 271 Then
                                                                   Dim RawDistance As Single = Math.Max(Math.Sqrt(RawDistanceSquared), 0.1)
                                                                   Dim Distance As Single = Math.Max(1, RawDistance - Node.DisplayProperties.Size - RepulsiveNode.DisplayProperties.Size)
                                                                   Dim ForceMagnitude As Single
                                                                   ForceMagnitude = Math.Min(LocalForceLimit, (AttractorPushConstant / (Distance * Distance * Distance)))
                                                                   Dim TempForceX As Single = ForceMagnitude * (DeltaX / RawDistance)
                                                                   Dim TempForceY As Single = ForceMagnitude * (DeltaY / RawDistance)
                                                                   RepulsiveLocalForce.X += TempForceX
                                                                   RepulsiveLocalForce.Y += TempForceY
                                                                   SyncLock Force
                                                                       Force(RepulsiveNodeIndex).X += TempForceX
                                                                       Force(RepulsiveNodeIndex).Y += TempForceY
                                                                   End SyncLock
                                                               End If
                                                           Next RepulsiveNodeIndex
                                                           SyncLock Force
                                                               Force(Node.Index).X -= RepulsiveLocalForce.X
                                                               Force(Node.Index).Y -= RepulsiveLocalForce.Y
                                                           End SyncLock
#If DEBUG Then
                                                           ParallelStopwatch.Stop()
                                                           System.Threading.Interlocked.Add(RepulsiveTime, ParallelStopwatch.ElapsedMilliseconds)

                                                           ParallelStopwatch = Stopwatch.StartNew()
#End If
                                                           If Graph.Nodes.Count < 1000 Then
                                                               Dim OtherClusterNodeIndex As Integer
                                                               For OtherClusterNodeIndex = Node.Index + 1 To Graph.Nodes.Count - 1
                                                                   Dim OtherClusterNode As CGraph.CNode = Graph.Nodes(OtherClusterNodeIndex)
                                                                   Dim DeltaX = OtherClusterNode.DisplayProperties.Position.X - Node.DisplayProperties.Position.X
                                                                   Dim DeltaY = OtherClusterNode.DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y
                                                                   Dim RawDistanceSquared = DeltaX * DeltaX + DeltaY * DeltaY
                                                                   If RawDistanceSquared > 271 * 271 Then
                                                                       Dim RawDistance As Double = Math.Max(Math.Sqrt((OtherClusterNode.DisplayProperties.Position.X - Node.DisplayProperties.Position.X) * (OtherClusterNode.DisplayProperties.Position.X - Node.DisplayProperties.Position.X) + (OtherClusterNode.DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y) * (OtherClusterNode.DisplayProperties.Position.Y - Node.DisplayProperties.Position.Y)), 0.1)
                                                                       Dim Distance As Double = Math.Max(1, RawDistance - Node.DisplayProperties.Size - OtherClusterNode.DisplayProperties.Size)
                                                                       'Dim ForceMagnitude = (Math.Pow(Distance, AttractorClusterPower) * AttractorClusterConstant)
                                                                       Dim ForceMagnitude As Single
                                                                       ForceMagnitude = Math.Min(LocalForceLimit, Distance * AttractorClusterConstant)
                                                                       Dim TempForceX As Single = ForceMagnitude * (DeltaX / RawDistance)
                                                                       Dim TempForceY As Single = ForceMagnitude * (DeltaY / RawDistance)
                                                                       SyncLock Force
                                                                           Force(Node.Index).X += TempForceX
                                                                           Force(Node.Index).Y += TempForceY
                                                                           Force(OtherClusterNodeIndex).X -= TempForceX
                                                                           Force(OtherClusterNodeIndex).Y -= TempForceY
                                                                       End SyncLock
                                                                   End If
                                                               Next OtherClusterNodeIndex
                                                           End If
#If DEBUG Then
                                                           ParallelStopwatch.Stop()
                                                           System.Threading.Interlocked.Add(ClusterTime, ParallelStopwatch.ElapsedMilliseconds)
#End If
                                                       End Sub)
        For f = 0 To UBound(Force)
            Dim Node = Graph.GetNode(f)
            Node.DisplayProperties.Position.X += Force(f).X / Node.DisplayProperties.Size
            Node.DisplayProperties.Position.Y += Force(f).Y / Node.DisplayProperties.Size
        Next f
#If DEBUG Then
        Stopwatch.Stop()
        Debug.WriteLine("Sort processing time: " + Trim(Str(Stopwatch.ElapsedMilliseconds)) + "ms")
        Debug.WriteLine("Connections time: " + Trim(Str(ConnectionsTime)) + "ms")
        Debug.WriteLine("Repulsive time: " + Trim(Str(RepulsiveTime)) + "ms")
        Debug.WriteLine("Cluster time: " + Trim(Str(ClusterTime)) + "ms")
#End If
    End Sub


    Friend Sub ProcessSort()
        Select Case Hl.SortMode
            Case ESortMode.Attractors
                Call SortAttractors()

        End Select
    End Sub
End Module
