Public Class CPropertyTemplate
    Public Structure SProperty
        Dim Name As String
        Dim DefaultValue As String
        Dim Display As Boolean
    End Structure

    Public Name As String
    Public AppliesToNodes As Boolean
    Public AppliesToConnections As Boolean
    Public DefaultTemplate As Boolean
    Public Size As Integer
    Public Color As Color
    Public Decoration As CGraph.EDecoration
    Public Properties As List(Of SProperty)

    Public Sub New()
        Properties = New List(Of SProperty)
    End Sub

    Public Overrides Function ToString() As String
        Return Name
    End Function
End Class
