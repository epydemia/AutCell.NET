﻿Public Class Neuron
    Public activity As Single       ' Attività del neurone
    Public sinapsi As Single()      ' Peso sinapsi afferrente
    Public inactivity As Integer 'Ex s(1,*,*,*) -> Numero di cicli di inattività
    Public ExternalInputWeight As Single = 1.0  ' Sinapsi afferente con peso massimo per iniettare stimoli esterni

    Public Sub New(NumSinapsi)
        ReDim sinapsi(NumSinapsi - 1)
    End Sub
End Class
