Imports System.Math
Imports System.Threading

Public Class CellularAutomata
    Public Const NumeroIntervalliDistribuzione = 100

    Public parallelEnabled As Boolean = False
    Public lockSynapticWeight As Boolean = False

    Public time As UInt32  ' Questo è il tempo numero di ciclo di simulazione

    Public t() As Int16

    Public NetworkConfiguration As Configuration
    Public NumCellLato As UInt32    ' ex ncl -> Numero di cellule per lato (ATTENZIONE in AUTC6 ncl era l'ultimo indice della matrice!!, ncl1 era il numero di celle di lato!!!)
    Public NumLivelli As UInt32     ' ex lv -> Numero di livelli
    Public ReadOnly NumCellIntorno As UInt32 ' ex nci -> Numero di cellule collegato  (ATTENZIONE in AUTC6 ncl era l'ultimo indice della matrice!!, ncl1 era il numero di celle di lato!!!)
    Public Neu As Neuron(,,)
    Public Nsp As Neuron(,,,)  'ex Nsp
    Public wlw(,,,) As Int16
    Public wl() As Int16
    Public d() As Single ' Fattore moltiplicativo di retrodiffusione (Inverso al quadrato della distanza)
    Public dt(,,,) As Single

    'Questi Dati sono utilizzati per monitoring
    Public globalActivity As Single
    Public SynapticWeightDistribution As Integer(,)
    Public ActivityDistribution As Integer()
    Public ThresholdDistribution As Integer()

    Public Sub New(config As Configuration)
        Me.New(config.cellforEachSide, config.levels)
        Me.NetworkConfiguration = New Configuration()
        Me.NetworkConfiguration = config
        time = 0
        Init()
    End Sub


    Private Sub New(CellLato As UInt16, Livelli As UInt16)
        NumCellLato = CellLato
        NumLivelli = Livelli
        Dim ncl = NumCellLato - 1
        ReDim Neu(ncl, ncl, ncl)

        ReDim Nsp(NumLivelli, ncl, ncl, ncl)

        NumCellIntorno = (NumLivelli * 2 + 1) ^ 3 - 1
        CreateNeurons()
        ReDim wlw(ncl, ncl, ncl, NumCellIntorno - 1)  ' Contiene il livello di ogni neurone
        ReDim wl(NumCellIntorno - 1) ' Contiene il livello di ogni sinapsi in un dato neurone
        ReDim d(NumLivelli)
        ReDim dt(ncl, ncl, ncl, NumCellIntorno - 1)

        ReDim t(NumLivelli)

        'thr(0) = New Thread(New ParameterizedThreadStart(AddressOf Update))
        'thr(1) = New Thread(New ParameterizedThreadStart(AddressOf Update))

    End Sub

    ' ex cellule -> Numero totale di cellule nella rete
    Public ReadOnly Property NumCellule
        Get
            Return NumCellLato ^ 3
        End Get
    End Property

    Public ReadOnly Property sgmax As Single
        Get
            Return (NumCellIntorno) * (1 - NetworkConfiguration.r)
        End Get
    End Property

    Public ReadOnly Property sgmd As Single
        Get
            Return (NumCellIntorno) * (1 - 2 * NetworkConfiguration.r)
        End Get
    End Property

    'attività max. raggiungibile dall'intorno
    Public ReadOnly Property ami As Single
        Get
            Return sgmax * (NetworkConfiguration.initialAverageWeight + NetworkConfiguration.vw) * 0.5
        End Get
    End Property

    'attività media dell'intorno compresa fra amd e amd2 (Intervallo di confidenza)
    Public ReadOnly Property amd As Single
        Get
            Return sgmd * (NetworkConfiguration.initialAverageWeight + NetworkConfiguration.vw) * 0.5
        End Get
    End Property

    Public ReadOnly Property amd2 As Single
        Get
            Return sgmd * (NetworkConfiguration.initialAverageWeight - NetworkConfiguration.vw) * 0.5
        End Get
    End Property

    Public ReadOnly Property GetLayer(layerNumber As Integer) As Single(,)
        Get
            Dim Layer(NumCellLato - 1, NumCellLato - 1) As Single
            For j As Integer = 0 To NumCellLato - 1
                For k As Integer = 0 To NumCellLato - 1
                    Layer(j, k) = Neu(layerNumber, j, k).activity
                Next
            Next
            Return Layer
        End Get
    End Property

    Public ReadOnly Property GetActivationLayer(layerNumber As Integer) As Single(,)
        Get
            Dim Layer(NumCellLato - 1, NumCellLato - 1) As Single
            For j As Integer = 0 To NumCellLato - 1
                For k As Integer = 0 To NumCellLato - 1
                    Layer(j, k) = Nsp(t(0), layerNumber, j, k).activity
                Next
            Next
            Return Layer
        End Get
    End Property

    Public Sub UpdateSynaptictWeightDistribution()
        ReDim SynapticWeightDistribution(NetworkConfiguration.levels, NumeroIntervalliDistribuzione)

        Dim MinValue = -1
        Dim MaxValue = 1
        Dim ScaleFactor = NumeroIntervalliDistribuzione / (MaxValue - MinValue)

        For i As Integer = 0 To NumCellLato - 1
            For j As Integer = 0 To NumCellLato - 1
                For k As Integer = 0 To NumCellLato - 1
                    For a As Integer = 0 To NumCellIntorno - 1
                        Dim li = wlw(i, j, k, a) - 1

                        ' Il peso sinaptico è compreso tra -1 e +1, bisogna normalizzarlo tra 0 e 20
                        Dim X As Integer = CInt((Neu(i, j, k).sinapsi(a) - MinValue) * ScaleFactor)
                        SynapticWeightDistribution(li, X) += 1
                    Next
                Next
            Next
        Next

    End Sub

    Public Sub UpdateActivityDistribution()
        Dim MinValue = 0
        Dim MaxValue = 1
        Dim ScaleFactor As Integer = NumeroIntervalliDistribuzione / (MaxValue - MinValue)

        ReDim ActivityDistribution(NumeroIntervalliDistribuzione)
        For i As Integer = 0 To NumCellLato - 1
            For j As Integer = 0 To NumCellLato - 1
                For k As Integer = 0 To NumCellLato - 1
                    Dim X = CInt(Nsp(t(0), i, j, k).activity * ScaleFactor)
                    ActivityDistribution(X) += 1

                Next k
            Next j
        Next i
    End Sub

    Public Sub UpdateThresholdDistribution()
        ' Si scala supponendo come valore centrale il valore massimo raggiungibile
        Dim scaleFactor = NumeroIntervalliDistribuzione '/ (2 * ami)
        ReDim ThresholdDistribution(NumeroIntervalliDistribuzione)

        For i As Integer = 0 To NumCellLato - 1
            For j As Integer = 0 To NumCellLato - 1
                For k As Integer = 0 To NumCellLato - 1

                    Dim X As Integer = CInt(Neu(i, j, k).activity * scaleFactor)
                    If X > NumeroIntervalliDistribuzione Then
                        ReDim Preserve ThresholdDistribution(X)
                    End If
                    ThresholdDistribution(X) += 1

                Next k
            Next j
        Next i

    End Sub

    Private Sub CreateNeurons()
        For i As UInt16 = 0 To NumCellLato - 1
            For j As UInt16 = 0 To NumCellLato - 1
                For k As UInt16 = 0 To NumCellLato - 1
                    'Ogni Neurone avrà una sinapsi per ogni neurone collegato
                    Neu(i, j, k) = New Neuron(NumCellIntorno)

                    For level As Integer = 0 To NumLivelli

                        Nsp(level, i, j, k) = New Neuron(0)
                    Next
                Next
            Next
        Next
    End Sub

    Public Sub Init()
        amps()
        Tempi()
        retrodiffusione()
        sinapsilivelli()
        sinapsilivelli2()
    End Sub

    Private Sub Update(limit As MatrixLimit)
        With limit
            For i As Integer = .startX To .endX
                For j As Integer = .startY To .endY
                    For k As Integer = .startZ To .endZ
                        If IsAlive(i, j, k) Then

                            Select Case NetworkConfiguration.modulation
                                Case Is = TModulation.Amplitude
                                    attivaneurone(i, j, k, NetworkConfiguration.sigmoide, time)
                                Case Is = TModulation.Frequency
                                    ' To Do
                            End Select

                            globalActivity += Nsp(t(0), i, j, k).activity

                        End If
                    Next
                Next
            Next
        End With
    End Sub

    Private Class MatrixLimit
        Public startX
        Public endX
        Public startY
        Public endY
        Public startZ
        Public endZ
    End Class

    Public Sub Update()
        parallelEnabled = False
        globalActivity = 0


#If 1 Then
        For i As Integer = 0 To NumCellLato - 1
            For j As Integer = 0 To NumCellLato - 1
                For k As Integer = 0 To NumCellLato - 1

                    ' If Neuron is not dead
                    If IsAlive(i, j, k) Then

                        Select Case NetworkConfiguration.modulation
                            Case Is = TModulation.Amplitude
                                attivaneurone(i, j, k, NetworkConfiguration.ThresholdGain, time)
                            Case Is = TModulation.Frequency
                                ' To Do
                        End Select

                        globalActivity += Nsp(t(0), i, j, k).activity

                    End If


                Next
            Next
        Next
#End If

        If lockSynapticWeight = False Then
            For i As Integer = 0 To NumCellLato - 1
                For j As Integer = 0 To NumCellLato - 1
                    For k As Integer = 0 To NumCellLato - 1
                        If IsAlive(i, j, k) Then
                            If NetworkConfiguration.Hebb = True Then modpesDH(i, j, k)
                            'If NetworkConfiguration.Kandel = True then modPesK(i,j,k)
                            'If NetworkConfiguration.BackProp = True then modPesR(i,j,k)
                        End If


                    Next
                Next
            Next

            aggiornaSinapsi()

        End If

        Sequenza()

        time += 1

    End Sub

    Public Sub UpdateParallelFor()
        parallelEnabled = True
        globalActivity = 0
        Parallel.For(0, NumCellLato, Sub(i)
                                         For j As Integer = 0 To NumCellLato - 1
                                             For k As Integer = 0 To NumCellLato - 1

                                                 ' If Neuron is not dead
                                                 If IsAlive(i, j, k) Then

                                                     Select Case NetworkConfiguration.modulation
                                                         Case Is = TModulation.Amplitude
                                                             attivaneurone(i, j, k, NetworkConfiguration.ThresholdGain, time)
                                                         Case Is = TModulation.Frequency
                                                             ' To Do
                                                     End Select

                                                     globalActivity += Nsp(t(0), i, j, k).activity

                                                 End If

                                             Next
                                         Next
                                     End Sub)

        Parallel.For(0, NumCellLato, Sub(i)
                                         For j As Integer = 0 To NumCellLato - 1
                                             For k As Integer = 0 To NumCellLato - 1
                                                 If IsAlive(i, j, k) Then
                                                     If NetworkConfiguration.Hebb = True Then modpesDH(i, j, k)
                                                     'If NetworkConfiguration.Kandel = True then modPesK(i,j,k)
                                                     'If NetworkConfiguration.BackProp = True then modPesR(i,j,k)
                                                 End If


                                             Next
                                         Next
                                     End Sub)


        aggiornaSinapsi()
        Sequenza()

    End Sub

    Private Function IsAlive(i, j, k) As Boolean
        If Neu(i, j, k).inactivity < NetworkConfiguration.cla And Neu(i, j, k).inactivity <> -1 Then
            Return True
        Else
            Neu(i, j, k).inactivity = -1
            Return False
        End If
    End Function

    Private Sub sinapsilivelli()
        Dim lv As UInt16 = NumLivelli
        Dim ncl As Integer = NumCellLato - 1
        Dim a As UInt16 = 0
        For i As Integer = 0 To ncl
            For j As Integer = 0 To ncl
                For k As Integer = 0 To ncl
                    a = 0
                    For l As Integer = i - lv To i + lv
                        For m As Integer = j - lv To j + lv
                            For N As Integer = k - lv To k + lv
                                If l <> i Or m <> j Or N <> k Then
                                    wlw(i, j, k, a) = Max(Max(Abs(l - i), Abs(m - j)), Abs(N - k))
                                    a = a + 1
                                End If
                            Next N
                        Next m
                    Next l
                Next k
            Next j
        Next i

    End Sub

    Private Sub sinapsilivelli2()
        Dim a = 0
        Dim lv = NumLivelli
        For l As Integer = -lv To +lv
            For m As Integer = -lv To +lv
                For N As Integer = -lv To +lv
                    If l <> 0 Or m <> 0 Or N <> 0 Then
                        wl(a) = Max(Max(Abs(l), Abs(m)), Abs(N))
                        a = a + 1
                    End If
        Next N, m, l

    End Sub

    Private Sub retrodiffusione()
        For li As Integer = 1 To NumLivelli
            d(li) = 1 / (li ^ 2)
        Next li
    End Sub

    Private Sub amps()
        Dim r = NetworkConfiguration.r
        Dim lv = NetworkConfiguration.levels
        Dim ncl = NetworkConfiguration.cellforEachSide - 1
        Dim nci = NetworkConfiguration.nci
        Dim vw = NetworkConfiguration.vw
        Dim vmw = NetworkConfiguration.initialAverageWeight

        'inizializza i neuroni e le sinapsi con valori casuali
        For li = 0 To lv
            For i = 0 To ncl
                For j = 0 To ncl
                    For k = 0 To ncl

                        Nsp(li, i, j, k).activity = Rnd()
                    Next k
                Next j
            Next i
        Next li

        For i = 0 To ncl
            For j = 0 To ncl
                For k = 0 To ncl
                    Neu(i, j, k).activity = 0.001
                Next k
            Next j
        Next i

        Select Case NetworkConfiguration.tipoSinapsi
            Case Is = TTipoSinapsi.Exciter_AND_Inhibiter
                For i = 0 To ncl
                    For j = 0 To ncl
                        For k = 0 To ncl
                            For a = 0 To nci
                                'w(i, j, k, a) = (Rnd() * Sgn(Rnd() - 0.5) * vw + vmw) * Sgn(Rnd() - r)
                                Neu(i, j, k).sinapsi(a) = (Rnd() * Math.Sign(Rnd() - 0.5) * vw * vmw) * Math.Sign(Rnd() - r)


                            Next a
                        Next k
                    Next j
                Next i


            Case Is = TTipoSinapsi.Exciter_OR_Inhibiter
                Dim a = nci
                For i = 0 To ncl
                    For j = 0 To ncl
                        For k = 0 To ncl
                            Dim seg = Math.Sign(Rnd() - r)
                            For l = i - lv To i + lv
                                'l1 = limiti(l)
                                For m = j - lv To j + lv
                                    'm1 = limiti(m)
                                    For n = k - lv To k + lv
                                        'If l = i And m = j And N = k Then N = N + 1
                                        If l <> i Or m <> j Or n <> k Then
                                            'n1 = limiti(N)
                                            'w(l1, m1, n1, a) = (Rnd() * Sgn(Rnd() - 0.5) * vw + vmw) * seg
                                            'a = a - 1
                                            Neu(limiterWrap(l), limiterWrap(m), limiterWrap(n)).sinapsi(a) = (Rnd() * Math.Sign(Rnd() - 0.5) * vw + vmw) * seg
                                            a = a - 1

                                        End If
                                    Next n
                                Next m
                            Next l

                            a = nci
                        Next k
                    Next j
                Next i

        End Select
    End Sub

    Private Sub Tempi()
        For i As Integer = 0 To NumLivelli
            t(i) = i
        Next
    End Sub

    Sub Sequenza()
        For li As Integer = 0 To NumLivelli
            t(li) = t(li) - 1
            If t(li) = -1 Then t(li) = NumLivelli
        Next li
    End Sub

    Protected Overridable Sub attivaneurone(i, j, k, sga, time)
        Dim lv As Integer = NumLivelli
        Dim ar = NetworkConfiguration.sigmoide
        Dim pr = NetworkConfiguration.pr
        Dim a As Integer = 0
        Dim nt As Single
        Dim l1, m1, n1 As Integer
        For l As Integer = i - lv To i + lv
            l1 = limiterWrap(l)
            For m As Integer = j - lv To j + lv
                m1 = limiterWrap(m)
                For n As Integer = k - lv To k + lv
                    If l <> i Or m <> j Or n <> k Then
                        n1 = limiterWrap(n)

                        Dim li As Integer = wlw(i, j, k, a)

                        nt = nt + Neu(i, j, k).sinapsi(a) * Nsp(t(li), l1, m1, n1).activity
                        a = a + 1
                    End If
                Next n
            Next m
        Next l



        'nt = sga * nt / 100000000.0 : sg099 = s(0, i, j, k) / 100 ' Se nt e sga sono normalizzate a 1, non è chiaro perchè sg099 sia normalizzato a 100
        nt = sga * nt
        Dim sg099 = Neu(i, j, k).activity * 100

        Dim at = 1 / (1 + Math.Exp(-(nt - sg099) / ar)) ' tanto più che qui sg099 viene sottratto a nt...
        Nsp(t(0), i, j, k).activity = at

        'sg099 = s(0, i, j, k) + sg099 * (at / 0.9) * 100 - s(0, i, j, k) / pr
        's(0, i, j, k) = sg099
        Neu(i, j, k).activity = Neu(i, j, k).activity + sg099 / 100 * (at / 0.9) - Neu(i, j, k).activity / pr
        ' sg099 * (at / 0.9) / 100 -> Nella linea di codice sopra sga e' stato diviso per 100 per normalizzare all'unità


        'If ABS(nt) < 1 And ABS(s(0, i, j, k)) < 2 Then s(1, i, j, k) = s(1, i, j, k) + 1 Else s(1, i, j, k) = 0

        'Se la cellula è inattiva incremento il contatore di inattività
        If Abs(nt) < 1 And Abs(Neu(i, j, k).activity) < 2 Then
            Neu(i, j, k).inactivity += 1

        Else ' Altrimenti lo resetto
            Neu(i, j, k).inactivity = 0

        End If

    End Sub

    Public Sub modpesDH(i, j, k)
        Dim e = NetworkConfiguration.e
        Dim lv = NetworkConfiguration.levels
        Dim a = 0
        Dim l1, m1, n1 As Integer
        For l = i - lv To i + lv
            l1 = limiterWrap(l)
            For m = j - lv To j + lv
                m1 = limiterWrap(m)
                For n = k - lv To k + lv
                    If l = i And m = j And n = k Then n = n + 1
                    n1 = limiterWrap(n)
                    'li = wl(a)
                    Dim pn = Nsp(wl(a), l1, m1, n1).activity
                    pn = pn * Nsp(0, i, j, k).activity
                    ' dh1 = 0.000001 * (pn# * Sgn(w(i, j, k, a)) - E * w(i, j, k, a))
                    'dt1 = dt(i, j, k, a)
                    'dt(i, j, k, a) = dt1 + dh1

                    dt(i, j, k, a) = dt(i, j, k, a) + _
                        pn * Sign(Neu(i, j, k).sinapsi(a)) - e * Neu(i, j, k).sinapsi(a)


                    a = a + 1
                Next n
            Next m
        Next l
    End Sub

    Private Sub aggiornaSinapsi()

        Dim ncl = NumCellLato - 1
        If parallelEnabled = False Then
            For i = 0 To ncl
                For j = 0 To ncl
                    For k = 0 To ncl

                        For a As Integer = 0 To NumCellIntorno - 1

                            'nw = w(i, j, k, a) + dt(i, j, k, a)
                            'If nw > 10000 Then
                            ' nw = 10000
                            'ElseIf nw < -10000 Then
                            ' nw = -10000
                            ' End If
                            'w(i, j, k, a) = nw
                            Neu(i, j, k).sinapsi(a) = Limiter(Neu(i, j, k).sinapsi(a) + dt(i, j, k, a), -1, 1)

                        Next a




                    Next k
                Next j
            Next i
        Else

            Parallel.For(0, NumCellLato - 1, Sub(i)
                                                 'Dim ncl = NumCellLato - 1

                                                 For j = 0 To ncl
                                                     For k = 0 To ncl

                                                         For a As Integer = 0 To NumCellIntorno - 1

                                                             'nw = w(i, j, k, a) + dt(i, j, k, a)
                                                             'If nw > 10000 Then
                                                             ' nw = 10000
                                                             'ElseIf nw < -10000 Then
                                                             ' nw = -10000
                                                             ' End If
                                                             'w(i, j, k, a) = nw
                                                             Neu(i, j, k).sinapsi(a) = Limiter(Neu(i, j, k).sinapsi(a) + dt(i, j, k, a), -1, 1)

                                                         Next a




                                                     Next k
                                                 Next j

                                             End Sub)

            'a = 0
        End If
        ReDim dt(ncl, ncl, ncl, NumCellIntorno - 1)
    End Sub

    Public Function Limiter(dato As Single, min As Single, max As Single) As Single
        If dato < min Then Return min
        If dato > max Then Return max
        Return dato
    End Function

    Protected Function limiterWrap(l As Integer) As Integer
        Return (l + NumCellLato) Mod (NumCellLato)
    End Function
End Class
