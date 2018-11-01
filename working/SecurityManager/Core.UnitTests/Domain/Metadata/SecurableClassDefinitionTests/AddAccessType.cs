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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.SecurableClassDefinitionTests
{
  [TestFixture]
  public class AddAccessType : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    [Test]
    public void AddsAccessTypes ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      securableClassDefinition.AddAccessType (accessType0);
      securableClassDefinition.AddAccessType (accessType1);

      Assert.That (securableClassDefinition.AccessTypes, Is.EqualTo (new[] { accessType0, accessType1 }));
      var references = new SecurableClassDefinitionWrapper (securableClassDefinition).AccessTypeReferences;
      Assert.That (((AccessTypeReference) references[0]).Index, Is.EqualTo (0));
      Assert.That (((AccessTypeReference) references[1]).Index, Is.EqualTo (1));
    }

    [Test]
    public void TouchesSecurableClassDefinition ()
    {
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        securableClassDefinition.EnsureDataAvailable();
        Assert.That (securableClassDefinition.State, Is.EqualTo (StateType.Unchanged));

        securableClassDefinition.AddAccessType (AccessTypeDefinition.NewObject());

        Assert.That (securableClassDefinition.State, Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void AddsPermissionsForNewAccessType()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.AddAccessType (accessType0);

      var testHelper = new AccessControlTestHelper (ClientTransaction.Current);
      var acls = new List<AccessControlList>();
      acls.Add (testHelper.CreateStatefulAcl (securableClassDefinition));
      acls.Add (testHelper.CreateStatelessAcl (securableClassDefinition));

      foreach (var acl in acls)
        acl.CreateAccessControlEntry();

      securableClassDefinition.AddAccessType (accessType1);
      foreach (var acl in acls)
      {
        var permissions = acl.AccessControlEntries[0].GetPermissions();
        Assert.That (permissions.Count, Is.EqualTo (2));
        Assert.That (permissions[1].AccessType, Is.SameAs (accessType1));
        Assert.That (permissions[1].Allowed, Is.Null);
      }
    }

    [Test]
    public void FailsForExistingAccessType ()
    {
      var accessType = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Test", 42);

      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddAccessType (accessType);
      Assert.That (
          () => securableClassDefinition.AddAccessType (accessType),
          Throws.ArgumentException
              .And.Message.StartsWith ("The access type 'Test' has already been added to the securable class definition."));
    }
  }
}