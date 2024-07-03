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
  public class CollectionExtensionsTest
  {
    private IReadOnlyCollection<int> _collection;

    [SetUp]
    public void SetUp ()
    {
      _collection = new[] { 1, 2, 3 };
    }

    [Test]
    public void AsReadOnly ()
    {
      ReadOnlyCollectionWrapper<int> decorator = _collection.AsReadOnly();

      Assert.That(decorator, Is.EqualTo(_collection));
    }
  }
}
