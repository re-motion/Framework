// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Threading;
using SystemExecutionContext = System.Threading.ExecutionContext;

namespace Remotion.Context
{
  public static partial class SafeContext
  {
    /// <summary>
    /// Provides methods for running callbacks inside of a <see cref="System.Threading.ExecutionContext"/>s in a <see cref="SafeContext"/> aware manner.
    /// <see cref="SafeContext"/> values do not flow into callback.
    /// </summary>
    public static class ExecutionContext
    {
      /// <inheritdoc cref="M:System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext,System.Threading.ContextCallback,System.Object)" />
      public static void Run (SystemExecutionContext executionContext, ContextCallback callback, object state) =>
          Run(Instance, executionContext, callback, state);

      /// <inheritdoc cref="M:System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext,System.Threading.ContextCallback,System.Object)" />
      public static void Run (ISafeContextStorageProvider provider, SystemExecutionContext executionContext, ContextCallback callback, object state) =>
          SystemExecutionContext.Run(executionContext, CreateWrapper(provider, callback), state);

      private static ContextCallback CreateWrapper (ISafeContextStorageProvider provider, ContextCallback callback)
      {
        return obj =>
        {
          provider.OpenSafeContextBoundary();
          callback(obj);
        };
      }
    }
  }
}
