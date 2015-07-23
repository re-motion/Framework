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

using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Enumerables;

// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTests.Core.UnitTesting.Enumerables
{
  [TestFixture]
  public class OneTimeEnumerableTest
  {
    [Test]
    public void GetEnumerator_WorksLikeANormalEnumerable ()
    {
      var source1 = new[] { 1, 2, 3 };
      var source2 = new[] { 1, 3, 2 };

      Assert.That (new OneTimeEnumerable<int> (source1), Is.EqualTo (source1));
      Assert.That (new OneTimeEnumerable<int> (source1), Is.Not.EqualTo (source2));
      Assert.That (new OneTimeEnumerable<int> (source1), Is.EquivalentTo (source2));
    }

    [Test]
    public void GetEnumerator_ThrowsForSecondCall ()
    {
      var source = Enumerable.Range(1, 3);

      var oneTime = new OneTimeEnumerable<int> (source);

      Assert.That (() => oneTime.GetEnumerator(), Throws.Nothing);
      Assert.That (
          () => oneTime.GetEnumerator(),
          Throws.TypeOf<InvalidOperationException> ().With.Message.EqualTo ("OneTimeEnumerable can only be iterated once."));
    }

    [Test]
    public void GetEnumerator_ReturnedEnumeratorDoesNotSupportReset ()
    {
      var source = new[] { 1, 2, 3 };

      var oneTimeEnumerator = new OneTimeEnumerable<int> (source).GetEnumerator();
      oneTimeEnumerator.MoveNext();

      Assert.That (
          () => oneTimeEnumerator.Reset(),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo ("OneTimeEnumerator does not support Reset()."));
    }
  }
}