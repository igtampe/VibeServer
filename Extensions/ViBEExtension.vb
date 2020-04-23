Imports Bank
Imports Checkbook
Imports Contractus
Imports Core
Imports EzTax
Imports Notif

Public Class ViBEExtension
    Implements ISmokeSignalExtension
    Public Const EXTENSION_NAME = "ViBE Super Extension"
    Public Const EXTENSION_VERS = "1.0"

    Public Sub New()

    End Sub

    Public Sub Tick() Implements ISmokeSignalExtension.Tick
        'Do nothing
    End Sub

    Public Function Parse(ClientMSG As String) As String Implements ISmokeSignalExtension.Parse
        If (ClientMSG.StartsWith("CU")) Then
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
            Return ""
        End If
    End Function
    Public Function getName() As String Implements ISmokeSignalExtension.GetName
        Return EXTENSION_NAME
    End Function

    Public Function getVersion() As String Implements ISmokeSignalExtension.GetVersion
        Return EXTENSION_VERS
    End Function

End Class
