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
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  ///   A hashtable that automatically creates new value objects when queried for a specific key.
  /// </summary>
  /// <remarks>
  ///		This collection cannot be modified using <see cref="Hashtable.Add"/> and setting values through the indexer. Getting values through the indexer
  ///		will assign a new object to the specified key if none exists.
  /// </remarks>
  [DebuggerDisplay ("Count={Count}")]
  public class AutoInitHashtable : Hashtable
  {
    public delegate object CreateMethod ();

    private readonly Type _valueType;
    private readonly CreateMethod _createMethod;

    public AutoInitHashtable (Type valueType)
    {
      ArgumentUtility.CheckNotNull ("valueType", valueType);
      _valueType = valueType;
      _createMethod = null;
    }

    public AutoInitHashtable (CreateMethod createMethod)
    {
      ArgumentUtility.CheckNotNull ("createMethod", createMethod);
      _createMethod = createMethod;
      _valueType = null;
    }

    private object CreateObject ()
    {
      if (_createMethod != null)
        return _createMethod();
      else
        return Activator.CreateInstance (_valueType);
    }

    public override object this [object key]
    {
      get
      {
        object obj = base[key];
        if (obj == null)
        {
          obj = CreateObject();
          base[key] = obj;
        }
        return obj;
      }

      set { throw new NotSupportedException(); }
    }

    #pragma warning disable 809 // C# 3.0: specifying obsolete for overridden methods causes a warning, but this is intended here.
    [Obsolete ("Explicitly adding or setting keys or values is not supported.")]
    public override void Add (object key, object value)
    {
      throw new NotSupportedException();
    }

    #pragma warning restore 809
  }
}
