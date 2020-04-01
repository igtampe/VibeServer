Imports Utils
Imports System.IO
Imports System.Collections

Public Class LBL
    Implements SmokeSignalExtension

    ''' <summary>
    ''' The name of the extension
    ''' </summary>
    Public Const EXTENSION_NAME = "LBL File Transfer"
    Public Const EXTENSION_VERS = "1.0"

    ''' <summary>
    ''' Directory on disk to save the received files
    ''' </summary>
    Public WebDirectory As String = "."

    ''' <summary>
    ''' Directory on the web to see received files
    ''' </summary>
    Public WebPrefix As String = "localhost"

    Private ActiveTransfers As ArrayList = New ArrayList()

    Public Sub New()
        If File.Exists("LBL.CFG") Then
            Dim Config As String() = ReadFromFile("LBL.CFG").Split(",")
            WebDirectory = Config(0)
        Else
            ToConsole("Unable to find LBL Config File, Made a new one.")
            ToFile("LBL.CFG", WebDirectory & "," & WebPrefix)
        End If
    End Sub

    Public Sub Tick() Implements SmokeSignalExtension.Tick
        'do nada
    End Sub

    Public Function Parse(Command As String) As String Implements SmokeSignalExtension.Parse
        'LBL:START:57174.IncomeRegistry.CSV
        Try
            If Command.StartsWith("LBL:") Then
                ToConsole("LBL has been invoked", ConsoleColor.DarkGreen)
                Dim LBLCommand() As String = Command.Split(":")

                If LBLCommand.Count < 3 Then
                    ToConsole("Invalid command sent", ConsoleColor.DarkRed)
                    Return "E"
                End If

                Select Case LBLCommand(1).ToUpper
                    Case "OPEN"
                        Return Open(LBLCommand(2))
                    Case "TRANSFER"
                        If LBLCommand.Count = 4 Then
                            Return Transfer(LBLCommand(2), LBLCommand(3))
                        Else
                            ToConsole("Invalid transfer command sent", ConsoleColor.DarkRed)
                            Return "E"
                        End If
                    Case "CLOSE"
                        Return Close(LBLCommand(2))
                    Case Else
                        Return "E"
                End Select
            Else
                Return ""
            End If
        Catch ex As Exception
            ErrorToConsole("LBL Suffered an unknown error", ex)
            Return "E"
        End Try
    End Function

    Private Function Open(Filename As String) As String
        'Start a transfer
        ToConsole("Attempting to start a transfer for file" & Filename & "...")

        Dim NFTID = ""

        Do
            'Generate a transfer ID
            NFTID = ""
            NFTID &= CInt(Math.Ceiling(Rnd() * 9))
            NFTID &= CInt(Math.Ceiling(Rnd() * 9))
            NFTID &= CInt(Math.Ceiling(Rnd() * 9))
            NFTID &= CInt(Math.Ceiling(Rnd() * 9))
            NFTID &= CInt(Math.Ceiling(Rnd() * 9))

            'Check that it isn't in use
        Loop While Not IsNothing(GetTransfer(NFTID))

        ToConsole("Attempting to initialize transfer " & NFTID)

        Try
            ActiveTransfers.Add(New LBLTransfer(NFTID, Filename, WebDirectory))
            ToConsole("Success! Transfer " & NFTID & " of file " & Filename & " has been initiaized. Available to receive data.", ConsoleColor.Green)
            Return NFTID
        Catch ex As Exception
            ErrorToConsole("Unable to add the new transfer to the list of active transfers", ex)
            Return "E"
        End Try

    End Function

    Private Function Transfer(TransferID As Integer, TransferData As String)
        Dim TheTransfer As LBLTransfer = GetTransfer(TransferID)

        If Not IsNothing(TheTransfer) Then
            Try
                TheTransfer.Append(TransferData)
                ToConsole("Successfully appended the received data to Transfer " & TransferID, ConsoleColor.Green)
                Return "OK"
            Catch ex As Exception
                ErrorToConsole("Unable to append data to the Transfer's file", ex)
            End Try
        Else
            ToConsole("Could not find Transfer " & TransferID, ConsoleColor.DarkRed)
            Return "E"
        End If

    End Function

    Private Function Close(TransferID As Integer)
        Dim TheTransfer As LBLTransfer = GetTransfer(TransferID)
        If Not IsNothing(TheTransfer) Then
            Try
                ToConsole("Closing off File Transfer" & TransferID)
                Dim filename As String = TheTransfer.Filename
                'TheTransfer.Close()
                ActiveTransfers.Remove(TheTransfer)
                Return WebPrefix & "/" & filename
            Catch ex As Exception
                ErrorToConsole("Error Closing file transfer " & TransferID, ex)
                Return "E"
            End Try
        Else
            ToConsole("Could not find transfer " & TransferID & ". Perhaps it has already been closed?", ConsoleColor.DarkRed)
            Return "E"
        End If
    End Function

    Private Function GetTransfer(TransferID As Integer)
        ToConsole("Attempting to find transfer " & TransferID)
        For Each LookingTransfer As LBLTransfer In ActiveTransfers
            If LookingTransfer.ID = TransferID Then Return LookingTransfer
        Next
        Return Nothing
    End Function

    Public Function getName() As String Implements SmokeSignalExtension.getName
        Return EXTENSION_NAME
    End Function

    Public Function getVersion() As String Implements SmokeSignalExtension.getVersion
        Return EXTENSION_VERS
    End Function

End Class
