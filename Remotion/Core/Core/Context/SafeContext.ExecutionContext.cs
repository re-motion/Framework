// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
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
