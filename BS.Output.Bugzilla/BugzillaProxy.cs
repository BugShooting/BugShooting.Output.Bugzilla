using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace BS.Output.Bugzilla
{
  
  /// <summary>
  /// BugzillaProxy uses Bugzilla 5.x REST API
  /// </summary>
  internal class BugzillaProxy
  {

    // TODO handle api rest errors
    
    static internal async Task<Dictionary<string, Product>> GetEditableProducts(string apiUrl, string userName, string password)
    {

      string requestUrl = GetApiUrl(apiUrl, "product");

      requestUrl = AddParameter(requestUrl, "login", userName);
      requestUrl = AddParameter(requestUrl, "password", password);
      requestUrl = AddParameter(requestUrl, "type", "enterable");

      string requestResult = await GetData(requestUrl);

      Products productsResult = FromJson<Products>(requestResult);

      Dictionary<string, Product> products = new Dictionary<string, Product>();
      foreach (Product product in productsResult.Items)
      {
        products.Add(product.Name, product);
      }

      return products;

    }

    static internal async Task<BugFieldValues> GetBugFields(string apiUrl)
    {

      string requestUrl = GetApiUrl(apiUrl, "field/bug");
      string requestResult = await GetData(requestUrl);

      Fields fields = FromJson<Fields>(requestResult);

      List<FieldValue> operatingSystemValues = null;
      List<FieldValue> platformValues = null;
      List<FieldValue> priorityValues = null;
      List<FieldValue> severityValues = null;

      foreach (Field field in fields.Items)
      {
        switch (field.Name)
        { 
          case "op_sys":
            operatingSystemValues = field.Values;
            break;
          case "rep_platform":
            platformValues = field.Values;
            break;
          case "priority":
            priorityValues = field.Values;
            break;
          case "bug_severity":
            severityValues = field.Values;
            break;
        }
      }

      return new BugFieldValues(operatingSystemValues, platformValues, priorityValues, severityValues);

    }


    static internal async Task<BugCreateResult> BugCreate(string apiUrl,
                                                          string userName,
                                                          string password,
                                                          string product, 
                                                          string component, 
                                                          string version,
                                                          string operatingSystem ,
                                                          string platform ,
                                                          string priority,
                                                          string severity,
                                                          string summary, 
                                                          string description)
    {

      string requestUrl = GetApiUrl(apiUrl, "bug");

      requestUrl = AddParameter(requestUrl, "login", userName);
      requestUrl = AddParameter(requestUrl, "password", password);
        
      string requestData = string.Format("{{\"product\":\"{0}\"," +
                                          "\"component\":\"{1}\"," +
                                          "\"version\":\"{2}\"," +
                                          "\"op_sys\":\"{3}\"," +
                                          "\"rep_platform\":\"{4}\"," +
                                          "\"priority\":\"{5}\"," +
                                          "\"severity\":\"{6}\"," +
                                          "\"summary\":\"{7}\"," +
                                          "\"description\":\"{8}\"}}",
                                          product, component, version, operatingSystem, platform, priority, severity,  summary, description);


      string requestResult = await SendData(requestUrl, requestData);

      BugID bugID = FromJson<BugID>(requestResult);

      return new BugCreateResult(true, bugID.ID, null);
      
    }

    static internal async Task<BugAddAttachmentResult> BugAddAttachment(string apiUrl,
                                                                        string userName,
                                                                        string password,
                                                                        int bugID, 
                                                                        string comment, 
                                                                        Byte[] imageData,
                                                                        string fullFileName,
                                                                        string fileMimeType)
    {

      string requestUrl = GetApiUrl(apiUrl, String.Format("bug/{0}/attachment", bugID));

      requestUrl = AddParameter(requestUrl, "login", userName);
      requestUrl = AddParameter(requestUrl, "password", password);

      string requestData = string.Format("{{\"ids\":[{0}]," +
                                         "\"data\":\"{1}\"," +
                                         "\"file_name\":\"{2}\"," +
                                         "\"summary\":\"{3}\"," +
                                         "\"content_type\":\"{4}\"," +
                                         "\"comment\":\"{5}\"}}",
                                         bugID, Convert.ToBase64String(imageData), fullFileName, fullFileName, fileMimeType, comment);
      
      string requestResult = await SendData(requestUrl, requestData);
          
      return new BugAddAttachmentResult(true, null);
      
    }
    
    private static string GetResponseResult(string apiUrl, string requestString)
    {

      byte[] requestData = System.Text.Encoding.ASCII.GetBytes(requestString);

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);

      request.Method = "POST";
      request.ContentType = "text/xml";
      request.ContentLength = requestData.Length;

      using (Stream requestStream = request.GetRequestStream())
      {

        requestStream.Write(requestData, 0, requestData.Length);

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
          using (Stream stream = response.GetResponseStream())
          {
            using (StreamReader reader = new StreamReader(stream))
            {
              return reader.ReadToEnd();
            }
          }
        }

      }
      
    }

    private static bool CheckFaultExist(string responseString, ref string faultMessage)
    {

      XmlDocument doc = new XmlDocument();
      doc.LoadXml(responseString);
      
      if (doc.GetElementsByTagName("fault").Count > 0)
      {
        foreach (XmlNode memberNode in doc.GetElementsByTagName("member"))
        {
          foreach (XmlNode memberName in memberNode.ChildNodes)
          {

            if (memberName.Name.ToLower() == "name" && memberName.InnerText.ToLower() == "faultstring")
            {
              foreach (XmlNode memberValue in memberNode.ChildNodes)
              {
                if (memberValue.Name.ToLower() == "value")
                {
                  faultMessage = memberValue.ChildNodes[0].InnerText;
                  return true;
                }
              }

            }
          }
        }

      }

      return false;

    }

    private static string GetApiUrl(string url, string method)
    {

      string apiUrl = url;

      if (!(apiUrl.LastIndexOf("/") == apiUrl.Length - 1))
      {
        apiUrl += "/";
      }

      return apiUrl + "rest/" + method;

    }

    private static T FromJson<T>(string jsonText)
    {

      DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

      using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(jsonText)))
      {
        return (T)serializer.ReadObject(stream);
      }

    }

    private static string AddParameter(string requestUrl, string parameter, string value)
    {

      if (requestUrl.Contains("?"))
      {
        requestUrl += "&";
      }
      else
      {
        requestUrl += "?";
      }

      return requestUrl + parameter + "=" + HttpUtility.UrlEncode(value);

    }

    private static async Task<string> GetData(string requestUrl)
    {

      HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
      request.Method = "GET";
      request.Accept = "application/json";
      request.ContentType = "application/json";

      using (WebResponse response = await request.GetResponseAsync())
      {
        using (Stream stream = response.GetResponseStream())
        {
          using (StreamReader reader = new StreamReader(stream))
          {
            return await reader.ReadToEndAsync();
          }
        }
      }
    }

    private static async Task<string> SendData(string requestUrl, string data)
    {
      
      HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
      request.Method = "POST";
      request.Accept = "application/json";
      request.ContentType = "application/json";
     
      byte[] postData = Encoding.UTF8.GetBytes(data);
      request.ContentLength = postData.Length;

      using (Stream requestStream = request.GetRequestStream())
      {
        requestStream.Write(postData, 0, postData.Length);
        requestStream.Close();
      }

      using (WebResponse response = await request.GetResponseAsync())
      {
        using (Stream stream = response.GetResponseStream())
        {
          using (StreamReader reader = new StreamReader(stream))
          {
            return await reader.ReadToEndAsync();
          }
        }
      }

    }

  }

  [DataContract()]
  internal class Products
  {

    [DataMember(Name = "products")]
    public List<Product> Items { get; set; }
    
  }

  [DataContract()]
  internal class Fields
  {

    [DataMember(Name = "fields")]
    public List<Field> Items { get; set; }

  }

  [DataContract()]
  internal class Field
  {

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "values")]
    public List<FieldValue> Values { get; set; }

  }

  [DataContract()]
  internal class FieldValue
  {

    [DataMember(Name = "name")]
    public string Name { get; set; }

  }

  [DataContract()]
  internal class Product
  {

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "components")]
    public List<Component> Components { get; set; }

    [DataMember(Name = "versions")]
    public List<Version> Versions { get; set; }

  }

  [DataContract()]
  internal class BugID
  {

    [DataMember(Name = "id")]
    public int ID { get; set; }

  }

  [DataContract()]
  internal class Component
  {

    [DataMember(Name = "name")]
    public string Name { get; set; }

  }

  [DataContract()]
  internal class Version
  {

    [DataMember(Name = "name")]
    public string Name { get; set; }

  }

  internal class BugFieldValues
  {

    List<FieldValue> operatingSystemValues;
    List<FieldValue> platformValues;
    List<FieldValue> priorityValues;
    List<FieldValue> severityValues;

    public BugFieldValues(List<FieldValue> operatingSystemValues,
                          List<FieldValue> platformValues,
                          List<FieldValue> priorityValues,
                          List<FieldValue> severityValues)
    {
      this.operatingSystemValues = operatingSystemValues;
      this.platformValues = platformValues;
      this.priorityValues = priorityValues;
      this.severityValues = severityValues;
    }


    public List<FieldValue> OperatingSystemValues
    {
      get { return operatingSystemValues; }
    }

    public List<FieldValue> PlatformValues
    {
      get { return platformValues; }
    }

    public List<FieldValue> PriorityValues
    {
      get { return priorityValues; }
    }

    public List<FieldValue> SeverityValues
    { 
      get { return severityValues; }
    }
  }

  internal class BugCreateResult
  {

    bool success;
    int bugID;
    string faultMessage;

    public BugCreateResult(bool success,
                           int bugID, 
                           string faultMessage)
    {
      this.success = success;
      this.bugID = bugID;
      this.faultMessage = faultMessage;
    }


    public bool Success
    {
      get { return success; }
    }

    public int BugID
    {
      get { return bugID; }
    }

    public string FaultMessage
    {
      get { return faultMessage; }
    }

  }

  internal class BugAddAttachmentResult
  {

    bool success;
    string faultMessage;

    public BugAddAttachmentResult(bool success,
                           string faultMessage)
    {
      this.success = success;
      this.faultMessage = faultMessage;
    }


    public bool Success
    {
      get { return success; }
    }

    public string FaultMessage
    {
      get { return faultMessage; }
    }

  }

}
