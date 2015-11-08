Public Class Neuron
    Public activity As Single
    Public sinapsi As Single()
    Public inactivity As Integer 'Ex s(1,*,*,*) -> Numero di cicli di inattività

    Public Sub New(NumSinapsi)
        ReDim sinapsi(NumSinapsi - 1)
    End Sub
End Class
