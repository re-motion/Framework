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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Relations
{
  [TestFixture]
  public class DomainObjectCollectionsWithDifferentClientTransactionsTest : ClientTransactionBaseTest
  {
    private Customer _customer1;

    private ClientTransaction _secondClientTransaction;
    private DomainObjectCollection _secondCollection;
    private Customer _secondCustomer1;

    public override void SetUp ()
    {
      base.SetUp ();

      _customer1 = DomainObjectIDs.Customer1.GetObject<Customer> ();

      _secondClientTransaction = ClientTransaction.CreateRootTransaction();
      _secondCollection = new DomainObjectCollection ();
      using (_secondClientTransaction.EnterDiscardingScope ())
      {
        _secondCustomer1 = DomainObjectIDs.Customer1.GetObject<Customer> ();
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The collection already contains an object with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid'.")]
    public void Item_Set_WithDifferentClientTransaction ()
    {
      _secondCollection.Add (_secondCustomer1);

      _secondCollection[0] = _customer1;
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collection already contains an object with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid'.\r\n"
        + "Parameter name: domainObject")]
    public void Add_SameObject_WithDifferentClientTransaction ()
    {
      _secondCollection.Add (_secondCustomer1);

      _secondCollection.Add (_customer1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collection already contains an object with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid'.\r\n"
        + "Parameter name: domainObject")]
    public void Insert_SameObject_WithDifferentClientTransaction ()
    {
      _secondCollection.Add (_secondCustomer1);

      _secondCollection.Insert (0, _customer1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The object to be removed has the same ID as an object in this collection, but "
                                                                      + "is a different object reference.\r\nParameter name: domainObject")]
    public void Remove_ObjectFromOtherTransaction_WhoseIDIsInCollection ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer> ();
      collection.Add (customer);

      Customer customerInOtherTx;
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        customerInOtherTx = customer.ID.GetObject<Customer> ();
      }

      collection.Remove (customerInOtherTx);

      Assert.That (collection.ContainsObject (customer), Is.True);
    }

  }
}