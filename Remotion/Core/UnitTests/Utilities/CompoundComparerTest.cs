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
using Moq;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class CompoundComparerTest
  {
    [Test]
    public void NoComparer_ReturnsZero ()
    {
      var compoundComparer = new CompoundComparer<int>();

      Assert.That(compoundComparer.Compare(1, 2), Is.EqualTo(0));
      Assert.That(compoundComparer.Compare(2, 1), Is.EqualTo(0));
      Assert.That(compoundComparer.Compare(2, 2), Is.EqualTo(0));
    }

    [Test]
    public void OneComparer_NonZero ()
    {
      var stubComparer = new Mock<IComparer<int>>();
      var compoundComparer = new CompoundComparer<int>(stubComparer.Object);

      stubComparer.Setup(stub => stub.Compare(1, 2)).Returns(-1);

      Assert.That(compoundComparer.Compare(1, 2), Is.EqualTo(-1));
    }

    [Test]
    public void OneComparer_Zero ()
    {
      var stubComparer = new Mock<IComparer<int>>();
      var compoundComparer = new CompoundComparer<int>(stubComparer.Object);

      stubComparer.Setup(stub => stub.Compare(1, 2)).Returns(0);

      Assert.That(compoundComparer.Compare(1, 2), Is.EqualTo(0));
    }

    [Test]
    public void ManyComparers_UsesFirstNonZero ()
    {
      var stubComparer1 = new Mock<IComparer<int>>();
      var stubComparer2 = new Mock<IComparer<int>>();
      var stubComparer3 = new Mock<IComparer<int>>();
      var stubComparer4 = new Mock<IComparer<int>>();

      var compoundComparer = new CompoundComparer<int>(stubComparer1.Object, stubComparer2.Object, stubComparer3.Object, stubComparer4.Object);

      stubComparer1.Setup(stub => stub.Compare(1, 2)).Returns(0);
      stubComparer2.Setup(stub => stub.Compare(1, 2)).Returns(0);
      stubComparer3.Setup(stub => stub.Compare(1, 2)).Returns(1);
      stubComparer4.Setup(stub => stub.Compare(1, 2)).Returns(-1);

      Assert.That(compoundComparer.Compare(1, 2), Is.EqualTo(1));
    }
  }
}
