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
using Remotion.Collections.Caching;
using Remotion.Mixins;
using Remotion.Security;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.Security.UnitTests.TestDomain
{
  [Instantiable]
  [DBTable]
  [Uses(typeof(SecurableObjectMixin))]
  public abstract class OtherSecurableObject : DomainObject, ISecurableObject, ISecurityContextFactory
  {
    public static OtherSecurableObject NewObject (ClientTransaction clientTransaction, IObjectSecurityStrategy securityStrategy)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return NewObject<OtherSecurableObject>(ParamList.Create(securityStrategy));
      }
    }

    private IObjectSecurityStrategy _securityStrategy;

    protected OtherSecurableObject (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded(loadMode);
      _securityStrategy = ObjectSecurityStrategy.Create(this, InvalidationToken.Create());
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    Type ISecurableObject.GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      return SecurityContext.CreateStateless(GetPublicDomainObjectType());
    }

    public new void Delete ()
    {
      base.Delete();
    }
  }
}
