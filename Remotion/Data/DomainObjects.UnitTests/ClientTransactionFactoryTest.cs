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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ClientTransactionFactoryTest
  {
    [Test]
    public void CreateRootTransaction ()
    {
      ITransactionFactory transactionFactory = new ClientTransactionFactory();

      ITransaction transaction = transactionFactory.CreateRootTransaction();
      Assert.That(transaction, Is.InstanceOf(typeof(ClientTransactionWrapper)));
      Assert.That(transaction.To<ClientTransaction>(), Is.InstanceOf(typeof(ClientTransaction)));

      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy(transaction.To<ClientTransaction>());
      Assert.That(persistenceStrategy, Is.InstanceOf(typeof(RootPersistenceStrategy)));
    }

    [Test]
    public void CreateRootTransaction_WithExtension ()
    {
      var factory = new Mock<ClientTransactionFactory>();

      var extensionStub = new Mock<IClientTransactionExtension>();
      extensionStub.Setup(stub => stub.Key).Returns("extension");

      factory
          .Protected()
          .Setup("OnTransactionCreated", true, ItExpr.Is<ClientTransaction>(obj => obj != null))
          .Callback((ClientTransaction transaction) => transaction.Extensions.Add(extensionStub.Object))
          .Verifiable();

      ITransaction transaction = ((ITransactionFactory)factory.Object).CreateRootTransaction();

      var clientTransaction = transaction.To<ClientTransaction>();
      Assert.That(clientTransaction.Extensions, Has.Member(extensionStub.Object));
    }

    [Test]
    public void CreateChildTransaction_WithExtension ()
    {
      var factory = new Mock<ClientTransactionFactory>();

      var extensionStub = new Mock<IClientTransactionExtension>();
      extensionStub.Setup(stub => stub.Key).Returns("extension");

      factory
          .Protected()
          .Setup("OnTransactionCreated", true, ItExpr.Is<ClientTransaction>(obj => obj != null))
          .Callback((ClientTransaction transaction) => transaction.Extensions.Add(extensionStub.Object))
          .Verifiable();

      ITransaction rootTransaction = ((ITransactionFactory)factory.Object).CreateRootTransaction();
      ITransaction childTransaction = rootTransaction.CreateChild();

      Assert.That(rootTransaction.To<ClientTransaction>().Extensions, Has.Member(extensionStub.Object));
      Assert.That(childTransaction.To<ClientTransaction>().Extensions, Has.No.Member(extensionStub.Object));
    }
  }
}
