using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTreeViewImplementation.Rendering
{
  /// <summary>
  /// Defines the API for rendering a <see cref="IBocTreeView"/> control.
  /// </summary>
  public interface IBocTreeViewRenderer
  {
    /// <summary>
    /// Temporary (!) method until <see cref="IBocTreeView"/> renders the whole control using an <see cref="IBocTreeViewRenderer"/>.
    /// </summary>
    void AddDiagnosticMetadataAttributes (RenderingContext<IBocTreeView> renderingContext);
  }
}