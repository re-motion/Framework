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
