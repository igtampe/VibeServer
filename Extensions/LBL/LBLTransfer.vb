Imports Utils
Public Class LBLTransfer

    Public Filename As String
    Public ID As Integer
    Public Directory As String

    ''' <summary>
    ''' Create a new transfer with the specified ID and to the specified filename
    ''' </summary>
    ''' <param name="ID">ID of this transfer</param>
    ''' <param name="Filename">Name of the file (IE Me.csv)</param>
    ''' <param name="Directory">Directory to save it (AS "C:\IIS\FILES")</param>
    Public Sub New(ID As Integer, Filename As String, Directory As String)
        Me.ID = ID
        Me.Filename = Filename
        Me.Directory = Directory
        If IO.File.Exists(Directory & "\" & Filename) Then IO.File.Delete(Directory & "\" & Filename)
    End Sub

    ''' <summary>
    ''' Add this to the file within this transfer
    ''' </summary>
    ''' <param name="What"></param>
    Public Sub Append(What As String)
        AddToFile(Directory & "\" & Filename, What)
    End Sub

    Public Sub Close()
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
