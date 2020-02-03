Imports Utils
Imports System.IO

''' <summary>
''' EzTax Expansion
''' </summary>
Public Class EzTax

    ''' <summary>
    ''' Executes EzTax Commands
    ''' </summary>
    ''' <param name="EZTAXMSG"></param>
    ''' <returns></returns>
    Public Shared Function EZT(EZTAXMSG As String) As String
        Console.WriteLine("[" & DateTime.Now.ToString & "] EZTax Has been invoked")

        'INF57174                  All Tax Information
        'UPD57174XXXXX             Update Income of specified User

        If EZTAXMSG.StartsWith("INF") Then
            Return Info(EZTAXMSG.Remove(0, 3))

        ElseIf EZTAXMSG.StartsWith("BRK") Then
            Return Breakdown(EZTAXMSG.Remove(0, 3))

        ElseIf EZTAXMSG.StartsWith("UPD") Then
            Return UpdateIncome(EZTAXMSG.Remove(0, 3))
        Else
            ToConsole("Could not parse EZTAX Command")
            Return "E"
        End If
    End Function

    Private Shared Function Breakdown(ID As String) As String
        Dim Income As String
        Call ToConsole("Attempting to send IncomeBreakdown on user (" & ID & ")")
        Try

            If File.Exists(UserFile(ID, "Breakdown.dll")) Then
                Income = ReadFromFile(UserFile(ID, "Breakdown.dll"))
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
    Private Shared Function Info(ID As String) As String
        Dim Income As Long
        Dim EI As Long
        Call ToConsole("Attempting to send Information on user (" & ID & ")")
        Try

            Income = ReadFromFile(UserFile(ID, "Income.dll"))

            If File.Exists(UserFile(ID, "EI.dll")) Then
                EI = ReadFromFile(UserFile(ID, "EI.dll"))
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

    ''' <summary>
    ''' Update Income of someone
    ''' </summary>
    ''' <param name="EZTAXMSG"></param>
    ''' <returns></returns>
    Private Shared Function UpdateIncome(EZTAXMSG As String) As String
        Dim SplitIncome As String() = EZTAXMSG.Split(",")

        'EZTax UPDATE Message:
        'ID,TotalIncome,NewpondIncome,UrbiaIncome,ParadisusIncome,LaertesIncome,NOIncome,SOIncome
        'EZTUPD57174,0,0,0,0,0,0


        If SplitIncome.Count = 1 Then
            Return UpdateIncomeClassic(EZTAXMSG)
        End If

        Dim ID As String
        Dim TotalIncome As Long
        Dim NewpondIncome As Long
        Dim UrbiaIncome As Long
        Dim ParadisusIncome As Long
        Dim LaertesIncome As Long
        Dim NOIncome As Long
        Dim SOIncome As Long

        Try
            ID = SplitIncome(0)
            TotalIncome = SplitIncome(1)
            NewpondIncome = SplitIncome(2)
            UrbiaIncome = SplitIncome(3)
            ParadisusIncome = SplitIncome(4)
            LaertesIncome = SplitIncome(5)
            NOIncome = SplitIncome(6)
            SOIncome = SplitIncome(7)

        Catch ex As Exception
            ErrorToConsole("Improperly Coded UpdateINcome Request", ex)
            Return "E"
        End Try

        ToConsole("Updating Income Classically")
        If UpdateIncomeClassic(ID & TotalIncome) = "E" Then
            Return "E"
        End If
        ToConsole("Updating Income Breakdown")

        Try
            ToFile(UserFile(ID, "Breakdown.dll"), NewpondIncome & "," & UrbiaIncome & "," & ParadisusIncome & "," & LaertesIncome & "," & NOIncome & "," & SOIncome)
        Catch ex As Exception
            ErrorToConsole("Could not update income", ex)
            Return "E"
        End Try

        Return "S"


    End Function

    Private Shared Function UpdateIncomeClassic(EZTAXMSG As String) As String
        Dim ID As String = EZTAXMSG.Remove(5, EZTAXMSG.Count - 5)
        Dim NewIncome As Long = EZTAXMSG.Remove(0, 5)

        If ID.Count = 5 Then
            Try
                ToFile(UserFile(ID, "Income.dll"), NewIncome)
            Catch ex As Exception
                ErrorToConsole("Could not update income", ex)
                Return "E"
            End Try

            Try
                AddToFile("IncomeManagementLog.log", "[" & DateTime.Now.ToString & "] " & ID & " Has modified their income to be " & NewIncome.ToString("N0"))
            Catch ex As Exception
                ErrorToConsole("Could not save income modification", ex)
                ToConsole("The user still received an S")
            End Try

            Return "S"

        Else
            Return "E"
        End If
    End Function

End Class
