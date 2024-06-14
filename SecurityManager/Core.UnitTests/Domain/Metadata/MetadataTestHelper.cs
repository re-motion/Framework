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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  public class MetadataTestHelper
  {
    public const string Confidentiality_NormalName = "Normal";
    public const int Confidentiality_NormalValue = 0;
    public const string Confidentiality_ConfidentialName = "Confidential";
    public const int Confidentiality_ConfidentialValue = 1;
    public const string Confidentiality_PrivateName = "Private";
    public const int Confidentiality_PrivateValue = 2;

    public const string State_NewName = "New";
    public const int State_NewValue = 0;
    public const string State_NormalName = "Normal";
    public const int State_NormalValue = 1;
    public const string State_ArchivedName = "Archived";
    public const int State_ArchivedValue = 2;

    private readonly ClientTransaction _transaction;

    public MetadataTestHelper ()
    {
      _transaction = ClientTransaction.CreateRootTransaction();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public AbstractRoleDefinition CreateClerkAbstractRole (int index)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AbstractRoleDefinition role = AbstractRoleDefinition.NewObject(
            new Guid("00000003-0001-0000-0000-000000000000"),
            "Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain",
            0);
        role.Index = index;

        return role;
      }
    }

    public AbstractRoleDefinition CreateSecretaryAbstractRole (int index)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AbstractRoleDefinition role = AbstractRoleDefinition.NewObject(
            new Guid("00000003-0002-0000-0000-000000000000"),
            "Secretary|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain",
            1);
        role.Index = index;

        return role;
      }
    }

    public AbstractRoleDefinition CreateAdministratorAbstractRole (int index)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AbstractRoleDefinition role = AbstractRoleDefinition.NewObject(
            new Guid("00000004-0001-0000-0000-000000000000"),
            "Administrator|Remotion.Security.UnitTests.TestDomain.SpecialAbstractRoles, Remotion.Security.UnitTests.TestDomain",
            0);
        role.Index = index;

        return role;
      }
    }

    public AccessTypeDefinition CreateAccessTypeCreate (int index)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessTypeDefinition type = AccessTypeDefinition.NewObject(
            new Guid("1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"),
            "Create|Remotion.Security.GeneralAccessTypes, Remotion.Security",
            0);
        type.Index = index;

        return type;
      }
    }

    public AccessTypeDefinition CreateAccessTypeRead (int index)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessTypeDefinition type = AccessTypeDefinition.NewObject(
            new Guid("62dfcd92-a480-4d57-95f1-28c0f5996b3a"),
            "Read|Remotion.Security.GeneralAccessTypes, Remotion.Security",
            1);
        type.Index = index;

        return type;
      }
    }

    public AccessTypeDefinition CreateAccessTypeEdit (int index)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessTypeDefinition type = AccessTypeDefinition.NewObject(
            new Guid("11186122-6de0-4194-b434-9979230c41fd"),
            "Edit|Remotion.Security.GeneralAccessTypes, Remotion.Security",
            2);
        type.Index = index;

        return type;
      }
    }

    public StatePropertyDefinition CreateConfidentialityProperty (int index)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition property = StatePropertyDefinition.NewObject(new Guid("00000000-0000-0000-0001-000000000001"), "Confidentiality");
        property.Index = index;
        property.AddState(CreateState(Confidentiality_NormalName, Confidentiality_NormalValue));
        property.AddState(CreateState(Confidentiality_ConfidentialName, Confidentiality_ConfidentialValue));
        property.AddState(CreateState(Confidentiality_PrivateName, Confidentiality_PrivateValue));

        return property;
      }
    }

    public StatePropertyDefinition CreateFileStateProperty (int index)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition property = StatePropertyDefinition.NewObject(new Guid("00000000-0000-0000-0002-000000000001"), "State");
        property.Index = index;
        property.AddState(CreateState(State_NewName, State_NewValue));
        property.AddState(CreateState(State_NormalName, State_NormalValue));
        property.AddState(CreateState(State_ArchivedName, State_ArchivedValue));

        return property;
      }
    }

    public StateDefinition CreateConfidentialState ()
    {
      return CreateState(Confidentiality_ConfidentialName, Confidentiality_ConfidentialValue);
    }

    public StateDefinition CreatePrivateState ()
    {
      return CreateState(Confidentiality_PrivateName, Confidentiality_PrivateValue);
    }

    public StatePropertyDefinition CreateNewStateProperty (string name)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        return StatePropertyDefinition.NewObject(Guid.NewGuid(), name);
      }
    }

    public StateDefinition CreateState (string name, int value)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StateDefinition state = StateDefinition.NewObject(name, value);
        state.Index = value;

        return state;
      }
    }
  }
}
