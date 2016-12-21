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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [TestFixture]
  public class DomainObjectCollectionDataExtensionsTest : ClientTransactionBaseTest
  {
    private DomainObjectCollectionData _data;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      _order2 = DomainObjectIDs.Order2.GetObject<Order> ();
      _order3 = DomainObjectIDs.Order3.GetObject<Order> ();
      _order4 = DomainObjectIDs.Order4.GetObject<Order> ();

      _data = new DomainObjectCollectionData (new[] { _order1, _order2 });
    }

    [Test]
    public void Add ()
    {
      _data.Add (_order3);

      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
    }

    [Test]
    public void AddRange ()
    {
      _data.AddRange (new[] { _order3, _order4 });

      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3, _order4 }));
    }

    [Test]
    public void AddRangeAndCheckItems ()
    {
      _data.AddRangeAndCheckItems (new[] { _order3, _order4 }, typeof (Order));

      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3, _order4 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage =
        "Item 1 of parameter 'domainObjects' is null.\r\nParameter name: domainObjects")]
    public void AddRangeAndCheckItems_NullItem ()
    {
      _data.AddRangeAndCheckItems (new[] { _order3, null }, typeof (Order));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Item 1 of parameter 'domainObjects' is a duplicate ('Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid')."
        + "\r\nParameter name: domainObjects")]
    public void AddRangeAndCheckItems_DuplicateItem ()
    {
      _data.AddRangeAndCheckItems (new[] { _order3, _order3 }, typeof (Order));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Item 0 of parameter 'domainObjects' has the type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' " +
        "instead of 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer'.\r\nParameter name: domainObjects")]
    public void AddRangeAndCheckItems_InvalidType ()
    {
      _data.AddRangeAndCheckItems (new[] { _order3 }, typeof (Customer));
    }

    [Test]
    public void ReplaceContents ()
    {
      _data.ReplaceContents(new[] { _order3, _order4, _order1 });

      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order3, _order4, _order1 }));
    }

    [Test]
    public void SetEquals ()
    {
      Assert.That (_data.SetEquals (new[] { _order1, _order2 }), Is.True);
      Assert.That (_data.SetEquals (new[] { _order2, _order1 }), Is.True);
      Assert.That (_data.SetEquals (new[] { _order1, _order2, _order3 }), Is.False);
    }

    [Test]
    public void SetEquals_UsesReferenceComparison ()
    {
      var order1FromOtherTransaction = DomainObjectMother.GetObjectInOtherTransaction<Order> (_order1.ID);
      Assert.That (_data.SetEquals (new[] { order1FromOtherTransaction, _order2 }), Is.False);
    }

    [Test]
    public void SetEquals_HandlesDuplicates ()
    {
      Assert.That (_data.SetEquals (new[] { _order1, _order2, _order2, _order1 }), Is.True);
    }
  }
}