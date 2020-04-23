''' <summary>An interface to develop extensions </summary>
Public Interface ISmokeSignalAuthenticatedExtension
    Inherits ISmokeSignalAuthenticatedExtension

    ''' <summary>Returns the response to the specified parsable command.</summary>
    ''' <param name="Command"></param>
    ''' <returns>An actual string if it could parse it, otherwise null</returns>
    Overloads Function Parse(User As ISmokeSignalUser, Command As String) As String

End Interface
