Public Class RowFactory
    Private _lblNode As New Label()
    Private _lLblShards As New List(Of Label)
    Private Position As Point
    Private frmForm As Window
    Private Hash As Int64 = 0

    Private Function setColor(ByVal status As String) As SolidColorBrush
        status = status.ToUpper()

        Select Case status
            Case "STARTED"
                Return New SolidColorBrush(Colors.Lime)
            Case "UNASSIGNED"
                Return New SolidColorBrush(Colors.LightGray)
            Case "RELOCATING"
                Return New SolidColorBrush(Colors.DarkSalmon)
            Case "INITIALIZING"
                Return New SolidColorBrush(Colors.Yellow)
            Case Else
                Return New SolidColorBrush(Colors.DarkGray)
        End Select
    End Function

    Private Function setThickness(ByVal type As String) As Thickness
        If type = "p" Then
            Return New Thickness(2)
        End If
        Return New Thickness(1)
    End Function

    Private Function setBorderColor(ByVal type As String) As SolidColorBrush
        If type = "p" Then
            Return New SolidColorBrush(Colors.Black)
        End If
        Return New SolidColorBrush(Colors.LightGray)
    End Function

    Private Function createLabel(ByVal elasticShardItem As ElasticCatShardsItem, ByVal Positon As Point) As Label
        Dim lblShard As New Label()

        lblShard.Content = elasticShardItem.shard.value
        lblShard.BorderThickness = Me.setThickness(elasticShardItem.prirep.value)
        lblShard.BorderBrush = Me.setBorderColor(elasticShardItem.prirep.value)
        lblShard.Width = 30
        lblShard.Height = 30
        lblShard.HorizontalAlignment = HorizontalAlignment.Left
        lblShard.VerticalAlignment = VerticalAlignment.Top
        lblShard.Margin = setPosition(Position, Convert.ToInt32(elasticShardItem.shard.value))
        lblShard.Foreground = New SolidColorBrush(Colors.Black)
        lblShard.HorizontalContentAlignment = HorizontalAlignment.Center
        lblShard.VerticalContentAlignment = VerticalAlignment.Center
        lblShard.Background = Me.setColor(elasticShardItem.state.value)
        lblShard.ToolTip = elasticShardItem.state.value
        lblShard.Visibility = Visibility.Visible

        Return lblShard
    End Function

    Private Sub doTheWork(ByVal elasticShardsItems As List(Of ElasticCatShardsItem), Optional ByVal nodeName As String = Nothing)
        Dim hash As Integer

        Me._lblNode.Content = elasticShardsItems(0).node
        Me._lblNode.Width = 110
        Me._lblNode.Height = 25
        Me._lblNode.HorizontalAlignment = HorizontalAlignment.Left
        Me._lblNode.VerticalAlignment = VerticalAlignment.Top
        Me._lblNode.Foreground = New SolidColorBrush(Colors.Black)
        Me._lblNode.Margin = New Thickness(Position.X, Position.Y, 0, 0)
        Me._lblNode.Visibility = Visibility.Visible

        For Each elasticShardItem As ElasticCatShardsItem In elasticShardsItems
            hash += elasticShardItem.GetHashCode()
            Dim tempLabel As Label
            tempLabel = Me.createLabel(elasticShardItem, Position)
            Me._lLblShards.Add(tempLabel)
        Next

        Me.Hash = hash
    End Sub

    Public Sub New(ByVal elasticShardsItems As List(Of ElasticCatShardsItem), ByVal Position As Point, Optional ByVal nodeName As String = Nothing)
        Me.Position = Position
        Me.doTheWork(elasticShardsItems, nodeName)
    End Sub

    Public Function checkRowUpdate(ByVal elasticShardsItems As List(Of ElasticCatShardsItem)) As Boolean
        Dim hash As Integer = 0

        For Each elasticShardItem As ElasticCatShardsItem In elasticShardsItems
            hash += elasticShardItem.GetHashCode()
        Next

        Return Not (Me.Hash.Equals(hash))

    End Function

    Public Sub rowUpdate(ByVal elasticShardsItems As List(Of ElasticCatShardsItem), Optional ByVal nodeName As String = Nothing)
        If checkRowUpdate(elasticShardsItems) Then
            Me._lblNode = New Label()
            Me._lLblShards = New List(Of Label)
            Me.doTheWork(elasticShardsItems, nodeName)
        End If
    End Sub

    Public Function getNodeLabel() As Label
        Return _lblNode
    End Function

    Public Function getShardLabel() As List(Of Label)
        Return _lLblShards
    End Function

    Private Function setPosition(ByVal Position As Point, ByVal value As Byte) As Thickness
        Position.X = 125 + (value * 40)
        Return New Thickness(Position.X, Position.Y, 0, 0)
    End Function

    Public Sub updatePosition(ByVal Position As Point)
        Me.Position = Position
        _lblNode.Margin = New Thickness(Position.X, Position.Y, 0, 0)
        For Each lbl As Label In _lLblShards
            lbl.Margin = setPosition(Position, Convert.ToInt32(lbl.Content))
        Next
    End Sub

End Class