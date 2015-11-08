Imports AutCell_Lib
Module Module1

    Sub Main()
#If 0 Then
        Dim parametri As ParametriRete
        parametri = New ParametriRete()
        parametri.ReadFromFile("testfile.uc3")
        parametri.toXMLFile("TestFile.xml")
#End If
        Dim c As Configuration = New Configuration()
        'c.cellforEachSide = parametri.ParametriRete.ncl
        'c.levels = parametri.ParametriRete.lv
        c.fromXMLFile("prova.xml")
        Dim net As CellularAutomata
        net = New CellularAutomata(c)
        Console.WriteLine(net.sgmax)

        Dim sw As New Stopwatch
        sw.Start()
        For i As Integer = 0 To 1000
            net.Update()
            Console.WriteLine(net.globalActivity)
        Next
        sw.Stop()
        Console.WriteLine("Tempo di esecuzione {0} secondi", sw.ElapsedMilliseconds / 1000)
        Console.ReadLine()
        'c.toXMLFile("prova.xml")

    End Sub

End Module
