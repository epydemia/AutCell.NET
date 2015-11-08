Imports System.Runtime.InteropServices
Imports System.Collections.Specialized
Imports System.IO
Imports System.Xml.Serialization

' Questa struttura serve per leggere i file UC3 creati con la versione QuickBasic di AUTCELL
<StructLayout(LayoutKind.Explicit, Pack:=1)> _
Public Structure TParametri
    <FieldOffset(0)> Public ncl As Single
    <FieldOffset(4)> Public nci As Single
    <FieldOffset(8)> Public lv As Single
    <FieldOffset(12)> Public ar As Single
    <FieldOffset(16)> Public e As Single
    <FieldOffset(20)> Public e1 As Single
    <FieldOffset(24)> Public der As Single
    <FieldOffset(28)> Public seme As Single
    <FieldOffset(32)> Public ec As Single
    <FieldOffset(36)> Public inib As Single
    <FieldOffset(40)> Public vmw As Single
    <FieldOffset(44)> Public vw As Single
    <FieldOffset(48)> Public pr As Single
    <FieldOffset(52)> Public ru As Single
    <FieldOffset(56)> Public rd As Single
    <FieldOffset(60)> Public rt As Single
    <FieldOffset(64)> Public nr As Single
    <FieldOffset(68)> Public cla As Single
    <FieldOffset(72)> Public sga As Single
End Structure

' questa qui sotto è la funzione originale di ELACELL6.BAS per la lettura del file .UC3

#If 0 Then
    OPEN nf$ FOR BINARY AS #6
IF ec = -1 THEN CLOSE #6: GOTO finerec
GET #6, , ncl: GET #6, , nci: GET #6, , lv: GET #6, , ar
GET #6, , e: GET #6, , e1
GET #6, , der: GET #6, , seme: GET #6, , ec: GET #6, , in
GET #6, , vmw: GET #6, , vw: GET #6, , pr
GET #6, , ru: GET #6, , rd: GET #6, , rt: GET #6, , nr
GET #6, , cla: GET #6, , sga
CLOSE #6
#End If

Public Class ParametriRete
    'ATTENZIONE!!! Quando più variabili sono definite su una sola linea, solo l'ultima è del tipo specificato, le altre sono del tipo di default di QB45 (in questo caso "Single")

    'COMMON SHARED le, lv, lvi, lvf, t1, nci, ncl, ncli, nclf, nr, pr, pri, prf, intercl, intercla, interpr AS INTEGER
    'COMMON SHARED ec, eci, ecf, in, ini, inf, y1, seme, semei, semef, ru, rd, rt, cla, clai, claf, sga, sgai, sgaf, cam AS INTEGER
    'COMMON SHARED ar, ari, arf, vmw, vmwi, vmwf, vw, vwi, vwf, intervar, intervmw, intervw, interve, interder, sm, e, e1, ei, ef, der, deri, derf AS SINGLE
    'COMMON SHARED nf$
    ' " cellule di lato (3-10) "; ncl
    ' "livelli di intorno (1-3)"; lv
    ' "Pendenza della sigmoide di attivazione dei neuroni (1-ripida...8-dolce)"; ar
    ' "variabilit… della pendenza"; vps
    ' "Valore di stimolazione per risposta del neurone del 50% "; sg05
    ' "Pendenza della sigmoide per apprendimento della sinapsi (1-8)"; aw
    ' "Coefficente d'apprendimento "; e
    ' "Decadimento della concentrazione del fattore di retrodiffusione (1.1-10)"; der
    ' "rapporto sinapsi eccitatorie/inibitorie "; ec, in
    ' "Numero cicli previsto "; y1; " effettuato ", y3
    ' "seme dell'avviamento casuale"; seme


    Private FileExtension As String = ".UC3"
    Public ParametriRete As TParametri

    Public Sub ReadFromFile(Filename As String)
        Dim RawData As Byte() = File.ReadAllBytes(Filename)
        fromByte(RawData)
    End Sub

    Public Sub fromByte(byteArray As Byte())
        ParametriRete = New TParametri()
        Dim size As UInt16 = Marshal.SizeOf(ParametriRete)
        Dim ptr As IntPtr = Marshal.AllocHGlobal(size)
        Marshal.Copy(byteArray, 0, ptr, size)
        ParametriRete = Marshal.PtrToStructure(ptr, ParametriRete.GetType())
        Marshal.FreeHGlobal(ptr)
    End Sub

    Public Function CSVHeader(separator As Char)

        Dim Str As String = "ncl" + separator + _
              "nci" + separator + _
              "lv" + separator + _
              "ar" + separator + _
              "e" + separator + _
              "e1" + separator + _
              "der" + separator + _
              "seme" + separator + _
              "ec" + separator + _
              "inib" + separator + _
              "vmw" + separator + _
              "vw" + separator + _
              "pr" + separator + _
              "ru" + separator + _
              "rd" + separator + _
              "rt" + separator + _
              "nr" + separator + _
              "cla" + separator + _
              "sga"
        Return Str
    End Function

    Public Function ToCSVString(separator As Char)
        Dim str As String

        With Me.ParametriRete
            str = .ncl.ToString() + separator + _
                  .nci.ToString() + separator + _
                  .lv.ToString() + separator + _
                  .ar.ToString() + separator + _
                  .e.ToString() + separator + _
                  .e1.ToString() + separator + _
                  .der.ToString() + separator + _
                  .seme.ToString() + separator + _
                  .ec.ToString() + separator + _
                  .inib.ToString() + separator + _
                  .vmw.ToString() + separator + _
                  .vw.ToString() + separator + _
                  .pr.ToString() + separator + _
                  .ru.ToString() + separator + _
                  .rd.ToString() + separator + _
                  .rt.ToString() + separator + _
                  .nr.ToString() + separator + _
                  .cla.ToString() + separator + _
                  .sga.ToString()
        End With

        Return str
    End Function

    Public Sub toXMLFile(XMLFileName As String)

        Dim ser As New XmlSerializer(Me.GetType())
        Dim str As New FileStream(XMLFileName, FileMode.Create)

        ser.Serialize(str, Me)


    End Sub
End Class
