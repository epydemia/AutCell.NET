
REM $DYNAMIC
COMMON SHARED le, lv, lvi, lvf, t1, nci, ncl, ncli, nclf, nr, pr, pri, prf, intercl, intercla, interpr AS INTEGER
COMMON SHARED ec, eci, ecf, in, ini, inf, y1, seme, semei, semef, ru, rd, rt, cla, clai, claf, sga, sgai, sgaf, cam AS INTEGER
COMMON SHARED ar, ari, arf, vmw, vmwi, vmwf, vw, vwi, vwf, intervar, intervmw, intervw, interve, interder, sm, e, e1, ei, ef, der, deri, derf AS SINGLE
COMMON SHARED nf$

DECLARE SUB aggiornasinapsi ()
DECLARE SUB amps (r, vmw, vw, r3$)
DECLARE SUB retrodiffusione (lv)
DECLARE SUB attivaneurone (i, j, k, sga)
DECLARE SUB attivaneurone2 (i, j, k, sga, fase#)
DECLARE SUB modpesDK (i, j, k, nc1)
DECLARE SUB modpesDR (i, j, k, nc1)
DECLARE SUB modpesDH (i, j, k)
DECLARE SUB cancfile (auto)
DECLARE SUB elettrodi ()
DECLARE SUB datelettr (i, j, k, y)
DECLARE SUB vedneursogl (a$)
DECLARE SUB vedipesi ()
DECLARE SUB vediDPS (g%, y, rs, nf$, nsn)
DECLARE SUB vedisigm (sgmax, amd, amd2, u)
DECLARE SUB provaschermo (g%, me)
DECLARE SUB inout (ami, sgmax, sga, r2$)
DECLARE SUB neurout (x2, nt1, sgmax, a1, b1, b3, sg, sga, r2$)
DECLARE SUB sinapsilivelli ()
DECLARE SUB sinapsilivelli2 ()
DECLARE SUB tempi ()
DECLARE SUB sequenza ()
DECLARE SUB menu ()
DECLARE SUB nomefile ()
DECLARE SUB distrpes (y, pmx%)
DECLARE SUB memS (y)
DECLARE SUB outhd (y2%, y3, atx, m%, r2%, r3%, r2$, r3$)
DECLARE SUB datiela (r2$, r3$, r2%, r3%, y2%, y4%)
DECLARE SUB reconfig (y2%, y4%)
DECLARE SUB reconfig2 (r2$, r3$, r2%, r3%, y3, atx, m%)
DECLARE SUB reconfig3 ()
DECLARE FUNCTION limiti (pj)
DECLARE SUB riassunto (r2$, r3$, r2%, r3%, y2%, y4%)
DECLARE SUB partenza ()
DECLARE SUB impostazioni ()

CLS

DIM SHARED s(2, ncl, ncl, ncl) AS INTEGER
DIM SHARED sp(lv, ncl, ncl, ncl) AS INTEGER
DIM SHARED t(lv) AS INTEGER
DIM SHARED w(ncl, ncl, ncl, nci) AS INTEGER
DIM SHARED wlw(ncl, ncl, ncl, nci) AS INTEGER
DIM SHARED d1(ncl, ncl, ncl) AS INTEGER
DIM SHARED xv(y2%) AS INTEGER
DIM SHARED dpst(lv, 20) AS INTEGER
DIM SHARED d(lv) AS DOUBLE
DIM SHARED dt(ncl, ncl, ncl, nci) AS INTEGER
DIM SHARED wl(nci) AS INTEGER
DIM SHARED dps(y2%, lv, 20) AS INTEGER
DIM SHARED nfc(8) AS STRING
DIM SHARED el(nr, 2) AS INTEGER
DIM SHARED ew(y2%, nr, nci) AS INTEGER

ON ERROR GOTO gestmem

CALL partenza
ec = 0

'CALL reconfig3
CALL reconfig(y2%, y4%)

IF ec <> -1 AND eci <> 0 THEN
rec% = 1
REDIM SHARED s(2, ncl, ncl, ncl) AS INTEGER
REDIM SHARED sp(lv, ncl, ncl, ncl) AS INTEGER
REDIM SHARED t(lv) AS INTEGER
IF rt = 3 THEN REDIM SHARED d1(ncl, ncl, ncl) AS INTEGER
REDIM SHARED el(nr, 3) AS INTEGER
REDIM SHARED w(ncl, ncl, ncl, nci) AS INTEGER

CALL reconfig2(r2$, r3$, r2%, r3%, y3, atx, m%)
END IF

IF rec% = 1 THEN GOTO elaborazione

ON ERROR GOTO gestmem

dimpop:
IF sse = 1 THEN sse = 0: CLEAR , , 4096
CONST falso = 0, vero = NOT falso

r$ = "": CALL provaschermo(g%, me)
IF g% = 1 THEN
ON KEY(11) GOSUB su: ON KEY(12) GOSUB sx: ON KEY(13) GOSUB dx
ON KEY(14) GOSUB giu: ON KEY(1) GOSUB fine
KEY(11) ON: KEY(12) ON: KEY(13) ON: KEY(14) ON: KEY(1) ON
END IF

PRINT "inizializzazione senza HD"

INPUT "Quante cellule di lato (3-10) "; ncl: ncl = ncl - 1
INPUT "Quanti livelli di intorno (1-3)"; lv
le = lv * 2 + 1
nci = le ^ 3 - 2: cellule = (ncl + 1) ^ 3: nc1 = nci + 1
lv1 = lv + 1

IF nc1 > cellule THEN
PRINT "ATTENZIONE, il numero di cellule dell'intorno di ogni neurone"
PRINT "ä maggiore di quello dell'intera popolazione; pertanto vi sono cellule"
PRINT "che inviano pió di una sinapsi allo stesso neurone centrale."
PRINT "Sei sicuro di ciï?": r$ = INPUT$(1)
END IF
IF r$ = "n" THEN GOTO dimpop
r$ = ""
INPUT "inizializzazione casuale dei pesi sinaptici: valore medio (0..1)"; vmw: vmw = vmw * 10000
INPUT "loro variabilitÖ % (0-100)"; vw: vw = vw * vmw / 100
PRINT "Coefficente di dimenticanza (attivitÖ media dei neuroni sotto la quale < il peso sinaptico)"
INPUT "(0.1-1)"; e: e = e ^ 2 * 10000
PRINT "scegli fra: 1) neuroni con sinapsi eccitatorie e inibitorie"
PRINT "            2)neuroni solo eccitatori o solo inibitori"
r3$ = INPUT$(1)

INPUT "rapporto eccitatorie/inibitorie"; ec, in
r = in / (ec + in): sgmax = (nci + 1) * (1 - r): sgmd = (nci + 1) * (1 - 2 * r)

PRINT "scegli il tipo di modifica del peso sinaptico secondo:"
INPUT "1-Hebb 2-Kandel 3-retrodiffusione (X,X,X ; 0=non scelto)"; ru, rd, rt
IF rt = 3 THEN INPUT "Decadimento della concentrazione del fattore di retrodiffusione (1.1-10)"; der
ami = sgmax * (vmw + vw) * .5 / 10000: PRINT "attivitÖ max. raggiungibile dall'intorno: "; ami
amd = sgmd * (vmw + vw) * .5 / 10000: amd2 = sgmd * (vmw - vw) * .5 / 10000
PRINT "attivitÖ media dell'intorno compresa fra "; amd2; "e"; amd; " (un intervallo di confidenza)"

carneur:
PRINT "scegli fra neuroni modulati in Frequenza o in Ampiezza (f/a)": r2$ = INPUT$(1)
INPUT "coefficente di amplificazione della soglia (1..10) "; sga

IF g% = 0 THEN
INPUT "Pendenza della sigmoide di attivazione dei neuroni (1-ripida...8-dolce)"; ar
END IF
IF g% = 1 THEN
CLS 0: CALL vedisigm(sgmax, amd, amd2, u): CLS 0
END IF

perefr:
INPUT "periodo refrattario in cicli dopo spike >= 90% "; pr
INPUT "numero di cicli per l'apoptosi"; cla
IF pr < 1 OR cla < 1 THEN GOTO perefr

PRINT "vuoi verificare la risposta dei neuroni?": r$ = INPUT$(1)
stimrisp:
IF r$ = "s" THEN CALL inout(ami, sgmax, sga, r2$): LOCATE 24, 1
PRINT "vuoi cambiare la forma d'onda di stimolo?": r$ = INPUT$(1)
IF r$ = "s" THEN GOTO stimrisp
PRINT "vuoi cambiare le caratteristiche del neurone?": r$ = INPUT$(1)
IF r$ = "s" THEN GOTO carneur

CALL elettrodi

CLS 0: KEY OFF: INPUT "Numero cicli (Max. 10000) "; y1
INPUT "seme dell'avviamento casuale"; seme: RANDOMIZE (seme)

y = 0: y2 = 0: IF ru = 1 THEN yu2 = 2
IF rd = 2 THEN yd2 = 37
IF rt = 3 THEN yt2 = 5
y2% = INT(22 * 4 / (yu2 + yd2 + yt2)): y4% = y2%

elaborazione:

IF rec% = 0 THEN

 REDIM SHARED s(2, ncl, ncl, ncl) AS INTEGER
 REDIM SHARED sp(lv, ncl, ncl, ncl) AS INTEGER
 REDIM SHARED t(lv) AS INTEGER
 REDIM SHARED w(ncl, ncl, ncl, nci) AS INTEGER
 REDIM SHARED d1(ncl, ncl, ncl) AS INTEGER
 REDIM SHARED el(nr, 2) AS INTEGER

END IF

 REDIM SHARED xv(y2%) AS INTEGER
 REDIM SHARED d(lv) AS DOUBLE
 REDIM SHARED wl(nci) AS INTEGER
 REDIM SHARED wlw(ncl, ncl, ncl, nci) AS INTEGER
 REDIM SHARED dt(ncl, ncl, ncl, nci) AS INTEGER
 REDIM SHARED dpst(lv - 1, 20) AS INTEGER
 REDIM SHARED dps(y2%, lv - 1, 20) AS INTEGER
 REDIM SHARED es(y2%, ncl, ncl, ncl) AS INTEGER
 REDIM SHARED sg(y2%, ncl, ncl, ncl) AS INTEGER
 REDIM SHARED ew(y2%, nr, nci) AS INTEGER

cicloprinc:

IF rec% = 0 OR r2% = 1 OR r3% = 1 THEN
CALL nomefile
CALL datiela(r2$, r3$, r2%, r3%, y2%, y4%)
END IF

ON KEY(2) GOSUB attivazione: ON KEY(3) GOSUB soglie
ON KEY(4) GOSUB pesi: ON KEY(5) GOSUB distribuzione
ON KEY(6) GOSUB fineprog: ON KEY(7) GOSUB vedpar
KEY(2) ON: KEY(3) ON: KEY(4) ON: KEY(5) ON: KEY(6) ON: KEY(7) ON

CLS
PRINT "i dati vengono memorizzati ogni "; y2%; " cicli"
PRINT : PRINT "in prova la seguente configurazione:": PRINT
PRINT "cellule di lato"; ncl + 1, "livelli di intorno"; lv
PRINT "valore medio iniziale dei pesi sinaptici"; vmw; " variabilitÖ"; vw
PRINT "rapporto sinapsi eccitatorie/inibitorie"; ec; "/"; in; "  "; r
PRINT "seme dell'avviamento casuale"; seme
PRINT "modifica del peso sinaptico secondo"
PRINT "1-Hebb 2-Kandel 3-retrodiffusione"; ru; rd; rt
PRINT "coefficente di dimenticanza (attivitÖ media dei neuroni sotto la quale diminuisce il peso sinaptico)"; e
PRINT "amplificazione della soglia"; sga
PRINT "retrodiffusione"; der
PRINT
PRINT "neuroni con sinapsi:"
PRINT "1-eccitatorie e inibitorie  2-solo eccitatori o solo inibitori "; r3$
PRINT "modulati in:  Frequenza  Ampiezza "; r2$
PRINT "pendenza della sigmoide di attivazione"; ar
PRINT "periodo refrattario (cicli)"; pr; " apoptosi"; cla

fase# = 3.141593 * 2 / lv

cellule = (ncl + 1) ^ 3
PRINT "Cellule"; cellule
nsn = cellule * (nci + 1): PRINT "sinapsi"; nsn
PRINT "Premere "
PRINT "F6 per fine programma, F2 per vedere le Attivazioni dei neuroni"
PRINT "F3 per le loro Soglie, F5 per la Distribuzione dei pesi sinaptici"
PRINT " F4 per i Pesi afferenti a quelli scelti (con le coordinate i,j,k)"
PRINT "F7 per vedere i parametri della rete"

PRINT "Passo", "AttivitÖ Globale"

IF rec% = 0 THEN
CALL amps(r, vmw, vw, r3$)
CALL tempi
END IF

CALL retrodiffusione(lv)
CALL sinapsilivelli
CALL sinapsilivelli2

ON ERROR GOTO 0

DO
 DO

 IF y3 = 0 AND y = 0 THEN tm1 = TIMER
 IF y3 = 0 AND y = 1 THEN tm2 = TIMER: sect = (tm2 - tm1) * y1: ora! = sect / 3600: ore = INT(ora!): min = (sect - ore * 3600) / 60: min% = CINT(min): PRINT "l'elaborazione durerÖ circa "; ore; " ore "; min%; " minuti"

  FOR i = 0 TO ncl
     FOR j = 0 TO ncl
         FOR k = 0 TO ncl
            IF s(1, i, j, k) = cla THEN s(1, i, j, k) = -1: m% = m% + 1
             IF s(1, i, j, k) = -1 THEN GOTO ritorno2
             IF r2$ = "a" THEN
             CALL attivaneurone(i, j, k, sga)
             ELSEIF r2$ = "f" THEN
             CALL attivaneurone2(i, j, k, sga, fase#)
             END IF
             att = att + sp(t(0), i, j, k) / 10000
             CALL datelettr(i, j, k, y)
ritorno2:
 NEXT k, j, i

 CALL memS(y)

 FOR i = 0 TO ncl
     FOR j = 0 TO ncl
         FOR k = 0 TO ncl
            IF s(1, i, j, k) = -1 THEN GOTO ritorno1
            IF ru = 1 THEN CALL modpesDH(i, j, k)
            IF rt = 3 THEN CALL modpesDR(i, j, k, nc1)
            IF rd = 2 THEN CALL modpesDK(i, j, k, nc1)
ritorno1:
 NEXT k, j, i

 CALL aggiornasinapsi
 CALL sequenza
 xv(y) = att
 IF att > atx THEN atx = att
 PRINT y3 + y; att, : att = 0
 CALL distrpes(y, pmx%)

 IF y3 = -1 THEN EXIT DO: EXIT DO: GOTO fineciclo1

 IF a$ = "f" THEN
 BEEP: y2% = y + 1
 CALL outhd(y2%, y3, atx, m%, r2%, r3%, r2$, r3$)
 CALL riassunto(r2$, r3$, r2%, r3%, y2%, y4%)

 PRINT "vuoi cancellare dall'HD una precedente simulazione ?"
 ec = 0: r$ = INPUT$(1): IF r$ = "s" THEN CALL cancfile(auto)
 PRINT "vuoi cambiare le impostazioni di avvço ?"
 r$ = INPUT$(1): IF r$ = "s" THEN CALL impostazioni

 END
 END IF

 IF y3 + y = y1 THEN y2% = y + 1: EXIT DO

 y = y + 1
LOOP UNTIL y = y2%

CALL outhd(y2%, y3, atx, m%, r2%, r3%, r2$, r3$)
CALL menu
y = 0: LOOP UNTIL y3 >= y1

fineciclo1:
y = 0: y3 = 0: y2% = y4%

fineciclo:
KILL "filincor"
PRINT "sequenza completata": BEEP: END



fine:
u = 1
RETURN

su:
ar2 = ar: ar = ar + .01
RETURN

giu:
ar2 = ar: ar = ar - .01
RETURN

dx:
ar2 = ar: ar = ar + .1
RETURN

sx:
ar2 = ar: ar = ar - .1
RETURN

attivazione:
 a$ = "a": CALL vedneursogl(a$)
RETURN

soglie:
 a$ = "s": CALL vedneursogl(a$)
RETURN

pesi:
 CALL vedipesi
RETURN

distribuzione:
 CALL vediDPS(g%, y, rs, nf$, nsn)
RETURN

vedpar:
 CALL riassunto(r2$, r3$, r2%, r3%, y2%, y4%)
RETURN

fineprog:
a$ = "f": PRINT "chiusura del ciclo in corso: l'operazione potrebbe richiedere qualche minuto..."
KEY OFF
RETURN

schermoinf:
me = falso
RESUME NEXT

gesterr:
IF ERR = 52 THEN ec = -1: PRINT "non memorizzato su HD"
IF ERR = 53 THEN ec = -1: PRINT "nome del file sbagliato"
IF ERR = 54 THEN ec = -1: PRINT "non memorizzato su HD"
IF ERR = 64 THEN ec = -1: PRINT "non memorizzato su HD"
RESUME NEXT

gestmem:
SELECT CASE ERR
CASE 7
PRINT "memoria esaurita: riduci la popolazione": ERASE s, w, d1, xv, d, dpst: ERASE dt, dps, es, sg, el, ew: RESUME dimpop
CASE 14
IF ERR = 14 THEN PRINT "memoria esaurita: riduci la popolazione": sse = 1: ERASE s, w, d1, xv, d, dpst: ERASE dt, dps, es, sg, el, ew: RESUME dimpop
CASE ELSE
PRINT "errore imprevisto"
ON ERROR GOTO 0
END SELECT

REM $STATIC
SUB aggiornasinapsi
FOR i = 0 TO ncl
 FOR j = 0 TO ncl
  FOR k = 0 TO ncl
   FOR a = 0 TO nci
  nw = w(i, j, k, a) + dt(i, j, k, a)
  IF nw > 10000 THEN nw = 10000 ELSE IF nw < -10000 THEN nw = -10000
  w(i, j, k, a) = nw
NEXT a, k, j, i
a = 0: REDIM dt(ncl, ncl, ncl, nci) AS INTEGER
END SUB

SUB amps (r, vmw, vw, r3$)
   FOR li = 0 TO lv
    FOR i = 0 TO ncl
        FOR j = 0 TO ncl
            FOR k = 0 TO ncl
                sp(li, i, j, k) = RND * 10000
                NEXT k, j, i, li

    FOR i = 0 TO ncl
        FOR j = 0 TO ncl
            FOR k = 0 TO ncl
                s(0, i, j, k) = 10
    NEXT k, j, i

     SELECT CASE r3$

     CASE "1"
     FOR i = 0 TO ncl
        FOR j = 0 TO ncl
            FOR k = 0 TO ncl
                FOR a = 0 TO nci
                w(i, j, k, a) = (RND * SGN(RND - .5) * vw + vmw) * SGN(RND - r)
     NEXT a, k, j, i

     CASE "2"
     a = nci
     FOR i = 0 TO ncl
        FOR j = 0 TO ncl
            FOR k = 0 TO ncl: sg = SGN(RND - r)
                FOR l = i - lv TO i + lv
                  l1 = limiti(l)
                    FOR m = j - lv TO j + lv
                      m1 = limiti(m)
                        FOR n = k - lv TO k + lv
                         IF l = i AND m = j AND n = k THEN n = n + 1
                         n1 = limiti(n)
                         w(l1, m1, n1, a) = (RND * SGN(RND - .5) * vw + vmw) * sg
                         a = a - 1
                         NEXT n, m, l
                         a = nci
                        NEXT k, j, i

             END SELECT
             END SUB

SUB attivaneurone (i, j, k, sga)
FOR l = i - lv TO i + lv
       l1 = limiti(l)
        FOR m = j - lv TO j + lv
           m1 = limiti(m)
            FOR n = k - lv TO k + lv
          IF l = i AND m = j AND n = k THEN n = n + 1
            n1 = limiti(n)
            w = w(i, j, k, a): li = wlw(i, j, k, a)
            u = sp(t(li), l1, m1, n1)
            nt = nt + w * u
         a = a + 1

NEXT n, m, l
            
            nt = sga * nt / 1E+08: sg099 = s(0, i, j, k) / 100
            at = 1 / (1 + EXP(-(nt - sg099) / ar))
            sp(t(0), i, j, k) = at * 10000
            sg099 = s(0, i, j, k) + sg099 * (at / .9) * 100 - s(0, i, j, k) / pr
            s(0, i, j, k) = sg099
            IF ABS(nt) < 1 AND ABS(s(0, i, j, k)) < 2 THEN s(1, i, j, k) = s(1, i, j, k) + 1 ELSE s(1, i, j, k) = 0

            END SUB

SUB attivaneurone2 (i, j, k, sga, fase#)
FOR l = i - lv TO i + lv
       l1 = limiti(l)
        FOR m = j - lv TO j + lv
           m1 = limiti(m)
            FOR n = k - lv TO k + lv
          IF l = i AND m = j AND n = k THEN n = n + 1
            n1 = limiti(n)
            w = w(i, j, k, a): li = wlw(i, j, k, a)
                st# = sp(t(li), l1, m1, n1)
                st1# = sp(t(li), l1, m1, n1) * 6.283186E-04
            u# = st# * COS(st1# * s(2, l1, m1, n1) + li * fase#)
            nt = nt + w * u#
         a = a + 1

NEXT n, m, l
           
     nt = sga * nt / 1E+08: sg099 = s(0, i, j, k) / 100
     at = 1 / (1 + EXP(-(nt - sg099) / ar))
     sp(t(0), i, j, k) = at * 10000
     sg099 = s(0, i, j, k) + ABS(sg099) * (at / .9) * 100 - s(0, i, j, k) / pr
     s(0, i, j, k) = sg099
     IF ABS(nt) < 1 AND ABS(s(0, i, j, k)) < 2 THEN s(1, i, j, k) = s(1, i, j, k) + 1 ELSE s(1, i, j, k) = 0
     IF sp(t(0), i, j, k) > 1000 THEN s(2, i, j, k) = s(2, i, j, k) + 1 ELSE s(2, i, j, k) = 0

END SUB

SUB cancfile (auto)

DO
IF nf$ = "" THEN
PRINT "scrivi la simulazione da cancellare"
ON ERROR GOTO gesterr
FILES "filincor": FILES "*.UC3": INPUT nf$
END IF

f$ = LEFT$(nf$, 8): KILL nf$

IF f$ <> "filincor" THEN
FOR k = 1 TO 8
k$ = STR$(k): car = LEN(k$) - 1
IF car = 1 THEN ext$ = ".A0" + RIGHT$(k$, car)
IF car = 2 THEN ext$ = ".A" + RIGHT$(k$, car)
fc$ = f$ + ext$: KILL fc$
NEXT k: k = 0
END IF

IF auto = 1 THEN EXIT SUB

PRINT "vuoi cancellarne un'altra?": r$ = INPUT$(1): IF r$ <> "s" THEN EXIT DO
nf$ = ""
LOOP
ON ERROR GOTO 0

END SUB

SUB datelettr (i, j, k, y)
DO
 IF el(p, 0) = i AND el(p, 1) = j AND el(p, 2) = k THEN cl = 1: EXIT DO
 p = p + 1
LOOP UNTIL p = nr - 1

IF cl = 1 THEN
    FOR l = i - lv TO i + lv
       FOR m = j - lv TO j + lv
           FOR n = k - lv TO k + lv
          IF l = i AND m = j AND n = k THEN n = n + 1
            ew(y, p, a) = w(i, j, k, a)
            a = a + 1
     NEXT n, m, l
ELSE

cl = 0
END IF

END SUB

SUB datiela (r2$, r3$, r2%, r3%, y2%, y4%)

OPEN "filincor" FOR BINARY AS #10
PUT #10, , nf$
CLOSE #10

OPEN nf$ FOR BINARY AS #6
PUT #6, , ncl: PUT #6, , nci: PUT #6, , lv: PUT #6, , ar
PUT #6, , e: PUT #6, , e1
PUT #6, , der: PUT #6, , seme: PUT #6, , ec: PUT #6, , in
PUT #6, , vmw: PUT #6, , vw: PUT #6, , pr
PUT #6, , ru: PUT #6, , rd: PUT #6, , rt: PUT #6, , nr
PUT #6, , cla: PUT #6, , sga
CLOSE #6

OPEN nfc(5) FOR BINARY AS #5
PUT #5, , y2%: PUT #5, , y4%: PUT #5, , r2%: PUT #5, , r3%
PUT #5, , ncli: PUT #5, , nclf: PUT #5, , lvi: PUT #5, , lvf
PUT #5, , ari: PUT #5, , arf: PUT #5, , ei: PUT #5, , ef
PUT #5, , deri: PUT #5, , derf: PUT #5, , semei: PUT #5, , semef
PUT #5, , eci: PUT #5, , ecf: PUT #5, , ini: PUT #5, , inf
PUT #5, , vmwi: PUT #5, , vmwf: PUT #5, , vwi: PUT #5, , vwf
PUT #5, , pri: PUT #5, , prf
PUT #5, , clai: PUT #5, , claf: PUT #5, , sgai: PUT #5, , sgaf
PUT #5, , intercla: PUT #5, , interpr
PUT #5, , interder: PUT #5, , intervar: PUT #5, , interve: PUT #5, , intervmw: PUT #5, , intervw: PUT #5, , intercl
PUT #5, , y1: PUT #5, , sm: PUT #5, , cam
CLOSE #5

OPEN nfc(6) FOR BINARY AS #6
FOR p = 0 TO nr - 1
 FOR q = 0 TO 2
 PUT #6, , el(p, q)
NEXT q, p
CLOSE #6

END SUB

SUB distrpes (y, pmx%)
FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl: FOR a = 0 TO nci
li = wlw(i, j, k, a)
x = INT((w(i, j, k, a) + 10000) * .001)
dps(y, li - 1, x) = dps(y, li - 1, x) + 1
IF dps(y, li - 1, x) > pmx THEN pmx = dps(y, li - 1, x)
NEXT a, k, j, i
END SUB

SUB elettrodi
INPUT "quanti neuroni vuoi controllare (max 10)"; nr
REDIM el(nr, 3) AS INTEGER
PRINT "per ognuno specifica la posizione i,j,k (valori ammessi 0-"; ncl; ")"
FOR p = 0 TO nr - 1: PRINT "neurone "; p + 1
 INPUT i, j, k
 el(p, 0) = i: el(p, 1) = j: el(p, 2) = k
NEXT p

END SUB

SUB impostazioni
DIM av AS INTEGER
PRINT "vuoi cancellare dall'HD una precedente simulazione ?"
ec = 0: r$ = INPUT$(1): IF r$ = "s" THEN CALL cancfile(auto)

PRINT "scegli fra avviare il programma con partenza:"
PRINT "1-manuale   2-automatica, continuando l'elaborazione in corso"
avc$ = INPUT$(1)
av = VAL(avc$)

SELECT CASE av

CASE IS = 1
sceglielab2:
PRINT "scrivi il file da continuare (nessuno se vuoi una nuova elaborazione da tastiera)"
FILES "*.UC3": INPUT nf$
IF ec = -1 AND nf$ <> "" THEN GOTO sceglielab2

CASE IS = 2
CALL reconfig3

END SELECT

OPEN "imposta" FOR BINARY AS #11
PUT #11, , av
CLOSE #11

END SUB

SUB inout (ami, sgmax, sga, r2$)
CLS 0: SCREEN 11
VIEW (140, 0)-(639, 399): WINDOW (0, 0)-(500, 400)
LOCATE 1, 20: PRINT "forma d'onda in INPUT al neurone:"
LOCATE 4, 1: PRINT "1-sinusoidale": LOCATE 9, 1: PRINT "2-triangolare"
LOCATE 15, 1: PRINT "3-quadra": LOCATE 22, 1: PRINT "4-reobase"
FOR rg = 1 TO 24 STEP 6
LOCATE rg, 5: PRINT "+sgmax"; sgmax
LOCATE rg + 1, 3: PRINT "+att.max.int"; ami
LOCATE rg + 4, 3: PRINT "-att.max.int"; ami
LOCATE rg + 5, 5: PRINT "-sgmax"; sgmax * (-1)
LOCATE rg + 1, 28: PRINT "lunghezza d'onda (cicli)"
LOCATE rg + 3, 25: PRINT "ampiezza": LOCATE rg + 3, 70: PRINT "cicli 500"
NEXT rg

FOR y = 50 TO 350 STEP 100: y5 = y + 50: y2 = y - 50
LINE (0, y)-(500, y)
FOR x = 10 TO 500 STEP 10: LINE (x, y)-(x, y - 5): NEXT x
NEXT y

LINE (0, 400)-(0, 0)
FOR y = 5 TO 400 STEP 5: LINE (0, y)-(5, y): NEXT y

lin = 250 / 4: lf = lin + 250: ym = ami * 50 / sgmax
FOR y = 75 TO 475 STEP 100
LINE (lin, y)-(lf, y): LINE (lin, y)-(lin, y - ym)
NEXT y

lo = 250: pi = 3.1416 * 2: q# = pi / lo: y5 = 350: d = 50 * ami / sgmax
FOR x2 = 0 TO 500: q1# = q1# + q#: y2 = INT(d * SIN(q1#)) + 350
LINE (x1, y5)-(x2, y2): x1 = x2: y5 = y2
NEXT x2

x1 = 0: q = lo / 2: y5 = ami * 50 / sgmax: y2 = y5 * (-1)
DO
  x2 = x1 + q
  LINE (x1, y5 + 250)-(x2, y2 + 250)
  SWAP y5, y2: x1 = x2
LOOP WHILE x2 < 500

q = lo / 2: y5 = ami * 50 / sgmax: y2 = y5 * (-1): x1 = 0
DO
  LINE (x1, y5 + 150)-(x1, y2 + 150): x2 = x1 + q
  LINE (x1, y2 + 150)-(x2, y2 + 150)
  SWAP y5, y2: x1 = x2
LOOP WHILE x2 < 500

LINE (0, 75)-(500, 75)

LOCATE 28, 1: INPUT "scrivi nell'ordine: la forma d'onda, la lunghezza, l'ampiezza"; fo, lo, ao
CLS 0
FOR y = 50 TO 350 STEP 100: y5 = y + 50: y2 = y - 50
LINE (0, y)-(500, y): LINE (480, y + 20)-(490, y + 20)
FOR x = 10 TO 500 STEP 10: LINE (x, y)-(x, y - 5): NEXT x
NEXT y
sg = 10
LINE (0, 400)-(0, 0)
FOR y = 5 TO 400 STEP 5: LINE (0, y)-(5, y): NEXT y

LOCATE 1, 1: PRINT "+soglia max"; sgmax
LOCATE 3, 4: PRINT "stimolo"
LOCATE 6, 1: PRINT "-soglia max"; -sgmax
LOCATE 7, 18: PRINT "+1"
LOCATE 13, 18: PRINT "-1"
LOCATE 10, 4: PRINT "risposta"
LOCATE 14, 1: PRINT "+sgmax neurone"; sgmax
LOCATE 20, 1: PRINT "-sgmax neurone"; -sgmax
LOCATE 17, 1: PRINT "soglia per att. neur. 90%"
FOR rg = 2 TO 25 STEP 6
LOCATE rg, 70: PRINT "cicli 10"
NEXT rg

SELECT CASE fo

CASE 1
q# = pi / lo: y5 = 350: d = 50 * ao / sgmax
FOR x2 = 0 TO 500
q1# = q1# + q#
nt1 = SIN(q1#): y2 = INT(d * nt1) + 350: nt1 = nt1 * ao
LINE (x1, y5)-(x2, y2): x1 = x2: y5 = y2
CALL neurout(x2, nt1, sgmax, a1, b1, b3, sg, sga, r2$)
NEXT x2

CASE 2
q = lo / 2: y5 = ao * 50 / sgmax: y2 = y5 * (-1): x = 0: x1 = 0
d1 = ao * 2 / q: d2 = d1 * (-1): nt1 = ao
DO
  x = x1 + q
  LINE (x1, y5 + 350)-(x, y2 + 350)
  FOR x2 = x1 TO x
  CALL neurout(x2, nt1, sgmax, a1, b1, b3, sg, sga, r2$): nt1 = nt1 - d1
  NEXT x2
  SWAP y5, y2: SWAP d1, d2: x1 = x
LOOP WHILE x < 500

CASE 3
q = lo / 2: y5 = ao * 50 / sgmax: y2 = y5 * (-1)
nt2 = ao: nt1 = ao * (-1): x = 0: x1 = 0
DO
  LINE (x1, y5 + 350)-(x1, y2 + 350): x = x1 + q
  LINE (x1, y2 + 350)-(x, y2 + 350)
  FOR x2 = x1 TO x: CALL neurout(x2, nt1, sgmax, a1, b1, b3, sg, sga, r2$): NEXT x2
  SWAP y5, y2: SWAP nt1, nt2: x1 = x
LOOP WHILE x < 500

CASE ELSE
y2 = (ao * 50 / sgmax) + 350: nt1 = ao
FOR x2 = 0 TO 500: PSET (x2, y2)
CALL neurout(x2, nt1, sgmax, a1, b1, b3, sg, sga, r2$)
NEXT x2

END SELECT

END SUB

FUNCTION limiti (pj)
SELECT CASE pj

CASE IS < 0
limiti = ncl + 1 + pj

CASE IS > ncl
limiti = pj - (ncl + 1)

CASE ELSE
limiti = pj

END SELECT

END FUNCTION

SUB memS (y)
FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl
es(y, i, j, k) = sp(t(0), i, j, k): sg(y, i, j, k) = s(0, i, j, k)
NEXT k, j, i

END SUB

SUB menu

CLS 0: PRINT
PRINT "file "; nf$
PRINT
PRINT "F6 per fine programma, F2 per vedere le Attivazioni dei neuroni"
PRINT "F3 per le loro Soglie, F5 per la Distribuzione dei pesi sinaptici"
PRINT " F4 per i Pesi afferenti a quelli scelti (con le coordinate i,j,k)"
PRINT "F7 per vedere i parametri"
PRINT

END SUB

SUB modpesDH (i, j, k)
   FOR l = i - lv TO i + lv
       l1 = limiti(l)
        FOR m = j - lv TO j + lv
           m1 = limiti(m)
            FOR n = k - lv TO k + lv
                IF l = i AND m = j AND n = k THEN n = n + 1
                n1 = limiti(n)
                li = wl(a)
                pn# = sp(li, l1, m1, n1)
                pn# = pn# * sp(0, i, j, k)
                dh1 = .000001 * (pn# * SGN(w(i, j, k, a)) - e * w(i, j, k, a))
                dt1 = dt(i, j, k, a): dt(i, j, k, a) = dt1 + dh1
                a = a + 1
               NEXT n: NEXT m: NEXT l
END SUB

SUB modpesDK (i, j, k, nc1)

FOR l = i - lv TO i + lv
 l1 = limiti(l)
  FOR m = j - lv TO j + lv
   m1 = limiti(m)
    FOR n = k - lv TO k + lv
     IF l = i AND m = j AND n = k THEN n = n + 1
     n1 = limiti(n)

     FOR o = i - lv TO i + lv
      o1 = limiti(o)
       FOR p = j - lv TO j + lv
        p1 = limiti(p)
         FOR q = k - lv TO k + lv
          IF o = i AND p = j AND q = k THEN q = q + 1
          IF o = l AND p = m AND q = n THEN GOTO ritorno
          q1 = limiti(q)
          li = wl(a): li1 = wl(a1)
         
          dist = (o - l) ^ 2 + (p - m) ^ 2 + (q - n) ^ 2
          d2# = sp(li, l1, m1, n1)
          dk1# = d2# * sp(li1, o1, p1, q1) * w(i, j, k, a1): dk2# = dist * 1E+08 * nc1
          dk = ABS(dk1# / dk2#)
          dt1 = dt(i, j, k, a): dt2 = dk * SGN(w(i, j, k, a))
          dt(i, j, k, a) = dt1 + CINT(dt2)
          GOTO ritorno
          
ritorno:
          a = a + 1
         NEXT q, p, o

   a = 0: a1 = a1 + 1
NEXT n: NEXT m: NEXT l
   a1 = 0

END SUB

SUB modpesDR (i, j, k, nc1)

sdd = (sp(0, i, j, k) + d1(i, j, k)) / der: d1(i, j, k) = CINT(sdd)
a = 0
FOR l = i - lv TO i + lv
 l1 = limiti(l)
  FOR m = j - lv TO j + lv
   m1 = limiti(m)
    FOR n = k - lv TO k + lv
     IF l = i AND m = j AND n = k THEN n = n + 1
     n1 = limiti(n)
     li = wl(a)
     d2# = d1(i, j, k)
     dr1# = d2# * sp(li, l1, m1, n1) * d(li) / (10000 * nc1): dr = ABS(dr1#)
     dt1 = dt(i, j, k, a): u = SGN(w(i, j, k, a)): dt2 = dr * u: dt3 = CINT(dt2)
     dt(i, j, k, a) = dt1 + dt3
     a = a + 1
NEXT n: NEXT m: NEXT l

FOR l = i - lv TO i + lv
 l1 = limiti(l)
  FOR m = j - lv TO j + lv
   m1 = limiti(m)
    FOR n = k - lv TO k + lv
    IF l = i AND m = j AND n = k THEN n = n + 1
    n1 = limiti(n)

    IF ABS((i - lv) - (l + lv)) < le THEN ino = i - lv: fino = l + lv ELSE ino = l - lv: fino = i + lv
  
    FOR o = ino TO fino
     o1 = limiti(o)

     IF ABS((j - lv) - (m + lv)) < le THEN inw = j - lv: finw = m + lv ELSE inw = m - lv: finw = j + lv
   
      FOR p = inw TO finw
       p1 = limiti(p)

       IF ABS((k - lv) - (n + lv)) < le THEN inq = k - lv: finq = n + lv ELSE inq = n - lv: finq = k + lv

       FOR q = inq TO finq

         IF o = i AND p = j AND q = k THEN GOTO rit3
         q1 = limiti(q)
         IF o = l AND p = m AND q = n THEN GOTO rit2
        
         ab = lv + l - o: ac = (lv + m - p) * le: ad = (lv + n - q) * le ^ 2
         a = ab + ac + ad: IF a > nci / 2 THEN a = a - 1
         li = wl(a)
         d2# = d1(i, j, k)
         dr1# = d2# * sp(li, l1, m1, n1) * sp(li, o1, p1, q1) * d(li): dr2# = 1E+08 * nc1
         dr3# = dr1# / dr2#: dr = ABS(dr3#)
         dt1 = dt(o1, p1, q1, a): u = SGN(w(o1, p1, q1, a)): dt2 = dr * u: dt3 = CINT(dt2)
         dt(o1, p1, q1, a) = dt1 + dt3
         GOTO rit2

rit2:
  NEXT q: NEXT p: NEXT o
  GOTO rit3

rit3:
  NEXT n: NEXT m: NEXT l
 
  END SUB

SUB neurout (x2, nt1, sgmax, a1, b1, b3, sg, sga, r2$)

SELECT CASE r2$

CASE "a"
nt = nt1 * sga

CASE "f"
nt = nt1 * sga * COS((nt1 * 6.283186 / sgmax * x2) + lv)

CASE ELSE
PRINT "modalitÖ non prevista": BEEP: SLEEP

END SELECT

sg09 = sg
at = 1 / (1 + EXP(-(nt - sg09) / ar))
sg = sg09 + sg09 * (at / .9) - sg09 / pr
y2 = at * 50
LINE (a1, b1 + 250)-(x2, y2 + 250): b1 = y2
y4 = sg09 * 50 / sgmax
LINE (a1, b3 + 150)-(x2, y4 + 150): a1 = x2: b3 = y4

END SUB

SUB nomefile

 nf$ = MID$(DATE$, 1, 6) + MID$(DATE$, 9): tp$ = TIME$
 nc = LEN(nf$)
 FOR c = 1 TO nc
 c$ = MID$(nf$, c, 1)
 IF c$ <> "-" THEN nf1$ = nf1$ + c$
 NEXT c
 nc1 = LEN(nf1$): IF nc1 > 6 THEN PRINT "nome troppo lungo": STOP
 tp$ = LEFT$(tp$, 2)
 nf$ = nf1$ + tp$ + ".UC3"

 nfc(0) = LEFT$(nf$, 8)
 FOR k = 1 TO 8
  k$ = STR$(k): car = LEN(k$) - 1
  IF car = 1 THEN ext$ = ".A0" + RIGHT$(k$, car)
  IF car = 2 THEN ext$ = ".A" + RIGHT$(k$, car)
  nfc(k) = nfc(0) + ext$
 NEXT k: k = 0

END SUB

SUB outhd (y2%, y3, atx, m%, r2%, r3%, r2$, r3$)

PRINT "configurazione dei neuroni e sinapsi: salvataggio su HD"
nc2 = nci: ncl1 = ncl + 1
OPEN nfc(1) FOR BINARY AS #1
p = (1 + y3) * 2 - 1: SEEK #1, p
FOR y = 0 TO y2% - 1
PUT #1, , xv(y)
NEXT y
CLOSE #1

OPEN nfc(2) FOR BINARY AS #2
p = (1 + y3 * ncl1 ^ 3) * 2 - 1: SEEK #2, p
FOR y = 0 TO y2% - 1: FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl
PUT #2, , es(y, i, j, k)
NEXT k, j, i, y
CLOSE #2

IF nr > 0 THEN
 OPEN nfc(3) FOR BINARY AS #3
 p = (1 + y3 * ncl1 * nc2) * 2 - 1: SEEK #3, p
 FOR y = 0 TO y2% - 1: FOR p = 0 TO nr - 1: FOR a = 0 TO nci
 PUT #3, , ew(y, p, a)
 NEXT a, p, y
 CLOSE #3
END IF

OPEN nfc(4) FOR BINARY AS #4
p = (1 + y3 * lv * 20) * 2 - 1: SEEK #4, p
FOR y = 0 TO y2% - 1: FOR li = 0 TO lv - 1: FOR x = 0 TO 19
PUT #4, , dps(y, li, x)
NEXT x, li, y
CLOSE #4
REDIM dps(y2%, lv, 20) AS INTEGER

OPEN nfc(8) FOR BINARY AS #8
p = (1 + y3 * (ncl + 1) ^ 3) * 2 - 1: SEEK #8, p
FOR y = 0 TO y2% - 1: FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl
PUT #8, , sg(y, i, j, k)
NEXT k, j, i, y
CLOSE #8

OPEN nfc(7) FOR BINARY AS #7
y3 = y3 + y2%

FOR u = 0 TO 2: FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl
PUT #7, , s(u, i, j, k)
NEXT k, j, i, u

FOR li = 0 TO lv: FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl
PUT #7, , sp(li, i, j, k)
NEXT k, j, i, li

FOR li = 0 TO lv: PUT #7, , t(li): NEXT li

FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl: FOR a = 0 TO nci
PUT #7, , w(i, j, k, a)
NEXT a, k, j, i

IF rt = 3 THEN
FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl
PUT #7, , d1(i, j, k)
NEXT k, j, i
END IF

PUT #7, , y3: PUT #7, , m%: PUT #7, , r2%: PUT #7, , r3%
PUT #7, , atx: PUT #7, , pmx%
PUT #7, , r2$: PUT #7, , r3$

CLOSE #7

END SUB

SUB partenza

ON ERROR GOTO gesterr

DIM av AS INTEGER
PRINT "ricerca del file di impostazione d'avvço:"
FILES "imposta"
OPEN "imposta" FOR BINARY AS #11
GET #11, , av
CLOSE #11

IF ec = -1 OR av = 1 THEN CALL impostazioni: EXIT SUB
IF av = 2 THEN CALL reconfig3: EXIT SUB
PRINT "errore di di avvço n¯ "; ERR

END SUB

SUB provaschermo (g%, me)
me = vero
ON ERROR GOTO schermoinf
SCREEN 11
IF me THEN PRINT "vga 640 X 480": g% = 1: GOTO finesub
SCREEN 9
IF me THEN PRINT "vga: 640 X 350": g% = 1: GOTO finesub
SCREEN 8
IF me THEN PRINT "ega: 640 X 200": g% = 1: GOTO finesub
SCREEN 4
IF me THEN PRINT "olivetti: 640 X 400": g% = 1: GOTO finesub
SCREEN 3
IF me THEN PRINT "hercules: 720 X 348": g% = 1: GOTO finesub
SCREEN 1, 1
IF me THEN PRINT "cga: 320 X 200": g% = 1: GOTO finesub
PRINT "nessuna interfaccia grafica": g% = 0
finesub:
ON ERROR GOTO 0
END SUB

SUB reconfig (y2%, y4%)

OPEN "filincor" FOR BINARY AS #10
IF ec = -1 THEN CLOSE #10: GOTO finerec
nf$ = INPUT$(12, #10)
CLOSE #10

OPEN nf$ FOR BINARY AS #6
IF ec = -1 THEN CLOSE #6: GOTO finerec
GET #6, , ncl: GET #6, , nci: GET #6, , lv: GET #6, , ar
GET #6, , e: GET #6, , e1
GET #6, , der: GET #6, , seme: GET #6, , ec: GET #6, , in
GET #6, , vmw: GET #6, , vw: GET #6, , pr
GET #6, , ru: GET #6, , rd: GET #6, , rt: GET #6, , nr
GET #6, , cla: GET #6, , sga
CLOSE #6

PRINT "file in corso di elaborazione: "; nf$
 nfc(0) = LEFT$(nf$, 8)
 FOR k = 1 TO 8
  k$ = STR$(k): car = LEN(k$) - 1
  IF car = 1 THEN ext$ = ".A0" + RIGHT$(k$, car)
  IF car = 2 THEN ext$ = ".A" + RIGHT$(k$, car)
  nfc(k) = nfc(0) + ext$
 NEXT k: k = 0

OPEN nfc(5) FOR BINARY AS #5
GET #5, , y2%: GET #5, , y4%: GET #5, , r2%: GET #5, , r3%
GET #5, , ncli: GET #5, , nclf: GET #5, , lvi: GET #5, , lvf
GET #5, , ari: GET #5, , arf: GET #5, , ei: GET #5, , ef
GET #5, , deri: GET #5, , derf: GET #5, , semei: GET #5, , semef
GET #5, , eci: GET #5, , ecf: GET #5, , ini: GET #5, , inf
GET #5, , vmwi: GET #5, , vmwf: GET #5, , vwi: GET #5, , vwf
GET #5, , pri: GET #5, , prf
GET #5, , clai: GET #5, , claf: GET #5, , sgai: GET #5, , sgaf
GET #5, , intercla: GET #5, , interpr
GET #5, , interder: GET #5, , intervar: GET #5, , interve: GET #5, , intervmw: GET #5, , intervw: GET #5, , intercl
GET #5, , y1: GET #5, , sm: GET #5, , cam
CLOSE #5

ncli = ncl + 1: lvi = lv: ari = ar: ei = e1: deri = der: semei = seme
eci = ec: ini = in: vmwi = vmw: vwi = vw: pri = pr: clai = cla: sgai = sga

finerec:
IF ec = -1 THEN PRINT "configurazione dei neuroni e sinapsi non recuperata da HD": SLEEP 5

END SUB

SUB reconfig2 (r2$, r3$, r2%, r3%, y3, atx, m%)

OPEN nfc(7) FOR BINARY AS #7

FOR u = 0 TO 2: FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl
GET #7, , s(u, i, j, k)
NEXT k, j, i, u

FOR li = 0 TO lv: FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl
GET #7, , sp(li, i, j, k)
NEXT k, j, i, li

FOR li = 0 TO lv: GET #7, , t(li): NEXT li

FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl: FOR a = 0 TO nci
GET #7, , w(i, j, k, a)
NEXT a, k, j, i

IF rt = 3 THEN
FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl
GET #7, , d1(i, j, k)
NEXT k, j, i
END IF

GET #7, , y3: GET #7, , m%: GET #7, , r2%: GET #7, , r3%
GET #7, , atx: GET #7, , pmx%
r2$ = INPUT$(1, #7): r3$ = INPUT$(1, #7)

CLOSE #7

OPEN nfc(6) FOR BINARY AS #6
FOR p = 0 TO nr - 1
 FOR q = 0 TO 2
 GET #6, , el(p, q)
NEXT q, p
CLOSE #6

END SUB

SUB reconfig3

OPEN "filincor" FOR BINARY AS #10
nf$ = INPUT$(12, #10)
IF ec = -1 THEN
elb% = 1
PRINT "non c'ä nessun file in elaborazione"
END IF
CLOSE #10: ec = 0

IF elb% = 1 THEN
sceglielab:
PRINT "scrivi il file da continuare (nessuno se vuoi una nuova elaborazione da tastiera)"
FILES "*.UC3": INPUT nf$
IF ec = -1 AND nf$ <> "" THEN GOTO sceglielab
END IF

IF elb% = 0 THEN
OPEN "filincor" FOR BINARY AS #10
nf$ = INPUT$(12, #10)
CLOSE #10
END IF
END SUB

SUB retrodiffusione (lv)
    FOR li = 1 TO lv
        d(li) = 1 / li ^ 2
    NEXT li
END SUB

SUB riassunto (r2$, r3$, r2%, r3%, y2%, y4%)
CLS 0
PRINT
PRINT "cellule di lato"; ncl + 1, "livelli di intorno"; lv
PRINT "valore medio iniziale dei pesi sinaptici"; vmw; " variabilitÖ"; vw
PRINT "rapporto sinapsi eccitatorie/inibitorie"; ec; "/"; in
PRINT "seme dell'avviamento casuale"; seme

PRINT "modifica del peso sinaptico secondo"
PRINT "1-Hebb 2-Kandel 3-retrodiffusione"; ru; rd; rt
PRINT "coefficente di dimenticanza (attivitÖ media dei neuroni sotto la quale diminuisce il peso sinaptico)"; e
PRINT "amplificazione della soglia"; sga
PRINT "retrodiffusione"; der
PRINT
PRINT "neuroni con sinapsi:"
PRINT "1-eccitatorie e inibitorie  2-solo eccitatori o solo inibitori "; r3$
PRINT "modulati in:  Frequenza  Ampiezza "; r2$
PRINT "pendenza della sigmoide di attivazione"; ar
PRINT "periodo refrattario (cicli)"; pr; " apoptosi"; cla
PRINT

END SUB

SUB sequenza
FOR li = 0 TO lv
t(li) = t(li) - 1
IF t(li) = -1 THEN t(li) = lv
NEXT li
END SUB

SUB sinapsilivelli
FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl: a = 0
   FOR l = i - lv TO i + lv
    FOR m = j - lv TO j + lv
        FOR n = k - lv TO k + lv
            IF l = i AND m = j AND n = k THEN n = n + 1
            ll = ABS(l - i): lm = ABS(m - j): ln = ABS(n - k)
            li = ll: IF lm > li THEN li = lm
            IF ln > li THEN li = ln
            wlw(i, j, k, a) = li
            a = a + 1

NEXT n, m, l, k, j, i

END SUB

SUB sinapsilivelli2
FOR l = i - lv TO i + lv
  FOR m = j - lv TO j + lv
       FOR n = k - lv TO k + lv
           IF l = i AND m = j AND n = k THEN n = n + 1
            ll = ABS(l - i): lm = ABS(m - j): ln = ABS(n - k)
            li = ll: IF lm > li THEN li = lm
            IF ln > li THEN li = ln
            wl(a) = li
            a = a + 1

NEXT n, m, l

END SUB

SUB tempi
FOR li = 0 TO lv
t(li) = li
NEXT li
END SUB

SUB vediDPS (g%, y, rs, nf$, nsn)

CLS : SCREEN 0
REDIM dpst(lv, 20) AS INTEGER
FOR i = 0 TO ncl: FOR j = 0 TO ncl: FOR k = 0 TO ncl: FOR a = 0 TO nci
li = wlw(i, j, k, a) - 1
x = CINT((w(i, j, k, a) + 10000) / 1000)
dpst(li, x) = dpst(li, x) + 1
IF dpst(li, x) > mx THEN mx = dpst(li, x)
NEXT a, k, j, i
FOR li = 0 TO lv - 1
CLS : PRINT "livello "; li + 1; "  premi un tasto per successivo": riga = CSRLIN
FOR x = 0 TO 20: dpst = dpst(li, x): riga = riga + 1: LOCATE riga, 1
PRINT CINT(x * 1000) - 10000; dpst: yc = INT(dpst * 65 / mx): LOCATE riga, 15
FOR col = 15 TO yc + 15: PRINT CHR$(176); : NEXT col: PRINT
totdpst = totdpst + dpst
NEXT x
SLEEP
NEXT li
PRINT "sinapsi totali "; totdpst
IF totdpst <> nsn THEN PRINT "SUB vedi DPS nuova inaffidabile: le sinapsi sono: "; nsn
totdpst = 0
PRINT "per continuare premi un tasto": SLEEP

END SUB

SUB vedipesi
PRINT : PRINT "coordinate i,j,k del neurone  (0-"; ncl; ")": INPUT ii, jj, kk
PRINT : CLS 0
lv2 = lv: riga = CSRLIN: a = 0: col1 = 1: stepcol = (lv * 2 + 1) * 3 + 3
PRINT "sinapsi (valore 0..100) in INPUT al neurone"; ii; jj; kk: rg = 2
 FOR l = ii - lv TO ii + lv: LOCATE (riga + rg), col1
 PRINT "livello l="; lv2: lv2 = lv2 - 1
  FOR m = jj - lv TO jj + lv: rg = rg + 1: LOCATE (riga + rg), col1
   FOR n = kk - lv TO kk + lv
   IF l = ii AND m = jj AND n = kk THEN PRINT "nc"; : n = n + 1
   PRINT CINT(w(ii, jj, kk, a) / 100); : a = a + 1
   NEXT n: PRINT
  NEXT m: PRINT : rg = 2: col1 = col1 + stepcol

  IF col1 + stepcol > 79 THEN col1 = 1
  IF col1 = 1 THEN riga = CSRLIN + 1

 NEXT l

 riga = CSRLIN + 1
 IF 2 * riga > 25 THEN
 PRINT : PRINT " premi un tasto per sinapsi in OUTPUT": SLEEP
 CLS
 END IF

lv2 = lv: riga = CSRLIN: a = nci: col1 = 1
PRINT "OUTPUT del neurone (0..100)"; ii; jj; kk: rg = 2
 FOR l = ii - lv TO ii + lv: LOCATE (riga + rg), col1
 l1 = limiti(l)
 PRINT "livello l="; lv2: lv2 = lv2 - 1
  FOR m = jj - lv TO jj + lv: rg = rg + 1: LOCATE (riga + rg), col1
  m1 = limiti(m)
   FOR n = kk - lv TO kk + lv
   IF l = ii AND m = jj AND n = kk THEN PRINT "nc"; : n = n + 1
   n1 = limiti(n)
   PRINT CINT(w(l1, m1, n1, a) / 100); : a = a - 1
   NEXT n: PRINT
  NEXT m: PRINT : rg = 2: col1 = col1 + stepcol

  IF col1 + stepcol > 79 THEN col1 = 1
  IF col1 = 1 THEN riga = CSRLIN + 1

 NEXT l: PRINT : PRINT


END SUB

SUB vedisigm (sgmax, amd, amd2, u)
nc1 = nci + 1: ar = 4: ar2 = 4: sgm = CINT(sgmax): intv = sgmax / 50: u = 0

CLS : PRINT "premi i tasti cursore del tastierino numerico; termina con f1"
PRINT "pendenza della sigmoide di attivazione dei neuroni (1-8)"
LOCATE 7, 31: PRINT "1": LOCATE 10, 31: PRINT "0"
LOCATE 11, 23: PRINT "att.max.intorno"
LOCATE 9, 23: PRINT -sgm: LOCATE 9, 38: PRINT sgm
xc = CINT(amd * 7 / nc1 + 200 / 8): LOCATE 12, xc: PRINT "att.med"; amd
xc = CINT(amd2 * 7 / nc1 + 200 / 8): LOCATE 12, xc: PRINT amd2
'xc = CINT(sgmax * 7 / nc1): LOCATE 11, xc: PRINT "sgmax"; sgmax

SCREEN 11
VIEW (200, 60)-(300, 130), 0, 1: WINDOW (-nc1, 0)-(nc1, 1)
LINE (1, 0)-(0, 0): LINE (amd, 1)-(amd, 0)
LINE (amd2, 1)-(amd2, 0)
DO
at = 1 / (1 + EXP(-nc1))
PSET (nt, at), 1: atp2 = at
at = 1 / (1 + EXP(-nc1))
PSET (nt, at): atp = at
FOR nt = -sgm TO sgm STEP intv
 at = 1 / (1 + EXP(-nt * 10 / ar2))
 LINE (nt, atp2)-(nt, at), 0: atp2 = at
 at = 1 / (1 + EXP(-nt * 10 / ar))
 LINE (nt, atp)-(nt, at), 1: atp = at
NEXT nt
LOCATE 2, 59: PRINT ar: LOCATE 3, 32: ar2 = ar
SLEEP
LOOP WHILE u = 0
CLS 0: LOCATE 14, 1: ar = ar2
END SUB

SUB vedneursogl (a$)
PRINT : CLS 0: riga = CSRLIN + 2
SELECT CASE a$

CASE IS = "s"
FOR i = 0 TO ncl: a1 = i / 2: a2% = i / 2

IF (riga + 1 + ncl) >= 25 THEN
PRINT "per continuare premi un tasto": SLEEP
CLS (0): PRINT : riga = CSRLIN + 2
END IF

IF a1 = a2% THEN colonna = 1 ELSE colonna = 40
LOCATE riga, colonna
PRINT "livello i="; i
 FOR j = 0 TO ncl: LOCATE (riga + 1 + j), colonna
  FOR k = 0 TO ncl
  IF s(1, i, j, k) = -1 THEN PRINT "M";  ELSE PRINT s(0, i, j, k);
  NEXT k: PRINT
 NEXT j: PRINT
 IF a1 <> a2% THEN riga = CSRLIN + 1
NEXT i

CASE ELSE
FOR i = 0 TO ncl: a1 = i / 2: a2% = i / 2

IF (riga + 1 + ncl) >= 25 THEN
PRINT "per continuare premi un tasto": SLEEP
CLS (0): PRINT : riga = CSRLIN + 2
END IF

IF a1 = a2% THEN colonna = 1 ELSE colonna = 40
LOCATE riga, colonna
PRINT "livello i="; i
 FOR j = 0 TO ncl: LOCATE (riga + 1 + j), colonna
  FOR k = 0 TO ncl
  IF s(1, i, j, k) = -1 THEN PRINT "M";  ELSE PRINT sp(t(0), i, j, k);
  NEXT k: PRINT
 NEXT j: PRINT
 IF a1 <> a2% THEN riga = CSRLIN + 1
NEXT i

END SELECT

IF a$ = "s" THEN PRINT "soglie " ELSE PRINT "attivazione(0...10000)"
PRINT "k 0 --"; CHR$(26); ncl
PRINT "j 0"
PRINT "  "; CHR$(179)
PRINT "  "; CHR$(25)
PRINT " "; ncl
END SUB

