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
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Queries;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class SearchTest : ObjectBindingTestBase
  {
    private SearchServiceTestHelper _searchServiceTestHelper;
    private string _stubbedQueryID;

    private IBusinessObject _referencingBusinessObject;
    private IBusinessObjectReferenceProperty _property;

    private ClientTransaction _clientTransaction;
    private ClientTransactionScope _transactionScope;

    public override void SetUp ()
    {
      base.SetUp();

      _searchServiceTestHelper = new SearchServiceTestHelper();
      _stubbedQueryID = "FakeQuery";

      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (
          typeof (ISearchAvailableObjectsService), new BindableDomainObjectCompoundSearchService());

      _clientTransaction = _searchServiceTestHelper.CreateStubbableTransaction<ClientTransaction>();
      _transactionScope = _clientTransaction.EnterDiscardingScope();

      var fakeResultData = _searchServiceTestHelper.CreateFakeResultData (_clientTransaction);
      _searchServiceTestHelper.StubQueryResult (_stubbedQueryID, new[] { fakeResultData });

      var referencingObject = SampleBindableMixinDomainObject.NewObject();
      _referencingBusinessObject = (IBusinessObject) referencingObject;
      _property = (IBusinessObjectReferenceProperty) _referencingBusinessObject.BusinessObjectClass.GetPropertyDefinition ("Relation");
    }

    public override void TearDown ()
    {
      _transactionScope.Leave();
      base.TearDown();
    }

    [Test]
    public void SearchViaReferencePropertyWithIdentity ()
    {
      Assert.That (_property.SupportsSearchAvailableObjects, Is.True);
      var results =
          (IBusinessObjectWithIdentity[]) _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (_stubbedQueryID));
      Assert.That (
          results,
          Is.EqualTo (ClientTransaction.Current.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration (_stubbedQueryID)).ToArray()));
    }

    [Test]
    public void SearchViaReferencePropertyWithoutIdentity ()
    {
      Assert.That (_property.SupportsSearchAvailableObjects, Is.True);
      IBusinessObject[] results = _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (_stubbedQueryID));
      Assert.That (
          results,
          Is.EqualTo (ClientTransaction.Current.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration (_stubbedQueryID)).ToArray()));
    }

    [Test]
    public void SearchAvailableObjectsUsesAssociatedTransaction ()
    {
      var transaction = _searchServiceTestHelper.CreateTransactionWithStubbedQuery<ClientTransaction> (_stubbedQueryID);

      IBusinessObject boundObject = (IBusinessObject) transaction.ExecuteInScope (() => SampleBindableMixinDomainObject.NewObject());

      IBusinessObject[] results = _property.SearchAvailableObjects (boundObject, new DefaultSearchArguments (_stubbedQueryID));
      Assert.That (results, Is.Not.Null);
      Assert.That (results.Length > 0, Is.True);

      var resultDomainObject = (DomainObject) results[0];
      Assert.That (ClientTransaction.Current.IsEnlisted (resultDomainObject), Is.False);
      Assert.That (transaction.IsEnlisted (resultDomainObject), Is.True);
    }

    [Test]
    public void SearchAvailableObjectsWithDifferentObject ()
    {
      IBusinessObject[] businessObjects =
          _property.SearchAvailableObjects (SampleBindableDomainObject.NewObject(), new DefaultSearchArguments (_stubbedQueryID));

      Assert.That (businessObjects, Is.Not.Null);
      Assert.That (businessObjects.Length > 0, Is.True);
    }

    [Test]
    public void SearchAvailableObjectsWithNullSearchArguments ()
    {
      var fakeResultData = _searchServiceTestHelper.CreateFakeResultData (_clientTransaction);
      _searchServiceTestHelper.StubSearchAllObjectsQueryResult (typeof (OppositeBidirectionalBindableDomainObject), new[] { fakeResultData });

      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_referencingBusinessObject, null);

      Assert.That (businessObjects, Is.Not.Null);
      Assert.That (((DomainObject) businessObjects[0]).ID, Is.EqualTo (fakeResultData.ObjectID));
      Assert.That (((DomainObject) businessObjects[0]).RootTransaction, Is.SameAs (_clientTransaction));
    }

    [Test]
    public void SearchAvailableObjectsWithNullQuery ()
    {
      var fakeResultData = _searchServiceTestHelper.CreateFakeResultData (_clientTransaction);
      _searchServiceTestHelper.StubSearchAllObjectsQueryResult (typeof (OppositeBidirectionalBindableDomainObject), new[] { fakeResultData });

      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (null));

      Assert.That (businessObjects, Is.Not.Null);
      Assert.That (((DomainObject) businessObjects[0]).ID, Is.EqualTo (fakeResultData.ObjectID));
    }

    [Test]
    public void SearchAvailableObjectsWithEmptyQuery ()
    {
      var fakeResultData = _searchServiceTestHelper.CreateFakeResultData (_clientTransaction);
      _searchServiceTestHelper.StubSearchAllObjectsQueryResult (typeof (OppositeBidirectionalBindableDomainObject), new[] { fakeResultData });

      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (""));

      Assert.That (businessObjects, Is.Not.Null);
      Assert.That (((DomainObject) businessObjects[0]).ID, Is.EqualTo (fakeResultData.ObjectID));
    }
  }
}