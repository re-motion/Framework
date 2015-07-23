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
using Remotion.ObjectBinding;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.ObjectBinding.IntegrationTests.TestDomain.Security
{
  [BindableObject]
  public class BindableSecurableObjectMixin : DomainObjectMixin<DomainObject>, IBindableSecurableObjectMixin
  {
    public string MixedPropertyWithDefaultPermission
    {
      get { return Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithDefaultPermission"].GetValue<string> (); }
      set { Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithDefaultPermission"].SetValue (value); }
    }

    public string MixedPropertyWithReadPermission
    {
      [DemandPermission(TestAccessTypes.First)]
      get { return Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithReadPermission"].GetValue<string> (); }
      set { Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithReadPermission"].SetValue (value); }
    }

    public string MixedPropertyWithWritePermission
    {
      get { return Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithWritePermission"].GetValue<string> (); }
      [DemandPermission (TestAccessTypes.First)]
      set { Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithWritePermission"].SetValue (value); }
    }

    public string DefaultPermissionMixedProperty
    {
      get { return Properties[typeof (BindableSecurableObjectMixin), "DefaultPermissionMixedProperty"].GetValue<string>(); }
      set { Properties[typeof (BindableSecurableObjectMixin), "DefaultPermissionMixedProperty"].SetValue (value); }
    }

    public string CustomPermissionMixedProperty
    {
      [DemandPermission (TestAccessTypes.First)]
      get { return Properties[typeof (BindableSecurableObjectMixin), "CustomPermissionMixedProperty"].GetValue<string>(); }
      [DemandPermission (TestAccessTypes.Second)]
      set { Properties[typeof (BindableSecurableObjectMixin), "CustomPermissionMixedProperty"].SetValue (value); }
    }
  }
}