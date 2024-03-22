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
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  [Serializable]
  public class BindableObjectWithIdentityBaseImplementation : BindableObjectWithIdentityMixin, IDeserializationCallback, IBindableObjectWithIdentityBaseImplementation
  {
    public static BindableObjectWithIdentityBaseImplementation Create (BindableObjectWithIdentityBase wrapper)
    {
      ArgumentUtility.CheckNotNull("wrapper", wrapper);
      var impl = new BindableObjectWithIdentityBaseImplementation(wrapper);
      ((IInitializableMixin)impl).Initialize(wrapper, null, false);
      return impl;
    }

    private readonly BindableObjectWithIdentityBase _wrapper;

    protected BindableObjectWithIdentityBaseImplementation (BindableObjectWithIdentityBase wrapper)
    {
      ArgumentUtility.CheckNotNull("wrapper", wrapper);
      _wrapper = wrapper;
    }

    public override string UniqueIdentifier
    {
      get { return ((BindableObjectWithIdentityBase)Target).UniqueIdentifier; }
    }

    void IDeserializationCallback.OnDeserialization (object? sender)
    {
      MixinTargetMockUtility.MockMixinTargetAfterDeserialization(this, _wrapper);
    }

    public string BaseDisplayName
    {
      get { return base.DisplayName; }
    }

    public override string DisplayName
    {
      get { return ((IBusinessObjectWithIdentity)Target).DisplayName; }
    }
  }
}
