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
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions
{
  [TestFixture]
  public class LoadEventsTest : ReadOnlyTransactionsTestBase
  {
    private Order _order;

    public override void SetUp ()
    {
      base.SetUp();

      _order = (Order)LifetimeService.GetObjectReference(WriteableSubTransaction, DomainObjectIDs.Order1);

      InstallExtensionMock();
    }

    [Test]
    public void ObjectsLoading_Loaded_RaisedInAllHierarchyLevels ()
    {
      var sequence = new VerifiableSequence();
      ExtensionStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(ReadOnlyRootTransaction, new[] { _order.ID }))
          .Callback((ClientTransaction _, IReadOnlyList<ObjectID> _) => Assert.That(ReadOnlyRootTransaction.IsWriteable, Is.False))
          .Verifiable();
      ExtensionStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(ReadOnlyRootTransaction, new[] { _order }))
          .Callback((ClientTransaction _, IReadOnlyList<IDomainObject> _) => Assert.That(ReadOnlyRootTransaction.IsWriteable, Is.False))
          .Verifiable();
      ExtensionStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(ReadOnlyMiddleTransaction, new[] { _order.ID }))
          .Callback((ClientTransaction _, IReadOnlyList<ObjectID> _) => Assert.That(ReadOnlyMiddleTransaction.IsWriteable, Is.False))
          .Verifiable();
      ExtensionStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(ReadOnlyMiddleTransaction, new[] { _order }))
          .Callback((ClientTransaction _, IReadOnlyList<IDomainObject> _) => Assert.That(ReadOnlyMiddleTransaction.IsWriteable, Is.False))
          .Verifiable();
      ExtensionStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(WriteableSubTransaction, new[] { _order.ID }))
          .Callback((ClientTransaction _, IReadOnlyList<ObjectID> _) => Assert.That(WriteableSubTransaction.IsWriteable, Is.True))
          .Verifiable();
      ExtensionStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(WriteableSubTransaction, new[] { _order }))
          .Callback((ClientTransaction _, IReadOnlyList<IDomainObject> _) => Assert.That(WriteableSubTransaction.IsWriteable, Is.True))
          .Verifiable();

      ExecuteInWriteableSubTransaction(() => _order.EnsureDataAvailable());

      ExtensionStrictMock.Verify();
      sequence.Verify();
    }
  }
}
