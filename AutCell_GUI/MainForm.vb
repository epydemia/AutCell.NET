Imports AutCell_Lib
Imports OxyPlot
Imports OxyPlot.Series

Public Class MainForm
    Public net As CellularAutomata
    Private ActivityPlot As New PlotModel()
    Private ActivitySeries As New LineSeries()

    Private WeightDistributionPlot As New PlotModel()
    Private WeightDistributionSeries As New ColumnSeries()

    Private ActivityDistributionPlot As New PlotModel()
    Private ActDistribSeries As New ColumnSeries()

    Private Running As Boolean = False


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim c As Configuration = New Configuration()
        'c.cellforEachSide = parametri.ParametriRete.ncl
        'c.levels = parametri.ParametriRete.lv
        c.fromXMLFile("prova.xml")
        'Dim net As CellularAutomata
        net = New CellularAutomata(c)

        BackgroundWorker2.WorkerReportsProgress = True


        ' Set up Activity Plot
        With ActivityPlot
            .Title = "Global Activity"
            .Series.Add(ActivitySeries)
            GlobalActivity.Model = ActivityPlot
            .Axes.Add(New Axes.LinearAxis(pos:=Axes.AxisPosition.Bottom, minimum:=0, maximum:=1000))
        End With


        'Set up Weight Distribution
        With WeightDistributionPlot
            .Title = "Synaptic Weight Distribution"
            .Series.Add(WeightDistributionSeries)
            DistribuzionePesi.Model = WeightDistributionPlot
            Dim CategoryAxis As New Axes.CategoryAxis()
            For lv As Integer = 1 To net.NumLivelli
                CategoryAxis.Labels.Add("Level " + lv.ToString())
            Next
            .Axes.Add(CategoryAxis)
        End With

        'Set Up Activity distribution
        With ActivityDistributionPlot
            .Title = "Activity Distribution"
            .Series.Add(ActDistribSeries)
            ActivityDistribution.Model = ActivityDistributionPlot
            Dim CategoryAxis As New Axes.CategoryAxis()
            CategoryAxis.Labels.Add("Activity")
            .Axes.Add(CategoryAxis)
        End With


        ' Set Up Status Bar
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
                net.UpdateSynaptictWeightDistribution()
                net.UpdateActivityDistribution()

                UpdateActivityPlot(i, net.globalActivity)
                UpdateWeightDistributionPlot(net)
                UpdataActivityDistributionPlot(net)

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
            GlobalActivity.InvalidatePlot(True)
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


    Private Sub UpdateActivityPlot(ByVal stepIndex As Double, ByVal globalActivityValue As Double)
        ActivitySeries.Points.Add(New ScatterPoint(stepIndex, globalActivityValue))
        GlobalActivity.InvalidatePlot(True)
    End Sub

    Private Sub UpdateWeightDistributionPlot(ByRef net As CellularAutomata)
        WeightDistributionSeries.Items.Clear()
        For lv = 0 To net.NumLivelli - 1
            For i As Integer = 0 To CellularAutomata.NumeroIntervalliDistribuzione
                WeightDistributionSeries.Items.Add(New ColumnItem(net.SynapticWeightDistribution(lv, i), lv))
            Next
        Next

        WeightDistributionPlot.InvalidatePlot(True)
    End Sub

    Private Sub UpdataActivityDistributionPlot(ByRef net As CellularAutomata)
        ActDistribSeries.Items.Clear()
        For i As Integer = 0 To CellularAutomata.NumeroIntervalliDistribuzione
            ActDistribSeries.Items.Add(New ColumnItem(net.ActivityDistribution(i), 0))

        Next
        ActivityDistributionPlot.InvalidatePlot(True)
    End Sub
End Class

