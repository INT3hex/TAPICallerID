Imports System.Reflection
Imports System.Configuration ' Add also Reference - System.Configuration


Public Class AppConfigClass

    Public Class AppConfig
        Private _config As Configuration
        Private _settings As AppSettingsSection

        Public Function GetProperty(propertyName As String, propertyDefault As String) As String

            Dim propertyValue As String = If(_settings.Settings.Item(propertyName) IsNot Nothing, _settings.Settings.Item(propertyName).Value, propertyDefault)
            SetProperty(propertyName, propertyValue)
            Return propertyValue

        End Function

        Public Sub SetProperty(propertyName As String, propertyValue As String)
            If _settings.Settings.Item(propertyName) IsNot Nothing Then
                _settings.Settings.Item(propertyName).Value = propertyValue
            Else
                _settings.Settings.Add(propertyName, propertyValue)
            End If
        End Sub

        Public Sub New()
            _config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location)
            _settings = _config.AppSettings
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
            _config.Save(ConfigurationSaveMode.Modified)
        End Sub
    End Class

End Class
