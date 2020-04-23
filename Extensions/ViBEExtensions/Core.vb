''' <summary>
''' Handles the Core Functions of the ViBE Server
''' </summary>
Public Module Core

    ''' <summary>Sends Money Between Accounts </summary>
    Public Function SM(ByRef VUser As ViBEUser, SMCommand As String) As String
        'SM,UMSNB,33118\UMSNB,5000

        Dim OriginBankString As String = SMCommand.Split(",")(1)
        Dim DestinationID As String = SMCommand.Split(",")(2).Split("\")(0)
        Dim DestinationBankString As String = SMCommand.Split(",")(2).Split("\")(1)
        Dim AmountToSend As Long = SMCommand.Split(",")(3)

        ToConsole("Transfering (" & AmountToSend & ") from (" & VUser.ID & "\" & OriginBankString & ") to (" & DestinationID & DestinationBankString & ")")

        Dim DestinationUser As ViBEUser

        For Each User As ViBEUser In VAuthenticator.AllUsers
            If User.ID = DestinationID Then DestinationUser = User
        Next

        If IsNothing(DestinationUser) Then
            ToConsole("Could not find User " & DestinationID, ConsoleColor.DarkRed)
            Return "E"
        End If

        Dim OriginBank As ViBEBank
        Dim DestinBank As ViBEBank

        Select Case OriginBankString.ToUpper
            Case "UMSNB"
                OriginBank = VUser.UMSNB
            Case "GBANK"
                OriginBank = VUser.GBANK
            Case "RIVER"
                OriginBank = VUser.RIVER
            Case Else
                ToConsole("Invalid Origin bank", ConsoleColor.DarkRed)
                Return "E"
        End Select

        Select Case DestinationBankString.ToUpper
            Case "UMSNB"
                DestinBank = DestinationUser.UMSNB
            Case "GBANK"
                DestinBank = DestinationUser.GBANK
            Case "RIVER"
                DestinBank = DestinationUser.RIVER
            Case Else
                ToConsole("Invalid Destination Bank", ConsoleColor.DarkRed)
                Return "E"
        End Select

        Try
            OriginBank.SendMoney(DestinBank, AmountToSend)
        Catch E2 As Exception
            ErrorToConsole("Couldnt Finish Money Transfer.", E2)
            Return "E"
        End Try

        Return "S"
    End Function


    ''' <summary>Certifies a Transaction</summary>
    Public Function Certify(Transaction As String) As String
        Try
            AddToFile(WEB_DIR & "\Certifications.log", "{Certified on: " & DateTime.Now.ToString & "} " & Transaction)
        Catch ex As Exception
            ErrorToConsole("Could not certify transaction", ex)
            Return "E"
        End Try

        Return "S"
    End Function

    ''' <summary>Adds nontaxed funds to an account</summary>
    Public Function NonTaxAdd(ByRef VUser As ViBEUser, NTAMSG As String) As String
        '57174|4640|NTA,57174,5000

        Dim VUserType As ViBEUserType = TryCast(VUser.GetUserType, ViBEUserType)
        If IsNothing(VUserType) Then Return ""
        If VUserType.AuthorityLevel < 4 Then
            ToConsole(VUser.ToString & " does not have enough privileges to access NTA")
            Return "E"
        End If

        Try

            Dim NTAUSR As String = NTAMSG.Split(",")(1)
            Dim NTAAMT As Long = NTAMSG.Split(",")(2)
            Dim OtherUser As ViBEUser

            For Each User As ViBEUser In VAuthenticator.AllUsers
                If User.ID = NTAUSR Then OtherUser = User
            Next

            If IsNothing(OtherUser) Then
                ToConsole("Could Not find user " & NTAUSR, ConsoleColor.DarkRed)
                Return "E"
            End If

            Dim Bank As ViBEBank

            If OtherUser.UMSNB.IsOpen Then
                Bank = OtherUser.UMSNB
            ElseIf OtherUser.GBANK.IsOpen Then
                Bank = OtherUser.GBANK
            ElseIf OtherUser.RIVER.IsOpen Then
                Bank = OtherUser.RIVER
            Else
                ToConsole("User has no open banks! Cannot NTA", ConsoleColor.DarkRed)
                Return "E"
            End If

            Bank.NTA(NTAAMT)
            Return ("S")

        Catch ex As Exception
            ErrorToConsole("Could not NTA money", ex)
            Return "E"
        End Try

    End Function

End Module
