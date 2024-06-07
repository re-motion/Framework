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
using System.Linq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetAllowedAccessTypes_EmptyAce ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      AccessTypeDefinition[] accessTypes = ace.GetAllowedAccessTypes();

      Assert.That(accessTypes.Length, Is.EqualTo(0));
    }

    [Test]
    public void GetAllowedAccessTypes_ReadAllowed ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessTypeAndAttachToAce(ace, true);
      _testHelper.CreateWriteAccessTypeAndAttachToAce(ace, null);
      _testHelper.CreateDeleteAccessTypeAndAttachToAce(ace, false);

      Assert.That(ace.GetAllowedAccessTypes(), Is.EquivalentTo(new[] { readAccessType }));
    }

    [Test]
    public void GetDeniedAccessTypes_DeleteDenied ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      _testHelper.CreateReadAccessTypeAndAttachToAce(ace, true);
      _testHelper.CreateWriteAccessTypeAndAttachToAce(ace, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessTypeAndAttachToAce(ace, false);

      Assert.That(ace.GetDeniedAccessTypes(), Is.EquivalentTo(new[] { deleteAccessType }));
    }

    [Test]
    public void AllowAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce(ace, null);

      ace.AllowAccess(accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.That(allowedAccessTypes.Length, Is.EqualTo(1));
      Assert.That(allowedAccessTypes, Has.Member(accessType));
    }

    [Test]
    public void DenyAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce(ace, null);

      ace.DenyAccess(accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.That(allowedAccessTypes, Is.Empty);
    }

    [Test]
    public void AllowAccess_InvalidAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject(Guid.NewGuid(), "Test", 42);
      Assert.That(
          () => ace.AllowAccess(accessType),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The access type 'Test' is not assigned to this access control entry.", "accessType"));
    }

    [Test]
    public void RemoveAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce(ace, true);

      ace.RemoveAccess(accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.That(allowedAccessTypes.Length, Is.EqualTo(0));
    }

    [Test]
    public void AddAccessType ()
    {
      var accessType = AccessTypeDefinition.NewObject(Guid.NewGuid(), "Access Type 0", 0);
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.CreateStatelessAccessControlList();
      securableClassDefinition.AddAccessType(accessType);
      var ace = AccessControlEntry.NewObject();
      securableClassDefinition.StatelessAccessControlList.AccessControlEntries.Add(ace);

      ace.AddAccessType(accessType);

      Assert.That(ace.GetPermissions().Count, Is.EqualTo(1));
      Assert.That(ace.GetPermissions()[0].AccessType, Is.SameAs(accessType));
    }

    [Test]
    public void AddAccessType_ExistingAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject(Guid.NewGuid(), "Test", 42);

      ace.AddAccessType(accessType);
      Assert.That(
          () => ace.AddAccessType(accessType),
          Throws.ArgumentException.And.Message.StartsWith("The access type 'Test' has already been added to this access control entry."));
    }

    [Test]
    public void RemoveAccessType ()
    {
      var accessType0 = AccessTypeDefinition.NewObject(Guid.NewGuid(), "Access Type 0", 0);
      var accessType1 = AccessTypeDefinition.NewObject(Guid.NewGuid(), "Access Type 1", 1);
      var accessType2 = AccessTypeDefinition.NewObject(Guid.NewGuid(), "Access Type 2", 2);
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.CreateStatelessAccessControlList();
      securableClassDefinition.AddAccessType(accessType0);
      securableClassDefinition.AddAccessType(accessType2);
      var ace = AccessControlEntry.NewObject();
      securableClassDefinition.StatelessAccessControlList.AccessControlEntries.Add(ace);

      ace.AddAccessType(accessType0);
      ace.AddAccessType(accessType1);
      ace.AddAccessType(accessType2);

      ace.RemoveAccessType(accessType1);

      var permissions = ace.GetPermissions();
      Assert.That(permissions.Count, Is.EqualTo(2));
      Assert.That(permissions[0].AccessType, Is.SameAs(accessType0));
      Assert.That(permissions[1].AccessType, Is.SameAs(accessType2));
    }

    [Test]
    public void RemoveAccessType_AccessTypeDoesNotExist ()
    {
      var ace = AccessControlEntry.NewObject();

      ace.AddAccessType(AccessTypeDefinition.NewObject());
      Assert.That(
          () => ace.RemoveAccessType(AccessTypeDefinition.NewObject(Guid.NewGuid(), "Test", 42)),
          Throws.ArgumentException.And.Message.StartsWith("The access type 'Test' is not associated with the access control entry."));
    }

    [Test]
    public void GetPermissions_SortedByAccessTypeFromSecurableClassDefinition ()
    {
      var accessTypes = new List<AccessTypeDefinition>();
      for (int i = 0; i < 10; i++)
        accessTypes.Add(AccessTypeDefinition.NewObject(Guid.NewGuid(), string.Format("Access Type {0}", i), i));

      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.CreateStatelessAccessControlList();

      foreach (var accessType in accessTypes)
        securableClassDefinition.AddAccessType(accessType);

      var ace = AccessControlEntry.NewObject();
      securableClassDefinition.StatelessAccessControlList.AccessControlEntries.Add(ace);
      foreach (var accessType in accessTypes.AsEnumerable().Reverse())
        ace.AddAccessType(accessType);

      var permissions = ace.GetPermissions();
      for (int i = 0; i < accessTypes.Count; i++)
        Assert.That(permissions[i].AccessType, Is.SameAs(accessTypes[i]));
    }

    [Test]
    public void GetChangedAt_AfterCreation ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      Assert.That(ace.State.IsNew, Is.True);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      ace.Index = 1;
      Assert.That(ace.Index, Is.EqualTo(1));
    }
  }
}
