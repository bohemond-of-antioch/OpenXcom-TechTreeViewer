Imports System.IO

Public Class CGraph
    Implements IEnumerable

    Const SaveVersion As Integer = 2

    Public Enum EDecoration
        None
        Square
        Triangle
    End Enum

    Public Structure SDisplayProperties
        Public Position As PointF
        Public Size As Integer
        Public ShownProperties As List(Of String)
        Public Color As Color
        Public Decoration As EDecoration
    End Structure

    Public MustInherit Class CGraphElement
        Public DisplayProperties As SDisplayProperties
        Public Properties As List(Of KeyValuePair(Of String, String))

        Public MustOverride Function SupportsDisplaySize() As Boolean
        Public MustOverride Function SupportsDisplayProperties() As Boolean
        Public MustOverride Function SupportsDisplayColor() As Boolean
        Public MustOverride Function SupportsDisplayDecoration() As Boolean

        Public Sub New()
            Properties = New List(Of KeyValuePair(Of String, String))
            If SupportsDisplayProperties() Then DisplayProperties.ShownProperties = New List(Of String)
            If SupportsDisplayColor() Then DisplayProperties.Color = Color.Black
            If SupportsDisplayDecoration() Then DisplayProperties.Decoration = EDecoration.None

        End Sub

        Public Sub ApplyTemplate(Template As CPropertyTemplate)
            DisplayProperties.Size = Template.Size
            DisplayProperties.Color = Template.Color
            DisplayProperties.Decoration = Template.Decoration
            For Each TemplateProperty In Template.Properties
                Properties.Add(New KeyValuePair(Of String, String)(TemplateProperty.Name, TemplateProperty.DefaultValue))
                If TemplateProperty.Display AndAlso Not DisplayProperties.ShownProperties.Contains(TemplateProperty.Name) Then DisplayProperties.ShownProperties.Add(TemplateProperty.Name)
            Next

        End Sub

        Public Function GetProperty(Name As String) As String
            For Each P In Properties
                If P.Key = Name Then Return P.Value
            Next P
            Return Nothing
        End Function

        Public Sub SetProperty(Name As String, Value As String)
            For Each P In Properties
                If P.Key = Name Then
                    Properties.Remove(P)
                    Exit For
                End If
            Next P
            Properties.Add(New KeyValuePair(Of String, String)(Name, Value))
        End Sub

        Public Overridable Sub Save(ByRef File As BinaryWriter)
            File.Write(Properties.Count)
            For Each P In Properties
                File.Write(P.Key)
                File.Write(If(P.Value, ""))
            Next P
            File.Write(DisplayProperties.Position.X)
            File.Write(DisplayProperties.Position.Y)
            File.Write(DisplayProperties.Size)
            File.Write(DisplayProperties.Color.ToArgb)
            File.Write(CType(DisplayProperties.Decoration, Int32))
            File.Write(DisplayProperties.ShownProperties.Count)
            For Each S In DisplayProperties.ShownProperties
                File.Write(S)
            Next S
        End Sub

        Friend Sub Load(File As BinaryReader, LoadVersion As Integer)
            Dim PropertyCount = File.ReadInt32()
            For f = 1 To PropertyCount
                Dim P As KeyValuePair(Of String, String)
                P = New KeyValuePair(Of String, String)(File.ReadString(), File.ReadString())
                Properties.Add(P)
            Next f
            DisplayProperties.Position.X = File.ReadSingle()
            DisplayProperties.Position.Y = File.ReadSingle()
            DisplayProperties.Size = File.ReadInt32()
            DisplayProperties.Color = Color.FromArgb(File.ReadInt32)
            If LoadVersion >= 2 Then
                DisplayProperties.Decoration = File.ReadInt32()
            End If
            Dim SPCount = File.ReadInt32()
            For f = 1 To SPCount
                DisplayProperties.ShownProperties.Add(File.ReadString())
            Next f
        End Sub
    End Class

    Public Class CNode
        Inherits CGraphElement
        Public Index As Integer
        Public Cluster As Integer

        Public Sub New()

        End Sub

        Public Sub New(File As BinaryReader, LoadVersion As Integer)
            MyBase.New
            MyBase.Load(File, LoadVersion)
            Index = File.ReadInt32()
            Cluster = Index
        End Sub

        Public Overrides Function SupportsDisplayProperties() As Boolean
            Return True
        End Function

        Public Overrides Function SupportsDisplaySize() As Boolean
            Return True
        End Function

        Public Overrides Function SupportsDisplayColor() As Boolean
            Return True
        End Function
        Public Overrides Function SupportsDisplayDecoration() As Boolean
            Return True
        End Function

        Public Overrides Sub Save(ByRef File As BinaryWriter)
            MyBase.Save(File)
            File.Write(Me.Index)
        End Sub
    End Class
    Public Class CConnection
        Inherits CGraphElement
        Public NodeA As Integer
        Public NodeB As Integer
        Public TwoWay As Boolean

        Public Sub New(File As BinaryReader, LoadVersion As Integer)
            MyBase.New
            MyBase.Load(File, LoadVersion)
            NodeA = File.ReadInt32()
            NodeB = File.ReadInt32()
            TwoWay = File.ReadBoolean()
        End Sub
        Public Sub New(NodeA As Integer, NodeB As Integer)
            MyBase.New
            Me.NodeA = NodeA
            Me.NodeB = NodeB
        End Sub

        Public Overrides Function SupportsDisplaySize() As Boolean
            Return False
        End Function

        Public Overrides Function SupportsDisplayProperties() As Boolean
            Return True
        End Function

        Public Overrides Function SupportsDisplayColor() As Boolean
            Return True
        End Function
        Public Overrides Function SupportsDisplayDecoration() As Boolean
            Return False
        End Function

        Public Overrides Sub Save(ByRef File As BinaryWriter)
            MyBase.Save(File)
            File.Write(Me.NodeA)
            File.Write(Me.NodeB)
            File.Write(Me.TwoWay)
        End Sub

    End Class
    Public Nodes(-1) As CNode
    Public Connections(-1) As CConnection

    Private Sub PropagateCluster(NodeIndex As Integer, Cluster As Integer)
        Nodes(NodeIndex).Cluster = Cluster
        Dim Connections = GetConnections(NodeIndex)
        For Each C In Connections
            If C.NodeA = NodeIndex Then
                If Nodes(C.NodeB).Cluster <> Nodes(NodeIndex).Cluster Then
                    PropagateCluster(C.NodeB, Cluster)
                End If
            Else
                If Nodes(C.NodeA).Cluster <> Nodes(NodeIndex).Cluster Then
                    PropagateCluster(C.NodeA, Cluster)
                End If
            End If
        Next
    End Sub

    Public Sub RecalculateClusters()
        For f = 0 To UBound(Nodes)
            Nodes(f).Cluster = f
        Next f
        For f = 0 To UBound(Nodes)
            If f = Nodes(f).Cluster Then PropagateCluster(f, Nodes(f).Cluster)
        Next f

        'For f = 0 To UBound(Nodes)
        '    Nodes(f).SetProperty("Cluster", Nodes(f).Cluster)
        '    If Not Nodes(f).DisplayProperties.ShownProperties.Contains("Cluster") Then Nodes(f).DisplayProperties.ShownProperties.Add("Cluster")
        'Next
    End Sub

    Public Function Add(Optional ByRef Template As CPropertyTemplate = Nothing) As CNode
        Dim NewNode = New CNode
        NewNode.DisplayProperties.Size = 20
        NewNode.DisplayProperties.Color = Color.Black
        If Not Template Is Nothing Then
            REM Aplikovat template
            NewNode.DisplayProperties.Size = Template.Size
            NewNode.DisplayProperties.Color = Template.Color
            NewNode.DisplayProperties.Decoration = Template.Decoration
            For Each TemplateProperty In Template.Properties
                NewNode.Properties.Add(New KeyValuePair(Of String, String)(TemplateProperty.Name, TemplateProperty.DefaultValue))
                If TemplateProperty.Display AndAlso Not NewNode.DisplayProperties.ShownProperties.Contains(TemplateProperty.Name) Then NewNode.DisplayProperties.ShownProperties.Add(TemplateProperty.Name)
            Next
        End If
        Me.Add(NewNode)
        Return NewNode
    End Function
    Public Sub Add(ByRef Node As CNode)
        ReDim Preserve Nodes(UBound(Nodes) + 1)
        Node.Index = UBound(Nodes)
        Node.Cluster = Node.Index
        Nodes(UBound(Nodes)) = Node
    End Sub
    Public Sub Remove(ByRef Node As CNode)
        Remove(Node.Index)
    End Sub
    Public Sub Remove(Index As Integer)
        For f = UBound(Connections) To 0 Step -1
            If Connections(f).NodeA = Index Or Connections(f).NodeB = Index Then Disconnect(Connections(f))
        Next f
        For f = Index To UBound(Nodes) - 1
            Nodes(f) = Nodes(f + 1)
            Nodes(f).Index = f
        Next f
        ReDim Preserve Nodes(UBound(Nodes) - 1)
        For f = 0 To UBound(Connections)
            If Connections(f).NodeA > Index Then Connections(f).NodeA -= 1
            If Connections(f).NodeB > Index Then Connections(f).NodeB -= 1
        Next f
        RecalculateClusters()
    End Sub
    Public Sub Connect(NodeA As Integer, NodeB As Integer, Optional ByRef Template As CPropertyTemplate = Nothing)
        For f = 0 To UBound(Connections)
            If Connections(f).NodeA = NodeA And Connections(f).NodeB = NodeB Then Exit Sub
        Next f
        ReDim Preserve Connections(UBound(Connections) + 1)
        Dim NewConnection = New CConnection(NodeA, NodeB)
        NewConnection.DisplayProperties.Color = Color.Black
        If Not Template Is Nothing Then
            REM Aplikovat Template
            NewConnection.DisplayProperties.Size = Template.Size
            NewConnection.DisplayProperties.Color = Template.Color
            For Each TemplateProperty In Template.Properties
                NewConnection.Properties.Add(New KeyValuePair(Of String, String)(TemplateProperty.Name, TemplateProperty.DefaultValue))
                If TemplateProperty.Display AndAlso Not NewConnection.DisplayProperties.ShownProperties.Contains(TemplateProperty.Name) Then NewConnection.DisplayProperties.ShownProperties.Add(TemplateProperty.Name)
            Next
        End If
        Connections(UBound(Connections)) = NewConnection
        For f = 0 To UBound(Connections)
            If Connections(f).NodeB = NodeA And Connections(f).NodeA = NodeB Then
                Connections(f).TwoWay = True
                NewConnection.TwoWay = True
            End If
        Next f
        RecalculateClusters()
    End Sub
    Public Sub Disconnect(NodeA As Integer, NodeB As Integer)
        For f = 0 To UBound(Connections)
            If Connections(f).NodeA = NodeA And Connections(f).NodeB = NodeB Then
                RemoveConnection(f)
                Exit For
            End If
        Next f
    End Sub
    Public Sub Disconnect(Connection As CConnection)
        For f = 0 To UBound(Connections)
            If Connections(f) Is Connection Then
                RemoveConnection(f)
                Exit For
            End If
        Next f
    End Sub
    Private Sub RemoveConnection(Index As Integer)
        Dim PrevNodeA = Connections(Index).NodeA
        Dim PrevNodeB = Connections(Index).NodeB

        For f = Index To UBound(Connections) - 1
            Connections(f) = Connections(f + 1)
        Next f
        ReDim Preserve Connections(UBound(Connections) - 1)
        For f = 0 To UBound(Connections)
            If Connections(f).NodeB = PrevNodeA And Connections(f).NodeA = PrevNodeB Then
                Connections(f).TwoWay = False
            End If
        Next f
        RecalculateClusters()
    End Sub
    Public Function GetNode(Index As Integer) As CNode
        Return Nodes(Index)
    End Function
    Public Function GetConnections(Index As Integer) As CConnection()
        Dim ResultArray(-1) As CConnection
        For f = 0 To UBound(Connections)
            If (Connections(f).NodeA = Index Or Connections(f).NodeB = Index) Then
                ReDim Preserve ResultArray(UBound(ResultArray) + 1)
                ResultArray(UBound(ResultArray)) = Connections(f)
            End If
        Next f
        Return ResultArray
    End Function

    Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Nodes.GetEnumerator()
    End Function

    Public Function GetAllConnections() As CConnection()
        Return Connections
    End Function
    Friend Sub Save(File As BinaryWriter)
        File.Write(SaveVersion)
        File.Write(Nodes.Count)
        For Each Node In Nodes
            Node.Save(File)
        Next Node
        File.Write(Connections.Count)
        For Each Connection In Connections
            Connection.Save(File)
        Next Connection
    End Sub

    Friend Sub Load(File As BinaryReader)
        Dim LoadVersion = File.ReadInt32()
        Dim NodeCount = File.ReadInt32()
        ReDim Nodes(NodeCount - 1)
        For f = 0 To NodeCount - 1
            Nodes(f) = New CNode(File, LoadVersion)
        Next f
        Dim ConnectionCount = File.ReadInt32()
        ReDim Connections(ConnectionCount - 1)
        For f = 0 To ConnectionCount - 1
            Connections(f) = New CConnection(File, LoadVersion)
        Next f
        RecalculateClusters()
    End Sub
End Class
