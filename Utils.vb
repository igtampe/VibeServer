﻿''' <summary>
''' General Utilities for the ViBE Server
''' </summary>
Public Class Utils

    ''' <summary>
    ''' Prints the requested text to the console
    ''' </summary>
    ''' <param name="ConsoleMSG"></param>
    Public Shared Sub ToConsole(ByVal ConsoleMSG As String)
        Console.WriteLine("[" & DateTime.Now.ToString & "] " & ConsoleMSG)
    End Sub

    ''' <summary>
    ''' Prints the requested text to the console
    ''' </summary>
    ''' <param name="ConsoleMSG">Message</param>
    ''' <param name="Ex">Exception</param>
    Public Shared Sub ErrorToConsole(ByVal ConsoleMSG As String, Ex As Exception)
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("-------------------------------------------------------------------------------")
        Console.ForegroundColor = ConsoleColor.Black
        Console.BackgroundColor = ConsoleColor.Red
        Console.WriteLine("ERROR: " & ConsoleMSG)
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("-------------------------------------------------------------------------------")
        Console.WriteLine(Ex.StackTrace)
        Console.WriteLine("-------------------------------------------------------------------------------")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    ''' <summary>
    ''' Overwrites the specified file with the specified text
    ''' </summary>
    ''' <param name="path">Path to the File</param>
    ''' <param name="what">Text to overwrite with</param>
    Public Shared Sub ToFile(ByVal path As String, ByVal what As String)
        FileOpen(50, path, OpenMode.Output)
        PrintLine(50, what)
        FileClose(50)
    End Sub

    ''' <summary>
    ''' Appends the specified text to the specified file
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <param name="What"></param>
    Public Shared Sub AddToFile(Path As String, What As String)
        FileOpen(50, Path, OpenMode.Append)
        PrintLine(50, What)
        FileClose(50)
    End Sub

    ''' <summary>
    ''' Returns the first line from a file
    ''' </summary>
    ''' <param name="Path"></param>
    Public Shared Function ReadFromFile(Path As String) As String
        FileOpen(50, Path, OpenMode.Input)
        Dim TheReturn As String = LineInput(50)
        FileClose(50)
        Return TheReturn
    End Function

    ''' <summary>
    ''' Sets the color of the console
    ''' </summary>
    ''' <param name="FG"></param>
    ''' <param name="BG"></param>
    Public Shared Sub Color(FG As ConsoleColor, Optional BG As ConsoleColor = ConsoleColor.Black)
        Console.ForegroundColor = FG
        Console.BackgroundColor = BG
    End Sub

End Class
