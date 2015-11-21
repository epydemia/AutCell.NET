Imports System.Math

Public Class CellularAutomateExternalInput
    Inherits CellularAutomata

    ' Questa rete cellulare implementa un livello aggiuntivo di input (modificabile dall'utente) cablato con peso sinaptico definito in
    ' Neuron.ExternalInput sul piano i=0

    Public InputLayer As Single(,)
    Public EnableExternalInput As Boolean = False

    Sub New(config As Configuration)
        MyBase.New(config)
        ReDim InputLayer(NumCellLato - 1, NumCellLato - 1)
    End Sub

    Protected Overrides Sub attivaneurone(i, j, k, sga)
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


        ' verifica se è un neurone di input ed è abilitato lo stimolo esterno
        If i = 0 And EnableExternalInput = True Then
            ' in caso positivo somma anche il valore del layer di input
            nt += Neu(i, j, k).ExternalInputWeight * InputLayer(j, k)
        End If


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
End Class
