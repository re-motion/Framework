﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Collections;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class LockingCacheInvalidationTokenTest
  {
#if DEBUG
    [Ignore ("Skipped if DEBUG build")]
#endif
    [Test]
    public void GetCurrent_FromSameCacheInvalidationToken_ReturnsSameRevisionTwice ()
    {
      var token = InvalidationToken.CreatWithLocking();

      Assert.That (token.GetCurrent(), Is.EqualTo (token.GetCurrent()));
    }

    [Test]
    public void GetCurrent_FromDifferentCacheInvalidationTokens_ReturnsDifferentRevisions ()
    {
      var token1 = InvalidationToken.CreatWithLocking();
      var token2 = InvalidationToken.CreatWithLocking();

      if (token1.GetHashCode() == token2.GetHashCode())
      {
        Assert.Ignore (
            "GetHashCode() happened to have returned the same value for different CacheInvalidationToken instances. "
            + "This means the same seed value has been used and the tokens should not be used for comparission.");
      }

      Assert.That (token1.GetCurrent(), Is.Not.EqualTo (token2.GetCurrent()));
    }

    [Test]
    public void IsCurrent_WithCurrentRevision_ReturnsTrue ()
    {
      var token = InvalidationToken.CreatWithLocking();

      var revision = token.GetCurrent();

      Assert.That (token.IsCurrent (revision), Is.True);
    }

    [Test]
    public void IsCurrent_WithInvalidatedRevision_ReturnsFalse ()
    {
      var token = InvalidationToken.CreatWithLocking();

      var revision = token.GetCurrent();
      token.Invalidate();

      Assert.That (token.IsCurrent (revision), Is.False);
    }

#if !DEBUG
    [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    public void IsCurrent_WithRevisionFromDifferentToken_ThrowsArgumentException ()
    {
      var token1 = InvalidationToken.CreatWithLocking();
      var token2 = InvalidationToken.CreatWithLocking();

      Assert.That (
          () => token2.IsCurrent (token1.GetCurrent()),
          Throws.ArgumentException.With.Message.EqualTo (
              "The Revision used for the comparision was not created by the current CacheInvalidationToken.\r\nParameter name: revision"));
    }

#if !DEBUG
    [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    public void IsCurrent_WithRevisionFromDefaultConstructor_ThrowsArgumentException ()
    {
      var token = InvalidationToken.CreatWithLocking();

      Assert.That (
          () => token.IsCurrent (new InvalidationToken.Revision()),
          Throws.ArgumentException.With.Message.EqualTo (
              "The Revision used for the comparision was either created via the default constructor or the associated CacheInvalidationToken has already been garbage collected.\r\n"
              + "Parameter name: revision"));
    }

    [Test]
    public void Serialization ()
    {
      var token = InvalidationToken.CreatWithLocking();
      var revision = token.GetCurrent();

      var deserializedObjects = Serializer.SerializeAndDeserialize (new object[] { token, revision });
      var deserializedToken = (InvalidationToken) deserializedObjects[0];
      var deserializedRevision = (InvalidationToken.Revision) deserializedObjects[1];

      Assert.That (deserializedToken.IsCurrent (deserializedRevision), Is.True);
#if DEBUG
      Assert.That (
          () => token.IsCurrent (deserializedRevision),
          Throws.ArgumentException.With.Message.EqualTo (
              "The Revision used for the comparision was not created by the current CacheInvalidationToken.\r\nParameter name: revision"));
#endif
    }
  }
}