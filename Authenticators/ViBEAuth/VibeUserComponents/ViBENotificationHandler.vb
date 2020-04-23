Imports System.Collections
Imports System.IO

Public Class ViBENotificationHandler

    Public ReadOnly AllNotifs As ArrayList
    Private NotifFile As String

    Public Sub New(ByRef TiedUser As ViBEUser)

        NotifFile = TiedUser.Directory & "Notifs.txt"

        AllNotifs = New ArrayList

        FileOpen(1, NotifFile, OpenMode.Input)

        While Not EOF(1)
            AllNotifs.Add(New ViBENotification(LineInput(1)))
        End While

    End Sub

    Public Sub ClearNotifs()
        ToConsole("Clearing Notifications")
        AllNotifs.Clear()
        SaveNotifs()
    End Sub

    Public Sub AddNotif(Message As String)
        ToConsole("Adding Notification '" & Message & "'")
        AllNotifs.Add(New ViBENotification(DateTime.Now.ToString, Message))
        SaveNotifs()
    End Sub

    Public Sub RemoveNotif(Index As Integer)
        ToConsole("Removing Notification " & Index)
        AllNotifs.RemoveAt(Index)
        SaveNotifs()
    End Sub

    Public Function GetNotifs() As String
        Return String.Join("`", AllNotifs.ToArray)
    End Function

    Public Sub SaveNotifs()
        ToConsole("Saving Notifications")
        If File.Exists(NotifFile) Then File.Delete(NotifFile)

        For Each Notif As ViBENotification In AllNotifs
            AddToFile(NotifFile, Notif.ToString)
        Next
    End Sub

End Class

Public Class ViBENotification

    Public ReadOnly MyDate As String
    Public ReadOnly Message As String

    Public Sub New(NotifString As String)
        MyDate = NotifString.Split("`")(0)
        Message = NotifString.Split("`")(1)
    End Sub

    Public Sub New(MyDate As String, Message As String)
        Me.MyDate = MyDate
        Me.Message = Message
    End Sub

    Public Overrides Function ToString() As String
        Return String.Join("`", {MyDate, Message})
    End Function


End Class
