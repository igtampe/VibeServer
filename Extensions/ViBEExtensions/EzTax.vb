Imports System.IO

''' <summary>EzTax Expansion</summary>
Public Module EzTax

    ''' <summary>Executes EzTax Commands</summary>
    ''' <param name="EZTAXMSG"></param>
    ''' <returns></returns>
    Public Function EZT(ByRef Vuser As ViBEUser, EZTAXMSG As String) As String
        Console.WriteLine("[" & DateTime.Now.ToString & "] EZTax Has been invoked")

        'INF57174                  All Tax Information
        'UPD57174XXXXX             Update Income of specified User

        If EZTAXMSG.StartsWith("INF") Then
            Return Info(Vuser)

        ElseIf EZTAXMSG.StartsWith("BRK") Then
            Return Breakdown(Vuser)

        ElseIf EZTAXMSG.StartsWith("UPD") Then
            Return UpdateIncome(Vuser, EZTAXMSG.Remove(0, 3))
        Else
            ToConsole("Could not parse EZTAX Command")
            Return "E"
        End If
    End Function

    Private Function Breakdown(ByRef Vuser As ViBEUser) As String
        Dim Income As String
        Call ToConsole("Attempting to send IncomeBreakdown on user (" & Vuser.ToString & ")")
        Try

            If File.Exists(Vuser.Directory & "Breakdown.dll") Then
                Income = ReadFromFile(Vuser.Directory & "Breakdown.dll")
                ToConsole("Sent income Breakdown: (" & Income & ")")
            Else
                Income = "0,0,0,0,0,0"
                ToConsole("Could not find Breakdown. Sent Blank Income in return: (" & Income & ")")
            End If

            Return Income

        Catch ex As Exception
            ErrorToConsole("Could not retrieve information.", ex)
            Return "E"
        End Try
    End Function

    ''' <summary>
    ''' Gets Income Info from the specified User
    ''' </summary>
    ''' <param name="ID"></param>
    ''' <returns></returns>
    Private Function Info(ByRef Vuser As ViBEUser) As String
        Dim Income As Long
        Dim EI As Long
        Call ToConsole("Attempting to send Information on user (" & Vuser.ToString & ")")
        Try

            Income = ReadFromFile(Vuser.Directory & "Income.dll")

            If File.Exists(Vuser.Directory & "EI.dll") Then
                EI = ReadFromFile(Vuser.Directory & "EI.dll")
            Else
                EI = 0
            End If

            ToConsole("Sent information income: (" & Income & ") and extra income (" & EI & ")")
            Return Income & "," & EI

        Catch ex As Exception
            ErrorToConsole("Could not retrieve information.", ex)
            Return "E"
        End Try

    End Function

    ''' <summary>Update Income of someone</summary>
    Private Function UpdateIncome(ByRef Vuser As ViBEUser, EZTAXMSG As String) As String
        Dim SplitIncome As String() = EZTAXMSG.Split(",")

        'EZTax UPDATE Message:
        'ID,TotalIncome,NewpondIncome,UrbiaIncome,ParadisusIncome,LaertesIncome,NOIncome,SOIncome
        'EZTUPD0,0,0,0,0,0

        If SplitIncome.Count = 1 Then
            ToConsole("Breakdown not received, updating only classically")
            Return UpdateIncomeClassic(Vuser, EZTAXMSG)
        End If

        Dim TotalIncome As Long
        Dim NewpondIncome As Long
        Dim UrbiaIncome As Long
        Dim ParadisusIncome As Long
        Dim LaertesIncome As Long
        Dim NOIncome As Long
        Dim SOIncome As Long

        Try
            TotalIncome = SplitIncome(0)
            NewpondIncome = SplitIncome(1)
            UrbiaIncome = SplitIncome(2)
            ParadisusIncome = SplitIncome(3)
            LaertesIncome = SplitIncome(4)
            NOIncome = SplitIncome(5)
            SOIncome = SplitIncome(6)

        Catch ex As Exception
            ErrorToConsole("Improperly Coded UpdateINcome Request", ex)
            Return "E"
        End Try

        ToConsole("Updating Income Classically")
        If UpdateIncomeClassic(Vuser, TotalIncome) = "E" Then
            Return "E"
        End If
        ToConsole("Updating Income Breakdown")

        Try
            ToFile(Vuser.Directory & "Breakdown.dll", NewpondIncome & "," & UrbiaIncome & "," & ParadisusIncome & "," & LaertesIncome & "," & NOIncome & "," & SOIncome)
        Catch ex As Exception
            ErrorToConsole("Could not update income", ex)
            Return "E"
        End Try

        Return "S"


    End Function

    Private Function UpdateIncomeClassic(ByRef Vuser As ViBEUser, NewIncome As Long) As String

        Try
            ToFile(Vuser.Directory & "Income.dll", NewIncome)
        Catch ex As Exception
            ErrorToConsole("Could not update income", ex)
            Return "E"
        End Try

        Try
            AddToFile("IncomeManagementLog.log", "[" & DateTime.Now.ToString & "] " & Vuser.ToString & " Has modified their income to be " & NewIncome.ToString("N0"))
        Catch ex As Exception
            ErrorToConsole("Could not save income modification", ex)
            ToConsole("The user still received an S")
        End Try

        Return "S"

    End Function

End Module
