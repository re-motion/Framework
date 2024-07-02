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
using System.Collections.Generic;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Security
{
  public sealed class DomainObjectSecurityStrategyDecorator : IObjectSecurityStrategy
  {
    private readonly IObjectSecurityStrategy _innerStrategy;
    private readonly IDomainObjectSecurityContextFactory _securityContextFactory;
    private readonly RequiredSecurityForStates _requiredSecurityForStates;

    public DomainObjectSecurityStrategyDecorator (
        IObjectSecurityStrategy innerStrategy,
        IDomainObjectSecurityContextFactory securityContextFactory,
        RequiredSecurityForStates requiredSecurityForStates)
    {
      ArgumentUtility.CheckNotNull("innerStrategy", innerStrategy);
      ArgumentUtility.CheckNotNull("securityContextFactory", securityContextFactory);

      _innerStrategy = innerStrategy;
      _securityContextFactory = securityContextFactory;
      _requiredSecurityForStates = requiredSecurityForStates;
    }

    public bool HasAccess (ISecurityProvider securityService, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      if (_securityContextFactory.IsInvalid)
        return true;

      bool isSecurityRequiredForNew = RequiredSecurityForStates.New == (RequiredSecurityForStates.New & _requiredSecurityForStates);
      if (!isSecurityRequiredForNew && _securityContextFactory.IsNew)
        return true;

      bool isSecurityRequiredForDeleted = RequiredSecurityForStates.Deleted == (RequiredSecurityForStates.Deleted & _requiredSecurityForStates);
      if (!isSecurityRequiredForDeleted && _securityContextFactory.IsDeleted)
        return true;

      return _innerStrategy.HasAccess(securityService, principal, requiredAccessTypes);
    }

    public IObjectSecurityStrategy InnerStrategy
    {
      get { return _innerStrategy; }
    }

    public IDomainObjectSecurityContextFactory SecurityContextFactory
    {
      get { return _securityContextFactory; }
    }

    public RequiredSecurityForStates RequiredSecurityForStates
    {
      get { return _requiredSecurityForStates; }
    }
  }
}
