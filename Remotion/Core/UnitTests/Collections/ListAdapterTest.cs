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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ListAdapterTest
  {
    private List<int> _innerList;
    private ListAdapter<int, string> _adapter;

    [SetUp]
    public void SetUp ()
    {
      _innerList = new List<int> { 1, 2, 3 };
      _adapter = new ListAdapter<int, string> (_innerList, i => i.ToString(), s => int.Parse (s));
    }

    [Test]
    public void Count ()
    {
      Assert.That (_adapter.Count, Is.EqualTo (3));
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_adapter.IsReadOnly, Is.False);

      var readOnlyAdapter = new ListAdapter<int, string> (new ReadOnlyCollection<int> (_innerList), i => i.ToString(), s => int.Parse (s));
      Assert.That (readOnlyAdapter.IsReadOnly, Is.True);
    }

    [Test]
    public void Item_Get ()
    {
      Assert.That (_adapter[1], Is.EqualTo ("2"));
    }

    [Test]
    public void Item_Set ()
    {
      _adapter[1] = "4";
      
      Assert.That (_innerList, Is.EqualTo (new[] { 1, 4, 3 }));
    }

    [Test]
    public void GetEnumerator ()
    {
      using (var enumerator = _adapter.GetEnumerator())
      {
        Assert.That (enumerator.MoveNext(), Is.True);
        Assert.That (enumerator.Current, Is.EqualTo ("1"));
        Assert.That (enumerator.MoveNext(), Is.True);
        Assert.That (enumerator.Current, Is.EqualTo ("2"));
        Assert.That (enumerator.MoveNext(), Is.True);
        Assert.That (enumerator.Current, Is.EqualTo ("3"));
        Assert.That (enumerator.MoveNext(), Is.False);
      }
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      var enumerator = ((IEnumerable) _adapter).GetEnumerator();

      Assert.That (enumerator.MoveNext(), Is.True);
      Assert.That (enumerator.Current, Is.EqualTo ("1"));
      Assert.That (enumerator.MoveNext(), Is.True);
      Assert.That (enumerator.Current, Is.EqualTo ("2"));
      Assert.That (enumerator.MoveNext(), Is.True);
      Assert.That (enumerator.Current, Is.EqualTo ("3"));
      Assert.That (enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void Insert ()
    {
      _adapter.Insert (1, "5");

      Assert.That (_innerList, Is.EqualTo (new[] { 1, 5, 2, 3 }));
    }

    [Test]
    public void Add ()
    {
      _adapter.Add ("5");

      Assert.That (_innerList, Is.EqualTo (new[] { 1, 2, 3, 5 }));
    }

    [Test]
    public void Remove ()
    {
      var result = _adapter.Remove ("2");

      Assert.That (result, Is.True);
      Assert.That (_innerList, Is.EqualTo (new[] { 1, 3 }));

      result = _adapter.Remove ("2");

      Assert.That (result, Is.False);
    }

    [Test]
    public void RemoveAt ()
    {
      _adapter.RemoveAt (0);

      Assert.That (_innerList, Is.EqualTo (new[] { 2, 3 }));
    }

    [Test]
    public void Clear ()
    {
      _adapter.Clear ();

      Assert.That (_innerList, Is.Empty);
    }

    [Test]
    public void Contains()
    {
      Assert.That (_adapter.Contains ("7"), Is.False);
      Assert.That (_adapter.Contains ("1"), Is.True);
      Assert.That (_adapter.Contains ("b"), Is.False);
    }

    [Test]
    public void Contains_WithReadOnlyAdapter ()
    {
      var readOnlyAdapter = ListAdapter.AdaptReadOnly (_innerList, i => i.ToString());
      Assert.That (readOnlyAdapter.Contains ("7"), Is.False);
      Assert.That (readOnlyAdapter.Contains ("1"), Is.True);
      Assert.That (readOnlyAdapter.Contains ("b"), Is.False);
    }

    [Test]
    public void IndexOf ()
    {
      Assert.That (_adapter.IndexOf ("1"), Is.EqualTo (0));
      Assert.That (_adapter.IndexOf ("7"), Is.EqualTo (-1));
      Assert.That (_adapter.IndexOf ("b"), Is.EqualTo (-1));
    }

    [Test]
    public void IndexOf_WithReadOnlyAdapter ()
    {
      var readOnlyAdapter = ListAdapter.AdaptReadOnly (_innerList, i => i.ToString ());
      Assert.That (readOnlyAdapter.IndexOf ("1"), Is.EqualTo (0));
      Assert.That (readOnlyAdapter.IndexOf ("7"), Is.EqualTo (-1));
      Assert.That (readOnlyAdapter.IndexOf ("b"), Is.EqualTo (-1));
    }

    [Test]
    public void CopyTo ()
    {
      var destArray = new[] { "x", "x", "x", "x", "x", "x" };
      
      _adapter.CopyTo (destArray, 2);

      Assert.That (destArray, Is.EqualTo (new[] { "x", "x", "1", "2", "3", "x" }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage = "Index must not be negative.\r\nParameter name: arrayIndex")]
    public void CopyTo_IndexLessThanZero ()
    {
      var destArray = new[] { "x", "x", "x", "x", "x", "x" };
      _adapter.CopyTo (destArray, -1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Index must be less than the length of the array.\r\nParameter name: arrayIndex")]
    public void CopyTo_ArrayIndexTooHigh ()
    {
      var destArray = new[] { "x", "x", "x", "x", "x", "x" };
      _adapter.CopyTo (destArray, 6);
    }

    [Test]
    public void CopyTo_JustEnoughSpace ()
    {
      var destArray = new[] { "x", "x", "x", "x", "x", "x" };

      _adapter.CopyTo (destArray, 3);

      Assert.That (destArray, Is.EqualTo (new[] { "x", "x", "x", "1", "2", "3" }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "There must be enough space to copy all items into the destination array starting at the given index.\r\nParameter name: arrayIndex")]
    public void CopyTo_NotEnoughSpace ()
    {
      var destArray = new[] { "x", "x", "x", "x", "x", "x" };
      _adapter.CopyTo (destArray, 4);
    }

    [Test]
    public void Adapt ()
    {
      var adapter = ListAdapter.Adapt (_innerList, i => i.ToString (), s => int.Parse (s));

      Assert.That (adapter[1], Is.EqualTo ("2"));

      adapter[1] = "5";

      Assert.That (_innerList[1], Is.EqualTo (5));
    }

    [Test]
    public void AdaptOneWay ()
    {
      var adapter = ListAdapter.AdaptOneWay (_innerList, i => i.ToString ());

      Assert.That (adapter[1], Is.EqualTo ("2"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "This list does not support setting of 'String' values.")]
    public void AdaptOneWay_ReverseNotSupported ()
    {
      var adapter = ListAdapter.AdaptOneWay (_innerList, i => i.ToString ());

      adapter[1] = "1";
    }

    [Test]
    public void AdaptReadOnly ()
    {
      var adapter = ListAdapter.AdaptReadOnly (_innerList, i => i.ToString ());

      Assert.That (adapter, Is.TypeOf (typeof (ReadOnlyCollection<string>)));
      Assert.That (adapter[1], Is.EqualTo ("2"));

      _innerList[1] = 5;
      Assert.That (adapter[1], Is.EqualTo ("5"));
    }
  }
}