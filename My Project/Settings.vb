'This file contains settings for the application.
Namespace My
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase

        Private Shared defaultInstance As MySettings

        <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.Configuration.UserScopedSettingAttribute(), Global.System.Configuration.DefaultSettingValueAttribute("")>
        Public Property DatabaseConnection() As String
            Get
            End Get
            Set(value As String)
            End Set
        End Property
    End Class
End Namespace
