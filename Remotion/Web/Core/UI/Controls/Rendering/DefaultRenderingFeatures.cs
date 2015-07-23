using System;
using Remotion.ServiceLocation;

namespace Remotion.Web.UI.Controls.Rendering
{
  /// <summary>
  /// Default <see cref="IRenderingFeatures"/> implementation.
  /// </summary>
  [ImplementationFor (typeof (IRenderingFeatures), Lifetime = LifetimeKind.Singleton, Position = Position)]
  public class DefaultRenderingFeatures : IRenderingFeatures
  {
    public const int Position = 79;

    public bool EnableDiagnosticMetadata
    {
      get { return false; }
    }
  }
}