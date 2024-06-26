// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Remotion.Context
{
  /// <summary>
  /// Represents a <see cref="SafeContext"/> boundary, across which values will not flow.
  /// The previous context is restored when disposed.
  /// </summary>
  /// <seealso cref="ISafeContextStorageProvider.OpenSafeContextBoundary"/>
  public readonly struct SafeContextBoundary : IDisposable
  {
    private readonly ISafeContextBoundaryStrategy? _boundaryStrategy;
    private readonly object? _state;

    public SafeContextBoundary (ISafeContextBoundaryStrategy boundaryStrategy, object? state)
    {
      _boundaryStrategy = boundaryStrategy;
      _state = state;
    }

    /// <inheritdoc />
    public void Dispose () => _boundaryStrategy?.RestorePreviousSafeContext(_state);
  }
}
