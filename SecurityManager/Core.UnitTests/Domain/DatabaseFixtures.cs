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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  public class DatabaseFixtures
  {
    private readonly OrganizationalStructureFactory _factory;

    public DatabaseFixtures ()
    {
      _factory = new OrganizationalStructureFactory();
    }

    public void CreateEmptyDomain ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper();
      dbHelper.SetupDB();
    }

    public void CreateAndCommitSecurableClassDefinitionWithLocalizedNames (ClientTransaction transaction)
    {
      CreateEmptyDomain();

      using (transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition();

        Culture germanCulture = Culture.NewObject("de");
        Culture englishCulture = Culture.NewObject("en");
        Culture russianCulture = Culture.NewObject("ru");

        LocalizedName classInGerman = LocalizedName.NewObject("Klasse", germanCulture, classDefinition);
        LocalizedName classInEnglish = LocalizedName.NewObject("Class", englishCulture, classDefinition);

        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }

    /// <summary>
    ///  Inits security manager database with users, groups, roles, group types, position, etc for 2 tenants.
    ///  Use to catch sideffect due to incorrect tenant domain seperation.
    ///  Caution: DB setup is performance critical. Avoid if possible.
    /// </summary>
    /// <returns>The tenant to use in the test.</returns>
    public Tenant CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction transaction)
    {
      CreateEmptyDomain();

      using (transaction.EnterNonDiscardingScope())
      {
        AbstractRoleDefinition qualityManagerRole = AbstractRoleDefinition.NewObject(
            Guid.NewGuid(),
            "QualityManager|Remotion.SecurityManager.UnitTests.TestDomain.ProjectRoles, Remotion.SecurityManager.UnitTests",
            0);
        qualityManagerRole.Index = 1;
        AbstractRoleDefinition developerRole = AbstractRoleDefinition.NewObject(
            Guid.NewGuid(),
            "Developer|Remotion.SecurityManager.UnitTests.TestDomain.ProjectRoles, Remotion.SecurityManager.UnitTests",
            1);
        developerRole.Index = 0;

        Position globalPosition = CreatePosition("Global");
        globalPosition.Delegation = Delegation.Enabled;
        globalPosition.UniqueIdentifier = "UID: Global";

        Position officialPosition = CreatePosition("Official");
        officialPosition.UniqueIdentifier = "UID: Official";
        officialPosition.Delegation = Delegation.Enabled;

        Position managerPosition = CreatePosition("Manager");
        managerPosition.Delegation = Delegation.Disabled;
        managerPosition.UniqueIdentifier = "UID: Manager";

        GroupType groupType1 = CreateGroupType("groupType 1");
        GroupType groupType2 = CreateGroupType("groupType 2");

        GroupTypePosition groupType1_managerPosition = GroupTypePosition.NewObject();
        groupType1_managerPosition.GroupType = groupType1;
        groupType1_managerPosition.Position = managerPosition;
        GroupTypePosition groupType1_officialPosition = GroupTypePosition.NewObject();
        groupType1_officialPosition.GroupType = groupType1;
        groupType1_officialPosition.Position = officialPosition;
        GroupTypePosition groupType2_officialPosition = GroupTypePosition.NewObject();
        groupType2_officialPosition.GroupType = groupType2;
        groupType2_officialPosition.Position = officialPosition;

        Tenant tenant1 = CreateTenant("TestTenant");
        tenant1.UniqueIdentifier = "UID: testTenant";
        Group rootGroup = CreateGroup("rootGroup", "RG", "UID: rootGroup", null, tenant1);
        for (int i = 0; i < 2; i++)
        {
          Group parentGroup = CreateGroup(
              string.Format("parentGroup{0}", i),
              string.Format("PG{0}", i),
              string.Format("UID: parentGroup{0}", i),
              rootGroup,
              tenant1);
          parentGroup.GroupType = groupType1;

          Group group = CreateGroup(
              string.Format("group{0}", i),
              string.Format("G{0}", i),
              string.Format("UID: group{0}", i),
              parentGroup,
              tenant1);
          group.GroupType = groupType2;

          User user1 = CreateUser(string.Format("group{0}/user1", i), string.Empty, "user1", string.Empty, group, tenant1);
          User user2 = CreateUser(string.Format("group{0}/user2", i), string.Empty, "user2", string.Empty, group, tenant1);

          CreateRole(user1, parentGroup, managerPosition);
          CreateRole(user2, parentGroup, officialPosition);
        }

        Group testRootGroup = CreateGroup("testRootGroup", null, "UID: testRootGroup", null, tenant1);
        Group testParentOfOwningGroup = CreateGroup("testParentOfOwningGroup", null, "UID: testParentOfOwningGroup", testRootGroup, tenant1);
        Group testOwningGroup = CreateGroup("testOwningGroup", null, "UID: testOwningGroup", testParentOfOwningGroup, tenant1);
        Group testGroup = CreateGroup("testGroup", null, "UID: testGroup", null, tenant1);
        User testUser = CreateUser("test.user", "test", "user", "Dipl.Ing.(FH)", testOwningGroup, tenant1);

        Role officialRole = CreateRole(testUser, testGroup, officialPosition);
        Role managerRole = CreateRole(testUser, testGroup, managerPosition);
        CreateRole(testUser, testOwningGroup, managerPosition);
        CreateRole(testUser, testRootGroup, officialPosition);

        User substitutingUser = CreateUser("substituting.user", null, "substitute", null, testRootGroup, tenant1);
        CreateRole(substitutingUser, testOwningGroup, managerPosition);
        CreateRole(substitutingUser, testRootGroup, officialPosition);
        CreateRole(substitutingUser, testGroup, officialPosition);

        Substitution enabledUserSubstitution = Substitution.NewObject();
        enabledUserSubstitution.SubstitutingUser = substitutingUser;
        enabledUserSubstitution.SubstitutedUser = testUser;

        Substitution enabledRoleSubstitution = Substitution.NewObject();
        enabledRoleSubstitution.SubstitutingUser = substitutingUser;
        enabledRoleSubstitution.SubstitutedUser = testUser;
        enabledRoleSubstitution.SubstitutedRole = officialRole;

        Substitution disabledRoleSubstitution = Substitution.NewObject();
        disabledRoleSubstitution.SubstitutingUser = substitutingUser;
        disabledRoleSubstitution.SubstitutedUser = testUser;
        disabledRoleSubstitution.SubstitutedRole = managerRole;
        disabledRoleSubstitution.IsEnabled = false;

        Tenant tenant2 = CreateTenant("Tenant 2");
        Group groupTenant2 = CreateGroup("Group Tenant 2", "GT2", "UID: group Tenant 2", null, tenant2);
        User userTenant2 = CreateUser("User.Tenant2", "User", "Tenant 2", string.Empty, groupTenant2, tenant2);
        Substitution userTenant2Substitution = Substitution.NewObject();
        userTenant2Substitution.SubstitutingUser = userTenant2;
        userTenant2Substitution.SubstitutedUser = testUser;

        ClientTransactionScope.CurrentTransaction.Commit();
        return tenant1;
      }
    }

    public SecurableClassDefinition[] CreateAndCommitSecurableClassDefinitionsWithSubClassesEach (
        int classDefinitionCount,
        int derivedClassDefinitionCount,
        ClientTransaction transaction)
    {
      CreateEmptyDomain();
      using (transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition[] classDefinitions = CreateSecurableClassDefinitions(classDefinitionCount, derivedClassDefinitionCount);

        ClientTransactionScope.CurrentTransaction.Commit();

        return classDefinitions;
      }
    }

    public SecurableClassDefinition[] CreateAndCommitSecurableClassDefinitions (int classDefinitionCount, ClientTransaction transaction)
    {
      CreateEmptyDomain();
      using (transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition[] classDefinitions = CreateSecurableClassDefinitions(classDefinitionCount, 0);

        ClientTransactionScope.CurrentTransaction.Commit();

        return classDefinitions;
      }
    }

    public SecurableClassDefinition CreateAndCommitSecurableClassDefinitionWithStates (ClientTransaction transaction)
    {
      CreateEmptyDomain();
      using (transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition();

        classDefinition.AddStateProperty(CreateFileStateProperty(ClientTransactionScope.CurrentTransaction));
        classDefinition.AddStateProperty(CreateConfidentialityProperty());

        ClientTransactionScope.CurrentTransaction.Commit();

        return classDefinition;
      }
    }

    public SecurableClassDefinition CreateAndCommitSecurableClassDefinitionWithAccessTypes (int accessTypes, ClientTransaction transaction)
    {
      CreateEmptyDomain();
      using (transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = CreateSecurableClassDefinitionWithAccessTypes(accessTypes);

        ClientTransactionScope.CurrentTransaction.Commit();

        return classDefinition;
      }
    }

    public SecurableClassDefinition CreateAndCommitSecurableClassDefinitionWithAccessControlLists (int accessControlLists, ClientTransaction transaction)
    {
      CreateEmptyDomain();

      using (transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition();
        StatelessAccessControlList statelessAccessControlList = StatelessAccessControlList.NewObject();
        classDefinition.StatelessAccessControlList = statelessAccessControlList;

        var stateProperty = StatePropertyDefinition.NewObject(Guid.NewGuid(), "Property");
        classDefinition.AddStateProperty(stateProperty);

        for (int i = 1; i < accessControlLists; i++)
        {
          StatefulAccessControlList statefulAccessControlList = StatefulAccessControlList.NewObject();
          classDefinition.StatefulAccessControlLists.Add(statefulAccessControlList);
          statefulAccessControlList.CreateAccessControlEntry();
          CreateStateCombination(statefulAccessControlList, stateProperty, StateDefinition.NewObject(string.Format("Value {0}", i), i));
        }

        ClientTransactionScope.CurrentTransaction.Commit();

        return classDefinition;
      }
    }

    public AccessControlList CreateAndCommitAccessControlListWithAccessControlEntries (int accessControlEntries, ClientTransaction transaction)
    {
      CreateEmptyDomain();

      using (transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition();
        StatefulAccessControlList acl = StatefulAccessControlList.NewObject();
        classDefinition.StatefulAccessControlLists.Add(acl);
        acl.CreateStateCombination();

        for (int i = 0; i < accessControlEntries; i++)
          acl.CreateAccessControlEntry();

        ClientTransactionScope.CurrentTransaction.Commit();

        return acl;
      }
    }

    public StatefulAccessControlList CreateAndCommitAccessControlListWithStateCombinations (int stateCombinations, ClientTransaction transaction)
    {
      CreateEmptyDomain();
      using (transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition();
        StatefulAccessControlList statefulAccessControlList = StatefulAccessControlList.NewObject();
       classDefinition.StatefulAccessControlLists.Add(statefulAccessControlList);
        statefulAccessControlList.CreateAccessControlEntry();

        var stateProperty = StatePropertyDefinition.NewObject(Guid.NewGuid(), "Property");
        classDefinition.AddStateProperty(stateProperty);

        for (int i = 1; i < stateCombinations; i++)
          CreateStateCombination(statefulAccessControlList, stateProperty, StateDefinition.NewObject(string.Format("Value {0}", i), i));

        ClientTransactionScope.CurrentTransaction.Commit();

        return statefulAccessControlList;
      }
    }

    public void CreateAndCommitAdministratorAbstractRole (ClientTransaction transaction)
    {
      CreateEmptyDomain();

      using (transaction.EnterNonDiscardingScope())
      {
        Guid metadataItemID = new Guid("00000004-0001-0000-0000-000000000000");
        string abstractRoleName = "Administrator|Remotion.Security.UnitTests.TestDomain.SpecialAbstractRoles, Remotion.Security.UnitTests.TestDomain";
        AbstractRoleDefinition administratorAbstractRole = AbstractRoleDefinition.NewObject(metadataItemID, abstractRoleName, 0);

        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }

    public ObjectID CreateAndCommitAccessControlEntryWithPermissions (int permissions, ClientTransaction transaction)
    {
      CreateEmptyDomain();

      using (transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = CreateSecurableClassDefinitionWithAccessTypes(permissions);
        AccessControlList acl = classDefinition.CreateStatefulAccessControlList();
        AccessControlEntry ace = acl.CreateAccessControlEntry();

        ClientTransactionScope.CurrentTransaction.Commit();

        return ace.ID;
      }
    }

    private Group CreateGroup (string name, string shortName, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      Group group = _factory.CreateGroup();
      group.Name = name;
      group.ShortName = shortName;
      group.Parent = parent;
      group.Tenant = tenant;
      group.UniqueIdentifier = uniqueIdentifier;

      return group;
    }

    private Tenant CreateTenant (string name)
    {
      Tenant tenant = _factory.CreateTenant();
      tenant.Name = name;

      return tenant;
    }

    private User CreateUser (string userName, string firstName, string lastName, string title, Group group, Tenant tenant)
    {
      User user = _factory.CreateUser();
      user.UserName = userName;
      user.FirstName = firstName;
      user.LastName = lastName;
      user.Title = title;
      user.Tenant = tenant;
      user.OwningGroup = group;

      return user;
    }

    private SecurableClassDefinition CreateOrderSecurableClassDefinition ()
    {
      SecurableClassDefinition classDefinition = CreateSecurableClassDefinition(
          ClientTransactionScope.CurrentTransaction,
          new Guid("b8621bc9-9ab3-4524-b1e4-582657d6b420"),
          "Remotion.SecurityManager.UnitTests.TestDomain.Order, Remotion.SecurityManager.UnitTests");
      return classDefinition;
    }

    private SecurableClassDefinition[] CreateSecurableClassDefinitions (int classDefinitionCount, int derivedClassDefinitionCount)
    {
      SecurableClassDefinition[] classDefinitions = new SecurableClassDefinition[classDefinitionCount];
      for (int i = 0; i < classDefinitionCount; i++)
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();
        classDefinition.MetadataItemID = Guid.NewGuid();
        classDefinition.Name = string.Format("Class {0}", i);
        classDefinition.Index = i;
        classDefinitions[i] = classDefinition;
        CreateDerivedSecurableClassDefinitions(classDefinition, derivedClassDefinitionCount);
      }
      return classDefinitions;
    }

    private void CreateDerivedSecurableClassDefinitions (SecurableClassDefinition baseClassDefinition, int classDefinitionCount)
    {
      for (int i = 0; i < classDefinitionCount; i++)
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();
        classDefinition.MetadataItemID = Guid.NewGuid();
        classDefinition.Name = string.Format("{0} - Subsclass {0}", baseClassDefinition.Name, i);
        classDefinition.Index = i;
        classDefinition.BaseClass = baseClassDefinition;
      }
    }

    private SecurableClassDefinition CreateSecurableClassDefinitionWithAccessTypes (int accessTypes)
    {
      SecurableClassDefinition classDefinition = CreateOrderSecurableClassDefinition();

      for (int i = 0; i < accessTypes; i++)
      {
        AccessTypeDefinition accessType = CreateAccessType(Guid.NewGuid(), string.Format("Access Type {0}", i));
        accessType.Index = i;
        classDefinition.AddAccessType(accessType);
      }

      return classDefinition;
    }

    private GroupType CreateGroupType (string name)
    {
      GroupType groupType = GroupType.NewObject();
      groupType.Name = name;

      return groupType;
    }

    private Position CreatePosition (string name)
    {
      Position position = _factory.CreatePosition();
      position.Name = name;

      return position;
    }

    private Role CreateRole (User user, Group group, Position position)
    {
      Role role = Role.NewObject();
      role.User = user;
      role.Group = group;
      role.Position = position;

      return role;
    }

    private SecurableClassDefinition CreateSecurableClassDefinition (ClientTransaction transaction, Guid metadataItemID, string name)
    {
      using (transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();
        classDefinition.MetadataItemID = metadataItemID;
        classDefinition.Name = name;

        return classDefinition;
      }
    }

    private StatePropertyDefinition CreateFileStateProperty (ClientTransaction transaction)
    {
      using (transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition fileStateProperty = StatePropertyDefinition.NewObject(new Guid("9e689c4c-3758-436e-ac86-23171289fa5e"), "FileState");
        fileStateProperty.AddState(CreateState("Open", 0));
        fileStateProperty.AddState(CreateState("Cancelled", 1));
        fileStateProperty.AddState(CreateState("Reaccounted", 2));
        fileStateProperty.AddState(CreateState("HandledBy", 3));
        fileStateProperty.AddState(CreateState("Approved", 4));

        return fileStateProperty;
      }
    }

    private StatePropertyDefinition CreateConfidentialityProperty ()
    {
      StatePropertyDefinition confidentialityProperty =
          StatePropertyDefinition.NewObject(new Guid("93969f13-65d7-49f4-a456-a1686a4de3de"), "Confidentiality");
      confidentialityProperty.AddState(CreateState("Public", 0));
      confidentialityProperty.AddState(CreateState("Secret", 1));
      confidentialityProperty.AddState(CreateState("TopSecret", 2));

      return confidentialityProperty;
    }

    private AccessTypeDefinition CreateAccessType (Guid metadataItemID, string name)
    {
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject();
      accessType.MetadataItemID = metadataItemID;
      accessType.Name = name;

      return accessType;
    }

    private void CreateStateCombination (StatefulAccessControlList acl, StatePropertyDefinition stateProperty, StateDefinition stateDefinition)
    {
      StateCombination stateCombination = acl.CreateStateCombination();
      stateProperty.AddState(stateDefinition);
      stateCombination.AttachState(stateDefinition);
    }

    private StateDefinition CreateState (string name, int value)
    {
      StateDefinition state = StateDefinition.NewObject(name, value);
      state.Index = value;

      return state;
    }
  }
}
