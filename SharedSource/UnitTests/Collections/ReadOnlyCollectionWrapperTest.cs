// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ReadOnlyCollectionWrapperTest
  {
    private ReadOnlyCollectionWrapper<string >_collection;

    [SetUp]
    public void SetUp ()
    {
      _collection = new ReadOnlyCollectionWrapper<string>(new List<string>(new[] { "test1", "test2" }));
    }

    [Test]
    public void Count ()
    {
      Assert.That(_collection.Count, Is.EqualTo(2));
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
    public void IsReadOnly ()
    {
      Assert.That(((ICollection<string>)_collection).IsReadOnly, Is.True);
    }

    [Test]
    public void Contains_True ()
    {
      Assert.That(((ICollection<string>)_collection).Contains("test1"), Is.True);
    }

    [Test]
    public void Contains_False ()
    {
      Assert.That(((ICollection<string>)_collection).Contains("dummy"), Is.False);
    }

    [Test]
    public void Contains_NullIsSupported ()
    {
      Assert.That(((ICollection<string>)_collection).Contains(null), Is.False);
    }

    [Test]
    public void CopyTo ()
    {
      var array = new string[2];
      ((ICollection<string>)_collection).CopyTo(array, 0);

      Assert.That(array[0], Is.EqualTo("test1"));
      Assert.That(array[1], Is.EqualTo("test2"));
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
  }
}
