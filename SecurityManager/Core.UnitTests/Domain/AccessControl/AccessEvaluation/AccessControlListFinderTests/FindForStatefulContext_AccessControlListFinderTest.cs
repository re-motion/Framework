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
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation.AccessControlListFinderTests
{
  [TestFixture]
  public class FindForStatefulContext_AccessControlListFinderTest : TestBase
  {
    public override void SetUp ()
    {
      base.SetUp();
      StubGetStatePropertyValues();
    }

    [Test]
    public void Find_WithoutStateProperties_ReturnsStatefulAcl ()
    {
      var acl = CreateStatefulAcl();
      StubClassDefinition<Customer>(acl);
      var context = CreateContextForCustomer();

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find(context);

      Assert.That(foundAcl, Is.EqualTo(acl.Handle));
    }

    [Test]
    public void Find_ClassWithoutAcl_ReturnsNull ()
    {
      StubClassDefinition<Customer>();
      var context = CreateContextForCustomer();

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find(context);

      Assert.That(foundAcl, Is.Null);
    }

    [Test]
    public void Find_WithMatchingState_ReturnsStatefulAcl ()
    {
      var acl = CreateStatefulAcl(OrderState_Delivered, PaymentState_None);
      StubClassDefinition<Order>(
          CreateStatefulAcl(OrderState_Received, PaymentState_None),
          acl);
      var context = CreateContextForOrder(OrderState.Delivered, PaymentState.None);

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find(context);

      Assert.That(foundAcl, Is.EqualTo(acl.Handle));
    }

    [Test]
    public void Find_WithMissingStateInContext_ThrowsAccessControlException ()
    {
      var acl = CreateStatefulAcl(OrderState_Delivered, PaymentState_None, CreateState("MissingState", "Value"));
      StubClassDefinition<Order>(acl);
      var context = CreateContextForOrder(OrderState.Delivered, PaymentState.None);

      var aclFinder = CreateAccessControlListFinder();
      Assert.That(
          () => aclFinder.Find(context),
          Throws.TypeOf<AccessControlException>().And.Message.EqualTo("The state 'MissingState' is missing in the security context."));
    }

    [Test]
    public void Find_WithInvalidStateValueInContext_ThrowsAccessControlException ()
    {
      var acl = CreateStatefulAcl(OrderState_Delivered, PaymentState_None);
      StubClassDefinition<Order>(acl);

      var states = new Dictionary<string, Enum>();
      states.Add("State", Delivery.Post);
      states.Add("Payment", PaymentState.None);
      var context = SecurityContext.Create(typeof(Order), "owner", "ownerGroup", "ownerTenant", states, new Enum[0]);

      var aclFinder = CreateAccessControlListFinder();
      Assert.That(
          () => aclFinder.Find(context),
          Throws.TypeOf<AccessControlException>()
                .And.Message.EqualTo(
                    "The state 'Post|Remotion.SecurityManager.UnitTests.TestDomain.Delivery, Remotion.SecurityManager.UnitTests' is not defined "
                    + "for the property 'State' of the securable class 'Remotion.SecurityManager.UnitTests.TestDomain.Order, Remotion.SecurityManager.UnitTests' "
                    + "or its base classes."));
    }

    [Test]
    public void Find_WithUnknownStatePropertyInContext_ReturnsNull ()
    {
      StubClassDefinition<Customer>();

      var states = new Dictionary<string, Enum>();
      states.Add("Unknown", Delivery.Post);
      var context = SecurityContext.Create(typeof(Customer), "owner", "ownerGroup", "ownerTenant", states, new Enum[0]);

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find(context);

      Assert.That(foundAcl, Is.Null);
    }

    [Test]
    public void Find_WithInheritedAcl_ReturnsStatefulAclFromBaseClass ()
    {
      var acl = CreateStatefulAcl(OrderState_Delivered, PaymentState_None);
      StubClassDefinition<Order>(acl);
      StubClassDefinitionWithoutStatelessAcl<SpecialOrder, Order>();
      var context = CreateContextForSpecialOrder(OrderState.Delivered, PaymentState.None);

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find(context);

      Assert.That(foundAcl, Is.EqualTo(acl.Handle));
    }

    [Test]
    public void Find_WithInheritedAclAndClassHasStatelessAcl_ReturnsNull ()
    {
      var acl = CreateStatefulAcl(OrderState_Delivered, PaymentState_None);
      StubClassDefinition<Order>(acl);
      StubClassDefinition<SpecialOrder, Order>();
      var context = CreateContextForSpecialOrder(OrderState.Delivered, PaymentState.None);

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find(context);

      Assert.That(foundAcl, Is.Null);
    }

    [Test]
    public void Find_WithAclAndInteritedAclMatchingStates_ReturnsStatefulAclFromDerivedClass ()
    {
      var acl = CreateStatefulAcl(OrderState_Delivered, PaymentState_None);
      StubClassDefinition<Order>(CreateStatefulAcl(OrderState_Delivered, PaymentState_None));
      StubClassDefinition<SpecialOrder, Order>(acl);
      var context = CreateContextForSpecialOrder(OrderState.Delivered, PaymentState.None);

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find(context);

      Assert.That(foundAcl, Is.EqualTo(acl.Handle));
    }

    [Test]
    public void Find_WithInheritedAclMatchingStatesAndAclNotMatchingStates_ReturnsNull ()
    {
      var acl = CreateStatefulAcl(OrderState_Delivered, PaymentState_None);
      StubClassDefinition<Order>(acl);
      StubClassDefinitionWithoutStatelessAcl<SpecialOrder, Order>( CreateStatefulAcl(OrderState_Received, PaymentState_None));
      var context = CreateContextForSpecialOrder(OrderState.Delivered, PaymentState.None);

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find(context);

      Assert.That(foundAcl, Is.Null);
    }

    [Test]
    public void Find_WithInheritedAclNotDefiningStateAndAclMatchingStates_ReturnsStatefulAclFromDerivedClass ()
    {
      StubClassDefinition<Order>(CreateStatefulAcl(OrderState_Delivered, PaymentState_None));
      var acl = CreateStatefulAcl(OrderState_Delivered, PaymentState_None, Delivery_Dhl);
      StubClassDefinitionWithoutStatelessAcl<PremiumOrder, Order>(acl);
      var context = CreateContextForPremiumOrder(OrderState.Delivered, PaymentState.None, Delivery.Dhl);

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find(context);

      Assert.That(foundAcl, Is.EqualTo(acl.Handle));
    }

    [Test]
    public void Find_WithInheritedAclNotDefiningStateAndAclNotMatchingStates_ReturnsNull ()
    {
      StubClassDefinition<Order>(CreateStatefulAcl(OrderState_Delivered, PaymentState_None));
      var acl = CreateStatefulAcl(OrderState_Received, PaymentState_None, Delivery_Dhl);
      StubClassDefinitionWithoutStatelessAcl<PremiumOrder, Order>(acl);
      var context = CreateContextForPremiumOrder(OrderState.Delivered, PaymentState.None, Delivery.Post);

      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find(context);

      Assert.That(foundAcl, Is.Null);
    }

    private void StubClassDefinition<TClass> ([NotNull] params StatefulAccessControlListData[] statefulAcls)
        where TClass : ISecurableObject
    {
      StubClassDefinition<TClass>(CreateStatelessAcl(), statefulAcls);
    }

    private void StubClassDefinition<TClass, TBaseClass> ([NotNull] params StatefulAccessControlListData[] statefulAcls)
        where TClass : TBaseClass
        where TBaseClass : ISecurableObject
    {
      StubClassDefinition<TClass, TBaseClass>(CreateStatelessAcl(), statefulAcls);
    }

    private void StubClassDefinitionWithoutStatelessAcl<TClass, TBaseClass> ([NotNull] params StatefulAccessControlListData[] statefulAcls)
        where TClass : TBaseClass
        where TBaseClass : ISecurableObject
    {
      StubClassDefinition<TClass, TBaseClass>(null, statefulAcls);
    }

    private SecurityContext CreateContextForCustomer ()
    {
      return SecurityContext.Create(
          typeof(Customer),
          "owner",
          "ownerGroup",
          "ownerTenant",
          new Dictionary<string, Enum>(),
          new Enum[0]);
    }

    private SecurityContext CreateContextForOrder (OrderState orderState, PaymentState paymentState)
    {
      var states = new Dictionary<string, Enum>();
      states.Add("State", orderState);
      states.Add("Payment", paymentState);

      return SecurityContext.Create(typeof(Order), "owner", "ownerGroup", "ownerTenant", states, new Enum[0]);
    }

    private SecurityContext CreateContextForSpecialOrder (OrderState orderState, PaymentState paymentState)
    {
      var states = new Dictionary<string, Enum>();
      states.Add("State", orderState);
      states.Add("Payment", paymentState);

      return SecurityContext.Create(typeof(SpecialOrder), "owner", "ownerGroup", "ownerTenant", states, new Enum[0]);
    }

    private SecurityContext CreateContextForPremiumOrder (OrderState orderState, PaymentState paymentState, Delivery delivery)
    {
      var states = new Dictionary<string, Enum>();
      states.Add("State", orderState);
      states.Add("Payment", paymentState);
      states.Add("Delivery", delivery);

      return SecurityContext.Create(typeof(PremiumOrder), "owner", "ownerGroup", "ownerTenant", states, new Enum[0]);
    }
  }
}
