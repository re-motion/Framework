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
using System.Collections;
using System.Threading;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Context
{
  [ImplementationFor (typeof (ISafeContextStorageProvider), Position = 1)]
  public class AsyncLocalStorageProvider : ISafeContextStorageProvider
  {
    private static readonly AsyncLocal<Hashtable> s_data = new (AsyncLocalValueChangedHandler);

    private static Hashtable Data => s_data.Value ??= new Hashtable();

    private static void AsyncLocalValueChangedHandler (AsyncLocalValueChangedArgs<Hashtable> args)
    {
      if (!args.ThreadContextChanged)
        return;

      if (args.CurrentValue == null)
        return;

      s_data.Value = new Hashtable();
    }

    /// <inheritdoc />
    public object? GetData (string key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return Data[key];
    }

    /// <inheritdoc />
    public void SetData (string key, object? value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

#if DEBUG && NETFRAMEWORK
      if (value is System.Runtime.Remoting.Messaging.ILogicalThreadAffinative)
        throw new NotSupportedException ("Remoting is not supported.");
#endif

      Data[key] = value;
    }

    /// <inheritdoc />
    public void FreeData (string key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      Data.Remove (key);
    }
  }
}
