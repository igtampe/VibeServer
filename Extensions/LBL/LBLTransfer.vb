Imports System.Collections
Imports Utils
Public Class LBLTransfer

    Public Filename As String
    Public ID As Integer
    Public Directory As String
    Public Type As TransferType
    Public AllLines As Queue
    Public Enum TransferType As Integer
        Receive
        Send
    End Enum

    Public Shared LBLAlreadyDoneException As Exception = New Exception("The File is Done")

    ''' <summary>
    ''' Create a new transfer with the specified ID and to the specified filename
    ''' </summary>
    ''' <param name="ID">ID of this transfer</param>
    ''' <param name="Filename">Name of the file (IE Me.csv)</param>
    ''' <param name="Directory">Directory to save it (AS "C:\IIS\FILES")</param>
    Public Sub New(ID As Integer, Filename As String, Directory As String, Type As TransferType)
        Me.ID = ID
        Me.Filename = Filename
        Me.Directory = Directory
        Me.Type = Type
        If Type = TransferType.Send Then
            'Open and read all the contents of the file. Add them to a queue to send
            FileOpen(1, Directory & "\" & Filename, OpenMode.Input)
            AllLines = New Queue
            While Not EOF(1)
                AllLines.Enqueue(LineInput(1))
            End While
            FileClose(1)
        Else
            If IO.File.Exists(Directory & "\" & Filename) Then IO.File.Delete(Directory & "\" & Filename)
        End If

    End Sub

    ''' <summary>
    ''' Add this to the file within this transfer
    ''' </summary>
    ''' <param name="What"></param>
    Public Sub Append(What As String)
        If Not Type = TransferType.Receive Then Throw New ArgumentException()
        AddToFile(Directory & "\" & Filename, What)
    End Sub

    Public Function GetLine() As String
        If Not Type = TransferType.Send Then Throw New ArgumentException()
        If AllLines.Count = 0 Then Throw LBLAlreadyDoneException
        Return AllLines.Dequeue()
    End Function

    Public Sub Close()
        If Type = TransferType.Send Then AllLines = Nothing

        ID = Nothing
        Filename = Nothing

    End Sub

    Public Overrides Function Equals(obj As Object) As Boolean
        If obj.GetType() Is GetType(LBLTransfer) Then
            Dim TheOtherOne As LBLTransfer = TryCast(obj, LBLTransfer)
            Return (TheOtherOne.ID = ID) And (TheOtherOne.Filename = Filename)
        End If
        Return False
    End Function

    Public Overrides Function ToString() As String
        Return Filename & " (ID: " & ID & ")"
    End Function



End Class
