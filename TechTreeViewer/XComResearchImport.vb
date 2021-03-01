Imports System.IO
Module XComResearchImport
    Friend Class CXComResearch
        Friend Name As String
        Friend Cost As Integer
        Friend Item As Boolean
        Friend ListOrder As Integer
        Friend Section As String
        Friend Planet As String
        Friend Lookup As String
        Friend Dependencies As List(Of String)
        Friend Requires As List(Of String)
        Friend GetOneFree As List(Of String)
        Friend GetOneFreeProtected As List(Of String)
        Friend SequentialGetOneFree As List(Of String)
        Friend Unlocks As List(Of String)
    End Class

    Private AllModNodes As List(Of YamlNode)

    Public Function LoadResearch(GamePath As String, ActiveMods As List(Of String)) As List(Of CXComResearch)
        LoadMods(GamePath, ActiveMods)
        ImportProgress.SetProgressBar(AllModNodes.Count, "Parsing research")
        Dim AllResearch As Dictionary(Of String, CXComResearch) = New Dictionary(Of String, CXComResearch)
        For Each ModRootNode In AllModNodes
            If Not ModRootNode Is Nothing AndAlso ModRootNode.HasMapping("research") Then
                Dim RootResearchNode = ModRootNode.GetMapping("research")
                For r = 0 To RootResearchNode.ItemCount() - 1
                    If RootResearchNode.GetItem(r).HasMapping("name") Then
                        Dim XComResearch As CXComResearch = New CXComResearch
                        XComResearch.Name = RootResearchNode.GetItem(r).GetMapping("name").GetValue()
                        XComResearch.Cost = Val(RootResearchNode.GetItem(r).GetMapping("cost", "0").GetValue())
                        XComResearch.ListOrder = Val(RootResearchNode.GetItem(r).GetMapping("listOrder", "0").GetValue())
                        XComResearch.Item = (RootResearchNode.GetItem(r).GetMapping("needItem", "false").GetValue().ToLower() = "true")
                        XComResearch.Planet = RootResearchNode.GetItem(r).GetMapping("planet", "").GetValue()
                        XComResearch.Lookup = RootResearchNode.GetItem(r).GetMapping("lookup", "").GetValue()
                        XComResearch.Dependencies = GetConnectedResearch(RootResearchNode.GetItem(r), "dependencies")
                        XComResearch.Requires = GetConnectedResearch(RootResearchNode.GetItem(r), "requires")
                        XComResearch.GetOneFree = GetConnectedResearch(RootResearchNode.GetItem(r), "getOneFree")
                        XComResearch.GetOneFreeProtected = GetConnectedResearch(RootResearchNode.GetItem(r), "getOneFreeProtected")
                        XComResearch.SequentialGetOneFree = GetConnectedResearch(RootResearchNode.GetItem(r), "sequentialGetOneFree")
                        XComResearch.Unlocks = GetConnectedResearch(RootResearchNode.GetItem(r), "unlocks")

                        Dim UfopaediaArticle = FindUfopaediaArticle(AllModNodes, XComResearch.Name)
                        If UfopaediaArticle Is Nothing Then
                            XComResearch.Section = "UNDEFINED"
                        Else
                            XComResearch.Section = UfopaediaArticle.GetMapping("section").GetValue()
                        End If
                        If AllResearch.ContainsKey(XComResearch.Name) Then
                            If RootResearchNode.GetItem(r).HasMapping("cost") Then AllResearch(XComResearch.Name).Cost = XComResearch.Cost
                            If RootResearchNode.GetItem(r).HasMapping("listOrder") Then AllResearch(XComResearch.Name).ListOrder = XComResearch.ListOrder
                            If RootResearchNode.GetItem(r).HasMapping("needItem") Then AllResearch(XComResearch.Name).Item = XComResearch.Item
                            If RootResearchNode.GetItem(r).HasMapping("planet") Then AllResearch(XComResearch.Name).Planet = XComResearch.Planet
                            If RootResearchNode.GetItem(r).HasMapping("lookup") Then AllResearch(XComResearch.Name).Lookup = XComResearch.Lookup
                            If RootResearchNode.GetItem(r).HasMapping("dependencies") Then AllResearch(XComResearch.Name).Dependencies = XComResearch.Dependencies
                            If RootResearchNode.GetItem(r).HasMapping("requires") Then AllResearch(XComResearch.Name).Requires = XComResearch.Requires
                            If RootResearchNode.GetItem(r).HasMapping("getOneFree") Then AllResearch(XComResearch.Name).GetOneFree = XComResearch.GetOneFree
                            If RootResearchNode.GetItem(r).HasMapping("getOneFreeProtected") Then AllResearch(XComResearch.Name).GetOneFreeProtected = XComResearch.GetOneFreeProtected
                            If RootResearchNode.GetItem(r).HasMapping("sequentialGetOneFree") Then AllResearch(XComResearch.Name).SequentialGetOneFree = XComResearch.SequentialGetOneFree
                            If RootResearchNode.GetItem(r).HasMapping("unlocks") Then AllResearch(XComResearch.Name).Unlocks = XComResearch.Unlocks
                        Else
                            AllResearch.Add(XComResearch.Name, XComResearch)
                        End If
                    End If
                    If RootResearchNode.GetItem(r).HasMapping("delete") Then
                        AllResearch.Remove(RootResearchNode.GetItem(r).GetMapping("delete").GetValue())
                    End If
                Next
            End If
            ImportProgress.ImportProgressBar.PerformStep()
        Next ModRootNode
        Return AllResearch.Values.ToList()
    End Function

    Private Sub LoadMods(GamePath As String, ActiveMods As List(Of String))
        Dim StandardMods = Directory.GetDirectories(GamePath + "\standard")
        Dim UserMods = Directory.GetDirectories(GamePath + "\user\mods")

        Dim AllMods As List(Of String) = New List(Of String)
        AllMods.AddRange(StandardMods)
        AllMods.AddRange(UserMods)

        AllModNodes = New List(Of YamlNode)
        ImportProgress.SetProgressBar(1, "Parsing rules")
        REM Count what we need to load
        Dim FilesToParse As Integer = 0
        For Each SearchMod In ActiveMods
            For Each GameModPath In AllMods
                Dim GameModId = Path.GetFileName(GameModPath)
                Dim ModMetadata = New YamlFileParser(GameModPath + "\metadata.yml").Parse()
                If ModMetadata.HasMapping("id") Then
                    GameModId = ModMetadata.GetMapping("id").GetValue()
                End If
                If GameModId.ToLower() = SearchMod.ToLower() Then
                    Dim IsMaster = ModMetadata.GetMapping("isMaster", "false").GetValue()
                    Dim Master = ModMetadata.GetMapping("master", "").GetValue()
                    If IsMaster.ToLower() = "true" OrElse ActiveMods.Contains(Master) Then
                        FilesToParse += CountModRules(GameModPath)
                    End If
                End If
            Next GameModPath
        Next SearchMod
        ImportProgress.SetProgressBar(FilesToParse, "Parsing rules")
        REM Now we parse
        For Each SearchMod In ActiveMods
            For Each GameModPath In AllMods
                Dim GameModId = Path.GetFileName(GameModPath)
                Dim ModMetadata = New YamlFileParser(GameModPath + "\metadata.yml").Parse()
                If ModMetadata.HasMapping("id") Then
                    GameModId = ModMetadata.GetMapping("id").GetValue()
                End If
                If GameModId.ToLower() = SearchMod.ToLower() Then
                    Dim IsMaster = ModMetadata.GetMapping("isMaster", "false").GetValue()
                    Dim Master = ModMetadata.GetMapping("master", "").GetValue()
                    If IsMaster.ToLower() = "true" OrElse ActiveMods.Contains(Master) Then
                        ImportMod(GameModPath)
                    End If
                End If
            Next GameModPath
        Next SearchMod
    End Sub

    Private Function FindUfopaediaArticle(AllModNodes As List(Of YamlNode), Name As String) As YamlNode
        FindUfopaediaArticle = Nothing
        For Each ModRootNode In AllModNodes
            If Not ModRootNode Is Nothing AndAlso ModRootNode.HasMapping("ufopaedia") Then
                Dim RootResearchNode = ModRootNode.GetMapping("ufopaedia")
                For r = 0 To RootResearchNode.ItemCount() - 1
                    If RootResearchNode.GetItem(r).HasMapping("id") AndAlso RootResearchNode.GetItem(r).GetMapping("id").GetValue() = Name AndAlso RootResearchNode.GetItem(r).HasMapping("section") Then
                        FindUfopaediaArticle = RootResearchNode.GetItem(r)
                    End If
                Next
            End If
        Next ModRootNode
        If Not FindUfopaediaArticle Is Nothing Then Exit Function
        For Each ModRootNode In AllModNodes
            If Not ModRootNode Is Nothing AndAlso ModRootNode.HasMapping("ufopaedia") Then
                Dim RootResearchNode = ModRootNode.GetMapping("ufopaedia")
                For r = 0 To RootResearchNode.ItemCount() - 1
                    If RootResearchNode.GetItem(r).HasMapping("id") AndAlso RootResearchNode.GetItem(r).HasMapping("requires") AndAlso RootResearchNode.GetItem(r).HasMapping("section") Then
                        Dim Requires = RootResearchNode.GetItem(r).GetMapping("requires")
                        For f = 0 To Requires.ItemCount - 1
                            If Requires.GetItem(f).GetValue() = Name Then
                                FindUfopaediaArticle = RootResearchNode.GetItem(r)
                            End If
                        Next f
                    End If
                Next
            End If
        Next ModRootNode

        Return Nothing
    End Function

    Private Function GetConnectedResearch(Node As YamlNode, Key As String) As List(Of String)
        GetConnectedResearch = New List(Of String)
        If Node.HasMapping(Key) Then
            Dim ConnectedNode = Node.GetMapping(Key)
            If ConnectedNode.Type = YamlNode.EType.Sequence Then
                For d = 0 To ConnectedNode.ItemCount - 1
                    GetConnectedResearch.Add(ConnectedNode.GetItem(d).GetValue())
                Next d
            ElseIf ConnectedNode.Type = YamlNode.EType.Mapping Then
                For Each ResearchID In ConnectedNode.GetMappingKeys()
                    GetConnectedResearch.Add(ResearchID)
                Next ResearchID
            End If

        End If
    End Function
    Public Function CountModRules(Path As String) As Integer
        Return CountModRulesParseDirectory(Path)
    End Function
    Private Function CountModRulesParseDirectory(Path As String) As Integer
        Dim FileName
        For Each FileName In Directory.GetFiles(Path)
            If FileName.EndsWith(".rul") Then
                CountModRulesParseDirectory += 1
            End If
        Next

        For Each DirName In Directory.GetDirectories(Path)
            CountModRulesParseDirectory += CountModRules(DirName)
        Next
    End Function


    Public Sub ImportMod(Path As String)
        ParseDirectory(Path)
    End Sub

    Private Sub ParseDirectory(Path As String)
        Dim FileName
        For Each FileName In Directory.GetFiles(Path)
            If FileName.EndsWith(".rul") Then
                Dim Parser = New YamlFileParser(FileName)
                Dim RootNode = Parser.Parse()
                AllModNodes.Add(RootNode)
                ImportProgress.ImportProgressBar.PerformStep()
            End If
        Next

        For Each DirName In Directory.GetDirectories(Path)
            ImportMod(DirName)
        Next
    End Sub

End Module
