using System;

namespace Remotion.Utilities
{
  /// <summary>
  /// Provides members holding information about the context an application is being executed under.
  /// </summary>
  public interface IAppContextProvider
  {
    /// <summary>
    /// Gets the pathname of the base directory that the assembly resolver uses to probe for assemblies.
    /// </summary>
    string BaseDirectory { get; }

    /// <summary>
    /// Gets the path under the <see cref="BaseDirectory"/> where the assembly resolver should probe for private assemblies.
    /// </summary>
    string? RelativeSearchPath { get; }
  }
}
