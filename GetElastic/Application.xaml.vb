Imports Microsoft.Win32

Class Application
    Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Private WithEvents Domaene As AppDomain = AppDomain.CurrentDomain

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup


    End Sub
    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.
    'Private Sub Application_Startup(sender As Object)
    ' 
    'End Sub



End Class
