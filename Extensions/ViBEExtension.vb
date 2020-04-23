Imports Contractus
Imports Core
Imports EzTax

Public Class ViBEExtension
    Implements ISmokeSignalAuthenticatedExtension
    Public Const EXTENSION_NAME = "ViBE Super Extension"
    Public Const EXTENSION_VERS = "1.0"

    Public Sub New()
    End Sub

    Public Sub Tick() Implements ISmokeSignalAuthenticatedExtension.Tick
        'Do nothing
    End Sub

    Public Function Parse(ClientMSG As String) As String Implements ISmokeSignalAuthenticatedExtension.Parse

        If ClientMSG = "CONNECTED" Then
            ToConsole("Classic ping packet received! Replying...")
            Return "You've connected to the ViBE Server! Congrats!"
        End If

        If ClientMSG = "DIR" Then
            'DIR
            Return String.Join(",", VAuthenticator.AllUsers.ToArray)
        ElseIf ClientMSG.StartsWith("INFO") Then
            'INFO57174
            For Each User As ViBEUser In VAuthenticator.AllUsers
                If User.ID = ClientMSG.Substring(4) Then Return User.Info
            Next

            'We couldn't find him :(
            Return "E"

        ElseIf Command.StartsWith("CERT") Then
            'CERT
            Return Certify(Command.Remove(0, 4))

        End If

        'Cannot parse
        Return ""
    End Function

    Public Function Parse(User As ISmokeSignalUser, Command As String) As String Implements ISmokeSignalAuthenticatedExtension.Parse
        Dim VUser As ViBEUser = TryCast(User, ViBEUser)
        If IsNothing(VUser) Then Return ""

        'Get the actual VUser, not a copy
        For Each Tipillo As ViBEUser In VAuthenticator.AllUsers
            If VUser.Equals(Tipillo) Then VUser = Tipillo
        Next

        If Command = "CONNECTED" Then
            ToConsole("Classic packet, but authenticated. Replying...")
            Return "You've connected to the ViBE Server, and successfully authenticated as " & VUser.ToString & " Congrats!"
        End If

        If (Command.StartsWith("SM")) Then
            'SM,UMSNB,33118\UMSNB,5000
            Return SM(VUser, Command.Remove(0, 2))

        ElseIf (Command.StartsWith("CP")) Then
            '57174|4640|CP4641
            Try
                VUser.ChangePin(Command.Remove(0, 2))
                Return "S"
            Catch ex As Exception
                ErrorToConsole("Could not update PIN", ex)
                Return "E"
            End Try

        ElseIf Command.StartsWith("NOTIF") Then
            '57174|4640|NOTIFREMO4
            Return Notifications(VUser, Command.Substring(5))

        ElseIf Command.StartsWith("BNK") Then
            '57174|4640|BNK,A,GBANK
            Return BNK(VUser, Command)

        ElseIf Command.StartsWith("CHCKBK") Then
            'Checkbook 2000 Subsystem
            Return CHCKBK(VUser, Command.Replace("CHCKBK", ""))

        ElseIf Command.StartsWith("NTA") Then
            '57174|4640|NTA,57174,5000
            Return NonTaxAdd(VUser, Command.Replace("NTA", ""))

        ElseIf Command.StartsWith("EZT") Then
            'EzTax
            Return EZT(VUser, Command.Remove(0, 3))

        ElseIf Command.StartsWith("CON") Then
            'Contractus
            Return CON(VUser, Command.Replace("CON", ""))
        Else

        End If


        Return ""
    End Function

    Public Function getName() As String Implements ISmokeSignalAuthenticatedExtension.GetName
        Return EXTENSION_NAME
    End Function

    Public Function getVersion() As String Implements ISmokeSignalAuthenticatedExtension.GetVersion
        Return EXTENSION_VERS
    End Function

End Class
