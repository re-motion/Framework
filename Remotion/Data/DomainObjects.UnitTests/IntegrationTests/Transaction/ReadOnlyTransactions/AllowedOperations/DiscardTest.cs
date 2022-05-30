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

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.AllowedOperations
{
  [TestFixture]
  public class DiscardTest : ReadOnlyTransactionsTestBase
  {
    [Test]
    public void DiscardReadOnlyRootTransaction_IsAllowed ()
    {
      Assert.That(ReadOnlyRootTransaction.IsDiscarded, Is.False);
      Assert.That(ReadOnlyMiddleTransaction.IsDiscarded, Is.False);
      Assert.That(WriteableSubTransaction.IsDiscarded, Is.False);

      var extensionMock = CreateAndAddExtensionMock();
      var sequence = new MockSequence();
      extensionMock
          .InSequence(sequence)
          .Setup(mock => mock.TransactionDiscard(ReadOnlyRootTransaction))
          .Callback((ClientTransaction _) => CheckTransactionHierarchy())
          .Verifiable();
      extensionMock
          .InSequence(sequence)
          .Setup(mock => mock.TransactionDiscard(ReadOnlyMiddleTransaction))
          .Callback((ClientTransaction _) => CheckTransactionHierarchy())
          .Verifiable();
      extensionMock
          .InSequence(sequence)
          .Setup(mock => mock.TransactionDiscard(WriteableSubTransaction))
          .Callback((ClientTransaction _) => CheckTransactionHierarchy())
          .Verifiable();

      ReadOnlyRootTransaction.Discard();

      extensionMock.Verify();
      Assert.That(ReadOnlyRootTransaction.IsDiscarded, Is.True);
      Assert.That(ReadOnlyMiddleTransaction.IsDiscarded, Is.True);
      Assert.That(WriteableSubTransaction.IsDiscarded, Is.True);
    }

    [Test]
    public void DiscardReadOnlyMiddleTransaction_IsAllowed ()
    {
      Assert.That(ReadOnlyRootTransaction.IsDiscarded, Is.False);
      Assert.That(ReadOnlyMiddleTransaction.IsDiscarded, Is.False);
      Assert.That(WriteableSubTransaction.IsDiscarded, Is.False);

      var extensionMock = CreateAndAddExtensionMock();
      var sequence = new MockSequence();
      extensionMock
          .InSequence(sequence)
          .Setup(mock => mock.TransactionDiscard(ReadOnlyMiddleTransaction))
          .Callback((ClientTransaction _) => CheckTransactionHierarchy())
          .Verifiable();
      extensionMock
          .InSequence(sequence)
          .Setup(mock => mock.TransactionDiscard(WriteableSubTransaction))
          .Callback((ClientTransaction _) => CheckTransactionHierarchy())
          .Verifiable();

      ReadOnlyMiddleTransaction.Discard();

      Assert.That(ReadOnlyRootTransaction.IsDiscarded, Is.False);
      Assert.That(ReadOnlyMiddleTransaction.IsDiscarded, Is.True);
      Assert.That(WriteableSubTransaction.IsDiscarded, Is.True);

      Assert.That(ReadOnlyRootTransaction.SubTransaction, Is.Null);
      Assert.That(ReadOnlyRootTransaction.IsWriteable, Is.True);
    }

    private void CheckTransactionHierarchy ()
    {
      Assert.That(WriteableSubTransaction.ParentTransaction, Is.SameAs(ReadOnlyMiddleTransaction));
      Assert.That(ReadOnlyMiddleTransaction.SubTransaction, Is.SameAs(WriteableSubTransaction));
      Assert.That(ReadOnlyMiddleTransaction.ParentTransaction, Is.SameAs(ReadOnlyRootTransaction));
      Assert.That(ReadOnlyRootTransaction.SubTransaction, Is.SameAs(ReadOnlyMiddleTransaction));

      Assert.That(WriteableSubTransaction.IsDiscarded, Is.False);
      Assert.That(ReadOnlyMiddleTransaction.IsDiscarded, Is.False);
      Assert.That(ReadOnlyRootTransaction.IsDiscarded, Is.False);

      Assert.That(WriteableSubTransaction.IsWriteable, Is.True);
      Assert.That(ReadOnlyMiddleTransaction.IsWriteable, Is.False);
      Assert.That(ReadOnlyRootTransaction.IsWriteable, Is.False);
    }

    private Mock<IClientTransactionExtension> CreateAndAddExtensionMock ()
    {
      var extensionMock = new Mock<IClientTransactionExtension>(MockBehavior.Strict);
      extensionMock.Setup(stub => stub.Key).Returns("Test");
      ReadOnlyRootTransaction.Extensions.Add(extensionMock.Object);
      ReadOnlyMiddleTransaction.Extensions.Add(extensionMock.Object);
      WriteableSubTransaction.Extensions.Add(extensionMock.Object);
      return extensionMock;
    }
  }
}
