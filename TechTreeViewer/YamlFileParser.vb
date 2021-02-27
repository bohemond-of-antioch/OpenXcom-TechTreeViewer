Imports TechTreeViewer

Public Class YamlFileParser

    Private FileName As String
    Private FileReader As System.IO.TextReader
    Private CurrentLine As String
    Private CurrentLineProcessed As Boolean

    Private Anchors As Dictionary(Of String, YamlNode)

    Public Sub New(FileName As String)
        Me.FileName = FileName
    End Sub

    Public Function Parse() As YamlNode
        Dim RootNode As YamlNode
        Debug.WriteLine("Parsing " + FileName)
        FileReader = System.IO.File.OpenText(FileName)
        Anchors = New Dictionary(Of String, YamlNode)
        RootNode = ParseNextLine()
        If Not RootNode Is Nothing Then RootNode.Source = FileName

        FileReader.Close()
        Return RootNode
    End Function

    Private Function CalculateIndent(Line As String) As Integer
        CalculateIndent = 0
        Do While Line(CalculateIndent) = " "
            CalculateIndent = CalculateIndent + 1
        Loop
    End Function

    Private Const EXPR_YAML_SKIP_LINE = "(?:^\s*#.*$)|(?:^\s*$)"
    Private Function ReadNextLine() As String
        Do
            ReadNextLine = FileReader.ReadLine()
            If ReadNextLine Is Nothing Then Return Nothing
        Loop While System.Text.RegularExpressions.Regex.IsMatch(ReadNextLine, EXPR_YAML_SKIP_LINE)
    End Function

    Private Const EXPR_YAML_SEQUENCE = "^- (.*)$"
    Private Const EXPR_YAML_SEQUENCE_REFERENCE = "^-\s*(?:\*(\w+)\s*)(?:#.*)?$"
    Private Const EXPR_YAML_PURE_SEQUENCE = "^-\s*(?:&(\w+)\s*)?(?:#.*)?$"
    Private Const EXPR_YAML_MAPPING_REFERENCE = "^(\w+?):\s*(?:\*(\w+)\s*)(?:#.*)?$"
    Private Const EXPR_YAML_MAPPING = "^(\w+?):\s*(?:&(\w+)\s*)?(?:#.*)?$"
    Private Const EXPR_YAML_INLINE_MAPPING = "^(\w+?):\s*(?:&(\w+)\s+)?""?(.+?)""?\s*(?:#.*)?$"
    Private Const EXPR_YAML_VALUE = "^""?(.+?)""?\s*(?:#.*)?$"



    Private Function ParseNextLine() As YamlNode
        CurrentLine = ReadNextLine()
        If CurrentLine Is Nothing Then Return Nothing
        Dim Indent As Integer
        Indent = CalculateIndent(CurrentLine)

        Return ParseNodeFromLine(Indent)
    End Function
    Private Function ParseNodeFromLine(Indent As Integer) As YamlNode
        Dim Node As YamlNode

        Dim FirstLine As Boolean = True

        Do While FirstLine = True Or CalculateIndent(CurrentLine) = Indent
            CurrentLineProcessed = True
            Dim UnindentedLine As String = Mid(CurrentLine, Indent + 1)
            If FirstLine Then
                FirstLine = False
                If System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_PURE_SEQUENCE) Then
                    Node = New YamlNode(YamlNode.EType.Sequence)
                ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_SEQUENCE_REFERENCE) Then
                    Node = New YamlNode(YamlNode.EType.Sequence)
                ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_SEQUENCE) Then
                    Node = New YamlNode(YamlNode.EType.Sequence)
                ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_MAPPING_REFERENCE) Then
                    Node = New YamlNode(YamlNode.EType.Mapping)
                ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_MAPPING) Then
                    Node = New YamlNode(YamlNode.EType.Mapping)
                ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_INLINE_MAPPING) Then
                    Node = New YamlNode(YamlNode.EType.Mapping)
                ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_VALUE) Then
                    Dim Matches = System.Text.RegularExpressions.Regex.Matches(UnindentedLine, EXPR_YAML_VALUE)
                    Return New YamlNode(Matches(0).Groups(1).Value)
                Else
                    Throw New Exception("Unknown Yaml node: " + UnindentedLine)
                End If
            End If

            If System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_PURE_SEQUENCE) Then
                Dim Matches = System.Text.RegularExpressions.Regex.Matches(UnindentedLine, EXPR_YAML_PURE_SEQUENCE)
                Dim ParsedNode = ParseNextLine()
                If Matches(0).Groups(1).Success Then
                    CreateAnchor(Matches(0).Groups(1).Value, ParsedNode)
                End If
                Node.AddItem(ParsedNode)
            ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_SEQUENCE_REFERENCE) Then
                Dim Matches = System.Text.RegularExpressions.Regex.Matches(UnindentedLine, EXPR_YAML_SEQUENCE_REFERENCE)
                Node.AddItem(GetAnchor(Matches(0).Groups(1).Value))
            ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_SEQUENCE) Then
                Node.AddItem(ParseNodeFromLine(Indent + 2))
            ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_MAPPING_REFERENCE) Then
                Dim Matches = System.Text.RegularExpressions.Regex.Matches(UnindentedLine, EXPR_YAML_MAPPING_REFERENCE)
                Node.SetMapping(Matches(0).Groups(1).Value, GetAnchor(Matches(0).Groups(2).Value))
            ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_MAPPING) Then
                Dim Matches = System.Text.RegularExpressions.Regex.Matches(UnindentedLine, EXPR_YAML_MAPPING)
                Dim ParsedNode = ParseNextLine()
                If Matches(0).Groups(2).Success Then
                    CreateAnchor(Matches(0).Groups(2).Value, ParsedNode)
                End If
                Node.SetMapping(Matches(0).Groups(1).Value, ParsedNode)
            ElseIf System.Text.RegularExpressions.Regex.IsMatch(UnindentedLine, EXPR_YAML_INLINE_MAPPING) Then
                Dim Matches = System.Text.RegularExpressions.Regex.Matches(UnindentedLine, EXPR_YAML_INLINE_MAPPING)
                Dim ParsedNode = New YamlNode(Matches(0).Groups(3).Value)
                If Matches(0).Groups(2).Success Then
                    CreateAnchor(Matches(0).Groups(2).Value, ParsedNode)
                End If
                Node.SetMapping(Matches(0).Groups(1).Value, ParsedNode)
            End If

            If CurrentLineProcessed Then
                CurrentLine = ReadNextLine()
                CurrentLineProcessed = False
            End If

            If CurrentLine Is Nothing Then Exit Do
        Loop
        Return Node
    End Function

    Private Sub CreateAnchor(key As String, node As YamlNode)
        Anchors.Add(key, node)
    End Sub
    Private Function GetAnchor(key As String) As YamlNode
        Return Anchors(key)
    End Function
End Class
