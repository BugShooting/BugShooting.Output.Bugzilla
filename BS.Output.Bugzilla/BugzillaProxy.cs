using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Runtime.Serialization.Json;
using System.Text;

namespace BS.Output.Bugzilla
{

  XXXXXXXXXXXXX ist noch nicht fertig umgesetzt

  /// <summary>
  /// BugzillaProxy uses Bugzilla 5.x REST API
  /// </summary>
  internal class BugzillaProxy
  {
    
    static internal async Task<Dictionary<string, Product>> GetEditableProductsAsync(string apiUrl, string userName, string password)
    {
      return await Task.Factory.StartNew(() => GetEditableProducts(apiUrl, userName, password));
    }

    static private Dictionary<string, Product> GetEditableProducts(string apiUrl, string userName, string password)
    {

      string requestUri = GetRequestUri(apiUrl, "product");

      requestUri = AddParameter(requestUri, "login", userName);
      requestUri = AddParameter(requestUri, "password", password);
      requestUri = AddParameter(requestUri, "type", "enterable");

      string requestResult = GetData(requestUri);

      DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Products));
      using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(requestResult)))
      {

        Dictionary<string, Product> products = new Dictionary<string, Product>();

        foreach (Product product in ((Products)serializer.ReadObject(stream)).Items)
        {
          products.Add(product.Name, product);
        }

        return products;
      }

    }

    static internal async Task<BugCreateResult> BugCreateAsync(string apiUrl,
                                                               string userName,
                                                               string password,
                                                               string product, 
                                                               string component, 
                                                               string version, 
                                                               string summary, 
                                                               string description)
    {
      return await Task.Factory.StartNew(() => BugCreate(apiUrl, userName, password, product, component, version, summary, description));
    }

    static private BugCreateResult BugCreate(string apiUrl,
                                             string userName,
                                             string password,
                                             string product,
                                             string component,
                                             string version,
                                             string summary,
                                             string description)
    {

      string requestUri = GetRequestUri(apiUrl, "bug");

      requestUri = AddParameter(requestUri, "login", userName);
      requestUri = AddParameter(requestUri, "password", password);

      string requestData = string.Format("{{\"product\":\"{0}\"," +
                                          "\"component\":\"{1}\"," +
                                          "\"version\":\"{2}\"," +
                                          "\"op_sys\":\"XXXXXXXXXXXXXXXXXX\"," +
                                          "\"platform\":\"XXXXXXXXXXXXXXXXXX\"," +
                                          "\"priority\":\"XXXXXXXXXXXXXXXXXX\"," +
                                          "\"severity\":\"XXXXXXXXXXXXXXXXXX\"," +
                                          "\"summary\":\"{3}\"," +
                                          "\"description\":\"{4}\"}}",
                                          product, component,version,summary,description);


      string requestResult = PostData(requestUri, requestData);

      DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Products));
      using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(requestResult)))
      {

        //Dictionary<string, Product> products = new Dictionary<string, Product>();

        //foreach (Product product in ((Products)serializer.ReadObject(stream)).Items)
        //{
        //  products.Add(product.Name, product);
        //}

        return null;
      }
       
    }

    static internal async Task<BugAddAttachmentResult> BugAddAttachmentAsync(string apiUrl,
                                                                             string userName,
                                                                             string password,
                                                                             Int32 bugID, 
                                                                             string comment, 
                                                                             Byte[] imageData,
                                                                             string fullFileName,
                                                                             string mimeType)
    {
      return await Task.Factory.StartNew(() => BugAddAttachment(apiUrl, userName, password, bugID, comment, imageData, fullFileName, mimeType));
    }

    static private BugAddAttachmentResult BugAddAttachment(string apiUrl,
                                                           string userName,
                                                           string password,
                                                           Int32 bugID,
                                                           string comment,
                                                           Byte[] imageData,
                                                           string fullFileName,
                                                           string mimeType)
    {

      XmlDocument doc = new XmlDocument();
      XmlNode node = default(XmlNode);

      doc.AppendChild(doc.CreateXmlDeclaration("1.0", string.Empty, string.Empty));

      XmlNode methodCallNode = doc.AppendChild(doc.CreateElement("methodCall"));

      node = methodCallNode.AppendChild(doc.CreateElement("methodName"));
      node.InnerText = "Bug.add_attachment";

      node = methodCallNode.AppendChild(doc.CreateElement("params"));
      node = node.AppendChild(doc.CreateElement("param"));
      node = node.AppendChild(doc.CreateElement("value"));

      XmlNode structNode = node.AppendChild(doc.CreateElement("struct"));

      XmlNode memberNode = default(XmlNode);

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "login";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = userName;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "password";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = password;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "ids";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("array"));
      node = node.AppendChild(doc.CreateElement("data"));
      node = node.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("i4"));
      node.InnerText = bugID.ToString();

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "data";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("base64"));
      node.InnerText = Convert.ToBase64String(imageData); ;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "file_name";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = fullFileName;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "summary";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = fullFileName;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "comment";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = comment;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "content_type";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = mimeType;

      string responseResult = GetResponseResult(apiUrl, doc.OuterXml);

      string faultMessage = null;
      if (CheckFaultExist(responseResult, ref faultMessage))
      {
        return new BugAddAttachmentResult(false, faultMessage);
      }
      else
      {
        return new BugAddAttachmentResult(true, null);
      }

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

    private static string GetRequestUri(string url, string method)
    {

      string apiUrl = url;

      if (!(apiUrl.LastIndexOf("/") == apiUrl.Length - 1))
      {
        apiUrl += "/";
      }

      return apiUrl + "rest/" + method;

    }

    private static string AddParameter(string requestUri, string parameter, string value)
    {

      if (requestUri.Contains("?"))
      {
        requestUri += "&";
      }
      else
      {
        requestUri += "?";
      }

      return requestUri + parameter + "=" + HttpUtility.UrlEncode(value);

    }

    private static string GetData(string requestUri)
    {

      HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
      request.Method = "GET";
      request.Accept = "application/json";
      request.ContentType = "application/json";

      using (WebResponse response = request.GetResponse())
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

    private static string PostData(string requestUri, string data)
    {
      
      HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
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

      using (WebResponse response = request.GetResponse())
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

  [System.Runtime.Serialization.DataContract()]
  internal class Products
  {

    [System.Runtime.Serialization.DataMember(Name = "products")]
    public List<Product> Items { get; set; }
    
  }

  [System.Runtime.Serialization.DataContract()]
  internal class Product
  {

    [System.Runtime.Serialization.DataMember(Name = "name")]
    public string Name { get; set; }

    [System.Runtime.Serialization.DataMember(Name = "components")]
    public List<Component> Components { get; set; }

    [System.Runtime.Serialization.DataMember(Name = "versions")]
    public List<Version> Versions { get; set; }

  }

  [System.Runtime.Serialization.DataContract()]
  internal class Component
  {

    [System.Runtime.Serialization.DataMember(Name = "name")]
    public string Name { get; set; }

  }

  [System.Runtime.Serialization.DataContract()]
  internal class Version
  {

    [System.Runtime.Serialization.DataMember(Name = "name")]
    public string Name { get; set; }

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
