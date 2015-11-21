Imports System.IO
Public Class ExternalInputTable
    Public Matrix As Single(,)
    Private RowsNumber, ColumnsNumber As Integer

    Public Sub New(RowsNumber As Integer, ColumnsNumber As Integer)
        InitializeComponent()
        ReDim Matrix(RowsNumber - 1, ColumnsNumber - 1)
        Me.RowsNumber = RowsNumber
        Me.ColumnsNumber = ColumnsNumber
        With DataGridView1
            .ColumnCount = ColumnsNumber
            .RowCount = RowsNumber
        End With

    End Sub
    Private Sub UpdateMatrix()

        For i As Integer = 0 To RowsNumber - 1
            For j As Integer = 0 To ColumnsNumber - 1
                Matrix(i, j) = Convert.ToSingle(DataGridView1.Rows(i).Cells(j).Value)
            Next
        Next
       

    End Sub


    Private Sub LoadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadToolStripMenuItem.Click
        OpenFileDialog1.ShowDialog()
        Dim filename As String = OpenFileDialog1.FileName
        Dim lines() As String
        Try
            lines = File.ReadAllLines(filename)
        Catch ex As Exception
            MsgBox("Error Opening " + filename + vbCrLf + ex.Message())
            Exit Sub
        End Try
        DataGridView1.Rows.Clear()
        For i As Integer = 0 To lines.Length - 1
            DataGridView1.Rows.Add(lines(i).Split(","))
        Next
        UpdateMatrix()
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        SaveFileDialog1.ShowDialog()
        Dim filename = SaveFileDialog1.FileName
        Dim csvFileWriter As System.IO.StreamWriter = New StreamWriter(filename, False)

        For Each dataRowObject As DataGridViewRow In DataGridView1.Rows
            If Not dataRowObject.IsNewRow Then
                Dim datafromGrid As String = ""
                If dataRowObject.Cells(0).Value Is Nothing Then
                    datafromGrid = "0"
                Else
                    datafromGrid = dataRowObject.Cells(0).Value.ToString()
                End If

                For i As Integer = 1 To DataGridView1.ColumnCount - 1
                    If dataRowObject.Cells(i).Value Is Nothing Then
                        datafromGrid += "," + "0"
                    Else
                        datafromGrid += "," + dataRowObject.Cells(i).Value.ToString()
                    End If


                Next
                csvFileWriter.WriteLine(datafromGrid)
            End If
        Next

        csvFileWriter.Flush()
        csvFileWriter.Close()
    End Sub
End Class