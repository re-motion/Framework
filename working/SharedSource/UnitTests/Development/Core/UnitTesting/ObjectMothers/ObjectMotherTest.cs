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
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;

// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTests.Core.UnitTesting.ObjectMothers
{
  [NUnit.Framework.TestFixture]
  public class ObjectMotherTest
  {
    [Test]
    public void NewListTest ()
    {
      var collection = ListObjectMother.New (7, 11, 13, 17);
      var collectionExpected = new List<int> { 7, 11, 13, 17 };
      Assert.That (collection, Is.EqualTo (collectionExpected));
    }

    [Test]
    public void NewQueueTest ()
    {
      var collection = QueueObjectMother.New (7, 11, 13, 17);
      var collectionExpected = new Queue<int>();
      collectionExpected.Enqueue (7);
      collectionExpected.Enqueue (11);
      collectionExpected.Enqueue (13);
      collectionExpected.Enqueue (17);
      Assert.That (collection, Is.EqualTo (collectionExpected));
    }


    [Test]
    public void NewDictionary1Test ()
    {
      var collection = DictionaryObjectMother.New ("B", 2);
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["B"] = 2;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }

    [Test]
    public void NewDictionary2Test ()
    {
      var collection = DictionaryObjectMother.New ("B", 2, "D", 4);
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["B"] = 2;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }

    [Test]
    public void NewDictionary3Test ()
    {
      var collection = DictionaryObjectMother.New ("B", 2, "D", 4, "C", 3);
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["B"] = 2;
      collectionExpected["C"] = 3;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }

    [Test]
    public void NewDictionaryTest4 ()
    {
      var collection = DictionaryObjectMother.New ("B", 2, "D", 4, "C", 3, "A", 1);
      //var collectionExpected = new Queue<int> { 7, 11, 13, 17 };
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["A"] = 1;
      collectionExpected["B"] = 2;
      collectionExpected["C"] = 3;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }


  }
}
