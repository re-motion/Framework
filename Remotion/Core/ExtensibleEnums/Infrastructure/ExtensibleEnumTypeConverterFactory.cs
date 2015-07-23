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
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums.Infrastructure
{
  /// <summary>
  /// Creates an <see cref="ExtensibleEnumConverter"/> if the requested type is an extensible enum.
  /// </summary>
  [ImplementationFor (typeof (ITypeConverterFactory), Position = Position, Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class ExtensibleEnumTypeConverterFactory : ITypeConverterFactory
  {
    public const int Position = EnumTypeConverterFactory.Position + 1;

    public ExtensibleEnumTypeConverterFactory ()
    {
    }

    public TypeConverter CreateTypeConverterOrDefault (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (ExtensibleEnumUtility.IsExtensibleEnumType (type))
        return new ExtensibleEnumConverter (type);
      return null;
    }
  }
}