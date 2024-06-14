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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.SecurableClassDefinitionTests
{
  [TestFixture]
  public class InsertAccessType : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    [Test]
    public void Insert_First ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var accessType2 = AccessTypeDefinition.NewObject();
      var accessType3 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      securableClassDefinition.AddAccessType(accessType0);
      securableClassDefinition.AddAccessType(accessType1);
      securableClassDefinition.AddAccessType(accessType2);

      securableClassDefinition.InsertAccessType(0, accessType3);

      Assert.That(securableClassDefinition.AccessTypes, Is.EqualTo(new[] { accessType3, accessType0, accessType1, accessType2 }));
      var references = new SecurableClassDefinitionWrapper(securableClassDefinition).AccessTypeReferences;
      for (int i = 0; i < references.Count; i++)
        Assert.That(((AccessTypeReference)references[i]).Index, Is.EqualTo(i));
    }

    [Test]
    public void Insert_Middle ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var accessType2 = AccessTypeDefinition.NewObject();
      var accessType3 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      securableClassDefinition.AddAccessType(accessType0);
      securableClassDefinition.AddAccessType(accessType1);
      securableClassDefinition.AddAccessType(accessType2);

      securableClassDefinition.InsertAccessType(1, accessType3);

      Assert.That(securableClassDefinition.AccessTypes, Is.EqualTo(new[] { accessType0, accessType3, accessType1, accessType2 }));
      var references = new SecurableClassDefinitionWrapper(securableClassDefinition).AccessTypeReferences;
      for (int i = 0; i < references.Count; i++)
        Assert.That(((AccessTypeReference)references[i]).Index, Is.EqualTo(i));
    }

    [Test]
    public void Insert_Last ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var accessType2 = AccessTypeDefinition.NewObject();
      var accessType3 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      securableClassDefinition.AddAccessType(accessType0);
      securableClassDefinition.AddAccessType(accessType1);
      securableClassDefinition.AddAccessType(accessType2);

      securableClassDefinition.InsertAccessType(3, accessType3);

      Assert.That(securableClassDefinition.AccessTypes, Is.EqualTo(new[] { accessType0, accessType1, accessType2, accessType3 }));
      var references = new SecurableClassDefinitionWrapper(securableClassDefinition).AccessTypeReferences;
      for (int i = 0; i < references.Count; i++)
        Assert.That(((AccessTypeReference)references[i]).Index, Is.EqualTo(i));
    }

    [Test]
    public void TouchesSecurableClassDefinition ()
    {
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        securableClassDefinition.EnsureDataAvailable();
        Assert.That(securableClassDefinition.State.IsUnchanged, Is.True);

        securableClassDefinition.InsertAccessType(0, AccessTypeDefinition.NewObject());

        Assert.That(securableClassDefinition.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void AddsPermissionsForNewAccessType ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.AddAccessType(accessType0);

      var testHelper = new AccessControlTestHelper(ClientTransaction.Current);
      var acls = new List<AccessControlList>();
      acls.Add(testHelper.CreateStatefulAcl(securableClassDefinition));
      acls.Add(testHelper.CreateStatelessAcl(securableClassDefinition));

      foreach (var acl in acls)
        acl.CreateAccessControlEntry();

      securableClassDefinition.InsertAccessType(1, accessType1);
      foreach (var acl in acls)
      {
        var permissions = acl.AccessControlEntries[0].GetPermissions();
        Assert.That(permissions.Count, Is.EqualTo(2));
        Assert.That(permissions[1].AccessType, Is.SameAs(accessType1));
        Assert.That(permissions[1].Allowed, Is.Null);
      }
    }

    [Test]
    public void FailsForExistingAccessType ()
    {
      var accessType = AccessTypeDefinition.NewObject(Guid.NewGuid(), "Test", 42);

      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddAccessType(accessType);
      Assert.That(
          () => securableClassDefinition.AddAccessType(accessType),
          Throws.ArgumentException
              .And.Message.StartsWith("The access type 'Test' has already been added to the securable class definition."));
    }

    [Test]
    public void FailsForIndexLessThanZero ()
    {
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddAccessType(AccessTypeDefinition.NewObject());
      Assert.That(
          () => securableClassDefinition.InsertAccessType(-1, AccessTypeDefinition.NewObject()),
          Throws.TypeOf<ArgumentOutOfRangeException>()
              .And.Message.StartsWith(
                  "The index must not be less than 0 or greater than the total number of access types for the securable class definition."));
    }

    [Test]
    public void FailsForIndexGreaterThanNumberOfItems ()
    {
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddAccessType(AccessTypeDefinition.NewObject());
      Assert.That(
          () => securableClassDefinition.InsertAccessType(2, AccessTypeDefinition.NewObject()),
          Throws.TypeOf<ArgumentOutOfRangeException>()
              .And.Message.StartsWith(
                  "The index must not be less than 0 or greater than the total number of access types for the securable class definition."));
    }
  }
}
