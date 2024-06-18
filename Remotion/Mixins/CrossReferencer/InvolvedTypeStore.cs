// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class InvolvedTypeStore : IEnumerable<InvolvedType>
  {
    private readonly Dictionary<Type, InvolvedType> _involvedTypes = new Dictionary<Type, InvolvedType> ();

    public InvolvedType GetOrCreateValue (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (!_involvedTypes.ContainsKey (type))
        _involvedTypes.Add (type, new InvolvedType (type));

      return _involvedTypes[type];
    }

    public IEnumerator<InvolvedType> GetEnumerator ()
    {
      return _involvedTypes.Values.OrderBy (t => t.Type.FullName).GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }
  }
}