Imports System.Xml.Serialization
Imports System.IO


Public Class Configuration
#If 0 Then
    Public lv As Integer ' livelli dell'intorno di un neurone;
    Public nci As Integer ' numero di cellule dell'intorno del neurone
    Public ncl As Integer ' numero di cellule di lato del cubo della popolazione di neuroni (ATTENZIONE zero-based)
    Public nr As Integer ': numero di neuroni e sinapsi memorizzati su HD;
    Public pr As Integer ': periodo refrattario del neurone;
    Public ec_in As Integer ': termini del rapporto fra sinapsi eccitatorie ed inibitorie;
    Public y1 As Integer ': numero previsto di cicli
    Public seme As Integer ': seme usato dalla funzione casuale di attribuzione iniziale dei pesi sinaptici e delle attività dei neuroni;
    Public ru, rd, rt As Integer ' indicano la modalità di modifica dei pesi sinaptici a seconda delle leggi di Hebb, Kandel, retrodiffusione;
    Public cla ' numero di cicli necessario perché una cellula inattiva diventi morta;
    Public sga As Integer ': amplificazione del'input al neurone;	
    Public cam As Integer ': numero di cicli ad attività globale costante oltre i quali la popolazione esibisce attività monomorfica (usato da AutSeq)
    Public ar As Integer ': pendenza della sigmoide di risposta del neurone all'input fornito dall'intorno;
    Public vmw As Integer ': valore medio iniziale dei pesi sinaptici;
    Public vw As Integer ': variabilità dei pesi sinaptici iniziali;
    Public sm As Integer ': soglia per attività monomorfica (usato da AutSeq);
    Public e As Integer ': coefficiente di dimenticanza sotto al quale la sinapsi diminuisce di peso;
    Public nf As String ' nome del file, attribuito dal programma, a cui corrisponde una simulazione;

#End If

    Public cellforEachSide As UInt16    ' ex ncl -> Number of cell for each side of the cube
    Public levels As UInt16         ' ex lv -> Number of levels for each neuron
    Public initialAverageWeight As Single   'ex vmw -> valore medio iniziale dei pesi sinaptici (AUTC6 era int moltiplicato x 10000)
    Public variabilityPercent As Single  'ex vw read from User!
    Public coeffDimenticanza As Single ' ex e read from user!
    Public ec, inib As Single ' ec in -> used for ratio exciter/inhibiter
    Public Hebb, Kandel, BackProp As Boolean
    Public DecadimentoRetrodifussione As Single 'ex der -> Decadimento della concentrazione del fattore di retrodiffusione (1.1-10)
    Public ThresholdGain As Single 'ex sga -> Coefficiente di amplificazione della soglia
    Public sigmoide As Single ' ex ar -> Coefficiente di pendenza della sigmoide (1-8)
    Public pr As Single ' ex pr -> Periodo refrattario dopo spike > 90%
    Public cla As Single ' ex cla -> cicli per apoptosi
    Public tipoSinapsi As TTipoSinapsi
    Public modulation As TModulation

    Public ReadOnly Property vw As Single ' Thjs is the REAL vw that must used!!!
        Get
            Return variabilityPercent / 100 * initialAverageWeight
        End Get
    End Property

    Public ReadOnly Property e As Single
        Get
            Return coeffDimenticanza * coeffDimenticanza
        End Get
    End Property

    Public ReadOnly Property r As Single  'exciter/inhibiter ratio
        Get
            Return inib / (ec + inib)
        End Get
    End Property

    Public ReadOnly Property nci
        Get
            Return (levels * 2 + 1) ^ 3 - 1 - 1
        End Get
    End Property

    

    Public ReadOnly Property ru As UInt16
        Get
            If Hebb Then
                Return 1
            Else
                Return 0
            End If

        End Get
    End Property

    Public ReadOnly Property rd As UInt16
        Get
            If Kandel Then
                Return 2
            Else
                Return 0
            End If

        End Get
    End Property

    Public ReadOnly Property rt As UInt16
        Get
            If Kandel Then
                Return 3
            Else
                Return 0
            End If

        End Get
    End Property
    Public Sub toXMLFile(XMLFileName As String)
        Dim ser As New XmlSerializer(Me.GetType())
        Dim str As New FileStream(XMLFileName, FileMode.Create)

        ser.Serialize(str, Me)
        str.Flush()
        str.Close()
    End Sub

    Public Sub fromXMLFile(XMLFileName As String)
        Dim ser As New XmlSerializer(Me.GetType())
        Dim str As New FileStream(XMLFileName, FileMode.Open)

        Dim temp As Configuration = ser.Deserialize(str)
        Me.cellforEachSide = temp.cellforEachSide
        Me.levels = temp.levels
        Me.initialAverageWeight = temp.initialAverageWeight
        Me.variabilityPercent = temp.variabilityPercent
        Me.coeffDimenticanza = temp.coeffDimenticanza
        Me.ec = temp.ec
        Me.inib = temp.inib
        Me.Hebb = temp.Hebb
        Me.Kandel = temp.Kandel
        Me.BackProp = temp.BackProp
        Me.DecadimentoRetrodifussione = temp.DecadimentoRetrodifussione
        Me.ThresholdGain = temp.ThresholdGain
        Me.sigmoide = temp.sigmoide
        Me.pr = temp.pr
        Me.cla = temp.cla
        Me.tipoSinapsi = temp.tipoSinapsi
        str.Close()
    End Sub
End Class

Public Enum TTipoSinapsi
    Exciter_AND_Inhibiter = 1
    Exciter_OR_Inhibiter = 2
End Enum

Public Enum TModulation
    Amplitude
    Frequency
End Enum