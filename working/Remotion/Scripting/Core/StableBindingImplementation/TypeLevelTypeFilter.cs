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
using Remotion.Utilities;

namespace Remotion.Scripting.StableBindingImplementation
{
  /// <summary>
  /// Categorizes <see cref="Type"/>|s into "valid" and "invalid" types, 
  /// based on whether the type is a member of the class's type collection.
  /// </summary>
  public class TypeLevelTypeFilter : ITypeFilter
  {
    private readonly HashSet<Type> _validTypes;

    public TypeLevelTypeFilter (params Type[] validTypes)
      : this ((IEnumerable<Type>) validTypes)
    {
    }

    public TypeLevelTypeFilter (IEnumerable<Type> validTypes)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("validTypes", validTypes);
      _validTypes = new HashSet<Type> (validTypes);
    }

    public bool IsTypeValid (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _validTypes.Contains (type);
    }
  }
}
