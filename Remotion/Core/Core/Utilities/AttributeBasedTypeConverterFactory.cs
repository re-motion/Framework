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
using System.ComponentModel;
using Remotion.ServiceLocation;

namespace Remotion.Utilities
{
  /// <summary>
  /// Creates the <see cref="TypeConverter"/> specified by the <see cref="TypeConverterAttribute"/> if the requested type has the attribute applied.
  /// </summary>
  [ImplementationFor(typeof(ITypeConverterFactory), Position = Position, Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class AttributeBasedTypeConverterFactory : ITypeConverterFactory
  {
    public const int Position = 0;

    public AttributeBasedTypeConverterFactory ()
    {
    }

    public TypeConverter? CreateTypeConverterOrDefault (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);
      TypeConverterAttribute? typeConverter = AttributeUtility.GetCustomAttribute<TypeConverterAttribute>(type, true);
      if (typeConverter == null)
        return null;

      var typeConverterType = TypeUtility.GetType(typeConverter.ConverterTypeName, true)!;
      return (TypeConverter)Activator.CreateInstance(typeConverterType)!;
    }
  }
}
