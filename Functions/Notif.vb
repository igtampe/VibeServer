Imports System.IO
Imports Utils

''' <summary>
''' Notifications Expansion
''' </summary>
Public Class Notif
    ''' <summary>
    ''' Notification Subsystem
    ''' </summary>
    ''' <param name="notifmsg"></param>
    ''' <returns></returns>
    Public Shared Function Notifications(notifmsg As String) As String
        Try
            ToConsole("invoking Notif System")

            If notifmsg.StartsWith("READ") Then
                Return ReadNotifs(notifmsg.Replace("READ", ""))
            ElseIf notifmsg.StartsWith("CLEAR") Then
                Return ClearNotifs(notifmsg.Replace("CLEAR", ""))
            ElseIf notifmsg.StartsWith("REMO") Then
                Return RemoveNotif(notifmsg.Replace("REMO", ""))
            Else
                ToConsole("Could not parse notif message")
                Return "E"
            End If

        Catch e As Exception
            ErrorToConsole("An error occurred in the notification system", e)
            Return "E"
        End Try

    End Function

    ''' <summary>
    ''' Read All Notifications
    ''' </summary>
    ''' <param name="NotifMSG"></param>
    ''' <returns></returns>
    Private Shared Function ReadNotifs(NotifMSG As String) As String
        Dim notifarray() As String
        ToConsole("trying to READ from " & NotifMSG & "'s messages")

        If Not File.Exists(UMSWEBDir & "\SSH\USERS\" & NotifMSG & "\notifs.txt") Then
            Return "N"
        End If

        FileOpen(1, UMSWEBDir & "\SSH\USERS\" & NotifMSG & "\notifs.txt", OpenMode.Input)

        Dim I As Integer = 0

        While Not EOF(1)
            ReDim Preserve notifarray(I)
            notifarray(I) = LineInput(1)
            ToConsole("Found Notif " & I)
            I = I + 1
        End While

        FileClose(1)

        Return String.Join("`", notifarray)
    End Function

    ''' <summary>
    ''' Clear All Notifications
    ''' </summary>
    ''' <param name="Notifuser"></param>
    ''' <returns></returns>
    Private Shared Function ClearNotifs(Notifuser As String) As String
        ToConsole("Attempting to remove all of " & Notifuser & "'s notifications")
        Try
            File.Delete(String.Concat(UMSWEBDir, "\SSH\USERS\", Notifuser, "\notifs.txt"))
            ToConsole("OK I did it yay")
            Return "S"
        Catch ex As exception
            ErrorToConsole("Oh no something happened", ex)
            Return "E"
        End Try
    End Function

    ''' <summary>
    ''' Remove a specified notification
    ''' </summary>
    ''' <param name="NotifMSG"></param>
    ''' <returns></returns>
    Private Shared Function RemoveNotif(NotifMSG As String) As String

        Dim notifindex As String = NotifMSG.Remove(0, 5)
        Dim notifuser As String = NotifMSG.Remove(5, notifindex.Length)

        If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\notifs.txt")) Then
            Return "N"
        End If

        ToConsole("Trying to remove index " & notifindex & " from " & notifuser & "'s notification file")

        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\notifs.txt"), OpenMode.Input)
        FileOpen(2, String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\tempnotifs.txt"), OpenMode.Output)

        Dim I As Integer = 0

        While Not EOF(1)
            If I = notifindex Then
                LineInput(1)
                GoTo notifskipwhile
            End If
            ToConsole("Copying Index " & I)
            PrintLine(2, LineInput(1))
notifskipwhile:
            I = I + 1
        End While
        FileClose(1)
        FileClose(2)
        ToConsole("finishing up")


        File.Delete(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\notifs.txt"))
        File.Move(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\tempnotifs.txt"), String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\notifs.txt"))
        Return "S"

    End Function


End Class
