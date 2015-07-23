using System;
using System.Web;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTreeViewImplementation.Rendering
{
  /// <summary>
  /// Groups all arguments required for rendering a <see cref="BocTreeView"/>.
  /// </summary>
  public class BocTreeViewRenderingContext : BocRenderingContext<IBocTreeView>
  {
    public BocTreeViewRenderingContext (HttpContextBase httpContext, HtmlTextWriter writer, IBocTreeView control)
        : base(httpContext, writer, control)
    {
    }
  }
}