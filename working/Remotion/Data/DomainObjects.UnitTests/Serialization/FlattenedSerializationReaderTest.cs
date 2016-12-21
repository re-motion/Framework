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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class FlattenedSerializationReaderTest
  {
    [Test]
    public void ReadValue ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.That (reader.ReadValue(), Is.EqualTo (1));
    }

    [Test]
    public void ReadValue_MultipleTimes ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.That (reader.ReadValue (), Is.EqualTo (1));
      Assert.That (reader.ReadValue (), Is.EqualTo (2));
      Assert.That (reader.ReadValue (), Is.EqualTo (3));
    }

    [Test]
    public void ReadPosition ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.That (reader.ReadPosition, Is.EqualTo (0));
      reader.ReadValue();
      Assert.That (reader.ReadPosition, Is.EqualTo (1));
      reader.ReadValue ();
      Assert.That (reader.ReadPosition, Is.EqualTo (2));
      reader.ReadValue ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no more data in the serialization stream at position 3.")]
    public void ReadValue_TooOften ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.That (reader.ReadValue (), Is.EqualTo (1));
      Assert.That (reader.ReadValue (), Is.EqualTo (2));
      Assert.That (reader.ReadValue (), Is.EqualTo (3));
      reader.ReadValue ();
    }

    [Test]
    public void EndReached_False ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.That (reader.EndReached, Is.False);
      reader.ReadValue ();
      Assert.That (reader.EndReached, Is.False);
      reader.ReadValue ();
      Assert.That (reader.EndReached, Is.False);
    }

    [Test]
    public void EndReached_True ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      reader.ReadValue ();
      reader.ReadValue ();
      reader.ReadValue ();
      Assert.That (reader.EndReached, Is.True);
    }

    [Test]
    public void EndReached_Empty ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[0]);
      Assert.That (reader.EndReached, Is.True);
    }
  }
}
