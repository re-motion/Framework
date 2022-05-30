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
using System.Threading;
using Remotion.ServiceLocation;

namespace Remotion.Context
{
  /// <summary>
  /// Superior alternative to the <see cref="ThreadStaticAttribute"/> and <see cref="T:System.Runtime.Remoting.Messaging.CallContext"/>
  /// for making member variables thread safe that also works with ASP.NET threads.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The data managed by this class is by default stored in the <see cref="T:System.Runtime.Remoting.Messaging.CallContext"/>,
  /// but the storage provider can be replaced by application code if needed. Replacements for the storage provider must guarantee
  /// that all data stored by the <see cref="SafeContext"/> is thread-local.
  /// </para>
  /// <para>
  /// The Remotion.Web assembly by default replaces the storage provider with one that stores all data in the <see cref="T:System.Web.HttpContext"/>. 
  /// This ensures that <see cref="SafeContext"/> works as expected in ASP.NET environments when a session migrates between threads.
  /// </para>
  /// </remarks>
  /// <threadsafety>
  /// The data managed by this class is thread-local. The class is safe to be used from multiple threads at the same time, but each thread will have 
  /// its own copy of the data.
  /// </threadsafety>
  public static partial class SafeContext
  {
    /// <summary>Workaround to allow reflection to reset the fields since setting a static readonly field is not supported in .NET 3.0 and later.</summary>
    private class Fields
    {
      public readonly Lazy<ISafeContextStorageProvider> SafeContextStorageProvider =
          new Lazy<ISafeContextStorageProvider>(
              () => SafeServiceLocator.Current.GetInstance<ISafeContextStorageProvider>(),
              LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private static readonly Fields s_fields = new Fields();

    public static ISafeContextStorageProvider Instance
    {
      get { return s_fields.SafeContextStorageProvider.Value; }
    }
  }
}
