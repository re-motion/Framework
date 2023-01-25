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
