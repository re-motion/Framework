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

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectQuerySearchServiceTest : ObjectBindingTestBase
  {
    private SearchServiceTestHelper _searchServiceTestHelper;
    private BindableDomainObjectQuerySearchService _service;

    private IBusinessObject _referencingBusinessObject;
    private IBusinessObjectReferenceProperty _property;

    private ClientTransactionScope _transactionScope;
    private string _stubbedQueryID;

    public override void SetUp ()
    {
      base.SetUp ();

      _searchServiceTestHelper = new SearchServiceTestHelper ();
      _service = new BindableDomainObjectQuerySearchService ();

      _stubbedQueryID = "FakeQuery";

      var transaction = _searchServiceTestHelper.CreateStubbableTransaction<ClientTransaction> ();
      _transactionScope = transaction.EnterDiscardingScope ();

      var fakeResultDataContainer = _searchServiceTestHelper.CreateFakeResultData (transaction);
      _searchServiceTestHelper.StubQueryResult (_stubbedQueryID, new[] { fakeResultDataContainer });

      var referencingObject = SampleBindableMixinDomainObject.NewObject ();
      _referencingBusinessObject = (IBusinessObject) referencingObject;
      _property = (IBusinessObjectReferenceProperty) _referencingBusinessObject.BusinessObjectClass.GetPropertyDefinition ("Relation");
    }

    public override void TearDown ()
    {
      _transactionScope.Leave ();
      base.TearDown ();
    }

    [Test]
    public void SearchViaReferencePropertyWithIdentity ()
    {
      Assert.That (_property.SupportsSearchAvailableObjects, Is.True);
      var results = (IBusinessObjectWithIdentity[]) _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (_stubbedQueryID));
      Assert.That (results, Is.EqualTo (ClientTransaction.Current.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration (_stubbedQueryID)).ToArray ()));
    }

    [Test]
    public void SearchViaReferencePropertyWithoutIdentity ()
    {
      Assert.That (_property.SupportsSearchAvailableObjects, Is.True);
      IBusinessObject[] results = _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (_stubbedQueryID));
      Assert.That (results, Is.EqualTo (ClientTransaction.Current.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration (_stubbedQueryID)).ToArray ()));
    }

    [Test]
    public void SearchAvailableObjectsUsesCurrentTransaction_NullObject ()
    {
      var outerTransaction = ClientTransaction.Current;

      var transaction = _searchServiceTestHelper.CreateTransactionWithStubbedQuery<ClientTransaction> (_stubbedQueryID);
      using (transaction.EnterDiscardingScope ())
      {
        IBusinessObject[] results = _property.SearchAvailableObjects (null, new DefaultSearchArguments (_stubbedQueryID));

        Assert.That (results, Is.Not.Null);
        Assert.That (results.Length > 0, Is.True);

        var resultDomainObject = (DomainObject) results[0];
        Assert.That (outerTransaction.IsEnlisted (resultDomainObject), Is.False);
        Assert.That (ClientTransaction.Current.IsEnlisted (resultDomainObject), Is.True);
      }
    }

    [Test]
    public void SearchAvailableObjectsUsesCurrentTransaction_NonDomainObject ()
    {
      var outerTransaction = ClientTransaction.Current;

      var transaction = _searchServiceTestHelper.CreateTransactionWithStubbedQuery<ClientTransaction> (_stubbedQueryID);
      using (transaction.EnterDiscardingScope ())
      {
        var nonDomainObject = new BindableNonDomainObjectReferencingDomainObject ();
        var property = (IBusinessObjectReferenceProperty) nonDomainObject.BusinessObjectClass.GetPropertyDefinition ("OppositeSampleMixinObject");
        IBusinessObject[] results = _service.Search (nonDomainObject, property, new DefaultSearchArguments (_stubbedQueryID));

        Assert.That (results, Is.Not.Null);
        Assert.That (results.Length > 0, Is.True);

        var resultDomainObject = (DomainObject) results[0];
        Assert.That (outerTransaction.IsEnlisted (resultDomainObject), Is.False);
        Assert.That (ClientTransaction.Current.IsEnlisted (resultDomainObject), Is.True);
      }
    }

    [Test]
    public void SearchAvailableObjectsUsesAssociatedTransaction ()
    {
      var transaction = _searchServiceTestHelper.CreateTransactionWithStubbedQuery<ClientTransaction> (_stubbedQueryID);

      IBusinessObject boundObject = (IBusinessObject) transaction.ExecuteInScope (() => SampleBindableMixinDomainObject.NewObject ());

      IBusinessObject[] results = _property.SearchAvailableObjects (boundObject, new DefaultSearchArguments (_stubbedQueryID));
      Assert.That (results, Is.Not.Null);
      Assert.That (results.Length > 0, Is.True);

      var resultDomainObject = (DomainObject) results[0];
      Assert.That (ClientTransaction.Current.IsEnlisted (resultDomainObject), Is.False);
      Assert.That (transaction.IsEnlisted (resultDomainObject), Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread or "
         + "the referencing object.")]
    public void SearchAvailableObjects_NoTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _service.Search (null, _property, new DefaultSearchArguments (_stubbedQueryID));
      }
    }

    [Test]
    public void SearchAvailableObjectsWithNullSearchArguments ()
    {
      IBusinessObject[] businessObjects = _service.Search (_referencingBusinessObject, _property, null);

      Assert.That (businessObjects, Is.Not.Null);
      Assert.That (businessObjects, Is.Empty);
    }

    [Test]
    public void SearchAvailableObjectsWithNullQuery ()
    {
      IBusinessObject[] businessObjects = _service.Search (_referencingBusinessObject, _property, new DefaultSearchArguments (null));

      Assert.That (businessObjects, Is.Not.Null);
      Assert.That (businessObjects, Is.Empty);
    }

    [Test]
    public void SearchAvailableObjectsWithEmptyQuery ()
    {
      IBusinessObject[] businessObjects = _service.Search (_referencingBusinessObject, _property, new DefaultSearchArguments (""));

      Assert.That (businessObjects, Is.Not.Null);
      Assert.That (businessObjects, Is.Empty);
    }
  }
}
