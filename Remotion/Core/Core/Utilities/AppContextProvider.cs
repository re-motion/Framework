using System;
using Remotion.ServiceLocation;

namespace Remotion.Utilities
{
  /// <summary>
  /// Represents the default implementation of the <see cref="IAppContextProvider"/> interface.
  /// </summary>
  [ImplementationFor(typeof(IAppContextProvider), Lifetime = LifetimeKind.Singleton)]
  public class AppContextProvider : IAppContextProvider
  {
    public string BaseDirectory => AppContext.BaseDirectory;

    public string? RelativeSearchPath
    {
      get
      {
        return null;
      }
    }
  }
}
