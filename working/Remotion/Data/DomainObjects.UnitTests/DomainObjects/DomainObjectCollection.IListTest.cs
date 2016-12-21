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
using System.Collections;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  public partial class DomainObjectCollectionTest
  {
    [Test]
    public void IList_Contains ()
    {
      IList list = _collection;

      Assert.That (list.Contains (_customer1), Is.True);
      Assert.That (list.Contains (_customer1.ID), Is.True);
      Assert.That (list.Contains (new object ()), Is.False);

      Assert.That (list.IndexOf (_customer1), Is.EqualTo (0));
      Assert.That (list[0], Is.SameAs (_customer1));
    }

    [Test]
    public void IList_IndexOf ()
    {
      IList list = _collection;

      Assert.That (list.IndexOf (_customer1), Is.EqualTo (0));
      Assert.That (list.IndexOf (_customer1.ID), Is.EqualTo (0));
      Assert.That (list.IndexOf (new object ()), Is.EqualTo (-1));
    }

    [Test]
    public void IList_Item ()
    {
      IList list = _collection;

      Assert.That (list[0], Is.SameAs (_customer1));
    }

    [Test]
    public void IList_Add ()
    {
      IList list = _collection;
      Assert.That (_collection, Has.No.Member(_customer3NotInCollection));

      list.Add (_customer3NotInCollection);
      Assert.That (_collection, Has.Member(_customer3NotInCollection));
    }

    [Test]
    public void IList_Remove ()
    {
      IList list = _collection;
      Assert.That (_collection, Has.Member(_customer1));
      Assert.That (_collection, Has.Member(_customer2));

      list.Remove (_customer1);
      Assert.That (_collection, Has.No.Member(_customer1));

      list.Remove (_customer2.ID);
      Assert.That (_collection, Has.No.Member(_customer2));

      list.Add (_customer3NotInCollection);
      Assert.That (_collection, Is.EqualTo (new[] { _customer3NotInCollection }));

      list.Remove (new object ());
      Assert.That (_collection, Is.EqualTo (new[] { _customer3NotInCollection }));
    }

    [Test]
    public void IList_Insert ()
    {
      IList list = _collection;
      Assert.That (_collection, Is.EqualTo (new[] { _customer1, _customer2 }));

      list.Insert (1, _customer3NotInCollection);
      Assert.That (_collection, Is.EqualTo (new[] { _customer1, _customer3NotInCollection, _customer2 }));
    }

    [Test]
    public void IList_IsFixedSize ()
    {
      Assert.That (((IList) _collection).IsFixedSize, Is.False);
    }

    [Test]
    public void IList_IsSynchronized ()
    {
      Assert.That (((IList) _collection).IsSynchronized, Is.False);
    }
  }
}