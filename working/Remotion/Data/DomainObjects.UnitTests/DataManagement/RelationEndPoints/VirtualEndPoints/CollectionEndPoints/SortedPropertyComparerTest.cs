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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class SortedPropertyComparerTest : StandardMappingTest
  {
    private IDataManager _dataManagerStub;
    private PropertyDefinition _orderNumberPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataManagerStub = MockRepository.GenerateStub<IDataManager> ();
      _orderNumberPropertyDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryPropertyDefinition (typeof (Order).FullName + ".OrderNumber");
    }

    [Test]
    public void CreateCompoundComparer ()
    {
      var specification1 = new SortedPropertySpecification (_orderNumberPropertyDefinition, SortOrder.Ascending);
      var specification2 = new SortedPropertySpecification (_orderNumberPropertyDefinition, SortOrder.Descending);

      var compoundComparer = (CompoundComparer<DomainObject>) SortedPropertyComparer.CreateCompoundComparer (new[] { specification1, specification2 }, _dataManagerStub);
      Assert.That (compoundComparer.Comparers.Count, Is.EqualTo (2));

      Assert.That (((SortedPropertyComparer) compoundComparer.Comparers[0]).SortedPropertySpecification, Is.SameAs (specification1));
      Assert.That (((SortedPropertyComparer) compoundComparer.Comparers[0]).DataManager, Is.SameAs (_dataManagerStub));

      Assert.That (((SortedPropertyComparer) compoundComparer.Comparers[1]).SortedPropertySpecification, Is.SameAs (specification2));
      Assert.That (((SortedPropertyComparer) compoundComparer.Comparers[1]).DataManager, Is.SameAs (_dataManagerStub));
    }

    [Test]
    public void Compare_Ascending ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();

      PrepareDataContainer (_dataManagerStub, domainObject1, 1);
      PrepareDataContainer (_dataManagerStub, domainObject2, 2);

      var specification = new SortedPropertySpecification (_orderNumberPropertyDefinition, SortOrder.Ascending);
      var comparer = new SortedPropertyComparer (specification, _dataManagerStub);

      Assert.That (comparer.Compare (domainObject1, domainObject1), Is.EqualTo (0));
      Assert.That (comparer.Compare (domainObject1, domainObject2), Is.EqualTo (-1));
      Assert.That (comparer.Compare (domainObject2, domainObject1), Is.EqualTo (1));
      Assert.That (comparer.Compare (domainObject2, domainObject2), Is.EqualTo (0));
    }

    [Test]
    public void Compare_Descending ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();

      PrepareDataContainer (_dataManagerStub, domainObject1, 1);
      PrepareDataContainer (_dataManagerStub, domainObject2, 2);

      var specification = new SortedPropertySpecification (_orderNumberPropertyDefinition, SortOrder.Descending);
      var comparer = new SortedPropertyComparer (specification, _dataManagerStub);

      Assert.That (comparer.Compare (domainObject1, domainObject1), Is.EqualTo (0));
      Assert.That (comparer.Compare (domainObject1, domainObject2), Is.EqualTo (1));
      Assert.That (comparer.Compare (domainObject2, domainObject1), Is.EqualTo (-1));
      Assert.That (comparer.Compare (domainObject2, domainObject2), Is.EqualTo (0));
    }

    [Test]
    public void Compare_DoesNotTriggerEvents ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();

      var dataContainer1 = PrepareDataContainer (_dataManagerStub, domainObject1, 1);
      var dataContainer2 = PrepareDataContainer (_dataManagerStub, domainObject2, 2);

      var transaction = ClientTransaction.CreateRootTransaction ();
      ClientTransactionTestHelper.RegisterDataContainer (transaction, dataContainer1);
      ClientTransactionTestHelper.RegisterDataContainer (transaction, dataContainer2);

      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents (transaction);

      var specification = new SortedPropertySpecification (_orderNumberPropertyDefinition, SortOrder.Descending);
      var comparer = new SortedPropertyComparer (specification, _dataManagerStub);

      Assert.That (comparer.Compare (domainObject1, domainObject1), Is.EqualTo (0));
    }

    [Test]
    public void Compare_NonExistingProperty ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Customer> ();

      PrepareDataContainer (_dataManagerStub, domainObject1, 1);
      PrepareDataContainer (_dataManagerStub, domainObject2);

      var specification = new SortedPropertySpecification (_orderNumberPropertyDefinition, SortOrder.Descending);
      var comparer = new SortedPropertyComparer (specification, _dataManagerStub);

      Assert.That (comparer.Compare (domainObject1, domainObject1), Is.EqualTo (0));
      Assert.That (comparer.Compare (domainObject1, domainObject2), Is.EqualTo (-1));
      Assert.That (comparer.Compare (domainObject2, domainObject1), Is.EqualTo (1));
      Assert.That (comparer.Compare (domainObject2, domainObject2), Is.EqualTo (0));
    }

    private DataContainer PrepareDataContainer (IDataManager dataManagerStub, Order domainObject, int orderNumber)
    {
      var dataContainer = PrepareDataContainer(dataManagerStub, domainObject);
      SetOrderNumber (dataContainer, orderNumber);
      return dataContainer;
    }

    private DataContainer PrepareDataContainer (IDataManager dataManagerStub, DomainObject domainObject)
    {
      var dataContainer = DataContainer.CreateNew (domainObject.ID);
      dataManagerStub.Stub (stub => stub.GetDataContainerWithLazyLoad (domainObject.ID, true)).Return (dataContainer);
      return dataContainer;
    }

    private void SetOrderNumber (DataContainer dataContainer, int orderNumber)
    {
      dataContainer.SetValue (_orderNumberPropertyDefinition, orderNumber);
    }
  }
}