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
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.AllowedOperations
{
  [TestFixture]
  public class DiscardTest : ReadOnlyTransactionsTestBase
  {
    [Test]
    public void DiscardReadOnlyRootTransaction_IsAllowed ()
    {
      Assert.That (ReadOnlyRootTransaction.IsDiscarded, Is.False);
      Assert.That (ReadOnlyMiddleTransaction.IsDiscarded, Is.False);
      Assert.That (WriteableSubTransaction.IsDiscarded, Is.False);

      var extensionMock = CreateAndAddExtensionMock();
      using (extensionMock.GetMockRepository().Ordered())
      {
        extensionMock
            .Expect (mock => mock.TransactionDiscard (ReadOnlyRootTransaction))
            .WhenCalled (mi => CheckTransactionHierarchy ());
        extensionMock
            .Expect (mock => mock.TransactionDiscard (ReadOnlyMiddleTransaction))
            .WhenCalled (mi => CheckTransactionHierarchy ());
        extensionMock
            .Expect (mock => mock.TransactionDiscard (WriteableSubTransaction))
            .WhenCalled (mi => CheckTransactionHierarchy());
      }

      ReadOnlyRootTransaction.Discard();

      extensionMock.VerifyAllExpectations();
      Assert.That (ReadOnlyRootTransaction.IsDiscarded, Is.True);
      Assert.That (ReadOnlyMiddleTransaction.IsDiscarded, Is.True);
      Assert.That (WriteableSubTransaction.IsDiscarded, Is.True);
    }

    [Test]
    public void DiscardReadOnlyMiddleTransaction_IsAllowed ()
    {
      Assert.That (ReadOnlyRootTransaction.IsDiscarded, Is.False);
      Assert.That (ReadOnlyMiddleTransaction.IsDiscarded, Is.False);
      Assert.That (WriteableSubTransaction.IsDiscarded, Is.False);

      var extensionMock = CreateAndAddExtensionMock ();
      using (extensionMock.GetMockRepository ().Ordered ())
      {
        extensionMock
            .Expect (mock => mock.TransactionDiscard (ReadOnlyMiddleTransaction))
            .WhenCalled (mi => CheckTransactionHierarchy ());
        extensionMock
            .Expect (mock => mock.TransactionDiscard (WriteableSubTransaction))
            .WhenCalled (mi => CheckTransactionHierarchy ());
      }

      ReadOnlyMiddleTransaction.Discard ();

      Assert.That (ReadOnlyRootTransaction.IsDiscarded, Is.False);
      Assert.That (ReadOnlyMiddleTransaction.IsDiscarded, Is.True);
      Assert.That (WriteableSubTransaction.IsDiscarded, Is.True);

      Assert.That (ReadOnlyRootTransaction.SubTransaction, Is.Null);
      Assert.That (ReadOnlyRootTransaction.IsWriteable, Is.True);
    }

    private void CheckTransactionHierarchy ()
    {
      Assert.That (WriteableSubTransaction.ParentTransaction, Is.SameAs (ReadOnlyMiddleTransaction));
      Assert.That (ReadOnlyMiddleTransaction.SubTransaction, Is.SameAs (WriteableSubTransaction));
      Assert.That (ReadOnlyMiddleTransaction.ParentTransaction, Is.SameAs (ReadOnlyRootTransaction));
      Assert.That (ReadOnlyRootTransaction.SubTransaction, Is.SameAs (ReadOnlyMiddleTransaction));

      Assert.That (WriteableSubTransaction.IsDiscarded, Is.False);
      Assert.That (ReadOnlyMiddleTransaction.IsDiscarded, Is.False);
      Assert.That (ReadOnlyRootTransaction.IsDiscarded, Is.False);

      Assert.That (WriteableSubTransaction.IsWriteable, Is.True);
      Assert.That (ReadOnlyMiddleTransaction.IsWriteable, Is.False);
      Assert.That (ReadOnlyRootTransaction.IsWriteable, Is.False);
    }

    private IClientTransactionExtension CreateAndAddExtensionMock ()
    {
      var extensionMock = MockRepository.GenerateStrictMock<IClientTransactionExtension>();
      extensionMock.Stub (stub => stub.Key).Return ("Test");
      ReadOnlyRootTransaction.Extensions.Add (extensionMock);
      ReadOnlyMiddleTransaction.Extensions.Add (extensionMock);
      WriteableSubTransaction.Extensions.Add (extensionMock);
      return extensionMock;
    }
  }
}