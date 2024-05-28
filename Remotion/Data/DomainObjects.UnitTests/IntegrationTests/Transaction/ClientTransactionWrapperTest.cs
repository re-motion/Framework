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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionWrapperTest : ClientTransactionBaseTest
  {
    private ITransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = TestableClientTransaction.ToITransaction();
    }

    [Test]
    public void To_ClientTransaction ()
    {
      var actual = _transaction.To<TestableClientTransaction>();

      Assert.That(actual, Is.SameAs(TestableClientTransaction));
    }

    [Test]
    public void To_InvalidType ()
    {
      Assert.That(
          () => _transaction.To<DomainObject>(),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'TTransaction' is a 'Remotion.Data.DomainObjects.DomainObject', "
                  + "which cannot be assigned to type 'Remotion.Data.DomainObjects.ClientTransaction'.", "TTransaction"));
    }

    [Test]
    public void CanCreateChild ()
    {
      Assert.That(_transaction.CanCreateChild, Is.True);
    }

    [Test]
    public void CreateChild ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.That(child, Is.Not.Null);
      Assert.That(child, Is.InstanceOf(typeof(ClientTransactionWrapper)));
      Assert.That(((ClientTransactionWrapper)child).WrappedInstance, Is.InstanceOf(typeof(ClientTransaction)));

      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy(((ClientTransactionWrapper)child).WrappedInstance);
      Assert.That(persistenceStrategy, Is.InstanceOf(typeof(SubPersistenceStrategy)));
    }

    [Test]
    public void IsChild ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.That(child.IsChild, Is.True);
      Assert.That(_transaction.IsChild, Is.False);
      Assert.That(child.CreateChild().IsChild, Is.True);
    }

    [Test]
    public void Parent ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.That(((ClientTransactionWrapper)child.Parent).WrappedInstance, Is.SameAs(((ClientTransactionWrapper)_transaction).WrappedInstance));
      Assert.That(((ClientTransactionWrapper)child.CreateChild().Parent).WrappedInstance, Is.SameAs(((ClientTransactionWrapper)child).WrappedInstance));
    }

    [Test]
    public void Parent_WithoutParent ()
    {
      Assert.That((ClientTransactionWrapper)_transaction.Parent, Is.Null);
    }

    [Test]
    public void Release ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.That(((ClientTransactionWrapper)_transaction).WrappedInstance.IsWriteable, Is.False);
      Assert.That(((ClientTransactionWrapper)child).WrappedInstance.IsDiscarded, Is.False);
      child.Release();
      Assert.That(((ClientTransactionWrapper)_transaction).WrappedInstance.IsWriteable, Is.True);
      Assert.That(((ClientTransactionWrapper)child).WrappedInstance.IsDiscarded, Is.True);
    }

    [Test]
    public void EnterScope ()
    {
      ITransaction transaction = ClientTransaction.CreateRootTransaction().ToITransaction();

      ClientTransactionScope.ResetActiveScope();
      Assert.That(ClientTransactionScope.ActiveScope, Is.Null);

      ITransactionScope transactionScope = transaction.EnterScope();

      Assert.That(ClientTransactionScope.ActiveScope, Is.SameAs(transactionScope));
      Assert.That(ClientTransactionScope.ActiveScope.ScopedTransaction, Is.SameAs(((ClientTransactionWrapper)transaction).WrappedInstance));
      Assert.That(ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo(AutoRollbackBehavior.None));
      ClientTransactionScope.ResetActiveScope();
    }

    [Test]
    public void EnterScope_WithInactiveTransaction ()
    {
      var inactiveTransaction = ClientTransaction.CreateRootTransaction();
      using (ClientTransactionTestHelper.MakeInactive(inactiveTransaction))
      {
        ITransaction transaction = inactiveTransaction.ToITransaction();
        Assert.That(inactiveTransaction.ActiveTransaction, Is.Not.SameAs(inactiveTransaction));

        var scope = transaction.EnterScope();

        Assert.That(inactiveTransaction.ActiveTransaction, Is.SameAs(inactiveTransaction));
        scope.Leave();
      }
    }

    [Test]
    public void EnsureCompatibility_WithNonDomainObject_Succeeds ()
    {
      _transaction.EnsureCompatibility(new[] { "what?" });
    }

    [Test]
    public void EnsureCompatibility_WithObjectFromSameTransaction_Succeeds ()
    {
      var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>(TestableClientTransaction);

      _transaction.EnsureCompatibility(new[] { domainObject });
    }

    [Test]
    public void EnsureCompatibility_WithObjectFromSubTransaction_Succeeds ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>(TestableClientTransaction);
        _transaction.EnsureCompatibility(new[] { domainObject });
      }
    }

    [Test]
    public void EnsureCompatibility_WithObjectFromParentTransaction_Succeeds ()
    {
      var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>(TestableClientTransaction);
      var transaction = TestableClientTransaction.CreateSubTransaction().ToITransaction();
      transaction.EnsureCompatibility(new[] { domainObject });
    }

    [Test]
    public void EnsureCompatibility_WithObjectsFromUnrelatedTransaction_ThrowsException ()
    {
      var domainObject1 = DomainObjectMother.GetObjectInOtherTransaction<ClassWithAllDataTypes>(DomainObjectIDs.ClassWithAllDataTypes1);
      var domainObject2 = DomainObjectMother.GetObjectInOtherTransaction<ClassWithAllDataTypes>(DomainObjectIDs.ClassWithAllDataTypes2);
      Assert.That(
          () => _transaction.EnsureCompatibility(new[] { domainObject1, domainObject2 }),
          Throws.TypeOf<InvalidOperationException>().With.Message.Matches(
              @"The following objects are incompatible with the target transaction\: "
              + @"ClassWithAllDataTypes\|.*\|System\.Guid, ClassWithAllDataTypes\|.*\|System\.Guid\. "
              + @"Objects of type \'Remotion\.Data\.DomainObjects\.IDomainObjectHandle\`1\[T\]\' could be used instead\.")
                .And.Message.Contains("ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid")
                .And.Message.Contains("ClassWithAllDataTypes|583ec716-8443-4b55-92bf-09f7c8768529|System.Guid"));
    }

    [Test]
    public void CanBeDerivedFrom ()
    {
      var ctor =  typeof(ClientTransactionWrapper).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
          new[] {typeof(ClientTransaction)}, null);
      Assert.That(typeof(ClientTransactionWrapper).IsSealed, Is.False);
      Assert.That(ctor.IsFamilyOrAssembly);
    }
  }
}
