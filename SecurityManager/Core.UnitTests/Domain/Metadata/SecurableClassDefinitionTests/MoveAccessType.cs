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
  public class MoveAccessType : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    [Test]
    public void MoveToFirstPosition ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var accessType2 = AccessTypeDefinition.NewObject();
      var accessType3 = AccessTypeDefinition.NewObject();
      var accessType4 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      securableClassDefinition.AddAccessType(accessType0);
      securableClassDefinition.AddAccessType(accessType1);
      securableClassDefinition.AddAccessType(accessType2);
      securableClassDefinition.AddAccessType(accessType3);
      securableClassDefinition.AddAccessType(accessType4);

      securableClassDefinition.MoveAccessType(0, accessType3);

      Assert.That(securableClassDefinition.AccessTypes, Is.EqualTo(new[] { accessType3, accessType0, accessType1, accessType2, accessType4 }));
      var references = new SecurableClassDefinitionWrapper(securableClassDefinition).AccessTypeReferences;
      for (int i = 0; i < references.Count; i++)
        Assert.That(((AccessTypeReference)references[i]).Index, Is.EqualTo(i));
    }

    [Test]
    public void MoveToEarlierPosition ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var accessType2 = AccessTypeDefinition.NewObject();
      var accessType3 = AccessTypeDefinition.NewObject();
      var accessType4 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      securableClassDefinition.AddAccessType(accessType0);
      securableClassDefinition.AddAccessType(accessType1);
      securableClassDefinition.AddAccessType(accessType2);
      securableClassDefinition.AddAccessType(accessType3);
      securableClassDefinition.AddAccessType(accessType4);

      securableClassDefinition.MoveAccessType(1, accessType3);

      Assert.That(securableClassDefinition.AccessTypes, Is.EqualTo(new[] { accessType0, accessType3, accessType1, accessType2, accessType4 }));
      var references = new SecurableClassDefinitionWrapper(securableClassDefinition).AccessTypeReferences;
      for (int i = 0; i < references.Count; i++)
        Assert.That(((AccessTypeReference)references[i]).Index, Is.EqualTo(i));
    }

    [Test]
    public void MoveToLaterPosition ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var accessType2 = AccessTypeDefinition.NewObject();
      var accessType3 = AccessTypeDefinition.NewObject();
      var accessType4 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      securableClassDefinition.AddAccessType(accessType0);
      securableClassDefinition.AddAccessType(accessType1);
      securableClassDefinition.AddAccessType(accessType2);
      securableClassDefinition.AddAccessType(accessType3);
      securableClassDefinition.AddAccessType(accessType4);

      securableClassDefinition.MoveAccessType(3, accessType1);

      Assert.That(securableClassDefinition.AccessTypes, Is.EqualTo(new[] { accessType0, accessType2, accessType3, accessType1, accessType4 }));
      var references = new SecurableClassDefinitionWrapper(securableClassDefinition).AccessTypeReferences;
      for (int i = 0; i < references.Count; i++)
        Assert.That(((AccessTypeReference)references[i]).Index, Is.EqualTo(i));
    }

    [Test]
    public void MoveToLastPosition ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var accessType2 = AccessTypeDefinition.NewObject();
      var accessType3 = AccessTypeDefinition.NewObject();
      var accessType4 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      securableClassDefinition.AddAccessType(accessType0);
      securableClassDefinition.AddAccessType(accessType1);
      securableClassDefinition.AddAccessType(accessType2);
      securableClassDefinition.AddAccessType(accessType3);
      securableClassDefinition.AddAccessType(accessType4);

      securableClassDefinition.MoveAccessType(4, accessType1);

      Assert.That(securableClassDefinition.AccessTypes, Is.EqualTo(new[] { accessType0, accessType2, accessType3, accessType4, accessType1 }));
      var references = new SecurableClassDefinitionWrapper(securableClassDefinition).AccessTypeReferences;
      for (int i = 0; i < references.Count; i++)
        Assert.That(((AccessTypeReference)references[i]).Index, Is.EqualTo(i));
    }

    [Test]
    public void TouchesSecurableClassDefinition ()
    {
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      var accessType = AccessTypeDefinition.NewObject();
      securableClassDefinition.AddAccessType(accessType);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        securableClassDefinition.EnsureDataAvailable();
        Assert.That(securableClassDefinition.State.IsUnchanged, Is.True);

        securableClassDefinition.MoveAccessType(0, accessType);

        Assert.That(securableClassDefinition.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void LeavesPermissionsUntouched ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.AddAccessType(accessType0);
      securableClassDefinition.AddAccessType(accessType1);

      var testHelper = new AccessControlTestHelper(ClientTransaction.Current);
      var acls = new List<AccessControlList>();
      acls.Add(testHelper.CreateStatefulAcl(securableClassDefinition));
      acls.Add(testHelper.CreateStatelessAcl(securableClassDefinition));

      foreach (var acl in acls)
      {
        var ace = acl.CreateAccessControlEntry();
        ace.DenyAccess(accessType0);
        ace.AllowAccess(accessType1);
      }

      securableClassDefinition.MoveAccessType(0, accessType1);
      foreach (var acl in acls)
      {
        var permissions = acl.AccessControlEntries[0].GetPermissions();
        Assert.That(permissions.Count, Is.EqualTo(2));
        Assert.That(permissions[0].AccessType, Is.SameAs(accessType1));
        Assert.That(permissions[0].Allowed, Is.True);
        Assert.That(permissions[1].AccessType, Is.SameAs(accessType0));
        Assert.That(permissions[1].Allowed, Is.False);
      }
    }

    [Test]
    public void FailsForNonExistentAccessType ()
    {
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddAccessType(AccessTypeDefinition.NewObject());
      securableClassDefinition.AddAccessType(AccessTypeDefinition.NewObject());
      Assert.That(
          () => securableClassDefinition.MoveAccessType(0, AccessTypeDefinition.NewObject(Guid.NewGuid(), "Test", 42)),
          Throws.ArgumentException
              .And.Message.StartsWith("The access type 'Test' is not associated with the securable class definition."));
    }

    [Test]
    public void FailsForIndexLessThanZero ()
    {
      var accessType = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddAccessType(AccessTypeDefinition.NewObject());
      securableClassDefinition.AddAccessType(accessType);
      Assert.That(
          () => securableClassDefinition.MoveAccessType(-1, accessType),
          Throws.TypeOf<ArgumentOutOfRangeException>()
              .And.Message.StartsWith(
                  "The index must not be less than 0 or greater than the top index of the access types for the securable class definition."));
    }

    [Test]
    public void FailsForIndexGreaterThanNumberOfItems ()
    {
      var accessType = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddAccessType(AccessTypeDefinition.NewObject());
      securableClassDefinition.AddAccessType(accessType);
      Assert.That(
          () => securableClassDefinition.MoveAccessType(2, accessType),
          Throws.TypeOf<ArgumentOutOfRangeException>()
              .And.Message.StartsWith(
                  "The index must not be less than 0 or greater than the top index of the access types for the securable class definition."));
    }
  }
}
