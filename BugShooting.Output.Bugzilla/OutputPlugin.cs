using BS.Plugin.V3.Common;
using BS.Plugin.V3.Output;
using BS.Plugin.V3.Utilities;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BugShooting.Output.Bugzilla
{
  public class OutputPlugin: OutputPlugin<Output>
  {

    protected override string Name
    {
      get { return "Bugzilla"; }
    }

    protected override Image Image64
    {
      get  { return Properties.Resources.logo_64; }
    }

    protected override Image Image16
    {
      get { return Properties.Resources.logo_16 ; }
    }

    protected override bool Editable
    {
      get { return true; }
    }

    protected override string Description
    {
      get { return "Attach screenshots to Bugzilla bugs."; }
    }
    
    protected override Output CreateOutput(IWin32Window Owner)
    {
      
      Output output = new Output(Name, 
                                 String.Empty, 
                                 String.Empty, 
                                 String.Empty, 
                                 "Screenshot",
                                 String.Empty, 
                                 true,
                                 String.Empty,
                                 String.Empty,
                                 String.Empty,
                                 String.Empty,
                                 String.Empty,
                                 String.Empty,
                                 String.Empty,
                                 1);

      return EditOutput(Owner, output);

    }

    protected override Output EditOutput(IWin32Window Owner, Output Output)
    {

      Edit edit = new Edit(Output);

      var ownerHelper = new System.Windows.Interop.WindowInteropHelper(edit);
      ownerHelper.Owner = Owner.Handle;
      
      if (edit.ShowDialog() == true) {

        return new Output(edit.OutputName,
                          edit.Url,
                          edit.UserName,
                          edit.Password,
                          edit.FileName,
                          edit.FileFormat,
                          edit.OpenItemInBrowser,
                          Output.LastProduct,
                          Output.LastComponent,
                          Output.LastVersion,
                          Output.LastOperatingSystem,
                          Output.LastPlatform,
                          Output.LastPriority,
                          Output.LastSeverity,
                          Output.LastBugID);
      }
      else
      {
        return null; 
      }

    }

    protected override OutputValues SerializeOutput(Output Output)
    {

      OutputValues outputValues = new OutputValues();

      outputValues.Add("Name", Output.Name);
      outputValues.Add("Url", Output.Url);
      outputValues.Add("UserName", Output.UserName);
      outputValues.Add("Password",Output.Password, true);
      outputValues.Add("OpenItemInBrowser", Convert.ToString(Output.OpenItemInBrowser));
      outputValues.Add("FileName", Output.FileName);
      outputValues.Add("FileFormat", Output.FileFormat);
      outputValues.Add("LastProduct", Output.LastProduct);
      outputValues.Add("LastComponent", Output.LastComponent);
      outputValues.Add("LastVersion", Output.LastVersion);
      outputValues.Add("LastOperatingSystem", Output.LastOperatingSystem);
      outputValues.Add("LastPlatform", Output.LastPlatform);
      outputValues.Add("LastPriority", Output.LastPriority);
      outputValues.Add("LastSeverity", Output.LastSeverity);
      outputValues.Add("LastBugID", Convert.ToString(Output.LastBugID));

      return outputValues;
      
    }

    protected override Output DeserializeOutput(OutputValues OutputValues)
    {

      return new Output(OutputValues["Name", this.Name],
                        OutputValues["Url", ""], 
                        OutputValues["UserName", ""],
                        OutputValues["Password", ""], 
                        OutputValues["FileName", "Screenshot"], 
                        OutputValues["FileFormat", ""],
                        Convert.ToBoolean(OutputValues["OpenItemInBrowser", Convert.ToString(true)]),
                        OutputValues["LastProduct", string.Empty],
                        OutputValues["LastComponent", string.Empty],
                        OutputValues["LastVersion", string.Empty],
                        OutputValues["LastOperatingSystem", string.Empty],
                        OutputValues["LastPlatform", string.Empty],
                        OutputValues["LastPriority", string.Empty],
                        OutputValues["LastSeverity", string.Empty],
                        Convert.ToInt32(OutputValues["LastBugID", "1"]));

    }

    protected override async Task<SendResult> Send(IWin32Window Owner, Output Output, ImageData ImageData)
    {

      try
      {

        string userName = Output.UserName;
        string password = Output.Password;
        bool showLogin = string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password);
        bool rememberCredentials = false;

        string fileName = FileHelper.GetFileName(Output.FileName, ImageData);

        while (true)
        {

          if (showLogin)
          {

            // Show credentials window
            Credentials credentials = new Credentials(Output.Url, userName, password, rememberCredentials);

            var credentialsOwnerHelper = new System.Windows.Interop.WindowInteropHelper(credentials);
            credentialsOwnerHelper.Owner = Owner.Handle;

            if (credentials.ShowDialog() != true)
            {
              return new SendResult(Result.Canceled);
            }

            userName = credentials.UserName;
            password = credentials.Password;
            rememberCredentials = credentials.Remember;

          }

          GetProductsResult productsResult = await BugzillaProxy.GetEditableProducts(Output.Url, userName, password);
          switch (productsResult.Status)
          {
            case ResultStatus.Success:
              break;
            case ResultStatus.LoginFailed:
              showLogin = true;
              continue;
            case ResultStatus.Failed:
              return new SendResult(Result.Failed, productsResult.FailedMessage);
          }

          GetBugFieldsResult bugFieldsResult = await BugzillaProxy.GetBugFields(Output.Url, userName, password);
          switch (bugFieldsResult.Status)
          {
            case ResultStatus.Success:
              break;
            case ResultStatus.LoginFailed:
              showLogin = true;
              continue;
            case ResultStatus.Failed:
              return new SendResult(Result.Failed, bugFieldsResult.FailedMessage);
          }

          // Show send window
          Send send = new Send(Output.Url,
                               Output.LastProduct,
                               Output.LastComponent,
                               Output.LastVersion,
                               Output.LastOperatingSystem,
                               Output.LastPlatform,
                               Output.LastPriority,
                               Output.LastSeverity,
                               Output.LastBugID,
                               productsResult.Products,
                               bugFieldsResult.OperatingSystemValues,
                               bugFieldsResult.PlatformValues,
                               bugFieldsResult.PriorityValues,
                               bugFieldsResult.SeverityValues,
                               userName,
                               password,
                               fileName);

          var sendOwnerHelper = new System.Windows.Interop.WindowInteropHelper(send);
          sendOwnerHelper.Owner = Owner.Handle;

          if (!send.ShowDialog() == true)
          {
            return new SendResult(Result.Canceled);
          }

          int bugID = 1;
          string product = null;
          string component = null;
          string version = null;
          string operatingSystem = null;
          string platform = null;
          string priority = null;
          string severity = null;

          if (send.CreateNewBug)
          {
            product = send.Product;
            component = send.Component;
            version = send.Version;
            operatingSystem = send.OperatingSystem;
            platform = send.Platform;
            priority = send.Priority;
            severity = send.Severity;

            BugCreateResult createResult = await BugzillaProxy.BugCreate(Output.Url,
                                                                         userName,
                                                                         password,
                                                                         product,
                                                                         component,
                                                                         version,
                                                                         operatingSystem,
                                                                         platform,
                                                                         priority,
                                                                         severity,
                                                                         send.Summary,
                                                                         send.Description);
            switch (createResult.Status)
            {
              case ResultStatus.Success:
                bugID = createResult.BugID;
                break;
              case ResultStatus.LoginFailed:
                showLogin = true;
                continue;
              case ResultStatus.Failed:
                return new SendResult(Result.Failed, createResult.FailedMessage);
            }
          
          }
          else
          {
            product = Output.LastProduct;
            component = Output.LastComponent;
            version = Output.LastVersion;
            operatingSystem = Output.LastOperatingSystem;
            platform = Output.LastPlatform;
            priority = Output.LastPriority;
            severity = Output.LastSeverity;
            bugID = send.BugID;

          }

          string fullFileName = send.FileName + "." + FileHelper.GetFileExtention(Output.FileFormat);
          string mimeType = FileHelper.GetMimeType(Output.FileFormat);
          byte[] fileBytes = FileHelper.GetFileBytes(Output.FileFormat, ImageData);


          BugAddAttachmentResult attachResult = await BugzillaProxy.BugAddAttachment(Output.Url, userName, password, bugID, send.Comment, fileBytes, fullFileName, mimeType);
          switch (attachResult.Status)
          {
            case ResultStatus.Success:
              break;
            case ResultStatus.LoginFailed:
              showLogin = true;
              continue;
            case ResultStatus.Failed:
              return new SendResult(Result.Failed, attachResult.FailedMessage);
          }



          // Open bug in browser
          if (Output.OpenItemInBrowser)
          {
            WebHelper.OpenUrl(Output.Url + "/show_bug.cgi?id=" + Convert.ToString(bugID));
          }

          return new SendResult(Result.Success,
                                new Output(Output.Name,
                                          Output.Url,
                                          (rememberCredentials) ? userName : Output.UserName,
                                          (rememberCredentials) ? password : Output.Password,
                                          Output.FileName,
                                          Output.FileFormat,
                                          Output.OpenItemInBrowser,
                                          product,
                                          component,
                                          version,
                                          operatingSystem,
                                          platform,
                                          priority,
                                          severity,
                                          bugID));

        }
        
      }
      catch (Exception ex)
      {
        return new SendResult(Result.Failed, ex.Message);
      }

    }
      
  }
}
