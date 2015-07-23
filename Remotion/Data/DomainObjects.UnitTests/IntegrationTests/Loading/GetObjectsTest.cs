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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Loading
{
  [TestFixture]
  public class GetObjectsTest : ClientTransactionBaseTest
  {
    [Test]
    public void GettingSameObjectID_MultipleTimes ()
    {
      var domainObjects = LifetimeService.GetObjects<DomainObject> (TestableClientTransaction, DomainObjectIDs.Order1, DomainObjectIDs.Order1);

      Assert.That (domainObjects, Is.EqualTo (new[] { DomainObjectIDs.Order1.GetObject<Order>(), DomainObjectIDs.Order1.GetObject<Order>() }));
    }

    [Test]
    public void OnLoadedAccessingObject_LoadedLaterInSameBatch_IsSupported_AndDoesNotThrow ()
    {
      var order1 = DomainObjectIDs.Order1.GetObjectReference<Order> ();
      var order3 = DomainObjectIDs.Order3.GetObjectReference<Order> ();

      bool order1LoadedCalled = false;
      order1.ProtectedLoaded += (sender, args) =>
      {
        order3.EnsureDataAvailable();
        order1LoadedCalled = true;
      };

      int order2LoadedCount = 0;
      order3.ProtectedLoaded += (sender, args) => { order2LoadedCount++; };

      var domainObjects = LifetimeService.GetObjects<DomainObject> (TestableClientTransaction, DomainObjectIDs.Order1, DomainObjectIDs.Order3);

      Assert.That (domainObjects, Is.EqualTo (new[] { order1, order3 }));
      Assert.That (order1LoadedCalled, Is.True);
      Assert.That (order2LoadedCount, Is.EqualTo (1));
    }
  }
}