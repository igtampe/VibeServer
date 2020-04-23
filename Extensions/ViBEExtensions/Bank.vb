Imports System.IO

''' <summary>
''' Bank Management Expansion
''' </summary>
Public Class Bank

    ''' <summary>
    ''' Launches BNK Tools
    ''' </summary>
    ''' <param name="BNKMSG"></param>
    ''' <returns></returns>
    Public Shared Function BNK(BNKMSG As String) As String
        ToConsole("BNK Tools invoked")
        'BNKA57174GBANK

        If Not BNKMSG.Count = 11 Then
            ToConsole("Something seems fishy, I'm not going to do it.")
            Return "E"
        End If                    'A57174GBANK
        Dim BNKACT As String = BNKMSG.Remove(1, BNKMSG.Count - 1)
        'A
        Dim BNKID As String = BNKMSG.Remove(6, 5)
        'A57174
        BNKID = BNKID.Remove(0, 1)
        '57174
        Dim BNKBNK As String = BNKMSG.Remove(0, 6)
        'GBANK

        Select Case BNKACT
            Case "C"
                Try
                    ToConsole("Attempting to close (" & BNKID & ")'s (" & BNKBNK & ") account.")
                    FileOpen(3, UMSWEB_DIR & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\Balance.dll", OpenMode.Input)
                    Dim BNKBAL As Long = LineInput(3)
                    ToConsole("Bank balance is currently (" & BNKBAL & ")")
                    FileClose(3)
                    If Not BNKBAL = "0" Then
                        ToConsole("Looks to me like it's not 0. Stopping.")
                        Return "E"
                    End If

                    ToConsole("Deleting Folder")
                    Directory.Delete(UMSWEB_DIR & "\SSH\USERS\" & BNKID & "\" & BNKBNK, True)
                    ToConsole("KK Done!")
                    Return "S"
                Catch ex As Exception
                    ErrorToConsole("Shoot I couldn't do that.", ex)
                    Return "E"
                End Try

            Case "O"
                Try
                    ToConsole("Attempting to open (" & BNKID & ")'s (" & BNKBNK & ") account.")
                    Directory.CreateDirectory(UMSWEB_DIR & "\SSH\USERS\" & BNKID & "\" & BNKBNK)
                    Directory.CreateDirectory(UMSWEB_DIR & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\CHECKS")

                    FileOpen(1, UMSWEB_DIR & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\BALANCE.dll", OpenMode.Output)
                    PrintLine(1, "0000")
                    FileClose(1)

                    FileOpen(1, UMSWEB_DIR & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\Log.log", OpenMode.Output)
                    PrintLine(1, "[" & DateTime.Now.ToString & "] Account Created on ViBE")
                    FileClose(1)
                    Return "S"
                Catch ex As Exception
                    ErrorToConsole("Shoot I couldn't do that.", ex)
                    Return "E"
                End Try

            Case "L"
                Try
                    ToConsole("Copying " & UMSWEB_DIR & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\Log.log to " & WEB_DIR & "\LOGS\" & BNKID & BNKBNK & ".log...")
                    File.Copy(UMSWEB_DIR & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\Log.log", WEB_DIR & "\LOGS\" & BNKID & BNKBNK & ".log", True)
                    ToConsole("Done!")
                    Return "S"
                Catch ex As Exception
                    ErrorToConsole("Shoot I couldn't do that.", ex)
                    Return "E"
                End Try

            Case Else
                Return "E"
        End Select

    End Function
End Class
