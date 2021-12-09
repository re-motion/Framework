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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe
{
  [TestFixture]
  public class DomainObjectCreatorTest : StandardMappingTest
  {
    private DomainObjectCreator _domainObjectCreator;

    private ClientTransaction _transaction;
    private IObjectInitializationContext _order1InitializationContext;
    private IObjectInitializationContext _targetClassForPersistentMixinInitializationContext;
    private IPipeline _pipeline;

    public override void SetUp ()
    {
      base.SetUp();

      var remixParticipant = new MixinParticipant(
          SafeServiceLocator.Current.GetInstance<IConfigurationProvider>(),
          SafeServiceLocator.Current.GetInstance<IMixinTypeProvider>(),
          SafeServiceLocator.Current.GetInstance<ITargetTypeModifier>(),
          SafeServiceLocator.Current.GetInstance<IConcreteTypeMetadataImporter>());
      var restoreParticipant = new DomainObjectParticipant(
          SafeServiceLocator.Current.GetInstance<IClassDefinitionProvider>(),
          SafeServiceLocator.Current.GetInstance<IInterceptedPropertyFinder>());
      _pipeline = SafeServiceLocator.Current.GetInstance<IPipelineFactory>().Create("DomainObjectCreatorTest", remixParticipant, restoreParticipant);
      var pipelineRegistry = new DefaultPipelineRegistry(_pipeline);
      _domainObjectCreator = new DomainObjectCreator(pipelineRegistry);

      _transaction = ClientTransaction.CreateRootTransaction();
      _order1InitializationContext = CreateFakeInitializationContext(DomainObjectIDs.Order1, _transaction);
      var objectID = new ObjectID(typeof(TargetClassForPersistentMixin), Guid.NewGuid());
      _targetClassForPersistentMixinInitializationContext = CreateFakeInitializationContext(objectID, _transaction);
    }

    [Test]
    public void CreateObjectReference ()
    {
      var order = _domainObjectCreator.CreateObjectReference(_order1InitializationContext, _transaction);

      Assert.That(order, Is.InstanceOf(typeof(Order)));
      Assert.That(order.ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(order.RootTransaction, Is.EqualTo(_transaction));
    }

    [Test]
    public void CreateObjectReference_UsesFactoryGeneratedType ()
    {
      var order = _domainObjectCreator.CreateObjectReference(_order1InitializationContext, _transaction);

      Assert.That(_pipeline.ReflectionService.IsAssembledType(((object)order).GetType()), Is.True);
    }

    [Test]
    public void CreateObjectReference_CallsNoCtor ()
    {
      var order = (Order)_domainObjectCreator.CreateObjectReference(_order1InitializationContext, _transaction);

      Assert.That(order.CtorCalled, Is.False);
    }

    [Test]
    public void CreateObjectReference_PreparesMixins ()
    {
      var instance = _domainObjectCreator.CreateObjectReference(_targetClassForPersistentMixinInitializationContext, _transaction);

      Assert.That(Mixin.Get<MixinAddingPersistentProperties>(instance), Is.Not.Null);
    }

    [Test]
    public void CreateObjectReference_InitializesObjectID ()
    {
      var instance = _domainObjectCreator.CreateObjectReference(_order1InitializationContext, _transaction);
      Assert.That(instance.ID, Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void CreateObjectReference_EnlistsObjectInTransaction ()
    {
      DomainObject registeredObject = null;

      var initializationContextMock = new Mock<IObjectInitializationContext>(MockBehavior.Strict);
      initializationContextMock.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      initializationContextMock.Setup(stub => stub.RootTransaction).Returns(_transaction);
      initializationContextMock
          .Setup(mock => mock.RegisterObject(It.Is<DomainObject>(obj => obj.ID == DomainObjectIDs.Order1)))
          .Callback((DomainObject domainObject) => registeredObject = domainObject)
          .Verifiable();

      var instance = _domainObjectCreator.CreateObjectReference(initializationContextMock.Object, _transaction);

      initializationContextMock.Verify();
      Assert.That(instance, Is.SameAs(registeredObject));
    }

    [Test]
    public void CreateObjectReference_ValidatesMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        Assert.That(
            () => _domainObjectCreator.CreateObjectReference(_targetClassForPersistentMixinInitializationContext, _transaction),
            Throws.InstanceOf<MappingException>()
                .With.Message.Contains("mixin"));
      }
    }

    [Test]
    public void CreateObjectReference_CallsReferenceInitializing ()
    {
      var domainObject = (Order)_domainObjectCreator.CreateObjectReference(_order1InitializationContext, _transaction);
      Assert.That(domainObject.OnReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void CreateObjectReference_CallsReferenceInitializing_InRightTransaction ()
    {
      var domainObject = (Order)_domainObjectCreator.CreateObjectReference(_order1InitializationContext, _transaction);
      Assert.That(domainObject.OnReferenceInitializingTx, Is.SameAs(_transaction));
    }

    [Test]
    public void CreateObjectReference_CallsReferenceInitializing_InRightTransaction_WithActivatedInactiveTransaction ()
    {
      using (ClientTransactionTestHelper.MakeInactive(_transaction))
      {
        var domainObject = (Order)_domainObjectCreator.CreateObjectReference(_order1InitializationContext, _transaction);
        Assert.That(domainObject.OnReferenceInitializingTx, Is.SameAs(_transaction));
        Assert.That(domainObject.OnReferenceInitializingActiveTx, Is.SameAs(_transaction));
      }
    }

    [Test]
    public void CreateNewObject ()
    {
      var initializationContext = CreateNewObjectInitializationContext(DomainObjectIDs.OrderItem1, _transaction);
      var result = _domainObjectCreator.CreateNewObject(initializationContext, ParamList.Create("A product"), _transaction);

      Assert.That(_pipeline.ReflectionService.IsAssembledType(((object)result).GetType()), Is.True);
      Assert.That(result, Is.AssignableTo<OrderItem>());
      Assert.That(result.ID, Is.EqualTo(DomainObjectIDs.OrderItem1));
      Assert.That(result.RootTransaction, Is.SameAs(_transaction));
      Assert.That(_transaction.IsDiscarded, Is.False);
      Assert.That(_transaction.IsEnlisted(result), Is.True);
      Assert.That(_transaction.ExecuteInScope(() => ((OrderItem)result).Product), Is.EqualTo("A product"));
      Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Null);
    }

    [Test]
    public void CreateNewObject_ValidatesMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        Assert.That(
            () => _domainObjectCreator.CreateNewObject(_targetClassForPersistentMixinInitializationContext, ParamList.Empty, _transaction),
            Throws.InstanceOf<MappingException>()
                .With.Message.Contains("mixin"));
      }
    }

    [Test]
    public void CreateNewObject_InitializesMixins ()
    {
      var initializationContext = CreateNewObjectInitializationContext(DomainObjectIDs.ClassWithAllDataTypes1, _transaction);

      var result = _domainObjectCreator.CreateNewObject(initializationContext, ParamList.Empty, _transaction);

      var mixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>>(result);
      Assert.That(mixin, Is.Not.Null);
      Assert.That(mixin.OnDomainObjectCreatedCalled, Is.True);
      Assert.That(mixin.OnDomainObjectCreatedTx, Is.SameAs(_transaction));
    }

    [Test]
    public void CreateNewObject_AllowsPublicAndNonPublicCtors ()
    {
      Assert.That(
          () =>
          _domainObjectCreator.CreateNewObject(
              CreateNewObjectInitializationContext(DomainObjectIDs.OrderItem1, _transaction), ParamList.Empty, _transaction),
          Is.Not.Null);

      Assert.That(
          () =>
          _domainObjectCreator.CreateNewObject(
              CreateNewObjectInitializationContext(DomainObjectIDs.OrderItem2, _transaction), ParamList.Empty, _transaction),
          Is.Not.Null);
    }

    private IObjectInitializationContext CreateFakeInitializationContext (ObjectID objectID, ClientTransaction rootTransaction)
    {
      var initializationContextStub = new Mock<IObjectInitializationContext>();

      initializationContextStub.Setup(stub => stub.ObjectID).Returns(objectID);
      initializationContextStub.Setup(stub => stub.RootTransaction).Returns(rootTransaction);
      return initializationContextStub.Object;
    }

    private NewObjectInitializationContext CreateNewObjectInitializationContext (ObjectID objectID, ClientTransaction rootTransaction)
    {
      return new NewObjectInitializationContext(
          objectID,
          rootTransaction,
          ClientTransactionTestHelper.GetEnlistedDomainObjectManager(rootTransaction),
          ClientTransactionTestHelper.GetIDataManager(rootTransaction));
    }

    [DBTable]
    public class DomainObjectWithPublicCtor : DomainObject
    {
      public DomainObjectWithPublicCtor ()
      {
      }
    }

    [DBTable]
    public class DomainObjectWithProtectedCtor : DomainObject
    {
      protected DomainObjectWithProtectedCtor ()
      {
      }
    }
  }
}
