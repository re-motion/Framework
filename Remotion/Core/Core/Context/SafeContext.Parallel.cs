// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using JetBrains.Annotations;

namespace Remotion.Context
{
  public static partial class SafeContext
  {
    /// <summary>
    /// Provides methods for executing <see cref="System.Threading.Tasks.Parallel"/> actions which are <see cref="SafeContext"/> aware.
    /// <see cref="SafeContext"/> values do not flow into the executed Parallel action.
    /// </summary>
    public static class Parallel
    {
      /// <summary>
      /// Opens a <see cref="SafeContextBoundary"/> in which <see cref="System.Threading.Tasks.Parallel"/> actions can be taken without
      /// flowing <see cref="SafeContext"/> values into the callbacks.
      /// </summary>
      /// <example>
      /// <code>
      /// using (SafeContext.Parallel.OpenSafeContextBoundary())
      /// {
      ///   // SafeContext values no longer available
      ///   Parallel.For(0, 10, v => ...)
      /// }
      /// // SafeContext values available again
      /// </code>
      /// </example>
      [MustUseReturnValue]
      public static SafeContextBoundary OpenSafeContextBoundary () => Instance.OpenSafeContextBoundary();

      /// <summary>
      /// Opens a <see cref="SafeContextBoundary"/> in which <see cref="System.Threading.Tasks.Parallel"/> actions can be taken without
      /// flowing <see cref="SafeContext"/> values into the callbacks.
      /// </summary>
      /// <example>
      /// <code>
      /// using (SafeContext.Parallel.OpenSafeContextBoundary())
      /// {
      ///   // SafeContext values no longer available
      ///   Parallel.For(0, 10, v => ...)
      /// }
      /// // SafeContext values available again
      /// </code>
      /// </example>
      [MustUseReturnValue]
      public static SafeContextBoundary OpenSafeContextBoundary (ISafeContextStorageProvider provider) => provider.OpenSafeContextBoundary();
    }
  }
}
