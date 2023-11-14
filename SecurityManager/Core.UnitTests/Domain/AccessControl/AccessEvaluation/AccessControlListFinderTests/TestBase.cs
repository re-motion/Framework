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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation.AccessControlListFinderTests
{
  public class TestBase
  {
    private static readonly IDomainObjectHandle<StatePropertyDefinition> s_orderStatePropertyHandle = CreatePropertyHandle();
    private static readonly IDomainObjectHandle<StatePropertyDefinition> s_paymentStatePropertyHandle = CreatePropertyHandle();
    private static readonly IDomainObjectHandle<StatePropertyDefinition> s_deliveryPropertyHandle = CreatePropertyHandle();

    protected readonly State OrderState_Delivered = CreateState(s_orderStatePropertyHandle, "State", OrderState.Delivered);
    protected readonly State OrderState_Received = CreateState(s_orderStatePropertyHandle, "State", OrderState.Received);
    protected readonly State PaymentState_None = CreateState(s_paymentStatePropertyHandle, "Payment", PaymentState.None);
    protected readonly State PaymentState_Paid = CreateState(s_paymentStatePropertyHandle, "Payment", PaymentState.Paid);
    protected readonly State Delivery_Post = CreateState(s_deliveryPropertyHandle, "Delivery", Delivery.Post);
    protected readonly State Delivery_Dhl = CreateState(s_deliveryPropertyHandle, "Delivery", Delivery.Dhl);

    private Mock<ISecurityContextRepository> _securityContextRepositoryStub;

    [SetUp]
    public virtual void SetUp ()
    {
      _securityContextRepositoryStub = new Mock<ISecurityContextRepository>();
    }

    protected AccessControlListFinder CreateAccessControlListFinder ()
    {
      return new AccessControlListFinder(_securityContextRepositoryStub.Object);
    }

    protected void StubClassDefinition<TClass> (
        [CanBeNull] IDomainObjectHandle<StatelessAccessControlList> statelessAcl,
        [NotNull] params StatefulAccessControlListData[] statefulAcls)
        where TClass : ISecurableObject
    {
      _securityContextRepositoryStub
          .Setup(_ => _.GetClass(TypeUtility.GetPartialAssemblyQualifiedName(typeof(TClass))))
          .Returns(new SecurableClassDefinitionData(null, statelessAcl, statefulAcls));
    }

    protected void StubClassDefinition<TClass, TBaseClass> (
        [CanBeNull] IDomainObjectHandle<StatelessAccessControlList> statelessAcl,
        [NotNull] params StatefulAccessControlListData[] statefulAcls)
        where TClass : TBaseClass
        where TBaseClass : ISecurableObject
    {
      _securityContextRepositoryStub
          .Setup(_ => _.GetClass(TypeUtility.GetPartialAssemblyQualifiedName(typeof(TClass))))
          .Returns(new SecurableClassDefinitionData(TypeUtility.GetPartialAssemblyQualifiedName(typeof(TBaseClass)), statelessAcl, statefulAcls));
    }

    protected void StubGetStatePropertyValues ()
    {
      _securityContextRepositoryStub
          .Setup(_ => _.GetStatePropertyValues(s_orderStatePropertyHandle))
          .Returns(new[] { OrderState_Delivered.Value, OrderState_Received.Value });

      _securityContextRepositoryStub
          .Setup(_ => _.GetStatePropertyValues(s_paymentStatePropertyHandle))
          .Returns(new[] { PaymentState_None.Value, PaymentState_Paid.Value });

      _securityContextRepositoryStub
          .Setup(_ => _.GetStatePropertyValues(s_deliveryPropertyHandle))
          .Returns(new[] { Delivery_Post.Value, Delivery_Dhl.Value });
    }

    protected IDomainObjectHandle<StatelessAccessControlList> CreateStatelessAcl ()
    {
      return new ObjectID(typeof(StatelessAccessControlList), Guid.NewGuid()).GetHandle<StatelessAccessControlList>();
    }

    protected StatefulAccessControlListData CreateStatefulAcl (params State[] states)
    {
      var handle = new ObjectID(typeof(StatefulAccessControlList), Guid.NewGuid()).GetHandle<StatefulAccessControlList>();
      return new StatefulAccessControlListData(handle, states);
    }

    private static IDomainObjectHandle<StatePropertyDefinition> CreatePropertyHandle ()
    {
      return new ObjectID(typeof(StatePropertyDefinition), Guid.NewGuid()).GetHandle<StatePropertyDefinition>();
    }

    protected static State CreateState<TEnum> (IDomainObjectHandle<StatePropertyDefinition> propertyHandle, string propertyName, TEnum value)
    {
      var enumValue = ArgumentUtility.CheckType<Enum>("value", value);

      return new State(propertyHandle, propertyName, EnumWrapper.Get(enumValue).Name);
    }

    protected static State CreateState (string propertyName, string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);

      return new State(
          new ObjectID(typeof(StatePropertyDefinition), Guid.NewGuid()).GetHandle<StatePropertyDefinition>(),
          propertyName,
          value);
    }
  }
}
