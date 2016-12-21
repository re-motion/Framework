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
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class CompoundComparerTest
  {
    [Test]
    public void NoComparer_ReturnsZero ()
    {
      var compoundComparer = new CompoundComparer<int> ();

      Assert.That (compoundComparer.Compare (1, 2), Is.EqualTo (0));
      Assert.That (compoundComparer.Compare (2, 1), Is.EqualTo (0));
      Assert.That (compoundComparer.Compare (2, 2), Is.EqualTo (0));
    }

    [Test]
    public void OneComparer_NonZero ()
    {
      var stubComparer = MockRepository.GenerateStub<IComparer<int>> ();
      var compoundComparer = new CompoundComparer<int> (stubComparer);

      stubComparer.Stub (stub => stub.Compare (1, 2)).Return (-1);

      Assert.That (compoundComparer.Compare (1, 2), Is.EqualTo (-1));
    }

    [Test]
    public void OneComparer_Zero ()
    {
      var stubComparer = MockRepository.GenerateStub<IComparer<int>> ();
      var compoundComparer = new CompoundComparer<int> (stubComparer);

      stubComparer.Stub (stub => stub.Compare (1, 2)).Return (0);

      Assert.That (compoundComparer.Compare (1, 2), Is.EqualTo (0));
    }

    [Test]
    public void ManyComparers_UsesFirstNonZero ()
    {
      var stubComparer1 = MockRepository.GenerateStub<IComparer<int>> ();
      var stubComparer2 = MockRepository.GenerateStub<IComparer<int>> ();
      var stubComparer3 = MockRepository.GenerateStub<IComparer<int>> ();
      var stubComparer4 = MockRepository.GenerateStub<IComparer<int>> ();
      
      var compoundComparer = new CompoundComparer<int> (stubComparer1, stubComparer2, stubComparer3, stubComparer4);

      stubComparer1.Stub (stub => stub.Compare (1, 2)).Return (0);
      stubComparer2.Stub (stub => stub.Compare (1, 2)).Return (0);
      stubComparer3.Stub (stub => stub.Compare (1, 2)).Return (1);
      stubComparer4.Stub (stub => stub.Compare (1, 2)).Return (-1);

      Assert.That (compoundComparer.Compare (1, 2), Is.EqualTo (1));
    }

    [Test]
    public void Serializable ()
    {
      var compoundComparer = new CompoundComparer<int> (Comparer<int>.Default);
      
      var result = Serializer.SerializeAndDeserialize (compoundComparer);

      Assert.That (result.Compare (7, 4), Is.EqualTo (1));
    }
  }
}