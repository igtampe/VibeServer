''' <summary>Interface that holds a SmokeSignal user </summary>
Public Interface ISmokeSignalUser

    ''' <summary>Gets the username of this user</summary>
    Function GetUsername() As String

    ''' <summary>Get the password of this user</summary>
    Function GetPassword() As String

    Function GetUserType() As ISmokeSignalUserType

End Interface
