﻿namespace Remotion.Context
{
  /// <summary>
  /// Implements a strategy for <see cref="SafeContextBoundary"/> to restore a previous <see cref="SafeContext"/>.
  /// </summary>
  /// <seealso cref="SafeContextBoundary"/>
  /// <seealso cref="ISafeContextStorageProvider.OpenSafeContextBoundary"/>
  public interface ISafeContextBoundaryStrategy
  {
    /// <summary>
    /// Restores a previous state of the context from <paramref name="state"/>.
    /// </summary>
    void RestorePreviousSafeContext (object? state);
  }
}
