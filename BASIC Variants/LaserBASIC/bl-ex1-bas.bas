2 REM BASIC Lightning Supplement Manual Example Page 7
5 REM TUNNEL
10 MULTI: S4COL: HIRES: ATT2ON
20 SETATR BLUE, WHITE, CYAN : HBORDER2: HPAPER2: SCLR0, ATR
30 FOR R=1 TO 60 STEP9
40 MODE1: W=R: GOSUB100: W=R+1: GOSUB100: W=R+2: GOSUB100
50 MODE2: W=R+3: GOSUB100: W=R+4: GOSUB100: W=R+5: GOSUB100
60 MODE3: W=R+6: GOSUB100: W=R+7: GOSUB100: W=R+8: GOSUB100
70 NEXTR
80 SETATR 7,2,5: GOSUB200
81 SETATR 2,5,7: GOSUB200
82 SETATR 5,7,2: GOSUB200
83 GOTO80
100 POLY0,160,100,W,W,80,0:RETURN
200 SETA 0,11,4,18,17,ATR: FOR N=1 TO 20: NEXT N: RETURN