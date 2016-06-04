Public Class Comparator

    Public Function FuzzyStringCompare(ByVal etalon As String, ByVal subject As String) As Integer
        If Len(etalon) = 0 Or Len(subject) = 0 And Len(etalon) <> Len(subject) Then
            FuzzyStringCompare = 0
        Else
            etalon = Replace(etalon, " ", "")
            subject = Replace(subject, " ", "")
            Dim i As Integer
            Dim j As Integer
            Dim lastFoundPos As Integer
            lastFoundPos = 0
            Dim res1 As Integer
            Dim res2 As Integer
            'сравниваем побуквенно эталон с субъектом и наоборот
            ' ищем буквы эталона
            For i = 1 To Len(etalon)
                For j = lastFoundPos + 1 To Len(subject)
                    If LCase(Mid(etalon, i, 1)) = LCase(Mid(subject, j, 1)) Then
                        '++ math.inc(res1)
                        res1 = res1 + 1
                        lastFoundPos = j
                        ''tracetranslit Mid(etalon,i,1) & "(" & i & ")" & "=" & Mid(subject, j, 1) & "(" & j & ")" & "( res = " & res1 & ")"
                        Exit For
                    End If
                Next
            Next
            res1 = res1 * 100 / Len(etalon)
            ''tracetranslit etalon & " in " & subject & " = " & Str(res1)

            ''lastFoundPos = 0
            ' ищем буквы субъекта
            ''For i = 1 To Len(subject)
            ''   For j = lastFoundPos+1 To Len(etalon)
            ''      If LCase(Mid(etalon,j,1)) = LCase(Mid(subject, i, 1)) Then
            '++ math.inc(res1)
            ''         res2 = res2+1
            ''         lastFoundPos = j
            ''tracetranslit Mid(etalon,j,1) & "(" & j & ")" & "=" & Mid(subject, i, 1) & "(" & i & ")" & "( res = " & res2 & ")"
            ''         Exit For
            ''      End If
            ''   Next
            ''Next
            ''res2 = res2*100/Len(subject)
            ''tracetranslit subject & " in " & etalon & " = " & Str(res2)

            ' возвращаем среднее от результата двух сравнений
            'FuzzyStringCompare = (res1+res2)/2
            Dim firstLet, firstLetterCoef As Double
            firstLet = 0
            If (Left(etalon, 1) = Left(subject, 1)) Then firstLet = 100


            Dim etalonCoef, subjectCoef, lenCoef As Integer
            etalonCoef = 2
            subjectCoef = 0
            lenCoef = 0
            firstLetterCoef = 0
            FuzzyStringCompare = (firstLet * firstLetterCoef + res1 * etalonCoef + 100 * (1 - Abs(Len(etalon) - Len(subject)) / Len(etalon)) * lenCoef) / (etalonCoef + subjectCoef + lenCoef + firstLetterCoef)
            'FuzzyStringCompare = res1*res2/100
        End If
    End Function


    Public Function AmountOfLettersOfWordInOtherWord(ByVal etalon As String, ByVal subject As String) As Double

        Const maxDiff = 3
        Dim i As Integer
        Dim j As Integer
        Dim lastFoundPos As Integer
        lastFoundPos = 0
        Dim res1 As Double
        Dim res2 As Double

        For i = 1 To Len(etalon)
            For j = lastFoundPos + 1 To Len(subject)
                If j > lastFoundPos + 1 + maxDiff Then Exit For

                If LCase(Mid(etalon, i, 1)) = LCase(Mid(subject, j, 1)) Then
                    '++ math.inc(res1)
                    res1 = res1 + 1
                    lastFoundPos = j
                    If InStr(i + 1, etalon, Mid(etalon, i, 1)) > 0 Then
                        lastFoundPos = i
                    End If
                    If (etalon = "Аораханскаяобласть" Or subject = "Аораханскаяобласть") And (etalon = "Астраханскаяобласть" Or subject = "Астраханскаяобласть") Then
                        'Trace Mid(etalon,i,1) & "(" & i & ")" & "=" & Mid(subject, j, 1) & "(" & j & ")" & "( res = " & res1 & ")"
                    End If
                    Exit For
                End If
            Next
        Next
        AmountOfLettersOfWordInOtherWord = res1 * 100 / Len(etalon)
    End Function


    Public Function FuzzyStringCompare_2side(ByVal etalon As String, ByVal subject As String) As Integer
        If Len(etalon) = 0 Or Len(subject) = 0 And Len(etalon) <> Len(subject) Then
            FuzzyStringCompare_2side = 0
        Else
            etalon = Replace(etalon, " ", "")
            subject = Replace(subject, " ", "")
            Dim i As Integer
            Dim j As Integer
            Dim lastFoundPos As Integer
            lastFoundPos = 0
            Dim res1 As Integer
            Dim res2 As Integer

            'сравниваем побуквенно эталон с субъектом и наоборот
            ' ищем буквы эталона

            res1 = AmountOfLettersOfWordInOtherWord(etalon, subject)
            res2 = AmountOfLettersOfWordInOtherWord(subject, etalon)

            '      lastFoundPos = 0
            '      ' ищем буквы субъекта
            '      For i = 1 To Len(subject)
            '         For j = lastFoundPos+1 To Len(etalon)
            '            If LCase(Mid(etalon,j,1)) = LCase(Mid(subject, i, 1)) Then
            '               '++ math.inc(res1)
            '               res2 = res2+1
            '               lastFoundPos = j
            '               If InStr(i+1, subject, Mid(subject,i,1)) >0 Then
            '                  lastFoundPos = i
            '               End If
            '               If (etalon = "Аораханскаяобласть" Or subject = "Аораханскаяобласть") And (etalon = "Астраханскаяобласть" Or subject = "Астраханскаяобласть") Then
            '                  'Trace Mid(etalon,j,1) & "(" & j & ")" & "=" & Mid(subject, i, 1) & "(" & i & ")" & "( res = " & res2 & ")"
            '               End If
            '               Exit For
            '            End If
            '         Next
            '      Next
            '      res2 = res2*100/Len(subject)


            'Debug.Print etalon & " in " & subject & " = " & Str(res1)
            'Debug.Print subject & " in " & etalon & " = " & Str(res2)

            ' возвращаем среднее от результата двух сравнений
            'FuzzyStringCompare = (res1+res2)/2
            Dim firstLet, firstLetterCoef As Double
            firstLet = 0
            If (Left(etalon, 1) = Left(subject, 1)) Then firstLet = 100


            Dim etalonCoef, subjectCoef, lenCoef As Integer
            etalonCoef = 1
            subjectCoef = 1
            lenCoef = 0
            firstLetterCoef = 0
            FuzzyStringCompare_2side = (firstLet * firstLetterCoef + res1 * etalonCoef + res2 * subjectCoef + 100 * (1 - Abs(Len(etalon) - Len(subject)) / Len(etalon)) * lenCoef) / (etalonCoef + subjectCoef + lenCoef + firstLetterCoef)

            'If etalon = "Аораханскаяобласть" Or subject = "Аораханскаяобласть" Then
            '   If False Then
            '       Trace etalon & " in " & subject & " = " & Str(res1)
            'Trace subject & " in " & etalon & " = " & Str(res2)
            'Trace "result=" & FuzzyStringCompare_2side
            'End If
            'FuzzyStringCompare = res1*res2/100
        End If
    End Function
End Class
