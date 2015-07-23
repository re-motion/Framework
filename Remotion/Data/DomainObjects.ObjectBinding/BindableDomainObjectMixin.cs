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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The <see cref="BindableDomainObjectMixin"/> applies the <see cref="IBusinessObjectWithIdentity"/> implementation for bindable types 
  /// that implement the <see cref="DomainObjects.IDomainObject"/> interface.
  /// </summary>
  /// <remarks>
  /// If you do not wish to cast to <see cref="IBusinessObject"/> and <see cref="IBusinessObjectWithIdentity"/>, you can use the default 
  /// implementation provided by <see cref="BindableDomainObject"/> type. This type exposes the aforementioned interfaces and delegates their 
  /// implementation to the mixin.
  /// </remarks>
  [Serializable]
  [BindableDomainObjectProvider]
  public class BindableDomainObjectMixin : BindableObjectMixinBase<IDomainObject>, IBusinessObjectWithIdentity
  {
    [Obsolete ("Use IDomainObject instead. (Version 1.25.23.0)", true)]
    public interface IDomainObject : DomainObjects.IDomainObject
    {
      new Type GetPublicDomainObjectType ();
      new ObjectID ID { get; }
      [Obsolete ("Use new PropertyIndexer (domainObject) instead. (Version 1.25.23.0)", true)]
      PropertyIndexer Properties { get; }
      StateType State { get; }
    }

    public BindableDomainObjectMixin ()
    {
    }

    protected override Type GetTypeForBindableObjectClass ()
    {
      return Target.GetPublicDomainObjectType();
    }

    public string UniqueIdentifier
    {
      get { return Target.ID.ToString(); }
    }

    public virtual string DisplayName
    {
      get { return BusinessObjectClass.Identifier; }
    }
  }
}