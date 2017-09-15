Imports System.Text
Imports System.Xml
Imports System.Runtime.Serialization.Json
Imports Microsoft.Win32
Imports System.Net
Imports System.IO

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
    Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Function GetJSONHttp(sURL As String)
        'Funktion zum Holen von JSON-Dateien, übergeben werden muss der Pfad nach Host und Port
        Dim sResult As String = ""
        Dim hRequest As WebRequest
        Dim hResult As WebResponse
        Dim responseFromServer As String = ""
        Dim dataStream As Stream
        Dim reader As StreamReader
        Log.Debug(gsIPAdress + sURL)
        hRequest = WebRequest.Create(gsIPAdress + sURL)
        hResult = hRequest.GetResponse()
        dataStream = hResult.GetResponseStream()
        reader = New StreamReader(dataStream)
        sResult = reader.ReadToEnd()
        Log.Debug("neu")
        Log.Debug(sResult)
        Log.Debug("#-#-#")
        Return sResult
        reader.Close()
        hResult.Close()

    End Function

    Public Sub PutReplica(iNumber)
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

    Function GetElasticInstall() As String()
        Dim regKey As RegistryKey
        Dim sPathElatic As String = ""
        Dim lines() As String
        Dim sReturn(1) As String

        Try
            regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
            regKey = regKey.OpenSubKey("SOFTWARE\OPTIMAL SYSTEMS\elasticsearch", False)
            sPathElatic = regKey.GetValue("Location")
            log.Debug(sPathElatic)
            lines = IO.File.ReadAllLines(sPathElatic & "\config\elasticsearch.yml")
            For i As Integer = 0 To lines.Length - 1
                If lines(i).StartsWith("network.host: ") Then
                    sReturn(0) = lines(i)
                    log.Debug(sReturn(0))
                    sReturn(0) = Strings.Mid(sReturn(0), (Strings.InStr(sReturn(0), ":") + 2))
                    Exit For
                End If
            Next
            For i As Integer = 0 To lines.Length - 1
                If lines(i).StartsWith("http.port: ") Then
                    sReturn(1) = lines(i)
                    log.Debug(sReturn(1))
                    sReturn(1) = Strings.Mid(sReturn(1), (Strings.InStr(sReturn(1), ":") + 2))
                    Exit For
                End If
            Next
            log.Debug(sReturn(0) + ":" + sReturn(1))

        Catch
            sReturn(0) = "10.10.77.182"
            sReturn(1) = "8041"
        End Try

        Return sReturn
    End Function

    Function GetDocumentCount(sShards As String) As Integer
        Dim iDocCount As Integer = 0
        Dim elasticcatshard As ElasticCatShards
        Dim sResult3 As String = ""
        Try
            log.Debug("GetDocumentCount")
            sResult3 = GetJSONHttp("/_cat/shards?format=json")
        Catch ex As Exception
            log.Error(gsIPAdress)
            log.Error(ex)
        End Try
        Int32.Parse(sShards)
        elasticcatshard = New ElasticCatShards(Converter.JsonToXML(sResult3))
        For i = 0 To Int32.Parse(sShards) - 1
            iDocCount += Int32.Parse(elasticcatshard.GetByIndex("enaioblue_0", i, False)(0).docs.value)
            log.Debug(iDocCount)
        Next
        Return iDocCount
    End Function

#Region "Testing"
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
#End Region

End Module
