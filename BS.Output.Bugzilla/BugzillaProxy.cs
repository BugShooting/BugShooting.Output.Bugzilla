using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;
using System.Threading.Tasks;

namespace BS.Output.Bugzilla
{

  internal class BugzillaProxy
  {

    static internal async Task<bool> Login(string apiUrl, string userName, string password, ref CookieCollection loginCookies)
    {

      XmlDocument doc = new XmlDocument();
      XmlNode node = default(XmlNode);

      doc.AppendChild(doc.CreateXmlDeclaration("1.0", string.Empty, string.Empty));

      XmlNode methodCallNode = doc.AppendChild(doc.CreateElement("methodCall"));

      node = methodCallNode.AppendChild(doc.CreateElement("methodName"));
      node.InnerText = "User.login";

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
      node.InnerText = "remember";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("boolean"));
      node.InnerText = "0";


      using (HttpWebResponse response = GetResponse(apiUrl,  null, doc.OuterXml))
      {

        string faultMessage = string.Empty;
        if (CheckFaultExist(response, ref faultMessage))
        {
          return false;
        }

        loginCookies = response.Cookies;
        return true;

      }

    }

    static internal async Task<bool> BugCreate(string apiUrl, 
                                               CookieCollection loginCookies, 
                                               string product, 
                                               string component, 
                                               string version, 
                                               string opSys, 
                                               string platform, 
                                               string priority, 
                                               string severity,
                                               string summary, 
                                               string description, 
                                               ref Int32 bugID, 
                                               ref string faultMessage)
    {

      XmlDocument doc = new XmlDocument();
      XmlNode node = default(XmlNode);

      doc.AppendChild(doc.CreateXmlDeclaration("1.0", string.Empty, string.Empty));

      XmlNode methodCallNode = doc.AppendChild(doc.CreateElement("methodCall"));

      node = methodCallNode.AppendChild(doc.CreateElement("methodName"));
      node.InnerText = "Bug.create";

      node = methodCallNode.AppendChild(doc.CreateElement("params"));
      node = node.AppendChild(doc.CreateElement("param"));
      node = node.AppendChild(doc.CreateElement("value"));

      XmlNode structNode = node.AppendChild(doc.CreateElement("struct"));

      XmlNode memberNode = default(XmlNode);

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "product";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = product;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "component";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = component;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "summary";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = summary;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "version";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = version;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "description";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = description;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "op_sys";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = opSys;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "platform";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = platform;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "priority";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = priority;

      memberNode = structNode.AppendChild(doc.CreateElement("member"));
      node = memberNode.AppendChild(doc.CreateElement("name"));
      node.InnerText = "severity";
      node = memberNode.AppendChild(doc.CreateElement("value"));
      node = node.AppendChild(doc.CreateElement("string"));
      node.InnerText = severity;


      using (HttpWebResponse response = GetResponse(apiUrl, loginCookies, doc.OuterXml))
      {

        using (Stream stream = response.GetResponseStream())
        {
          using (StreamReader reader = new StreamReader(stream))
          {

            string responseString = reader.ReadToEnd();

            if (CheckFaultExist(responseString, ref faultMessage))
            {
              return false;

            }
            else
            {
              XmlDocument responseDoc = new XmlDocument();
              responseDoc.LoadXml(responseString);

              bugID = Convert.ToInt32(responseDoc.DocumentElement.SelectSingleNode("descendant::value/int").InnerText);

              return true;

            }

          }
        }

      }

    }

    static internal async Task<bool> BugAddAttachment(string apiUrl, 
                                                      CookieCollection loginCookies, 
                                                      Int32 bugID, 
                                                      string comment, 
                                                      Byte[] imageData,
                                                      string fullFileName,
                                                      string mimeType,
                                                      ref string faultMessage)
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

      using (HttpWebResponse response = GetResponse(apiUrl, loginCookies, doc.OuterXml))
      {
        return !CheckFaultExist(response, ref faultMessage);
      }

    }

    static internal async Task<List<Item>> GetEditableProducts(string apiUrl, CookieCollection loginCookies)
    {

      string productIDRequest = "<?xml version=\"1.0\"?>" + "<methodCall>" + "<methodName>Product.get_enterable_products</methodName>" + "</methodCall>";

      XmlDocument productIDDoc = new XmlDocument();
      productIDDoc.LoadXml(GetResponseResult(apiUrl, loginCookies, productIDRequest));

      List<string> prodctIDs = new List<string>();
      foreach (XmlNode node in productIDDoc.GetElementsByTagName("int"))
      {
        prodctIDs.Add(node.InnerText);
      }

      // ---------------------------------------------------------------------------------

      string productRequest = "<?xml version=\"1.0\"?>" + "<methodCall>" + "<methodName>Product.get</methodName>" + "<params>" + "<param>" + "<value>" + "<struct>" + "<member>" + "<name>ids</name>" + "<value>" + "<array>" + "<data>";

      foreach (string productID in prodctIDs)
      {
        productRequest += "<value><i4>" + productID + "</i4></value>";
      }

      productRequest += "</data>" + "</array>" + "</value>" + "</member>" + "</struct>" + "</value>" + "</param>" + "</params>" + "</methodCall>";

      XmlDocument productDoc = new XmlDocument();
      productDoc.LoadXml(GetResponseResult(apiUrl,  loginCookies, productRequest));

      Dictionary<string, Item> products = new Dictionary<string, Item>();

      foreach (XmlNode node in productDoc.GetElementsByTagName("member"))
      {

        foreach (XmlNode nameNode in node.ChildNodes)
        {

          if (nameNode.Name.ToLower() == "name" && nameNode.InnerText.ToLower() == "name")
          {

            foreach (XmlNode valueNode in node.ChildNodes)
            {

              if (valueNode.Name.ToLower() == "value")
              {
                if (!products.ContainsKey(valueNode.InnerText))
                {
                  products.Add(valueNode.InnerText, new Item(valueNode.InnerText));
                }
                break;
              }

            }

            break;
          }

        }
      }

      return new List<Item>(products.Values);

    }

    static internal async Task<Dictionary<string, ProductDetails>> GetProductDetails(string apiUrl, CookieCollection loginCookies, List<Item> products)
    {

      string request = "<?xml version=\"1.0\"?>" + "<methodCall>" + "<methodName>Bug.fields</methodName>" + "</methodCall>";

      XmlDocument doc = new XmlDocument();
      doc.LoadXml(GetResponseResult(apiUrl,  loginCookies, request));


      Dictionary<string, ProductDetails> productDetails = new Dictionary<string, ProductDetails>();
      foreach (Item product in products)
      {
        productDetails.Add(product.Name, new ProductDetails());
      }

      FillProductDetails(doc, "component", productDetails);
      FillProductDetails(doc, "version", productDetails);
      FillProductDetails(doc, "op_sys", productDetails);
      FillProductDetails(doc, "rep_platform", productDetails);
      FillProductDetails(doc, "priority", productDetails);
      FillProductDetails(doc, "bug_severity", productDetails);

      return productDetails;

    }

    private static void FillProductDetails(XmlDocument doc, string itemName, Dictionary<string, ProductDetails> productDetails)
    {
      XmlNode rootNode = doc.DocumentElement.SelectSingleNode("descendant::value[struct/member/name='name' and struct/member/value/string='" + itemName + "']").SelectSingleNode("descendant::member[name='values']");

      foreach (XmlNode itemNode in rootNode.SelectNodes("descendant::struct[member/name='visibility_values']"))
      {
        XmlNode productNode = itemNode.SelectSingleNode("descendant:: member/value/array/data/value/string");
        XmlNode nameNode = itemNode.SelectSingleNode("descendant::member[name='name']").SelectSingleNode("descendant::value/string");


        if ((productNode == null))
        {
          foreach (string product in productDetails.Keys)
          {
            productDetails[product].Add(itemName, nameNode.InnerText);
          }
        }
        else
        {
          string product = productNode.InnerText;

          if (productDetails.ContainsKey(product))
          {
            productDetails[product].Add(itemName, nameNode.InnerText);
          }

        }

      }

    }
    
    private static HttpWebResponse GetResponse(string apiUrl, CookieCollection cookies, string requestString)
    {

      byte[] requestData = System.Text.Encoding.ASCII.GetBytes(requestString);

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
      
      request.Method = "POST";
      request.ContentType = "text/xml";
      request.ContentLength = requestData.Length;

      request.CookieContainer = new CookieContainer();
      if ((cookies != null))
      {
        request.CookieContainer.Add(cookies);
      }

      using (Stream requestStream = request.GetRequestStream())
      {

        requestStream.Write(requestData, 0, requestData.Length);

        return (HttpWebResponse)request.GetResponse();

      }

    }

    private static string GetResponseResult(string apiUrl, CookieCollection cookies, string requestString)
    {

      using (HttpWebResponse response = GetResponse(apiUrl, cookies, requestString))
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

    private static bool CheckFaultExist(HttpWebResponse response, ref string faultMessage)
    {

      using (Stream stream = response.GetResponseStream())
      {
        using (StreamReader reader = new StreamReader(stream))
        {

          return CheckFaultExist(reader.ReadToEnd(), ref faultMessage);

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

  }


  internal class Item
  {

    private string name;

    public Item(string name)
    {
      this.name = name;
    }

    public string Name
    {
      get { return name; }
    }

  }

  internal class ProductDetails
  {
    
    Dictionary<string, List<Item>> items;

    public ProductDetails()
    {
      items = new Dictionary<string, List<Item>>();
      items.Add("component", new List<Item>());
      items.Add("version", new List<Item>());
      items.Add("op_sys", new List<Item>());
      items.Add("rep_platform", new List<Item>());
      items.Add("priority", new List<Item>());
      items.Add("bug_severity", new List<Item>());
    }

    public void Add(string name, string value)
    {
      items[name].Add(new Item(value));
    }

    public List<Item> Components
    {
      get { return items["component"]; }
    }

    public List<Item> Versions
    {
      get { return items["version"]; }
    }

    public List<Item> OPSys
    {
      get { return items["op_sys"]; }
    }

    public List<Item> Platforms
    {
      get { return items["rep_platform"]; }
    }

    public List<Item> Priority
    {
      get { return items["priority"]; }
    }

    public List<Item> Severity
    {
      get { return items["bug_severity"]; }
    }

  }

}
