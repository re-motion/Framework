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
using System.Collections.ObjectModel;
using Remotion.ExtensibleEnums.Infrastructure;

// ReSharper disable once CheckNamespace

namespace Remotion.ExtensibleEnums
{
  [Obsolete ("Dummy declaration for DependDB. Moved to Remotion.ExtensibleEnums.dll", true)]
  internal abstract class ExtensibleEnumDefinition<T>
  {
    public ExtensibleEnumDefinition (IExtensibleEnumValueDiscoveryService valueDiscoveryService)
    {
      throw new NotImplementedException();
    }

    public abstract Type GetEnumType ();

    public abstract bool IsDefined (string id);

    public abstract bool IsDefined (IExtensibleEnum value);

    public abstract ReadOnlyCollection<ExtensibleEnumInfo<T>> GetValueInfos ();

    public abstract ExtensibleEnumInfo<T> GetValueInfoByID (string id);

    public abstract bool TryGetValueInfoByID (string id, out ExtensibleEnumInfo<T> value);

    public abstract object[] GetCustomAttributes (Type attributeType);

    public abstract TAttribute[] GetCustomAttributes<TAttribute> ();
  }
}