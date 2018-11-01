// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// Proxy for the <see cref="User"/> domain object class.
  /// </summary>
  /// <remarks>
  /// Used when a threadsafe representation of the domain object is required.
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  [Serializable]
  public sealed class UserProxy : OrganizationalStructureObjectProxy<User>
  {
    public static UserProxy Create (User user)
    {
      ArgumentUtility.CheckNotNull ("user", user);

      return new UserProxy (
          user.GetHandle(),
          ((IBusinessObjectWithIdentity) user).UniqueIdentifier,
          ((IBusinessObjectWithIdentity) user).DisplayName);
    }

    private UserProxy (IDomainObjectHandle<User> handle, string uniqueIdentifier, string displayName)
        : base (handle, uniqueIdentifier, displayName)
    {
    }
  }
}