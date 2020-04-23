''' <summary>Bank Management Expansion</summary>
Public Module Bank

    ''' <summary>Launches BNK Tools</summary>
    Public Function BNK(ByRef Vuser As ViBEUser, BNKMSG As String) As String
        ToConsole("BNK Tools invoked")
        'BNK,A,GBANK

        Dim BNKACT As String = BNKMSG.Split(",")(1)
        Dim BNKBNK As ViBEBank

        Select Case BNKMSG.Split(",")(2).ToUpper
            Case "UMSNB"
                BNKBNK = Vuser.UMSNB
            Case "GBANK"
                BNKBNK = Vuser.GBANK
            Case "RIVER"
                BNKBNK = Vuser.RIVER
            Case Else
                ToConsole("Invlaid bank specified", ConsoleColor.DarkRed)
                Return "E"
        End Select

        Select Case BNKACT
            Case "C"
                Try
                    ToConsole("Attempting to close (" & Vuser.ID & ")'s (" & BNKBNK.Name & ") account.")

                    BNKBNK.CloseBank()

                    Return "S"
                Catch ex As Exception
                    ErrorToConsole("Shoot I couldn't do that.", ex)
                    Return "E"
                End Try

            Case "O"
                Try

                    BNKBNK.OpenBank()
                    Return "S"

                Catch ex As Exception

                    ErrorToConsole("Shoot I couldn't do that.", ex)
                    Return "E"
                End Try

            Case "L"
                Try

                    BNKBNK.UploadLog()
                    Return "S"

                Catch ex As Exception
                    ErrorToConsole("Shoot I couldn't do that.", ex)
                    Return "E"
                End Try

            Case Else
                ToConsole("Couldn't Parse command")
                Return "E"
        End Select

    End Function
End Module
