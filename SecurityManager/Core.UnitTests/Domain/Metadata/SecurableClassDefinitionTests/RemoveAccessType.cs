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
  public class RemoveAccessType : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    [Test]
    public void RemovesAccessType ()
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

      securableClassDefinition.RemoveAccessType(accessType2);

      Assert.That(securableClassDefinition.AccessTypes, Is.EqualTo(new[] { accessType0, accessType1, accessType3, accessType4 }));
      var references = new SecurableClassDefinitionWrapper(securableClassDefinition).AccessTypeReferences;
      Assert.That(((AccessTypeReference)references[0]).Index, Is.EqualTo(0));
      Assert.That(((AccessTypeReference)references[1]).Index, Is.EqualTo(1));
      Assert.That(((AccessTypeReference)references[2]).Index, Is.EqualTo(2));
      Assert.That(((AccessTypeReference)references[3]).Index, Is.EqualTo(3));
    }

    [Test]
    public void RemovesPermissionsForRemovedAccessType ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var accessType2 = AccessTypeDefinition.NewObject();

      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.AddAccessType(accessType0);
      securableClassDefinition.AddAccessType(accessType1);
      securableClassDefinition.AddAccessType(accessType2);

      var testHelper = new AccessControlTestHelper(ClientTransaction.Current);
      var acls = new List<AccessControlList>();
      acls.Add(testHelper.CreateStatefulAcl(securableClassDefinition));
      acls.Add(testHelper.CreateStatelessAcl(securableClassDefinition));

      foreach (var acl in acls)
        acl.CreateAccessControlEntry();

      securableClassDefinition.RemoveAccessType(accessType1);
      foreach (var acl in acls)
      {
        var permissions = acl.AccessControlEntries[0].GetPermissions();
        Assert.That(permissions.Count, Is.EqualTo(2));
        Assert.That(permissions[0].AccessType, Is.SameAs(accessType0));
        Assert.That(permissions[1].AccessType, Is.SameAs(accessType2));
      }
    }

    [Test]
    public void TouchesSecurableClassDefinition ()
    {
      var accessType = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.AddAccessType(accessType);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        securableClassDefinition.EnsureDataAvailable();
        Assert.That(securableClassDefinition.State.IsUnchanged, Is.True);

        securableClassDefinition.RemoveAccessType(accessType);

        Assert.That(securableClassDefinition.State.IsChanged, Is.True);
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
          () => securableClassDefinition.RemoveAccessType(AccessTypeDefinition.NewObject(Guid.NewGuid(), "Test", 42)),
          Throws.ArgumentException
              .And.Message.StartsWith("The access type 'Test' is not associated with the securable class definition."));
    }
  }
}
