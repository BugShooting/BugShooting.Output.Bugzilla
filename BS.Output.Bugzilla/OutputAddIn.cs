using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net;

namespace BS.Output.Bugzilla
{
  public class OutputAddIn: V3.OutputAddIn<Output>
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

    protected override OutputValueCollection SerializeOutput(Output Output)
    {

      OutputValueCollection outputValues = new OutputValueCollection();

      outputValues.Add(new OutputValue("Name", Output.Name));
      outputValues.Add(new OutputValue("Url", Output.Url));
      outputValues.Add(new OutputValue("UserName", Output.UserName));
      outputValues.Add(new OutputValue("Password",Output.Password, true));
      outputValues.Add(new OutputValue("OpenItemInBrowser", Convert.ToString(Output.OpenItemInBrowser)));
      outputValues.Add(new OutputValue("FileName", Output.FileName));
      outputValues.Add(new OutputValue("FileFormat", Output.FileFormat));
      outputValues.Add(new OutputValue("LastProduct", Output.LastProduct));
      outputValues.Add(new OutputValue("LastComponent", Output.LastComponent));
      outputValues.Add(new OutputValue("LastVersion", Output.LastVersion));
      outputValues.Add(new OutputValue("LastOperatingSystem", Output.LastOperatingSystem));
      outputValues.Add(new OutputValue("LastPlatform", Output.LastPlatform));
      outputValues.Add(new OutputValue("LastPriority", Output.LastPriority));
      outputValues.Add(new OutputValue("LastSeverity", Output.LastSeverity));
      outputValues.Add(new OutputValue("LastBugID", Convert.ToString(Output.LastBugID)));

      return outputValues;
      
    }

    protected override Output DeserializeOutput(OutputValueCollection OutputValues)
    {

      return new Output(OutputValues["Name", this.Name].Value,
                        OutputValues["Url", ""].Value, 
                        OutputValues["UserName", ""].Value,
                        OutputValues["Password", ""].Value, 
                        OutputValues["FileName", "Screenshot"].Value, 
                        OutputValues["FileFormat", ""].Value,
                        Convert.ToBoolean(OutputValues["OpenItemInBrowser", Convert.ToString(true)].Value),
                        OutputValues["LastProduct", string.Empty].Value,
                        OutputValues["LastComponent", string.Empty].Value,
                        OutputValues["LastVersion", string.Empty].Value,
                        OutputValues["LastOperatingSystem", string.Empty].Value,
                        OutputValues["LastPlatform", string.Empty].Value,
                        OutputValues["LastPriority", string.Empty].Value,
                        OutputValues["LastSeverity", string.Empty].Value,
                        Convert.ToInt32(OutputValues["LastBugID", "1"].Value));

    }

    protected override async Task<V3.SendResult> Send(IWin32Window Owner, Output Output, V3.ImageData ImageData)
    {

      try
      {

        string userName = Output.UserName;
        string password = Output.Password;
        bool showLogin = string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password);
        bool rememberCredentials = false;

        string fileName = V3.FileHelper.GetFileName(Output.FileName, Output.FileFormat, ImageData);

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
              return new V3.SendResult(V3.Result.Canceled);
            }

            userName = credentials.UserName;
            password = credentials.Password;
            rememberCredentials = credentials.Remember;

          }

          Dictionary<string, Product> products = await BugzillaProxy.GetEditableProducts(Output.Url, userName, password);
          BugFieldValues bugFieldValues = await BugzillaProxy.GetBugFields(Output.Url);

          // Show send window
          Send send = new Send(Output.Url,
                               Output.LastProduct,
                               Output.LastComponent,
                               Output.LastVersion,
                               Output.LastBugID,
                               products,
                               bugFieldValues,
                               userName,
                               password,
                               fileName);

          var sendOwnerHelper = new System.Windows.Interop.WindowInteropHelper(send);
          sendOwnerHelper.Owner = Owner.Handle;

          if (!send.ShowDialog() == true)
          {
            return new V3.SendResult(V3.Result.Canceled);
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
            if (!createResult.Success)
            {
              return new V3.SendResult(V3.Result.Failed, createResult.FaultMessage);
            }

            bugID = createResult.BugID;

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

          string fullFileName = send.FileName + "." + V3.FileHelper.GetFileExtention(Output.FileFormat);
          string mimeType = V3.FileHelper.GetMimeType(Output.FileFormat);
          byte[] fileBytes = V3.FileHelper.GetFileBytes(Output.FileFormat, ImageData);


          BugAddAttachmentResult attachResult = await BugzillaProxy.BugAddAttachment(Output.Url, userName, password, bugID, send.Comment, fileBytes, fullFileName, mimeType);

          if (!attachResult.Success)
          {
            return new V3.SendResult(V3.Result.Failed, attachResult.FaultMessage);
          }


          // Open bug in browser
          if (Output.OpenItemInBrowser)
          {
            V3.WebHelper.OpenUrl(Output.Url + "/show_bug.cgi?id=" + Convert.ToString(bugID));
          }

          return new V3.SendResult(V3.Result.Success,
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
        return new V3.SendResult(V3.Result.Failed, ex.Message);
      }

    }
      
  }
}
