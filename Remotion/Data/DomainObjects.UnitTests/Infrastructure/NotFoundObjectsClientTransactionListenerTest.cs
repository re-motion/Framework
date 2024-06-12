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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class NotFoundObjectsClientTransactionListenerTest : StandardMappingTest
  {
    private NotFoundObjectsClientTransactionListener _listener;

    public override void SetUp ()
    {
      base.SetUp();

      _listener = new NotFoundObjectsClientTransactionListener();
    }

    [Test]
    public void ObjectsNotFound ()
    {
      var objectID1 = DomainObjectIDs.Order1;
      var objectID2 = DomainObjectIDs.Order3;
      var objectIDs = new[] { objectID1, objectID2 };

      var parentTransaction = ClientTransaction.CreateRootTransaction();
      var subTransaction1 = parentTransaction.CreateSubTransaction();
      var subTransaction2 = subTransaction1.CreateSubTransaction();

      Assert.That(parentTransaction.IsInvalid(objectID1), Is.False);
      Assert.That(parentTransaction.IsInvalid(objectID2), Is.False);
      Assert.That(subTransaction1.IsInvalid(objectID1), Is.False);
      Assert.That(subTransaction1.IsInvalid(objectID2), Is.False);
      Assert.That(subTransaction2.IsInvalid(objectID1), Is.False);
      Assert.That(subTransaction2.IsInvalid(objectID2), Is.False);

      _listener.ObjectsNotFound(parentTransaction, Array.AsReadOnly(objectIDs));

      Assert.That(parentTransaction.IsInvalid(objectID1), Is.True);
      Assert.That(parentTransaction.IsInvalid(objectID2), Is.True);
      Assert.That(subTransaction1.IsInvalid(objectID1), Is.True);
      Assert.That(subTransaction1.IsInvalid(objectID2), Is.True);
      Assert.That(subTransaction2.IsInvalid(objectID1), Is.True);
      Assert.That(subTransaction1.IsInvalid(objectID2), Is.True);

      Assert.That(
          LifetimeService.GetObjectReference(parentTransaction, objectID1),
          Is.SameAs(LifetimeService.GetObjectReference(subTransaction1, objectID1)));
      Assert.That(
          LifetimeService.GetObjectReference(parentTransaction, objectID1),
          Is.SameAs(LifetimeService.GetObjectReference(subTransaction2, objectID1)));
    }
  }
}
