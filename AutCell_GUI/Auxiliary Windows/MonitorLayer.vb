Imports AutCell_Lib
Public Class MonitorLayer
    Public LayerBitMap As Bitmap(,)
    Public LayerPictureBox As PictureBox(,)
    Public Gain As Single = 255
    Private Rows, Columns As Integer
    Public Sub New(Rows As Integer, Columns As Integer)
        Me.Rows = Rows
        Me.Columns = Columns

        InitializeComponent()
        'TableLayoutPanel1.RowCount = Rows
        'TableLayoutPanel1.ColumnCount = Columns
        ReDim LayerPictureBox(Rows - 1, Columns - 1)
        ReDim LayerBitMap(Rows - 1, Columns - 1)

        With TableLayoutPanel1

            For i As Integer = 0 To Rows - 1
                For j As Integer = 0 To Columns - 1
                    LayerPictureBox(i, j) = New PictureBox()
                    With LayerPictureBox(i, j)
                        .Dock = DockStyle.Fill
                        .SizeMode = PictureBoxSizeMode.StretchImage
                    End With

                    LayerBitMap(i, j) = New Bitmap(200, 200)
                    .Controls.Add(LayerPictureBox(i, j), j, i)
                Next
            Next
        End With

    End Sub

    Private Sub MonitorLayer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        

#If 0 Then
        'Used to Test CreateImageFromMatrix
        Dim m(,) As Single = {{0.5, 0, 0.75, 0, 0.25, 0, 1, 0, 1, 0}, _
                                {0, 1, 0, 1, 0, 1, 0, 1, 0, 1}, _
                                {1, 0, 1, 0, 1, 0, 1, 0, 1, 0}, _
                                {0, 1, 0, 1, 0, 1, 0, 1, 0, 1}, _
                                {1, 0, 1, 0, 1, 0, 1, 0, 1, 0}, _
                                {0, 1, 0, 1, 0, 1, 0, 1, 0, 1}, _
                                {1, 0, 1, 0, 1, 0, 1, 0, 1, 0}, _
                                {0, 1, 0, 1, 0, 1, 0, 1, 0, 1}, _
                                {1, 0, 1, 0, 1, 0, 1, 0, 1, 0}, _
                                {0, 1, 0, 1, 0, 1, 0, 1, 0, 1}}
        Dim index As Integer = 3
        CreateImageFromMatrix(m, LayerBitMap(GetRowFromIndex(index), GetColumnsFromIndex(index)))
        LayerPictureBox(GetRowFromIndex(index), GetColumnsFromIndex(index)).Image = LayerBitMap(GetRowFromIndex(index), GetColumnsFromIndex(index))
#End If

    End Sub

    Public Sub UpdateWindow(ByRef net As CellularAutomata)

        For i As Integer = 0 To net.NumCellLato - 1
            Dim row = GetRowFromIndex(i)
            Dim col = GetColumnsFromIndex(i)

            CreateImageFromMatrix(net.GetLayer(i), LayerBitMap(row, col))
            LayerPictureBox(row, col).Image = LayerBitMap(row, col)


        Next
    End Sub

    Public Sub CreateImageFromMatrix(ByVal Matrix As Single(,), ByRef Image As Bitmap)
        Dim gfx As Graphics = Graphics.FromImage(Image)
        Dim pW As Integer = Image.Width / (Matrix.GetUpperBound(1) + 1) 'pixelPerElementWidth 
        Dim pH As Integer = Image.Height / (Matrix.GetUpperBound(0) + 1) 'pixedPerElementHeight 

        For i As Integer = 0 To Matrix.GetUpperBound(0)
            For j As Integer = 0 To Matrix.GetUpperBound(1)

                Dim val = Limit(Convert.ToInt16(Matrix(i, j) * Gain), 255)
                Dim brush As SolidBrush
                If val < 255 Then
                    brush = New SolidBrush(Color.FromArgb(val, val, val))
                Else
                    brush = New SolidBrush(Color.FromArgb(255))
                End If
                gfx.FillRectangle(Brush, j * pW, i * pH, pW, pH)

            Next
        Next

    End Sub


    Private Function GetRowFromIndex(index As Integer) As Integer
        Return index \ Columns
    End Function

    Private Function GetColumnsFromIndex(index As Integer) As Integer
        Return index Mod Columns
    End Function

    Private Function Limit(value, maxValue)
        If value < maxValue Then
            Return value
        End If
        Return maxValue
    End Function
End Class