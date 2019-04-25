Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace VibeServer.My
	<GeneratedCode("MyTemplate", "11.0.0.0")>
	<HideModuleName>
	Friend Module MyProject
		Private ReadOnly m_ComputerObjectProvider As MyProject.ThreadSafeObjectProvider(Of MyComputer)

		Private ReadOnly m_AppObjectProvider As MyProject.ThreadSafeObjectProvider(Of MyApplication)

		Private ReadOnly m_UserObjectProvider As MyProject.ThreadSafeObjectProvider(Of Microsoft.VisualBasic.ApplicationServices.User)

		Private ReadOnly m_MyWebServicesObjectProvider As MyProject.ThreadSafeObjectProvider(Of MyProject.MyWebServices)

		<HelpKeyword("My.Application")>
		Friend ReadOnly Property Application As MyApplication
			<DebuggerHidden>
			Get
				Return MyProject.m_AppObjectProvider.GetInstance
			End Get
		End Property

		<HelpKeyword("My.Computer")>
		Friend ReadOnly Property Computer As MyComputer
			<DebuggerHidden>
			Get
				Return MyProject.m_ComputerObjectProvider.GetInstance
			End Get
		End Property

		<HelpKeyword("My.User")>
		Friend ReadOnly Property User As Microsoft.VisualBasic.ApplicationServices.User
			<DebuggerHidden>
			Get
				Return MyProject.m_UserObjectProvider.GetInstance
			End Get
		End Property

		<HelpKeyword("My.WebServices")>
		Friend ReadOnly Property WebServices As MyProject.MyWebServices
			<DebuggerHidden>
			Get
				Return MyProject.m_MyWebServicesObjectProvider.GetInstance
			End Get
		End Property

		Sub New()
			MyProject.m_ComputerObjectProvider = New MyProject.ThreadSafeObjectProvider(Of MyComputer)()
			MyProject.m_AppObjectProvider = New MyProject.ThreadSafeObjectProvider(Of MyApplication)()
			MyProject.m_UserObjectProvider = New MyProject.ThreadSafeObjectProvider(Of Microsoft.VisualBasic.ApplicationServices.User)()
			MyProject.m_MyWebServicesObjectProvider = New MyProject.ThreadSafeObjectProvider(Of MyProject.MyWebServices)()
		End Sub

		<EditorBrowsable(EditorBrowsableState.Never)>
		<MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")>
		Friend NotInheritable Class MyWebServices
			<DebuggerHidden>
			<EditorBrowsable(EditorBrowsableState.Never)>
			Public Sub New()
				MyBase.New()
			End Sub

			<DebuggerHidden>
			Private Shared Function Create__Instance__(Of T As New)(ByVal instance As T) As T
				Dim t1 As T
				t1 = If(instance IsNot Nothing, instance, Activator.CreateInstance(Of T)())
				Return t1
			End Function

			<DebuggerHidden>
			Private Sub Dispose__Instance__(Of T)(ByRef instance As T)
				instance = Nothing
			End Sub

			<DebuggerHidden>
			<EditorBrowsable(EditorBrowsableState.Never)>
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return Me.Equals(RuntimeHelpers.GetObjectValue(o))
			End Function

			<DebuggerHidden>
			<EditorBrowsable(EditorBrowsableState.Never)>
			Public Overrides Function GetHashCode() As Integer
				Return Me.GetHashCode()
			End Function

			<DebuggerHidden>
			<EditorBrowsable(EditorBrowsableState.Never)>
			Friend Shadows Function [GetType]() As Type
				Return GetType(MyProject.MyWebServices)
			End Function

			<DebuggerHidden>
			<EditorBrowsable(EditorBrowsableState.Never)>
			Public Overrides Function ToString() As String
				Return Me.ToString()
			End Function
		End Class

		<ComVisible(False)>
		<EditorBrowsable(EditorBrowsableState.Never)>
		Friend NotInheritable Class ThreadSafeObjectProvider(Of T As New)
			<CompilerGenerated>
			<ThreadStatic>
			Private Shared m_ThreadStaticValue As T

			Friend ReadOnly Property GetInstance As T
				<DebuggerHidden>
				Get
					If (MyProject.ThreadSafeObjectProvider(Of T).m_ThreadStaticValue Is Nothing) Then
						MyProject.ThreadSafeObjectProvider(Of T).m_ThreadStaticValue = Activator.CreateInstance(Of T)()
					End If
					Return MyProject.ThreadSafeObjectProvider(Of T).m_ThreadStaticValue
				End Get
			End Property

			<DebuggerHidden>
			<EditorBrowsable(EditorBrowsableState.Never)>
			Public Sub New()
				MyBase.New()
			End Sub
		End Class
	End Module
End Namespace