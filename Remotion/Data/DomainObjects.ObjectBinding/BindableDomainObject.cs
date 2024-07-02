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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Provides a base class for bindable <see cref="DomainObject"/> classes.
  /// </summary>
  /// <remarks>
  /// Deriving from this base class is equivalent to deriving from the <see cref="DomainObject"/> class and applying the
  /// <see cref="BindableDomainObjectAttribute"/> to the derived class.
  /// </remarks>
  [BindableDomainObjectProvider]
  [BindableObjectBaseClass]
  [IgnoreForMappingConfiguration]
  public abstract class BindableDomainObject : DomainObject, IBusinessObjectWithIdentity
  {
    private IBindableDomainObjectImplementation? _implementation;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableDomainObject"/> class.
    /// </summary>
    protected BindableDomainObject ()
    {
    }

    protected override void OnReferenceInitializing ()
    {
      base.OnReferenceInitializing();

      if (_implementation == null) // may have been set by ctor
        _implementation = BindableDomainObjectImplementation.Create(this);
    }

    /// <summary>
    /// Provides a possibility to override the display name of the bindable domain object.
    /// </summary>
    /// <value>The display name.</value>
    /// <remarks>Override this property to replace the default display name provided by the <see cref="BindableObjectClass"/> with a custom one.
    /// </remarks>
    [StorageClassNone]
    public virtual string DisplayName
    {
      get { return Implementation.BaseDisplayName; }
    }

    string IBusinessObjectWithIdentity.UniqueIdentifier
    {
      get { return Implementation.BaseUniqueIdentifier; }
    }

    object? IBusinessObject.GetProperty (IBusinessObjectProperty property)
    {
      return Implementation.GetProperty(property);
    }

    void IBusinessObject.SetProperty (IBusinessObjectProperty property, object? value)
    {
      Implementation.SetProperty(property, value);
    }

    string IBusinessObject.GetPropertyString (IBusinessObjectProperty property, string? format)
    {
      return Implementation.GetPropertyString(property, format);
    }

    IBusinessObjectClass IBusinessObject.BusinessObjectClass
    {
      get { return Implementation.BusinessObjectClass; }
    }

    [StorageClassNone]
    private IBindableDomainObjectImplementation Implementation
    {
      get { return Assertion.IsNotNull(_implementation, "_implementation != null when object was initialized via infrastructure."); }
    }
  }
}
