Imports System.Data
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Xml
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports Microsoft.Win32

Public Class Converter

    Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Public Shared Function JsonToXML(jsonDocuments As String) As XmlDocument
        Dim reader As XmlDictionaryReader
        Dim xml As New XmlDocument

        reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.Default.GetBytes(jsonDocuments), XmlDictionaryReaderQuotas.Max)

        xml.Load(reader)

        Return xml
    End Function

End Class

Module HelperClass
    Public gsIPAdress As String

    Function GetClusterState()
        Try
            Dim webClient As New System.Net.WebClient
            Dim sResult As String = webClient.DownloadString("http://" + gsIPAdress + "/enaioblue_*/_settings/index.number_of_*")
            Dim sReplicas = Strings.Mid(sResult, (Strings.InStr(sResult, "cas") + 6))
            sReplicas = Strings.Left(sReplicas, (Strings.InStr(sReplicas, "}") - 2))
            Dim sShards = Strings.Mid(sResult, (Strings.InStr(sResult, "rds") + 6))
            sShards = Strings.Left(sShards, (Strings.InStr(sShards, ",") - 2))
            sResult = ""
            sResult = webClient.DownloadString("http://" + gsIPAdress + "/_cluster/health")
            Dim sHealth As String = Strings.Mid(sResult, Strings.InStr(sResult, "tus") + 6)
            sHealth = Strings.Left(sHealth, Strings.InStr(sHealth, ",") - 2)
            Dim aResult(2) As String
            aResult(0) = sReplicas
            aResult(1) = sShards
            aResult(2) = sHealth
            Return (aResult)
        Catch ex As Exception
            Return ("Fehler")
        End Try
    End Function

    Function getCluster() As XmlDocument
        Dim webClient As New System.Net.WebClient
        Dim sResult As String = webClient.DownloadString("http://" + gsIPAdress + "/enaioblue_*/_settings/")
        Dim xmlDoc As Xml.XmlDocument

        xmlDoc = Converter.JsonToXML(sResult)
        Return xmlDoc
    End Function

    Function PutReplicaUp() As String
        Try
            Dim uri As String = "http://" + gsIPAdress + "/enaioblue_0/_settings"
            Dim data = Encoding.UTF8.GetBytes("{index:{number_of_replicas:2}}")
            Dim result_post = SendRequest(uri, data, "application/json", "POST")
            Return (0)
            'Dim someurl As String = ("http://" + gsIPAdress + "/enaioblue_0/_settings")
            'Using client As New Net.WebClient
            '    Dim reqparm As New Specialized.NameValueCollection
            '    reqparm.Add("number_of_replicas", "2")
            '    'reqparm.Add("param2", "othervalue")
            '    Dim responsebytes = client.UploadValues(someurl, "POST", reqparm)
            '    Dim responsebody = (New Text.UTF8Encoding).GetString(responsebytes)
            'End Using

            'Dim sResult As String = webRequest.DownloadString("http://" + gsIPAdress + "/enaioblue_*/_settings/index.number_of_replicas")
            ' Dim sSplit = Strings.Mid(sResult, (Strings.InStr(sResult, "cas") + 6))
            'sSplit = Strings.Left(sSplit, (Strings.InStr(sSplit, "}") - 2))
            ' Dim iSplit As Integer = Convert.ToInt32(sSplit) + 1
            'Return (iSplit)
        Catch ex As Exception
            MsgBox(ex)
            Return ("0")
        End Try
    End Function

    Function GetElasticInstall() As Boolean
        Dim regKey As RegistryKey
        Try
            regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
            regKey = regKey.OpenSubKey("SOFTWARE\OPTIMAL SYSTEMS\elasticsearch", False)
            Return True
        Catch
            Return False
        End Try

    End Function

    Function PutReplicaDown()
        Try
            Dim uri As String = "http://" + gsIPAdress + "/enaioblue_0/_settings"
            Dim data = Encoding.UTF8.GetBytes("{index:{number_of_replicas:2}}")
            Dim result_post = SendRequest(uri, data, "application/json", "POST")
            Return (0)
            'Dim someurl As String = ("http://" + gsIPAdress + "/enaioblue_0/_settings")
            'Using client As New Net.WebClient
            '    Dim reqparm As New Specialized.NameValueCollection
            '    reqparm.Add("number_of_replicas", "1")
            '    'reqparm.Add("param2", "othervalue")
            '    Dim responsebytes = client.UploadValues(someurl, "POST", reqparm)
            '    Dim responsebody = (New Text.UTF8Encoding).GetString(responsebytes)
            'End Using
            'Dim webClient As New System.Net.WebClient
            'Dim sResult As String = webClient.DownloadString("http://" + gsIPAdress + "/enaioblue_*/_settings/index.number_of_replicas")
            'Dim sSplit = Strings.Mid(sResult, (Strings.InStr(sResult, "cas") + 6))
            'sSplit = Strings.Left(sSplit, (Strings.InStr(sSplit, "}") - 2))
            'Dim iSplit As Integer = Convert.ToInt32(sSplit) - 1
            'Return (iSplit)
        Catch ex As Exception
            'MsgBox(ex)
            Return (ex)
        End Try
    End Function

    Private Function SendRequest(uri As String, jsonDataBytes As Byte(), contentType As String, method As String) As String
        Dim req As WebRequest = WebRequest.Create(uri)
        req.ContentType = contentType
        req.Method = method
        req.ContentLength = jsonDataBytes.Length


        Dim stream = req.GetRequestStream()
        stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
        stream.Close()

        Dim response = req.GetResponse().GetResponseStream()

        Dim reader As New StreamReader(response)
        Dim res = reader.ReadToEnd()
        reader.Close()
        response.Close()

        Return res
    End Function

    Function GetJSON() As String
        Dim json As New System.Web.Script.Serialization.JavaScriptSerializer()

        Try
            'Dim xml_Doc As XmlDocument = New XmlDocument()
            'xml_Doc.Load("json.xml")
            'Return json.Serialize(DataSetToJSON(New DataSet().ReadXml(xml_Doc)))
            'reader 
            'Dim reader As XmlReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.Default.GetBytes(jsonDocuments), XmlDictionaryReaderQuotas.Max)
            'Dim Xml = New XmlDocument()
            'Xml.Load(reader)

        Catch ex As Exception
            MsgBox(ex.ToString)
            Return ("0")
        End Try

    End Function

    Function DataSetToJSON(ds As DataSet) As String
        Dim dict As New Dictionary(Of String, Object)

        For Each dt As DataTable In ds.Tables
            Dim arr(dt.Rows.Count) As Object

            For i As Integer = 0 To dt.Rows.Count - 1
                arr(i) = dt.Rows(i).ItemArray
            Next

            dict.Add(dt.TableName, arr)
        Next

        Dim json As New JavaScriptSerializer
        Return json.Serialize(dict)
    End Function

    Function WriteXML()
        Dim enc As New System.Text.UTF8Encoding
        Dim xmlString As XmlTextWriter = New XmlTextWriter("json.xml", enc)


        With xmlString
            'Formatierung festlegen(4er Einzug)
            '    .Formatting = Formatting.Indented
            ' .Indentation = 4
            'Start des Schreibens
            .WriteStartDocument()
            .WriteStartElement("index")
            .WriteElementString("number_of_replicas", "0")
            .WriteEndElement()
            .Close()
        End With
        Return xmlString

    End Function

    Function ReadXML(sRueckgabe As String)
        Const XMLDateiPfad As String = "C:\Users\administrator.TSE\Desktop\Debug\config.xml"
        Dim xml_Doc As XmlDocument                                  ' XML-Dokument (Datei)
        Dim xml_Wurzel As XmlNode                                   ' Stammknoten (Wurzelknoten)

        Dim xml_Knotenliste1 As XmlNodeList
        Dim xml_Knotenliste2 As XmlNodeList
        Dim xml_Knoten As XmlNodeList

        Dim Email As New Collection
        Dim Password As New Collection
        Dim Fehler As New Collection
        Fehler.Add("Fehler")
        ' logger.Debug("ReadConfigXML.Variablen definiert")
        xml_Doc = New XmlDocument()                                 ' Instanz bilden ...
        xml_Doc.Load(XMLDateiPfad)                                 ' ... XML-Datei laden

        xml_Wurzel = xml_Doc.DocumentElement()                      ' Auf den Wurzelknoten gehen
        xml_Knoten = xml_Wurzel.ChildNodes
        'logger.Debug("Wurzelkind: " & xml_Knoten.Item(0).Name)
        ' xml_Knotenliste1 = xml_Wurzel.ChildNodes                    ' Liste der Kinder direkt unterhalb des Wurzelknotens
        For Each C As XmlNode In xml_Knoten
            xml_Knotenliste1 = C.ChildNodes
            Select Case C.Name
                Case "Mail-Accounts"
                    '  logger.Debug("<><>")
                    '  logger.Debug("Auslesen der E-Mail-Adressen")
                    For Each K As XmlNode In xml_Knotenliste1
                        xml_Knotenliste2 = K.ChildNodes                         ' Liste der Kindknoten dieser Kinder
                        For Each KK As XmlNode In xml_Knotenliste2              ' Gemäß des Knotennamens den Wert speichern
                            If KK.Name = "E-Mail-Adresse" Then
                                'logger.Debug("E-Mail: " & KK.InnerText)
                                Email.Add(KK.InnerText)
                            ElseIf KK.Name = "Passwort" Then
                                'logger.Debug("Passwort: " & KK.InnerText)
                                Password.Add(KK.InnerText)
                            End If
                        Next
                    Next
                Case "Konfig-Test"
                    '  logger.Debug("<><>")
                  '  logger.Debug("Test-Konfig")
                Case "Attachments"
                    '   logger.Debug("<><>")
                    '   logger.Debug("Auslesen der Attachment-Pfade")
            End Select
            '  logger.Debug("><><")
        Next

        ' logger.Debug("+++")

        If sRueckgabe = "E-Mail" Then
            Return Email
        ElseIf sRueckgabe = "Password" Then
            Return Password
        Else
            Return Fehler
        End If
    End Function

    Sub AddLabel()

    End Sub

#Region "OBSOLETE"

    Function GetHealth()
        Try
            Dim webClient As New System.Net.WebClient
            Dim sResult As String = webClient.DownloadString("http://" + gsIPAdress + "/_cluster/health")
            Try
                Dim sSplit As String = Strings.Mid(sResult, Strings.InStr(sResult, "tus") + 6)
                sSplit = Strings.Left(sSplit, Strings.InStr(sSplit, ",") - 2)
                Return (sSplit)

            Catch ex As Exception
                Return ("Fehler bei Split")
            End Try
        Catch ex As Exception
            Return ("Fehler bei Get")
        End Try
    End Function

    Function GetShardsNumber()
        Try
            Dim webClient As New System.Net.WebClient
            Dim sResult As String = webClient.DownloadString("http://" + gsIPAdress + "/enaioblue_*/_settings/index.number_of_shards")
            Dim sSplit = Strings.Mid(sResult, (Strings.InStr(sResult, "rds") + 6))
            sSplit = Strings.Left(sSplit, (Strings.InStr(sSplit, "}") - 2))
            Return (sSplit)
        Catch ex As Exception
            Return ("Fehler")
        End Try
        'Dim sResult As String
        'Try
        '    Dim webClient As New System.Net.WebClient
        '    'Dim rawresp As String = webClient.DownloadString("http://" + gsIPAdress + "/enaioblue_*/_settings/index.number_of_replicas")
        '    Dim rawresp As String = webClient.DownloadString("{enaioblue_0: {settings: {index: {number_of_shards: ""6"",number_of_replicas: ""1""}}}}")
        '    Dim jss As New JavaScriptSerializer()
        '    Dim dict As Dictionary(Of String, String) = jss.Deserialize(Of Dictionary(Of String, String))(rawresp)

        '    sResult = dict("number_of_replicas")
        '    Return sResult
        'Catch ex As Exception
        '    Return ("Fe")
        'End Try

    End Function

    Function GetReplicasNumber()
        Try
            Dim webClient As New System.Net.WebClient
            Dim sResult As String = webClient.DownloadString("http://" + gsIPAdress + "/enaioblue_*/_settings/index.number_of_replicas")
            Dim sSplit = Strings.Mid(sResult, (Strings.InStr(sResult, "cas") + 6))
            sSplit = Strings.Left(sSplit, (Strings.InStr(sSplit, "}") - 2))
            Return (sSplit)
        Catch ex As Exception
            Return ("Fehler")
        End Try
    End Function
#End Region

End Module
