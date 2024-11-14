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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class DelegateBasedComparerTest
  {
    [Test]
    public void Compare ()
    {
      var comparer = new DelegateBasedComparer<List<int>>((x, y) => x.Count.CompareTo(y.Count));

      var l1 = new List<int> { 1 };
      var l2 = new List<int> { 1, 2 };
      var l3 = new List<int> { 1, 2 };

      Assert.That(comparer.Compare(l1, l2), Is.EqualTo(-1));
      Assert.That(comparer.Compare(l2, l1), Is.EqualTo(1));
      Assert.That(comparer.Compare(l2, l3), Is.EqualTo(0));
    }
  }
}
