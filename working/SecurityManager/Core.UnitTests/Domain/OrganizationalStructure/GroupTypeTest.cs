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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTypeTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());

      var groupTypes = GroupType.FindAll ();

      Assert.That (groupTypes.Count(), Is.EqualTo (2));
    }

    [Test]
    public void DeleteGroupType_WithAccessControlEntry ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        dbFixtures.CreateEmptyDomain();
        GroupType groupType = testHelper.CreateGroupType ("GroupType");
        AccessControlEntry ace = testHelper.CreateAceWithBranchOfOwningGroup (groupType);
        ClientTransaction.Current.Commit ();

        groupType.Delete ();

        ClientTransaction.Current.Commit ();

        Assert.That (ace.State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void DeleteGroupType_WithGroupTypePosition ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        GroupType groupType = testHelper.CreateGroupType ("GroupType");
        Position position = testHelper.CreatePosition ("Position");
        GroupTypePosition concretePosition = testHelper.CreateGroupTypePosition (groupType, position);

        groupType.Delete ();

        Assert.That (concretePosition.State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The GroupType 'groupType 1' is still assigned to at least one group. Please update or delete the dependent groups before proceeding.")]
    public void DeleteGroupType_WithGroup ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.Current);
        Group group = Group.FindByTenant (tenant.GetHandle()).Where (g => g.Name == "parentGroup0").Single ();
        GroupType groupType = group.GroupType;

        groupType.Delete ();
      }
    }

    [Test]
    public void GetDisplayName ()
    {
      GroupType groupType = GroupType.NewObject();
      groupType.Name = "GroupTypeName";

      Assert.That (groupType.DisplayName, Is.EqualTo ("GroupTypeName"));
    }
  }
}
