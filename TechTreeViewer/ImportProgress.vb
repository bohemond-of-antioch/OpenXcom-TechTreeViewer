Public Class ImportProgress
    Public Sub SetProgressBar(Maximum As Integer, Description As String)
        ImportProgressBar.Minimum = 0
        ImportProgressBar.Maximum = Maximum
        ImportProgressBar.Value = 0
        ImportProgressBar.Step = 1
        LabelProgressInfo.Text = Description
        LabelProgressInfo.Refresh()
    End Sub
End Class