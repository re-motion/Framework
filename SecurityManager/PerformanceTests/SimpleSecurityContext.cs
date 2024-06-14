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

namespace Remotion.SecurityManager.PerformanceTests
{
  public sealed class SimpleSecurityContext : ISecurityContext
  {
    private readonly string _class;
    private readonly string _owner;
    private readonly string _ownerGroup;
    private readonly string _ownerTenant;
    private readonly bool _isStateless;
    private readonly Dictionary<string, EnumWrapper> _states;
    private readonly EnumWrapper[] _abstractRoles;

    public SimpleSecurityContext (
        string @class,
        string owner,
        string ownerGroup,
        string ownerTenant,
        bool isStateless,
        Dictionary<string, EnumWrapper> states,
        EnumWrapper[] abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrEmpty("class", @class);
      ArgumentUtility.CheckNotNull("states", states);
      ArgumentUtility.CheckNotNull("abstractRoles", abstractRoles);

      _class = @class;
      _owner = StringUtility.EmptyToNull(owner);
      _ownerGroup = StringUtility.EmptyToNull(ownerGroup);
      _ownerTenant = StringUtility.EmptyToNull(ownerTenant);
      _isStateless = isStateless;
      _states = states;
      _abstractRoles = abstractRoles;
    }

    public string Class
    {
      get { return _class; }
    }

    public string Owner
    {
      get { return _owner; }
    }

    public string OwnerGroup
    {
      get { return _ownerGroup; }
    }

    public string OwnerTenant
    {
      get { return _ownerTenant; }
    }

    public IEnumerable<EnumWrapper> AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public EnumWrapper GetState (string propertyName)
    {
      return _states[propertyName];
    }

    public bool ContainsState (string propertyName)
    {
      return _states.ContainsKey(propertyName);
    }

    public bool IsStateless
    {
      get { return _isStateless; }
    }

    public int GetNumberOfStates ()
    {
      return _states.Count;
    }
  }
}
