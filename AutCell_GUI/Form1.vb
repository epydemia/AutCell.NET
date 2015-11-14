Imports AutCell_Lib
Imports OxyPlot

Public Class Form1
    Public net As CellularAutomata
    Private ActivityPlot As New PlotModel()
    Private ActivitySeries As New Series.LineSeries()
    Private Running As Boolean = False


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim c As Configuration = New Configuration()
        'c.cellforEachSide = parametri.ParametriRete.ncl
        'c.levels = parametri.ParametriRete.lv
        c.fromXMLFile("prova.xml")
        'Dim net As CellularAutomata
        net = New CellularAutomata(c)

        BackgroundWorker2.WorkerReportsProgress = True
        ActivityPlot.Series.Add(ActivitySeries)
        Plot1.Model = ActivityPlot

        ActivityPlot.Axes.Add(New Axes.LinearAxis(pos:=Axes.AxisPosition.Bottom, Minimum:=0, Maximum:=1000))

        ToolStripStatusLabel2.Spring = True
        ToolStripStatusLabel2.Text = ""

        ToolStripProgressBar1.Minimum = 0
        ToolStripProgressBar1.Maximum = 100
        ToolStripProgressBar1.Alignment = ToolStripItemAlignment.Right
    End Sub
    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
       
        For i As Integer = 0 To 1000
            If BackgroundWorker2.CancellationPending Then
                e.Cancel = True

            Else

                'net.Update()
                net.UpdateParallelFor()

                ActivitySeries.Points.Add(New Series.ScatterPoint(i, net.globalActivity))
                Plot1.InvalidatePlot(True)
                BackgroundWorker2.ReportProgress(i / 10)
            End If
            
        Next
        Running = False
    End Sub



    Private Sub BackgroundWorker2_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker2.ProgressChanged
        ToolStripProgressBar1.Value = e.ProgressPercentage
    End Sub

    Private Sub StartToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartToolStripMenuItem.Click
        If Running = False Then

            Running = True

            ActivitySeries.Points.Clear()
            Plot1.InvalidatePlot(True)
            BackgroundWorker2.RunWorkerAsync()

        Else
            Running = False
            BackgroundWorker2.CancelAsync()

        End If
        updateStatusBar()
    End Sub

    Private Sub updateStatusBar()
        If Running Then
            ToolStripStatusLabel1.Text = "Running..."
        Else
            ToolStripStatusLabel1.Text = "Ready"
        End If
    End Sub

    Private Sub BackgroundWorker2_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        Running = False
        updateStatusBar()
    End Sub
End Class

