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
using NUnit.Framework;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ImmutableSingletonTest
  {
    [Test]
    public void Count ()
    {
      IReadOnlyList<string> list = ImmutableSingleton.Create ("A");
      Assert.That (list.Count, Is.EqualTo (1));
    }

    [Test]
    public void Index_With0_ReturnsItem ()
    {
      IReadOnlyList<string> list = ImmutableSingleton.Create ("B");
      Assert.That (list[0], Is.EqualTo ("B"));
    }

    [Test]
    public void Index_WithGreaterThan0_ThrowsArgumentOutOfRangeException ()
    {
      IReadOnlyList<string> list = ImmutableSingleton.Create ("B");
      Assert.That (
          () => list[1],
          Throws.TypeOf<ArgumentOutOfRangeException>()
              .With.Message.EqualTo ("The list contains only a single item.\r\nParameter name: index\r\nActual value was 1."));
    }

    [Test]
    public void Index_WithLessThan0_ThrowsArgumentOutOfRangeException ()
    {
      IReadOnlyList<string> list = ImmutableSingleton.Create ("B");
      Assert.That (
          () => list[-1],
          Throws.TypeOf<ArgumentOutOfRangeException>()
              .With.Message.EqualTo ("The list contains only a single item.\r\nParameter name: index\r\nActual value was -1."));
    }

    [Test]
    public void Enumerate_Generic ()
    {
      IReadOnlyList<string> list = ImmutableSingleton.Create ("B");
      IEnumerator<string> enumerator = list.GetEnumerator();

      Assert.That (() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration has not started. Call MoveNext."));
      Assert.That (enumerator.MoveNext(), Is.True);
      Assert.That (enumerator.Current, Is.EqualTo ("B"));
      Assert.That (enumerator.MoveNext(), Is.False);
      Assert.That (() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration already finished."));
      enumerator.Reset();
      Assert.That (() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration has not started. Call MoveNext."));
      Assert.That (enumerator.MoveNext(), Is.True);
      Assert.That (enumerator.Current, Is.EqualTo ("B"));
    }

    [Test]
    public void Enumerate_NonGeneric ()
    {
      IReadOnlyList<string> list = ImmutableSingleton.Create ("B");
      IEnumerator enumerator = list.ToNonGenericEnumerable().GetEnumerator();

      Assert.That (() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration has not started. Call MoveNext."));
      Assert.That (enumerator.MoveNext(), Is.True);
      Assert.That (enumerator.Current, Is.EqualTo ("B"));
      Assert.That (enumerator.MoveNext(), Is.False);
      Assert.That (() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration already finished."));
      enumerator.Reset();
      Assert.That (() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration has not started. Call MoveNext."));
      Assert.That (enumerator.MoveNext(), Is.True);
      Assert.That (enumerator.Current, Is.EqualTo ("B"));
    }

    [Test]
    public void Enumerate_Foreach ()
    {
      var items = new List<string>();
      foreach (var item in  ImmutableSingleton.Create ("B"))
        items.Add (item);

      Assert.That (items, Is.EqualTo (new[] { "B" }));
    }

    [Test]
    public void InitializeWithNull ()
    {
      IReadOnlyList<object> list = ImmutableSingleton.Create<object> (null);
      Assert.That (list.Count, Is.EqualTo (1));
      Assert.That (list[0], Is.Null);
    }
  }
}