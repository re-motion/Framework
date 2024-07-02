// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTests.Core.UnitTesting.ObjectMothers
{
  [TestFixture]
  public class ObjectMotherTest
  {
    [Test]
    public void NewListTest ()
    {
      var collection = ListObjectMother.New(7, 11, 13, 17);
      var collectionExpected = new List<int> { 7, 11, 13, 17 };
      Assert.That(collection, Is.EqualTo(collectionExpected));
    }

    [Test]
    public void NewQueueTest ()
    {
      var collection = QueueObjectMother.New(7, 11, 13, 17);
      var collectionExpected = new Queue<int>();
      collectionExpected.Enqueue(7);
      collectionExpected.Enqueue(11);
      collectionExpected.Enqueue(13);
      collectionExpected.Enqueue(17);
      Assert.That(collection, Is.EqualTo(collectionExpected));
    }


    [Test]
    public void NewDictionary1Test ()
    {
      var collection = DictionaryObjectMother.New("B", 2);
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["B"] = 2;
      Assert.That(collection, Is.EquivalentTo(collectionExpected));
    }

    [Test]
    public void NewDictionary2Test ()
    {
      var collection = DictionaryObjectMother.New("B", 2, "D", 4);
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["B"] = 2;
      collectionExpected["D"] = 4;
      Assert.That(collection, Is.EquivalentTo(collectionExpected));
    }

    [Test]
    public void NewDictionary3Test ()
    {
      var collection = DictionaryObjectMother.New("B", 2, "D", 4, "C", 3);
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["B"] = 2;
      collectionExpected["C"] = 3;
      collectionExpected["D"] = 4;
      Assert.That(collection, Is.EquivalentTo(collectionExpected));
    }

    [Test]
    public void NewDictionaryTest4 ()
    {
      var collection = DictionaryObjectMother.New("B", 2, "D", 4, "C", 3, "A", 1);
      //var collectionExpected = new Queue<int> { 7, 11, 13, 17 };
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["A"] = 1;
      collectionExpected["B"] = 2;
      collectionExpected["C"] = 3;
      collectionExpected["D"] = 4;
      Assert.That(collection, Is.EquivalentTo(collectionExpected));
    }


  }
}
