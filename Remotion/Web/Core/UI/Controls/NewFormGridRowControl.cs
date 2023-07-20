using System.Web.UI;

namespace Remotion.Web.UI.Controls
{
  public class NewFormGridRowControl : Control
  {
    private readonly NewFormGridRow _row;

    public NewFormGridRowControl (NewFormGridRow row)
    {
      _row = row;
    }

    protected override void Render (HtmlTextWriter writer)
    {
      base.Render(writer);
    }
  }
}
