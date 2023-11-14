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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.DomainImplementation.Cloning;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Cloning
{
  [TestFixture]
  public class DomainObjectClonerTest : ClientTransactionBaseTest
  {
    private DomainObjectCloner _cloner;
    private ClassWithAllDataTypes _classWithAllDataTypes;
    private Order _order1;
    private Computer _computer1;
    private ClassWithAllDataTypes _boundSource;
    private ClassWithClonerCallback _classWithClonerCallback;

    public override void SetUp ()
    {
      base.SetUp();
      _cloner = new DomainObjectCloner();
      _classWithAllDataTypes = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _computer1 = DomainObjectIDs.Computer1.GetObject<Computer>();

      _boundSource = ClientTransaction.CreateRootTransaction().ExecuteInScope(() => ClassWithAllDataTypes.NewObject());
      _boundSource.Int32Property = 123;

      _classWithClonerCallback =
          (ClassWithClonerCallback)LifetimeService.NewObject(TestableClientTransaction, typeof(ClassWithClonerCallback), ParamList.Empty);
    }

    [Test]
    public void CloneTransaction_CurrentByDefault ()
    {
      Assert.That(_cloner.CloneTransaction, Is.SameAs(TestableClientTransaction));
    }

    [Test]
    public void CloneTransaction_ManualSet ()
    {
      ClientTransaction cloneTransaction = ClientTransactionObjectMother.Create();
      _cloner.CloneTransaction = cloneTransaction;
      Assert.That(_cloner.CloneTransaction, Is.SameAs(cloneTransaction));
    }

    [Test]
    public void CloneTransaction_Reset ()
    {
      ClientTransaction cloneTransaction = ClientTransactionObjectMother.Create();
      _cloner.CloneTransaction = cloneTransaction;
      _cloner.CloneTransaction = null;
      Assert.That(_cloner.CloneTransaction, Is.SameAs(TestableClientTransaction));
    }

    [Test]
    public void CreateCloneHull_CreatesNewObject ()
    {
      DomainObject clone = _cloner.CreateCloneHull(_classWithAllDataTypes);
      Assert.That(clone, Is.Not.SameAs(_classWithAllDataTypes));
      Assert.That(clone.ID, Is.Not.EqualTo(_classWithAllDataTypes));
      Assert.That(clone.State.IsNew, Is.True);
    }

    [Test]
    public void CreateCloneHull_CreatesObjectOfSameType ()
    {
      var clone = _cloner.CreateCloneHull<DomainObject>(_classWithAllDataTypes);
      Assert.That(clone.GetPublicDomainObjectType(), Is.SameAs(typeof(ClassWithAllDataTypes)));
    }

    [Test]
    public void CreateCloneHull_CallsNoCtor_UsesFactory ()
    {
      Order clone = _cloner.CreateCloneHull(_order1);
      Assert.That(clone.CtorCalled, Is.False);
      var pipeline = ((DomainObjectCreator)clone.ID.ClassDefinition.InstanceCreator).PipelineRegistry.DefaultPipeline;
      Assert.That(pipeline.ReflectionService.IsAssembledType(((object)clone).GetType()));
    }

    [Test]
    public void CreateCloneHull_RegistersDataContainer ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();
      _cloner.CloneTransaction = transaction;
      var clone = _cloner.CreateCloneHull(_classWithAllDataTypes);
      Assert.That(clone.GetInternalDataContainerForTransaction(transaction).ClientTransaction, Is.SameAs(transaction));
    }

    [Test]
    public void CreateCloneHull_SetsDomainObjectOfDataContainer ()
    {
      ClassWithAllDataTypes clone = _cloner.CreateCloneHull(_classWithAllDataTypes);
      Assert.That(clone.InternalDataContainer.DomainObject, Is.SameAs(clone));
    }

    [Test]
    public void CreateCloneHull_BindsAndEnlistsObjectToCloneTransaction ()
    {
      var cloneTransaction = ClientTransaction.CreateRootTransaction();
      _cloner.CloneTransaction = cloneTransaction;

      DomainObject clone = _cloner.CreateCloneHull(_classWithAllDataTypes);

      Assert.That(clone.RootTransaction, Is.SameAs(cloneTransaction));
      Assert.That(cloneTransaction.IsEnlisted(clone), Is.True);
    }

    [Test]
    public void CreateCloneHull_TouchesNoProperties ()
    {
      ClassWithAllDataTypes clone = _cloner.CreateCloneHull(_classWithAllDataTypes);
      Assert.That(clone.Int32Property, Is.Not.EqualTo(_classWithAllDataTypes.Int32Property));
      Assert.That(clone.Properties[typeof(ClassWithAllDataTypes), "Int32Property"].HasBeenTouched, Is.False);
    }

    [Test]
    public void CreateCloneHull_DoesNotInvokeClonerCallback ()
    {
      var clone = _cloner.CreateCloneHull(_classWithClonerCallback);
      Assert.That(clone.CallbackInvoked, Is.False);
    }

    [Test]
    public void CreateValueClone_SimpleProperties ()
    {
      ClassWithAllDataTypes clone = _cloner.CreateValueClone(_classWithAllDataTypes);
      Assert.That(clone.Int32Property, Is.EqualTo(_classWithAllDataTypes.Int32Property));
    }

    [Test]
    public void CreateValueClone_OriginalValueNotCloned ()
    {
      _classWithAllDataTypes.Int32Property = -2;
      ClassWithAllDataTypes clone = _cloner.CreateValueClone(_classWithAllDataTypes);

      Assert.That(clone.Int32Property, Is.EqualTo(_classWithAllDataTypes.Int32Property));
      Assert.That(clone.Properties[typeof(ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int>(),
          Is.Not.EqualTo(_classWithAllDataTypes.Properties[typeof(ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int>()));
      Assert.That(clone.Properties[typeof(ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int>(), Is.EqualTo(0));
    }

    [Test]
    public void CreateValueClone_RelationProperties_NonForeignKey ()
    {
      Order clone = _cloner.CreateValueClone(_order1);
      Assert.That(clone.OrderItems, Is.Empty);
      Assert.That(clone.OrderTicket, Is.Null);
    }

    [Test]
    public void CreateValueClone_RelationProperties_ForeignKey ()
    {
      Computer clone = _cloner.CreateValueClone(_computer1);
      Assert.That(_computer1.Employee, Is.Not.Null);
      Assert.That(clone.Employee, Is.Null);
    }

    [Test]
    public void CreateValueClone_RelationProperties_ForeignKey_OriginalValue ()
    {
      Computer clone = _cloner.CreateValueClone(_computer1);
      Assert.That(_computer1.Employee, Is.Not.Null);
      Assert.That(clone.Properties[typeof(Computer), "Employee"].GetOriginalValue<Employee>(), Is.Null);
    }

    [Test]
    public void CreateValueClone_InvokesClonerCallback ()
    {
      var cloneTransaction = ClientTransaction.CreateRootTransaction();
      _cloner.CloneTransaction = cloneTransaction;
      _classWithClonerCallback.Property = 42;

      var clone = _cloner.CreateValueClone(_classWithClonerCallback);

      Assert.That(clone.CallbackInvoked, Is.True);
      Assert.That(clone.CallbackOriginal, Is.SameAs(_classWithClonerCallback));
      Assert.That(clone.CallbackCloneTransaction, Is.SameAs(cloneTransaction));
      Assert.That(clone.CallbackCurrentTransaction, Is.SameAs(ClientTransaction.Current));
      Assert.That(clone.PropertyValueInCallback, Is.EqualTo(42));
    }

    [Test]
    public void SourceTransaction_IsRespected ()
    {
      ClassWithAllDataTypes unboundClone = _cloner.CreateValueClone(_boundSource);
      Assert.That(unboundClone.Int32Property, Is.EqualTo(123));
    }

    [Test]
    public void NullTransaction_ForCloneTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.That(
            () => _cloner.CreateValueClone(_boundSource),
            Throws.InvalidOperationException
                .With.Message.EqualTo("No ClientTransaction has been associated with the current thread."));
      }
    }

    [Test]
    public void CreateClone_CreatesValueClone ()
    {
      var strategyMock = new Mock<ICloneStrategy>();

      Order clone = _cloner.CreateClone(_order1, strategyMock.Object);
      Assert.That(clone.OrderNumber, Is.EqualTo(_order1.OrderNumber));
      Assert.That(clone.DeliveryDate, Is.EqualTo(_order1.DeliveryDate));
    }

    [Test]
    public void CreateClone_CallsStrategyForReferences ()
    {
      var strategyMock = new Mock<ICloneStrategy>(MockBehavior.Strict);
      var contextMock = new Mock<CloneContext>(_cloner);
      Order clone = Order.NewObject();
      var shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>>();

      contextMock.Setup(_ => _.GetCloneFor(_order1)).Returns(clone);
      shallowClonesFake.Enqueue(new Tuple<DomainObject, DomainObject>(_order1, clone));
      contextMock.Setup(_ => _.CloneHulls).Returns(shallowClonesFake);

      ExpectHandleReference(strategyMock, _order1, clone, "OrderItems", ClientTransaction.Current, ClientTransaction.Current);
      ExpectHandleReference(strategyMock, _order1, clone, "OrderTicket", ClientTransaction.Current, ClientTransaction.Current);
      ExpectHandleReference(strategyMock, _order1, clone, "Official", ClientTransaction.Current, ClientTransaction.Current);
      ExpectHandleReference(strategyMock, _order1, clone, "Customer", ClientTransaction.Current, ClientTransaction.Current);

      _cloner.CreateClone(_order1, strategyMock.Object, contextMock.Object);
      strategyMock.Verify();
    }

    [Test]
    public void CreateClone_CallsStrategyForReferences_OnReferencedObjectsToo ()
    {
      var strategyMock = new Mock<ICloneStrategy>(MockBehavior.Strict);
      var contextMock = new Mock<CloneContext>(_cloner);
      Order clone = Order.NewObject();
      OrderItem clone2 = OrderItem.NewObject();
      var shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>>();

      contextMock.Setup(_ => _.GetCloneFor(_order1)).Returns(clone);
      shallowClonesFake.Enqueue(new Tuple<DomainObject, DomainObject>(_order1, clone));
      shallowClonesFake.Enqueue(new Tuple<DomainObject, DomainObject>(_order1.OrderItems[0], clone2));
      contextMock.Setup(_ => _.CloneHulls).Returns(shallowClonesFake);

      ExpectHandleReference(strategyMock, _order1, clone, "OrderItems", ClientTransaction.Current, ClientTransaction.Current);
      ExpectHandleReference(strategyMock, _order1, clone, "OrderTicket", ClientTransaction.Current, ClientTransaction.Current);
      ExpectHandleReference(strategyMock, _order1, clone, "Official", ClientTransaction.Current, ClientTransaction.Current);
      ExpectHandleReference(strategyMock, _order1, clone, "Customer", ClientTransaction.Current, ClientTransaction.Current);

      ExpectHandleReference(strategyMock, _order1.OrderItems[0], clone2, "Order", ClientTransaction.Current, ClientTransaction.Current);

      _cloner.CreateClone(_order1, strategyMock.Object, contextMock.Object);
      strategyMock.Verify();
    }

    [Test]
    public void CreateClone_CallsStrategyForReferences_OnlyWhenNotTouched ()
    {
      var strategyMock = new Mock<ICloneStrategy>(MockBehavior.Strict);
      var contextMock = new Mock<CloneContext>(_cloner);
      Order clone = Order.NewObject();
      OrderItem clone2 = OrderItem.NewObject();
      var shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>>();

      clone.OrderTicket = clone.OrderTicket;
      clone.Official = clone.Official;
      clone.Customer = clone.Customer;
      clone.OrderItems.Add(clone2);

      contextMock.Setup(_ => _.GetCloneFor(_order1)).Returns(clone);
      shallowClonesFake.Enqueue(new Tuple<DomainObject, DomainObject>(_order1, clone));
      shallowClonesFake.Enqueue(new Tuple<DomainObject, DomainObject>(_order1.OrderItems[0], clone2));
      contextMock.Setup(_ => _.CloneHulls).Returns(shallowClonesFake);

      // not called: ExpectHandleReference (strategyMock, _order1, clone, "OrderItems", ClientTransaction.Current, ClientTransaction.Current);
      // not called: ExpectHandleReference (strategyMock, _order1, clone, "OrderTicket", ClientTransaction.Current, ClientTransaction.Current);
      // not called: ExpectHandleReference (strategyMock, _order1, clone, "Official", ClientTransaction.Current, ClientTransaction.Current);
      // not called: ExpectHandleReference (strategyMock, _order1, clone, "Customer", ClientTransaction.Current, ClientTransaction.Current);
      // not called: ExpectHandleReference (strategyMock, _order1.OrderItems[0], clone2, "Order", ClientTransaction.Current, ClientTransaction.Current);

      _cloner.CreateClone(_order1, strategyMock.Object, contextMock.Object);
      strategyMock.Verify();
    }

    [Test]
    public void CreateClone_CallsStrategy_WithCorrectTransactions ()
    {
      ClientTransaction sourceTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransaction cloneTransaction = ClientTransaction.CreateRootTransaction();

      _cloner.CloneTransaction = cloneTransaction;

      var strategyMock = new Mock<ICloneStrategy>(MockBehavior.Strict);
      var contextMock = new Mock<CloneContext>(_cloner);

      Order source;
      using (sourceTransaction.EnterNonDiscardingScope())
        source = Order.NewObject();
      Order clone;
      using (cloneTransaction.EnterNonDiscardingScope())
        clone = Order.NewObject();

      var shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>>();

      contextMock.Setup(_ => _.GetCloneFor(source)).Returns(clone);
      shallowClonesFake.Enqueue(new Tuple<DomainObject, DomainObject>(source, clone));
      contextMock.Setup(_ => _.CloneHulls).Returns(shallowClonesFake);

      ExpectHandleReference(strategyMock, source, clone, "OrderItems", sourceTransaction, cloneTransaction);
      ExpectHandleReference(strategyMock, source, clone, "OrderTicket", sourceTransaction, cloneTransaction);
      ExpectHandleReference(strategyMock, source, clone, "Official", sourceTransaction, cloneTransaction);
      ExpectHandleReference(strategyMock, source, clone, "Customer", sourceTransaction, cloneTransaction);

      _cloner.CreateClone(source, strategyMock.Object, contextMock.Object);
      strategyMock.Verify();
    }

    [Test]
    public void CreateClone_InvokesClonerCallback ()
    {
      var cloneTransaction = ClientTransaction.CreateRootTransaction();
      _cloner.CloneTransaction = cloneTransaction;
      _classWithClonerCallback.Property = 42;

      var clone = _cloner.CreateClone(_classWithClonerCallback, new CompleteCloneStrategy());

      Assert.That(clone.CallbackInvoked, Is.True);
      Assert.That(clone.CallbackOriginal, Is.SameAs(_classWithClonerCallback));
      Assert.That(clone.CallbackCloneTransaction, Is.SameAs(cloneTransaction));
      Assert.That(clone.CallbackCurrentTransaction, Is.SameAs(ClientTransaction.Current));
      Assert.That(clone.PropertyValueInCallback, Is.EqualTo(42));
    }

    [Test]
    public void CreateClone_InvokesClonerCallback_OnReferencedObjects ()
    {
      var cloneTransaction = ClientTransaction.CreateRootTransaction();
      _cloner.CloneTransaction = cloneTransaction;

      var referencedObject = (ClassWithClonerCallback)LifetimeService.NewObject(TestableClientTransaction, typeof(ClassWithClonerCallback), ParamList.Empty);
      referencedObject.Property = 42;
      _classWithClonerCallback.ReferencedObject = referencedObject;

      var clone = _cloner.CreateClone(_classWithClonerCallback, new CompleteCloneStrategy());
      var clonedReferencedObject = cloneTransaction.ExecuteInScope(() => clone.ReferencedObject);

      Assert.That(clonedReferencedObject.CallbackInvoked, Is.True);
      Assert.That(clonedReferencedObject.CallbackOriginal, Is.SameAs(referencedObject));
      Assert.That(clonedReferencedObject.CallbackCloneTransaction, Is.SameAs(cloneTransaction));
      Assert.That(clonedReferencedObject.CallbackCurrentTransaction, Is.SameAs(ClientTransaction.Current));
      Assert.That(clonedReferencedObject.PropertyValueInCallback, Is.EqualTo(42));
    }

    [Test]
    public void CreateClone_WithInvalidContext ()
    {
      Assert.That(() => _cloner.CreateClone(_classWithAllDataTypes, new CompleteCloneStrategy(), new CloneContext(_cloner)), Throws.Nothing);
      var otherCloner = new DomainObjectCloner();
      Assert.That(
          () => _cloner.CreateClone(_classWithAllDataTypes, new CompleteCloneStrategy(), new CloneContext(otherCloner)),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The given CloneContext must have been created for this DomainObjectCloner.", "context"));
    }

    private void ExpectHandleReference (
        Mock<ICloneStrategy> strategyMock,
        TestDomainBase original,
        TestDomainBase clone,
        string propertyName,
        ClientTransaction sourceTransaction,
        ClientTransaction cloneTransaction)
    {
      strategyMock
          .Setup(
              _ => _.HandleReference(
                  original.Properties[original.GetPublicDomainObjectType(), propertyName, sourceTransaction],
                  clone.Properties[clone.GetPublicDomainObjectType(), propertyName, cloneTransaction],
                  It.IsAny<CloneContext>()))
          .Verifiable();
    }

    [DBTable]
    [IncludeInMappingTestDomain]
    public class ClassWithClonerCallback : DomainObject, IClonerCallback
    {
      public virtual int Property { get; set; }
      public virtual ClassWithClonerCallback ReferencedObject { get; set; }

      [StorageClassNone]
      public bool CallbackInvoked { get; private set; }
      [StorageClassNone]
      public ClientTransaction CallbackCloneTransaction { get; private set; }
      [StorageClassNone]
      public ClientTransaction CallbackCurrentTransaction { get; private set; }
      [StorageClassNone]
      public DomainObject CallbackOriginal { get; private set; }
      [StorageClassNone]
      public int PropertyValueInCallback { get; private set; }

      public void OnObjectCreatedAsClone (ClientTransaction cloneTransaction, DomainObject original)
      {
        CallbackInvoked = true;
        CallbackCloneTransaction = cloneTransaction;
        CallbackCurrentTransaction = ClientTransaction.Current;
        CallbackOriginal = original;
        PropertyValueInCallback = cloneTransaction.ExecuteInScope(() => Property);
      }
    }
  }
}
