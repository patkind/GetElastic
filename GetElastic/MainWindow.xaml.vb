Imports System.ComponentModel
Imports Microsoft.Win32
Imports System.Windows.Threading
Imports System.Net
Imports System.Text
Imports System.IO

Class MainWindow
#Region "Definitions"
    Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public WithEvents BW_RefreshShards As New BackgroundWorker

    Dim dispatcherTimer As DispatcherTimer = New DispatcherTimer()
    Public Rows As List(Of RowFactory)
    Public iLabels As New List(Of Integer)
    Public sShards As String
#End Region

    Public Function CreateShards()
        'Funktion zum Erzeugen der notwendigen Labels für Shards, Nodenames und Setzen der Anzahl von Shards und Replica
        Dim sResult As String = ""
        Dim sResult2 As String = ""
        Dim sResult3 As String = ""
        log.Debug("Start")

        Dim elasticsettings As ElasticClusterSettings
        Dim elastichealth As ElasticClusterHealth
        Dim elasticcatshard As ElasticCatShards

        'Holen der notwendigen JSON-Dateien
        Try
            sResult = GetJSONHttp("/enaioblue_*/_settings/index.number_of_*")
            log.Debug("----")
            sResult2 = GetJSONHttp("/_cluster/health")
            log.Debug("-----")
            sResult3 = GetJSONHttp("/_cat/shards?format=json")
            log.Debug("-----")

        Catch ex As Exception
            log.Error(gsIPAdress)
            log.Error(ex)
        End Try

        'Übergeben der JSON-Dateien zum Parsen
        elasticsettings = New ElasticClusterSettings(Converter.JsonToXML(sResult))
        elastichealth = New ElasticClusterHealth(Converter.JsonToXML(sResult2))
        elasticcatshard = New ElasticCatShards(Converter.JsonToXML(sResult3))

        'Farbe / Visibility für den Status setzen
        setElipStatusVisibility(elastichealth.status.value)

        'Setzen der Anzahl von Shards und Replica
        sShards = elasticsettings.enaioblue.settings.index.number_of_shards.value
        txtShards.Text = sShards
        txtReplicas.Text = elasticsettings.enaioblue.settings.index.number_of_replicas.value
        txtDocCount.Text = GetDocumentCount(sShards).ToString

        'Erzeugen der Reihen und Rückgabe an den Aufrufenden
        Return createRowData(elasticcatshard)

    End Function

    Public Sub RefreshShards()
        'Funktion zum Refreshen der Shard-Labels
        'Aktuell deaktiviert ist das Refreshen der Anzahl von Shards und Replica
        Dim sResult As String = ""
        Dim sResult2 As String = ""
        Dim sResult3 As String = ""
        log.Debug("Start")

        '  Dim elasticsettings As ElasticClusterSettings
        Dim elastichealth As ElasticClusterHealth
        Dim elasticcatshard As ElasticCatShards

        'Holen der notwendigen JSON-Dateien
        Try
            '  sResult = GetJSONHttp("/enaioblue_*/_settings/index.number_of_")
            '  log.Debug("-----")
            sResult2 = GetJSONHttp("/_cluster/health")
            log.Debug("-----")
            sResult3 = GetJSONHttp("/_cat/shards?format=json")
            log.Debug("-----")
        Catch ex As Exception
            log.Error(gsIPAdress)
            log.Error(ex)
        End Try

        'Übergeben der JSON-Dateien zum Parsen
        ' elasticsettings = New ElasticClusterSettings(Converter.JsonToXML(sResult))
        elastichealth = New ElasticClusterHealth(Converter.JsonToXML(sResult2))
        elasticcatshard = New ElasticCatShards(Converter.JsonToXML(sResult3))

        'Farbe / Visibility für den Status setzen
        setElipStatusVisibility(elastichealth.status.value)

        'Setzen der Anzahl von Shards und Replica

        ' sShards = elasticsettings.enaioblue.settings.index.number_of_shards.value
        'txtShards.Text = sShards
        ' txtReplicas.Text = elasticsettings.enaioblue.settings.index.number_of_replicas.value
        txtDocCount.Text = GetDocumentCount(sShards).ToString

        'Erzeugen der Reihen und Rückgabe an den Aufrufenden
        updateRowData(elasticcatshard)

    End Sub

    Public Sub setElipStatusVisibility(ByVal status As String)
        elipStatus.Visibility = Visibility.Visible
        If status = "green" Then
            elipStatus.Fill = New SolidColorBrush(Colors.Lime)

        ElseIf status = "yellow" Then
            elipStatus.Fill = New SolidColorBrush(Colors.Yellow)

        ElseIf status = "red" Then
            elipStatus.Fill = New SolidColorBrush(Colors.Red)

        Else
            elipStatus.Fill = New SolidColorBrush(Colors.Gray)
            log.Error("Status kann nicht ausgelesen werden")
        End If
    End Sub

    Private Sub deleteFromRows(sNodes As List(Of String))
        Dim lRowsToDelete As New List(Of Integer)
        Dim count As Byte

        'ermitteln der nicht mehr vorhandenen Nodes
        For Each Row As RowFactory In Rows
            If Not (sNodes.Contains(Row.getNodeLabel.Content.ToString)) Then
                lRowsToDelete.Add(count)
            End If
            count += 1
        Next

        'Rückwärts sortieren der Liste
        lRowsToDelete.Sort()
        lRowsToDelete.Reverse()

        'entfernen der Reihen aus der Liste
        If lRowsToDelete.Count > 0 Then
            For Each row As Integer In lRowsToDelete
                Rows.RemoveAt(row)
            Next
        End If
    End Sub

    Private Sub AddToRows(elasticShardItems As List(Of ElasticCatShardsItem))
        'Neue Reihe erzeugen
        Rows.Add(New RowFactory(elasticShardItems, New Point(10, 250 + (Rows.Count * 40))))
    End Sub

    Function createRowData(ByVal elasticcatshard As ElasticCatShards)
        'Initiale Erzeugung der Reihen aus dem Ergebnis der Elasticabfrage
        Dim sNodes As List(Of String) = elasticcatshard.GetAllNodeNames()
        Dim count As Byte = 0
        Dim lLabel As New List(Of Label)
        Dim Rows As New List(Of RowFactory)

        'erzeugen der Reihe für jeden Datensatz nach Nodename
        For Each nodeName As String In sNodes
            Rows.Add(New RowFactory(elasticcatshard.GetByIndex("enaioblue_0", nodeName), New Point(10, 250 + (count * 40))))
            lLabel.Add(Rows(Rows.Count - 1).getNodeLabel())
            lLabel.AddRange(Rows(Rows.Count - 1).getShardLabel)
            count += 1
        Next

        'Hinzufügen der Elemente zur GUI
        For Each lblLabel As Label In lLabel
            Grid.SetRow(lblLabel, 0)
            Grid.SetColumn(lblLabel, 0)
            elasticStatus.Children.Add(lblLabel)
            'Zwischenspeichern der Position in Children Liste damit diese sauber wieder entfernt werden können
            iLabels.Add(elasticStatus.Children.Count - 1)
        Next

        Return Rows
    End Function

    Sub updateRowData(ByVal elasticcatshard As ElasticCatShards)
        'hole NodeNames
        Dim sNodes As List(Of String) = elasticcatshard.GetAllNodeNames()

        'entferne Nicht mehr vorhandene Nodes
        deleteFromRows(sNodes)

        Dim count As Byte = 0

        For Each Row As RowFactory In Rows
            'Update des Status
            Row.rowUpdate(elasticcatshard.GetByIndex("enaioblue_0", Row.getNodeLabel().Content.ToString()))
            'Update der Position
            Row.updatePosition(New Point(10, 250 + (count * 40)))
            count += 1
        Next

        'Liste der aktuell im Porgramm bekannten Nodes erzeugen
        Dim sActualNodes As New List(Of String)

        For Each Row As RowFactory In Rows
            sActualNodes.Add(Row.getNodeLabel.Content.ToString())
        Next

        'Füge neue Nodes hinzu
        For Each sNodeName As String In sNodes
            If Not (sActualNodes.Contains(sNodeName)) Then
                AddToRows(elasticcatshard.GetByIndex("enaioblue_0", sNodeName))
            End If
        Next

        'Rückwärtssortierung der Liste
        iLabels.Sort()
        iLabels.Reverse()

        'entfernen der Labels von der Oberfläche
        For Each index As Integer In iLabels
            elasticStatus.Children.RemoveAt(index)
        Next

        'hinzufügen der Labels aus den Reihen
        Dim lLabel As New List(Of Label)

        For Each Row As RowFactory In Rows
            lLabel.Add(Row.getNodeLabel())
            lLabel.AddRange(Row.getShardLabel)
        Next

        'Leeren der Liste
        iLabels.Clear()

        'Hinzufügen der Label auf die Oberfläche & Listenposition in die Liste iLabels um bei der nächsten Aktualisierung entfernen zu können
        For Each lblLabel As Label In lLabel
            Grid.SetRow(lblLabel, 0)
            Grid.SetColumn(lblLabel, 0)
            elasticStatus.Children.Add(lblLabel)
            iLabels.Add(elasticStatus.Children.Count - 1)
        Next
    End Sub

#Region "General"
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        pgRefreshShards.Visibility = Visibility.Hidden
        lblIndexName.Visibility = Visibility.Hidden

        txtIP.Text = GetElasticInstall(0)
        txtPort.Text = GetElasticInstall(1)

        elipStatus.Visibility = Visibility.Hidden

        AddHandler dispatcherTimer.Tick, AddressOf dispatcherTimer_Tick
        dispatcherTimer.Interval = New TimeSpan(0, 0, 10)

    End Sub

    Private Sub btnQuit_Click(sender As Object, e As RoutedEventArgs) Handles btnQuit.Click
        Application.Current.Shutdown()
    End Sub

    Private Sub btnConnect_Click(sender As Object, e As RoutedEventArgs) Handles btnConnect.Click
        gsIPAdress = "http://" + txtIP.Text + ":" + txtPort.Text
        lblIndexName.Content = "enaioblue__0"
        lblIndexName.Visibility = Visibility.Visible
        Me.Rows = CreateShards()
        pgRefreshShards.Visibility = Visibility.Visible
        btnConnect.IsEnabled = False
        dispatcherTimer.Start()

    End Sub

    Private Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        RefreshShards()
    End Sub

    Private Sub BW_RefreshShards_DoWork(sender As Object, e As DoWorkEventArgs) Handles BW_RefreshShards.DoWork
        'RefreshShards()
    End Sub

    Private Sub BW_RefreshShards_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BW_RefreshShards.RunWorkerCompleted
        pgRefreshShards.Visibility = Visibility.Hidden
    End Sub
#End Region

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
#End Region

End Class