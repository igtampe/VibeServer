Imports System.Collections
Imports System.IO

Public Class DummyAuthenticator
    Implements ISmokeSignalAuthenticator

    Private Const AUTHENTICATOR_NAME = "Dummy Authenticator"
    Private Const AUTHENTICATOR_VERSION = "1.0"

    Private ReadOnly AllUsers As ArrayList
    Private ReadOnly UserFile As String

    Public ReadOnly Dummy As DummyUserType
    Public ReadOnly Anonymous As DummyUserType
    Public ReadOnly AnonymousUser As DummyUser


    Public Sub New(UserFile As String)

        Me.UserFile = UserFile

        AllUsers = New ArrayList
        Dummy = New DummyUserType("Dummy")
        Anonymous = New DummyUserType("Anonymous")
        AnonymousUser = New DummyUser("Anonymous", "", Anonymous)

        If File.Exists(UserFile) Then
            FileOpen(1, UserFile, OpenMode.Input)

            While Not EOF(1)
                Dim CurrentLine As String() = LineInput(1).Split(",")
                AllUsers.Add(New DummyUser(CurrentLine(0), CurrentLine(1), Dummy))
            End While

            FileClose(1)

        End If

    End Sub

    Public Function Authenticate(Username As String, Password As String) As ISmokeSignalUser Implements ISmokeSignalAuthenticator.Authenticate

        If Username = "Anonymous" Then Return AnonymousUser

        For Each User As DummyUser In AllUsers
            If User.GetUsername = Username And User.GetPassword = Password Then Return User
        Next

        Return Nothing
    End Function

    Public Function GetAllUsers() As ArrayList Implements ISmokeSignalAuthenticator.GetAllUsers
        Return AllUsers
    End Function

    Public Function RegisterUser(Username As String, Password As String) As String Implements ISmokeSignalAuthenticator.RegisterUser

        Dim NewUser As DummyUser = New DummyUser(Username, Password, Dummy)

        If AllUsers.Contains(NewUser) Then Return "E"

        AllUsers.Add(NewUser)
        SaveUsers()

        Return "OK"

    End Function

    Private Sub SaveUsers()

        FileOpen(1, UserFile, OpenMode.Output)

        For Each User As DummyUser In AllUsers
            PrintLine(1, User.ToString)
        Next

        FileClose(1)

    End Sub

    Public Sub UpdateUser(User As DummyUser)

        For X = 0 To AllUsers.Count - 1
            If AllUsers(X).Equals(User) Then
                AllUsers(X) = User
                SaveUsers()
                Return
            End If
        Next

    End Sub


    Public Function Parse(ClientMSG As String) As String Implements ISmokeSignalAuthenticator.Parse
        If Not ClientMSG.ToUpper.StartsWith("DUMMYAUTH") Then Return ""

        Dim DummyAuthCommand As String() = ClientMSG.Split("|")
        If DummyAuthCommand.Length < 2 Then Return ""

        Select Case DummyAuthCommand(1)
            Case "REG"
                Return RegisterUser(DummyAuthCommand(2), DummyAuthCommand(3))
        End Select

        Return ""
    End Function

    Public Function GetName() As String Implements ISmokeSignalAuthenticator.GetName
        Return AUTHENTICATOR_NAME
    End Function

    Public Function GetVersion() As String Implements ISmokeSignalAuthenticator.GetVersion
        Return AUTHENTICATOR_VERSION
    End Function
End Class

Public Class DummyUser
    Implements ISmokeSignalUser

    Private ReadOnly Username As String
    Private ReadOnly Password As String
    Private ReadOnly Type As ISmokeSignalUserType

    Public Sub New(Username As String, Password As String, Type As ISmokeSignalUserType)
        Me.Username = Username
        Me.Password = Password
        Me.Type = Type
    End Sub

    Public Function GetUsername() As String Implements ISmokeSignalUser.GetUsername
        Return Username
    End Function

    Public Function GetPassword() As String Implements ISmokeSignalUser.GetPassword
        Return Password
    End Function

    Public Function GetUserType() As ISmokeSignalUserType Implements ISmokeSignalUser.GetUserType
        Return Type
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim OtherUser As DummyUser = TryCast(obj, DummyUser)
        If IsNothing(OtherUser) Then Return Nothing

        Return Username = OtherUser.GetUsername

    End Function

    Public Overrides Function ToString() As String
        Return Username & "," & Password
    End Function

End Class

Public Class DummyUserType
    Implements ISmokeSignalUserType

    Private ReadOnly Name As String

    Public Sub New(Name As String)
        Me.Name = Name
    End Sub

    Public Function GetName() As String Implements ISmokeSignalUserType.GetName
        Return Name
    End Function
End Class
