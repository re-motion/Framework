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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.AclTools
{
  public class AclToolsTestBase : DomainTest
  {
    public StatefulAccessControlList Acl { get; private set; }
    public StatefulAccessControlList Acl2 { get; private set; }

    public AccessTypeDefinition DeleteAccessType { get; private set; }
    public AccessTypeDefinition WriteAccessType { get; private set; }
    public AccessTypeDefinition ReadAccessType { get; private set; }

    public AccessTypeDefinition[] AccessTypeDefinitions { get; private set; }
    public AccessControlTestHelper TestHelper { get; private set; }
    public Tenant Tenant { get; private set; }
    public Group Group { get; private set; }
    public Position Position { get; private set; }
    public Role Role { get; private set; }
    public User User { get; private set; }
    public AccessControlEntry Ace { get; private set; }

    public AccessTypeDefinition[] AccessTypeDefinitions2 { get; private set; }
    public AccessControlEntry Ace2 { get; private set; }
    public Role Role2 { get; private set; }
    public User User2 { get; private set; }
    public Position Position2 { get; private set; }
    public Group Group2 { get; private set; }

    public AccessControlEntry Ace3 { get; private set; }
    public Role Role3 { get; private set; }
    public User User3 { get; private set; }
    public Position Position3 { get; private set; }
    public Group Group3 { get; private set; }

    // Called before each test gets executed.
    public override void SetUp ()
    {
      base.SetUp();
      TestHelper = new AccessControlTestHelper();

      // base class TearDown()-method (by MK) calls ClientTransactionScope.ResetActiveScope(),
      // discarding the transaction opened by EnterNonDiscardingScope below. 
      TestHelper.Transaction.EnterNonDiscardingScope();


      ReadAccessType = TestHelper.CreateReadAccessType();  // read access
      WriteAccessType = TestHelper.CreateWriteAccessType();  // write access
      DeleteAccessType = TestHelper.CreateDeleteAccessType();  // delete permission

      AccessTypeDefinitions = new[] { ReadAccessType, WriteAccessType, DeleteAccessType };
      AccessTypeDefinitions2 = new[] { ReadAccessType, DeleteAccessType };


      Tenant = TestHelper.CreateTenant("Da Tenant");
      Group = TestHelper.CreateGroup("Da Group", null, Tenant);
      Position = TestHelper.CreatePosition("Supreme Being");
      User = TestHelper.CreateUser("DaUs", "Da", "Usa", "Dr.", Group, Tenant);
      Role = TestHelper.CreateRole(User, Group, Position);
      Ace = TestHelper.CreateAceWithOwningTenant();

      TestHelper.AttachAccessType(Ace, ReadAccessType, null);
      TestHelper.AttachAccessType(Ace, WriteAccessType, true);
      TestHelper.AttachAccessType(Ace, DeleteAccessType, null);


      Group2 = TestHelper.CreateGroup("Anotha Group", null, Tenant);
      Position2 = TestHelper.CreatePosition("Working Drone");
      User2 = TestHelper.CreateUser("mr.smith", "", "Smith", "Mr.", Group2, Tenant);
      Role2 = TestHelper.CreateRole(User2, Group2, Position2);
      Ace2 = TestHelper.CreateAceWithSpecificTenant(Tenant);

      TestHelper.AttachAccessType(Ace2, ReadAccessType, true);
      TestHelper.AttachAccessType(Ace2, WriteAccessType, null);
      TestHelper.AttachAccessType(Ace2, DeleteAccessType, true);


      Group3 = TestHelper.CreateGroup("Da 3rd Group", null, Tenant);
      Position3 = TestHelper.CreatePosition("Combatant");
      User3 = TestHelper.CreateUser("ryan_james", "Ryan", "James", "", Group3, Tenant);
      Role3 = TestHelper.CreateRole(User3, Group3, Position3);
      Ace3 = TestHelper.CreateAceWithPositionAndGroupCondition(Position3, GroupCondition.None);

      TestHelper.AttachAccessType(Ace3, ReadAccessType, true);
      TestHelper.AttachAccessType(Ace3, WriteAccessType, true);
      TestHelper.AttachAccessType(Ace3, DeleteAccessType, null);


      //--------------------------------
      // Create ACLs
      //--------------------------------

      SecurableClassDefinition orderClass = SetUpFixture.OrderClassHandle.GetObject();
      var aclList = orderClass.StatefulAccessControlLists;
      Assert.That(aclList.Count, Is.GreaterThanOrEqualTo(2));

      Acl = aclList[0];
      TestHelper.AttachAces(Acl, Ace, Ace2, Ace3);

      var ace2_1 = TestHelper.CreateAceWithAbstractRole();
      var ace2_2 = TestHelper.CreateAceWithPositionAndGroupCondition(Position2, GroupCondition.OwningGroup);

      Acl2 = aclList[1];
      TestHelper.AttachAces(Acl2, ace2_1, ace2_2, Ace3);

      // Additional roles for users
      TestHelper.CreateRole(User2, Group, Position2);
      TestHelper.CreateRole(User2, Group2, Position);
      TestHelper.CreateRole(User2, Group3, Position2);

      TestHelper.CreateRole(User3, Group, Position);
      TestHelper.CreateRole(User3, Group2, Position2);
      TestHelper.CreateRole(User3, Group3, Position3);
      TestHelper.CreateRole(User3, Group, Position3);
      TestHelper.CreateRole(User3, Group2, Position);

    }

    public void AttachAccessTypeReadWriteDelete (AccessControlEntry ace, bool? allowRead, bool? allowWrite, bool? allowDelete)
    {
      TestHelper.AttachAccessType(ace, ReadAccessType, allowRead);
      TestHelper.AttachAccessType(ace, WriteAccessType, allowWrite);
      TestHelper.AttachAccessType(ace, DeleteAccessType, allowDelete);
    }


    protected List<AclExpansionEntry> GetAclExpansionEntryList (List<User> userList, List<AccessControlList> aclList,
      bool sortedAndDistinct)
    {
      var userFinderStub = new Mock<IAclExpanderUserFinder>();
      userFinderStub.Setup(mock => mock.FindUsers()).Returns(userList).Verifiable();

      var aclFinderStub = new Mock<IAclExpanderAclFinder>();
      aclFinderStub.Setup(mock => mock.FindAccessControlLists()).Returns(aclList).Verifiable();

      var aclExpander = new AclExpander(userFinderStub.Object, aclFinderStub.Object);

      List<AclExpansionEntry> aclExpansionEntryList;

      if (sortedAndDistinct)
      {
        aclExpansionEntryList = aclExpander.GetAclExpansionEntryListSortedAndDistinct();
      }
      else
      {
        aclExpansionEntryList = aclExpander.GetAclExpansionEntryList();
      }
      return aclExpansionEntryList;
    }

  }
}
