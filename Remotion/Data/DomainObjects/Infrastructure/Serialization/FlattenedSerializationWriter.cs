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
using System.Collections.Generic;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.Infrastructure.Serialization
{
  public class FlattenedSerializationWriter<T>
  {
    private readonly List<T> _data = new List<T>();

    public T[] GetData ()
    {
      return _data.ToArray();
    }

    public void AddSimpleValue (T value)
    {
#if DEBUG
      if (value is Type)
          // ReSharper disable once PossibleMistakenCallToGetType.2
        throw new ArgumentException(string.Format("Cannot serialize values of type '{0}'.", typeof(Type).GetType().GetFullNameSafe()), "value");

      if (value is Delegate)
        throw new ArgumentException(string.Format("Cannot serialize values of type '{0}'.", typeof(Delegate).GetFullNameSafe()), "value");

#pragma warning disable SYSLIB0050
      if (value is not null && !value.GetType().IsSerializable)
        throw new ArgumentException(string.Format("Cannot serialize values of type '{0}'.", value.GetType().GetFullNameSafe()), "value");
#pragma warning restore SYSLIB0050
#endif

      _data.Add(value);
    }
  }
}
