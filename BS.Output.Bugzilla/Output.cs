namespace BS.Output.Bugzilla
{

  public class Output: IOutput 
  {
    
    string name;
    string url;
    string userName;
    string password;
    string fileName;
    string fileFormat;
    bool openItemInBrowser;
    string lastProduct;
    string lastComponent;
    string lastVersion;
    int lastBugID;

    public Output(string name, 
                  string url, 
                  string userName,
                  string password, 
                  string fileName, 
                  string fileFormat,
                  bool openItemInBrowser,
                  string lastProduct,
                  string lastComponent,
                  string lastVersion,
                  int lastBugID)
    {
      this.name = name;
      this.url = url;
      this.userName = userName;
      this.password = password;
      this.fileName = fileName;
      this.fileFormat = fileFormat;
      this.openItemInBrowser = openItemInBrowser;
      this.lastProduct = lastProduct;
      this.lastComponent = lastComponent;
      this.lastVersion = lastVersion;
      this.lastBugID = lastBugID;
    }
    
    public string Name
    {
      get { return name; }
    }

    public string Information
    {
      get { return url; }
    }

    public string Url
    {
      get { return url; }
    }
       
    public string UserName
    {
      get { return userName; }
    }

    public string Password
    {
      get { return password; }
    }
          
    public string FileName
    {
      get { return fileName; }
    }

    public string FileFormat
    {
      get { return fileFormat; }
    }

    public bool OpenItemInBrowser
    {
      get { return openItemInBrowser; }
    }
    
    public string LastProduct
    {
      get { return lastProduct; }
    }

    public string LastComponent
    {
      get { return lastComponent; }
    }

    public string LastVersion
    {
      get { return lastVersion; }
    }
    
    public int LastBugID
    {
      get { return lastBugID; }
    }

  }
}
