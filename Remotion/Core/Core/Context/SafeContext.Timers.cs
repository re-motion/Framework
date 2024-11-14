// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Timers;

namespace Remotion.Context
{
  public static partial class SafeContext
  {
    /// <summary>
    /// Provides methods for setting a callback on <see cref="System.Timers.Timer"/> which are <see cref="SafeContext"/> aware.
    /// </summary>
    public static class Timers
    {
      /// <summary>
      /// Adds a <paramref name="handler"/> to the <see cref="System.Timers.Timer.Elapsed"/> event on the specified <paramref name="timer"/>.
      /// The delegate is wrapped to open a <see cref="SafeContextBoundary"/> and the added delegate returned.
      /// </summary>
      /// <remarks>
      /// If you want to remove the delegate from the event at a later time use the return value of this method.
      /// </remarks>
      /// <param name="timer">The timer where to add the event.</param>
      /// <param name="handler">The delegate to be added to the event.</param>
      /// <returns>The wrapped delegate that was added to the event.</returns>
      public static ElapsedEventHandler AddElapsedEventHandler (System.Timers.Timer timer, ElapsedEventHandler handler)
      {
        var wrappedHandler = CreateWrapper(Instance, handler);
        timer.Elapsed += wrappedHandler;

        return wrappedHandler;
      }

      /// <summary>
      /// Adds a <paramref name="handler"/> to the <see cref="System.Timers.Timer.Elapsed"/> event on the specified <paramref name="timer"/>.
      /// The delegate is wrapped to open a <see cref="SafeContextBoundary"/> and the added delegate returned.
      /// </summary>
      /// <remarks>
      /// If you want to remove the delegate from the event at a later time use the return value of this method.
      /// </remarks>
      /// <param name="provider">The <see cref="ISafeContextStorageProvider"/> which is used to create the <see cref="SafeContextBoundary"/>.</param>
      /// <param name="timer">The timer where to add the event.</param>
      /// <param name="handler">The delegate to be added to the event.</param>
      /// <returns>The wrapped delegate that was added to the event.</returns>
      public static ElapsedEventHandler AddElapsedEventHandler (ISafeContextStorageProvider provider, System.Timers.Timer timer, ElapsedEventHandler handler)
      {
        var wrappedHandler = CreateWrapper(provider, handler);
        timer.Elapsed += wrappedHandler;

        return wrappedHandler;
      }

      private static ElapsedEventHandler CreateWrapper (ISafeContextStorageProvider provider, ElapsedEventHandler callback)
      {
        return (sender, args) =>
        {
          provider.OpenSafeContextBoundary();
          callback(sender, args);
        };
      }
    }
  }
}
