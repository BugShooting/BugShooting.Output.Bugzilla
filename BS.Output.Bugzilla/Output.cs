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
    string lastOperatingSystem;
    string lastPlatform;
    string lastPriority;
    string lastSeverity;
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
                  string lastOperatingSystem,
                  string lastPlatform,
                  string lastPriority,
                  string lastSeverity,
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
      this.lastOperatingSystem = lastOperatingSystem;
      this.lastPlatform = lastPlatform;
      this.lastPriority = lastPriority;
      this.lastSeverity = lastSeverity;
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

    public string LastOperatingSystem
    {
      get { return lastOperatingSystem; }
    }

    public string LastPlatform
    {
      get { return lastPlatform; }
    }

    public string LastPriority
    {
      get { return lastPriority; }
    }

    public string LastSeverity
    {
      get { return lastSeverity; }
    }

    public int LastBugID
    {
      get { return LastBugID; }
    }

  }
}
