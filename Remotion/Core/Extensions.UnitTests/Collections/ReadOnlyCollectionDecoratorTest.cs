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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ReadOnlyCollectionDecoratorTest
  {
    private ReadOnlyCollectionDecorator<string>_collection;

    [SetUp]
    public void SetUp ()
    {
      _collection = new ReadOnlyCollectionDecorator<string>(new List<string>(new[]{"test1", "test2"}));
    }

    [Test]
    public void Count ()
    {
      Assert.That(_collection.Count, Is.EqualTo(2));
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That(_collection.IsReadOnly, Is.True);
    }

    [Test]
    public void Contains_True ()
    {
      Assert.That(_collection.Contains("test1"), Is.True);
    }

    [Test]
    public void Contains_False ()
    {
      Assert.That(_collection.Contains("dummy"), Is.False);
    }

    [Test]
    public void Contains_NullIsSupported ()
    {
      Assert.That(_collection.Contains(null), Is.False);
    }

    [Test]
    public void CopyTo ()
    {
      var array = new string[2];
      _collection.CopyTo(array, 0);

      Assert.That(array[0], Is.EqualTo("test1"));
      Assert.That(array[1], Is.EqualTo("test2"));
    }

    [Test]
    public void GetEnumerator ()
    {
      var enumerator = _collection.GetEnumerator();
      enumerator.MoveNext();
      Assert.That(enumerator.Current, Is.EqualTo("test1"));
      enumerator.MoveNext();
      Assert.That(enumerator.Current, Is.EqualTo("test2"));
      Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void Add ()
    {
      Assert.That(
          () => ((ICollection<string>)_collection).Add("test"),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "'Add' ist not supported for read-only collections."));
    }

    [Test]
    public void Remove ()
    {
      Assert.That(
          () => ((ICollection<string>)_collection).Remove("test"),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "'Remove' ist not supported for read-only collections."));
    }

    [Test]
    public void Clear ()
    {
      Assert.That(
          () => ((ICollection<string>)_collection).Clear(),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "'Clear' ist not supported for read-only collections."));
    }

    [Test]
    public void Serialization ()
    {
      Assert.That(() => Serializer.Serialize(_collection), Throws.Nothing);
    }
  }
}
