using System.Threading;
// We must use an alias as the class name is the same
using ExecutionContext_= System.Threading.ExecutionContext;

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
      public static void Run (ExecutionContext_ executionContext, ContextCallback callback, object state) =>
          Run(Instance, executionContext, callback, state);

      /// <inheritdoc cref="M:System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext,System.Threading.ContextCallback,System.Object)" />
      public static void Run (ISafeContextStorageProvider provider, ExecutionContext_ executionContext, ContextCallback callback, object state) =>
          ExecutionContext_.Run(executionContext, CreateWrapper(provider, callback), state);

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
