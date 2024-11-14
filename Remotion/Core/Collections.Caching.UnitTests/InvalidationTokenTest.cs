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
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Collections.Caching.UnitTests
{
  [TestFixture]
  public class InvalidationTokenTest
  {
#if DEBUG
    [Ignore("Skipped if DEBUG build")]
#endif
    [Test]
    public void GetCurrent_FromSameCacheInvalidationToken_ReturnsSameRevisionTwice ()
    {
      var token = InvalidationToken.Create();

      Assert.That(token.GetCurrent(), Is.EqualTo(token.GetCurrent()));
    }

    [Test]
    public void GetCurrent_FromDifferentCacheInvalidationTokens_ReturnsDifferentRevisions ()
    {
      var token1 = InvalidationToken.Create();
      var token2 = InvalidationToken.Create();

      if (token1.GetHashCode() == token2.GetHashCode())
      {
        Assert.Ignore(
            "GetHashCode() happened to have returned the same value for different CacheInvalidationToken instances. "
            + "This means the same seed value has been used and the tokens should not be used for comparission.");
      }

      Assert.That(token1.GetCurrent(), Is.Not.EqualTo(token2.GetCurrent()));
    }

    [Test]
    public void IsCurrent_WithCurrentRevision_ReturnsTrue ()
    {
      var token = InvalidationToken.Create();

      var revision = token.GetCurrent();

      Assert.That(token.IsCurrent(revision), Is.True);
    }

    [Test]
    public void IsCurrent_WithInvalidatedRevision_ReturnsFalse ()
    {
      var token = InvalidationToken.Create();

      var revision = token.GetCurrent();
      token.Invalidate();

      Assert.That(token.IsCurrent(revision), Is.False);
    }

#if !DEBUG
    [Ignore("Skipped unless DEBUG build")]
#endif
    [Test]
    public void IsCurrent_WithRevisionFromDifferentToken_ThrowsArgumentException ()
    {
      var token1 = InvalidationToken.Create();
      var token2 = InvalidationToken.Create();

      Assert.That(
          () => token2.IsCurrent(token1.GetCurrent()),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The Revision used for the comparision was not created by the current CacheInvalidationToken.", "revision"));
    }

#if !DEBUG
    [Ignore("Skipped unless DEBUG build")]
#endif
    [Test]
    public void IsCurrent_WithRevisionFromDefaultConstructor_ThrowsArgumentException ()
    {
      var token = InvalidationToken.Create();

      Assert.That(
          () => token.IsCurrent(new InvalidationToken.Revision()),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The Revision used for the comparision was either created via the default constructor or the associated CacheInvalidationToken has already been garbage collected.",
              "revision"));
    }
  }
}
