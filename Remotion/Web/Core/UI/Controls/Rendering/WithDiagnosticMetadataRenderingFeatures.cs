using System;

namespace Remotion.Web.UI.Controls.Rendering
{
  /// <summary>
  /// <see cref="IRenderingFeatures"/> implementation with enabled diagnostic metadata feature.
  /// </summary>
  public class WithDiagnosticMetadataRenderingFeatures : IRenderingFeatures
  {
    public bool EnableDiagnosticMetadata
    {
      get { return true; }
    }
  }
}