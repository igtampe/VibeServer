Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Imports Utils
Imports Core
Imports Notif
Imports Bank
Imports EzTax
Imports Checkbook
Imports Contractus

Public Module Main

    Public UMSWEBDir As String
    Public WEBDir As String
    Public IP As String

    Public Sub Main()
        Color(ConsoleColor.White)
        Console.WriteLine("Visual Basic Economy Server [Version 4.0]")
        Console.WriteLine("(c)2019 Igtampe, No Rights Reserved.")
        Color(ConsoleColor.Gray)
        Console.WriteLine("")
        Console.WriteLine("")
        ToConsole("Starting Server...")

        If (File.Exists("Settings1.cfg")) Then
            FileOpen(1, "Settings1.cfg", OpenMode.Input)
            Dim str15 As String = LineInput(1).Replace("""", "")
            Dim Settings As String() = str15.Split(",")
            IP = Settings(0)
            UMSWEBDir = Settings(1)
            WEBDir = Settings(2)
            FileClose(1)
        Else
            IP = "127.0.0.1"
            UMSWEBDir = "A:\MARSH"
            WEBDir = "A:\MARSH"
            FileOpen(1, "Settings1.cfg", OpenMode.Output)
            WriteLine(1, New Object() {IP, UMSWEBDir, WEBDir})
            FileClose(1)
            ToConsole("Could Not Find Settings.cfg in current directory, rendered default one")
        End If

        Dim tcpListener As TcpListener = New TcpListener(IPAddress.Parse(IP), 757)
        Dim tcpClient As TcpClient = New TcpClient()
        tcpListener.Start()

        Color(ConsoleColor.Green)
        ToConsole("Server Started!")
        Color(ConsoleColor.Gray)

        Dim ClientMSG As String = ""


        While True
            Color(ConsoleColor.Yellow)
            ToConsole("Waiting for connection...")

            Dim networkStream As NetworkStream = New NetworkStream(tcpListener.AcceptSocket())
            Dim binaryWriter As BinaryWriter = New BinaryWriter(networkStream)
            Dim binaryReader As BinaryReader = New BinaryReader(networkStream)

            Color(ConsoleColor.Green)
            ToConsole("Connected! Waiting for string...")
            Color(ConsoleColor.Gray)

            Try
                ClientMSG = binaryReader.ReadString().Trim()
                ToConsole("Received (" & ClientMSG & ")")
                binaryWriter.Write(ParseCommand(ClientMSG))
            Catch ex As Exception
                ErrorToConsole("Could not read string for some reason.", ex)
            End Try


        End While
    End Sub

    Function ParseCommand(ClientMSG As String) As String

        If (ClientMSG = "CONNECTED") Then
            'Ping
            ToConsole("Classic Packet, replied.")
            Return "You've connected to the server! Congrats."

        ElseIf (ClientMSG.StartsWith("CU")) Then
            'Check User
            Return CU(ClientMSG.Remove(0, 2))

        ElseIf (ClientMSG.StartsWith("SM")) Then
            'Send Money
            Return SM(ClientMSG.Remove(0, 2))

        ElseIf (ClientMSG.StartsWith("TM")) Then
            'Transfer Money
            Return TM(ClientMSG.Remove(0, 2))

        ElseIf (ClientMSG.StartsWith("CP")) Then
            'Change Pin
            Return ChangePin(ClientMSG.Remove(0, 2))

        ElseIf (ClientMSG.StartsWith("INFO")) Then
            'Client Information Request
            Return INFO(ClientMSG.Remove(0, 4))

        ElseIf ClientMSG.StartsWith("NOTIF") Then
            'Notification Request
            Return Notifications(ClientMSG.Replace("NOTIF", ""))

        ElseIf ClientMSG = "DIR" Then
            'Directory Request
            Return GetDirectory()

        ElseIf ClientMSG.StartsWith("REG") Then
            'User Registration Request
            Return RegisterUser(ClientMSG.Remove(0, 3))

        ElseIf ClientMSG.StartsWith("BNK") Then
            'Bank Tools
            Return BNK(ClientMSG.Remove(0, 3))

        ElseIf ClientMSG.StartsWith("CERT") Then
            'Certification System
            Return Certify(ClientMSG.Remove(0, 4))

        ElseIf ClientMSG.StartsWith("CHCKBK") Then
            'Checkbook 2000 Subsystem
            Return CHCKBK(ClientMSG.Replace("CHCKBK", ""))

        ElseIf ClientMSG.StartsWith("NTA") Then
            'Non-Taxed Add
            Return NonTaxAdd(ClientMSG.Replace("NTA", ""))

        ElseIf ClientMSG.StartsWith("EZT") Then
            'EzTax
            Return EZT(ClientMSG.Remove(0, 3))

        ElseIf ClientMSG.StartsWith("CON") Then
            'Contractus
            Return CON(ClientMSG.Replace("CON", ""))

        Else
            'Invalid Packet
            ToConsole("Invalid Packet Sent")
            Return "invalid Packet Sent"
        End If
    End Function


End Module