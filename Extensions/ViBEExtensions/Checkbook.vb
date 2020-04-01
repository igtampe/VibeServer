Imports Utils
Imports System.IO

''' <summary>
''' Checkbook 2000 Extension
''' </summary>
Public Class Checkbook

    ''' <summary>
    ''' Execute Checkbook Functions
    ''' </summary>
    ''' <param name="CHCKMSG"></param>
    ''' <returns></returns>
    Public Shared Function CHCKBK(CHCKMSG As String) As String
        Try

            ToConsole("invoking Checkbook 2000 System")

            If CHCKMSG.StartsWith("READ") Then
                Return ReadChecks(CHCKMSG.Replace("READ", ""))

            ElseIf CHCKMSG.StartsWith("REMO") Then
                Return RemoCheck(CHCKMSG.Replace("REMO", ""))

            ElseIf CHCKMSG.StartsWith("ADD") Then
                Return AddCheck(CHCKMSG.Replace("ADD", ""))

            Else
                Return "E"
            End If

        Catch e As Exception
            ErrorToConsole("Something Happened", e)
            Return "E"

        End Try
    End Function

    ''' <summary>
    ''' Read All Checks
    ''' </summary>
    ''' <param name="CHCKMSG"></param>
    ''' <returns></returns>
    Private Shared Function ReadChecks(CHCKMSG As String) As String
        CHCKMSG = CHCKMSG.Replace("READ", "")
        Dim notifarray() As String
        ToConsole("trying to READ from " & CHCKMSG & "'s messages")
        If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", CHCKMSG, "\chckbk.txt")) Then
            Return "N"
        End If
        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", CHCKMSG, "\chckbk.txt"), OpenMode.Input)
        Dim I As Integer = 0
        ReDim notifarray(0)
        notifarray(0) = "NothingPls"

        While Not EOF(1)
            ReDim Preserve notifarray(I)
            notifarray(I) = LineInput(1)
            ToConsole("Found Check " & I)
            I += 1
        End While

        FileClose(1)

        If notifarray(0) = "NothingPls" Then
            Return "F"
        End If

        Return String.Join("`", notifarray)
    End Function

    ''' <summary>
    ''' Removes a specified Check
    ''' </summary>
    ''' <param name="CHCKMSG"></param>
    ''' <returns></returns>
    Private Shared Function RemoCheck(CHCKMSG As String) As String
        '5717410
        Dim notifindex As String = CHCKMSG.Remove(0, 5)
        Dim notifuser As String = CHCKMSG.Remove(5, notifindex.Length)

        If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\chckbk.txt")) Then
            Return "N"
        End If

        ToConsole("Trying to remove index " & notifindex & " from " & notifuser & "'s notification file")
        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\chckbk.txt"), OpenMode.Input)
        FileOpen(2, String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\tempchckbk.txt"), OpenMode.Output)
        Dim I As Integer = 0
        While Not EOF(1)
            If I = notifindex Then
                LineInput(1)
                GoTo Chckskipwhile
            End If
            ToConsole("Copying Index " & I)
            PrintLine(2, LineInput(1))
Chckskipwhile:
            I += 1
        End While
        FileClose(1)
        FileClose(2)
        ToConsole("finishing up")


        File.Delete(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\chckbk.txt"))
        File.Move(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\tempchckbk.txt"), String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\chckbk.txt"))
        Return "S"
    End Function

    ''' <summary>
    ''' Adds a Check
    ''' </summary>
    ''' <param name="CHCKMSG"></param>
    ''' <returns></returns>
    Private Shared Function AddCheck(CHCKMSG As String) As String
        Dim ChckWrite As String = CHCKMSG.Remove(0, 5)
        Dim Chckuser As String = CHCKMSG.Remove(5, ChckWrite.Length)
        Dim chckdetails() As String = ChckWrite.Split("`")

        ToConsole("Adding '" & ChckWrite & "' to " & Chckuser & "'s chckbck.txt")

        Try
            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\users\", Chckuser, "\chckbk.txt"), OpenMode.Append)
            PrintLine(1, ChckWrite)
            FileClose(1)

            ToConsole("Writing to Checkbook")

            Select Case chckdetails(0)
                Case "0"
                    FileOpen(1, String.Concat(UMSWEBDir, "\SSH\users\", Chckuser, "\notifs.txt"), OpenMode.Append)
                    '0`12/4/2018 7:42:42 PM`A Test Account`57174\UMSNB`100`This is a Check
                    PrintLine(1, DateTime.Now.ToString & "`You have a new check from " & chckdetails(2) & " that's worth " & CInt(chckdetails(4)).ToString("N0") & "p")
                    FileClose(1)

                Case "1"
                    FileOpen(1, String.Concat(UMSWEBDir, "\SSH\users\", Chckuser, "\notifs.txt"), OpenMode.Append)
                    '1`12/4/2018 7:42:42 PM`A Test Account`57174\UMSNB`100`This is a Bill
                    PrintLine(1, DateTime.Now.ToString & "`You have a new bill from " & chckdetails(2) & " that's worth " & CInt(chckdetails(4)).ToString("N0") & "p")
                    FileClose(1)


            End Select



            Return "S"
            ToConsole("OK done")
        Catch ex As Exception
            ErrorToConsole("Something Happened", ex)
            Try
                FileClose(1)
            Catch

            End Try
            Return "E"
        End Try


    End Function

End Class
