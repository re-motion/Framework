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
  public class InvertedComparerDecoratorTest
  {
    [Test]
    public void Compare ()
    {
      var innerComparerStub = MockRepository.GenerateStub<IComparer<object>>();
      var comparer = new InvertedComparerDecorator<object> (innerComparerStub);

      var obj1 = new object();
      var obj2 = new object();

      innerComparerStub.Stub (_ => _.Compare (obj2, obj1)).Return (1);
      innerComparerStub.Stub (_ => _.Compare (obj1, obj2)).Return (-1);
      innerComparerStub.Stub (_ => _.Compare (obj1, obj1)).Return (0);

      Assert.That (comparer.Compare (obj1, obj2), Is.EqualTo (1));
      Assert.That (comparer.Compare (obj2, obj1), Is.EqualTo (-1));
      Assert.That (comparer.Compare (obj1, obj1), Is.EqualTo (0));
    }

    [Test]
    public void Serializable ()
    {
      var comparer = new InvertedComparerDecorator<string> (StringComparer.InvariantCulture);
      var deserializedComparer = Serializer.SerializeAndDeserialize (comparer);

      Assert.That (deserializedComparer.Compare ("a", "b"), Is.EqualTo (1));
      Assert.That (deserializedComparer.Compare ("b", "a"), Is.EqualTo (-1));
      Assert.That (deserializedComparer.Compare ("a", "a"), Is.EqualTo (0));
    }
  }
}