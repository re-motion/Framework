using System;

namespace Remotion.Web.UI.Controls.Rendering
{
  /// <summary>
  /// Allows to enable or disable certain control rendering features.
  /// </summary>
  public interface IRenderingFeatures
  {
    /// <summary>
    /// Enables/disables diagnostic metadata rendering for controls.
    /// </summary>
    /// <remarks>
    /// This feature is required by the WebTesting framework.
    /// </remarks>
    bool EnableDiagnosticMetadata { get; }
  }
}