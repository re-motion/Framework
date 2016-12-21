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
using Remotion.Security;

namespace Remotion.Data.DomainObjects.ObjectBinding.IntegrationTests.TestDomain.Security
{
  public class DerivedBindableSecurableObject : BindableSecurableObject
  {
    private string _accessibleProperty = string.Empty;

    public DerivedBindableSecurableObject (IObjectSecurityStrategy securityStrategy)
        : base (securityStrategy)
    {
    }

    public override string StringProperty
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public override string OtherStringProperty
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public override BindableSecurableObject Parent
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public override ObjectList<BindableSecurableObject> Children
    {
      get { throw new NotImplementedException(); }
    }

    public override BindableSecurableObject OtherParent
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public override ObjectList<BindableSecurableObject> OtherChildren
    {
      get { throw new NotImplementedException(); }
    }

    public override string PropertyWithDefaultPermission
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public override string PropertyWithCustomPermission
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public override string PropertyToOverride
    {
      get { return _accessibleProperty; }
      set { _accessibleProperty = value; }
    }
  }
}