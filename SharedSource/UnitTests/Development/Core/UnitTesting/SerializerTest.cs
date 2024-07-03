// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

#nullable enable
// ReSharper disable once CheckNamespace

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class SerializerTest
  {
    [Test]
    public void SerializeAndDeserialize ()
    {
      int[] array = new int[] { 1, 2, 3 };
      int[] array2 = Serializer.SerializeAndDeserialize(array);
      Assert.That(array2, Is.Not.SameAs(array));

      Assert.That(array2.Length, Is.EqualTo(array.Length));
      Assert.That(array2[0], Is.EqualTo(array[0]));
      Assert.That(array2[1], Is.EqualTo(array[1]));
      Assert.That(array2[2], Is.EqualTo(array[2]));
    }
  }
}
