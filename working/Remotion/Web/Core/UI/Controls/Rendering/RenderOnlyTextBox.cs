using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.Web.UI.Controls.Rendering
{
  [ToolboxItem (false)]
  public class RenderOnlyTextBox : TextBox
  {
    protected override void OnPreRender (EventArgs e)
    {
      throw new NotSupportedException();
    }

    protected override void Render (HtmlTextWriter writer)
    {
      base.OnPreRender (EventArgs.Empty);
      base.Render (writer);
    }
  }
}