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
using System.Collections;
using NUnit.Framework;
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class EqualityUtilityTest
  {
    [Test]
    public void GetRotatedHashCode_ForEnumerable()
    {
      IEnumerable objects1 = new int[] {1, 2, 3};
      IEnumerable objects2 = new int[] {1, 2, 3};
      Assert.That (EqualityUtility.GetRotatedHashCode (objects2), Is.EqualTo (EqualityUtility.GetRotatedHashCode (objects1)));

      IEnumerable objects3 = new int[] {3, 2, 1};
      Assert.That (EqualityUtility.GetRotatedHashCode (objects3), Is.Not.EqualTo (EqualityUtility.GetRotatedHashCode (objects1)));

      IEnumerable objects4 = new int[] { 1, 2, 17 };
      Assert.That (EqualityUtility.GetRotatedHashCode (objects4), Is.Not.EqualTo (EqualityUtility.GetRotatedHashCode (objects1)));
    }

    [Test]
    public void GetXorHashCode ()
    {
      IEnumerable objects1 = new int[] { 1, 2, 3 };
      IEnumerable objects2 = new int[] { 1, 2, 3 };
      Assert.That (EqualityUtility.GetXorHashCode (objects2), Is.EqualTo (EqualityUtility.GetXorHashCode (objects1)));

      IEnumerable objects3 = new int[] { 3, 2, 1 };
      Assert.That (EqualityUtility.GetXorHashCode (objects3), Is.EqualTo (EqualityUtility.GetXorHashCode (objects1)));

      IEnumerable objects4 = new int[] { 1, 2, 17 };
      Assert.That (EqualityUtility.GetXorHashCode (objects4), Is.Not.EqualTo (EqualityUtility.GetXorHashCode (objects1)));
    }

    [Test]
    public void GetRotatedHashCode_Nulls ()
    {
      var array1 = new object[] { 1, null, 2 };
      var array2 = new object[] { 1, null, 2 };
      var array3 = new object[] { 1, null, null, 2 };

      Assert.That (EqualityUtility.GetRotatedHashCode (array1), Is.EqualTo (EqualityUtility.GetRotatedHashCode (array2)));
      Assert.That (EqualityUtility.GetRotatedHashCode (array1), Is.Not.EqualTo (EqualityUtility.GetRotatedHashCode (array3)));

      Assert.That (EqualityUtility.GetRotatedHashCode ((IEnumerable) array1), Is.EqualTo (EqualityUtility.GetRotatedHashCode ((IEnumerable) array2)));
      Assert.That (EqualityUtility.GetRotatedHashCode ((IEnumerable) array1), Is.Not.EqualTo (EqualityUtility.GetRotatedHashCode ((IEnumerable) array3)));

      Assert.That (EqualityUtility.GetRotatedHashCode (array1), Is.EqualTo (EqualityUtility.GetRotatedHashCode ((IEnumerable) array1)));
      Assert.That (EqualityUtility.GetRotatedHashCode (array3), Is.EqualTo (EqualityUtility.GetRotatedHashCode ((IEnumerable) array3)));
    }
  }
}
