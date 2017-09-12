Imports System.ComponentModel
Imports Microsoft.Win32
Imports System.Windows.Threading
Imports System.Net
Imports System.Text

Class MainWindow

    Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public WithEvents BW_RefreshShards As New BackgroundWorker

    Dim dispatcherTimer As DispatcherTimer = New DispatcherTimer()
    Public aLabel() As Label
    Public iNumberNodes As Integer = 0
    Public iNumberShards As Integer = 0

    Private Sub btnConnect_Click(sender As Object, e As RoutedEventArgs) Handles btnConnect.Click
#Region "ALT"
        '        Dim webClient As New System.Net.WebClient
        '        Dim sResult As String = ""
        '        Dim sResult2 As String = ""
        '        Dim sResult3 As String = ""
        '        log.Debug("Start")
        '        gsIPAdress = txtIP.Text + ":" + txtPort.Text
        '        Dim elasticsettings As ElasticClusterSettings
        '        Dim elastichealth As ElasticClusterHealth
        '        Dim elasticcatshard As ElasticCatShards

        '#Region "Per WebClient JSON abholen"
        '        Try
        '            sResult = webClient.DownloadString("http://" + gsIPAdress + "/enaioblue_*/_settings/index.number_of_*")
        '            log.Debug(sResult)
        '            log.Debug("-----")

        '            sResult2 = webClient.DownloadString("http://" + gsIPAdress + "/_cluster/health")
        '            log.Debug(sResult2)
        '            log.Debug("-----")


        '            sResult3 = webClient.DownloadString("http://" + gsIPAdress + "/_cat/shards?format=json")
        '            log.Debug(sResult3)
        '            log.Debug("-----")

        '        Catch ex As Exception
        '            log.Error(gsIPAdress)
        '            log.Error(ex)
        '        End Try

        '        elasticsettings = New ElasticClusterSettings(Converter.JsonToXML(sResult))
        '        elastichealth = New ElasticClusterHealth(Converter.JsonToXML(sResult2))
        '        elasticcatshard = New ElasticCatShards(Converter.JsonToXML(sResult3))
        '#End Region

        '#Region "Maske manipulieren"
        '        lblIndexName.Content = "enaioblue__0"

        '        If elastichealth.status.value = "green" Then
        '            elipRed.Visibility = Visibility.Hidden
        '            elipYellow.Visibility = Visibility.Hidden
        '            elipGreen.Visibility = Visibility.Visible
        '            elipUnknown.Visibility = Visibility.Hidden
        '        ElseIf elastichealth.status.value = "yellow" Then
        '            elipRed.Visibility = Visibility.Hidden
        '            elipYellow.Visibility = Visibility.Visible
        '            elipGreen.Visibility = Visibility.Hidden
        '            elipUnknown.Visibility = Visibility.Hidden
        '        ElseIf elastichealth.status.value = "red" Then
        '            elipRed.Visibility = Visibility.Visible
        '            elipYellow.Visibility = Visibility.Hidden
        '            elipGreen.Visibility = Visibility.Hidden
        '            elipUnknown.Visibility = Visibility.Hidden
        '        Else
        '            MsgBox("Status kann nicht ausgelesen werden")
        '        End If
        '        txtShards.Text = elasticsettings.enaioblue.settings.index.number_of_shards.value
        '        txtReplicas.Text = elasticsettings.enaioblue.settings.index.number_of_replicas.value

        '        Dim sNodes As List(Of String) = elasticcatshard.GetAllNodeNames()

        '        For i As Integer = 0 To elasticcatshard.GetAllNodeNames.Count - 1
        '            Dim dynamicLabel As New Label()
        '            dynamicLabel.Name = String.Format("lblNode{0}", i)
        '            dynamicLabel.Content = sNodes(i)
        '            dynamicLabel.Width = 110
        '            dynamicLabel.Height = 25
        '            dynamicLabel.HorizontalAlignment = HorizontalAlignment.Left
        '            dynamicLabel.VerticalAlignment = VerticalAlignment.Top
        '            dynamicLabel.Margin = New Thickness(10, 250 + (i * 60), 0, 0)
        '            dynamicLabel.Foreground = New SolidColorBrush(Colors.Black)

        '            Grid.SetRow(dynamicLabel, 0)
        '            Grid.SetColumn(dynamicLabel, 0)
        '            elasticStatus.Children.Add(dynamicLabel)

        '        Next

        '        Dim aLabelNode((elasticcatshard.GetAllNodeNames().Count * 100) + elasticsettings.enaioblue.settings.index.number_of_shards.value - 1) As Label

        '        For i As Integer = 0 To elasticcatshard.GetAllNodeNames().Count - 1
        '            'Dim sShards As List(Of ElasticCatShardsItem) = elasticcatshard.GetByNodeName(sNodes(i))
        '            ' log.Debug(sShards(i))
        '            For ii As Integer = 0 To (elasticsettings.enaioblue.settings.index.number_of_shards.value - 1)
        '                Dim dynamicLabel As New Label()
        '                Dim sShards As List(Of ElasticCatShardsItem) = elasticcatshard.GetByNodeName(sNodes(i), "enaioblue_0", ii)

        '                dynamicLabel.Name = String.Format("lblNode{0}Shard{1}", i, ii)
        '                dynamicLabel.Content = ii
        '                dynamicLabel.Width = 30
        '                dynamicLabel.Height = 30
        '                dynamicLabel.HorizontalAlignment = HorizontalAlignment.Left
        '                dynamicLabel.VerticalAlignment = VerticalAlignment.Top
        '                dynamicLabel.Margin = New Thickness((125 + (ii * 60)), 250 + (i * 60), 0, 0)
        '                dynamicLabel.Foreground = New SolidColorBrush(Colors.Black)
        '                dynamicLabel.HorizontalContentAlignment = HorizontalContentAlignment.Center
        '                dynamicLabel.VerticalContentAlignment = VerticalContentAlignment.Center

        '                If sShards.Count > 0 Then
        '                    log.Debug("____")
        '                    log.Debug(sShards(0).ToString)
        '                    dynamicLabel.Visibility = Visibility.Visible

        '                    If sShards(0).prirep.value = "p" Then
        '                        dynamicLabel.BorderThickness = New Thickness(2)
        '                        dynamicLabel.BorderBrush = New SolidColorBrush(Colors.Black)
        '                    End If

        '                    If sShards(0).state.value = "STARTED" Then
        '                        dynamicLabel.Background = New SolidColorBrush(Colors.Lime)
        '                    ElseIf sShards(0).state.value = "UNASSIGNED" Then
        '                        dynamicLabel.Background = New SolidColorBrush(Colors.LightGray)
        '                    ElseIf sShards(0).state.value = "RELOCATING" Then
        '                        dynamicLabel.Background = New SolidColorBrush(Colors.Purple)
        '                    ElseIf sShards(0).state.value = "INITIALIZING" Then
        '                        dynamicLabel.Background = New SolidColorBrush(Colors.Yellow)
        '                    Else
        '                        dynamicLabel.Background = New SolidColorBrush(Colors.DarkGray)
        '                    End If
        '                    dynamicLabel.ToolTip = sShards(0).state.value
        '                Else
        '                    dynamicLabel.Visibility = Visibility.Hidden
        '                End If

        '                Grid.SetRow(dynamicLabel, 0)
        '                Grid.SetColumn(dynamicLabel, 0)
        '                elasticStatus.Children.Add(dynamicLabel)
        '                aLabelNode(i * 100 + ii) = dynamicLabel

        '            Next

        '        Next

        '#End Region
#End Region
        gsIPAdress = txtIP.Text + ":" + txtPort.Text
        lblIndexName.Content = "enaioblue__0"
        lblIndexName.Visibility = Visibility.Visible
        aLabel = CreateShards()
        pgRefreshShards.Visibility = Visibility.Visible
        btnConnect.IsEnabled = False
        dispatcherTimer.Start()


    End Sub

    Public Function CreateShards()
        Dim webClient As New System.Net.WebClient
        Dim sResult As String = ""
        Dim sResult2 As String = ""
        Dim sResult3 As String = ""
        log.Debug("Start")

        Dim elasticsettings As ElasticClusterSettings
        Dim elastichealth As ElasticClusterHealth
        Dim elasticcatshard As ElasticCatShards

        Try
            sResult = webClient.DownloadString("http://" + gsIPAdress + "/enaioblue_*/_settings/index.number_of_*")
            log.Debug(sResult)
            log.Debug("-----")

            sResult2 = webClient.DownloadString("http://" + gsIPAdress + "/_cluster/health")
            log.Debug(sResult2)
            log.Debug("-----")


            sResult3 = webClient.DownloadString("http://" + gsIPAdress + "/_cat/shards?format=json")
            log.Debug(sResult3)
            log.Debug("-----")

        Catch ex As Exception
            log.Error(gsIPAdress)
            log.Error(ex)
        End Try

        elasticsettings = New ElasticClusterSettings(Converter.JsonToXML(sResult))
        elastichealth = New ElasticClusterHealth(Converter.JsonToXML(sResult2))
        elasticcatshard = New ElasticCatShards(Converter.JsonToXML(sResult3))

        elipStatus.Visibility = Visibility.Visible
        If elastichealth.status.value = "green" Then
            elipStatus.Fill = New SolidColorBrush(Colors.Lime)

        ElseIf elastichealth.status.value = "yellow" Then
            elipStatus.Fill = New SolidColorBrush(Colors.Yellow)

        ElseIf elastichealth.status.value = "red" Then
            elipStatus.Fill = New SolidColorBrush(Colors.Red)

        Else
            elipStatus.Fill = New SolidColorBrush(Colors.Gray)
            MsgBox("Status kann nicht ausgelesen werden")
        End If

        iNumberNodes = elasticcatshard.GetAllNodeNames.Count
        iNumberShards = elasticsettings.enaioblue.settings.index.number_of_shards.value

        txtShards.Text = iNumberShards
        txtReplicas.Text = elasticsettings.enaioblue.settings.index.number_of_replicas.value

        Dim sNodes As List(Of String) = elasticcatshard.GetAllNodeNames()
        Dim aLabelNode((iNumberNodes * 1000) + iNumberShards - 1) As Label

        For i As Integer = 0 To iNumberNodes - 1
            Dim dynamicLabel As New Label()
            dynamicLabel.Name = String.Format("lblNode{0}", i)
            dynamicLabel.Content = sNodes(i)
            dynamicLabel.Width = 110
            dynamicLabel.Height = 25
            dynamicLabel.HorizontalAlignment = HorizontalAlignment.Left
            dynamicLabel.VerticalAlignment = VerticalAlignment.Top
            dynamicLabel.Margin = New Thickness(10, 250 + (i * 40), 0, 0)
            dynamicLabel.Foreground = New SolidColorBrush(Colors.Black)

            Grid.SetRow(dynamicLabel, 0)
            Grid.SetColumn(dynamicLabel, 0)
            elasticStatus.Children.Add(dynamicLabel)
            aLabelNode(i + 1000) = dynamicLabel
        Next

        For i As Integer = 0 To iNumberNodes - 1

            For ii As Integer = 0 To (iNumberShards - 1)
                Dim dynamicLabel As New Label()
                Dim sShards As List(Of ElasticCatShardsItem) = elasticcatshard.GetByNodeName(sNodes(i), "enaioblue_0", ii)

                dynamicLabel.Name = String.Format("lblNode{0}Shard{1}", i, ii)
                dynamicLabel.Content = ii
                dynamicLabel.Width = 30
                dynamicLabel.Height = 30
                dynamicLabel.HorizontalAlignment = HorizontalAlignment.Left
                dynamicLabel.VerticalAlignment = VerticalAlignment.Top
                dynamicLabel.Margin = New Thickness((125 + (ii * 40)), 250 + (i * 40), 0, 0)
                dynamicLabel.Foreground = New SolidColorBrush(Colors.Black)
                dynamicLabel.HorizontalContentAlignment = HorizontalContentAlignment.Center
                dynamicLabel.VerticalContentAlignment = VerticalContentAlignment.Center

                If sShards.Count > 0 Then
                    log.Debug("____")
                    log.Debug(sShards(0).ToString)
                    dynamicLabel.Visibility = Visibility.Visible

                    If sShards(0).prirep.value = "p" Then
                        dynamicLabel.BorderThickness = New Thickness(2)
                        dynamicLabel.BorderBrush = New SolidColorBrush(Colors.Black)
                    End If

                    If sShards(0).state.value = "STARTED" Then
                        dynamicLabel.Background = New SolidColorBrush(Colors.Lime)
                    ElseIf sShards(0).state.value = "UNASSIGNED" Then
                        dynamicLabel.Background = New SolidColorBrush(Colors.LightGray)
                    ElseIf sShards(0).state.value = "RELOCATING" Then
                        dynamicLabel.Background = New SolidColorBrush(Colors.Purple)
                    ElseIf sShards(0).state.value = "INITIALIZING" Then
                        dynamicLabel.Background = New SolidColorBrush(Colors.Yellow)
                    Else
                        dynamicLabel.Background = New SolidColorBrush(Colors.DarkGray)
                    End If
                    dynamicLabel.ToolTip = sShards(0).state.value
                Else
                    dynamicLabel.Visibility = Visibility.Hidden
                End If

                Grid.SetRow(dynamicLabel, 0)
                Grid.SetColumn(dynamicLabel, 0)
                elasticStatus.Children.Add(dynamicLabel)
                aLabelNode(i * 100 + ii) = dynamicLabel

            Next

        Next
        Return aLabelNode
    End Function

    Sub RefreshShards()
#Region "Orga"
        Dim webClient As New System.Net.WebClient
        Dim sResult As String = ""
        Dim sResult2 As String = ""
        Dim sResult3 As String = ""
        log.Debug("Start")

        Dim elasticsettings As ElasticClusterSettings
        Dim elastichealth As ElasticClusterHealth
        Dim elasticcatshard As ElasticCatShards

        Try
            sResult = webClient.DownloadString("http://" + gsIPAdress + "/enaioblue_*/_settings/index.number_of_*")
            log.Debug(sResult)
            log.Debug("-----")

            sResult2 = webClient.DownloadString("http://" + gsIPAdress + "/_cluster/health")
            log.Debug(sResult2)
            log.Debug("-----")


            sResult3 = webClient.DownloadString("http://" + gsIPAdress + "/_cat/shards?format=json")
            log.Debug(sResult3)
            log.Debug("-----")

        Catch ex As Exception
            log.Error(gsIPAdress)
            log.Error(ex)
        End Try

        elasticsettings = New ElasticClusterSettings(Converter.JsonToXML(sResult))
        elastichealth = New ElasticClusterHealth(Converter.JsonToXML(sResult2))
        elasticcatshard = New ElasticCatShards(Converter.JsonToXML(sResult3))
#End Region
        If elastichealth.status.value = "green" Then
            elipStatus.Fill = New SolidColorBrush(Colors.Lime)

        ElseIf elastichealth.status.value = "yellow" Then
            elipStatus.Fill = New SolidColorBrush(Colors.Yellow)

        ElseIf elastichealth.status.value = "red" Then
            elipStatus.Fill = New SolidColorBrush(Colors.Red)

        Else
            elipStatus.Fill = New SolidColorBrush(Colors.Gray)
            MsgBox("Status kann nicht ausgelesen werden")
        End If
        txtShards.Text = elasticsettings.enaioblue.settings.index.number_of_shards.value
        txtReplicas.Text = elasticsettings.enaioblue.settings.index.number_of_replicas.value

        Dim sNodes As List(Of String) = elasticcatshard.GetAllNodeNames()
        '  iNumberNodes = elasticcatshard.GetAllNodeNames.Count

        For i As Integer = 0 To iNumberNodes - 1
            Try
                aLabel(i + 1000).Content = sNodes(i)
            Catch
            End Try
        Next
        For i As Integer = 0 To iNumberNodes - 1

            For ii As Integer = 0 To (iNumberShards - 1)
                Dim sShards As List(Of ElasticCatShardsItem)
                Try
                    sShards = elasticcatshard.GetByNodeName(sNodes(i), "enaioblue_0", ii)

                Catch ex As Exception
                    sShards = elasticcatshard.GetByNodeName("keine da", "enaioblue_0", ii)
                End Try
                If sShards.Count > 0 Then
                    log.Debug("____")
                    log.Debug(sShards(0).ToString)
                    aLabel(i * 100 + ii).Visibility = Visibility.Visible

                    If sShards(0).prirep.value = "p" Then
                        aLabel(i * 100 + ii).BorderThickness = New Thickness(2)
                        aLabel(i * 100 + ii).BorderBrush = New SolidColorBrush(Colors.Black)
                    End If

                    If sShards(0).state.value = "STARTED" Then
                        aLabel(i * 100 + ii).Background = New SolidColorBrush(Colors.Lime)
                    ElseIf sShards(0).state.value = "UNASSIGNED" Then
                        aLabel(i * 100 + ii).Background = New SolidColorBrush(Colors.LightGray)
                        aLabel(i * 100 + ii).BorderThickness = New Thickness(0)
                    ElseIf sShards(0).state.value = "RELOCATING" Then
                        aLabel(i * 100 + ii).Background = New SolidColorBrush(Colors.Purple)
                    ElseIf sShards(0).state.value = "INITIALIZING" Then
                        aLabel(i * 100 + ii).Background = New SolidColorBrush(Colors.Yellow)
                    Else
                        aLabel(i * 100 + ii).Visibility = Visibility.Hidden

                    End If
                    aLabel(i * 100 + ii).ToolTip = sShards(0).state.value
                Else
                    aLabel(i * 100 + ii).Visibility = Visibility.Hidden
                    log.Debug("keine Shards gefunden")
                    log.Debug((i * 100 + ii))
                End If


            Next

        Next

        '    aLabel(0).Visibility = Visibility.Hidden
        '  aLabel(100).Visibility = Visibility.Hidden

    End Sub

    Private Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        RefreshShards()
    End Sub

#Region "Testing"
    Private Sub btnTest_Click(sender As Object, e As RoutedEventArgs) Handles btnTest.Click
        PutReplica(1)
    End Sub

    Private Sub btnTest2_Click(sender As Object, e As RoutedEventArgs) Handles btnTest2.Click
        PutReplica(0)
    End Sub

    Private Sub btnWriteXML_Click(sender As Object, e As RoutedEventArgs) Handles btnWriteXML.Click
        WriteXML()
    End Sub

    Private Sub PutReplica(iNumber)
        Dim s As HttpWebRequest
        Dim enc As UTF8Encoding
        Dim postdata As String
        Dim postdatabytes As Byte()
        s = HttpWebRequest.Create("http://10.10.77.182:8041/_settings")
        enc = New System.Text.UTF8Encoding()
        postdata = "{index:{number_of_replicas:" & iNumber & "}}"
        postdatabytes = enc.GetBytes(postdata)
        s.Method = "PUT"
        s.ContentType = "application/x-www-form-urlencoded"
        s.ContentLength = postdatabytes.Length

        Using stream = s.GetRequestStream()
            stream.Write(postdatabytes, 0, postdatabytes.Length)
        End Using
        Dim result = s.GetResponse()
    End Sub

#End Region

#Region "General"

    Public Sub CreateLabelShards()
        Dim webClient As New System.Net.WebClient
        Dim sResult As String = ""


    End Sub

    Public Sub CreateLabelNodes()
        Dim webClient As New System.Net.WebClient
        Dim sResult As String = ""


    End Sub

    Private Sub btnQuit_Click(sender As Object, e As RoutedEventArgs) Handles btnQuit.Click
        Application.Current.Shutdown()
    End Sub

    Private Sub btnAddLabel_Click(sender As Object, e As RoutedEventArgs) Handles btnAddLabel.Click
        gsIPAdress = txtIP.Text + ":" + txtPort.Text
        CreateLabelShards()
        CreateLabelNodes()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim regKey As RegistryKey
        Dim sPathElatic As String = ""
        Dim lines() As String
        Dim sIP As String = "10.10.77.182"
        Dim sPort As String = "8041"
        pgRefreshShards.Visibility = Visibility.Hidden
        lblIndexName.Visibility = Visibility.Hidden

        Try
            regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
            regKey = regKey.OpenSubKey("SOFTWARE\OPTIMAL SYSTEMS\elasticsearch", False)
            sPathElatic = regKey.GetValue("Location")
            log.Debug(sPathElatic)
            lines = IO.File.ReadAllLines(sPathElatic & "\config\elasticsearch.yml")
            For i As Integer = 0 To lines.Length - 1
                If lines(i).StartsWith("network.host: ") Then
                    sIP = lines(i)
                    log.Debug(sIP)
                    sIP = Strings.Mid(sIP, (Strings.InStr(sIP, ":") + 2))
                    Exit For
                End If
            Next
            For i As Integer = 0 To lines.Length - 1
                If lines(i).StartsWith("http.port: ") Then
                    sPort = lines(i)
                    log.Debug(sPort)
                    sPort = Strings.Mid(sPort, (Strings.InStr(sPort, ":") + 2))
                    Exit For
                End If
            Next
            log.Debug(sIP + ":" + sPort)

        Catch

        End Try
        txtIP.Text = sIP
        txtPort.Text = sPort

        elipStatus.Visibility = Visibility.Hidden

        AddHandler dispatcherTimer.Tick, AddressOf dispatcherTimer_Tick
        dispatcherTimer.Interval = New TimeSpan(0, 0, 10)

    End Sub

    Private Sub BW_RefreshShards_DoWork(sender As Object, e As DoWorkEventArgs) Handles BW_RefreshShards.DoWork
        'RefreshShards()
    End Sub

    Private Sub BW_RefreshShards_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BW_RefreshShards.RunWorkerCompleted
        pgRefreshShards.Visibility = Visibility.Hidden
    End Sub
#End Region

End Class