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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionCommitRollbackTest : ClientTransactionBaseTest
  {
    private ClientTransaction _subTransaction;

    public override void SetUp ()
    {
      base.SetUp();
      _subTransaction = TestableClientTransaction.CreateSubTransaction();
    }

    [Test]
    public void DiscardMakesParentWriteable ()
    {
      Assert.That(_subTransaction.ParentTransaction.IsWriteable, Is.False);
      Assert.That(_subTransaction.IsDiscarded, Is.False);
      _subTransaction.Discard();
      Assert.That(_subTransaction.IsDiscarded, Is.True);
      Assert.That(_subTransaction.ParentTransaction.IsWriteable, Is.True);
    }

    [Test]
    public void DiscardRendersSubTransactionUnusable ()
    {
      _subTransaction.Discard();
      using (_subTransaction.EnterNonDiscardingScope())
      {
        Assert.That(
            () => Order.NewObject(),
            Throws.InvalidOperationException
                .With.Message.EqualTo("The transaction can no longer be used because it has been discarded."));
      }
    }

    [Test]
    public void SubTransactionCanContinueToBeUsedAfterRollback ()
    {
      _subTransaction.Rollback();
      Assert.That(_subTransaction.IsDiscarded, Is.False);
      using (_subTransaction.EnterDiscardingScope())
      {
        Order order = Order.NewObject();
        Assert.That(order, Is.Not.Null);
      }
    }

    [Test]
    public void SubTransactionCanContinueToBeUsedAfterCommit ()
    {
      _subTransaction.Commit();
      Assert.That(_subTransaction.IsDiscarded, Is.False);
      using (_subTransaction.EnterDiscardingScope())
      {
        Order order = Order.NewObject();
        Assert.That(order, Is.Not.Null);
      }
    }

    [Test]
    public void RollbackResetsNewedObjects ()
    {
      using (_subTransaction.EnterDiscardingScope())
      {
        Order order = Order.NewObject();
        _subTransaction.Rollback();
        Assert.That(
            () => order.OrderNumber,
            Throws.InstanceOf<ObjectInvalidException>()
                .With.Message.Matches("Object 'Order.*' is invalid in this transaction."));
      }
    }

    [Test]
    public void RollbackResetsLoadedObjects ()
    {
      using (_subTransaction.EnterDiscardingScope())
      {
        Order order = DomainObjectIDs.Order1.GetObject<Order>();
        order.OrderNumber = 5;

        _subTransaction.Rollback();

        Assert.That(order.OrderNumber, Is.Not.EqualTo(5));
      }
    }

    [Test]
    public void SubRollbackDoesNotRollbackParent ()
    {
      _subTransaction.Discard();
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(order.OrderNumber, Is.EqualTo(1));
      order.OrderNumber = 3;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        order.OrderNumber = 5;
        ClientTransactionScope.CurrentTransaction.Rollback();
        Assert.That(order.OrderNumber, Is.EqualTo(3));
      }

      Assert.That(order.OrderNumber, Is.EqualTo(3));
      TestableClientTransaction.Rollback();
      Assert.That(order.OrderNumber, Is.EqualTo(1));
    }


    [Test]
    public void ParentTransactionStillReadOnlyAfterCommit ()
    {
      using (_subTransaction.EnterDiscardingScope())
      {
        Assert.That(TestableClientTransaction.IsWriteable, Is.False);
        ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
        Assert.That(classWithAllDataTypes.Int32Property, Is.Not.EqualTo(7));
        classWithAllDataTypes.Int32Property = 7;
        _subTransaction.Commit();
        Assert.That(TestableClientTransaction.IsWriteable, Is.False);
      }
    }

    [Test]
    public void CommitPropagatesNewObjectsToParentTransaction ()
    {
      ClassWithAllDataTypes classWithAllDataTypes;
      using (_subTransaction.EnterDiscardingScope())
      {
        classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
        Assert.That(classWithAllDataTypes.Int32Property, Is.Not.EqualTo(7));
        classWithAllDataTypes.Int32Property = 7;
        _subTransaction.Commit();
        Assert.That(classWithAllDataTypes.Int32Property, Is.EqualTo(7));
      }

      Assert.That(classWithAllDataTypes, Is.Not.Null);
      Assert.That(classWithAllDataTypes.Int32Property, Is.EqualTo(7));
    }

    [Test]
    public void CommitPropagatesChangedObjectsToParentTransaction ()
    {
      Order order;
      using (_subTransaction.EnterDiscardingScope())
      {
        order = DomainObjectIDs.Order1.GetObject<Order>();
        order.OrderNumber = 5;

        _subTransaction.Commit();

        Assert.That(order.OrderNumber, Is.EqualTo(5));
      }

      Assert.That(order, Is.Not.Null);
      Assert.That(order.OrderNumber, Is.EqualTo(5));
    }

    [Test]
    public void SubCommitDoesNotCommitParent ()
    {
      _subTransaction.Discard();
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        order.OrderNumber = 5;
        ClientTransactionScope.CurrentTransaction.Commit();
      }

      Assert.That(order.OrderNumber, Is.EqualTo(5));
      TestableClientTransaction.Rollback();
      Assert.That(order.OrderNumber, Is.EqualTo(1));
    }

    [Test]
    public void SubCommit_OfDeletedObject_DoesNotRaiseDeletedEvent ()
    {
      using (_subTransaction.EnterDiscardingScope())
      {
        ClassWithAllDataTypes domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();

        var extensionMock = new Mock<IClientTransactionExtension>(MockBehavior.Strict);
        extensionMock.Setup(stub => stub.Key).Returns("Mock");
        _subTransaction.Extensions.Add(extensionMock.Object);
        try
        {
          extensionMock.Reset();

          extensionMock.Setup(_ => _.ObjectDeleting(_subTransaction, domainObject)).Verifiable();
          extensionMock.Setup(_ => _.ObjectDeleted(_subTransaction, domainObject)).Verifiable();

          domainObject.Delete();
          extensionMock.Verify();

          extensionMock.Reset();
          extensionMock
              .Setup(_ => _.Committing(It.IsAny<ClientTransaction>(), It.IsAny<IReadOnlyList<IDomainObject>>(), It.IsAny<ICommittingEventRegistrar>()))
              .Verifiable();
          extensionMock.Setup(_ => _.CommitValidate(It.IsAny<ClientTransaction>(), It.IsAny<IReadOnlyList<PersistableData>>())).Verifiable();
          extensionMock.Setup(_ => _.Committed(It.IsAny<ClientTransaction>(), It.IsAny<IReadOnlyList<IDomainObject>>())).Verifiable();

          _subTransaction.Commit();
          extensionMock.Verify();
        }
        finally
        {
          _subTransaction.Extensions.Remove("Mock");
        }
      }
    }

    [Test]
    public void SubCommit_OfDeletedObject_DoesNotRaiseDeletedEvent_WithRelations ()
    {
      using (_subTransaction.EnterDiscardingScope())
      {
        Order domainObject = DomainObjectIDs.Order1.GetObject<Order>();
        domainObject.OrderItems[0].Delete();

        var extensionMock = new Mock<IClientTransactionExtension>(MockBehavior.Strict);
        extensionMock.Setup(stub => stub.Key).Returns("Mock");
        _subTransaction.Extensions.Add(extensionMock.Object);
        try
        {
          extensionMock.Reset();

          var sequence = new VerifiableSequence();

          extensionMock
              .InVerifiableSequence(sequence)
              .Setup(_ => _.Committing(It.IsAny<ClientTransaction>(), It.IsAny<IReadOnlyList<IDomainObject>>(), It.IsAny<ICommittingEventRegistrar>()))
              .Verifiable();

          extensionMock
              .InVerifiableSequence(sequence)
              .Setup(_ => _.CommitValidate(It.IsAny<ClientTransaction>(), It.IsAny<IReadOnlyList<PersistableData>>()))
              .Verifiable();

          extensionMock
              .InVerifiableSequence(sequence)
              .Setup(_ => _.Committed(It.IsAny<ClientTransaction>(), It.IsAny<IReadOnlyList<IDomainObject>>()))
              .Verifiable();

          _subTransaction.Commit();

          extensionMock.Verify();
        }
        finally
        {
          _subTransaction.Extensions.Remove("Mock");
        }
      }
    }
  }
}
