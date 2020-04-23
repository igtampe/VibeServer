''' <summary>Notifications Expansion</summary>
Public Module Notif
    ''' <summary>Notification Subsystem</summary>
    Public Function Notifications(ByRef VUser As ViBEUser, notifmsg As String) As String
        Try
            ToConsole("invoking Notif System")

            If notifmsg.StartsWith("READ") Then
                'READ
                Return VUser.NotifHandler.GetNotifs
            ElseIf notifmsg.StartsWith("CLEAR") Then
                'CLEAR
                VUser.NotifHandler.ClearNotifs()
                Return "S"
            ElseIf notifmsg.StartsWith("REMO") Then
                'REMO4
                VUser.NotifHandler.RemoveNotif(notifmsg.Substring(4))
                Return "S"
            Else
                ToConsole("Could not parse notif message")
                Return "E"
            End If

        Catch e As Exception
            ErrorToConsole("An error occurred in the notification system", e)
            Return "E"
        End Try

    End Function

End Module
