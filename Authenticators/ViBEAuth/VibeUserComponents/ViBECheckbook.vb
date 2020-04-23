Imports System.Collections
Imports System.IO

Public Class ViBECheckbook

    Public ReadOnly AllItems As ArrayList
    Public ReadOnly TiedUser As ViBEUser
    Public ReadOnly CheckFile As String

    Public Sub New(ByRef TiedUser As ViBEUser)

        CheckFile = TiedUser.Directory & "chckbk.txt"

        AllItems = New ArrayList

        FileOpen(1, CheckFile, OpenMode.Input)

        While Not EOF(1)
            AllItems.Add(New ViBECheckbookItem(LineInput(1)))
        End While

    End Sub

    Public Sub AddItem(Message As String)
        ToConsole("Adding Item '" & Message & "'")
        AllItems.Add(New ViBECheckbookItem(Message))
        SaveItems()
    End Sub

    Public Sub RemoveItems(Index As Integer)
        ToConsole("Removing Item " & Index)
        AllItems.RemoveAt(Index)
        SaveItems()
    End Sub

    Public Sub ExecuteItem(Index As Integer, CurrentUserBankString As String)

        Dim CurrentItem As ViBECheckbookItem = AllItems(Index)
        Dim OtherUser As ViBEUser
        Dim OtherUserID As String = CurrentItem.From.Split("\")(0)
        Dim OtherUserBankString As String = CurrentItem.From.Split("\")(1)

        For Each user As ViBEUser In VAuthenticator.AllUsers
            If user.ID = OtherUserID Then OtherUser = user
        Next

        If IsNothing(OtherUser) Then Throw New Exception("Could not find other user")

        Dim CurrentUserBank As ViBEBank
        Dim OtherUserBank As ViBEBank

        'Get the banks for both parties
        Select Case CurrentUserBankString
            Case "UMSNB"
                CurrentUserBank = TiedUser.UMSNB
            Case "GBANK"
                CurrentUserBank = TiedUser.GBANK
            Case "RIVER"
                CurrentUserBank = TiedUser.RIVER
            Case Else
                Throw New ArgumentException("Invalid current user bank string")
        End Select

        Select Case OtherUserBankString
            Case "UMSNB"
                OtherUserBank = OtherUser.UMSNB
            Case "GBANK"
                OtherUserBank = OtherUser.GBANK
            Case "RIVER"
                OtherUserBank = OtherUser.RIVER
            Case Else
                Throw New ArgumentException("Invalid other user bank string")
        End Select

        If CurrentItem.Type = ViBECheckbookItem.ItemType.Check Then
            OtherUserBank.SendMoney(CurrentUserBank, CurrentItem.Amount)
        Else
            CurrentUserBank.SendMoney(OtherUserBank, CurrentItem.Amount)
        End If

        'Remove the tiem
        RemoveItems(Index)

    End Sub


    Public Function GetChecks() As String
        Return String.Join("`", AllItems.ToArray)
    End Function

    Public Sub SaveItems()
        ToConsole("Saving Notifications")
        If File.Exists(CheckFile) Then File.Delete(CheckFile)

        For Each Item As ViBECheckbookItem In AllItems
            AddToFile(CheckFile, Item.ToString)
        Next
    End Sub



End Class

Public Class ViBECheckbookItem

    '0`11/2/2019 12:03:42 AM`The UMS National Bank`00000\UMSNB`5966906`::1::Interest Check

    Public Enum ItemType
        Check = 0
        Bill = 1
    End Enum

    Public ReadOnly Type As ItemType
    Public ReadOnly MyDate As String
    Public ReadOnly Name As String
    Public ReadOnly From As String
    Public ReadOnly Amount As Long
    Public ReadOnly Comment As String

    Public Sub New(CheckbookItemString As String)
        Dim Split As String() = CheckbookItemString.Split("`")

        Type = CInt(Split(0))
        MyDate = Split(1)
        Name = Split(2)
        From = Split(3)
        Amount = Split(4)
        Comment = Split(5)

    End Sub

    Public Sub New(Type As ItemType, Mydate As String, Name As String, From As String, Amount As Long, Comment As String)
        Me.Type = Type
        Me.MyDate = Mydate
        Me.Name = Name
        Me.From = From
        Me.Amount = Amount
        Me.Comment = Comment
    End Sub

    Public Overrides Function ToString() As String
        Return String.Join("`", {Type, MyDate, Name, From, Amount, Comment})
    End Function


End Class
