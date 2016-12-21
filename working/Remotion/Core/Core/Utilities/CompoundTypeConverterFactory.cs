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
using System.ComponentModel;
using System.Linq;
using Remotion.ServiceLocation;

namespace Remotion.Utilities
{
  /// <summary>
  /// Creates a <see cref="TypeConverter"/> from the list of <see cref="ITypeConverterFactory"/> implementations passed during initialization.
  /// </summary>
  [ImplementationFor (typeof (ITypeConverterFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public sealed class CompoundTypeConverterFactory : ITypeConverterFactory
  {
    private readonly IReadOnlyCollection<ITypeConverterFactory> _typeConverterFactories;

    public CompoundTypeConverterFactory (IEnumerable<ITypeConverterFactory> typeConverterFactories)
    {
      ArgumentUtility.CheckNotNull ("typeConverterFactories", typeConverterFactories);

      _typeConverterFactories = typeConverterFactories.ToList().AsReadOnly();
    }

    public IReadOnlyCollection<ITypeConverterFactory> TypeConverterFactories
    {
      get { return _typeConverterFactories; }
    }

    public TypeConverter CreateTypeConverterOrDefault (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _typeConverterFactories.Select (f => f.CreateTypeConverterOrDefault (type)).FirstOrDefault (c => c != null);
    }
  }
}