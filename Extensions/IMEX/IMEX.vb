Imports Utils
Imports TaxCalc
Imports System.IO
Imports System.Collections

Public Class IMEX
    Implements ISmokeSignalExtension

    Public Const EXTENSION_NAME = "IncomeMan Express"
    Public Const EXTENSION_VERS = "2.1"
    Public Calculator As TaxCalc


    Public Structure IMEXUser
        Public ID As String
        Public Category As Integer
        Public TaxInfo As TaxInformation
        Public Income As Long
        Public EI As Long

        Public Function TopBank() As String
            If HasBank("UMSNB") Then Return "UMSNB"
            If HasBank("GBANK") Then Return "GBANK"
            If HasBank("RIVER") Then Return "RIVER"
            Throw NoBankException
        End Function

        Public Function TaxBank() As String
            Dim TopBank As String = "NOBANK"
            Try
                If GetBankBalance(TopBank) < GetBankBalance("UMSNB") Then TopBank = "UMSNB"
                If GetBankBalance(TopBank) < GetBankBalance("GBANK") Then TopBank = "GBANK"
                If GetBankBalance(TopBank) < GetBankBalance("RIVER") Then TopBank = "RIVER"
                Return TopBank
            Catch ex As Exception
                Return TopBank
            End Try
        End Function

        Public Function HasBank(Bank As String) As Boolean
            Return Directory.Exists(UMSWEB_DIR & "\ssh\Users\" & ID & "\" & Bank)
        End Function

        Public Function GetBankBalance(Bank As String) As Long
            If Bank = "NOBANK" Then Return 0
            If Not HasBank(Bank) Then Return 0

            Try
                FileOpen(4, UMSWEB_DIR & "\ssh\Users\" & ID & "\" & Bank & "\BALANCE.DLL", OpenMode.Input)
                Dim Balance As Long = LineInput(4)
                FileClose(4)
                Return Balance
            Catch ex As Exception
                Return 0
            End Try

        End Function

        Public Sub New(ID As String, Category As Long, Calculator As TaxCalc)
            Me.ID = ID
            Me.Category = Category

            Dim NewpondIncome As Long
            Dim UrbiaIncome As Long
            Dim ParadisusIncome As Long
            Dim LaertesIncome As Long
            Dim NOstenIncome As Long
            Dim SOstenIncome As Long


            'GRAB THE INCOME
            Try
                FileOpen(2, UMSWEB_DIR & "\ssh\Users\" & ID & "\BREAKDOWN.dll", OpenMode.Input)
                Dim TempBreakdown() As String = LineInput(2).Split(",")
                FileClose(2)
                NewpondIncome = TempBreakdown(0)
                UrbiaIncome = TempBreakdown(1)
                ParadisusIncome = TempBreakdown(2)
                LaertesIncome = TempBreakdown(3)
                NOstenIncome = TempBreakdown(4)
                SOstenIncome = TempBreakdown(5)
            Catch
                If Not Category = 2 Then
                    ToConsole("Unable to get " & ID & "'s income breakdown", ConsoleColor.Yellow)
                    ToLog("WARN: Unable to get " & ID & "'s income breakdown.")
                End If
                NewpondIncome = 0
                UrbiaIncome = 0
                ParadisusIncome = 0
                LaertesIncome = 0
                NOstenIncome = 0
                SOstenIncome = 0
            End Try

            Try
                FileOpen(2, UMSWEB_DIR & "\ssh\Users\" & ID & "\EI.dll", OpenMode.Input)
                EI = LineInput(2)
                FileClose(2)
            Catch
                EI = 0
            End Try

            TaxInfo = New TaxInformation(EI, NewpondIncome, UrbiaIncome, ParadisusIncome, LaertesIncome, NOstenIncome, SOstenIncome, Category, Calculator)
            Income = TaxInfo.FederalIncome - EI
        End Sub

        Public Sub ClearEI()
            File.Delete(UMSWEB_DIR & "\ssh\Users\" & ID & "\EI.dll")
        End Sub

        Public Sub Pay()
            'PAY
            NTA(Income)

            'Send to Logs
            FileOpen(4, UMSWEB_DIR & "\ssh\Users\" & ID & "\" & TopBank() & "\log.log", OpenMode.Append)
            PrintLine(4, "[" & DateTime.Now.ToString & "] IMEX has applied your monthly income of " & Income.ToString("N0") & "p")
            FileClose(4)
        End Sub

        Public Sub Tax()
            'Take the money out
            NTA(-1 * TaxInfo.TotalTax)

            'Send to Logs
            FileOpen(4, UMSWEB_DIR & "\ssh\Users\" & ID & "\" & TopBank() & "\log.log", OpenMode.Append)
            PrintLine(4, "[" & DateTime.Now.ToString & "] IMEX applied a tax of " & TaxInfo.TotalTax.ToString("N0") & "p to your account.")
            PrintLine(4, "[" & DateTime.Now.ToString & "] Your total income (monthly and extra) last month was " & (TaxInfo.FederalIncome).ToString("N0") & "p")
            FileClose(4)

            'Clear the EI file
            ClearEI()

            'Send Taxes to appropriate accounts
            Try
                UMSGov.NTA(TaxInfo.Federal.MoneyOwed, True, ID)
                Newpond.NTA(TaxInfo.Newpond.MoneyOwed, True, ID)
                Paradisus.NTA(TaxInfo.Paradisus.MoneyOwed, True, ID)
                Urbia.NTA(TaxInfo.Urbia.MoneyOwed, True, ID)
                Laertes.NTA(TaxInfo.Laertes.MoneyOwed, True, ID)
                NorthOsten.NTA(TaxInfo.NorthOsten.MoneyOwed, True, ID)
                SouthOsten.NTA(TaxInfo.SouthOsten.MoneyOwed, True, ID)
            Catch ex As Exception
                ToLog("Could not send money to UMSGov")
            End Try

        End Sub

        Public Sub NTA(Amount As Long, Optional Log As Boolean = False, Optional From As String = "")
            Dim top As String = TopBank()
            If top = "NOBANK" Then
                'Do nothing
            Else
                Dim Balance As Long = GetBankBalance(top) + Amount
                FileOpen(4, UMSWEB_DIR & "\ssh\Users\" & ID & "\" & TopBank() & "\BALANCE.DLL", OpenMode.Output)
                WriteLine(4, Balance)
                FileClose(4)

                If Log Then
                    FileOpen(4, UMSWEB_DIR & "\ssh\Users\" & ID & "\" & TopBank() & "\log.log", OpenMode.Append)
                    PrintLine(4, "[" & DateTime.Now.ToString & "] IMEX Moved " & Amount.ToString("N0") & "p to your account from " & From)
                    FileClose(4)
                End If

            End If
        End Sub
    End Structure

    Public Shared NoBankException As Exception = New Exception("The User has no bank")
    Private Shared UMSGov As IMEXUser
    Private Shared Newpond As IMEXUser
    Private Shared Paradisus As IMEXUser
    Private Shared Urbia As IMEXUser
    Private Shared Laertes As IMEXUser
    Private Shared NorthOsten As IMEXUser
    Private Shared SouthOsten As IMEXUser

    Private NormalUsers() As IMEXUser
    Private CorporateUsers() As IMEXUser
    Private GovUsers() As IMEXUser

    Private Shared ErrorList As ArrayList

    Public Sub New()
        ToConsole("Initializing IMEX", ConsoleColor.Cyan)

        If Not File.Exists("TaxInfo.txt") Then
            ErrorToConsole("Could not initialize IMEX! Could not find TaxInfo.txt", New FileNotFoundException)
            Return
        End If

        Calculator = New TaxCalc("TaxInfo.txt")
        ToConsole("Loaded TaxInfo version " & Calculator.TaxInfoID & " With " & Calculator.NumberOfBrackets & " Brackets in total", ConsoleColor.Cyan)

        UMSGov = New IMEXUser("33118", 2, Calculator)
        Newpond = New IMEXUser("86700", 2, Calculator)
        Paradisus = New IMEXUser("86701", 2, Calculator)
        Urbia = New IMEXUser("86702", 2, Calculator)
        Laertes = New IMEXUser("86703", 2, Calculator)
        NorthOsten = New IMEXUser("86704", 2, Calculator)
        SouthOsten = New IMEXUser("86705", 2, Calculator)
    End Sub

    Public Sub Tick() Implements ISmokeSignalExtension.Tick
        'Check if it's the first or 15th of the month, and make sure we haven't already done this cosita
        'That will be implemented over in 1.1 because first we need to test this.
    End Sub

    Public Function Parse(Command As String) As String Implements ISmokeSignalExtension.Parse
        If IsNothing(Calculator) Then Return ""
        If Command.StartsWith("IMEX") Then

            ToConsole("Hello yes, IMEX", ConsoleColor.DarkCyan)
            Dim IMEXCommand As String() = Command.Split(",")

            'Basic checks and initialization 
            If IMEXCommand.Count < 2 Then Return ""
            If IMEXCommand(1).ToUpper = "TAXVER" Then Return Calculator.TaxInfoID

            If Not Initialize() Then Return "IE"

            Select Case IMEXCommand(1).ToUpper
                Case "TAX"
                    Tax(NormalUsers)
                    Tax(CorporateUsers)
                Case "INCOME"
                    Payday(NormalUsers)
                    Payday(CorporateUsers)
                    Payday(GovUsers)
                Case Else
                    Return Nothing
            End Select

            'If the command to tax or pay was not sent, this will already not be executed

            If ErrorList.Count = 0 Then Return "S"
            Dim ErrorReturn As String = "E"

            For Each Cosa As String In ErrorList
                ErrorReturn += "," & Cosa
            Next

            Return ErrorReturn

        Else
            Return Nothing
        End If
    End Function

    Private Function Initialize() As Boolean
        ToConsole("Initializing User Arrays...")
        NormalUsers = LoadUsers(UMSWEB_DIR & "\ssh\incomeman\UserList.isf", 0)
        If IsNothing(NormalUsers) Then
            ToConsole("Could not retrieve Normal Users", ConsoleColor.DarkRed)
            Return False
        End If

        CorporateUsers = LoadUsers(UMSWEB_DIR & "\ssh\incomeman\Corporate.isf", 1)
        If IsNothing(CorporateUsers) Then
            ToConsole("Could not retrieve Corporate Users", ConsoleColor.DarkRed)
            Return False
        End If

        GovUsers = LoadUsers(UMSWEB_DIR & "\ssh\incomeman\NonTaxed.isf", 1)
        If IsNothing(GovUsers) Then
            ToConsole("Could not retrieve Government Users", ConsoleColor.DarkRed)
            Return False
        End If

        ErrorList = New ArrayList
        Return True
    End Function

    Function LoadUsers(ISFDir As String, Category As Integer) As IMEXUser()

        'OPEN THE ISF
        Try
            FileOpen(1, ISFDir, OpenMode.Input)
        Catch ex As Exception
            ErrorToConsole(ex.Message, ex)
            Return Nothing
        End Try

        Dim Counter As Integer = 1
        Dim TempStringHolder As String
        Dim Users(0) As IMEXUser

        'READ THE FILE
        While Not EOF(1)
            TempStringHolder = LineInput(1)
            If (TempStringHolder.StartsWith("USER")) Then
                ReDim Preserve Users(Counter - 1)

                'ADD THE USER
                Users(Counter - 1) = New IMEXUser(TempStringHolder.Replace("USER" & Counter & ":", ""), Category, Calculator)

                'LOG IT
                ToLog("INFO: Loaded user " & Counter & " which is " & Users(Counter - 1).ID & " and has an income of " & (Users(Counter - 1).TaxInfo.FederalIncome - Users(Counter - 1).EI).ToString("N0") & "p")

                Counter += 1

            End If
        End While
        FileClose(1)

        'AND RETURN THE USERS
        Return Users

        'THE NEW EMERGENCY LOAD USERS FUNCTION FROM LEGO CITY
    End Function

    Sub Tax(Users As IMEXUser())
        For Each Tipillo As IMEXUser In Users
            'Tax
            Try
                Tipillo.Tax()
                ToLog("INFO: Applied a tax of (" & Tipillo.TaxInfo.TotalTax & ") to " & Tipillo.ID & "'s Income (" & Tipillo.Income & ")")
            Catch EX As Exception
                If EX.Equals(NoBankException) Then
                    ToConsole("Unable to Tax " & Tipillo.ID & " because he has no bank!", ConsoleColor.DarkRed)
                    ToLog("ERROR: Failed to apply a tax to " & Tipillo.ID & "'s Income since he has no bank!")
                    ToErrorList("W", Tipillo.ID & " Has no bank")
                Else
                    ErrorToConsole("Unable to tax " & Tipillo.ID, EX)
                    ToLog("ERROR: Failed to apply a tax to " & Tipillo.ID & "'s Income. " & EX.Message & vbNewLine & EX.StackTrace)
                    ToErrorList("E", Tipillo.ID & " Could not be paid")
                End If
            End Try
        Next

    End Sub

    Sub Payday(Users As IMEXUser())
        For Each tipillo As IMEXUser In Users
            Try
                tipillo.Pay()
                ToLog("INFO: Payed out " & tipillo.ID & "'s Income (" & tipillo.Income & ")")
            Catch ex As Exception
                If ex.Equals(NoBankException) Then
                    ToConsole("Unable to Pay " & tipillo.ID & " because he has no bank!", ConsoleColor.DarkRed)
                    ToLog("ERROR: Failed to pay " & tipillo.ID & "'s Income because he has no bank!")
                    ToErrorList("W", tipillo.ID & " Has no bank")
                Else
                    ErrorToConsole("Unable to pay " & tipillo.ID, ex)
                    ToLog("ERROR: Failed to pay " & tipillo.ID & "'s Income." & vbNewLine & ex.StackTrace)
                    ToErrorList("E", tipillo.ID & " Could not be paid")
                End If
            End Try
        Next
    End Sub

    Private Shared Sub ToErrorList(ErrorType As String, ErrorMessage As String)
        ErrorList.Add(ErrorType & ":" & ErrorMessage)
    End Sub

    Private Shared Sub ToLog(message As String)
        FileOpen(50, "IMEXLOG.log", OpenMode.Append)
        PrintLine(50, message)
        FileClose(50)
    End Sub

    Private Sub LogHeader()
        FileOpen(50, "IMEXLOG.log", OpenMode.Append)
        PrintLine(50, ":::::IMEX WAS STARTED ON " & DateTime.Now.ToString & ":::::")
        FileClose(50)
    End Sub

    Public Function getName() As String Implements ISmokeSignalExtension.GetName
        Return EXTENSION_NAME
    End Function

    Public Function getVersion() As String Implements ISmokeSignalExtension.GetVersion
        Return EXTENSION_VERS
    End Function
End Class
