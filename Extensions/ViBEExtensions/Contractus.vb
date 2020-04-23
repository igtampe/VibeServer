Imports System.IO

''' <summary>Contractus Expansion</summary>
Public Module Contractus

    ''' <summary>Executes Contractus Functions</summary>
    ''' <param name="ConMSG"></param>
    ''' <returns></returns>
    Public Function CON(ByRef Vuser As ViBEUser, ConMSG As String) As String
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
                Return ReadUserContracts(Vuser)

            ElseIf ConMSG.StartsWith("ADDTOALL") Then
                'Add Contract to all contracts
                Return AddContractToAll(Vuser, ConMSG.Replace("ADDTOALL", ""))

            ElseIf ConMSG.StartsWith("ADDBID") Then
                'Add Bid
                Return AddBid(Vuser, ConMSG.Replace("ADDBID", ""))

            ElseIf ConMSG.StartsWith("MOVETOUSER") Then
                'Move Contract
                Return MoveToUser(Vuser, ConMSG.Replace("MOVETOUSER", "").Split(";"))

            ElseIf ConMSG.StartsWith("REMOVE") Then
                'Remove a contract
                Return RemoveContract(Vuser, ConMSG.Replace("REMOVE", ""))

            Else
                ToConsole("Unable to parse Contractus Command")
                Return "E"
            End If

        Catch ex As Exception
            ErrorToConsole("The Contractus subsystem crashed.", ex)
            Return "E"
        End Try

    End Function

    ''' <summary>Returns all contracts</summary>
    Private Function ReadAllContracts() As String
        Dim AllContracts(0) As String
        ToConsole("trying to read all contracts")
        If Not File.Exists(String.Concat(UMSWEB_DIR, "\SSH\Contracts.txt")) Then
            Return "N"
        End If

        FileOpen(1, String.Concat(UMSWEB_DIR, "\SSH\Contracts.txt"), OpenMode.Input)
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

    ''' <summary>Returns a contract's details</summary>
    Private Function ConDetails(Details As String) As String
        ToConsole("Retrieving details from contract #" & Details)
        If Not File.Exists(UMSWEB_DIR & "\SSH\CONTRACTS\" & Details & ".txt") Then
            Return "E"
            ToConsole("Looks like it doesn't exist")
        End If

        Try
            Return ReadFromFile(UMSWEB_DIR & "\SSH\CONTRACTS\" & Details & ".txt")
        Catch ex As Exception
            Return "E"
            ErrorToConsole("w o o p s", ex)
        End Try

    End Function

    ''' <summary>Returns a User's Contracts</summary>
    Private Function ReadUserContracts(ByRef VUser As ViBEUser) As String
        Dim notifarray(0) As String
        ToConsole("trying to READ from " & VUser.ToString & "'s Contracts")
        If Not File.Exists(VUser.Directory & "Contracts.txt") Then Return "N"

        FileOpen(1, VUser.Directory & "Contracts.txt", OpenMode.Input)

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

    ''' <summary>Add Contract to all contracts</summary>
    Private Function AddContractToAll(ByRef Vuser As ViBEUser, ConMSG As String) As String
        ToConsole("Attempting to add " & ConMSG & "to all contracts")
        'Build The Building;Build the Building and make it real good boio pls help
        Dim ConMSGSplit() As String
        ConMSGSplit = ConMSG.Split(";")
        Dim ContractID As Integer

        ContractID = 0
        While (File.Exists(UMSWEB_DIR & "\SSH\Contracts\" & ContractID & ".txt"))
            ContractID += 1
        End While

        ToConsole("This contract will be Contract #" & ContractID)
        AddToFile(UMSWEB_DIR & "\SSH\Contracts.txt", String.Join("~", {ContractID, ConMSGSplit(0), Vuser.ID, Vuser.Username, "-1", "Uninitialized", "Uninitialized"}))

        ToFile(UMSWEB_DIR & "\SSH\Contracts\" & ContractID & ".txt", ConMSGSplit(1))

        ToConsole("OK Done")
        Return "S"
    End Function

    ''' <summary>
    ''' Adds a bid to a specified contract
    ''' </summary>
    ''' <param name="ConMSG"></param>
    ''' <returns></returns>
    Private Function AddBid(ByRef VUser As ViBEUser, ConMSG As String) As String
        Dim ConMSGSplit() As String
        'ContractID;NewBid
        ' 0           1   
        ConMSGSplit = ConMSG.Split(";")
        ToConsole("Ok time to add a bid AAAAAAAAAAAAAA")

        If Not File.Exists(String.Concat(UMSWEB_DIR, "\SSH\Contracts.txt")) Then
            Return "E"
        End If

        ToConsole("Trying to Find Contract #" & ConMSGSplit(0))

        FileOpen(1, String.Concat(UMSWEB_DIR, "\SSH\Contracts.txt"), OpenMode.Input)
        FileOpen(2, String.Concat(UMSWEB_DIR, "\SSH\TempContracts.txt"), OpenMode.Output)
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
                    CurrentLine(5) = VUser.ID
                    CurrentLine(6) = VUser.Username

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
            File.Delete(String.Concat(UMSWEB_DIR, "\SSH\TempContracts.txt"))
            Return "E"
        Else
            File.Delete(String.Concat(UMSWEB_DIR, "\SSH\Contracts.txt"))
            File.Move(String.Concat(UMSWEB_DIR, "\SSH\TempContracts.txt"), String.Concat(UMSWEB_DIR, "\SSH\Contracts.txt"))
            Return "S"
        End If


    End Function

    ''' <summary>
    ''' Moves the contract
    ''' </summary>
    ''' <param name="ConMSGSplit"></param>
    ''' <returns></returns>
    Private Function MoveToUser(ByRef VUser As ViBEUser, ConMSGSplit() As String) As String
        'ContractID;User
        ' 0           1
        ToConsole("Ok time to add a bid AAAAAAAAAAAAAA")

        If Not File.Exists(String.Concat(UMSWEB_DIR, "\SSH\Contracts.txt")) Then
            Return "E"
        End If

        ToConsole("Trying to Find Contract #" & ConMSGSplit(0))

        FileOpen(1, String.Concat(UMSWEB_DIR, "\SSH\Contracts.txt"), OpenMode.Input)
        FileOpen(2, String.Concat(UMSWEB_DIR, "\SSH\TempContracts.txt"), OpenMode.Output)
        Dim CurrentLine() As String
        Dim TransferedContract(0) As String
        Dim woops As Boolean = True
        While Not EOF(1)
            CurrentLine = LineInput(1).Split("~")
            'ID~Name~FromID~FromNAME~TopBid~TopBidID~TopBidNAME
            ' 0  1      2     3        4        5          6

            If CurrentLine(0) = ConMSGSplit(0) Then

                If CurrentLine(2) = VUser.ID Then
                    woops = False
                    ToConsole("Found it")
                    TransferedContract = CurrentLine
                Else
                    ToConsole("Found it, but the user executing this call does not have authority to move it", ConsoleColor.DarkRed)

                End If

            Else
                    PrintLine(2, String.Join("~", CurrentLine))
            End If
        End While
        FileClose(1)
        FileClose(2)

        If woops Then
            File.Delete(String.Concat(UMSWEB_DIR, "\SSH\TempContracts.txt"))
            Return "E"
        Else
            File.Delete(String.Concat(UMSWEB_DIR, "\SSH\Contracts.txt"))
            File.Move(String.Concat(UMSWEB_DIR, "\SSH\TempContracts.txt"), String.Concat(UMSWEB_DIR, "\SSH\Contracts.txt"))

            ToConsole("OK now that we've removed it its time to add it to the user's coso")
            If File.Exists(String.Concat(UMSWEB_DIR, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt")) Then
                FileOpen(1, String.Concat(UMSWEB_DIR, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"), OpenMode.Append)
            Else
                FileOpen(1, String.Concat(UMSWEB_DIR, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"), OpenMode.Output)
            End If

            PrintLine(1, String.Join("~", TransferedContract))
            FileClose(1)
            Return "S"
        End If

    End Function

    ''' <summary> Removes a contract</summary>
    Private Function RemoveContract(ByRef VUser As ViBEUser, ContractID As Integer) As String
        'ContractID
        ' 0        

        If Not File.Exists(VUser.Directory & "Contracts.txt") Then
            Return "E"
        End If

        ToConsole("Trying to Find Contract #" & ContractID)

        FileOpen(1, VUser.Directory & "Contracts.txt", OpenMode.Input)
        FileOpen(2, VUser.Directory & "TempContracts.txt", OpenMode.Output)
        Dim CurrentLine() As String
        Dim woops As Boolean = True
        While Not EOF(1)
            CurrentLine = LineInput(1).Split("~")
            'ID~Name~FromID~FromNAME~TopBid~TopBidID~TopBidNAME
            ' 0  1      2     3        4        5          6

            If CurrentLine(0) = ContractID Then
                woops = False
                ToConsole("Found it, not copying it")
            Else
                PrintLine(2, String.Join("~", CurrentLine))
            End If
        End While
        FileClose(1)
        FileClose(2)

        If woops Then
            File.Delete(VUser.Directory & "TempContracts.txt")
            Return "E"
        Else
            File.Delete(VUser.Directory & "Contracts.txt")
            File.Move(VUser.Directory & "TempContracts.txt", VUser.Directory & "Contracts.txt")
            ToConsole("OK we should be good to go")
            Return "S"
        End If
    End Function
End Module
