using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class DomainObjectTransactionContextImplementationTest : ClientTransactionBaseTest
  {
    [Test]
    public void CheckForDomainObjectReferenceInitializing_WhenReferenceInitializationIsActive_ThrowsInvalidOperationException ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();

      var transactionContextImplementation = new DomainObjectTransactionContextImplementation(order);

      Assert.That(() => transactionContextImplementation.CheckForDomainObjectReferenceInitializing(), Throws.Nothing);

      transactionContextImplementation.BeginDomainObjectReferenceInitializing();

      Assert.That(() => transactionContextImplementation.CheckForDomainObjectReferenceInitializing(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
    }

    [Test]
    public void CheckForDomainObjectReferenceInitializing_WhenReferenceInitializationIsComplete_DoesNotThrow ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var transactionContextImplementation = new DomainObjectTransactionContextImplementation(order);
      transactionContextImplementation.BeginDomainObjectReferenceInitializing();

      Assert.That(() => transactionContextImplementation.CheckForDomainObjectReferenceInitializing(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));

      transactionContextImplementation.EndDomainObjectReferenceInitializing();

      Assert.That(() => transactionContextImplementation.CheckForDomainObjectReferenceInitializing(), Throws.Nothing);

    }

    [Test]
    public void DomainObject_WhenReferenceInitializationIsActive_ReturnsValue ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var transactionContextImplementation = new DomainObjectTransactionContextImplementation(order);
      transactionContextImplementation.BeginDomainObjectReferenceInitializing();
      Assert.That(() => transactionContextImplementation.DomainObject, Is.SameAs(order));
    }

    [Test]
    public void IsDomainObjectReferenceInitializing_WhenReferenceInitializationIsActive_ReturnsTrue ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var transactionContextImplementation = new DomainObjectTransactionContextImplementation(order);
      transactionContextImplementation.BeginDomainObjectReferenceInitializing();
      Assert.That(() => transactionContextImplementation.IsDomainObjectReferenceInitializing, Is.True);
    }

    [Test]
    public void IsDomainObjectReferenceInitializing_WhenReferenceInitializationIsActive_ReturnsFalse ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var transactionContextImplementation = new DomainObjectTransactionContextImplementation(order);
      transactionContextImplementation.BeginDomainObjectReferenceInitializing();
      Assert.That(() => transactionContextImplementation.IsDomainObjectReferenceInitializing, Is.True);

      transactionContextImplementation.EndDomainObjectReferenceInitializing();
      Assert.That(() => transactionContextImplementation.IsDomainObjectReferenceInitializing, Is.False);
    }

    [Test]
    public void GetState_WhenReferenceInitializationIsActive_ThrowsInvalidOperationException ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var transactionContextImplementation = new DomainObjectTransactionContextImplementation(order);
      transactionContextImplementation.BeginDomainObjectReferenceInitializing();
      Assert.That(
          () => transactionContextImplementation.GetState(TestableClientTransaction),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
    }

    [Test]
    public void GetTimestamp_WhenReferenceInitializationIsActive_ThrowsInvalidOperationException ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var transactionContextImplementation = new DomainObjectTransactionContextImplementation(order);
      transactionContextImplementation.BeginDomainObjectReferenceInitializing();
      Assert.That(
          () => transactionContextImplementation.GetTimestamp(TestableClientTransaction),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
    }

    [Test]
    public void RegisterForCommit_WhenReferenceInitializationIsActive_ThrowsInvalidOperationException ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var transactionContextImplementation = new DomainObjectTransactionContextImplementation(order);
      transactionContextImplementation.BeginDomainObjectReferenceInitializing();

      Assert.That(
          () => transactionContextImplementation.RegisterForCommit(TestableClientTransaction),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
    }

    [Test]
    public void EnsureDataAvailable_WhenReferenceInitializationIsActive_ThrowsInvalidOperationException ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var transactionContextImplementation = new DomainObjectTransactionContextImplementation(order);
      transactionContextImplementation.BeginDomainObjectReferenceInitializing();

      Assert.That(
          () => transactionContextImplementation.EnsureDataAvailable(TestableClientTransaction),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
    }

    [Test]
    public void TryEnsureDataAvailable_WhenReferenceInitializationIsActive_ThrowsInvalidOperationException ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var transactionContextImplementation = new DomainObjectTransactionContextImplementation(order);
      transactionContextImplementation.BeginDomainObjectReferenceInitializing();

      Assert.That(
          () => transactionContextImplementation.TryEnsureDataAvailable(TestableClientTransaction),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used."));
    }
  }
}
