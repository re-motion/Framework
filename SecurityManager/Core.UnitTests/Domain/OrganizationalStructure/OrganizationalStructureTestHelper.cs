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
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  public class OrganizationalStructureTestHelper
  {
    private readonly ClientTransaction _transaction;
    private readonly OrganizationalStructureFactory _factory;

    public OrganizationalStructureTestHelper ()
      : this (ClientTransaction.CreateRootTransaction ())
    {
    }

    public OrganizationalStructureTestHelper (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      _transaction = transaction;
      _factory = new OrganizationalStructureFactory ();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public Tenant CreateTenant (string name, string uniqueIdentifier)
    {
      return CreateTenant (_transaction, name, uniqueIdentifier);
    }

    public Tenant CreateTenant (ClientTransaction transaction, string name, string uniqueIdentifier)
    {
      using (transaction.EnterNonDiscardingScope())
      {
        Tenant tenant = _factory.CreateTenant ();
        tenant.UniqueIdentifier = uniqueIdentifier;
        tenant.Name = name;

        return tenant;
      }
    }

    public Group CreateGroup (string name, Group parent, Tenant tenant)
    {
      return CreateGroup (_transaction, name, Guid.NewGuid().ToString(), parent, tenant);
    }

    public Group CreateGroup (string name, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      return CreateGroup (_transaction, name, uniqueIdentifier, parent, tenant);
    }

    public Group CreateGroup (ClientTransaction transaction, string name, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      using (transaction.EnterNonDiscardingScope())
      {
        Group group = _factory.CreateGroup ();
        group.Name = name;
        group.Parent = parent;
        group.Tenant = tenant;
        group.UniqueIdentifier = uniqueIdentifier;

        return group;
      }
    }

    public User CreateUser (string userName, string firstName, string lastName, string title, Group owningGroup, Tenant tenant)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        User user = _factory.CreateUser ();
        user.UserName = userName;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Title = title;
        user.Tenant = tenant;
        user.OwningGroup = owningGroup;

        return user;
      }
    }

    public Position CreatePosition (string name)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        Position position = _factory.CreatePosition ();
        position.Name = name;

        return position;
      }
    }

    public Role CreateRole (User user, Group group, Position position)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        Role role = Role.NewObject();
        role.User = user;
        role.Group = group;
        role.Position = position;

        return role;
      }
    }

    public GroupType CreateGroupType (string name)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        GroupType groupType = GroupType.NewObject();
        groupType.Name = name;

        return groupType;
      }
    }

    public GroupTypePosition CreateGroupTypePosition (GroupType groupType, Position position)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        GroupTypePosition concretePosition = GroupTypePosition.NewObject();
        concretePosition.GroupType = groupType;
        concretePosition.Position = position;

        return concretePosition;
      }
    }
  }
}
