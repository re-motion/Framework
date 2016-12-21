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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators
{
  /// <summary>
  /// Implements the <see cref="IBusinessObjectPropertyPathPropertyEnumerator"/> for a sequence of <see cref="IBusinessObjectProperty"/> objects.
  /// </summary>
  public sealed class ResolvedBusinessObjectPropertyPathPropertyEnumerator : IBusinessObjectPropertyPathPropertyEnumerator
  {
    private readonly IBusinessObjectProperty[] _properties;
    private int _index = -1;

    public ResolvedBusinessObjectPropertyPathPropertyEnumerator (IBusinessObjectProperty[] properties)
    {
      ArgumentUtility.CheckNotNull ("properties", properties);

      _properties = properties;
    }

    public IBusinessObjectProperty Current
    {
      get
      {
        if (_index < 0)
          throw new InvalidOperationException ("Enumeration has not started. Call MoveNext.");

        if (_index == _properties.Length)
          throw new InvalidOperationException ("Enumeration already finished.");

        return _properties[_index];
      }
    }

    public bool HasNext
    {
      get { return _index + 1 < _properties.Length; }
    }

    public bool MoveNext (IBusinessObjectClass currentClass)
    {
      ArgumentUtility.CheckNotNull ("currentClass", currentClass);

      if (_index == _properties.Length)
        return false;

      _index++;

      if (_index == _properties.Length)
        return false;

      return true;
    }
  }
}