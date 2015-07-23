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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  public class PrincipalRole
  {
    private readonly IDomainObjectHandle<Position> _position;
    private readonly IDomainObjectHandle<Group> _group;

    public PrincipalRole ([NotNull] IDomainObjectHandle<Position> position, [NotNull] IDomainObjectHandle<Group> group)
    {
      ArgumentUtility.CheckNotNull ("position", position);
      ArgumentUtility.CheckNotNull ("group", group);

      _position = position;
      _group = @group;
    }

    [NotNull]
    public IDomainObjectHandle<Position> Position
    {
      get { return _position; }
    }

    [NotNull]
    public IDomainObjectHandle<Group> Group
    {
      get { return _group; }
    }
  }
}