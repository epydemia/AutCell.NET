Imports AutCell_Lib
Imports OxyPlot
Imports OxyPlot.Series
Imports ViewPortOpenGL


Public Class MainForm
    Public net As CellularAutomateExternalInput

    Public LayerMonitor As MonitorLayer
    Public ActivationMonitor As MonitorLayer

    Public StateSpace3D As New ViewPortOpenGL.ViewPortWindow()

    Private ActivityPlot As New PlotModel()
    Private ActivitySeries As New LineSeries()

    Private WeightDistributionPlot As New PlotModel()
    Private WeightDistributionSeries As New ColumnSeries()

    Private ActivityDistributionPlot As New PlotModel()
    Private ActDistribSeries As New ColumnSeries()

    Private ThresholdDistributionPlot As New PlotModel()
    Private ThresholdDistribSeries As New ColumnSeries()

    Private Running As Boolean = False
    Private ExternalInputEnabled = False
    Private LockSynapse = False

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim c As Configuration = New Configuration()
        'c.cellforEachSide = parametri.ParametriRete.ncl
        'c.levels = parametri.ParametriRete.lv
        c.fromXMLFile("prova.xml")
        'Dim net As CellularAutomata
        net = New CellularAutomateExternalInput(c)

        BackgroundWorker2.WorkerReportsProgress = True

        ' Set up Activity Plot
        With ActivityPlot
            .Title = "Global Activity"
            .Series.Add(ActivitySeries)
            Plot_GlobalActivity.Model = ActivityPlot
            .Axes.Add(New Axes.LinearAxis(pos:=Axes.AxisPosition.Bottom, minimum:=0, maximum:=1000))
        End With

        'Set up Weight Distribution
        With WeightDistributionPlot
            .Title = "Synaptic Weight Distribution"
            .Series.Add(WeightDistributionSeries)
            Plot_WeightDistribution.Model = WeightDistributionPlot
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
            Plot_ActivityDistribution.Model = ActivityDistributionPlot
            Dim CategoryAxis As New Axes.CategoryAxis()
            CategoryAxis.Labels.Add("Activity")
            .Axes.Add(CategoryAxis)
        End With

        'Set Up Activity distribution
        With ThresholdDistributionPlot
            .Title = "Threshold Distribution"
            .Series.Add(ThresholdDistribSeries)
            Plot_ThresholdDistribution.Model = ThresholdDistributionPlot
            Dim CategoryAxis As New Axes.CategoryAxis()
            CategoryAxis.Labels.Add("Threshold")
            .Axes.Add(CategoryAxis)
        End With


        ' Set Up Status Bar
        ToolStripStatusLabel2.Spring = True
        ToolStripStatusLabel2.Text = ""
        ToolStripProgressBar1.Minimum = 0
        ToolStripProgressBar1.Maximum = 100
        ToolStripProgressBar1.Alignment = ToolStripItemAlignment.Right

        'Set up MonitorLayer Window
        LayerMonitor = New MonitorLayer(3, 4, MonitorType.Activity)
        ActivationMonitor = New MonitorLayer(3, 4, MonitorType.Activation)
    End Sub
    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork

        For i As Integer = 0 To 1000
            If BackgroundWorker2.CancellationPending Then
                e.Cancel = True

            Else
                net.Update()
                'net.UpdateParallelFor()
                net.UpdateSynaptictWeightDistribution()
                net.UpdateActivityDistribution()
                net.UpdateThresholdDistribution()

                ' Update GUI
                UpdateActivityPlot(i, net.globalActivity)
                UpdateWeightDistributionPlot(net)
                UpdateActivityDistributionPlot(net)
                UpdateThresholdDistributionPlot(net)

                LayerMonitor.UpdateWindow(net)
                ActivationMonitor.UpdateWindow(net)

                ' Update SS3D
                StateSpace3D.addPoints(net.Neu(1, 1, 1).activity, net.Neu(3, 3, 3).activity, net.Neu(5, 5, 5).activity, ExternalInputEnabled)

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
            Plot_GlobalActivity.InvalidatePlot(True)
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
        Plot_GlobalActivity.InvalidatePlot(True)
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

    Private Sub UpdateActivityDistributionPlot(ByRef net As CellularAutomata)
        ActDistribSeries.Items.Clear()
        For i As Integer = 0 To CellularAutomata.NumeroIntervalliDistribuzione
            ActDistribSeries.Items.Add(New ColumnItem(net.ActivityDistribution(i), 0))

        Next
        ActivityDistributionPlot.InvalidatePlot(True)
    End Sub

    Private Sub UpdateThresholdDistributionPlot(ByRef net As CellularAutomata)
        ThresholdDistribSeries.Items.Clear()
        For i As Integer = 0 To net.ThresholdDistribution.Length - 1
            ThresholdDistribSeries.Items.Add(New ColumnItem(net.ThresholdDistribution(i), 0))
        Next
        ThresholdDistributionPlot.InvalidatePlot(True)
    End Sub


    Private Sub ExternalInputActivateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExternalInputActivateToolStripMenuItem.Click
        ExternalInputEnabled = Not ExternalInputEnabled
        net.EnableExternalInput = ExternalInputEnabled

        ExternalInputToolStripMenuItem.Checked = ExternalInputEnabled
    End Sub

    Private Sub LoadEditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadEditToolStripMenuItem.Click
        Dim ExtInputTable As New ExternalInputTable(net.NumCellLato, net.NumCellLato)
        ExtInputTable.ShowDialog()
        net.InputSequence = New ExternalInput(net.NumCellLato, 1)

        net.InputSequence.Data(0).Value = ExtInputTable.Matrix
        'net.InputLayer = ExtInputTable.Matrix
    End Sub

    Private Sub LayerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LayerToolStripMenuItem.Click
        LayerMonitor.Text = "Activity"
        LayerMonitor.Show()
    End Sub

    Private Sub LockSynapticWeightToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LockSynapticWeightToolStripMenuItem.Click
        LockSynapse = Not LockSynapse
        LockSynapticWeightToolStripMenuItem.Checked = LockSynapse
        net.lockSynapticWeight = LockSynapse
    End Sub

    Private Sub ActivationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ActivationToolStripMenuItem.Click
        ActivationMonitor.Text = "Activation"
        ActivationMonitor.Show()
    End Sub

    Private Sub StateSpace3DToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StateSpace3DToolStripMenuItem.Click
        If Not OpenGLBackgroundWorker.IsBusy Then
            OpenGLBackgroundWorker.RunWorkerAsync()
        End If
    End Sub

    Private Sub OpenGLBackgroundWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles OpenGLBackgroundWorker.DoWork
        StateSpace3D.Run()
    End Sub
End Class

