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

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  /// <summary>
  /// Used to collect information about matching/contributing <see cref="AccessControlEntry"/>|s in calls
  /// to <see cref="AccessControlList.GetAccessTypes(SecurityToken, AccessTypeStatistics)"/>.
  /// </summary>
  public class AccessTypeStatistics
  {
    private readonly List<AccessControlEntry> _accessTypesSupplyingAces = new List<AccessControlEntry>();
    private readonly List<AccessControlEntry> _matchingAces = new List<AccessControlEntry>();

    public List<AccessControlEntry> AccessTypesSupplyingAces
    {
      get { return _accessTypesSupplyingAces; }
    }

    public List<AccessControlEntry> MatchingAces
    {
      get { return _matchingAces; }
    }

    public void AddAccessTypesContributingAce (AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull("ace", ace);
      if (!IsInAccessTypesContributingAces(ace))
      {
        AccessTypesSupplyingAces.Add(ace);
      }
    }

    /// <summary>
    /// Returns true if the passed <see cref="AccessControlEntry"/> has contributed either to the allowing or denying access types
    /// in the call to <see cref="AccessControlList.GetAccessTypes(SecurityToken,AccessTypeStatistics)"/>.
    /// </summary>
    public virtual bool IsInAccessTypesContributingAces (AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull("ace", ace);
      return AccessTypesSupplyingAces.Contains(ace);
    }


    public void AddMatchingAce (AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull("ace", ace);
      if (!IsInMatchingAces(ace))
      {
        _matchingAces.Add(ace);
      }
    }

    /// <summary>
    /// Returns true if the passed <see cref="AccessControlEntry"/> matched internally
    /// in the call to <see cref="AccessControlList.GetAccessTypes(SecurityToken,AccessTypeStatistics)"/>.
    /// </summary>
    /// <remarks>
    /// Note that an <see cref="AccessControlEntry"/> matching does not mean that it contributed to either 
    /// allowing or denying resulting access types (see <see cref="IsInAccessTypesContributingAces"/> to check for this).
    /// </remarks>
    public bool IsInMatchingAces (AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull("ace", ace);
      return _matchingAces.Contains(ace);
    }
  }
}
