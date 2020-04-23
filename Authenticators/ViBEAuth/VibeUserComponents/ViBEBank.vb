Public Class ViBEBank

    Public TiedUser As ViBEUser
    Public ReadOnly Name As String
    Public ReadOnly Directory As String

    Private Open As Boolean
    Private Balance As Long

    Public Sub New(Name As String, ByRef TiedUser As ViBEUser)
        Me.Name = Name
        Me.TiedUser = TiedUser

        Directory = TiedUser.Directory & Name & "\"

        If IO.Directory.Exists(Directory) Then
            Open = True
            Balance = ReadFromFile(Directory & "Balance.dll")
        Else
            Open = False
            Balance = 0
        End If

    End Sub

    ''' <summary>Opens this bank account</summary>
    Public Sub OpenBank()
        If Open Then Throw New Exception("User already has this bank!")

        ToConsole("Attempting to Open Bank account " & Name & "From user " & TiedUser.ToString)

        IO.Directory.CreateDirectory(Directory)
        ToFile(Directory & "Balance.dll", 0)
        Balance = 0
        Open = True

        ToBankLog("Account Created!")

    End Sub

    ''' <summary>Closes this bank account</summary>
    Public Sub CloseBank()
        If Not Open Then Throw New Exception("Cannot close a bank that's not open!")
        If Not Balance = 0 Then Throw New Exception("Cannot close bank. Non-zero balance!")

        ToConsole("Attempting to close Bank account " & Name & "From user " & TiedUser.ToString)

        IO.Directory.Delete(Directory, True)
        Balance = 0
        Open = False
    End Sub


    ''' <summary>Send money from this bank account to the other bank account</summary>
    Public Sub SendMoney(OtherBank As ViBEBank, Amount As Long)

        If Not Open Or Not OtherBank.Open Then Throw New Exception("Cannot transfer money. One of these accounts is not opened.")

        ToConsole("Transfering " & Amount.ToString("N0") & "p from " & TiedUser.ID & "\" & Name & " to " & OtherBank.TiedUser.ID & "\" & OtherBank.Name)

        Balance -= Amount
        OtherBank.Balance += Amount

        SaveBalance()
        OtherBank.SaveBalance()

        ToBankLog("You ~vibed~ " & Amount.ToString("N0") & "p to " & OtherBank.TiedUser.ToString & "'s " & OtherBank.Name & " account.")
        OtherBank.ToBankLog(TiedUser.Username.ToString & " ~vibed~ " & Amount.ToString("N0") & "p from their " & Name & " Account to you")

    End Sub

    Public Sub NTA(Amount As Long)
        If Not Open Then Throw New Exception("Cannot transfer money. One of these accounts is not opened.")

        ToConsole("NTA-ing " & Amount.ToString("N0") & "p")

        Balance += Amount

        ToBankLog("Your account was credited " & Amount.ToString("N0") & "p, Non-Taxed")

    End Sub

    Public Function IsOpen() As Boolean
        Return Open
    End Function

    Public Function GetBalance() As Long
        Return Balance
    End Function

    ''' <summary>Saves this bank's balance to disk</summary>
    Public Sub SaveBalance()
        ToConsole("Saving " & TiedUser.ID & "\" & Name & "'s Balance")
        If Not Open Then Throw New Exception("Cannot save balance, User does not have bank open!")
        ToFile(Directory & "Balance.dll", Balance)
    End Sub

    ''' <summary>Copies the log from the log directory to the WEB_DIRECTORY for download by ViBE</summary>
    Public Sub UploadLog()
        ToConsole("Copying " & TiedUser.ID & "\" & Name & "'s Log to the Web Directory")
        IO.File.Copy(Directory & "\Log.log", WEB_DIR & "\LOGS\" & TiedUser.ID & Name & ".log", True)
    End Sub

    ''' <summary>Appends specified text to the bank logs</summary>
    Public Sub ToBankLog(Message As String)
        AddToFile(Directory & "Log.log", "[" & DateTime.Now.ToString & "] " & Message)
    End Sub

End Class
