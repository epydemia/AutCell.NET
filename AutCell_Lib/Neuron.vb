Public Class Neuron
    Public _activity As Single       ' Attività del neurone
    Public sinapsi As Single()      ' Peso sinapsi afferrente
    Public inactivity As Integer 'Ex s(1,*,*,*) -> Numero di cicli di inattività
    Public ExternalInputWeight As Single = 10.0  ' Sinapsi afferente con peso massimo per iniettare stimoli esterni
    Public ciclo = 0
    Private sumActivity As Single = 0

    Public Sub New(NumSinapsi)
        ReDim sinapsi(NumSinapsi - 1)
    End Sub

    Public Property activity As Single
        Get
            Return _activity
        End Get
        Set(value As Single)
            _activity = value
            ciclo += 1
            sumActivity += _activity
        End Set
    End Property

    Public ReadOnly Property AverageActivty As Single
        Get
            If ciclo <> 0 Then
                Return sumActivity / ciclo
            Else
                Return 0
            End If
        End Get
    End Property

End Class
