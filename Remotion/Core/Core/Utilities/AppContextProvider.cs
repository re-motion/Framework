using System;

namespace Remotion.Utilities
{
  /// <summary>
  /// Represents the default implementation of the <see cref="IAppContextProvider"/> interface.
  /// </summary>
  public class AppContextProvider : IAppContextProvider
  {
    public string BaseDirectory => AppContext.BaseDirectory;

    public string? RelativeSearchPath
    {
      get
      {
#if NETFRAMEWORK
        return AppDomain.CurrentDomain.RelativeSearchPath;
#else
        return null;
#endif
      }
    }
  }
}
