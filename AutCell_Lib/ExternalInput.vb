Public Class ExternalInput
    Public Data() As Layer

    Public Sub New(NumCellLato As UInt32, timeLenght As UInt32)
        ReDim Data(timeLenght - 1)
        For i As Integer = 0 To Data.Length - 1
            Data(i) = New Layer(NumCellLato)
        Next
    End Sub


    Public ReadOnly Property Value(time As UInt32) As Single(,)
        Get
            Return Data(time Mod Data.Length).Value
        End Get

    End Property
End Class

Public Class Layer

    Public Value(,) As Single

    Public Sub New(NumCellLato)
        ReDim Value(NumCellLato - 1, NumCellLato - 1)
    End Sub

End Class
