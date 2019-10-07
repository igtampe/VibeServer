Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports VibeServer.My
Namespace VibeServer
    Public Module Module1


        <MethodImpl(MethodImplOptions.NoInlining Or MethodImplOptions.NoOptimization)>
        <STAThread>
        Public Sub Main()
            Dim IP As String
            Dim UMSWEBDir As String
            Dim WEBDir As String
            Dim str2 As String
            Dim str3 As String
            Dim str4 As String
            Dim str5 As String
            Dim num As Long
            Dim str6 As String
            Dim str7 As String
            Dim str8 As String
            Dim num1 As Long
            Dim str9 As String
            Dim [integer] As Integer
            Dim str10 As String = Nothing
            Dim str11 As String
            Dim str12 As String
            Dim str13 As String
            Dim str14 As String
            Dim EIC As Long
            Console.WriteLine("Visual Basic Economy Server [Version 2.8]")
            Console.WriteLine("(c)2019 Igtampe, No Rights Reserved.")
            Console.WriteLine("")




            Console.WriteLine("")
            Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Starting Server"))
            If (File.Exists("Settings1.cfg")) Then
                FileOpen(1, "Settings1.cfg", OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                Dim str15 As String = LineInput(1).Replace("""", "")
                Dim strArrays As String() = str15.Split(New Char() {","c})
                IP = strArrays(0)
                UMSWEBDir = strArrays(1)
                WEBDir = strArrays(2)
                FileClose(New Integer() {1})
            Else
                IP = "127.0.0.1"
                UMSWEBDir = "A:\MARSH"
                WEBDir = "A:\MARSH"
                FileOpen(1, "Settings1.cfg", OpenMode.Output, OpenAccess.[Default], OpenShare.[Default], -1)
                WriteLine(1, New Object() {IP, UMSWEBDir, WEBDir})
                FileClose(New Integer() {1})
                Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Could Not Find Settings.cfg in current directory, rendered default one"))
            End If
            Dim tcpListener As TcpListener = New TcpListener(IPAddress.Parse(IP), 757)
            Dim tcpClient As TcpClient = New TcpClient()
            tcpListener.Start()
            Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Server Started"))
            Dim ClientMSG As String = ""
            While CObj(ClientMSG) <> CObj("EXIT")
                Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Waiting for connection..."))
                Dim networkStream As NetworkStream = New NetworkStream(tcpListener.AcceptSocket())
                Dim binaryWriter As BinaryWriter = New BinaryWriter(networkStream)
                Dim binaryReader As BinaryReader = New BinaryReader(networkStream)
                Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Connected! Waiting for string..."))

                Try
                    ClientMSG = binaryReader.ReadString().Trim()
                Catch ex As Exception
                    ToConsole("Could not read string for some reason.")
                    GoTo Restart
                End Try

                Console.WriteLine(String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] Received (", ClientMSG, ")"}))
                If (ClientMSG = "CONNECTED") Then
                    ToConsole("Classic Packet, replied.")
                    binaryWriter.Write("You've connected to the server! Congrats.")
                ElseIf (ClientMSG = "NUMBER") Then
                    ToConsole("Classic Packet, replied.")
                    binaryWriter.Write("Random number generator was broken today")
                ElseIf (ClientMSG = "FACT") Then
                    ToConsole("Classic Packet, replied.")
                    binaryWriter.Write("AAAAAAAAAAAAAAAAA")
                ElseIf (ClientMSG.StartsWith("CU")) Then
                    Dim str17 As String = ClientMSG.Remove(0, 2)
                    Try
                        str2 = str17.Remove(5, 4)
                        str3 = str17.Remove(0, 5)
                    Catch exception As System.Exception
                        ProjectData.SetProjectError(exception)
                        binaryWriter.Write("1")
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Improperly Coded Check User Request"))
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    Console.WriteLine(String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] Checking User (", str2, ") with pin (", str3, "), (", str17, ")"}))
                    If (File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", str2, "\pin.dll"))) Then
                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", str2, "\pin.dll"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                        Dim str18 As String = FileSystem.LineInput(1)
                        FileSystem.FileClose(New Integer() {1})
                        If (Operators.CompareString(str18, str3, False) = 0) Then
                            binaryWriter.Write("3")
                            Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Pin is correct! User has logged in."))
                        Else
                            binaryWriter.Write("2")
                            Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Pin is Incorrect."))
                        End If
                    Else
                        binaryWriter.Write("1")
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Could not find user"))
                    End If
                ElseIf (ClientMSG.StartsWith("SM")) Then
                    Try
                        Dim str19 As String = ClientMSG.Remove(0, 2)
                        Dim length As Integer = str19.Length - 22
                        str4 = str19.Remove(11, 11 + length)
                        str5 = str19.Remove(0, 11)
                        str5 = str5.Remove(11, length)
                        num = Conversions.ToLong(str19.Remove(0, 22))
                    Catch exception1 As System.Exception
                        ProjectData.SetProjectError(exception1)
                        binaryWriter.Write("1")
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Improperly Coded Vibing request"))
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    Console.WriteLine(String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] Transfering (", Conversions.ToString(num), ") from (", str4, ") to (", str5, ")"}))
                    Try
                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", str4, "\Balance.dll"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                        Dim num2 As Long = Conversions.ToLong(FileSystem.LineInput(1))
                        FileSystem.FileClose(New Integer() {1})
                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", str5, "\Balance.dll"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                        Dim num3 As Long = Conversions.ToLong(FileSystem.LineInput(1))
                        FileSystem.FileClose(New Integer() {1})
                        num2 = num2 - num
                        num3 = num3 + num

                        'Num2 is Origina Balance
                        'Num3 is destination balance
                        'num is the amount
                        'str5 is the desitnation account. This is the one that needs the income logged

                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", str4, "\balance.dll"), OpenMode.Output, OpenAccess.[Default], OpenShare.[Default], -1)
                        FileSystem.WriteLine(1, New Object() {num2})
                        FileSystem.FileClose(New Integer() {1})
                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", str5, "\balance.dll"), OpenMode.Output, OpenAccess.[Default], OpenShare.[Default], -1)
                        FileSystem.WriteLine(1, New Object() {num3})
                        FileSystem.FileClose(New Integer() {1})

                        Dim STR5ID = str5.Remove(5, 6)

                        Try
                            FileOpen(101, UMSWEBDir & "\SSH\USERS\" & STR5ID & "\EI.dll", OpenMode.Input)
                            EIC = LineInput(101)
                            FileClose(101)
                        Catch
                            FileClose(101)
                            EIC = 0
                        End Try
                        Call ToConsole("Adding (" & num & ") to (" & STR5ID & ")'s pre-existing Extra income of (" & EIC & ")")
                        EIC = EIC + num
                        ToConsole("Added, opening the file again.")

                        FileOpen(101, UMSWEBDir & "\SSH\USERS\" & STR5ID & "\EI.dll", OpenMode.Output)
                        FileSystem.WriteLine(101, EIC)
                        FileClose(101)

                        ToConsole("Added it, writing to log1")


                        MyProject.Computer.FileSystem.WriteAllText(String.Concat(UMSWEBDir, "\SSH\users\", str4, "\log.log"), String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] You ~vibed~ ", Conversions.ToString(num), "p to ", str5, "" & vbCrLf & ""}), True)
                        ToConsole("Writing log2")
                        MyProject.Computer.FileSystem.WriteAllText(String.Concat(UMSWEBDir, "\SSH\users\", str5, "\log.log"), String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] ", str4, " ~vibed~ ", Conversions.ToString(num), "p to you." & vbCrLf & ""}), True)

                        ToConsole("Writing to Notif")

                        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\users\", str5, "\..\notifs.txt"), OpenMode.Append)
                        PrintLine(1, DateTime.Now.ToString & "`" & str4 & " Sent " & num.ToString("N0") & "p to your " & str5.Remove(0, 6) & " account")
                        FileClose(1)


                    Catch exception2 As System.Exception
                        ProjectData.SetProjectError(exception2)
                        binaryWriter.Write("E")
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Couldnt Finish Money Transfer."))
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    binaryWriter.Write("S")


                ElseIf (ClientMSG.StartsWith("TM")) Then
                    Dim num4 As Integer = 0
                    Try
                        num4 = 1
                        Dim str20 As String = ClientMSG.Remove(0, 2)
                        num4 = 2
                        Dim length1 As Integer = str20.Length
                        num4 = 3
                        Dim num5 As Integer = length1 - 15
                        num4 = 4
                        str6 = str20.Remove(5, 10 + num5)
                        num4 = 5
                        str7 = str20.Remove(0, 5)
                        num4 = 6
                        str7 = str7.Remove(5, 5 + num5)
                        num4 = 7
                        str8 = str20.Remove(0, 10)
                        num4 = 8
                        str8 = str8.Remove(5, num5)
                        num4 = 9
                        num1 = Conversions.ToLong(str20.Remove(0, 15))
                    Catch exception3 As System.Exception
                        ProjectData.SetProjectError(exception3)
                        binaryWriter.Write("1")
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Improperly Coded Transfer request. Stuck on Stage ", Conversions.ToString(num4)))
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    Console.WriteLine(String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] Transfering (", Conversions.ToString(num1), ") from (", str7, ") to (", str8, ") of (", str6, ")"}))
                    Try
                        num4 = 0
                        FileSystem.FileOpen(1, UMSWEBDir & "\SSH\USERS\" & str6 & "\" & str7 & "\Balance.dll", OpenMode.Input, OpenAccess.[Default])
                        num4 = 1
                        Dim num6 As Long = Conversions.ToLong(FileSystem.LineInput(1))
                        num4 = 2
                        FileSystem.FileClose(New Integer() {1})
                        num4 = 3
                        FileSystem.FileOpen(1, String.Concat(New String() {UMSWEBDir, "\SSH\USERS\", str6, "\", str8, "\Balance.dll"}), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                        num4 = 4
                        Dim num7 As Long = Conversions.ToLong(FileSystem.LineInput(1))
                        num4 = 5
                        FileSystem.FileClose(New Integer() {1})
                        num4 = 6
                        num6 = num6 - num1
                        num4 = 7
                        num7 = num7 + num1
                        num4 = 8
                        FileSystem.FileOpen(1, String.Concat(New String() {UMSWEBDir, "\SSH\USERS\", str6, "\", str7, "\balance.dll"}), OpenMode.Output, OpenAccess.[Default], OpenShare.[Default], -1)
                        num4 = 9
                        FileSystem.WriteLine(1, New Object() {num6})
                        num4 = 10
                        FileSystem.FileClose(New Integer() {1})
                        num4 = 11
                        FileSystem.FileOpen(1, String.Concat(New String() {UMSWEBDir, "\SSH\USERS\", str6, "\", str8, "\balance.dll"}), OpenMode.Output, OpenAccess.[Default], OpenShare.[Default], -1)
                        num4 = 12
                        FileSystem.WriteLine(1, New Object() {num7})
                        num4 = 13
                        FileSystem.FileClose(New Integer() {1})
                        num4 = 14
                        MyProject.Computer.FileSystem.WriteAllText(String.Concat(New String() {UMSWEBDir, "\SSH\users\", str6, "\", str7, "\log.log"}), String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] You ~vibed~ ", Conversions.ToString(num1), "p to ", str8, "" & vbCrLf & ""}), True)
                        num4 = 15
                        MyProject.Computer.FileSystem.WriteAllText(String.Concat(New String() {UMSWEBDir, "\SSH\users\", str6, "\", str8, "\log.log"}), String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] You ~vibed~ ", Conversions.ToString(num1), "p from ", str7, "" & vbCrLf & ""}), True)
                        num4 = 16
                    Catch exception4 As System.Exception
                        ProjectData.SetProjectError(exception4)
                        binaryWriter.Write("E")
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Couldnt Finish Money Transfer. Stuck on Stage " & num4.ToString))
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    binaryWriter.Write("S")
                ElseIf (ClientMSG.StartsWith("CP")) Then
                    Dim str21 As String = ClientMSG.Remove(0, 2)
                    Try
                        str9 = str21.Remove(5, 4)
                        [integer] = Conversions.ToInteger(str21.Remove(0, 5))
                    Catch exception5 As System.Exception
                        ProjectData.SetProjectError(exception5)
                        binaryWriter.Write("1")
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Improperly Coded Check User Request"))
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    Try
                        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", str9, "\PIN.dll"), OpenMode.Output, OpenAccess.[Default], OpenShare.[Default], -1)
                        FileSystem.WriteLine(1, New Object() {[integer]})
                        FileClose(New Integer() {1})
                    Catch exception6 As System.Exception
                        ProjectData.SetProjectError(exception6)
                        binaryWriter.Write("2")
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Could not complete pin change"))
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    Console.WriteLine(String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] Changing Pin of (", str9, ") with (", Conversions.ToString([integer]), "), (", str21, ")"}))
                    binaryWriter.Write("S")
                ElseIf (ClientMSG.StartsWith("INFO")) Then
                    Try
                        str10 = ClientMSG.Remove(0, 4)
                    Catch exception7 As System.Exception
                        ProjectData.SetProjectError(exception7)
                        binaryWriter.Write("E")
                        Console.WriteLine(String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] Improperly Coded Check User Request (", str10, ")"}))
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    Dim str22 As String = Conversions.ToString(0)
                    Dim str23 As String = Conversions.ToString(0)
                    Dim str24 As String = Conversions.ToString(0)
                    Try
                        FileSystem.FileClose(New Integer() {1})
                    Catch exception8 As System.Exception
                        ProjectData.SetProjectError(exception8)
                        ProjectData.ClearProjectError()
                    End Try
                    FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", str10, "\name.dll"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                    Dim str25 As String = FileSystem.LineInput(1)
                    FileSystem.FileClose(New Integer() {1})
                    Dim str26 As String = Conversions.ToString(0)
                    Dim str27 As String = Conversions.ToString(0)
                    Dim str28 As String = Conversions.ToString(0)
                    If (File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", str10, "\UMSNB\Balance.dll"))) Then
                        str26 = Conversions.ToString(1)
                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", str10, "\UMSNB\Balance.dll"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                        str22 = FileSystem.LineInput(1)
                        FileSystem.FileClose(New Integer() {1})
                    End If
                    If (File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", str10, "\GBANK\Balance.dll"))) Then
                        str27 = Conversions.ToString(1)
                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", str10, "\GBANK\Balance.dll"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                        str23 = FileSystem.LineInput(1)
                        FileSystem.FileClose(New Integer() {1})
                    End If
                    If (File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", str10, "\RIVER\Balance.dll"))) Then
                        str28 = Conversions.ToString(1)
                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", str10, "\RIVER\Balance.dll"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                        str24 = FileSystem.LineInput(1)
                        FileSystem.FileClose(New Integer() {1})
                    End If

                    ToConsole("Attempting to find any notifications...")
                    Dim notifs As String

                    Try
                        notifs = File.ReadAllLines(String.Concat(UMSWEBDir, "\SSH\USERS\", str10, "\notifs.txt")).Length
                        ToConsole("Found " & notifs & " notification(s)")
                    Catch ex As Exception
                        notifs = 0
                        ToConsole("Found no notification file")
                    End Try

                    Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Sending info... ", str10))
                    binaryWriter.Write(String.Concat(New String() {str26, ",", str22, ",", str27, ",", str23, ",", str28, ",", str24, ",", str25, ",", notifs}))


                ElseIf ClientMSG.StartsWith("NOTIF") Then
                    Try
                        Dim notifmsg As String = ClientMSG.Replace("NOTIF", "")

                        ToConsole("invoking Notif System")

                        If notifmsg.StartsWith("READ") Then
                            notifmsg = notifmsg.Replace("READ", "")
                            Dim notifarray() As String
                            ToConsole("trying to READ from " & notifmsg & "'s messages")
                            If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", notifmsg, "\notifs.txt")) Then
                                binaryWriter.Write("N")
                                Exit Try
                            End If

                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", notifmsg, "\notifs.txt"), OpenMode.Input)
                            Dim I As Integer = 0
                            While Not EOF(1)
                                ReDim Preserve notifarray(I)
                                notifarray(I) = LineInput(1)
                                ToConsole("Found Notif " & I)
                                I = I + 1
                            End While
                            FileClose(1)
                            binaryWriter.Write(String.Join("`", notifarray))
                        ElseIf notifmsg.StartsWith("CLEAR") Then
                            Dim Notifuser As String = notifmsg.Replace("CLEAR", "")
                            '57174
                            ToConsole("Attempting to remove all of " & Notifuser & "'s notifications")
                            Try
                                File.Delete(String.Concat(UMSWEBDir, "\SSH\USERS\", Notifuser, "\notifs.txt"))
                                binaryWriter.Write("S")
                                ToConsole("OK I did it yay")
                            Catch
                                binaryWriter.Write("E")
                                ToConsole("Oh no something happened")
                            End Try

                        ElseIf notifmsg.StartsWith("REMO") Then
                            notifmsg = notifmsg.Replace("REMO", "")
                            '5717410
                            Dim notifindex As String = notifmsg.Remove(0, 5)
                            Dim notifuser As String = notifmsg.Remove(5, notifindex.Length)

                            If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\notifs.txt")) Then
                                binaryWriter.Write("N")
                                Exit Try
                            End If
                            ToConsole("Trying to remove index " & notifindex & " from " & notifuser & "'s notification file")
                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\notifs.txt"), OpenMode.Input)
                            FileOpen(2, String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\tempnotifs.txt"), OpenMode.Output)
                            Dim I As Integer = 0
                            While Not EOF(1)
                                If I = notifindex Then
                                    LineInput(1)
                                    GoTo notifskipwhile
                                End If
                                ToConsole("Copying Index " & I)
                                PrintLine(2, LineInput(1))
notifskipwhile:
                                I = I + 1
                            End While
                            FileClose(1)
                            FileClose(2)
                            ToConsole("finishing up")


                            File.Delete(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\notifs.txt"))
                            File.Move(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\tempnotifs.txt"), String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\notifs.txt"))
                            binaryWriter.Write("S")



                        Else
                            binaryWriter.Write("E")
                        End If

                    Catch e As Exception
                        ToConsole(e.ToString)
                        binaryWriter.Write("E")

                    End Try


                ElseIf (ClientMSG.StartsWith("UMSNB")) Then
                    Dim [single] As Single = Conversions.ToSingle(ClientMSG.Remove(0, 5))
                    Try
                        [single] = Conversions.ToSingle(ClientMSG.Remove(0, 5))
                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", Conversions.ToString([single]), "\UMSNB\Balance.dll"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                        str11 = FileSystem.LineInput(1)
                        FileSystem.FileClose(New Integer() {1})
                    Catch exception9 As System.Exception
                        ProjectData.SetProjectError(exception9)
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Unwilling to find balance of UMSNB"))
                        binaryWriter.Write("E")
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Sending UMSNB Balance"))
                    binaryWriter.Write(str11)
                ElseIf (ClientMSG.StartsWith("GBANK")) Then
                    Try
                        Dim single1 As Single = Conversions.ToSingle(ClientMSG.Remove(0, 5))
                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", Conversions.ToString(single1), "\GBANK\Balance.dll"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                        str12 = FileSystem.LineInput(1)
                        FileSystem.FileClose(New Integer() {1})
                    Catch exception10 As System.Exception
                        ProjectData.SetProjectError(exception10)
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Unwilling to find balance of GBANK"))
                        binaryWriter.Write("E")
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Sending GBANK Balance"))
                    binaryWriter.Write(str12)
                ElseIf (ClientMSG.StartsWith("RIVER")) Then
                    Dim single2 As Single = Conversions.ToSingle(ClientMSG.Remove(0, 5))
                    Try
                        single2 = Conversions.ToSingle(ClientMSG.Remove(0, 5))
                        FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", Conversions.ToString(single2), "\RIVER\Balance.dll"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                        str13 = FileSystem.LineInput(1)
                        FileSystem.FileClose(New Integer() {1})
                    Catch exception11 As System.Exception
                        ProjectData.SetProjectError(exception11)
                        Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Unwilling to find balance of RIVER"))
                        binaryWriter.Write("E")
                        ProjectData.ClearProjectError()
                        GoTo Restart
                    End Try
                    Console.WriteLine(String.Concat("[", Conversions.ToString(DateTime.Now), "] Sending RIVER Balance"))
                    binaryWriter.Write(str13)

                ElseIf ClientMSG = "DIR" Then
                    ToConsole("Time to debug this shit")
                    FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\INCOMEMAN\UserList.isf"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                    Dim num8 As Integer = 1
                    Dim directoryarray() As String
                    While Not FileSystem.EOF(1)
                        str14 = FileSystem.LineInput(1)
                        If (str14.StartsWith("USER")) Then
                            ReDim Preserve directoryarray(num8 - 1)
                            directoryarray(num8 - 1) = str14.Replace("USER" & num8 & ":", "")

                            FileOpen(2, String.Concat(UMSWEBDir, "\SSH\USERS\", directoryarray(num8 - 1), "\NAME.DLL"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                            directoryarray(num8 - 1) = directoryarray(num8 - 1) & ": " & FileSystem.LineInput(2)
                            FileClose(2)

                            ToConsole("Found USER " & num8 & " with ID and name " & directoryarray(num8 - 1))

                            num8 = num8 + 1

                        End If
                    End While
                    FileClose(1)
                    ToConsole("Opening the Corporate Accounts")

                    Dim NTNum As Integer = 1

                    FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\INCOMEMAN\Corporate.isf"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                    While Not FileSystem.EOF(1)
                        str14 = FileSystem.LineInput(1)
                        If (str14.StartsWith("USER")) Then
                            ReDim Preserve directoryarray(num8 - 1)
                            directoryarray(num8 - 1) = str14.Replace("USER" & NTNum & ":", "")

                            FileOpen(2, String.Concat(UMSWEBDir, "\SSH\USERS\", directoryarray(num8 - 1), "\NAME.DLL"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                            directoryarray(num8 - 1) = directoryarray(num8 - 1) & ": " & FileSystem.LineInput(2)
                            FileClose(2)

                            ToConsole("Found USER " & NTNum & " with ID and name " & directoryarray(num8 - 1))
                            NTNum = NTNum + 1

                            num8 = num8 + 1

                        End If
                    End While

                    FileClose(1)

                    ToConsole("Opening the Nontaxed Accounts")

                    NTNum = 1

                    FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\INCOMEMAN\NonTaxed.isf"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                    While Not FileSystem.EOF(1)
                        str14 = FileSystem.LineInput(1)
                        If (str14.StartsWith("USER")) Then
                            ReDim Preserve directoryarray(num8 - 1)
                            directoryarray(num8 - 1) = str14.Replace("USER" & NTNum & ":", "")

                            FileOpen(2, String.Concat(UMSWEBDir, "\SSH\USERS\", directoryarray(num8 - 1), "\NAME.DLL"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                            directoryarray(num8 - 1) = directoryarray(num8 - 1) & ": " & FileSystem.LineInput(2)
                            FileClose(2)

                            ToConsole("Found USER " & NTNum & " with ID and name " & directoryarray(num8 - 1))
                            NTNum = NTNum + 1

                            num8 = num8 + 1

                        End If
                    End While

                    FileClose(1)


                    ToConsole("Sending it.")
                    binaryWriter.Write(String.Join(",", directoryarray))

                ElseIf ClientMSG.StartsWith("REG") Then

                    Try
                        'REG4640,Igtampe
                        ClientMSG = ClientMSG.Remove(0, 3)
                        Dim SplitValues() As String

                        SplitValues = ClientMSG.Split(",")


                        Dim RegPIN As String = SplitValues(0)
                        Dim RegNAME As String = SplitValues(1)

                        ToConsole("Attempting to add(" & RegNAME & ") with pin (" & RegPIN & ")")


                        Dim regid As String


RedoIDGEN:
                        regid = ""
                        regid = regid & CInt(Math.Ceiling(Rnd() * 9))
                        regid = regid & CInt(Math.Ceiling(Rnd() * 9))
                        regid = regid & CInt(Math.Ceiling(Rnd() * 9))
                        regid = regid & CInt(Math.Ceiling(Rnd() * 9))
                        regid = regid & CInt(Math.Ceiling(Rnd() * 9))

                        ToConsole("Got ID (" & regid & ") Checking if it exists...")
                        If Directory.Exists(UMSWEBDir & "\SSH\USERS\" & regid) Then GoTo RedoIDGEN
                        ToConsole("Good it doesn't lets keep going. Creating the folder.")

                        Directory.CreateDirectory(UMSWEBDir & "\SSH\USERS\" & regid)
                        ToConsole("Creating income file")
                        ToFile(UMSWEBDir & "\SSH\USERS\" & regid & "\INCOME.dll", "0000")
                        ToConsole("Creating Name File")
                        ToFile(UMSWEBDir & "\SSH\USERS\" & regid & "\NAME.dll", RegNAME)
                        ToConsole("Creating PIN File")
                        ToFile(UMSWEBDir & "\SSH\USERS\" & regid & "\PIN.dll", RegPIN)

                        If RegNAME.EndsWith(" (Corp.)") Then
                            ToConsole("Attempting to add him to the Corporate.ISF")
                            FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\INCOMEMAN\Corporate.isf"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                            Dim N As Integer = 1
                            While Not FileSystem.EOF(1)
                                str14 = FileSystem.LineInput(1)
                                If (str14.StartsWith("USER")) Then
                                    N = N + 1
                                    ToConsole("Found record (" & N & ") which is (" & str14 & ")")
                                End If
                            End While
                            FileClose(1)
                            ToConsole("Adding as record " & N)
                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\INCOMEMAN\Corporate.isf"), OpenMode.Append)
                            PrintLine(1, "USER" & N & ":" & regid)
                            FileClose(1)
                            ToConsole("Added successfully!")
                        Else
                            ToConsole("Attempting to add him to the USERLIST.ISF")
                            FileSystem.FileOpen(1, String.Concat(UMSWEBDir, "\SSH\INCOMEMAN\UserList.isf"), OpenMode.Input, OpenAccess.[Default], OpenShare.[Default], -1)
                            Dim N As Integer = 1
                            While Not FileSystem.EOF(1)
                                str14 = FileSystem.LineInput(1)
                                If (str14.StartsWith("USER")) Then
                                    N = N + 1
                                    ToConsole("Found record (" & N & ") which is (" & str14 & ")")
                                End If
                            End While
                            FileClose(1)
                            ToConsole("Adding as record " & N)
                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\INCOMEMAN\UserList.isf"), OpenMode.Append)
                            PrintLine(1, "USER" & N & ":" & regid)
                            FileClose(1)
                            ToConsole("Added successfully!")
                        End If


                        binaryWriter.Write(regid)
                        GoTo Restart

                    Catch
                        ToConsole("Shoot I couldn't do that.")
                        binaryWriter.Write("E")
                        GoTo Restart
                    End Try




                ElseIf ClientMSG.StartsWith("BNK") Then
                    ToConsole("BNK Tools invoked")
                    'BNKA57174GBANK
                    Dim BNKMSG As String = ClientMSG.Remove(0, 3)
                    If Not BNKMSG.Count = 11 Then
                        ToConsole("Something seems fishy, I'm not going to do it.")
                        binaryWriter.Write("E")
                        GoTo Restart
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
                                FileOpen(3, UMSWEBDir & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\Balance.dll", OpenMode.Input)
                                Dim BNKBAL As Long = LineInput(3)
                                ToConsole("Bank balance is currently (" & BNKBAL & ")")
                                FileClose(3)
                                If Not BNKBAL = "0" Then
                                    ToConsole("Looks to me like it's not 0. Stopping.")
                                    binaryWriter.Write("E")
                                    GoTo Restart
                                End If

                                ToConsole("Deleting Folder")
                                Directory.Delete(UMSWEBDir & "\SSH\USERS\" & BNKID & "\" & BNKBNK, True)
                                ToConsole("KK Done!")
                                binaryWriter.Write("S")
                                GoTo Restart

                            Catch
                                ToConsole("Shoot I couldn't do that.")
                                binaryWriter.Write("E")
                                GoTo Restart
                            End Try


                            'close
                        Case "O"

                            Try
                                ToConsole("Attempting to open (" & BNKID & ")'s (" & BNKBNK & ") account.")
                                Directory.CreateDirectory(UMSWEBDir & "\SSH\USERS\" & BNKID & "\" & BNKBNK)
                                Directory.CreateDirectory(UMSWEBDir & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\CHECKS")

                                FileOpen(1, UMSWEBDir & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\BALANCE.dll", OpenMode.Output)
                                PrintLine(1, "0000")
                                FileClose(1)

                                FileOpen(1, UMSWEBDir & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\Log.log", OpenMode.Output)
                                PrintLine(1, "[" & DateTime.Now.ToString & "] Account Created on ViBE")
                                FileClose(1)
                                binaryWriter.Write("S")
                                GoTo Restart

                            Catch
                                ToConsole("Shoot I couldn't do that.")
                                binaryWriter.Write("E")
                                GoTo Restart
                            End Try

                            'open
                        Case "L"
                            Try
                                ToConsole("Copying " & UMSWEBDir & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\Log.log to " & WEBDir & "\LOGS\" & BNKID & BNKBNK & ".log...")
                                File.Copy(UMSWEBDir & "\SSH\USERS\" & BNKID & "\" & BNKBNK & "\Log.log", WEBDir & "\LOGS\" & BNKID & BNKBNK & ".log", True)
                                ToConsole("Done!")
                                binaryWriter.Write("S")
                                GoTo Restart
                            Catch ex As Exception
                                ToConsole("Shoot I couldn't do that.")
                                binaryWriter.Write("E")
                                GoTo Restart
                            End Try

                            'Log

                        Case Else
                            binaryWriter.Write("E")
                            GoTo Restart
                    End Select

                ElseIf ClientMSG.StartsWith("CERT") Then
                    Try
                        Dim Certify As String = ClientMSG.Remove(0, 4)
                        FileOpen(1, WEBDir & "\Certifications.log", OpenMode.Append)
                        PrintLine(1, "{Certified on: " & DateTime.Now.ToString & "} " & Certify)
                        FileClose(1)
                        binaryWriter.Write("S")

                        GoTo Restart
                    Catch
                        binaryWriter.Write("E")

                    End Try

                ElseIf ClientMSG.StartsWith("CHCKBK") Then
                    Try
                        Dim chckmsg As String = ClientMSG.Replace("CHCKBK", "")

                        ToConsole("invoking Checkbook 2000 System")

                        If chckmsg.StartsWith("READ") Then
                            chckmsg = chckmsg.Replace("READ", "")
                            Dim notifarray() As String
                            ToConsole("trying to READ from " & chckmsg & "'s messages")
                            If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", chckmsg, "\chckbk.txt")) Then
                                binaryWriter.Write("N")
                                Exit Try
                            End If
                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", chckmsg, "\chckbk.txt"), OpenMode.Input)
                            Dim I As Integer = 0
                            ReDim notifarray(0)
                            notifarray(0) = "NothingPls"

                            While Not EOF(1)
                                ReDim Preserve notifarray(I)
                                notifarray(I) = LineInput(1)
                                ToConsole("Found Notif " & I)
                                I = I + 1
                            End While

                            FileClose(1)

                            If notifarray(0) = "NothingPls" Then
                                binaryWriter.Write("F")
                                Exit Try
                            End If

                            binaryWriter.Write(String.Join("`", notifarray))

                        ElseIf chckmsg.StartsWith("REMO") Then
                            chckmsg = chckmsg.Replace("REMO", "")
                            '5717410
                            Dim notifindex As String = chckmsg.Remove(0, 5)
                            Dim notifuser As String = chckmsg.Remove(5, notifindex.Length)

                            If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\chckbk.txt")) Then
                                binaryWriter.Write("N")
                                Exit Try
                            End If
                            ToConsole("Trying to remove index " & notifindex & " from " & notifuser & "'s notification file")
                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\chckbk.txt"), OpenMode.Input)
                            FileOpen(2, String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\tempchckbk.txt"), OpenMode.Output)
                            Dim I As Integer = 0
                            While Not EOF(1)
                                If I = notifindex Then
                                    LineInput(1)
                                    GoTo Chckskipwhile
                                End If
                                ToConsole("Copying Index " & I)
                                PrintLine(2, LineInput(1))
Chckskipwhile:
                                I = I + 1
                            End While
                            FileClose(1)
                            FileClose(2)
                            ToConsole("finishing up")


                            File.Delete(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\chckbk.txt"))
                            File.Move(String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\tempchckbk.txt"), String.Concat(UMSWEBDir, "\SSH\USERS\", notifuser, "\chckbk.txt"))
                            binaryWriter.Write("S")

                        ElseIf chckmsg.StartsWith("ADD") Then

                            chckmsg = chckmsg.Replace("ADD", "")
                            Dim ChckWrite As String = chckmsg.Remove(0, 5)
                            Dim Chckuser As String = chckmsg.Remove(5, ChckWrite.Length)
                            Dim chckdetails() As String = ChckWrite.Split("`")

                            ToConsole("Adding '" & ChckWrite & "' to " & Chckuser & "'s chckbck.txt")

                            Try
                                FileOpen(1, String.Concat(UMSWEBDir, "\SSH\users\", Chckuser, "\chckbk.txt"), OpenMode.Append)
                                PrintLine(1, ChckWrite)
                                FileClose(1)

                                ToConsole("Writing to Notif")

                                Select Case chckdetails(0)
                                    Case 0
                                        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\users\", Chckuser, "\notifs.txt"), OpenMode.Append)
                                        '0`12/4/2018 7:42:42 PM`A Test Account`57174\UMSNB`100`This is a Check
                                        PrintLine(1, DateTime.Now.ToString & "`You have a new check from " & chckdetails(2) & " that's worth " & CInt(chckdetails(4)).ToString("N0") & "p")
                                        FileClose(1)

                                    Case 1
                                        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\users\", Chckuser, "\notifs.txt"), OpenMode.Append)
                                        '0`12/4/2018 7:42:42 PM`A Test Account`57174\UMSNB`100`This is a Check
                                        PrintLine(1, DateTime.Now.ToString & "`You have a new bill from " & chckdetails(2) & " that's worth " & CInt(chckdetails(4)).ToString("N0") & "p")
                                        FileClose(1)


                                End Select



                                binaryWriter.Write("S")
                                ToConsole("OK done")
                            Catch ex As Exception
                                binaryWriter.Write("E")
                                ToConsole(ex.ToString)

                            End Try

                        Else
                            binaryWriter.Write("E")
                        End If

                    Catch e As Exception
                        ToConsole(e.ToString)
                        binaryWriter.Write("E")

                    End Try

                ElseIf ClientMSG.StartsWith("NTA") Then
                    ToConsole("Non-Taxed Add Invoked")
                    Dim NTAMSG As String = ClientMSG.Replace("NTA", "")
                    'NTA57174400000000
                    Dim NTAUSR As String = NTAMSG.Remove(5, NTAMSG.Count - 5)
                    Dim NTAAMT As Long = NTAMSG.Remove(0, 5)
                    ToConsole("Adding " & NTAAMT.ToString("N0") & "p to " & NTAUSR & "'s first account.")
                    If Not Directory.Exists(UMSWEBDir & "\SSH\USERS\" & NTAUSR) Then
                        binaryWriter.Write("E")
                        ToConsole("Unable to find user " & NTAUSR)
                        GoTo Restart
                    End If
                    Dim NTABNK As String = "LEMON"
                    If Directory.Exists(UMSWEBDir & "\SSH\USERS\" & NTAUSR & "\UMSNB") Then
                        NTABNK = "UMSNB"
                    ElseIf Directory.Exists(UMSWEBDir & "\SSH\USERS\" & NTAUSR & "\GBANK") Then
                        NTABNK = "GBANK"
                    ElseIf Directory.Exists(UMSWEBDir & "\SSH\USERS\" & NTAUSR & "\RIVER") Then
                        NTABNK = "RIVER"
                    Else
                        binaryWriter.Write("E")
                        ToConsole("User " & NTAUSR & " Has no bank accounts!")
                        GoTo Restart
                    End If

                    Try
                        FileOpen(1, UMSWEBDir & "\SSH\USERS\" & NTAUSR & "\" & NTABNK & "\Balance.dll", OpenMode.Input)
                        Dim NTCB As Long = Conversions.ToLong(FileSystem.LineInput(1))
                        FileClose(1)
                        NTCB = NTCB + NTAAMT


                        FileOpen(1, UMSWEBDir & "\SSH\USERS\" & NTAUSR & "\" & NTABNK & "\Balance.dll", OpenMode.Output)
                        WriteLine(1, NTCB)
                        FileClose(1)
                        ToConsole("Added it, writing to log")


                        MyProject.Computer.FileSystem.WriteAllText(String.Concat(UMSWEBDir, "\SSH\users\", NTAUSR, "\", NTABNK, "\log.log"), String.Concat(New String() {"[", Conversions.ToString(DateTime.Now), "] ASIMOV added ", NTAAMT.ToString("N0"), "p to your account, non-taxed ", "" & vbCrLf & ""}), True)

                        ToConsole("Writing to Notif")

                        FileOpen(1, String.Concat(UMSWEBDir, "\SSH\users\", NTAUSR, "\notifs.txt"), OpenMode.Append)
                        PrintLine(1, DateTime.Now.ToString & "`" & "ASIMoV added" & NTAAMT.ToString("N0") & "p to your " & NTABNK & " account, non-taxed.")
                        FileClose(1)

                        binaryWriter.Write("S")
                    Catch ex As Exception
                        binaryWriter.Write("E")
                        ToConsole("Something went wrong" & vbNewLine & vbNewLine * ex.ToString)
                        GoTo Restart
                    End Try

                ElseIf ClientMSG.StartsWith("EZT") Then

                    Console.WriteLine("[" & DateTime.Now.ToString & "] EZTax Has been invoked")
                    Dim EZTAXMSG As String
                    EZTAXMSG = ClientMSG.Remove(0, 3)

                    'Three Commands:
                    'INF57174                  All Tax Information
                    'ADD57174{NAME,INCOME}   Add the specified entry to record
                    'MOD57174{INDEX,NAME,INCOME}
                    'REM57174{INDEX}         Remove the specified entry to record
                    'UPD57174XXXXX             Update Income of specified User

                    'All tax info is stored in the IGTNET Website (hidden dir)

                    If EZTAXMSG.StartsWith("INF") Then
                        Dim ID As String = EZTAXMSG.Remove(0, 3)
                        Dim Income As Long
                        Dim EI As Long
                        Call ToConsole("Attempting to send Information on user (" & ID & ")")
                        Try
                            FileSystem.FileOpen(1, UMSWEBDir & "\SSH\USERS\" & ID & "\Income.dll", OpenMode.Input)
                            Income = FileSystem.LineInput(1)
                            FileSystem.FileClose(1)

                            Try
                                FileSystem.FileOpen(1, UMSWEBDir & "\SSH\USERS\" & ID & "\EI.dll", OpenMode.Input)
                                EI = FileSystem.LineInput(1)
                                FileSystem.FileClose(1)
                            Catch
                                EI = 0
                            End Try


                            binaryWriter.Write(Income & "," & EI)

                            Call ToConsole("Sent information income: (" & Income & ") and extra income (" & EI & ")")

                        Catch
                            binaryWriter.Write("E")
                            Call ToConsole("Could not retrieve information.")
                        End Try


                        GoTo Restart

                    ElseIf EZTAXMSG.StartsWith("UPD") Then
                        EZTAXMSG = EZTAXMSG.Remove(0, 3)
                        Dim ID As String = EZTAXMSG.Remove(5, EZTAXMSG.Count - 5)
                        Dim NewIncome As Long = EZTAXMSG.Remove(0, 5)

                        If ID.Count = 5 Then
                            Try
                                FileSystem.FileOpen(1, UMSWEBDir & "\SSH\USERS\" & ID & "\Income.dll", OpenMode.Output)
                                FileSystem.PrintLine(1, NewIncome)
                                FileSystem.FileClose(1)
                                binaryWriter.Write("S")

                            Catch
                                binaryWriter.Write("E")
                            End Try

                            FileOpen(1, "IncomeManagementLog.log", OpenMode.Append)
                            PrintLine(1, "[" & DateTime.Now.ToString & "] " & ID & " Has modified their income to be " & NewIncome.ToString("N0"))
                            FileClose(1)

                        End If
                    End If

                ElseIf ClientMSG.StartsWith("CON") Then
                    ToConsole("Invoking Contractus Subsystem...")
                    Dim ConMSG As String
                    ConMSG = ClientMSG.Replace("CON", "")
                    Try

                        If ConMSG.StartsWith("READALL") Then
                            Dim AllContracts() As String
                            ToConsole("trying to read all contracts")
                            If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\Contracts.txt")) Then
                                binaryWriter.Write("N")
                                Exit Try
                            End If

                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Contracts.txt"), OpenMode.Input)
                            Dim I As Integer = 0
                            While Not EOF(1)
                                ReDim Preserve AllContracts(I)
                                AllContracts(I) = LineInput(1)
                                ToConsole("Found Contract " & I)
                                I = I + 1
                            End While
                            FileClose(1)
                            If I = 0 Then
                                binaryWriter.Write("N")
                            Else
                                binaryWriter.Write(String.Join(";", AllContracts))
                            End If

                        ElseIf ConMSG.StartsWith("DETAILS") Then
                            Dim Details = ConMSG.Replace("DETAILS", "")
                            ToConsole("Retrieving details from contract #" & Details)
                            If Not File.Exists(UMSWEBDir & "\SSH\CONTRACTS\" & Details & ".txt") Then
                                binaryWriter.Write("E")
                                ToConsole("Looks like it doesn't exist")
                                GoTo Restart
                            End If

                            Try
                                FileOpen(1, UMSWEBDir & "\SSH\CONTRACTS\" & Details & ".txt", OpenMode.Input)
                                binaryWriter.Write(LineInput(1))
                                FileClose(1)
                                ToConsole("Done time to go adiosito")
                            Catch
                                ToConsole("w o o p s")
                            End Try

                        ElseIf ConMSG.StartsWith("READUSR") Then
                            Dim notifmsg = ConMSG.Replace("READUSR", "")
                            Dim notifarray() As String
                            ToConsole("trying to READ from " & notifmsg & "'s Contracts")
                            If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\USERS\", notifmsg, "\Contracts.txt")) Then
                                binaryWriter.Write("N")
                                Exit Try
                            End If

                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\USERS\", notifmsg, "\Contracts.txt"), OpenMode.Input)
                            Dim I As Integer = 0
                            While Not EOF(1)
                                ReDim Preserve notifarray(I)
                                notifarray(I) = LineInput(1)
                                ToConsole("Found Contract " & I)
                                I = I + 1
                            End While

                            FileClose(1)
                            If I = 0 Then
                                binaryWriter.Write("N")
                            Else
                                binaryWriter.Write(String.Join(";", notifarray))
                            End If


                        ElseIf ConMSG.StartsWith("ADDTOALL") Then
                            ConMSG = ConMSG.Replace("ADDTOALL", "")
                            ToConsole("Attempting to add " & ConMSG & "to all contracts")
                            'Build The Building~57174~Igtampe;Build the Building and make it real good boio pls help
                            Dim ConMSGSplit() As String
                            ConMSGSplit = ConMSG.Split(";")
                            Dim ContractID As Integer
                            ContractID = 0
                            While (File.Exists(UMSWEBDir & "\SSH\Contracts\" & ContractID & ".txt"))
                                ContractID = ContractID + 1
                            End While
                            ToConsole("This contract will be Contract #" & ContractID)
                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Contracts.txt"), OpenMode.Append)
                            PrintLine(1, ContractID & "~" & ConMSGSplit(0) & "~-1~Uninitialized~Uninitialized")
                            FileClose(1)
                            ToConsole("Written to the contracts, Now its on to the description")
                            FileOpen(1, UMSWEBDir & "\SSH\Contracts\" & ContractID & ".txt", OpenMode.Output)
                            PrintLine(1, ConMSGSplit(1))
                            FileClose(1)
                            ToConsole("OK Done")
                            binaryWriter.Write("S")
                            GoTo Restart

                        ElseIf ConMSG.StartsWith("ADDBID") Then
                            Dim ConMSGSplit() As String
                            'ContractID;NewBid;UserID;UserName
                            ' 0           1      2        3
                            ConMSG = ConMSG.Replace("ADDBID", "")
                            ConMSGSplit = ConMSG.Split(";")
                            ToConsole("Oh fuck time to add a bid AAAAAAAAAAAAAA")

                            If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\Contracts.txt")) Then
                                binaryWriter.Write("E")
                                Exit Try
                            End If

                            ToConsole("Trying to Find Contract #" & ConMSGSplit(0))

                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Contracts.txt"), OpenMode.Input)
                            FileOpen(2, String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"), OpenMode.Output)
                            Dim CurrentLine() As String
                            Dim woops As Boolean = True
                            While Not EOF(1)
                                CurrentLine = LineInput(1).Split("~")
                                'ID~Name~FromID~FromNAME~TopBid~TopBidID~TopBidNAME
                                ' 0  1      2     3        4        5          6

                                If CurrentLine(0) = ConMSGSplit(0) Then
                                    woops = False
                                    ToConsole("Found it")
                                    If ConMSGSplit(1) < CurrentLine(4) Or CurrentLine(4) = -1 Then

                                        CurrentLine(4) = ConMSGSplit(1)
                                        CurrentLine(5) = ConMSGSplit(2)
                                        CurrentLine(6) = ConMSGSplit(3)

                                    Else
                                        ToConsole("Looks like this bid isn't less, so I cannot do.")
                                        woops = True
                                    End If
                                End If

                                PrintLine(2, String.Join("~", CurrentLine))

                            End While

                            FileClose(1)
                            FileClose(2)

                            If woops Then
                                File.Delete(String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"))
                                binaryWriter.Write("E")
                            Else
                                File.Delete(String.Concat(UMSWEBDir, "\SSH\Contracts.txt"))
                                File.Move(String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"), String.Concat(UMSWEBDir, "\SSH\Contracts.txt"))
                                binaryWriter.Write("S")
                            End If


                        ElseIf ConMSG.StartsWith("MOVETOUSER") Then
                            Dim ConMSGSplit() As String
                            'ContractID;User
                            ' 0           1
                            ConMSGSplit = ConMSG.Replace("MOVETOUSER", "").Split(";")
                            ToConsole("Oh fuck time to add a bid AAAAAAAAAAAAAA")

                            If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\Contracts.txt")) Then
                                binaryWriter.Write("E")
                                Exit Try
                            End If

                            ToConsole("Trying to Find Contract #" & ConMSGSplit(0))

                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Contracts.txt"), OpenMode.Input)
                            FileOpen(2, String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"), OpenMode.Output)
                            Dim CurrentLine() As String
                            Dim TransferedContract() As String
                            Dim woops As Boolean = True
                            While Not EOF(1)
                                CurrentLine = LineInput(1).Split("~")
                                'ID~Name~FromID~FromNAME~TopBid~TopBidID~TopBidNAME
                                ' 0  1      2     3        4        5          6

                                If CurrentLine(0) = ConMSGSplit(0) Then
                                    woops = False
                                    ToConsole("Found it")
                                    TransferedContract = CurrentLine
                                Else
                                    PrintLine(2, String.Join("~", CurrentLine))
                                End If
                            End While
                            FileClose(1)
                            FileClose(2)

                            If woops Then
                                File.Delete(String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"))
                                binaryWriter.Write("E")
                            Else
                                File.Delete(String.Concat(UMSWEBDir, "\SSH\Contracts.txt"))
                                File.Move(String.Concat(UMSWEBDir, "\SSH\TempContracts.txt"), String.Concat(UMSWEBDir, "\SSH\Contracts.txt"))

                                ToConsole("OK now that we've removed it its time to add it to the user's coso")
                                If File.Exists(String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt")) Then
                                    FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"), OpenMode.Append)
                                Else
                                    FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"), OpenMode.Output)
                                End If

                                PrintLine(1, String.Join("~", TransferedContract))
                                FileClose(1)
                                binaryWriter.Write("S")
                            End If
                        ElseIf ConMSG.StartsWith("REMOVE") Then
                            Dim ConMSGSplit() As String
                            'ContractID;User
                            ' 0           1
                            ConMSGSplit = ConMSG.Replace("REMOVE", "").Split(";")
                            ToConsole("Oh fuck time to add a bid AAAAAAAAAAAAAA")

                            If Not File.Exists(String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt")) Then
                                binaryWriter.Write("E")
                                Exit Try
                            End If

                            ToConsole("Trying to Find Contract #" & ConMSGSplit(0))

                            FileOpen(1, String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"), OpenMode.Input)
                            FileOpen(2, String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\TempContracts.txt"), OpenMode.Output)
                            Dim CurrentLine() As String
                            Dim woops As Boolean = True
                            While Not EOF(1)
                                CurrentLine = LineInput(1).Split("~")
                                'ID~Name~FromID~FromNAME~TopBid~TopBidID~TopBidNAME
                                ' 0  1      2     3        4        5          6

                                If CurrentLine(0) = ConMSGSplit(0) Then
                                    woops = False
                                    ToConsole("Found it, not copying it")
                                Else
                                    PrintLine(2, String.Join("~", CurrentLine))
                                End If
                            End While
                            FileClose(1)
                            FileClose(2)

                            If woops Then
                                File.Delete(String.Concat(UMSWEBDir, "\SSH\Users" & ConMSGSplit(1) & "\TempContracts.txt"))
                                binaryWriter.Write("E")
                            Else
                                File.Delete(String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"))
                                File.Move(String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\TempContracts.txt"), String.Concat(UMSWEBDir, "\SSH\Users\" & ConMSGSplit(1) & "\Contracts.txt"))
                                ToConsole("OK we should be good to go")
                                binaryWriter.Write("S")
                            End If
                        Else
                            binaryWriter.Write("E")
                        End If

                    Catch ex As Exception
                        ToConsole("The Contractus subsystem crashed." & vbNewLine & vbNewLine & ex.ToString)
                        binaryWriter.Write("E")
                    End Try

                Else
                    ToConsole("Invalid Packet Sent")
                    binaryWriter.Write("invalid Packet Sent")
                End If
Restart:
            End While
        End Sub

        Sub ToConsole(ByVal ConsoleMSG As String)

            Console.WriteLine("[" & DateTime.Now.ToString & "] " & ConsoleMSG)

        End Sub

        Sub ToFile(ByVal path As String, ByVal what As String)

            FileOpen(10, path, OpenMode.Output)
            PrintLine(10, what)
            FileClose(10)

        End Sub

    End Module
End Namespace