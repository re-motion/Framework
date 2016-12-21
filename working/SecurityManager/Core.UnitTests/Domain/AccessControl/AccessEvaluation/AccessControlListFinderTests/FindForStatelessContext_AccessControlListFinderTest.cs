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
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation.AccessControlListFinderTests
{
  [TestFixture]
  public class FindForStatelessContext_AccessControlListFinderTest : TestBase
  {
    [Test]
    public void Find_ClassWithoutStateProperties_ReturnsStatelessAcl ()
    {
      var acl = CreateStatelessAcl();
      StubClassDefinition<Customer> (acl);
      var context = CreateContext<Customer>();

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find (context);

      Assert.That (foundAcl, Is.EqualTo (acl));
    }

    [Test]
    public void Find_ClassWithStateProperties_ReturnsStatelessAcl ()
    {
      var acl = CreateStatelessAcl();
      StubClassDefinition<Order> (acl);
      var context = CreateContext<Order>();

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find (context);

      Assert.That (foundAcl, Is.EqualTo (acl));
    }

    [Test]
    public void Find_WithInheritedAcl_ReturnsStatelessAclFromBaseClass ()
    {
      var acl = CreateStatelessAcl();
      StubClassDefinition<Order> (acl);
      StubClassDefinition<PremiumOrder, Order> (null, new StatefulAccessControlListData[0]);
      var context = CreateContext<PremiumOrder>();

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find (context);

      Assert.That (foundAcl, Is.EqualTo (acl));
    }

    [Test]
    public void Find_WithInheritedAclAndDerivedClassContainsOnlyStatefulAcl_ReturnsNull ()
    {
      var acl = CreateStatelessAcl();
      StubClassDefinition<Order> (acl);
      StubClassDefinition<PremiumOrder, Order> (null, CreateStatefulAcl());
      var context = CreateContext<PremiumOrder>();

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find (context);

      Assert.That (foundAcl, Is.Null);
    }

    [Test]
    public void Find_ClassWithoutAcl_ReturnsNull ()
    {
      StubClassDefinition<Customer> (null);
      var context = CreateContext<Customer>();

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find (context);

      Assert.That (foundAcl, Is.Null);
    }

    private void StubClassDefinition<TClass> ([CanBeNull] IDomainObjectHandle<StatelessAccessControlList> statelessAcl)
        where TClass : ISecurableObject
    {
      StubClassDefinition<TClass> (statelessAcl, CreateStatefulAcl());
    }

    private SecurityContext CreateContext<TClass>()
        where TClass : ISecurableObject
    {
      return SecurityContext.CreateStateless (typeof (TClass));
    }
  }
}