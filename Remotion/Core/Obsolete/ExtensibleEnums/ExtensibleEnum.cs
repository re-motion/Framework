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
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Remotion.ExtensibleEnums
{
  [Obsolete("Dummy declaration for DependDB. Moved to Remotion.ExtensibleEnums.dll", true)]
  internal abstract class ExtensibleEnum<T> {

    public static readonly ExtensibleEnumDefinition<T> Values = (ExtensibleEnumDefinition<T>)ExtensibleEnumUtility.GetDefinition(typeof(T));

    public static bool operator == (ExtensibleEnum<T> value1, ExtensibleEnum<T> value2)
    {
      throw new NotImplementedException();
    }

    public static bool operator != (ExtensibleEnum<T> value1, ExtensibleEnum<T> value2)
    {
      throw new NotImplementedException();
    }

    protected ExtensibleEnum (string declarationSpace, string valueName)
    {
      throw new NotImplementedException();
    }

    protected ExtensibleEnum (string id)
        : this((string)null, id)
    {
      throw new NotImplementedException();
    }

    protected ExtensibleEnum (Type declaringType, string valueName)
        : this(declaringType.FullName, valueName)
    {
      throw new NotImplementedException();
    }

    protected ExtensibleEnum (MethodBase currentMethod)
        : this(currentMethod.DeclaringType, currentMethod.Name)
    {
      throw new NotImplementedException();
    }

    public abstract string ID { get; }

    public abstract string DeclarationSpace { get; }

    public abstract string ValueName { get;  }

    public abstract ExtensibleEnumInfo<T> GetValueInfo ();

    public abstract string GetLocalizedName ();

    public abstract Type GetEnumType ();

    public abstract bool Equals (T obj);

    public abstract override bool Equals (object obj);

    public override abstract int GetHashCode ();

  }
}
