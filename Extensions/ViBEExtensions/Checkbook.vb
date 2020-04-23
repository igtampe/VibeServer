''' <summary>Checkbook 2000 Extension</summary>
Public Module Checkbook

    ''' <summary>Execute Checkbook Functions</summary>
    Public Function CHCKBK(ByRef VUser As ViBEUser, CHCKMSG As String) As String
        Try

            ToConsole("Invoking Checkbook 2000 System")

            If CHCKMSG.StartsWith("READ") Then
                Return VUser.Checkbook.GetChecks

            ElseIf CHCKMSG.StartsWith("REMO") Then
                VUser.Checkbook.RemoveItems(CHCKMSG.Replace("REMO", ""))
                Return "S"

            ElseIf CHCKMSG.StartsWith("ADD") Then
                VUser.Checkbook.AddItem(CHCKMSG.Replace("ADD", ""))
                Return "S"

            ElseIf CHCKMSG.StartsWith("EXECUTE") Then
                'EXECUTE1,UMSNB
                VUser.Checkbook.ExecuteItem(CHCKMSG.Substring(7).Split(",")(0), CHCKMSG.Substring(7).Split(",")(1))
                Return "S"

            Else
                Return "E"
            End If

        Catch e As Exception
            ErrorToConsole("Something Happened", e)
            Return "E"

        End Try
    End Function

End Module
