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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.FunctionalProgramming;
using Remotion.UnitTests.FunctionalProgramming.TestDomain;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.FunctionalProgramming
{
  [TestFixture]
  public class EnumerableUtilityTest
  {
    [Test]
    public void Combine ()
    {
      var combined = EnumerableUtility.Combine (new[] {1, 2, 3}, new List<int> (new[] {3, 4, 5}));
      Assert.That (combined.ToArray (), Is.EqualTo (new[] { 1, 2, 3, 3, 4, 5 }));
    }

    [Test]
    public void Singleton_WithObject ()
    {
      var element = new object ();
      var actual = EnumerableUtility.Singleton (element);
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { element }));
    }

    [Test]
    public void Singleton_WithNull ()
    {
      var actual = EnumerableUtility.Singleton (((object) null));
      Assert.That (actual.ToArray (), Is.EqualTo (new object[] { null }));
    }

    [Test]
    public void Singleton_WithValueType ()
    {
      var actual = EnumerableUtility.Singleton (0);
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { 0 }));
    }

    [Test]
    public void SelectRecursiveDepthFirst_Single ()
    {
      var item = new RecursiveItem();

      var result = EnumerableUtility.SelectRecursiveDepthFirst (item, i => i.Children).ToArray();

      Assert.That (result, Is.EqualTo (new[] { item }));
    }

    [Test]
    public void SelectRecursiveDepthFirst_Nested ()
    {
      var item0a = new RecursiveItem ();
      var item0b = new RecursiveItem();
      var item0c = new RecursiveItem();
      var item1a = new RecursiveItem (item0a, item0b);
      var item1b = new RecursiveItem();
      var item1c = new RecursiveItem(item0c);
      var item2a = new RecursiveItem (item1a);
      var item2b = new RecursiveItem (item1b, item1c);
      var item3 = new RecursiveItem (item2a, item2b);

      var result = EnumerableUtility.SelectRecursiveDepthFirst (item3, i => i.Children).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { item3, item2a, item1a, item0a, item0b, item2b, item1b, item1c, item0c }));
    }
  }
}