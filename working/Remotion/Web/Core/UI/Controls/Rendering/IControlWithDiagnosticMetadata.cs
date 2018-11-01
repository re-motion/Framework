namespace Remotion.Web.UI.Controls.Rendering
{
  /// <summary>
  /// /// Common interface for controls which expose properties relevant to rendering.
  /// </summary>
  public interface IControlWithDiagnosticMetadata
  {
    string ControlType { get; }
  }
}