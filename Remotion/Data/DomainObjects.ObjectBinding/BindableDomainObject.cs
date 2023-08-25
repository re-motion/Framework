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
using System.Runtime.Serialization;
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
  [Serializable]
  public abstract class BindableDomainObject : DomainObject, IBusinessObjectWithIdentity
  {
    private IBindableDomainObjectImplementation? _implementation;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableDomainObject"/> class.
    /// </summary>
    protected BindableDomainObject ()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableDomainObject"/> class in the process of deserialization.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> coming from the .NET serialization infrastructure.</param>
    /// <param name="context">The <see cref="StreamingContext"/> coming from the .NET serialization infrastructure.</param>
    /// <remarks>Be sure to call this base constructor from the deserialization constructor of any concrete <see cref="BindableDomainObject"/> type
    /// implementing the <see cref="ISerializable"/> interface.</remarks>
    protected BindableDomainObject (SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      _implementation = (IBindableDomainObjectImplementation)info.GetValue("BDO._implementation", typeof(IBindableDomainObjectImplementation))!;
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected new void BaseGetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue("BDO._implementation", _implementation);

      base.BaseGetObjectData(info, context);
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
