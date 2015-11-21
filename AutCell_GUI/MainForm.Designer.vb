<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla nell'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.BackgroundWorker2 = New System.ComponentModel.BackgroundWorker()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripProgressBar1 = New System.Windows.Forms.ToolStripProgressBar()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.StartToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Splitter2 = New System.Windows.Forms.Splitter()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.GlobalActivity = New OxyPlot.WindowsForms.Plot()
        Me.DistribuzionePesi = New OxyPlot.WindowsForms.Plot()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.ActivityDistribution = New OxyPlot.WindowsForms.Plot()
        Me.StatusStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.SuspendLayout()
        '
        'BackgroundWorker2
        '
        Me.BackgroundWorker2.WorkerReportsProgress = True
        Me.BackgroundWorker2.WorkerSupportsCancellation = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ToolStripStatusLabel2, Me.ToolStripProgressBar1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 490)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(905, 22)
        Me.StatusStrip1.TabIndex = 3
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(56, 17)
        Me.ToolStripStatusLabel1.Text = "StatusBar"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(41, 17)
        Me.ToolStripStatusLabel2.Text = "Spring"
        '
        'ToolStripProgressBar1
        '
        Me.ToolStripProgressBar1.Name = "ToolStripProgressBar1"
        Me.ToolStripProgressBar1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ToolStripProgressBar1.Size = New System.Drawing.Size(100, 16)
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StartToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(905, 24)
        Me.MenuStrip1.TabIndex = 4
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'StartToolStripMenuItem
        '
        Me.StartToolStripMenuItem.Name = "StartToolStripMenuItem"
        Me.StartToolStripMenuItem.Size = New System.Drawing.Size(43, 20)
        Me.StartToolStripMenuItem.Text = "Start"
        '
        'Splitter2
        '
        Me.Splitter2.Location = New System.Drawing.Point(0, 24)
        Me.Splitter2.Name = "Splitter2"
        Me.Splitter2.Size = New System.Drawing.Size(3, 466)
        Me.Splitter2.TabIndex = 7
        Me.Splitter2.TabStop = False
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer1.Size = New System.Drawing.Size(902, 466)
        Me.SplitContainer1.SplitterDistance = 233
        Me.SplitContainer1.TabIndex = 11
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.GlobalActivity)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.DistribuzionePesi)
        Me.SplitContainer2.Size = New System.Drawing.Size(902, 233)
        Me.SplitContainer2.SplitterDistance = 485
        Me.SplitContainer2.TabIndex = 0
        '
        'GlobalActivity
        '
        Me.GlobalActivity.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GlobalActivity.KeyboardPanHorizontalStep = 0.1R
        Me.GlobalActivity.KeyboardPanVerticalStep = 0.1R
        Me.GlobalActivity.Location = New System.Drawing.Point(0, 0)
        Me.GlobalActivity.Name = "GlobalActivity"
        Me.GlobalActivity.PanCursor = System.Windows.Forms.Cursors.Hand
        Me.GlobalActivity.Size = New System.Drawing.Size(485, 233)
        Me.GlobalActivity.TabIndex = 11
        Me.GlobalActivity.Text = "GlobalActivity"
        Me.GlobalActivity.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE
        Me.GlobalActivity.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE
        Me.GlobalActivity.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS
        '
        'DistribuzionePesi
        '
        Me.DistribuzionePesi.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DistribuzionePesi.KeyboardPanHorizontalStep = 0.1R
        Me.DistribuzionePesi.KeyboardPanVerticalStep = 0.1R
        Me.DistribuzionePesi.Location = New System.Drawing.Point(0, 0)
        Me.DistribuzionePesi.Name = "DistribuzionePesi"
        Me.DistribuzionePesi.PanCursor = System.Windows.Forms.Cursors.Hand
        Me.DistribuzionePesi.Size = New System.Drawing.Size(413, 233)
        Me.DistribuzionePesi.TabIndex = 12
        Me.DistribuzionePesi.Text = "Synaptic Weight Distribution"
        Me.DistribuzionePesi.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE
        Me.DistribuzionePesi.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE
        Me.DistribuzionePesi.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.ActivityDistribution)
        Me.SplitContainer3.Size = New System.Drawing.Size(902, 229)
        Me.SplitContainer3.SplitterDistance = 479
        Me.SplitContainer3.TabIndex = 0
        '
        'ActivityDistribution
        '
        Me.ActivityDistribution.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ActivityDistribution.KeyboardPanHorizontalStep = 0.1R
        Me.ActivityDistribution.KeyboardPanVerticalStep = 0.1R
        Me.ActivityDistribution.Location = New System.Drawing.Point(0, 0)
        Me.ActivityDistribution.Name = "ActivityDistribution"
        Me.ActivityDistribution.PanCursor = System.Windows.Forms.Cursors.Hand
        Me.ActivityDistribution.Size = New System.Drawing.Size(479, 229)
        Me.ActivityDistribution.TabIndex = 12
        Me.ActivityDistribution.Text = "Plot1"
        Me.ActivityDistribution.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE
        Me.ActivityDistribution.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE
        Me.ActivityDistribution.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(905, 512)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Splitter2)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.Text = "Form1"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BackgroundWorker2 As System.ComponentModel.BackgroundWorker
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripProgressBar1 As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents StartToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Splitter2 As System.Windows.Forms.Splitter
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents GlobalActivity As OxyPlot.WindowsForms.Plot
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents DistribuzionePesi As OxyPlot.WindowsForms.Plot
    Friend WithEvents ActivityDistribution As OxyPlot.WindowsForms.Plot

End Class
