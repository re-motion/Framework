using System;
using Remotion.ObjectBinding.Web.UI.Controls.BocTreeViewImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTreeViewImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocTreeViewImplementation.Rendering
{
  public class BocTreeViewQuirksModeRenderer : BocQuirksModeRendererBase<IBocTreeView>, IBocTreeViewRenderer
  {
    public BocTreeViewQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory)
        : base (resourceUrlFactory)
    {
    }

    public void AddDiagnosticMetadataAttributes (RenderingContext<IBocTreeView> renderingContext)
    {
    }

    public override string CssClassBase
    {
      get { return "bocTreeView"; }
    }
  }
}