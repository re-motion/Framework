// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

//
using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class ReferenceEqualityComparerTest
  {
    private ReferenceEqualityComparer<object> _comparer;
    private object _obj1;
    private object _obj2;

    [SetUp]
    public void SetUp ()
    {
      _comparer = ReferenceEqualityComparer<object>.Instance;
      _obj1 = new Object ();
      _obj2 = new Object ();
    }

    [Test]
    public void Equal_True ()
    {
      Assert.That (_comparer.Equals (_obj1, _obj1), Is.True);
    }

    [Test]
    public void Equal_False ()
    {
      Assert.That (_comparer.Equals (_obj1, _obj2), Is.False);
    }

    [Test]
    public void GetHashcode ()
    {
      Assert.That (_comparer.GetHashCode (_obj1), Is.EqualTo (_comparer.GetHashCode (_obj1)));
    }
  }
}