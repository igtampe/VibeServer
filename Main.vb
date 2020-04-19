Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Imports BasicRender
Imports Utils

''' <summary>
''' SMOKESIGNAL SERVER VERSION 6.0.1
''' 
''' Modified for ViBE Server compatibility. It really is quite the bodge but hey at least we're now on SmokeSignal V6
''' </summary>
Public Module Main

    Public IP As String = "127.0.0.1"
    Public Port As Integer = 757
    Public UMSWEBDir As String = "."
    Public WEBDir As String = "."
    Public Extensions(0) As SmokeSignalExtension
    'I would make that an arraylist to be simple pero no puedo sadly. Oh well.

    'SERVER SETUP
    Public Const SERVER_NAME As String = "ViBE Server"
    Public Const SERVER_VERSION As String = "6.0"
    Public Const HEADER_BACK_COLOR As ConsoleColor = ConsoleColor.DarkCyan
    Public Const HEADER_FONT_COLOR As ConsoleColor = ConsoleColor.White

    '(pls do not touch me)
    Public Const SMOKESIGNAL_VERSION As String = "6.1"

    Public Sub RegisterAllExtensions()
        ReDim Extensions(3) 'Redim the Extensions array to the size of the number of extensions you want.

        'Add your extensions. When creating the extension, the extension should initialize
        Extensions(0) = New DummyExtension()
        Extensions(1) = New ViBEExtension()
        Extensions(2) = New LBL()
        Extensions(3) = New IMEX()
    End Sub

    Public Sub Main()

        'Console Size (Remember to update on BasicRender if u want to change this)
        Console.SetWindowSize(120, 30)
        Console.SetBufferSize(120, 30)

        'Server Initialization
        Console.Title = SERVER_NAME & " [Version " & SERVER_VERSION & "]"
        ToConsole("Starting Server...")

        'Read settings
        If (File.Exists("Settings1.cfg")) Then
            'Set Settings
            Dim Settings As String() = ReadFromFile("Settings1.cfg").Split(",")
            IP = Settings(0)
            UMSWEBDir = Settings(1)
            WEBDir = Settings(2)
            FileClose(1)
        Else
            ToFile("Settings1.cfg", IP & "," & UMSWEBDir & "," & WEBDir)
            ToConsole("Could Not Find Settings.cfg in current directory, rendered default one", ConsoleColor.Yellow)
        End If

        'Extensions Registering
        RegisterAllExtensions()

        ToConsole("Registered " & Extensions.Length & " Extension(s): ", ConsoleColor.Blue)
        For Each SmokeSignal In Extensions
            ToConsole(" - " & SmokeSignal.getName & " [Version " & SmokeSignal.getVersion & "]", ConsoleColor.Blue)
        Next

        'Actually start the server
        Dim tcpListener As TcpListener = New TcpListener(IPAddress.Parse(IP), Port)
        Dim tcpClient As TcpClient = New TcpClient()
        tcpListener.Start()

        ToConsole("Server Started!", ConsoleColor.Green)

        Dim ClientMSG As String
        ToConsole("Waiting for connection...", ConsoleColor.Yellow)
        DrawHeader()
        Dim Wait As Boolean = True

        'The bulk loop
        While True
            Wait = True
            'Check if we have a pending connection
            If tcpListener.Pending Then
                ClearHeader()

                'Accept it...
                Dim networkStream As NetworkStream = New NetworkStream(tcpListener.AcceptSocket())
                Dim binaryWriter As BinaryWriter = New BinaryWriter(networkStream)
                Dim binaryReader As BinaryReader = New BinaryReader(networkStream)

                ToConsole("Connected! Waiting for string...", ConsoleColor.Green)

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
        CenterText(Extensions.Length & " Extension(s) loaded | Listening on " & IP & ":" & Port & " ")
    End Sub

    Public Sub ClearHeader()
        Box(ConsoleColor.Black, 120, 2, 0, 0)
    End Sub

    Function ParseCommand(ClientMSG As String) As String
        Dim Result As String
        For Each SmokeSignal In Extensions
            Result = SmokeSignal.Parse(ClientMSG)
            If Not String.IsNullOrEmpty(Result) Then Return Result
        Next

        'Invalid Packet
        ToConsole("Invalid Packet Sent")
        Return "invalid Packet Sent"
    End Function


End Module