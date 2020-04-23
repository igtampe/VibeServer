Imports System.Collections

Public Interface ISmokeSignalAuthenticator

    ''' <summary>Authenticates the current user</summary>
    ''' <returns>True if the user is correct, false if otherwise.</returns>
    Function Authenticate(Username As String, Password As String) As ISmokeSignalUser

    ''' <summary>Get all users this authenticator has</summary>
    Function GetAllUsers() As ArrayList

    ''' <summary>Register a new user</summary>
    Function RegistierUser(Username As String, Password As String) As String

    ''' <summary>Parses an Authenticator Command</summary>
    Function Parse(ClientMSG As String) As String

    ''' <summary>Gets name of this Authenticator </summary>
    Function GetName() As String

    ''' <summary>Gets version of this authenticator</summary>
    Function GetVersion() As String

End Interface
