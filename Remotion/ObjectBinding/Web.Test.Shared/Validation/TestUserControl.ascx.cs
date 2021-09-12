using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Test.Shared.Validation
{
  public partial class TestUserControl : DataEditUserControl, IControl, IFormGridRowProvider
  {
    protected void Page_Load (object sender, EventArgs e)
    {

    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public StringCollection GetHiddenRows (HtmlTable table)
    {
      return new StringCollection();
    }

    public FormGridRowInfoCollection GetAdditionalRows (HtmlTable table)
    {
      return new FormGridRowInfoCollection();
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate();
      isValid &= FormGridManager.Validate();
      return isValid;
    }
  }
}
