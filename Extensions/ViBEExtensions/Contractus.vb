Imports Utils
Imports System.IO

''' <summary>
''' Contractus Expansion
''' </summary>
Public Class Contractus

    ''' <summary>
    ''' Executes Contractus Functions
    ''' </summary>
    ''' <param name="ConMSG"></param>
    ''' <returns></returns>
    Public Shared Function CON(ConMSG As String) As String
        ToConsole("Invoking Contractus Subsystem...")
        Try

            If ConMSG.StartsWith("READALL") Then
                'Read All Contracts
                Return ReadAllContracts()

            ElseIf ConMSG.StartsWith("DETAILS") Then
                'Details
                Return ConDetails(ConMSG.Replace("DETAILS", ""))

            ElseIf ConMSG.StartsWith("READUSR") Then
                'Read User Contracts
                Return ReadUserContracts(ConMSG.Replace("READUSR", ""))

            ElseIf ConMSG.StartsWith("ADDTOALL") Then
                'Add Contract to all contracts
                Return AddContractToAll(ConMSG.Replace("ADDTOALL", ""))

            ElseIf ConMSG.StartsWith("ADDBID") Then
                'Add Bid
                Return AddBid(ConMSG.Replace("ADDBID", ""))

            ElseIf ConMSG.StartsWith("MOVETOUSER") Then
                'Move Contract
                Return MoveToUser(ConMSG.Replace("MOVETOUSER", "").Split(";"))

            ElseIf ConMSG.StartsWith("REMOVE") Then
                'Remove a contract
                Return RemoveContract(ConMSG.Replace("REMOVE", "").Split(";"))

            Else
                ToConsole("Unable to parse Contractus Command")
                Return "E"
            End If

        Catch ex As Exception
            ErrorToConsole("The Contractus subsystem crashed.", ex)
            Return "E"
        End Try

    End Function

    ''' <summary>
    ''' Returns all contracts
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function ReadAllContracts() As String
        Dim AllContracts(0) As String
        ToConsole("trying to read all contracts")
        If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\Contracts.txt")) Then
            Return "N"
        End If

        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Contracts.txt"), OpenMode.Input)
        Dim I As Integer = 0
        While Not EOF(1)
            ReDim Preserve AllContracts(I)
            AllContracts(I) = LineInput(1)
            ToConsole("Found Contract " & I)
            I += 1
        End While
        FileClose(1)
        If I = 0 Then
            Return "N"
        Else
            Return String.Join(";", AllContracts)
        End If
    End Function

    ''' <summary>
    ''' Returns a contract's details
    ''' </summary>
    ''' <param name="Details"></param>
    ''' <returns></returns>
    Private Shared Function ConDetails(Details As String) As String
        ToConsole("Retrieving details from contract #" & Details)
        If Not File.Exists(UMSWEBDir & "\SSH\CONTRACTS\" & Details & ".txt") Then
            Return "E"
            ToConsole("Looks like it doesn't exist")
        End If

        Try
            Return ReadFromFile(UMSWEBDir & "\SSH\CONTRACTS\" & Details & ".txt")
        Catch ex As Exception
            Return "E"
            ErrorToConsole("w o o p s", ex)
        End Try

    End Function

    ''' <summary>
    ''' Returns a User's Contracts
    ''' </summary>
    ''' <param name="Notifmsg"></param>
    ''' <returns></returns>
    Private Shared Function ReadUserContracts(Notifmsg As String) As String
        Dim notifarray(0) As String
        ToConsole("trying to READ from " & Notifmsg & "'s Contracts")
        If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", Notifmsg, "\Contracts.txt")) Then
            Return "N"
        End If

        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", Notifmsg, "\Contracts.txt"), OpenMode.Input)
        Dim I As Integer = 0
        While Not EOF(1)
            ReDim Preserve notifarray(I)
            notifarray(I) = LineInput(1)
            ToConsole("Found Contract " & I)
            I += 1
        End While

        FileClose(1)
        If I = 0 Then
            Return "N"
        Else
            Return String.Join(";", notifarray)
        End If
    End Function

    ''' <summary>
    ''' Add Contract to all contracts
    ''' </summary>
    ''' <param name="ConMSG"></param>
    ''' <returns></returns>
    Private Shared Function AddContractToAll(ConMSG As String) As String
        ToConsole("Attempting to add " & ConMSG & "to all contracts")
        'Build The Building~57174~Igtampe;Build the Building and make it real good boio pls help
        Dim ConMSGSplit() As String
        ConMSGSplit = ConMSG.Split(";")
        Dim ContractID As Integer
        ContractID = 0

        While (File.Exists(UMSWEBDir & "\SSH\Contracts\" & ContractID & ".txt"))
            ContractID += 1
        End While

        ToConsole("This contract will be Contract #" & ContractID)

        AddToFile(String.Concat(UMSWEBDir, "\SSH\Contracts.txt"), ContractID & "~" & ConMSGSplit(0) & "~-1~Uninitialized~Uninitialized")
        ToFile(UMSWEBDir & "\SSH\Contracts\" & ContractID & ".txt", ConMSGSplit(1))

        ToConsole("OK Done")
        Return "S"
    End Function

    ''' <summary>
    ''' Adds a bid to a specified contract
    ''' </summary>
    ''' <param name="ConMSG"></param>
    ''' <returns></returns>
    Private Shared Function AddBid(ConMSG As String) As String
        Dim ConMSGSplit() As String
        'ContractID;NewBid;UserID;UserName
        ' 0           1      2        3
        ConMSGSplit = ConMSG.Split(";")
        ToConsole("Oh fuck time to add a bid AAAAAAAAAAAAAA")

        If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\Contracts.txt")) Then
            Return "E"
        End If

        ToConsole("Trying to Find Contract #" & ConMSGSplit(0))

        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Contracts.txt"), OpenMode.Input)
        FileOpen(2, String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"), OpenMode.Output)
        Dim CurrentLine() As String
        Dim woops As Boolean = True
        While Not EOF(1)
            CurrentLine = LineInput(1).Split("~")
            'ID~Name~FromID~FromNAME~TopBid~TopBidID~TopBidNAME
            ' 0  1      2     3        4        5          6

            If CurrentLine(0) = ConMSGSplit(0) Then
                woops = False
                ToConsole("Found it")
                If ConMSGSplit(1) < CurrentLine(4) Or CurrentLine(4) = -1 Then

                    CurrentLine(4) = ConMSGSplit(1)
                    CurrentLine(5) = ConMSGSplit(2)
                    CurrentLine(6) = ConMSGSplit(3)

                Else
                    ToConsole("Looks like this bid isn't less, so I cannot do.")
                    woops = True
                End If
            End If

            PrintLine(2, String.Join("~", CurrentLine))

        End While

        FileClose(1)
        FileClose(2)

        If woops Then
            File.Delete(String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"))
            Return "E"
        Else
            File.Delete(String.Concat(UMSWEBDir, "\SSH\Contracts.txt"))
            File.Move(String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"), String.Concat(UMSWEBDir, "\SSH\Contracts.txt"))
            Return "S"
        End If


    End Function

    ''' <summary>
    ''' Moves the contract
    ''' </summary>
    ''' <param name="ConMSGSplit"></param>
    ''' <returns></returns>
    Private Shared Function MoveToUser(ConMSGSplit() As String) As String
        'ContractID;User
        ' 0           1
        ToConsole("Oh fuck time to add a bid AAAAAAAAAAAAAA")

        If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\Contracts.txt")) Then
            Return "E"
        End If

        ToConsole("Trying to Find Contract #" & ConMSGSplit(0))

        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Contracts.txt"), OpenMode.Input)
        FileOpen(2, String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"), OpenMode.Output)
        Dim CurrentLine() As String
        Dim TransferedContract(0) As String
        Dim woops As Boolean = True
        While Not EOF(1)
            CurrentLine = LineInput(1).Split("~")
            'ID~Name~FromID~FromNAME~TopBid~TopBidID~TopBidNAME
            ' 0  1      2     3        4        5          6

            If CurrentLine(0) = ConMSGSplit(0) Then
                woops = False
                ToConsole("Found it")
                TransferedContract = CurrentLine
            Else
                PrintLine(2, String.Join("~", CurrentLine))
            End If
        End While
        FileClose(1)
        FileClose(2)

        If woops Then
            File.Delete(String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"))
            Return "E"
        Else
            File.Delete(String.Concat(UMSWEBDir, "\SSH\Contracts.txt"))
            File.Move(String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"), String.Concat(UMSWEBDir, "\SSH\Contracts.txt"))

            ToConsole("OK now that we've removed it its time to add it to the user's coso")
            If File.Exists(String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt")) Then
                FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"), OpenMode.Append)
            Else
                FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"), OpenMode.Output)
            End If

            PrintLine(1, String.Join("~", TransferedContract))
            FileClose(1)
            Return "S"
        End If

    End Function

    ''' <summary>
    ''' Removes a contract
    ''' </summary>
    ''' <param name="ConMSGSplit"></param>
    ''' <returns></returns>
    Private Shared Function RemoveContract(ConMSGSplit() As String) As String
        'ContractID;User
        ' 0           1
        ToConsole("Oh fuck time to add a bid AAAAAAAAAAAAAA")

        If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt")) Then
            Return "E"
        End If

        ToConsole("Trying to Find Contract #" & ConMSGSplit(0))

        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"), OpenMode.Input)
        FileOpen(2, String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\TempContracts.txt"), OpenMode.Output)
        Dim CurrentLine() As String
        Dim woops As Boolean = True
        While Not EOF(1)
            CurrentLine = LineInput(1).Split("~")
            'ID~Name~FromID~FromNAME~TopBid~TopBidID~TopBidNAME
            ' 0  1      2     3        4        5          6

            If CurrentLine(0) = ConMSGSplit(0) Then
                woops = False
                ToConsole("Found it, not copying it")
            Else
                PrintLine(2, String.Join("~", CurrentLine))
            End If
        End While
        FileClose(1)
        FileClose(2)

        If woops Then
            File.Delete(String.Concat(UMSWEBDir, "\SSH\Users" & ConMSGSplit(1) & "\TempContracts.txt"))
            Return "E"
        Else
            File.Delete(String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"))
            File.Move(String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\TempContracts.txt"), String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"))
            ToConsole("OK we should be good to go")
            Return "S"
        End If
    End Function
End Class
