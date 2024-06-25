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
using System.Reflection;
#if NETFRAMEWORK
using System.Runtime.Serialization;
#else
using System.Runtime.CompilerServices;
#endif
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class DomainObjectTest : StandardMappingTest
  {
    private TestableClientTransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = new TestableClientTransaction();
    }

    [Test]
    public void Ctor_SetsObjectID ()
    {
      var instance = _transaction.ExecuteInScope(() => Order.NewObject());

      Assert.That(instance.ID, Is.Not.Null);
      Assert.That(instance.ID.ClassDefinition, Is.SameAs(MappingConfiguration.Current.GetTypeDefinition(typeof(Order))));
    }

    [Test]
    public void Ctor_SetsRootTransaction ()
    {
      var instance = _transaction.ExecuteInScope(() => Order.NewObject());

      Assert.That(instance.RootTransaction, Is.SameAs(_transaction));
    }

    [Test]
    public void Ctor_RegistersObject ()
    {
      TestDomainBase.StaticCtorHandler +=
          (sender, args) =>
          Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext.RegisteredObject, Is.SameAs(sender));

      Order instance;
      try
      {
        instance = _transaction.ExecuteInScope(() => Order.NewObject());
      }
      finally
      {
        TestDomainBase.ClearStaticCtorHandlers();
      }

      Assert.That(_transaction.IsEnlisted(instance), Is.True);
      var dataContainer = _transaction.DataManager.DataContainers[instance.ID];
      Assert.That(dataContainer, Is.Not.Null);
      Assert.That(dataContainer.DomainObject, Is.SameAs(instance));
      Assert.That(dataContainer.ClientTransaction, Is.SameAs(_transaction));
    }

    [Test]
    public void Ctor_RaisesReferenceInitializing ()
    {
      var domainObject = _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(domainObject.OnReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void Ctor_RaisesReferenceInitializing_InRightTransaction ()
    {
      var domainObject = _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(domainObject.OnReferenceInitializingTx, Is.SameAs(_transaction));
    }

    [Test]
    public void Ctor_RaisesReferenceInitializing_CalledBeforeCtor ()
    {
      var domainObject = _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(domainObject.OnReferenceInitializingCalledBeforeCtor, Is.True);
    }

    [Test]
    public void Ctor_WithVirtualPropertyCall_Allowed ()
    {
      var orderItem = _transaction.ExecuteInScope(() => OrderItem.NewObject("Test Toast"));
      Assert.That(_transaction.ExecuteInScope(() => orderItem.Product), Is.EqualTo("Test Toast"));
    }

    [Test]
    public void Ctor_DirectCall ()
    {
      Assert.That(
          () => new DomainObjectWithPublicCtor(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "DomainObject constructors must not be called directly. Use DomainObject.NewObject to create DomainObject instances."));
    }

    [Test]
    public void Initialize_ThrowsForNewObject ()
    {
      var orderItem = _transaction.ExecuteInScope(() => OrderItem.NewObject("Test Toast"));
      Assert.That(
          () => orderItem.Initialize(DomainObjectIDs.OrderItem1, _transaction),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The object cannot be initialized, it already has an ID."));
    }

    [Test]
    public void Initialize_ThrowsForLoadedObject ()
    {
      var orderItem = _transaction.ExecuteInScope(() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>());
      Assert.That(
          () => orderItem.Initialize(DomainObjectIDs.OrderItem1, _transaction),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The object cannot be initialized, it already has an ID."));
    }

    [Test]
    public void Initialize_WithUninitializedObject_SetsIDAndRootTransaction ()
    {
      var type = GetConcreteType(typeof(OrderItem));
#if NETFRAMEWORK
      var orderItem = (OrderItem)FormatterServices.GetSafeUninitializedObject(type);
#else
      var orderItem = (OrderItem)RuntimeHelpers.GetUninitializedObject(type);
#endif
      orderItem.Initialize(DomainObjectIDs.OrderItem1, _transaction);

      Assert.That(orderItem.ID, Is.EqualTo(DomainObjectIDs.OrderItem1));
      Assert.That(orderItem.RootTransaction, Is.SameAs(_transaction));
    }

    [Test]
    public void Initialize_ThrowsForNonRootTransaction ()
    {
      var type = GetConcreteType(typeof(OrderItem));
#if NETFRAMEWORK
      var orderItem = (OrderItem)FormatterServices.GetSafeUninitializedObject(type);
#else
      var orderItem = (OrderItem)RuntimeHelpers.GetUninitializedObject(type);
#endif
      Assert.That(
          () => orderItem.Initialize(DomainObjectIDs.OrderItem1, _transaction.CreateSubTransaction()),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The rootTransaction parameter must be passed a root transaction.",
              "rootTransaction"));
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_IDAndActiveTransaction ()
    {
      var domainObject = _transaction.ExecuteInScope(() => Order.NewObject()); // indirect call of RaiseReferenceInitializatingEvent
      Assert.That(domainObject.OnReferenceInitializingCalled, Is.True);
      Assert.That(domainObject.OnReferenceInitializingID, Is.EqualTo(domainObject.ID));
      Assert.That(domainObject.OnReferenceInitializingActiveTx, Is.SameAs(_transaction));

      using (ClientTransactionTestHelper.MakeInactive(_transaction))
      {
        Assert.That(_transaction.ActiveTransaction, Is.Not.SameAs(_transaction));

        // Note that GetObjectReference makes _transaction the active transaction.
        var objectReference = (Order)LifetimeService.GetObjectReference(_transaction, DomainObjectIDs.Order1);
        Assert.That(objectReference.OnReferenceInitializingCalled, Is.True);
        Assert.That(objectReference.OnReferenceInitializingID, Is.EqualTo(objectReference.ID));
        Assert.That(objectReference.OnReferenceInitializingActiveTx, Is.SameAs(_transaction));
      }
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_PropertyAccessForbidden ()
    {
      Assert.That(
          () => _transaction.ExecuteInScope(() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject(o => o.OrderNumber)),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_PropertiesForbidden ()
    {
      Assert.That(
          () => _transaction.ExecuteInScope(() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject(o => o.Properties)),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_CurrentPropertyForbidden ()
    {
      Assert.That(
          () => _transaction.ExecuteInScope(() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject(o => o.CurrentProperty)),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_TransactionContextIsRestricted ()
    {
      var result = _transaction.ExecuteInScope(() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject(o =>
      {
        var transactionContext = o.DefaultTransactionContext;
        Assert.That(() => transactionContext.State,
            Throws.InvalidOperationException.With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
        return transactionContext;
      }));
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_DeleteForbidden ()
    {
      Assert.That(
          () => _transaction.ExecuteInScope(() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject(o => { o.Delete(); return o; })),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_InvokesMixinHook ()
    {
      var domainObject = _transaction.ExecuteInScope(() => HookedTargetClass.NewObject()); // indirect call of RaiseReferenceInitializatingEvent
      var mixinInstance = Mixin.Get<HookedDomainObjectMixin>(domainObject);

      Assert.That(mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_InvokesMixinHook_WhilePropertyAccessForbidden ()
    {
      var mixinInstance = new HookedDomainObjectMixin();
      mixinInstance.InitializationHandler += (sender, args) => Dev.Null = ((HookedDomainObjectMixin)sender).Target.Property;

      using (new MixedObjectInstantiationScope(mixinInstance))
      {
        Assert.That(
            () => _transaction.ExecuteInScope(() => HookedTargetClass.NewObject()),  // indirect call of RaiseReferenceInitializatingEvent);
            Throws.InvalidOperationException
                .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
      }
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_ResetsFlagAfterNotification ()
    {
      var order = _transaction.ExecuteInScope(() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject(o => o));
      Dev.Null = _transaction.ExecuteInScope(() => order.OrderNumber); // succeeds
    }

    [Test]
    public void DefaultTransactionContext_SameAsAssociatedRootTransaction ()
    {
      var order = _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(order.DefaultTransactionContext.ClientTransaction, Is.SameAs(_transaction));
    }

    [Test]
    public void DefaultTransactionContext_SameAsActivatedTransaction ()
    {
      var order = _transaction.ExecuteInScope(() => Order.NewObject());

      var subTransaction = _transaction.CreateSubTransaction();
      Assert.That(order.DefaultTransactionContext.ClientTransaction, Is.SameAs(_transaction));

      using (subTransaction.EnterDiscardingScope())
      {
        Assert.That(order.DefaultTransactionContext.ClientTransaction, Is.SameAs(subTransaction));
      }
    }

    [Test]
    public void Delete_UsesActiveTransaction_NotCurrentTransaction ()
    {
      var order = DomainObjectMother.CreateObjectInOtherTransaction<Order>();
      Assert.That(_transaction.IsEnlisted(order), Is.False);
      _transaction.ExecuteInScope(order.Delete);

      Assert.That(_transaction.IsEnlisted(order), Is.False);
      Assert.That(order.State.IsInvalid, Is.True);
    }

    [Test]
    public void PropertyAccess_UsesActiveTransaction_NotCurrentTransaction ()
    {
      Order order = _transaction.ExecuteInScope(() => Order.NewObject());
      order.OrderNumber = 10;

      var otherTransaction = ClientTransaction.CreateRootTransaction();
      Assert.That(otherTransaction.IsEnlisted(order), Is.False);
      Assert.That(otherTransaction.ExecuteInScope(() => order.OrderNumber), Is.EqualTo(10));
    }

    [Test]
    public void OnLoaded_CanAccessPropertyValues ()
    {
      Order order = _transaction.ExecuteInScope(() => DomainObjectIDs.Order1.GetObjectReference<Order>());
      order.ProtectedLoaded += ((sender, e) => Assert.That(((Order)sender).OrderNumber, Is.EqualTo(1)));

      Assert.That(order.OnLoadedCalled, Is.False);

      _transaction.ExecuteInScope(order.EnsureDataAvailable);

      Assert.That(order.OnLoadedCalled, Is.True);
    }

    [Test]
    public void GetType_Throws ()
    {
      Order order = _transaction.ExecuteInScope(() => Order.NewObject());

      var targetInvocatioException = Assert.Throws<TargetInvocationException>(
          () => typeof(DomainObject).GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
              .Invoke(order, new object[0]));
      Assert.That(targetInvocatioException.InnerException, Is.InstanceOf<InvalidOperationException>());
      Assert.That(targetInvocatioException.InnerException?.Message, Is.EqualTo("DomainObject.GetType should not be used."));
    }

    [Test]
    public void GetPublicDomainObjectType ()
    {
      Customer customer = _transaction.ExecuteInScope(() => Customer.NewObject());
      Assert.That(customer.GetPublicDomainObjectType(), Is.SameAs(typeof(Customer)));
    }

    [Test]
    public new void ToString ()
    {
      Order order = _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(order.ToString(), Is.EqualTo(order.ID.ToString()));
    }

    [Test]
    public void State ()
    {
      Customer customer = _transaction.ExecuteInScope(() => DomainObjectIDs.Customer1.GetObject<Customer>());

      _transaction.ExecuteInScope(() => Assert.That(customer.State.IsUnchanged, Is.True));
      _transaction.ExecuteInScope(() => customer.Name = "New name");
      _transaction.ExecuteInScope(() => Assert.That(customer.State.IsChanged, Is.True));
    }

    [Test]
    public void NewObject_WithoutTransaction ()
    {
      Assert.That(
          () => Order.NewObject(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void NewObject_CallsCtor ()
    {
      var order = _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(order.CtorCalled, Is.True);
    }

    [Test]
    public void NewObject_ProtectedConstructor ()
    {
      Dev.Null = _transaction.ExecuteInScope(() => Company.NewObject());
    }

    [Test]
    public void NewObject_PublicConstructor ()
    {
      Dev.Null = _transaction.ExecuteInScope(() => Customer.NewObject());
    }

    [Test]
    public void NewObject_SetsNeedsLoadModeDataContainerOnly_True ()
    {
      var order = _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void GetObject_SetsNeedsLoadModeDataContainerOnly_ToTrue ()
    {
      var order = _transaction.ExecuteInScope(() => DomainObjectIDs.Order1.GetObject<Order>());
      Assert.That(order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void GetObject_WithoutTransaction ()
    {
      Assert.That(
          () => DomainObjectIDs.Order1.GetObject<Order>(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObject_Deleted ()
    {
      var order = _transaction.ExecuteInScope(() => DomainObjectIDs.Order1.GetObject<Order>());

      _transaction.ExecuteInScope(order.Delete);

      _transaction.ExecuteInScope(() => Assert.That(DomainObjectIDs.Order1.GetObject<Order>(includeDeleted: true), Is.SameAs(order)));
      _transaction.ExecuteInScope(() => Assert.That(order.State.IsDeleted, Is.True));
    }

    [Test]
    public void TryGetObject ()
    {
      Assert.That(_transaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);

      var order = _transaction.ExecuteInScope(() => DomainObjectIDs.Order1.TryGetObject<TestDomainBase>());

      Assert.That(order.ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(_transaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That(order.TransactionContext[_transaction].State.IsUnchanged, Is.True);
    }

    [Test]
    public void TryGetObject_NotFound ()
    {
      var objectID = new ObjectID(typeof(Order), Guid.NewGuid());
      Assert.That(_transaction.IsInvalid(objectID), Is.False);

      var order = _transaction.ExecuteInScope(() => objectID.TryGetObject<TestDomainBase>());

      Assert.That(order, Is.Null);
      Assert.That(_transaction.IsInvalid(objectID), Is.True);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_False_BeforeGetObject ()
    {
      var order = (Order)LifetimeService.GetObjectReference(_transaction, DomainObjectIDs.Order1);
      Assert.That(order.NeedsLoadModeDataContainerOnly, Is.False);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_True_AfterOnLoaded ()
    {
      var order = (Order)LifetimeService.GetObjectReference(_transaction, DomainObjectIDs.Order1);
      Assert.That(order.NeedsLoadModeDataContainerOnly, Is.False);

      PrivateInvoke.InvokeNonPublicMethod(order, typeof(DomainObject), "OnLoaded");

      Assert.That(order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void Properties ()
    {
      var order = _transaction.ExecuteInScope(() => Order.NewObject());
      var propertyIndexer = _transaction.ExecuteInScope(() => order.Properties);
      Assert.That(propertyIndexer, Is.Not.Null);
      Assert.That(propertyIndexer.DomainObject, Is.SameAs(order));
    }

    [Test]
    public void TransactionContext ()
    {
      var order = _transaction.ExecuteInScope(() => Order.NewObject());
      var transactionContextIndexer = order.TransactionContext;

      Assert.That(transactionContextIndexer, Is.InstanceOf(typeof(DomainObjectTransactionContextIndexer)));
      Assert.That(transactionContextIndexer[_transaction].DomainObject, Is.SameAs(order));
    }

    private Type GetConcreteType (Type requestedType)
    {
      var pipeline = ((DomainObjectCreator)GetTypeDefinition(requestedType).InstanceCreator).PipelineRegistry.DefaultPipeline;
      var type = pipeline.ReflectionService.GetAssembledType(requestedType);
      return type;
    }

    [DBTable]
    [ClassID("DomainObjectTest_DomainObjectWithPublicCtor")]
    public class DomainObjectWithPublicCtor : DomainObject
    {
      public DomainObjectWithPublicCtor ()
      {
      }
    }
  }
}
