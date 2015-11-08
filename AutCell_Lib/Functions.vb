Imports System.Math
Module Functions

    Public Enum TipoSinapsi
        '1-eccitatorie e inibitorie  2-solo eccitatori o solo inibitori
        EccitatorieInibitorie = 1
        SoloEccitatorie_SoloInibitorie = 2
    End Enum

    Public Sub Retrodiffusione(lv As Integer, ByRef d() As Double)
        'Call BarraMonitor("Inizio Retrodiffusione...")
        For li = 1 To lv
            d(li) = 1 / li ^ 2
        Next li
        'Call BarraMonitor("Retrodiffusione completata")
    End Sub


    Public Sub sinapsilivelli(ncl As Integer, lv As Integer, ByRef wlw(,,,) As Integer)
        'associa in wlw (ijka) ogni sinapsi al livello che occupa rispetto al neurone centrale
        'Call BarraMonitor("Inizializzazione Sinapsi 1...")
        Dim a As Integer

        For i = 0 To ncl
            For j = 0 To ncl
                For k = 0 To ncl
                    a = 0
                    For l = i - lv To i + lv
                        For m = j - lv To j + lv
                            For n = k - lv To k + lv
                                'If l = i And m = j And n = k Then n = n + 1
                                'Dim ll = Abs(l - i)
                                'Dim lm = Abs(m - j)
                                'Dim ln = Abs(n - k)
                                'Dim li = ll
                                'If lm > li Then li = lm
                                'If ln > li Then li = ln
                                'wlw(i, j, k, a) = li
                                'a=a+1

                                ' Compattate in modo più leggibile le linee di codice qui sopra (Daniele)
                                If l <> i Or m <> j Or n <> k Then ' Se non si sta processando il punto della rete coincidente con il neurone
                                    wlw(i, j, k, a) = Max(Abs(l - i), Max(Abs(m - j), Abs(n - k)))
                                    a += 1
                                End If
                            Next n
                        Next m
                    Next l
                Next k
            Next j
        Next i
        'Call BarraMonitor("Inizializzazione Sinapsi 1...terminata")
    End Sub

    Public Sub sinapsilivelli2(lv As Integer, ByRef wl() As Integer)
        'associa in wl (a) ogni neurone al livello che occupa nell'intorno
        'Call BarraMonitor("Inizializzazione Sinapsi 2...")

        ' La seguenti 2 righe sono state aggiunta per eliminare errore in compilazione, ma temo che ci sia un bug (Daniele)
        Dim i, j, k As Integer
        Dim a As Integer = 0

        For l = i - lv To i + lv
            For m = j - lv To j + lv
                For n = k - lv To k + lv
                    'If l = i And m = j And n = k Then n = n + 1
                    'll = Abs(l - i)
                    'lm = Abs(m - j)
                    'ln = Abs(n - k)
                    'li = ll
                    'If lm > li Then li = lm
                    'If ln > li Then li = ln
                    'wl(a) = li
                    'a = a + 1

                    ' Compattate in modo più leggibile le linee di codice qui sopra (Daniele)
                    If l <> i Or m <> j Or n <> k Then ' Se non si sta processando il punto della rete coincidente con il neurone
                        wl(a) = Max(Abs(l - i), Max(Abs(m - j), Abs(n - k)))
                        a += 1
                    End If

                Next n
            Next m
        Next l
        'Call BarraMonitor("Inizializzazione Sinapsi 2...terminata")
    End Sub

    Public Sub aggiornasinapsi(ncl As Integer, nci As Integer, ByRef w(,,,) As Integer, ByRef dt(,,,) As Integer)
        For i = 0 To ncl
            For j = 0 To ncl
                For k = 0 To ncl
                    For a = 0 To nci
                        Dim nw As Integer = w(i, j, k, a) + dt(i, j, k, a)
                        If nw > 10000 Then
                            nw = 10000
                        ElseIf nw < -10000 Then
                            nw = -10000
                        End If
                        w(i, j, k, a) = nw
                    Next a
                Next k
            Next j
        Next i

        'a = 0
        ReDim dt(ncl, ncl, ncl, nci) 'As Integer
    End Sub

    Public Sub amps(r As Double, lv As Integer, ncl As Integer, nci As Integer, vw As Single, vmw As Single, optEI As TipoSinapsi, _
                    ByRef s(,,,) As Integer, ByRef sp(,,,) As Integer, ByRef w(,,,) As Integer)

        'inizializza i neuroni e le sinapsi con valori casuali
        For li = 0 To lv
            For i = 0 To ncl
                For j = 0 To ncl
                    For k = 0 To ncl
                        sp(li, i, j, k) = Rnd() * 10000
                    Next k
                Next j
            Next i
        Next li

        For i = 0 To ncl
            For j = 0 To ncl
                For k = 0 To ncl
                    s(0, i, j, k) = 10
                Next k
            Next j
        Next i

        Select Case optEI
            Case TipoSinapsi.EccitatorieInibitorie
                For i = 0 To ncl
                    For j = 0 To ncl
                        For k = 0 To ncl
                            For a = 0 To nci
                                w(i, j, k, a) = (Rnd() * Sign(Rnd() - 0.5) * vw + vmw) * Sign(Rnd() - r)
                Next a, k, j, i

            Case TipoSinapsi.SoloEccitatorie_SoloInibitorie
                Dim a = nci
                For i = 0 To ncl
                    For j = 0 To ncl
                        For k = 0 To ncl
                            Dim seg = Sign(Rnd() - r)
                            For l = i - lv To i + lv
                                Dim l1 = limiti(l, ncl)
                                For m = j - lv To j + lv
                                    Dim m1 = limiti(m, ncl)
                                    For n = k - lv To k + lv
                                        If l = i And m = j And n = k Then n = n + 1
                                        Dim n1 = limiti(n, ncl)
                                        w(l1, m1, n1, a) = (Rnd() * Sign(Rnd() - 0.5) * vw + vmw) * seg
                                        a = a - 1
                            Next n, m, l
                            a = nci
                Next k, j, i

        End Select
    End Sub

    Public Sub attivaneurone(i As Integer, j As Integer, k As Integer, ncl As Integer, lv As Integer, sga As Integer, ar As Single, pr As Integer, _
                             ByRef w(,,,) As Integer, ByRef wlw(,,,) As Integer, ByRef sp(,,,) As Integer, ByRef s(,,,) As Integer, ByRef t() As Integer)

        Dim nt As Integer
        Dim a = 0

        For l = i - lv To i + lv
            Dim l1 = limiti(l, ncl)
            For m = j - lv To j + lv
                Dim m1 = limiti(m, ncl)
                For n = k - lv To k + lv
                    If l = i And m = j And n = k Then n = n + 1
                    Dim n1 = limiti(n, ncl)
                    Dim wo = w(i, j, k, a)
                    Dim li = wlw(i, j, k, a)
                    Dim u = sp(t(li), l1, m1, n1)
                    nt = nt + wo * u
                    a = a + 1
                Next n
            Next m
        Next l


        nt = sga * nt / 100000000.0#
        Dim sg099 = s(0, i, j, k)
        Dim at = 1 / (1 + Exp(-(nt - sg099) / ar))
        sp(t(0), i, j, k) = at * 10000
        sg099 = s(0, i, j, k) + (at / 0.9) - s(0, i, j, k) / pr
        s(0, i, j, k) = sg099
        If Abs(nt) < 1 And Abs(s(0, i, j, k)) < 2 Then
            s(1, i, j, k) = s(1, i, j, k) + 1
        Else
            s(1, i, j, k) = 0
        End If
    End Sub


    Public Function limiti(pj As Integer, ncl As Integer)
        Return (pj + ncl + 1) Mod (ncl + 1)
#If 0 Then
        Select Case pj

            Case Is < 0
                limiti = ncl + 1 + pj

            Case Is > ncl
                limiti = pj - (ncl + 1)

            Case Else
                limiti = pj

        End Select
#End If
    End Function

End Module
