Imports System.IO

Public Class ViBEUser
    Implements ISmokeSignalUser

    ''' <summary>This User's UMSWEB ID</summary>
    Public ReadOnly ID As String

    ''' <summary>This User's Username</summary>
    Public ReadOnly Username As String

    Private Pin As String

    ''' <summary>This user's directory (ends in \)</summary>
    Public ReadOnly Directory As String

    Public ReadOnly UMSNB As ViBEBank
    Public ReadOnly GBANK As ViBEBank
    Public ReadOnly RIVER As ViBEBank

    Public ReadOnly NotifHandler As ViBENotificationHandler
    Public ReadOnly Checkbook As ViBECheckbook

    Private UserType As ViBEUserType

    Public Sub New(ID As String, Category As Integer)
        Me.ID = ID

        Directory = UMSWEB_DIR & "\ssh\users\" & ID & "\"

        Username = ReadFromFile(Directory & "Name.dll")
        Pin = ReadFromFile(Directory & "Pin.dll")

        Dim Auth As Integer = 0

        If File.Exists(Directory & "Authority.dll") Then
            Auth = ReadFromFile(Directory & "Authority.dll")
        End If

        UserType = New ViBEUserType(Category, Auth)

        UMSNB = New ViBEBank("UMSNB", Me)
        GBANK = New ViBEBank("GBANK", Me)
        RIVER = New ViBEBank("RIVER", Me)

        NotifHandler = New ViBENotificationHandler(Me)
        Checkbook = New ViBECheckbook(Me)

    End Sub

    Public Function GetUsername() As String Implements ISmokeSignalUser.GetUsername
        Return Username
    End Function

    Public Function CheckPin(Pin As String) As Boolean
        Return Me.Pin = Pin
    End Function

    Public Sub ChangePin(NewPin As String)
        Pin = NewPin
        ToFile(Directory & "Pin.dll", NewPin)
    End Sub

    Public Function GetPassword() As String Implements ISmokeSignalUser.GetPassword
        Throw New NotSupportedException
    End Function

    Public Function GetUserType() As ISmokeSignalUserType Implements ISmokeSignalUser.GetUserType
        Return UserType
    End Function

    Public Overrides Function ToString() As String
        Return Username & " (" & ID & ")"
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim OtherUser As ViBEUser = TryCast(obj, ViBEUser)
        If IsNothing(OtherUser) Then Return False
        Return OtherUser.ID = ID
    End Function

    Public Function ToDirectoryString()
        Dim RString As String = ID & ": " & Username

        If UserType.Category = ViBEUserType.ViBECategory.Corporate Then RString &= " (Corp.)"
        If UserType.Category = ViBEUserType.ViBECategory.Government Then RString &= " (Gov.)"

        Return ID & ": " & Username

    End Function

    Public Function Info()
        Return String.Join(",", {UMSNB.IsOpen, UMSNB.GetBalance, GBANK.IsOpen, GBANK.GetBalance, RIVER.IsOpen, RIVER.GetBalance, Username, NotifHandler.AllNotifs.Count})
    End Function

    Public Sub AddEI(Amount As Long)
        Dim EI As Long = Amount
        If File.Exists(Directory & "EI.dll") Then EI += ReadFromFile(Directory & "EI.dll")
        ToFile(Directory & "EI.dll", EI)
    End Sub

End Class

Public Class ViBEUserType
    Implements ISmokeSignalUserType

    Public ReadOnly Category As ViBECategory
    Public ReadOnly AuthorityLevel As Integer

    Public Enum ViBECategory
        Normal = 0
        Corporate = 1
        Government = 2
    End Enum

    Public Sub New(Category As ViBECategory, AuthLevel As Integer)
        Me.Category = Category
        AuthorityLevel = AuthLevel
    End Sub

    Public Function GetName() As String Implements ISmokeSignalUserType.GetName
        Return Category
    End Function

End Class