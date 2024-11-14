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
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Provides a base implementation of <see cref="IBusinessObject"/> associated with the reflection-based <see cref="BindableObjectProvider"/>.
  /// </summary>
  /// <remarks>
  /// If derivation from this class is not possible, the reflection-based implementation can also be added via the 
  /// <see cref="BindableObjectAttribute"/>, which adds the <see cref="BindableObjectMixin"/> to the class.
  /// </remarks>
  [BindableObjectProvider]
  [BindableObjectBaseClass]
  public abstract class BindableObjectBase : IBusinessObject
  {
    private readonly IBindableObjectBaseImplementation _implementation;

    protected BindableObjectBase ()
    {
      _implementation = BindableObjectBaseImplementation.Create(this);
    }

    public object? GetProperty (IBusinessObjectProperty property)
    {
      return _implementation.GetProperty(property);
    }

    public void SetProperty (IBusinessObjectProperty property, object? value)
    {
      _implementation.SetProperty(property, value);
    }

    public string GetPropertyString (IBusinessObjectProperty property, string? format)
    {
      return _implementation.GetPropertyString(property, format);
    }

    public IBusinessObjectClass BusinessObjectClass
    {
      get { return _implementation.BusinessObjectClass; }
    }
  }
}
