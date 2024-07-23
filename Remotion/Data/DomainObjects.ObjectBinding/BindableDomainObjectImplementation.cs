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
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectImplementation : BindableDomainObjectMixin, IBindableDomainObjectImplementation
  {
    public static BindableDomainObjectImplementation Create (BindableDomainObject wrapper)
    {
      ArgumentUtility.CheckNotNull("wrapper", wrapper);
      var impl = new BindableDomainObjectImplementation(wrapper);
      ((IInitializableMixin)impl).Initialize(wrapper, null);
      return impl;
    }

    private readonly BindableDomainObject _wrapper;

    protected BindableDomainObjectImplementation (BindableDomainObject wrapper)
    {
      ArgumentUtility.CheckNotNull("wrapper", wrapper);
      _wrapper = wrapper;
    }

    public string BaseDisplayName
    {
      get { return base.DisplayName; }
    }

    public string BaseUniqueIdentifier
    {
      get { return base.UniqueIdentifier; }
    }

    public override string DisplayName
    {
      get { return ((IBusinessObjectWithIdentity)Target).DisplayName; }
    }
  }
}
