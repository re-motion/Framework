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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class SortedPropertyComparerTest : StandardMappingTest
  {
    private Mock<IDataContainerMapReadOnlyView> _dataContainerMapStub;
    private PropertyDefinition _orderNumberPropertyDefinition;
    private PropertyDefinition _officialPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _dataContainerMapStub = new Mock<IDataContainerMapReadOnlyView>();
      _orderNumberPropertyDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryPropertyDefinition(typeof(Order).FullName + ".OrderNumber");
      _officialPropertyDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryPropertyDefinition(typeof(Order).FullName + ".Official");
    }

    [Test]
    public void CreateCompoundComparer ()
    {
      var specification1 = new SortedPropertySpecification(_orderNumberPropertyDefinition, SortOrder.Ascending);
      var specification2 = new SortedPropertySpecification(_orderNumberPropertyDefinition, SortOrder.Descending);

      var compoundComparer = (CompoundComparer<IDomainObject>)SortedPropertyComparer.CreateCompoundComparer(
          new[] { specification1, specification2 },
          _dataContainerMapStub.Object,
          ValueAccess.Current);
      Assert.That(compoundComparer.Comparers.Count, Is.EqualTo(2));

      Assert.That(((SortedPropertyComparer)compoundComparer.Comparers[0]).SortedPropertySpecification, Is.SameAs(specification1));
      Assert.That(((SortedPropertyComparer)compoundComparer.Comparers[0]).DataContainerMap, Is.SameAs(_dataContainerMapStub.Object));

      Assert.That(((SortedPropertyComparer)compoundComparer.Comparers[1]).SortedPropertySpecification, Is.SameAs(specification2));
      Assert.That(((SortedPropertyComparer)compoundComparer.Comparers[1]).DataContainerMap, Is.SameAs(_dataContainerMapStub.Object));
    }

    [Test]
    public void Compare_Ascending ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Order>();

      PrepareDataContainer(_dataContainerMapStub, domainObject1, 1);
      PrepareDataContainer(_dataContainerMapStub, domainObject2, 2);

      var specification = new SortedPropertySpecification(_orderNumberPropertyDefinition, SortOrder.Ascending);
      var comparer = new SortedPropertyComparer(specification, _dataContainerMapStub.Object, ValueAccess.Current);

      Assert.That(comparer.Compare(domainObject1, domainObject1), Is.EqualTo(0));
      Assert.That(comparer.Compare(domainObject1, domainObject2), Is.EqualTo(-1));
      Assert.That(comparer.Compare(domainObject2, domainObject1), Is.EqualTo(1));
      Assert.That(comparer.Compare(domainObject2, domainObject2), Is.EqualTo(0));
    }

    [Test]
    public void Compare_Descending ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Order>();

      PrepareDataContainer(_dataContainerMapStub, domainObject1, 1);
      PrepareDataContainer(_dataContainerMapStub, domainObject2, 2);

      var specification = new SortedPropertySpecification(_orderNumberPropertyDefinition, SortOrder.Descending);
      var comparer = new SortedPropertyComparer(specification, _dataContainerMapStub.Object, ValueAccess.Current);

      Assert.That(comparer.Compare(domainObject1, domainObject1), Is.EqualTo(0));
      Assert.That(comparer.Compare(domainObject1, domainObject2), Is.EqualTo(1));
      Assert.That(comparer.Compare(domainObject2, domainObject1), Is.EqualTo(-1));
      Assert.That(comparer.Compare(domainObject2, domainObject2), Is.EqualTo(0));
    }

    [Test]
    public void Compare_Ascending_Null ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();

      PrepareDataContainer(_dataContainerMapStub, domainObject1, 1);

      var specification = new SortedPropertySpecification(_officialPropertyDefinition, SortOrder.Ascending);
      var comparer = new SortedPropertyComparer(specification, _dataContainerMapStub.Object, ValueAccess.Current);

      Assert.That(comparer.Compare(domainObject1, domainObject1), Is.EqualTo(0));
      Assert.That(comparer.Compare(domainObject1, null), Is.EqualTo(-1));
      Assert.That(comparer.Compare(null, domainObject1), Is.EqualTo(1));
      Assert.That(comparer.Compare(null, null), Is.EqualTo(0));
    }

    [Test]
    public void Compare_Descending_Null ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();

      PrepareDataContainer(_dataContainerMapStub, domainObject1, 1);

      var specification = new SortedPropertySpecification(_officialPropertyDefinition, SortOrder.Descending);
      var comparer = new SortedPropertyComparer(specification, _dataContainerMapStub.Object, ValueAccess.Current);

      Assert.That(comparer.Compare(domainObject1, domainObject1), Is.EqualTo(0));
      Assert.That(comparer.Compare(domainObject1, null), Is.EqualTo(1));
      Assert.That(comparer.Compare(null, domainObject1), Is.EqualTo(-1));
      Assert.That(comparer.Compare(null, null), Is.EqualTo(0));
    }

    [Test]
    public void Compare_DoesNotTriggerEvents ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Order>();

      var dataContainer1 = PrepareDataContainer(_dataContainerMapStub, domainObject1, 1);
      var dataContainer2 = PrepareDataContainer(_dataContainerMapStub, domainObject2, 2);

      var transaction = ClientTransaction.CreateRootTransaction();
      ClientTransactionTestHelper.RegisterDataContainer(transaction, dataContainer1);
      ClientTransactionTestHelper.RegisterDataContainer(transaction, dataContainer2);

      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(transaction);

      var specification = new SortedPropertySpecification(_orderNumberPropertyDefinition, SortOrder.Descending);
      var comparer = new SortedPropertyComparer(specification, _dataContainerMapStub.Object, ValueAccess.Current);

      Assert.That(comparer.Compare(domainObject1, domainObject1), Is.EqualTo(0));
    }

    [Test]
    public void Compare_NonExistingProperty ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Customer>();

      PrepareDataContainer(_dataContainerMapStub, domainObject1, 1);
      PrepareDataContainer(_dataContainerMapStub, domainObject2);

      var specification = new SortedPropertySpecification(_orderNumberPropertyDefinition, SortOrder.Descending);
      var comparer = new SortedPropertyComparer(specification, _dataContainerMapStub.Object, ValueAccess.Current);

      Assert.That(comparer.Compare(domainObject1, domainObject1), Is.EqualTo(0));
      Assert.That(comparer.Compare(domainObject1, domainObject2), Is.EqualTo(-1));
      Assert.That(comparer.Compare(domainObject2, domainObject1), Is.EqualTo(1));
      Assert.That(comparer.Compare(domainObject2, domainObject2), Is.EqualTo(0));
    }

    [Test]
    public void Compare_PropertyOnBaseType ()
    {
      var propertyDefinition = DomainObjectIDs.Company1.ClassDefinition.GetMandatoryPropertyDefinition(typeof(Company).FullName + ".Name");

      var domainObject1 = DomainObjectMother.CreateFakeObject<Company>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Customer>();

      var dataContainer1 = PrepareDataContainer(_dataContainerMapStub, domainObject1);
      dataContainer1.SetValue(propertyDefinition, "A");

      var dataContainer2 = PrepareDataContainer(_dataContainerMapStub, domainObject2);
      dataContainer2.SetValue(propertyDefinition, "B");

      var specification = new SortedPropertySpecification(propertyDefinition, SortOrder.Ascending);
      var comparer = new SortedPropertyComparer(specification, _dataContainerMapStub.Object, ValueAccess.Current);

      Assert.That(comparer.Compare(domainObject1, domainObject1), Is.EqualTo(0));
      Assert.That(comparer.Compare(domainObject1, domainObject2), Is.EqualTo(-1));
      Assert.That(comparer.Compare(domainObject2, domainObject1), Is.EqualTo(1));
      Assert.That(comparer.Compare(domainObject2, domainObject2), Is.EqualTo(0));
    }

    [Test]
    public void Compare_PropertyOnMixinType ()
    {
      var propertyDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(ClassWithMixedProperty))
          .GetMandatoryPropertyDefinition(typeof(MixinAddingProperty).FullName + ".MixedProperty");

      var domainObject1 = DomainObjectMother.CreateFakeObject<ClassWithMixedProperty>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<ClassWithMixedProperty>();

      var dataContainer1 = PrepareDataContainer(_dataContainerMapStub, domainObject1);
      dataContainer1.SetValue(propertyDefinition, "A");

      var dataContainer2 = PrepareDataContainer(_dataContainerMapStub, domainObject2);
      dataContainer2.SetValue(propertyDefinition, "B");

      var specification = new SortedPropertySpecification(propertyDefinition, SortOrder.Ascending);
      var comparer = new SortedPropertyComparer(specification, _dataContainerMapStub.Object, ValueAccess.Current);

      Assert.That(comparer.Compare(domainObject1, domainObject1), Is.EqualTo(0));
      Assert.That(comparer.Compare(domainObject1, domainObject2), Is.EqualTo(-1));
      Assert.That(comparer.Compare(domainObject2, domainObject1), Is.EqualTo(1));
      Assert.That(comparer.Compare(domainObject2, domainObject2), Is.EqualTo(0));
    }

    [Test]
    public void Compare_UnloadedObject ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var unloadedDomainObject = DomainObjectMother.GetObjectReference<Order>(
          ClientTransaction.CreateRootTransaction(),
          new ObjectID(typeof(Order), Guid.NewGuid()));

      PrepareDataContainer(_dataContainerMapStub, domainObject, 1);

      var specification = new SortedPropertySpecification(_orderNumberPropertyDefinition, SortOrder.Descending);
      var comparer = new SortedPropertyComparer(specification, _dataContainerMapStub.Object, ValueAccess.Current);

      Assert.That(comparer.Compare(domainObject, domainObject), Is.EqualTo(0));
      Assert.That(comparer.Compare(domainObject, unloadedDomainObject), Is.EqualTo(-1));
      Assert.That(comparer.Compare(unloadedDomainObject, domainObject), Is.EqualTo(1));
      Assert.That(comparer.Compare(unloadedDomainObject, unloadedDomainObject), Is.EqualTo(0));
    }

    [Test]
    public void Compare_OriginalValues ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Order>();

      var dataContainer1 = PrepareDataContainer(_dataContainerMapStub, domainObject1, 1);
      dataContainer1.CommitState();
      SetOrderNumber(dataContainer1, 3);
      PrepareDataContainer(_dataContainerMapStub, domainObject2, 2);

      var specification = new SortedPropertySpecification(_orderNumberPropertyDefinition, SortOrder.Ascending);
      var comparer = new SortedPropertyComparer(specification, _dataContainerMapStub.Object, ValueAccess.Original);

      Assert.That(comparer.Compare(domainObject1, domainObject1), Is.EqualTo(0));
      Assert.That(comparer.Compare(domainObject1, domainObject2), Is.EqualTo(1));
      Assert.That(comparer.Compare(domainObject2, domainObject1), Is.EqualTo(-1));
      Assert.That(comparer.Compare(domainObject2, domainObject2), Is.EqualTo(0));
    }

    private DataContainer PrepareDataContainer (Mock<IDataContainerMapReadOnlyView> dataContainerMapStub, Order domainObject, int orderNumber)
    {
      var dataContainer = PrepareDataContainer(dataContainerMapStub, domainObject);
      SetOrderNumber(dataContainer, orderNumber);
      return dataContainer;
    }

    private DataContainer PrepareDataContainer (Mock<IDataContainerMapReadOnlyView> dataContainerMapStub, DomainObject domainObject)
    {
      var dataContainer = DataContainer.CreateNew(domainObject.ID);
      dataContainerMapStub.Setup(stub => stub[domainObject.ID]).Returns(dataContainer);
      return dataContainer;
    }

    private void SetOrderNumber (DataContainer dataContainer, int orderNumber)
    {
      dataContainer.SetValue(_orderNumberPropertyDefinition, orderNumber);
    }
  }
}
