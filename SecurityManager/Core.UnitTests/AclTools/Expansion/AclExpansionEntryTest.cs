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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionEntryTest : AclToolsTestBase
  {
    [Test]
    public void CtorTest ()
    {
      var accessConditions = new AclExpansionAccessConditions();
      var aclExpansionEntry = new AclExpansionEntry(User, Role, Acl, accessConditions, AccessTypeDefinitions, AccessTypeDefinitions2);
      Assert.That(aclExpansionEntry.User, Is.EqualTo(User));
      Assert.That(aclExpansionEntry.Role, Is.EqualTo(Role));
      Assert.That(aclExpansionEntry.Class, Is.EqualTo(Acl.Class));
      Assert.That(aclExpansionEntry.GetStateCombinations(), Is.EqualTo(Acl.StateCombinations));
      Assert.That(aclExpansionEntry.AccessConditions, Is.EqualTo(accessConditions));
      Assert.That(aclExpansionEntry.AllowedAccessTypes, Is.EqualTo(AccessTypeDefinitions));
      Assert.That(aclExpansionEntry.DeniedAccessTypes, Is.EqualTo(AccessTypeDefinitions2));
    }

    [Test]
    public void Initialize_WithAclHavingNullClass_ThrowsArgumentException ()
    {
      StatelessAccessControlList acl;
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        acl = StatelessAccessControlList.NewObject();
      }

      var accessConditions = new AclExpansionAccessConditions();
      Assert.That(
          () => new AclExpansionEntry(User, Role, acl, accessConditions, AccessTypeDefinitions, AccessTypeDefinitions2),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("AccessControlList must have a Class set.", "accessControlList"));
    }

    [Test]
    public void StateCombinationsForStatelessAclThrowsTest ()
    {
      SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition();
      var statlessAcl = TestHelper.CreateStatelessAcl(classDefinition);

      var accessConditions = new AclExpansionAccessConditions();
      var aclExpansionEntry = new AclExpansionEntry(User, Role, statlessAcl, accessConditions, AccessTypeDefinitions, AccessTypeDefinitions2);
      Assert.That(
          () => aclExpansionEntry.GetStateCombinations(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  @"StateCombinations not defined for StatelessAccessControlList. Test for ""is StatefulAccessControlList"" in calling code."));
    }


    [Test]
    public void GetStateCombinationsTest ()
    {
      SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition();
      var aclExpansionEntry = new AclExpansionEntry(User, Role, Acl, new AclExpansionAccessConditions(), AccessTypeDefinitions, AccessTypeDefinitions2);
      var result = aclExpansionEntry.GetStateCombinations();
      Assert.That(result, Is.EqualTo(Acl.StateCombinations));
    }
  }
}
