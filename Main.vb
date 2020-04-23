Imports System.IO
Imports System.Net
Imports System.Net.Sockets

''' <summary>SMOKESIGNAL SERVER VERSION 7</summary>
Public Module Main

    'CONFIGURATION HAS MOVED! See Config.VB to make any changes to the configuration of your server.
    'Hopefully this will mean that if you ever need to update this server, all you have to do is update this file, and not have to worry about the configuration.

    ''' <summary>Main SmokeSignal Authenticator for this server</summary>
    Public Authenticator As ISmokeSignalAuthenticator

    ''' <summary>All Registered SmokeSignal Non-Authenticated Extensions</summary>
    Public Extensions As ISmokeSignalAuthenticatedExtension()

    ''' <summary>All Registered SmokeSignal Authenticated Extensions</summary>
    Public AuthenticatedExtensions As ISmokeSignalAuthenticatedExtension()

    ''' <summary>SmokeSignal Version</summary>
    Public Const SMOKESIGNAL_VERSION As String = "7.0"

    Public Sub Main()

        Console.SetWindowSize(120, 30)
        Console.SetBufferSize(120, 30)

        'Server Initialization
        Console.Title = SERVER_NAME & " [Version " & SERVER_VERSION & "]"
        ToConsole("Starting Server...")

        'Read settings
        If (File.Exists("SmokeSettings.cfg")) Then
            'Set Settings
            Dim Settings As String() = ReadFromFile("SmokeSettings.cfg").Split(",")
            IP = Settings(0)
            Port = Integer.Parse(Settings(1))
            FileClose(1)
        Else
            ToFile("SmokeSettings.cfg", IP & "," & Port)
            ToConsole("Could Not Find Settings.cfg in current directory, rendered default one", ConsoleColor.Yellow)
        End If

        'Register Global Variables
        RegisterGlobalVariables()

        'Registering Authenticator
        RegisterAuthenticator()
        ToConsole("Registered Authenticator " & Authenticator.GetName & " [Version " & Authenticator.GetVersion() & "] With " & Authenticator.GetAllUsers.Count & " User(s)", ConsoleColor.Cyan)

        'Extensions Registering
        RegisterAllExtensions()

        ToConsole("Registered " & Extensions.Count & " Extension(s): ", ConsoleColor.Blue)
        For Each SmokeSignal In Extensions
            ToConsole(" - " & SmokeSignal.GetName & " [Version " & SmokeSignal.GetVersion & "]", ConsoleColor.Blue)
        Next

        For Each SmokeSignal In AuthenticatedExtensions
            ToConsole(" - " & SmokeSignal.GetName & " [Version " & SmokeSignal.GetVersion & "] (Authenticated)", ConsoleColor.DarkBlue)
        Next

        'Actually start the server
        Dim tcpListener As TcpListener = New TcpListener(IPAddress.Parse(IP), Port)
        Dim tcpClient As TcpClient = New TcpClient()
        tcpListener.Start()

        ToConsole("Server Started!", ConsoleColor.Green)

        Dim ClientMSG As String
        ToConsole("Waiting for connection...", ConsoleColor.Yellow)
        DrawHeader()

        'The bulk loop
        While True
            Dim Wait As Boolean = True

            'Check if we have a pending connection
            If tcpListener.Pending Then
                ClearHeader()

                'Accept it...
                Dim theSocket As Socket = tcpListener.AcceptSocket
                Dim networkStream As NetworkStream = New NetworkStream(theSocket)
                Dim binaryWriter As BinaryWriter = New BinaryWriter(networkStream)
                Dim binaryReader As BinaryReader = New BinaryReader(networkStream)

                ToConsole("Connected from (" & (TryCast(theSocket.RemoteEndPoint, IPEndPoint).Address.ToString) & ")! Waiting for string...", ConsoleColor.Green)

                'Try to take the string, and parse it
                Try
                    ClientMSG = binaryReader.ReadString().Trim()
                    ToConsole("Received (" & ClientMSG & ")")
                    binaryWriter.Write(ParseCommand(ClientMSG))
                Catch ex As Exception
                    ErrorToConsole("Could not read string for some reason.", ex)
                End Try

                'Return to the waiting state
                ToConsole("Waiting for connection...", ConsoleColor.Yellow)
                DrawHeader()
                Wait = False
            End If

            'Tick each time we can.
            For Each SmokeSignal In Extensions
                SmokeSignal.Tick()
            Next

            'S P E E N
            Spinner(Console.CursorLeft, Console.CursorTop)

            'Wait for another go around
            If Wait Then Sleep(100)

        End While
    End Sub

    Public Sub DrawHeader()
        Box(HEADER_BACK_COLOR, 120, 2, 0, 0)
        SetPos(0, 0)
        Color(HEADER_BACK_COLOR, HEADER_FONT_COLOR)
        CenterText(SERVER_NAME + " [Version " & SERVER_VERSION & "] | Running on SmokeSignal V" & SMOKESIGNAL_VERSION)
        SetPos(0, 1)
        CenterText(Extensions.Count & " Extension(s) loaded | Listening on " & IP & ":" & Port & " ")
    End Sub

    Public Sub ClearHeader()
        Box(ConsoleColor.Black, 120, 2, 0, 0)
    End Sub

    Private Function ParseCommand(ClientMSG As String) As String

        'Just in case
        If String.IsNullOrWhiteSpace(ClientMSG) Then Return InvalidPacketSent()

        Dim Result As String

        'Attempt parsing with Non-Authenticated extensions
        For Each SmokeSignal As ISmokeSignalAuthenticatedExtension In Extensions
            Try
                Result = SmokeSignal.Parse(ClientMSG)
                If Not String.IsNullOrEmpty(Result) Then Return Result
            Catch ex As Exception
                ErrorToConsole(SmokeSignal.GetName & " suffered an uncontained exception when processing this command", ex)
            End Try
        Next

        'Ask the authenticator to parse this
        Result = Authenticator.Parse(ClientMSG)
        If Not String.IsNullOrEmpty(Result) Then Return Result

        'Split the client message, Should be USER|PASS|COMMAND. If there's less than 3 parts, it must be an invalid packet
        Dim ClientSplit As String() = ClientMSG.Split("|")
        If ClientSplit.Length < 3 Then Return InvalidPacketSent()

        'Attempt to authenticate the executing user. If we cannot, then there was an authentication error
        Dim ExecutingUser As ISmokeSignalUser = Authenticator.Authenticate(ClientSplit(0), ClientSplit(1))
        If IsNothing(ExecutingUser) Then Return AuthenticationError(ClientSplit(0))

        'Attempt to parse with authenticated extensions
        For Each AuthSmokeSignal As ISmokeSignalAuthenticatedExtension In AuthenticatedExtensions
            Try
                Result = AuthSmokeSignal.Parse(ExecutingUser, ClientSplit(2))
                If Not String.IsNullOrEmpty(Result) Then Return Result
            Catch ex As Exception
                ErrorToConsole(AuthSmokeSignal.GetName & " suffered an uncontained exception when processing this command", ex)
            End Try
        Next

        'Invalid Packet
        Return InvalidPacketSent()
    End Function

    Private Function AuthenticationError(Username As String) As String
        ToConsole("Authentication Failed for user " & Username, ConsoleColor.DarkRed)
        Return "AUTHERROR"

    End Function

    Private Function InvalidPacketSent() As String
        ToConsole("Invalid Packet Sent")
        Return "invalid Packet Sent"
    End Function

End Module