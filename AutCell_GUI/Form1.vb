Imports AutCell_Lib
Public Class Form1
    Public net As CellularAutomata
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim c As Configuration = New Configuration()
        'c.cellforEachSide = parametri.ParametriRete.ncl
        'c.levels = parametri.ParametriRete.lv
        c.fromXMLFile("prova.xml")
        'Dim net As CellularAutomata
        net = New CellularAutomata(c)

    End Sub
    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        For i As Integer = 0 To 1000
            net.Update()
        Next
    End Sub

 
    Private Sub Start_Click(sender As Object, e As EventArgs) Handles Start.Click
        BackgroundWorker2.RunWorkerAsync()
    End Sub
End Class
