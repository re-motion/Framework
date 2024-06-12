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
  public class InvertedComparerDecoratorTest
  {
    [Test]
    public void Compare ()
    {
      var innerComparerStub = new Mock<IComparer<object>>();
      var comparer = new InvertedComparerDecorator<object>(innerComparerStub.Object);

      var obj1 = new object();
      var obj2 = new object();

      innerComparerStub.Setup(_ => _.Compare(obj2, obj1)).Returns(1);
      innerComparerStub.Setup(_ => _.Compare(obj1, obj2)).Returns(-1);
      innerComparerStub.Setup(_ => _.Compare(obj1, obj1)).Returns(0);

      Assert.That(comparer.Compare(obj1, obj2), Is.EqualTo(1));
      Assert.That(comparer.Compare(obj2, obj1), Is.EqualTo(-1));
      Assert.That(comparer.Compare(obj1, obj1), Is.EqualTo(0));
    }
  }
}
