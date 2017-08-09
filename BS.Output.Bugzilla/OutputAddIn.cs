using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

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
                          Output.LastOPSys,
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
      outputValues.Add(new OutputValue("LastOPSys", Output.LastOPSys));
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
                        OutputValues["LastOPSys", string.Empty].Value,
                        OutputValues["LastPlatform", string.Empty].Value,
                        OutputValues["LastPriority", string.Empty].Value,
                        OutputValues["LastSeverity", string.Empty].Value,
                        Convert.ToInt32(OutputValues["LastBugID", "1"].Value));

    }

    protected override async Task<V3.SendResult> Send(IWin32Window Owner, Output Output, V3.ImageData ImageData)
    {

      // TODO
      return null;
          
    }
      
  }
}
