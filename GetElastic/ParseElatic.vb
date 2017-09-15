Imports System.Reflection
Imports System.Text.RegularExpressions

#Region "Base"

Public Class PropInfo
    Public value As String
    Public ReadOnly location As String

    Public Sub New(ByVal location As String)
        Me.location = location
    End Sub

    Public Overrides Function ToString() As String
        Return value
    End Function

End Class

Public Class PropInfoList
    Public ReadOnly location As String
    Public values As New List(Of PropInfo)

    Public Sub New(ByVal location As String)
        Me.location = location
    End Sub
End Class

Public MustInherit Class ElasticBase

    Protected path As String
    Protected xml As Xml.XmlDocument

    Public Function GetProperties() As List(Of PropertyInfo)
        Return Me.GetType().GetProperties().ToList()
    End Function

    Public Function parse(Optional ByVal ignoreException As Boolean = True) As Boolean
        Dim properties As New List(Of PropertyInfo)

        properties = GetProperties()

        For Each propertie In properties
            Try
                If (propertie.PropertyType.Name.Equals("PropInfo")) Then
                    Dim prop As PropInfo = CType(propertie.GetValue(Me), PropInfo)

                    prop.value = xml.SelectSingleNode(prop.location).InnerText

                ElseIf (propertie.PropertyType.Name.Equals("PropInfoList")) Then
                    Dim prop As PropInfoList = CType(propertie.GetValue(Me), PropInfoList)
                    Dim xmlList As Xml.XmlNodeList = xml.SelectNodes(prop.location)

                    For Each xmlnode As Xml.XmlNode In xmlList
                        Dim newProp As New PropInfo(prop.location)

                        newProp.value = xmlnode.InnerText

                        prop.values.Add(newProp)
                    Next
                End If
            Catch ex As NullReferenceException
                If Not ignoreException Then
                    Return False
                End If
            End Try
        Next

        Return True
    End Function

    Public Sub New(ByVal xml As Xml.XmlDocument, Optional ByVal path As String = "")
        Me.path = path
        Me.xml = xml
    End Sub

End Class

#End Region

#Region "_cluster/health"

Public Class ElasticClusterHealth
    Inherits ElasticBase

    'http://127.0.0.1:8041/_cluster/health

#Region "Fields"

    Private _cluster_name As New PropInfo("//cluster_name")
    Private _status As New PropInfo("//status")
    Private _timed_out As New PropInfo("//timed_out")
    Private _number_of_nodes As New PropInfo("//number_of_nodes")
    Private _number_of_data_nodes As New PropInfo("//number_of_data_nodes")
    Private _active_primary_shards As New PropInfo("//active_primary_shards")
    Private _active_shards As New PropInfo("//active_shards")
    Private _relocating_shards As New PropInfo("//relocating_shards")
    Private _initializing_shards As New PropInfo("//initializing_shards")
    Private _unassigned_shards As New PropInfo("//unassigned_shards")
    Private _delayed_unassigned_shards As New PropInfo("//delayed_unassigned_shards")
    Private _number_of_pending_tasks As New PropInfo("//number_of_pending_tasks")
    Private _number_of_in_flight_fetch As New PropInfo("//number_of_in_flight_fetch")
    Private _task_max_waiting_in_queue_millis As New PropInfo("//task_max_waiting_in_queue_millis")
    Private _active_shards_percent_as_number As New PropInfo("//active_shards_percent_as_number")

#End Region

#Region "Properties"

    Public Property cluster_name As PropInfo
        Get
            Return _cluster_name
        End Get
        Set(value As PropInfo)
            _cluster_name = value
        End Set
    End Property

    Public Property status As PropInfo
        Get
            Return _status
        End Get
        Set(value As PropInfo)
            _status = value
        End Set
    End Property

    Public Property timed_out As PropInfo
        Get
            Return _timed_out
        End Get
        Set(value As PropInfo)
            _timed_out = value
        End Set
    End Property

    Public Property number_of_nodes As PropInfo
        Get
            Return _number_of_nodes
        End Get
        Set(value As PropInfo)
            _number_of_nodes = value
        End Set
    End Property

    Public Property number_of_data_nodes As PropInfo
        Get
            Return _number_of_data_nodes
        End Get
        Set(value As PropInfo)
            _number_of_data_nodes = value
        End Set
    End Property

    Public Property active_primary_shards As PropInfo
        Get
            Return _active_primary_shards
        End Get
        Set(value As PropInfo)
            _active_primary_shards = value
        End Set
    End Property

    Public Property active_shards As PropInfo
        Get
            Return _active_shards
        End Get
        Set(value As PropInfo)
            _active_shards = value
        End Set
    End Property

    Public Property relocating_shards As PropInfo
        Get
            Return _relocating_shards
        End Get
        Set(value As PropInfo)
            _relocating_shards = value
        End Set
    End Property

    Public Property initializing_shards As PropInfo
        Get
            Return _initializing_shards
        End Get
        Set(value As PropInfo)
            _initializing_shards = value
        End Set
    End Property

    Public Property unassigned_shards As PropInfo
        Get
            Return _unassigned_shards
        End Get
        Set(value As PropInfo)
            _unassigned_shards = value
        End Set
    End Property

    Public Property delayed_unassigned_shards As PropInfo
        Get
            Return _delayed_unassigned_shards
        End Get
        Set(value As PropInfo)
            _delayed_unassigned_shards = value
        End Set
    End Property

    Public Property number_of_pending_tasks As PropInfo
        Get
            Return _number_of_pending_tasks
        End Get
        Set(value As PropInfo)
            _number_of_pending_tasks = value
        End Set
    End Property

    Public Property number_of_in_flight_fetch As PropInfo
        Get
            Return _number_of_in_flight_fetch
        End Get
        Set(value As PropInfo)
            _number_of_in_flight_fetch = value
        End Set
    End Property

    Public Property task_max_waiting_in_queue_millis As PropInfo
        Get
            Return _task_max_waiting_in_queue_millis
        End Get
        Set(value As PropInfo)
            _task_max_waiting_in_queue_millis = value
        End Set
    End Property

    Public Property active_shards_percent_as_number As PropInfo
        Get
            Return _active_shards_percent_as_number
        End Get
        Set(value As PropInfo)
            _active_shards_percent_as_number = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        parse()
    End Sub

#End Region

End Class

#End Region

#Region "_cluster/stats?human&pretty"
'offen nodes

#Region "nodes"
'offen jvm -> versions
'offen jvm -> mem
'offen os -> names
'offen nodes -> plugins

Public Class ElasticClusterStatsNodesFs
    Inherits ElasticBase

#Region "Fields"

    Private _total As PropInfo
    Private _total_in_bytes As PropInfo
    Private _free As PropInfo
    Private _free_in_bytes As PropInfo
    Private _available As PropInfo
    Private _available_in_bytes As PropInfo

#End Region

#Region "Properties"

    Public Property total As PropInfo
        Get
            Return _total
        End Get
        Set(value As PropInfo)
            _total = value
        End Set
    End Property

    Public Property total_in_bytes As PropInfo
        Get
            Return _total_in_bytes
        End Get
        Set(value As PropInfo)
            _total_in_bytes = value
        End Set
    End Property

    Public Property free As PropInfo
        Get
            Return _free
        End Get
        Set(value As PropInfo)
            _free = value
        End Set
    End Property

    Public Property free_in_bytes As PropInfo
        Get
            Return _free_in_bytes
        End Get
        Set(value As PropInfo)
            _free_in_bytes = value
        End Set
    End Property

    Public Property available As PropInfo
        Get
            Return _available
        End Get
        Set(value As PropInfo)
            _available = value
        End Set
    End Property

    Public Property available_in_bytes As PropInfo
        Get
            Return _available_in_bytes
        End Get
        Set(value As PropInfo)
            _available_in_bytes = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/fs", path))
        Me._total = New PropInfo(String.Format("{0}/total", Me.path))
        Me._total_in_bytes = New PropInfo(String.Format("{0}/total_in_bytes", Me.path))
        Me._free = New PropInfo(String.Format("{0}/free", Me.path))
        Me._free_in_bytes = New PropInfo(String.Format("{0}/free_in_bytes", Me.path))
        Me._available = New PropInfo(String.Format("{0}/available", Me.path))
        Me._available_in_bytes = New PropInfo(String.Format("{0}/available_in_bytes", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsNodesJvm
    Inherits ElasticBase

#Region "Fields"

    Private _max_uptime As PropInfo
    Private _max_uptime_in_millis As PropInfo
    'Private _versions As ElasticClusterStatsNodesJvmVersions
    'Private _mem As ElasticClusterStatsNodesJvmMem
    Private _threads As PropInfo

#End Region

#Region "Properties"

    Public Property max_uptime As PropInfo
        Get
            Return _max_uptime
        End Get
        Set(value As PropInfo)
            _max_uptime = value
        End Set
    End Property

    Public Property max_uptime_in_millis As PropInfo
        Get
            Return _max_uptime_in_millis
        End Get
        Set(value As PropInfo)
            _max_uptime_in_millis = value
        End Set
    End Property

    Public Property threads As PropInfo
        Get
            Return _threads
        End Get
        Set(value As PropInfo)
            _threads = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/jvm", path))
        Me._max_uptime = New PropInfo(String.Format("{0}/max_uptime", Me.path))
        Me._max_uptime_in_millis = New PropInfo(String.Format("{0}/max_uptime_in_millis", Me.path))
        'Me._versions = New ElasticClusterStatsNodesJvmVersions()
        'Me._mem = New ElasticClusterStatsNodesJvmMem()
        Me._threads = New PropInfo(String.Format("{0}/threads", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsNodesProcessOpen_file_descriptors
    Inherits ElasticBase

#Region "Fields"

    Private _min As PropInfo
    Private _max As PropInfo
    Private _avg As PropInfo

#End Region

#Region "Properties"

    Public Property min As PropInfo
        Get
            Return _min
        End Get
        Set(value As PropInfo)
            _min = value
        End Set
    End Property

    Public Property max As PropInfo
        Get
            Return _max
        End Get
        Set(value As PropInfo)
            _max = value
        End Set
    End Property

    Public Property avg As PropInfo
        Get
            Return _avg
        End Get
        Set(value As PropInfo)
            _avg = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/open_file_descriptors", path))
        Me._min = New PropInfo(String.Format("{0}/min", Me.path))
        Me._max = New PropInfo(String.Format("{0}/max", Me.path))
        Me._avg = New PropInfo(String.Format("{0}/avg", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsNodesProcessCPU
    Inherits ElasticBase

#Region "Fields"

    Private _percent As PropInfo

#End Region

#Region "Properties"

    Public Property percent As PropInfo
        Get
            Return _percent
        End Get
        Set(value As PropInfo)
            _percent = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/cpu", path))
        Me._percent = New PropInfo(String.Format("{0}/percent", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsNodesProcess
    Inherits ElasticBase

#Region "Fields"

    Private _cpu As ElasticClusterStatsNodesProcessCPU
    Private _open_file_descriptors As ElasticClusterStatsNodesProcessOpen_file_descriptors

#End Region

#Region "Properties"

    Public Property cpu As ElasticClusterStatsNodesProcessCPU
        Get
            Return _cpu
        End Get
        Set(value As ElasticClusterStatsNodesProcessCPU)
            _cpu = value
        End Set
    End Property

    Public Property open_file_descriptors As ElasticClusterStatsNodesProcessOpen_file_descriptors
        Get
            Return _open_file_descriptors
        End Get
        Set(value As ElasticClusterStatsNodesProcessOpen_file_descriptors)
            _open_file_descriptors = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/process", path))
        Me._cpu = New ElasticClusterStatsNodesProcessCPU(xml, Me.path)
        Me._open_file_descriptors = New ElasticClusterStatsNodesProcessOpen_file_descriptors(xml, Me.path)
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsNodesOsMem
    Inherits ElasticBase

#Region "Fields"

    Private _total As PropInfo
    Private _total_in_bytes As PropInfo

#End Region

#Region "Properties"

    Public Property total As PropInfo
        Get
            Return _total
        End Get
        Set(value As PropInfo)
            _total = value
        End Set
    End Property

    Public Property total_in_bytes As PropInfo
        Get
            Return _total_in_bytes
        End Get
        Set(value As PropInfo)
            _total_in_bytes = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/mem", path))
        Me._total = New PropInfo(String.Format("{0}/total", Me.path))
        Me._total_in_bytes = New PropInfo(String.Format("{0}/total_in_bytes", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsNodesOs
    Inherits ElasticBase

#Region "Fields"

    Private _available_processors As PropInfo
    Private _allocated_processors As PropInfo
    Private _mem As ElasticClusterStatsNodesOsMem
    'Private _names As ElasticClusterStatsNodesOsNames

#End Region

#Region "Properties"

    Public Property available_processors As PropInfo
        Get
            Return _available_processors
        End Get
        Set(value As PropInfo)
            _available_processors = value
        End Set
    End Property

    Public Property allocated_processors As PropInfo
        Get
            Return _allocated_processors
        End Get
        Set(value As PropInfo)
            _allocated_processors = value
        End Set
    End Property

    Public Property mem As ElasticClusterStatsNodesOsMem
        Get
            Return _mem
        End Get
        Set(value As ElasticClusterStatsNodesOsMem)
            _mem = value
        End Set
    End Property

    'Public Property names As ElasticClusterStatsNodesOsNames
    '    Get
    '        Return _names
    '    End Get
    '    Set(value As ElasticClusterStatsNodesOsNames)
    '        _names = value
    '    End Set
    'End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/os", path))
        Me._available_processors = New PropInfo(String.Format("{0}/available_processors", Me.path))
        Me._allocated_processors = New PropInfo(String.Format("{0}/allocated_processors", Me.path))
        Me._mem = New ElasticClusterStatsNodesOsMem(xml, Me.path)
        'Me._names = New ElasticClusterStatsNodesOsNames(xml, String.Format("{0}/names", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsNodesCount
    Inherits ElasticBase

#Region "Fields"

    Private _total As PropInfo
    Private _master_only As PropInfo
    Private _data_only As PropInfo
    Private _master_data As PropInfo
    Private _client As PropInfo

#End Region

#Region "Properties"

    Public Property total As PropInfo
        Get
            Return _total
        End Get
        Set(value As PropInfo)
            _total = value
        End Set
    End Property

    Public Property master_only As PropInfo
        Get
            Return _master_only
        End Get
        Set(value As PropInfo)
            _master_only = value
        End Set
    End Property

    Public Property data_only As PropInfo
        Get
            Return _data_only
        End Get
        Set(value As PropInfo)
            _data_only = value
        End Set
    End Property

    Public Property master_data As PropInfo
        Get
            Return _master_data
        End Get
        Set(value As PropInfo)
            _master_data = value
        End Set
    End Property

    Public Property client As PropInfo
        Get
            Return _client
        End Get
        Set(value As PropInfo)
            _client = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/count", path))
        Me._total = New PropInfo(String.Format("{0}/total", Me.path))
        Me._master_only = New PropInfo(String.Format("{0}/master_only", Me.path))
        Me._data_only = New PropInfo(String.Format("{0}/data_only", Me.path))
        Me._master_data = New PropInfo(String.Format("{0}/master_data", Me.path))
        Me._client = New PropInfo(String.Format("{0}/client", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsNodes
    Inherits ElasticBase

#Region "Fields"

    Private _count As ElasticClusterStatsNodesCount
    Private _versions As PropInfoList
    Private _os As ElasticClusterStatsNodesOs
    Private _process As ElasticClusterStatsNodesProcess
    Private _jvm As ElasticClusterStatsNodesJvm
    Private _fs As ElasticClusterStatsNodesFs

#End Region

#Region "Properties"

    Public Property count As ElasticClusterStatsNodesCount
        Get
            Return _count
        End Get
        Set(value As ElasticClusterStatsNodesCount)
            _count = value
        End Set
    End Property

    Public Property versions As PropInfoList
        Get
            Return _versions
        End Get
        Set(value As PropInfoList)
            _versions = value
        End Set
    End Property

    Public Property os As ElasticClusterStatsNodesOs
        Get
            Return _os
        End Get
        Set(value As ElasticClusterStatsNodesOs)
            _os = value
        End Set
    End Property

    Public Property process As ElasticClusterStatsNodesProcess
        Get
            Return _process
        End Get
        Set(value As ElasticClusterStatsNodesProcess)
            _process = value
        End Set
    End Property

    Public Property jvm As ElasticClusterStatsNodesJvm
        Get
            Return _jvm
        End Get
        Set(value As ElasticClusterStatsNodesJvm)
            _jvm = value
        End Set
    End Property

    Public Property fs As ElasticClusterStatsNodesFs
        Get
            Return _fs
        End Get
        Set(value As ElasticClusterStatsNodesFs)
            _fs = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml, "//nodes")
        Me._count = New ElasticClusterStatsNodesCount(xml, Me.path)
        Me._versions = New PropInfoList(String.Format("{0}/versions/item", Me.path))
        Me._os = New ElasticClusterStatsNodesOs(xml, Me.path)
        Me._process = New ElasticClusterStatsNodesProcess(xml, Me.path)
        Me._jvm = New ElasticClusterStatsNodesJvm(xml, Me.path)
        Me._fs = New ElasticClusterStatsNodesFs(xml, Me.path)
        parse()
    End Sub

#End Region

End Class

#End Region

#Region "Indices"

Public Class ElasticClusterStatsIndicesShardsIndexShards
    Inherits ElasticBase

#Region "Fields"

    Private _min As PropInfo
    Private _max As PropInfo
    Private _avg As PropInfo

#End Region

#Region "Properties"

    Public Property min As PropInfo
        Get
            Return _min
        End Get
        Set(value As PropInfo)
            _min = value
        End Set
    End Property

    Public Property max As PropInfo
        Get
            Return _max
        End Get
        Set(value As PropInfo)
            _max = value
        End Set
    End Property

    Public Property avg As PropInfo
        Get
            Return _avg
        End Get
        Set(value As PropInfo)
            _avg = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml)
        Me._min = New PropInfo(String.Format("{0}min", path))
        Me._max = New PropInfo(String.Format("{0}max", path))
        Me._avg = New PropInfo(String.Format("{0}avg", path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsIndicesShardsIndex
    Inherits ElasticBase

#Region "Fields"

    Private _shards As ElasticClusterStatsIndicesShardsIndexShards
    Private _primaries As ElasticClusterStatsIndicesShardsIndexShards
    Private _replication As ElasticClusterStatsIndicesShardsIndexShards

#End Region

#Region "Properties"

    Public Property shards As ElasticClusterStatsIndicesShardsIndexShards
        Get
            Return _shards
        End Get
        Set(value As ElasticClusterStatsIndicesShardsIndexShards)
            _shards = value
        End Set
    End Property

    Public Property primaries As ElasticClusterStatsIndicesShardsIndexShards
        Get
            Return _primaries
        End Get
        Set(value As ElasticClusterStatsIndicesShardsIndexShards)
            _primaries = value
        End Set
    End Property

    Public Property replication As ElasticClusterStatsIndicesShardsIndexShards
        Get
            Return _replication
        End Get
        Set(value As ElasticClusterStatsIndicesShardsIndexShards)
            _replication = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        Me._shards = New ElasticClusterStatsIndicesShardsIndexShards(xml, "//indices/shards/index/shards/")
        Me._primaries = New ElasticClusterStatsIndicesShardsIndexShards(xml, "//indices/shards/index/primaries/")
        Me._replication = New ElasticClusterStatsIndicesShardsIndexShards(xml, "//indices/shards/index/replication/")
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsIndicesShards
    Inherits ElasticBase

#Region "Fields"

    Private _total As New PropInfo("//indices/shards/total")
    Private _primaries As New PropInfo("//indices/shards/primaries")
    Private _replication As New PropInfo("//indices/shards/replication")
    Private _index As ElasticClusterStatsIndicesShardsIndex

#End Region

#Region "Properties"

    Public Property total As PropInfo
        Get
            Return _total
        End Get
        Set(value As PropInfo)
            _total = value
        End Set
    End Property

    Public Property primaries As PropInfo
        Get
            Return _primaries
        End Get
        Set(value As PropInfo)
            _primaries = value
        End Set
    End Property

    Public Property replication As PropInfo
        Get
            Return _replication
        End Get
        Set(value As PropInfo)
            _replication = value
        End Set
    End Property

    Public Property index As ElasticClusterStatsIndicesShardsIndex
        Get
            Return _index
        End Get
        Set(value As ElasticClusterStatsIndicesShardsIndex)
            _index = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        Me._index = New ElasticClusterStatsIndicesShardsIndex(xml)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsIndicesDocs
    Inherits ElasticBase

#Region "Fields"

    Private _count As New PropInfo("//indices/docs/count")
    Private _deleted As New PropInfo("//indices/docs/deleted")

#End Region

#Region "Properties"

    Public Property count As PropInfo
        Get
            Return _count
        End Get
        Set(value As PropInfo)
            _count = value
        End Set
    End Property

    Public Property deleted As PropInfo
        Get
            Return _deleted
        End Get
        Set(value As PropInfo)
            _deleted = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsIndicesStore
    Inherits ElasticBase

#Region "Fields"

    Private _size As New PropInfo("//indices/store/size")
    Private _size_in_bytes As New PropInfo("//indices/store/size_in_bytes")
    Private _throttle_time As New PropInfo("//indices/store/throttle_time")
    Private _throttle_time_in_millis As New PropInfo("//indices/store/throttle_time_in_millis")

#End Region

#Region "Properties"

    Public Property size As PropInfo
        Get
            Return _size
        End Get
        Set(value As PropInfo)
            _size = value
        End Set
    End Property

    Public Property size_in_bytes As PropInfo
        Get
            Return _size_in_bytes
        End Get
        Set(value As PropInfo)
            _size_in_bytes = value
        End Set
    End Property

    Public Property throttle_time As PropInfo
        Get
            Return _throttle_time
        End Get
        Set(value As PropInfo)
            _throttle_time = value
        End Set
    End Property

    Public Property throttle_time_in_millis As PropInfo
        Get
            Return _throttle_time_in_millis
        End Get
        Set(value As PropInfo)
            _throttle_time_in_millis = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsIndicesFielddata
    Inherits ElasticBase

#Region "Fields"

    Private _memory_size As New PropInfo("//indices/fielddata/memory_size")
    Private _memory_size_in_bytes As New PropInfo("//indices/fielddata/memory_size_in_bytes")
    Private _evictions As New PropInfo("//indices/fielddata/evictions")

#End Region

#Region "Properties"

    Public Property memory_size As PropInfo
        Get
            Return _memory_size
        End Get
        Set(value As PropInfo)
            _memory_size = value
        End Set
    End Property

    Public Property memory_size_in_bytes As PropInfo
        Get
            Return _memory_size_in_bytes
        End Get
        Set(value As PropInfo)
            _memory_size_in_bytes = value
        End Set
    End Property

    Public Property evictions As PropInfo
        Get
            Return _evictions
        End Get
        Set(value As PropInfo)
            _evictions = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsIndicesQuery_cache
    Inherits ElasticBase

#Region "Fields"

    Private _memory_size As New PropInfo("//indices/query_cache/memory_size")
    Private _memory_size_in_bytes As New PropInfo("//indices/query_cache/memory_size_in_bytes")
    Private _total_count As New PropInfo("//indices/query_cache/total_count")
    Private _hit_count As New PropInfo("//indices/query_cache/hit_count")
    Private _miss_count As New PropInfo("//indices/query_cache/miss_count")
    Private _cache_size As New PropInfo("//indices/query_cache/cache_size")
    Private _cache_count As New PropInfo("//indices/query_cache/cache_count")
    Private _evictions As New PropInfo("//indices/query_cache/evictions")

#End Region

#Region "Properties"

    Public Property memory_size As PropInfo
        Get
            Return _memory_size
        End Get
        Set(value As PropInfo)
            _memory_size = value
        End Set
    End Property

    Public Property memory_size_in_bytes As PropInfo
        Get
            Return _memory_size_in_bytes
        End Get
        Set(value As PropInfo)
            _memory_size_in_bytes = value
        End Set
    End Property

    Public Property total_count As PropInfo
        Get
            Return _total_count
        End Get
        Set(value As PropInfo)
            _total_count = value
        End Set
    End Property

    Public Property hit_count As PropInfo
        Get
            Return _hit_count
        End Get
        Set(value As PropInfo)
            _hit_count = value
        End Set
    End Property

    Public Property miss_count As PropInfo
        Get
            Return _miss_count
        End Get
        Set(value As PropInfo)
            _miss_count = value
        End Set
    End Property

    Public Property cache_size As PropInfo
        Get
            Return _cache_size
        End Get
        Set(value As PropInfo)
            _cache_size = value
        End Set
    End Property

    Public Property cache_count As PropInfo
        Get
            Return _cache_count
        End Get
        Set(value As PropInfo)
            _cache_count = value
        End Set
    End Property

    Public Property evictions As PropInfo
        Get
            Return _evictions
        End Get
        Set(value As PropInfo)
            _evictions = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsIndicesCompletion
    Inherits ElasticBase

#Region "Fields"

    Private _size As New PropInfo("//indices/completion/size")
    Private _size_in_bytes As New PropInfo("//indices/completion/size_in_bytes")

#End Region

#Region "Properties"

    Public Property size As PropInfo
        Get
            Return _size
        End Get
        Set(value As PropInfo)
            _size = value
        End Set
    End Property

    Public Property size_in_bytes As PropInfo
        Get
            Return _size_in_bytes
        End Get
        Set(value As PropInfo)
            _size_in_bytes = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsIndicesSegments
    Inherits ElasticBase

#Region "Fields"

    Private _count As New PropInfo("//indices/segments/count")
    Private _memory As New PropInfo("//indices/segments/memory")
    Private _memory_in_bytes As New PropInfo("//indices/segments/memory_in_bytes")
    Private _terms_memory As New PropInfo("//indices/segments/terms_memory")
    Private _terms_memory_in_bytes As New PropInfo("//indices/segments/terms_memory_in_bytes")
    Private _stored_fields_memory As New PropInfo("//indices/segments/stored_fields_memory")
    Private _stored_fields_memory_in_bytes As New PropInfo("//indices/segments/stored_fields_memory_in_bytes")
    Private _term_vectors_memory As New PropInfo("//indices/segments/term_vectors_memory")
    Private _term_vectors_memory_in_bytes As New PropInfo("//indices/segments/term_vectors_memory_in_bytes")
    Private _norms_memory As New PropInfo("//indices/segments/norms_memory")
    Private _norms_memory_in_bytes As New PropInfo("//indices/segments/norms_memory_in_bytes")
    Private _doc_values_memory As New PropInfo("//indices/segments/doc_values_memory")
    Private _doc_values_memory_in_bytes As New PropInfo("//indices/segments/doc_values_memory_in_bytes")
    Private _index_writer_memory As New PropInfo("//indices/segments/index_writer_memory")
    Private _index_writer_memory_in_bytes As New PropInfo("//indices/segments/index_writer_memory_in_bytes")
    Private _index_writer_max_memory As New PropInfo("//indices/segments/index_writer_max_memory")
    Private _index_writer_max_memory_in_bytes As New PropInfo("//indices/segments/index_writer_max_memory_in_bytes")
    Private _version_map_memory As New PropInfo("//indices/segments/version_map_memory")
    Private _version_map_memory_in_bytes As New PropInfo("//indices/segments/version_map_memory_in_bytes")
    Private _fixed_bit_set As New PropInfo("//indices/segments/fixed_bit_set")
    Private _fixed_bit_set_memory_in_bytes As New PropInfo("//indices/segments/fixed_bit_set_memory_in_bytes")

#End Region

#Region "Properties"

    Public Property count As PropInfo
        Get
            Return _count
        End Get
        Set(value As PropInfo)
            _count = value
        End Set
    End Property

    Public Property memory As PropInfo
        Get
            Return _memory
        End Get
        Set(value As PropInfo)
            _memory = value
        End Set
    End Property

    Public Property memory_in_bytes As PropInfo
        Get
            Return _memory_in_bytes
        End Get
        Set(value As PropInfo)
            _memory_in_bytes = value
        End Set
    End Property

    Public Property terms_memory As PropInfo
        Get
            Return _terms_memory
        End Get
        Set(value As PropInfo)
            _terms_memory = value
        End Set
    End Property

    Public Property terms_memory_in_bytes As PropInfo
        Get
            Return _terms_memory_in_bytes
        End Get
        Set(value As PropInfo)
            _terms_memory_in_bytes = value
        End Set
    End Property

    Public Property stored_fields_memory As PropInfo
        Get
            Return _stored_fields_memory
        End Get
        Set(value As PropInfo)
            _stored_fields_memory = value
        End Set
    End Property

    Public Property stored_fields_memory_in_bytes As PropInfo
        Get
            Return _stored_fields_memory_in_bytes
        End Get
        Set(value As PropInfo)
            _stored_fields_memory_in_bytes = value
        End Set
    End Property

    Public Property term_vectors_memory As PropInfo
        Get
            Return _term_vectors_memory
        End Get
        Set(value As PropInfo)
            _term_vectors_memory = value
        End Set
    End Property

    Public Property term_vectors_memory_in_bytes As PropInfo
        Get
            Return _term_vectors_memory_in_bytes
        End Get
        Set(value As PropInfo)
            _term_vectors_memory_in_bytes = value
        End Set
    End Property

    Public Property norms_memory As PropInfo
        Get
            Return _norms_memory
        End Get
        Set(value As PropInfo)
            _norms_memory = value
        End Set
    End Property

    Public Property norms_memory_in_bytes As PropInfo
        Get
            Return _norms_memory_in_bytes
        End Get
        Set(value As PropInfo)
            _norms_memory_in_bytes = value
        End Set
    End Property

    Public Property doc_values_memory As PropInfo
        Get
            Return _doc_values_memory
        End Get
        Set(value As PropInfo)
            _doc_values_memory = value
        End Set
    End Property

    Public Property doc_values_memory_in_bytes As PropInfo
        Get
            Return _doc_values_memory_in_bytes
        End Get
        Set(value As PropInfo)
            _doc_values_memory_in_bytes = value
        End Set
    End Property

    Public Property index_writer_memory As PropInfo
        Get
            Return _index_writer_memory
        End Get
        Set(value As PropInfo)
            _index_writer_memory = value
        End Set
    End Property

    Public Property index_writer_memory_in_bytes As PropInfo
        Get
            Return _index_writer_memory_in_bytes
        End Get
        Set(value As PropInfo)
            _index_writer_memory_in_bytes = value
        End Set
    End Property

    Public Property index_writer_max_memory As PropInfo
        Get
            Return _index_writer_max_memory
        End Get
        Set(value As PropInfo)
            _index_writer_max_memory = value
        End Set
    End Property

    Public Property index_writer_max_memory_in_bytes As PropInfo
        Get
            Return _index_writer_max_memory_in_bytes
        End Get
        Set(value As PropInfo)
            _index_writer_max_memory_in_bytes = value
        End Set
    End Property

    Public Property version_map_memory As PropInfo
        Get
            Return _version_map_memory
        End Get
        Set(value As PropInfo)
            _version_map_memory = value
        End Set
    End Property

    Public Property version_map_memory_in_bytes As PropInfo
        Get
            Return _version_map_memory_in_bytes
        End Get
        Set(value As PropInfo)
            _version_map_memory_in_bytes = value
        End Set
    End Property

    Public Property fixed_bit_set As PropInfo
        Get
            Return _fixed_bit_set
        End Get
        Set(value As PropInfo)
            _fixed_bit_set = value
        End Set
    End Property

    Public Property fixed_bit_set_memory_in_bytes As PropInfo
        Get
            Return _fixed_bit_set_memory_in_bytes
        End Get
        Set(value As PropInfo)
            _fixed_bit_set_memory_in_bytes = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsIndicesPercolate
    Inherits ElasticBase

#Region "Fields"

    Private _total As New PropInfo("//indices/percolate/total")
    Private _time As New PropInfo("//indices/percolate/time")
    Private _time_in_millis As New PropInfo("//indices/percolate/time_in_millis")
    Private _current As New PropInfo("//indices/percolate/current")
    Private _memory_size_in_bytes As New PropInfo("//indices/percolate/memory_size_in_bytes")
    Private _memory_size As New PropInfo("//indices/percolate/memory_size")
    Private _queries As New PropInfo("//indices/percolate/queries")

#End Region

#Region "Properties"

    Public Property total As PropInfo
        Get
            Return _total
        End Get
        Set(value As PropInfo)
            _total = value
        End Set
    End Property

    Public Property time As PropInfo
        Get
            Return _time
        End Get
        Set(value As PropInfo)
            _time = value
        End Set
    End Property

    Public Property time_in_millis As PropInfo
        Get
            Return _time_in_millis
        End Get
        Set(value As PropInfo)
            _time_in_millis = value
        End Set
    End Property

    Public Property current As PropInfo
        Get
            Return _current
        End Get
        Set(value As PropInfo)
            _current = value
        End Set
    End Property

    Public Property memory_size_in_bytes As PropInfo
        Get
            Return _memory_size_in_bytes
        End Get
        Set(value As PropInfo)
            _memory_size_in_bytes = value
        End Set
    End Property

    Public Property memory_size As PropInfo
        Get
            Return _memory_size
        End Get
        Set(value As PropInfo)
            _memory_size = value
        End Set
    End Property

    Public Property queries As PropInfo
        Get
            Return _queries
        End Get
        Set(value As PropInfo)
            _queries = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterStatsIndices
    Inherits ElasticBase

#Region "Fields"

    Private _count As New PropInfo("//indices/count")
    Private _shards As ElasticClusterStatsIndicesShards
    Private _docs As ElasticClusterStatsIndicesDocs
    Private _store As ElasticClusterStatsIndicesStore
    Private _fielddata As ElasticClusterStatsIndicesFielddata
    Private _query_cache As ElasticClusterStatsIndicesQuery_cache
    Private _completion As ElasticClusterStatsIndicesCompletion
    Private _segments As ElasticClusterStatsIndicesSegments
    Private _percolate As ElasticClusterStatsIndicesPercolate

#End Region

#Region "Properties"

    Public Property count As PropInfo
        Get
            Return _count
        End Get
        Set(value As PropInfo)
            _count = value
        End Set
    End Property

    Public Property shards As ElasticClusterStatsIndicesShards
        Get
            Return _shards
        End Get
        Set(value As ElasticClusterStatsIndicesShards)
            _shards = value
        End Set
    End Property

    Public Property docs As ElasticClusterStatsIndicesDocs
        Get
            Return _docs
        End Get
        Set(value As ElasticClusterStatsIndicesDocs)
            _docs = value
        End Set
    End Property

    Public Property store As ElasticClusterStatsIndicesStore
        Get
            Return _store
        End Get
        Set(value As ElasticClusterStatsIndicesStore)
            _store = value
        End Set
    End Property

    Public Property fielddata As ElasticClusterStatsIndicesFielddata
        Get
            Return _fielddata
        End Get
        Set(value As ElasticClusterStatsIndicesFielddata)
            _fielddata = value
        End Set
    End Property

    Public Property query_cache As ElasticClusterStatsIndicesQuery_cache
        Get
            Return _query_cache
        End Get
        Set(value As ElasticClusterStatsIndicesQuery_cache)
            _query_cache = value
        End Set
    End Property

    Public Property completion As ElasticClusterStatsIndicesCompletion
        Get
            Return _completion
        End Get
        Set(value As ElasticClusterStatsIndicesCompletion)
            _completion = value
        End Set
    End Property

    Public Property segments As ElasticClusterStatsIndicesSegments
        Get
            Return _segments
        End Get
        Set(value As ElasticClusterStatsIndicesSegments)
            _segments = value
        End Set
    End Property

    Public Property percolate As ElasticClusterStatsIndicesPercolate
        Get
            Return _percolate
        End Get
        Set(value As ElasticClusterStatsIndicesPercolate)
            _percolate = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        Me._shards = New ElasticClusterStatsIndicesShards(xml)
        Me._docs = New ElasticClusterStatsIndicesDocs(xml)
        Me._store = New ElasticClusterStatsIndicesStore(xml)
        Me._fielddata = New ElasticClusterStatsIndicesFielddata(xml)
        Me._query_cache = New ElasticClusterStatsIndicesQuery_cache(xml)
        Me._completion = New ElasticClusterStatsIndicesCompletion(xml)
        Me._segments = New ElasticClusterStatsIndicesSegments(xml)
        Me._percolate = New ElasticClusterStatsIndicesPercolate(xml)
        parse()
    End Sub

#End Region

End Class

#End Region

Public Class ElasticClusterStats
    Inherits ElasticBase

#Region "Fields"

    Private _timestamp As New PropInfo("//timestamp")
    Private _cluster_name As New PropInfo("//cluster_name")
    Private _status As New PropInfo("//status")

    Private _indices As ElasticClusterStatsIndices
    Private _nodes As ElasticClusterStatsNodes

#End Region

#Region "Properties"

    Public Property timestamp As PropInfo
        Get
            Return _timestamp
        End Get
        Set(value As PropInfo)
            _timestamp = value
        End Set
    End Property

    Public Property cluster_name As PropInfo
        Get
            Return _cluster_name
        End Get
        Set(value As PropInfo)
            _cluster_name = value
        End Set
    End Property

    Public Property status As PropInfo
        Get
            Return _status
        End Get
        Set(value As PropInfo)
            _status = value
        End Set
    End Property

    Public Property indices As ElasticClusterStatsIndices
        Get
            Return _indices
        End Get
        Set(value As ElasticClusterStatsIndices)
            _indices = value
        End Set
    End Property

    Public Property nodes As ElasticClusterStatsNodes
        Get
            Return _nodes
        End Get
        Set(value As ElasticClusterStatsNodes)
            _nodes = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        Me._indices = New ElasticClusterStatsIndices(xml)
        Me._nodes = New ElasticClusterStatsNodes(xml)
        parse()
    End Sub

#End Region

End Class

#End Region

#Region "_settings/"

Public Class ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild
    Inherits ElasticBase

#Region "Fields"
    Private _type As PropInfo
    Private _tokenizer As PropInfo
    Private _filter As PropInfo

#End Region

#Region "Properties"

    Public Property type As PropInfo
        Get
            Return _type
        End Get
        Set(value As PropInfo)
            _type = value
        End Set
    End Property

    Public Property tokenizer As PropInfo
        Get
            Return _tokenizer
        End Get
        Set(value As PropInfo)
            _tokenizer = value
        End Set
    End Property

    Public Property filter As PropInfo
        Get
            Return _filter
        End Get
        Set(value As PropInfo)
            _filter = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String, ByVal name As String)
        MyBase.New(xml, String.Format("{0}/{1}", path, name))
        Me._filter = New PropInfo(String.Format("{0}/filter", Me.path))
        Me._tokenizer = New PropInfo(String.Format("{0}/tokenizer", Me.path))
        Me._type = New PropInfo(String.Format("{0}/type", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerPath_parentids
    Inherits ElasticBase

#Region "Fields"
    Private _type As PropInfo
    Private _tokenizer As PropInfo
    Private _filter As PropInfoList

#End Region

#Region "Properties"

    Public Property type As PropInfo
        Get
            Return _type
        End Get
        Set(value As PropInfo)
            _type = value
        End Set
    End Property

    Public Property tokenizer As PropInfo
        Get
            Return _tokenizer
        End Get
        Set(value As PropInfo)
            _tokenizer = value
        End Set
    End Property

    Public Property filter As PropInfoList
        Get
            Return _filter
        End Get
        Set(value As PropInfoList)
            _filter = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/path_parentids", path))
        Me._filter = New PropInfoList(String.Format("{0}/filter/item", Me.path))
        Me._tokenizer = New PropInfo(String.Format("{0}/tokenizer", Me.path))
        Me._type = New PropInfo(String.Format("{0}/type", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizerChild
    Inherits ElasticBase

#Region "Fields"
    Private _pattern As PropInfo
    Private _type As PropInfo
    Private _group As PropInfo

#End Region

#Region "Properties"

    Public Property pattern As PropInfo
        Get
            Return _pattern
        End Get
        Set(value As PropInfo)
            _pattern = value
        End Set
    End Property

    Public Property type As PropInfo
        Get
            Return _type
        End Get
        Set(value As PropInfo)
            _type = value
        End Set
    End Property

    Public Property group As PropInfo
        Get
            Return _group
        End Get
        Set(value As PropInfo)
            _group = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String, ByVal name As String)
        MyBase.New(xml, String.Format("{0}/{1}", path, name))
        Me._group = New PropInfo(String.Format("{0}/group", Me.path))
        Me._pattern = New PropInfo(String.Format("{0}/pattern", Me.path))
        Me._type = New PropInfo(String.Format("{0}/type", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizer
    Inherits ElasticBase

#Region "Fields"
    Private _direct_parent_id As ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizerChild
    Private _only_parents_pattern As ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizerChild

#End Region

#Region "Properties"

    Public Property direct_parent_id As ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizerChild
        Get
            Return _direct_parent_id
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizerChild)
            _direct_parent_id = value
        End Set
    End Property

    Public Property only_parents_pattern As ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizerChild
        Get
            Return _only_parents_pattern
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizerChild)
            _only_parents_pattern = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/tokenizer", path))
        Me._direct_parent_id = New ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizerChild(xml, Me.path, "direct_parent_id")
        Me._only_parents_pattern = New ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizerChild(xml, Me.path, "only_parents_pattern")
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzer
    Inherits ElasticBase

#Region "Fields"
    Private _path_parentids As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerPath_parentids
    Private _collation_insensitive As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild
    Private _path_directparent As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild
    Private _paths As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild

#End Region

#Region "Properties"

    Public Property path_parentids As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerPath_parentids
        Get
            Return _path_parentids
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerPath_parentids)
            _path_parentids = value
        End Set
    End Property

    Public Property collation_insensitive As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild
        Get
            Return _collation_insensitive
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild)
            _collation_insensitive = value
        End Set
    End Property

    Public Property path_directparent As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild
        Get
            Return _path_directparent
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild)
            _path_directparent = value
        End Set
    End Property

    Public Property paths As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild
        Get
            Return _paths
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild)
            _paths = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/analyzer", path))
        Me._path_parentids = New ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerPath_parentids(xml, Me.path)
        Me._path_directparent = New ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild(xml, Me.path, "path_directparent")
        Me._collation_insensitive = New ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild(xml, Me.path, "collation_insensitive")
        Me._paths = New ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzerChild(xml, Me.path, "paths")
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterSettingsIndexSettingsIndexAnalysis
    Inherits ElasticBase

#Region "Fields"
    Private _analyzer As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzer
    Private _tokenizer As ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizer

#End Region

#Region "Properties"

    Public Property analyzer As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzer
        Get
            Return _analyzer
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzer)
            _analyzer = value
        End Set
    End Property

    Public Property tokenizer As ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizer
        Get
            Return _tokenizer
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizer)
            _tokenizer = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/analysis", path))
        Me._analyzer = New ElasticClusterSettingsIndexSettingsIndexAnalysisAnalyzer(xml, Me.path)
        Me._tokenizer = New ElasticClusterSettingsIndexSettingsIndexAnalysisTokenizer(xml, Me.path)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterSettingsIndexSettingsIndexVersion
    Inherits ElasticBase

#Region "Fields"
    Private _created As PropInfo

#End Region

#Region "Properties"

    Public Property created As PropInfo
        Get
            Return _created
        End Get
        Set(value As PropInfo)
            _created = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/index", path))
        Me._created = New PropInfo(String.Format("{0}/created", Me.path))
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterSettingsIndexSettingsIndex
    Inherits ElasticBase

#Region "Fields"
    Private _creation_date As PropInfo
    Private _number_of_shards As PropInfo
    Private _number_of_replicas As PropInfo
    Private _uuid As PropInfo
    Private _analysis As ElasticClusterSettingsIndexSettingsIndexAnalysis
    Private _version As ElasticClusterSettingsIndexSettingsIndexVersion

#End Region

#Region "Properties"

    Public Property creation_date As PropInfo
        Get
            Return _creation_date
        End Get
        Set(value As PropInfo)
            _creation_date = value
        End Set
    End Property

    Public Property number_of_shards As PropInfo
        Get
            Return _number_of_shards
        End Get
        Set(value As PropInfo)
            _number_of_shards = value
        End Set
    End Property

    Public Property number_of_replicas As PropInfo
        Get
            Return _number_of_replicas
        End Get
        Set(value As PropInfo)
            _number_of_replicas = value
        End Set
    End Property

    Public Property uuid As PropInfo
        Get
            Return _uuid
        End Get
        Set(value As PropInfo)
            _uuid = value
        End Set
    End Property

    Public Property analysis As ElasticClusterSettingsIndexSettingsIndexAnalysis
        Get
            Return _analysis
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndexAnalysis)
            _analysis = value
        End Set
    End Property

    Public Property version As ElasticClusterSettingsIndexSettingsIndexVersion
        Get
            Return _version
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndexVersion)
            _version = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/index", path))
        Me._creation_date = New PropInfo(String.Format("{0}/creation_date", Me.path))
        Me._number_of_shards = New PropInfo(String.Format("{0}/number_of_shards", Me.path))
        Me._number_of_replicas = New PropInfo(String.Format("{0}/number_of_replicas", Me.path))
        Me._uuid = New PropInfo(String.Format("{0}/uuid", Me.path))
        Me._analysis = New ElasticClusterSettingsIndexSettingsIndexAnalysis(xml, Me.path)
        Me._version = New ElasticClusterSettingsIndexSettingsIndexVersion(xml, Me.path)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterSettingsIndexSettings
    Inherits ElasticBase

#Region "Fields"
    Private _index As ElasticClusterSettingsIndexSettingsIndex
#End Region

#Region "Properties"

    Public Property index As ElasticClusterSettingsIndexSettingsIndex
        Get
            Return _index
        End Get
        Set(value As ElasticClusterSettingsIndexSettingsIndex)
            _index = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}/settings", path))
        _index = New ElasticClusterSettingsIndexSettingsIndex(xml, Me.path)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterSettingsIndex
    Inherits ElasticBase

#Region "Fields"
    Private _settings As ElasticClusterSettingsIndexSettings
#End Region

#Region "Properties"

    Public Property settings As ElasticClusterSettingsIndexSettings
        Get
            Return _settings
        End Get
        Set(value As ElasticClusterSettingsIndexSettings)
            _settings = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, ByVal path As String)
        MyBase.New(xml, String.Format("{0}", path))
        Me._settings = New ElasticClusterSettingsIndexSettings(xml, Me.path)
        parse()
    End Sub

#End Region

End Class

Public Class ElasticClusterSettings
    Inherits ElasticBase

#Region "Fields"

    Private _autocomplete As ElasticClusterSettingsIndex
    Private _enaioblue As ElasticClusterSettingsIndex
    Private _systeminfo As ElasticClusterSettingsIndex

#End Region

#Region "Properties"

    Public Property autocomplete As ElasticClusterSettingsIndex
        Get
            Return _autocomplete
        End Get
        Set(value As ElasticClusterSettingsIndex)
            _autocomplete = value
        End Set
    End Property

    Public Property enaioblue As ElasticClusterSettingsIndex
        Get
            Return _enaioblue
        End Get
        Set(value As ElasticClusterSettingsIndex)
            _enaioblue = value
        End Set
    End Property

    Public Property systeminfo As ElasticClusterSettingsIndex
        Get
            Return _systeminfo
        End Get
        Set(value As ElasticClusterSettingsIndex)
            _systeminfo = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument)
        MyBase.New(xml)
        _autocomplete = New ElasticClusterSettingsIndex(xml, "//autocomplete_0")
        _enaioblue = New ElasticClusterSettingsIndex(xml, "//enaioblue_0")
        _systeminfo = New ElasticClusterSettingsIndex(xml, "//systeminfo")
        parse()
    End Sub

#End Region

End Class

#End Region

#Region "_cat/shards?format=json"

Public Class ElasticCatShardsItem

#Region "Fields"

    Private xmlNode As Xml.XmlNode
    Private _sRegex As String
    Private _index As PropInfo
    Private _shard As PropInfo
    Private _prirep As PropInfo
    Private _state As PropInfo
    Private _docs As PropInfo
    Private _store As PropInfo
    Private _ip As PropInfo
    Private _node As PropInfo

#End Region

#Region "Properties"

    Public Property index As PropInfo
        Get
            Return _index
        End Get
        Set(value As PropInfo)
            _index = value
        End Set
    End Property

    Public Property shard As PropInfo
        Get
            Return _shard
        End Get
        Set(value As PropInfo)
            _shard = value
        End Set
    End Property

    Public Property prirep As PropInfo
        Get
            Return _prirep
        End Get
        Set(value As PropInfo)
            _prirep = value
        End Set
    End Property

    Public Property state As PropInfo
        Get
            Return _state
        End Get
        Set(value As PropInfo)
            _state = value
        End Set
    End Property

    Public Property docs As PropInfo
        Get
            Return _docs
        End Get
        Set(value As PropInfo)
            _docs = value
        End Set
    End Property

    Public Property store As PropInfo
        Get
            Return _store
        End Get
        Set(value As PropInfo)
            _store = value
        End Set
    End Property

    Public Property ip As PropInfo
        Get
            Return _ip
        End Get
        Set(value As PropInfo)
            _ip = value
        End Set
    End Property

    Public Property node As PropInfo
        Get
            Return _node
        End Get
        Set(value As PropInfo)
            Dim regex As New Regex(Me._sRegex)
            value.value = regex.Match(value.value).Value.TrimEnd()
            _node = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xmlNode As Xml.XmlNode, Optional ByVal sRegex As String = "^[a-zA-Z0-9 _]*")
        Me.xmlNode = xmlNode
        Me._sRegex = sRegex

        Me._index = New PropInfo(".//index")
        Me._shard = New PropInfo(".//shard")
        Me._prirep = New PropInfo(".//prirep")
        Me._state = New PropInfo(".//state")
        Me._docs = New PropInfo(".//docs")
        Me._store = New PropInfo(".//store")
        Me._ip = New PropInfo(".//ip")
        Me._node = New PropInfo(".//node")
        Me._node.value = "Unassigned"

        parse()
    End Sub

    Public Function GetProperties() As List(Of PropertyInfo)
        Return Me.GetType().GetProperties().ToList()
    End Function

    Public Function parse(Optional ByVal ignoreException As Boolean = True) As Boolean
        Dim properties As New List(Of PropertyInfo)

        properties = GetProperties()

        For Each propertie In properties
            Try
                Dim prop As PropInfo = CType(propertie.GetValue(Me), PropInfo)
                Dim value As String = xmlNode.SelectSingleNode(prop.location).InnerText

                If prop.location.Equals(".//node") Then
                    Dim regex As New Regex(Me._sRegex)
                    value = regex.Match(value).Value.TrimEnd()
                End If
                prop.value = value

            Catch ex As NullReferenceException
                If Not ignoreException Then
                    Return False
                End If
            End Try
        Next

        Return True
    End Function

#End Region

    Public Overrides Function ToString() As String
        Return String.Format("node: {0}, ip: {1}, index: {2}, shard: {3}, prirep: {4}, state: {5}, docs: {6}, store: {7} ", Me.node, Me.ip, Me.index, Me.shard, Me.prirep, Me.state, Me.docs, Me.store)
    End Function

End Class

Public Class ElasticCatShards
    Inherits ElasticBase

#Region "Fields"

    Private _items As New List(Of ElasticCatShardsItem)

#End Region

#Region "Properties"

    Public Property items As List(Of ElasticCatShardsItem)
        Get
            Return _items
        End Get
        Set(value As List(Of ElasticCatShardsItem))
            _items = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal xml As Xml.XmlDocument, Optional ByVal sRegex As String = "^[a-zA-Z0-9 _]*")
        MyBase.New(xml)

        For Each tempNode As Xml.XmlNode In xml.SelectNodes("//item")
            Me._items.Add(New ElasticCatShardsItem(tempNode, sRegex))
        Next
    End Sub

#End Region

#Region "SearchMethods"

    Public Function GetAllNodeNames() As List(Of String)
        Dim NodeList As New List(Of String)

        For Each item In _items
            If Not (NodeList.Contains(item.node.value)) Then
                NodeList.Add(item.node.value)
            End If
        Next

        Return NodeList
    End Function

    Public Function GetByNodeName(ByVal nodeName As String) As List(Of ElasticCatShardsItem)
        Dim nodesByNodeName = From item In _items
                              Where item.node.value = nodeName

        Return nodesByNodeName.ToList()
    End Function

    Public Function GetByNodeName(ByVal nodeName As String, ByVal prirep As String) As List(Of ElasticCatShardsItem)
        Dim nodesByNodeName = From item In _items
                              Where item.node.value = nodeName
                              Where item.prirep.value = prirep

        Return nodesByNodeName.ToList()
    End Function

    Public Function GetByNodeName(ByVal nodeName As String, ByVal shard As Integer) As List(Of ElasticCatShardsItem)
        Dim sShard As String = shard
        Dim nodesByNodeName = From item In _items
                              Where item.node.value = nodeName
                              Where item.shard.value = shard

        Return nodesByNodeName.ToList()
    End Function

    Public Function GetByNodeName(ByVal nodeName As String, ByVal index As String, ByVal shard As Integer) As List(Of ElasticCatShardsItem)
        Dim sShard As String = shard
        Dim nodesByNodeName = From item In _items
                              Where item.index.value = index
                              Where item.node.value = nodeName
                              Where item.shard.value = shard

        Return nodesByNodeName.ToList()
    End Function

    Public Function GetByState(ByVal state As String) As List(Of ElasticCatShardsItem)
        Dim nodesByState = From item In _items
                           Where item.state.value = state

        Return nodesByState.ToList()
    End Function

    Public Function GetByIndex(ByVal index As String) As List(Of ElasticCatShardsItem)
        Dim nodesByindex = From item In _items
                           Where item.index.value = index

        Return nodesByindex.ToList()
    End Function

    Public Function GetByIndex(ByVal index As String, ByVal nodeName As String) As List(Of ElasticCatShardsItem)
        Dim nodesByindex = From item In _items
                           Where item.index.value = index
                           Where item.node.value = nodeName

        Return nodesByindex.ToList()
    End Function

    Public Function GetByIndex(ByVal index As String, ByVal shard As Integer, ByVal replic As Boolean) As List(Of ElasticCatShardsItem)
        Dim prirep As String = "p"
        If replic Then
            prirep = "r"
        End If
        Dim nodesByindex = From item In _items
                           Where item.index.value = index
                           Where item.shard.value = shard
                           Where item.prirep.value = prirep

            Return nodesByindex.ToList()
    End Function

    Public Function GetByIP(ByVal ip As String) As List(Of ElasticCatShardsItem)
        Dim nodesByIp = From item In _items
                        Where item.ip.value = ip

        Return nodesByIp.ToList()
    End Function

#End Region

End Class

#End Region