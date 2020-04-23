Imports System.Collections

Public Class ViBEAuthenticator
    Implements ISmokeSignalAuthenticator

    Private Const AUTHENTICATOR_NAME = "ViBE Authenticator and User System"
    Private Const AUTHENTICATOR_VERSION = "1.0"

    Public ReadOnly AllUsers As ArrayList

    Public Sub New()

        AllUsers = New ArrayList

        LoadUsers(UMSWEB_DIR & "\ssh\incomeman\UserList.isf", 0)
        LoadUsers(UMSWEB_DIR & "\ssh\incomeman\Corporate.isf", 1)
        LoadUsers(UMSWEB_DIR & "\ssh\incomeman\NonTaxed.isf", 2)

    End Sub

    Public Sub ReloadAllUsers()
        AllUsers.Clear()

        LoadUsers(UMSWEB_DIR & "\ssh\incomeman\UserList.isf", 0)
        LoadUsers(UMSWEB_DIR & "\ssh\incomeman\Corporate.isf", 1)
        LoadUsers(UMSWEB_DIR & "\ssh\incomeman\NonTaxed.isf", 2)
    End Sub

    Public Sub UpdateUser(User As ViBEUser)

        For X = 0 To AllUsers.Count - 1
            'Equals only checks ID, so as long as we don't touch that, we can search for the user, and then update him or her.
            If AllUsers(X).Equals(User) Then AllUsers(X) = User
        Next

    End Sub

    ''' <summary>Load users into the allusers arraylist</summary>
    Private Sub LoadUsers(ISFDir As String, Category As Integer)

        'OPEN THE ISF
        Try
            FileOpen(1, ISFDir, OpenMode.Input)
        Catch ex As Exception
            ErrorToConsole(ex.Message, ex)
        End Try

        Dim TempStringHolder As String
        Dim Counter As Integer = 1

        'READ THE FILE
        While Not EOF(1)
            TempStringHolder = LineInput(1)
            If (TempStringHolder.StartsWith("USER")) Then

                'ADD THE USER
                AllUsers.Add(New ViBEUser(TempStringHolder.Replace("USER" & Counter & ":", ""), Category))
                Counter += 1

            End If
        End While
        FileClose(1)

    End Sub

    Public Function Authenticate(Username As String, Password As String) As ISmokeSignalUser Implements ISmokeSignalAuthenticator.Authenticate
        For Each User As ViBEUser In AllUsers
            If User.ID = Username And User.CheckPin(Password) Then Return User
        Next

        Return Nothing
    End Function

    Public Function GetAllUsers() As ArrayList Implements ISmokeSignalAuthenticator.GetAllUsers
        Return AllUsers
    End Function

    Public Function RegisterUser(Username As String, Password As String) As String Implements ISmokeSignalAuthenticator.RegisterUser

        Try
            ToConsole("Attempting to add(" & Username & ") with pin (" & Password & ")")

            Dim regid As String

RedoIDGEN:
            regid = ""
            regid &= CInt(Math.Ceiling(Rnd() * 9))
            regid &= CInt(Math.Ceiling(Rnd() * 9))
            regid &= CInt(Math.Ceiling(Rnd() * 9))
            regid &= CInt(Math.Ceiling(Rnd() * 9))
            regid &= CInt(Math.Ceiling(Rnd() * 9))

            ToConsole("Got ID (" & regid & ") Checking if it exists...")
            If IO.Directory.Exists(UMSWEB_DIR & "\SSH\USERS\" & regid) Then GoTo RedoIDGEN

            ToConsole("Good it doesn't lets keep going. Creating the folder.")

            IO.Directory.CreateDirectory(UMSWEB_DIR & "\SSH\USERS\" & regid)
            ToConsole("Creating Name File")
            ToFile(UMSWEB_DIR & "\SSH\USERS\" & regid & "\NAME.dll", Username)
            ToConsole("Creating PIN File")
            ToFile(UMSWEB_DIR & "\SSH\USERS\" & regid & "\PIN.dll", Password)

            Dim TempString As String
            Dim ISFDir As String
            Dim Category As Integer

            If Username.EndsWith(" (Corp.)") Then
                Category = 1
                ISFDir = UMSWEB_DIR & "\SSH\INCOMEMAN\Corporate.isf"
            Else
                Category = 0
                ISFDir = UMSWEB_DIR & "\SSH\INCOMEMAN\UserList.isf"
            End If

            ToConsole("Attempting to add him to " & ISFDir)
            FileOpen(1, ISFDir, OpenMode.Input)

            'Find Position
            Dim N As Integer = 1
            While Not EOF(1)
                TempString = LineInput(1)
                If (TempString.StartsWith("USER")) Then
                    ToConsole("Found record (" & N & ") which is (" & TempString & ")")
                    N += 1
                End If
            End While

            'Add to Directory
            FileClose(1)
            ToConsole("Adding as record " & N)
            AddToFile(ISFDir, "USER" & N & ":" & regid)

            'Add to the AllUsers Directory file
            AllUsers.Add(New ViBEUser(regid, Category))

            Return regid

        Catch ex As Exception
            ErrorToConsole("Shoot I couldn't do that.", ex)
            Return "E"
        End Try

    End Function

    Public Function Parse(ClientMSG As String) As String Implements ISmokeSignalAuthenticator.Parse
        'REG4640,Igtampe

        If ClientMSG.StartsWith("REG") Then
            Dim SplitValues() As String = ClientMSG.Substring(3).Split(",")
            Return RegisterUser(SplitValues(1), SplitValues(0))
        ElseIf ClientMSG.StartsWith("CU") Then

            Dim CUCommand = ClientMSG.Substring(2)
            Dim CUUser As String
            Dim CUPin As String

            Try
                CUUser = CUCommand.Remove(5, 4)
                CUPin = CUCommand.Remove(0, 5)
            Catch exception As Exception
                ToConsole("Improperly Codded CheckUser Request")
                Return 1
            End Try

            Dim User As ViBEUser = Authenticate(CUUser, CUPin)

            If IsNothing(User) Then Return 1 Else Return 3

        End If

        Return ""

    End Function

    Public Function GetName() As String Implements ISmokeSignalAuthenticator.GetName
        Return AUTHENTICATOR_NAME
    End Function

    Public Function GetVersion() As String Implements ISmokeSignalAuthenticator.GetVersion
        Return AUTHENTICATOR_VERSION
    End Function
End Class
