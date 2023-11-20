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
using NUnit.Framework;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ReadOnlyListExtensionsTest
  {
    [Test]
    public void IndexOf_WithElementNotInList_ReturnsNegativeOne ()
    {
      var values = new int[] { 1, 2, 3 };

      Assert.That(ReadOnlyListExtensions.IndexOf(values, 4), Is.EqualTo(-1));
    }

    [Test]
    public void IndexOf_WithElementInList_ReturnsCorrectIndex ()
    {
      var values = new[] { 1, 2, 3 };

      Assert.That(ReadOnlyListExtensions.IndexOf(values, 2), Is.EqualTo(1));
    }

    [Test]
    public void IndexOf_WithMultipleOccurrencesInList_ReturnsFirstIndex ()
    {
      var values = new[] { 1, 2, 3, 2 };

      Assert.That(ReadOnlyListExtensions.IndexOf(values, 2), Is.EqualTo(1));
    }

    [Test]
    public void IndexOf_WithNullValueInList_ReturnsCorrectIndex ()
    {
      var values = new[] { "a", null, "b" };

      Assert.That(ReadOnlyListExtensions.IndexOf(values, "b"), Is.EqualTo(2));
    }

    [Test]
    public void IndexOf_WithNullValueSearch_ReturnsCorrectIndex ()
    {
      var values = new[] { "a", null, "b" };

      Assert.That(ReadOnlyListExtensions.IndexOf(values, null), Is.EqualTo(1));
    }
  }
}
