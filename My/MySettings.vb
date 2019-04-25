Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Configuration
Imports System.Runtime.CompilerServices

Namespace VibeServer.My
	<CompilerGenerated>
	<EditorBrowsable(EditorBrowsableState.Advanced)>
	<GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.8.0.0")>
	Friend NotInheritable Class MySettings
		Inherits ApplicationSettingsBase
		Private Shared defaultInstance As MySettings

		Public ReadOnly Shared Property [Default] As MySettings
			Get
				Return MySettings.defaultInstance
			End Get
		End Property

		Shared Sub New()
			MySettings.defaultInstance = DirectCast(SettingsBase.Synchronized(New MySettings()), MySettings)
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub
	End Class
End Namespace