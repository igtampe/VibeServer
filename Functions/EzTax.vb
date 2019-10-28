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

        ElseIf EZTAXMSG.StartsWith("UPD") Then
            Return UpdateIncome(EZTAXMSG.Remove(0, 3))
        Else
            ToConsole("Could not parse EZTAX Command")
            Return "E"
        End If
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

            Income = ReadFromFile(UMSWEBDir & "\SSH\USERS\" & ID & "\Income.dll")

            If File.Exists(UMSWEBDir & "\SSH\USERS\" & ID & "\EI.dll") Then
                EI = ReadFromFile(UMSWEBDir & "\SSH\USERS\" & ID & "\EI.dll")
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
        Dim ID As String = EZTAXMSG.Remove(5, EZTAXMSG.Count - 5)
        Dim NewIncome As Long = EZTAXMSG.Remove(0, 5)

        If ID.Count = 5 Then
            Try
                ToFile(UMSWEBDir & "\SSH\USERS\" & ID & "\Income.dll", NewIncome)
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

        End If
    End Function

End Class
