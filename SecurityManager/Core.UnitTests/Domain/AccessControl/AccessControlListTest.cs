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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlListTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void FindMatchingEntries_WithMatchingAce ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject();
      AccessControlList acl = _testHelper.CreateStatefulAcl(entry);
      SecurityToken token = _testHelper.CreateTokenWithoutUser();

      AccessControlEntry[] foundEntries = acl.FindMatchingEntries(token);

      Assert.That(foundEntries.Length, Is.EqualTo(1));
      Assert.That(foundEntries, Has.Member(entry));
    }

    [Test]
    public void FindMatchingEntries_WithoutMatchingAce ()
    {
      AccessControlList acl = _testHelper.CreateStatefulAcl(_testHelper.CreateAceWithAbstractRole());
      SecurityToken token = _testHelper.CreateTokenWithoutUser();

      AccessControlEntry[] foundEntries = acl.FindMatchingEntries(token);

      Assert.That(foundEntries.Length, Is.EqualTo(0));
    }

    [Test]
    public void FindMatchingEntries_WithMultipleMatchingAces ()
    {
      AccessControlEntry ace1 = AccessControlEntry.NewObject();
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessTypeAndAttachToAce(ace1, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessTypeAndAttachToAce(ace1, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessTypeAndAttachToAce(ace1, null);

      AbstractRoleDefinition role2 = AbstractRoleDefinition.NewObject(Guid.NewGuid(), "SoftwareDeveloper", 1);
      AccessControlEntry ace2 = AccessControlEntry.NewObject();
      ace2.SpecificAbstractRole = role2;
      _testHelper.AttachAccessType(ace2, readAccessType, null);
      _testHelper.AttachAccessType(ace2, writeAccessType, true);
      _testHelper.AttachAccessType(ace2, deleteAccessType, null);

      AccessControlList acl = _testHelper.CreateStatefulAcl(ace1, ace2);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole(role2);

      AccessControlEntry[] entries = acl.FindMatchingEntries(token);

      Assert.That(entries.Length, Is.EqualTo(2));
      Assert.That(entries, Has.Member(ace2));
      Assert.That(entries, Has.Member(ace1));
    }

    [Test]
    public void GetAccessTypes_WithMatchingAce ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessTypeAndAttachToAce(ace, true);
      _testHelper.CreateWriteAccessTypeAndAttachToAce(ace, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessTypeAndAttachToAce(ace, false);
      AccessTypeDefinition copyAccessType = _testHelper.CreateAccessTypeForAce(ace, true, Guid.NewGuid(), "Copy", 3);
      AccessTypeDefinition moveAccessType = _testHelper.CreateAccessTypeForAce(ace, false, Guid.NewGuid(), "Move", 4);

      AccessControlList acl = _testHelper.CreateStatefulAcl(ace);
      SecurityToken token = _testHelper.CreateTokenWithoutUser();

      AccessInformation accessInformation = acl.GetAccessTypes(token);

      Assert.That(accessInformation.AllowedAccessTypes, Is.EquivalentTo(new[] { readAccessType, copyAccessType }));
      Assert.That(accessInformation.DeniedAccessTypes, Is.EquivalentTo(new[] { deleteAccessType, moveAccessType }));
    }

    [Test]
    public void GetAccessTypes_WithoutMatchingAce ()
    {
      AccessControlEntry ace = _testHelper.CreateAceWithAbstractRole();
      _testHelper.CreateReadAccessTypeAndAttachToAce(ace, true);
      _testHelper.CreateWriteAccessTypeAndAttachToAce(ace, null);
      _testHelper.CreateDeleteAccessTypeAndAttachToAce(ace, false);
      AccessControlList acl = _testHelper.CreateStatefulAcl(ace);
      SecurityToken token = _testHelper.CreateTokenWithoutUser();

      AccessInformation accessInformation = acl.GetAccessTypes(token);

      Assert.That(accessInformation.AllowedAccessTypes, Is.Empty);
      Assert.That(accessInformation.DeniedAccessTypes, Is.Empty);
    }

    [Test]
    public void GetAccessTypes_WithMultipleMatchingAces ()
    {
      AbstractRoleDefinition role1 = AbstractRoleDefinition.NewObject(Guid.NewGuid(), "QualityManager", 0);
      AccessControlEntry ace1 = AccessControlEntry.NewObject();
      ace1.SpecificAbstractRole = role1;
      AccessTypeDefinition readAccessType = _testHelper.CreateAccessTypeForAce(ace1, true, Guid.NewGuid(), "Read", 0);
      AccessTypeDefinition copyAccessType = _testHelper.CreateAccessTypeForAce(ace1, true, Guid.NewGuid(), "Copy", 1);
      AccessTypeDefinition indexAccessType = _testHelper.CreateAccessTypeForAce(ace1, true, Guid.NewGuid(), "Index", 2);

      AccessTypeDefinition moveAccessType = _testHelper.CreateAccessTypeForAce(ace1, false, Guid.NewGuid(), "Move", 3);
      AccessTypeDefinition appendAccessType = _testHelper.CreateAccessTypeForAce(ace1, false, Guid.NewGuid(), "Append", 4);
      AccessTypeDefinition renameAccessType = _testHelper.CreateAccessTypeForAce(ace1, false, Guid.NewGuid(), "Rename", 5);

      AccessTypeDefinition writeAccessType = _testHelper.CreateAccessTypeForAce(ace1, true, Guid.NewGuid(), "Write", 6);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateAccessTypeForAce(ace1, true, Guid.NewGuid(), "Delete", 7);
      AccessTypeDefinition findAccessType = _testHelper.CreateAccessTypeForAce(ace1, null, Guid.NewGuid(), "Find", 8);

      AbstractRoleDefinition role2 = AbstractRoleDefinition.NewObject(Guid.NewGuid(), "SoftwareDeveloper", 1);
      AccessControlEntry ace2 = AccessControlEntry.NewObject();
      ace2.SpecificAbstractRole = role2;
      _testHelper.AttachAccessType(ace2, readAccessType, true);
      _testHelper.AttachAccessType(ace2, copyAccessType, false);
      _testHelper.AttachAccessType(ace2, indexAccessType, null);

      _testHelper.AttachAccessType(ace2, moveAccessType, true);
      _testHelper.AttachAccessType(ace2, appendAccessType, false);
      _testHelper.AttachAccessType(ace2, renameAccessType, null);

      _testHelper.AttachAccessType(ace2, writeAccessType, true);
      _testHelper.AttachAccessType(ace2, deleteAccessType, false);
      _testHelper.AttachAccessType(ace2, findAccessType, null);

      AccessControlList acl = _testHelper.CreateStatefulAcl(ace1, ace2);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole(role1, role2);

      AccessInformation accessInformation = acl.GetAccessTypes(token);

      //read    y y   y
      //copy    y n   n
      //index   y -   y
      //move    n y   n
      //append  n n   n
      //rename  n -   n
      //write   - y   y
      //delete  - n   n
      //find    - -   -

      Assert.That(
        accessInformation.AllowedAccessTypes,
        Is.EquivalentTo(new[] { readAccessType, indexAccessType, writeAccessType }));
      Assert.That(
        accessInformation.DeniedAccessTypes,
        Is.EquivalentTo(new[] { copyAccessType, moveAccessType, appendAccessType, renameAccessType, deleteAccessType }));
    }

    [Test]
    public void CreateAccessControlEntry ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      AccessTypeDefinition readAccessType = _testHelper.AttachAccessType(classDefinition, Guid.NewGuid(), "Read", 0);
      AccessTypeDefinition deleteAccessType = _testHelper.AttachAccessType(classDefinition, Guid.NewGuid(), "Delete", 1);
      AccessControlList acl = _testHelper.CreateStatefulAcl(classDefinition);
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        acl.EnsureDataAvailable();
        Assert.That(acl.State.IsUnchanged, Is.True);

        AccessControlEntry entry = acl.CreateAccessControlEntry();

        Assert.That(entry.AccessControlList, Is.SameAs(acl));
        Assert.That(entry.GetPermissions().Count, Is.EqualTo(2));
        Assert.That((entry.GetPermissions()[0]).AccessType, Is.SameAs(readAccessType));
        Assert.That((entry.GetPermissions()[0]).AccessControlEntry, Is.SameAs(entry));
        Assert.That((entry.GetPermissions()[1]).AccessType, Is.SameAs(deleteAccessType));
        Assert.That((entry.GetPermissions()[1]).AccessControlEntry, Is.SameAs(entry));
        Assert.That(acl.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void CreateAccessControlEntry_TwoNewEntries ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      AccessControlList acl = _testHelper.CreateStatefulAcl(classDefinition);
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        acl.EnsureDataAvailable();
        Assert.That(acl.State.IsUnchanged, Is.True);

        AccessControlEntry ace0 = acl.CreateAccessControlEntry();
        AccessControlEntry acel = acl.CreateAccessControlEntry();

        Assert.That(acl.AccessControlEntries.Count, Is.EqualTo(2));
        Assert.That(acl.AccessControlEntries[0], Is.SameAs(ace0));
        Assert.That(ace0.Index, Is.EqualTo(0));
        Assert.That(acl.AccessControlEntries[1], Is.SameAs(acel));
        Assert.That(acel.Index, Is.EqualTo(1));
        Assert.That(acl.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void CreateAccessControlEntry_DeleteEntries ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      AccessControlList acl = _testHelper.CreateStatefulAcl(classDefinition);
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        acl.EnsureDataAvailable();
        Assert.That(acl.State.IsUnchanged, Is.True);

        AccessControlEntry ace0 = acl.CreateAccessControlEntry();
        AccessControlEntry acel = acl.CreateAccessControlEntry();
        AccessControlEntry ace2 = acl.CreateAccessControlEntry();
        AccessControlEntry ace3 = acl.CreateAccessControlEntry();

        Assert.That(acl.AccessControlEntries, Is.EqualTo(new[] { ace0, acel, ace2, ace3 }));
        Assert.That(ace0.Index, Is.EqualTo(0));
        Assert.That(acel.Index, Is.EqualTo(1));
        Assert.That(ace2.Index, Is.EqualTo(2));
        Assert.That(ace3.Index, Is.EqualTo(3));

        acel.Delete();

        Assert.That(acl.AccessControlEntries, Is.EqualTo(new[] { ace0, ace2, ace3 }));
        Assert.That(ace0.Index, Is.EqualTo(0));
        Assert.That(ace2.Index, Is.EqualTo(1));
        Assert.That(ace3.Index, Is.EqualTo(2));
      }
    }

    [Test]
    public void GetChangedAt_AfterCreation ()
    {
      AccessControlList acl = _testHelper.CreateStatefulAcl(_testHelper.CreateOrderClassDefinitionWithProperties());

      Assert.That(acl.State.IsNew, Is.True);
    }

    [Test]
    public void Get_AccessControlEntriesFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      AccessControlList expectedAcl =
          dbFixtures.CreateAndCommitAccessControlListWithAccessControlEntries(10, ClientTransactionScope.CurrentTransaction);
      ObjectList<AccessControlEntry> expectedAces = expectedAcl.AccessControlEntries;

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        var actualAcl = (AccessControlList)LifetimeService.GetObject(ClientTransaction.Current, expectedAcl.ID, false);

        Assert.That(actualAcl.AccessControlEntries.Count, Is.EqualTo(10));
        for (int i = 0; i < 10; i++)
          Assert.That(actualAcl.AccessControlEntries[i].ID, Is.EqualTo(expectedAces[i].ID));
      }
    }

    [Test]
    public void CreateAccessControlEntry_BeforeClassIsSet_WithStatefulAccessControlList ()
    {
      AccessControlList acl = StatefulAccessControlList.NewObject();
      Assert.That(
          () => acl.CreateAccessControlEntry(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot create AccessControlEntry if no SecurableClassDefinition is assigned to this AccessControlList."));
    }

    [Test]
    public void CreateAccessControlEntry_BeforeClassIsSet_WithStatelessAccessControlList ()
    {
      AccessControlList acl = StatelessAccessControlList.NewObject();
      Assert.That(
          () => acl.CreateAccessControlEntry(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot create AccessControlEntry if no SecurableClassDefinition is assigned to this AccessControlList."));
    }
  }
}
