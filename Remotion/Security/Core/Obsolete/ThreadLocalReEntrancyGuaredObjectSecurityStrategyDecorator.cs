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

// ReSharper disable once CheckNamespace
namespace Remotion.Security
{
  /// <summary>
  /// The <see cref="ThreadLocalReEntrancyGuaredObjectSecurityStrategyDecorator"/> can be used to guard against nested security checks on the same thread.
  /// </summary>
  /// <remarks>
  /// This guard is intended to discover missing dependencies on <see cref="SecurityFreeSection"/>.<see cref="SecurityFreeSection.IsActive"/> when
  /// performing additional secured operations as part of a security check.
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  [Obsolete ("Use InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator instead. (Version 1.15.26.0)", true)]
  public class ThreadLocalReEntrancyGuaredObjectSecurityStrategyDecorator : IObjectSecurityStrategy
  {
    public ThreadLocalReEntrancyGuaredObjectSecurityStrategyDecorator (IObjectSecurityStrategy objectSecurityStrategy)
    {
    }

    public bool HasAccess (
        ISecurityProvider securityProvider,
        ISecurityPrincipal principal,
        IReadOnlyList<AccessType> requiredAccessTypes)
    {
      throw new NotSupportedException ("Use InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator instead. (Version 1.15.26.0)");
    }
  }
}