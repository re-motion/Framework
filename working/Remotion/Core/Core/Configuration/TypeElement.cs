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
using System.Configuration;
using TypeNameConverter = Remotion.Utilities.TypeNameConverter;

namespace Remotion.Configuration
{
  public class TypeElement<TBase> : ConfigurationElement
      where TBase : class
  {
    //TODO: test
    public static ConfigurationProperty CreateTypeProperty (Type defaultValue)
    {
      return new ConfigurationProperty (
          "type",
          typeof (Type),
          defaultValue,
          new TypeNameConverter (),
          new SubclassTypeValidator (typeof (TBase)),
          ConfigurationPropertyOptions.IsRequired);
    }

    private readonly ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _typeProperty;

    public TypeElement ()
      : this (null)
    {
    }

    protected TypeElement (Type defaultValue)
    {
      _typeProperty = CreateTypeProperty (defaultValue);

      _properties = new ConfigurationPropertyCollection ();
      _properties.Add (_typeProperty);
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public Type Type
    {
      get { return (Type) base[_typeProperty]; }
      set { base[_typeProperty] = value; }
    }

    public TBase CreateInstance ()
    {
      if (Type == null)
        return null;
        
      return (TBase) Activator.CreateInstance (Type);
    }
  }

  public class TypeElement<TBase, TDefault> : TypeElement<TBase>
    where TBase : class
    where TDefault : TBase
  {
    public TypeElement ()
      : base (typeof (TDefault))
    {
    }
  }
}
