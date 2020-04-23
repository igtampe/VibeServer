Imports System.IO

''' <summary>
''' Handles the Core Functions of the ViBE Server
''' </summary>
Public Class Core

    ''' <summary>
    ''' Sends Money Between Accounts
    ''' </summary>
    ''' <param name="SMCommand">Formatted as [ACCOUNT1][ACCOUNT2][AMOUNT]</param>
    ''' <returns></returns>
    Public Shared Function SM(SMCommand As String) As String
        Dim Origin As String
        Dim Destination As String
        Dim AmmountToSend As Long

        Try
            Dim length As Integer = SMCommand.Length - 22
            Origin = SMCommand.Remove(11, 11 + length)
            Destination = SMCommand.Remove(0, 11)
            Destination = Destination.Remove(11, length)
            AmmountToSend = SMCommand.Remove(0, 22)
        Catch exception1 As Exception
            ErrorToConsole("Improper Send Money Request", exception1)
            Return 1
        End Try

        ToConsole("Transfering (" & AmmountToSend & ") from (" & Origin & ") to (" & Destination & ")")

        Try
            'Get Origin Balance
            Dim OriginBalance As Long = ReadFromFile(UserFile(Origin, "Balance.dll"))

            'Get Destination Balance
            Dim DestinationBalance As Long = ReadFromFile(UserFile(Destination, "Balance.dll"))

            'Commit the Operation
            OriginBalance -= AmmountToSend
            DestinationBalance += AmmountToSend

            'Save Origin balance
            ToFile(UserFile(Origin, "Balance.dll"), OriginBalance)

            'Save Destination Balance
            ToFile(UserFile(Destination, "Balance.dll"), DestinationBalance)

            'Extra Income Declaration
            Dim DestinationID = Destination.Remove(5, 6)
            Dim ExtraIncome As Long

            'Get Extra Income
            If File.Exists(UserFile(DestinationID, "EI.dll")) Then
                ExtraIncome = ReadFromFile(UserFile(DestinationID, "EI.dll"))
            Else
                ExtraIncome = 0
            End If

            'The Operation
            ToConsole("Adding (" & AmmountToSend & ") to (" & DestinationID & ")'s pre-existing Extra income of (" & ExtraIncome & ")")
            ExtraIncome += AmmountToSend

            'Save the Exctra Income
            ToConsole("Added, opening the file again.")
            ToFile(UserFile(DestinationID, "EI.dll"), ExtraIncome)

            'Write Log 1
            ToConsole("Added it, writing to log1")
            AddToFile(UserFile(Origin, "Log.Log"), "[" & DateTime.Now & "] You ~vibed~ " & AmmountToSend.ToString("N0") & "p to " & UserIDToLabel(Destination))

            'Write Log 2
            ToConsole("Writing log2")
            AddToFile(UserFile(Destination, "Log.Log"), "[" & DateTime.Now & "] " & UserIDToLabel(Origin) & " ~vibed~ " & AmmountToSend.ToString("N0") & "p to you.")

            'Write Notif
            ToConsole("Writing to Notif")
            AddToFile(UserFile(DestinationID, "notifs.txt"), DateTime.Now.ToString & "`" & UserIDToLabel(Origin) & " Sent " & AmmountToSend.ToString("N0") & "p to your " & Destination.Remove(0, 6) & " account")
        Catch E2 As Exception
            ErrorToConsole("Couldnt Finish Money Transfer.", E2)
            Return "E"
        End Try
        Return "S"
    End Function

    ''' <summary>
    ''' Transfers money between owned bank accounts
    ''' </summary>
    ''' <param name="TMCommand">Formatted as [ID][OriginBank][DestinationBank][Amount]</param>
    ''' <returns></returns>
    Public Shared Function TM(TMCommand As String) As String
        Dim UserID As String
        Dim OriginBank As String
        Dim DestinationBank As String
        Dim TransferAmount As Long

        Try
            Dim length1 As Integer = TMCommand.Length
            Dim Length2 As Integer = length1 - 15
            UserID = TMCommand.Remove(5, 10 + Length2)
            OriginBank = TMCommand.Remove(0, 5)
            OriginBank = OriginBank.Remove(5, 5 + Length2)
            DestinationBank = TMCommand.Remove(0, 10)
            DestinationBank = DestinationBank.Remove(5, Length2)
            TransferAmount = TMCommand.Remove(0, 15)
        Catch exception3 As Exception
            ErrorToConsole("Improperly Coded Transfer Request", exception3)
            Return 1
        End Try

        ToConsole("Transfering (" & TransferAmount & ") from (" & OriginBank & ") to (" & DestinationBank & ") of (" & UserID & ")")

        Try

            'Get the Balances
            Dim OriginBalance As Long = ReadFromFile(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\" & OriginBank & "\Balance.dll")
            Dim DestinationBalance As Long = ReadFromFile(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\" & DestinationBank & "\Balance.dll")

            'Operations
            OriginBalance -= TransferAmount
            DestinationBalance += TransferAmount

            'Write Balances
            ToFile(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\" & OriginBank & "\balance.dll", OriginBalance)
            ToFile(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\" & DestinationBank & "\balance.dll", DestinationBalance)

            'Logging
            AddToFile(UMSWEB_DIR & "\SSH\users\" & UserID & "\" & OriginBank & "\log.log", "[" & DateTime.Now & "] You ~vibed~ " & TransferAmount.ToString("N0") & "p to " & DestinationBank)
            AddToFile(UMSWEB_DIR & "\SSH\users\" & UserID & "\" & DestinationBank & "\log.log", "[" & DateTime.Now & "] You ~vibed~ " & TransferAmount.ToString("N0") & "p from " & OriginBank)

        Catch E4 As Exception
            ErrorToConsole("Couldnt Finish Money Transfer.", E4)
            Return "E"
        End Try
        Return "S"
        End

    End Function

    ''' <summary>
    ''' Change Pin Request
    ''' </summary>
    ''' <param name="ChangePinCommand">[ID][NEWPIN]</param>
    ''' <returns></returns>
    Public Shared Function ChangePin(ChangePinCommand As String) As String
        Dim UserID As String
        Dim NewPin As Integer

        Try
            UserID = ChangePinCommand.Remove(5, 4)
            NewPin = ChangePinCommand.Remove(0, 5)
        Catch E5 As Exception
            ErrorToConsole("Improperly Coded Pin Change Request", E5)
            Return 1
        End Try

        ToConsole("Changing Pin of " & GetUsername(UserID) & " (" & UserID & ") with (" & NewPin & "), (" & ChangePinCommand & ")")

        Try
            ToFile(UserFile(UserID, "Pin.DLL"), NewPin)
        Catch E6 As Exception
            ErrorToConsole("Could not complete pin change", E6)
            Return 2
        End Try

        Return "S"

    End Function

    ''' <summary>
    ''' Returns Information on a Specific User
    ''' </summary>
    ''' <param name="UserID"></param>
    ''' <returns></returns>
    Public Shared Function INFO(UserID As String)

        Dim UMSNBBalance As String = 0
        Dim GBANKBalance As String = 0
        Dim RIVERBalance As String = 0
        Dim HasUMSNB As String = 0
        Dim HasGBANK As String = 0
        Dim HasRIVER As String = 0

        Dim UserName As String = GetUsername(UserID)

        If File.Exists(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\UMSNB\Balance.dll") Then
            HasUMSNB = 1
            UMSNBBalance = ReadFromFile(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\UMSNB\Balance.dll")
        End If

        If File.Exists(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\GBANK\Balance.dll") Then
            HasGBANK = 1
            GBANKBalance = ReadFromFile(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\GBANK\Balance.dll")
        End If

        If File.Exists(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\RIVER\Balance.dll") Then
            HasRIVER = 1
            RIVERBalance = ReadFromFile(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\RIVER\Balance.dll")
        End If

        ToConsole("Attempting to find any notifications...")
        Dim notifs As String

        Try
            notifs = File.ReadAllLines(UMSWEB_DIR & "\SSH\USERS\" & UserID & "\notifs.txt").Length
            ToConsole("Found " & notifs & " notification(s)")
        Catch ex As Exception
            notifs = 0
            ToConsole("Found no notification file")
        End Try

        ToConsole("Sending Info...")
        Return String.Concat(New String() {HasUMSNB, ",", UMSNBBalance, ",", HasGBANK, ",", GBANKBalance, ",", HasRIVER, ",", RIVERBalance, ",", UserName, ",", notifs})

    End Function

    ''' <summary>
    ''' Certifies a Transaction
    ''' </summary>
    ''' <param name="Transaction"></param>
    ''' <returns></returns>
    Public Shared Function Certify(Transaction As String) As String
        Try
            AddToFile(WEB_DIR & "\Certifications.log", "{Certified on: " & DateTime.Now.ToString & "} " & Transaction)
        Catch ex As Exception
            ErrorToConsole("Could not certify transaction", ex)
            Return "E"
        End Try

        Return "S"

    End Function

    ''' <summary>
    ''' Adds nontaxed funds to an account
    ''' </summary>
    ''' <param name="NTAMSG"></param>
    ''' <returns></returns>
    Public Shared Function NonTaxAdd(NTAMSG As String) As String
        ToConsole("Non-Taxed Add Invoked")
        'NTA57174400000000
        Dim NTAUSR As String = NTAMSG.Remove(5, NTAMSG.Count - 5)
        Dim NTAAMT As Long = NTAMSG.Remove(0, 5)

        ToConsole("Adding " & NTAAMT.ToString("N0") & "p to " & NTAUSR & "'s first account.")
        If Not Directory.Exists(UMSWEB_DIR & "\SSH\USERS\" & NTAUSR) Then
            Return "E"
            ToConsole("Unable to find user " & NTAUSR)
        End If

        Dim NTABNK As String
        If Directory.Exists(UMSWEB_DIR & "\SSH\USERS\" & NTAUSR & "\UMSNB") Then
            NTABNK = "UMSNB"
        ElseIf Directory.Exists(UMSWEB_DIR & "\SSH\USERS\" & NTAUSR & "\GBANK") Then
            NTABNK = "GBANK"
        ElseIf Directory.Exists(UMSWEB_DIR & "\SSH\USERS\" & NTAUSR & "\RIVER") Then
            NTABNK = "RIVER"
        Else
            ToConsole("User " & NTAUSR & " Has no bank accounts!")
            Return "E"
        End If

        Try
            FileOpen(1, UMSWEB_DIR & "\SSH\USERS\" & NTAUSR & "\" & NTABNK & "\Balance.dll", OpenMode.Input)
            Dim NTCB As Long = Conversions.ToLong(LineInput(1))
            FileClose(1)
            NTCB += NTAAMT


            FileOpen(1, UMSWEB_DIR & "\SSH\USERS\" & NTAUSR & "\" & NTABNK & "\Balance.dll", OpenMode.Output)
            WriteLine(1, NTCB)
            FileClose(1)
            ToConsole("Added it, writing to log")


            MyProject.Computer.FileSystem.WriteAllText(String.Concat(UMSWEB_DIR, "\SSH\users\", NTAUSR, "\", NTABNK, "\log.log"), String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] ASIMOV added ", NTAAMT.ToString("N0"), "p to your account, non-taxed ", "" & vbCrLf & ""}), True)

            ToConsole("Writing to Notif")

            FileOpen(1, String.Concat(UMSWEB_DIR, "\SSH\users\", NTAUSR, "\notifs.txt"), OpenMode.Append)
            PrintLine(1, DateTime.Now.ToString & "`" & "ASIMoV added" & NTAAMT.ToString("N0") & "p to your " & NTABNK & " account, non-taxed.")
            FileClose(1)

            Return "S"
        Catch ex As Exception
            ErrorToConsole("Something went wrong", ex)
            Return "E"
        End Try
    End Function

    ''' <summary>
    ''' Gets the name of a user
    ''' </summary>
    ''' <param name="User">(Can take Destinations)</param>
    ''' <returns></returns>
    Private Shared Function GetUsername(User As String) As String
        If User.EndsWith("\UMSNB") Then User = User.Replace("\UMSNB", "")
        If User.EndsWith("\GBANK") Then User = User.Replace("\GBANK", "")
        If User.EndsWith("\RIVER") Then User = User.Replace("\RIVER", "")

        Return ReadFromFile(UMSWEB_DIR & "\SSH\USERS\" & User & "\NAME.DLL")
    End Function
    ''' <summary>
    ''' Presents the name of the user in a label (USERNAME (USERID))
    ''' </summary>
    ''' <param name="User"></param>
    ''' <returns></returns>
    Private Shared Function UserIDToLabel(User As String) As String
        Return GetUsername(User) & " (" & User & ")"
    End Function

End Class
