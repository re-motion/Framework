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
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class DelegateBasedEqualityComparerTest
  {
    private DelegateBasedEqualityComparer<object> _comparer;
    private object _obj1;
    private object _obj2;

    [SetUp]
    public void SetUp ()
    {
      _comparer = new DelegateBasedEqualityComparer<object> ((o1, o2) => o1 == o2, o => o.GetHashCode ());
      _obj1 = new Object ();
      _obj2 = new Object ();
    }

    [Test]
    public void Equals_DifferentObjects ()
    {
      Assert.That (_comparer.Equals (_obj1, _obj2), Is.False);
    }

    [Test]
    public void Equals_SameObjects ()
    {
      Assert.That (_comparer.Equals(_obj1, _obj1), Is.True);
    }

    [Test]
    public void Equals_ObjectWithNull ()
    {
      Assert.That (_comparer.Equals (_obj1, null), Is.False);
    }

    [Test]
    public void Equals_NullWithNull ()
    {
      Assert.That (_comparer.Equals (null, null), Is.True);
    }

    [Test]
    public void GetHashcode ()
    {
      Assert.That (_comparer.GetHashCode (_obj1), Is.EqualTo (_comparer.GetHashCode (_obj1)));
    }
    
  }
}