Imports System.IO

Public Class CTemplateStorage
    Implements IEnumerable

    Private Templates As List(Of CPropertyTemplate)

    Public Sub New()
        Templates = New List(Of CPropertyTemplate)
    End Sub

    Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Templates.GetEnumerator()
    End Function

    Public Sub Add()
        Dim NewTemplate As CPropertyTemplate
        NewTemplate = New CPropertyTemplate
        NewTemplate.Name = "New template"
        NewTemplate.Size = 20
        NewTemplate.Color = Color.Black
        Templates.Add(NewTemplate)
    End Sub

    Public Sub Remove(Item)
        Templates.Remove(Item)
    End Sub

    Const SaveVersion As Integer = 2
    Friend Sub Save(File As BinaryWriter)
        File.Write(SaveVersion)
        File.Write(Templates.Count)
        For Each Template In Templates
            File.Write(Template.Name)
            File.Write(Template.AppliesToConnections)
            File.Write(Template.AppliesToNodes)
            File.Write(Template.DefaultTemplate)
            File.Write(Template.Size)
            File.Write(Template.Color.ToArgb())
            File.Write(CType(Template.Decoration, Int32))
            File.Write(Template.Properties.Count)
            For Each P In Template.Properties
                File.Write(P.Name)
                File.Write(If(P.DefaultValue, ""))
                File.Write(P.Display)
            Next P
        Next Template
    End Sub

    Friend Sub Load(File As BinaryReader)
        Templates.Clear()
        Call AddFromFile(File)
    End Sub

    Friend Sub AddFromFile(File As BinaryReader)
        Dim FileVersion As Integer
        FileVersion = File.ReadInt32()
        Dim TemplateCount = File.ReadInt32()
        For f = 1 To TemplateCount
            Dim NewTemplate As CPropertyTemplate
            NewTemplate = New CPropertyTemplate()
            NewTemplate.Name = File.ReadString()
            NewTemplate.AppliesToConnections = File.ReadBoolean()
            NewTemplate.AppliesToNodes = File.ReadBoolean()
            NewTemplate.DefaultTemplate = File.ReadBoolean()
            NewTemplate.Size = File.ReadInt32()
            NewTemplate.Color = Color.FromArgb(File.ReadInt32())
            If (FileVersion >= 2) Then NewTemplate.Decoration = File.ReadInt32()
            Dim PropertyCount = File.ReadInt32()
            For p = 1 To PropertyCount
                Dim NewProperty As CPropertyTemplate.SProperty
                NewProperty.Name = File.ReadString()
                NewProperty.DefaultValue = File.ReadString()
                If NewProperty.DefaultValue = "" Then NewProperty.DefaultValue = Nothing
                NewProperty.Display = File.ReadBoolean()
                NewTemplate.Properties.Add(NewProperty)
            Next p
            Templates.Add(NewTemplate)
        Next f
        Call MainView.InvalidateFile()
    End Sub
End Class
