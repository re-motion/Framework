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
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// The <see cref="InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator"/> can be used to guard against nested security checks on the same instnace.
  /// </summary>
  /// <remarks>
  /// This guard is intended to discover missing dependencies on <see cref="SecurityFreeSection"/>.<see cref="SecurityFreeSection.IsActive"/> when
  /// performing additional secured operations as part of a security check.
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  public class InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator : IObjectSecurityStrategy
  {
    private bool _isEvaluatingAccess;

    private readonly IObjectSecurityStrategy _objectSecurityStrategy;

    public InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator (IObjectSecurityStrategy objectSecurityStrategy)
    {
      ArgumentUtility.CheckNotNull("objectSecurityStrategy", objectSecurityStrategy);

      _objectSecurityStrategy = objectSecurityStrategy;
    }

    public bool HasAccess (
        ISecurityProvider securityProvider,
        ISecurityPrincipal principal,
        IReadOnlyList<AccessType> requiredAccessTypes)
    {
      ArgumentUtility.DebugCheckNotNull("securityProvider", securityProvider);
      ArgumentUtility.DebugCheckNotNull("principal", principal);
      ArgumentUtility.DebugCheckNotNullOrEmpty("requiredAccessTypes", requiredAccessTypes);

      if (_isEvaluatingAccess)
      {
        throw new InvalidOperationException(
            "Multiple reentrancies on InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator.HasAccess(...) are not allowed as they can indicate a possible infinite recursion. "
            + "Use SecurityFreeSection.IsActive to guard the computation of the SecurityContext returned by ISecurityContextFactory.CreateSecurityContext().");
      }

      try
      {
        _isEvaluatingAccess = true;
        return _objectSecurityStrategy.HasAccess(securityProvider, principal, requiredAccessTypes);
      }
      finally
      {
        _isEvaluatingAccess = false;
      }
    }
  }
}
