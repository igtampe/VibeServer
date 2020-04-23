Imports System.IO
''' <summary>This module holds all configurable data for this SmokeSigal Server</summary>
Module Config

    'Default IP and Port
    Public IP As String = "127.0.0.1"
    Public Port As Integer = 757

    'SERVER SETUP
    Public Const SERVER_NAME As String = "ViBE Server"
    Public Const SERVER_VERSION As String = "7.0"
    Public Const HEADER_BACK_COLOR As ConsoleColor = ConsoleColor.DarkCyan
    Public Const HEADER_FONT_COLOR As ConsoleColor = ConsoleColor.White

    'Ohter Global Variables
    Public UMSWEB_DIR As String
    Public WEB_DIR As String
    Public VAuthenticator As ViBEAuthenticator


    Public Sub RegisterGlobalVariables()
        'Get ViBE Global Variables
        If (File.Exists("Settings1.cfg")) Then
            'Set Settings
            Dim Settings As String() = ReadFromFile("Settings1.cfg").Split(",")
            UMSWEB_DIR = Settings(1)
            WEB_DIR = Settings(2)
            FileClose(1)
        Else
            ToFile("Settings1.cfg", IP & "," & UMSWEB_DIR & "," & WEB_DIR)
            ToConsole("Could Not Find Settings.cfg in current directory, rendered default one", ConsoleColor.Yellow)
        End If

        VAuthenticator = New ViBEAuthenticator()

    End Sub

    Public Sub RegisterAuthenticator()

        'Register your authenticator here.
        Authenticator = VAuthenticator

    End Sub

    Public Sub RegisterAllExtensions()

        Dim ViBESuperExtension = New ViBEExtension()
        Dim IMEX As IMEX = New IMEX()

        'Add your extensions. When creating the extension, the extension should initialize
        Extensions = {ViBESuperExtension, New LBL(), IMEX}
        AuthenticatedExtensions = {ViBESuperExtension, IMEX}

    End Sub



End Module
