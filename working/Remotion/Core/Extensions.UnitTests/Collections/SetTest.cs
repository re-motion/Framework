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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Extensions.UnitTests.Collections
{
  [TestFixture]
  [Obsolete ("1.13.185.0")]
  public class SetTest
  {
    [Test]
    public void SetInitialization()
    {
      Set<int> set1 = new Set<int>();
      Assert.That (set1.Count, Is.EqualTo (0));
      Assert.That (set1.Contains (1), Is.False);
      Assert.That (set1.Contains (2), Is.False);
      Assert.That (set1.Contains (3), Is.False);
      Assert.That (set1.Contains (4), Is.False);

      Set<int> set2 = new Set<int> (new int[] {1, 2, 3});
      Assert.That (set2.Count, Is.EqualTo (3));
      Assert.That (set2.Contains (1), Is.True);
      Assert.That (set2.Contains (2), Is.True);
      Assert.That (set2.Contains (3), Is.True);
      Assert.That (set2.Contains (4), Is.False);

      Set<string> set3 = new Set<string> (new string[] { "1", "2", "3", "3", "1", "2", "1", "2", "4", "2", "4" });
      Assert.That (set3.Count, Is.EqualTo (4));
      Assert.That (set3.Contains ("1"), Is.True);
      Assert.That (set3.Contains ("2"), Is.True);
      Assert.That (set3.Contains ("3"), Is.True);
      Assert.That (set3.Contains ("4"), Is.True);
      Assert.That (set3.Contains ("5"), Is.False);
    }

    [Test]
    public void AddAndAddRangeAndRemove()
    {
      Set<int> set1 = new Set<int> ();
      Assert.That (set1.Count, Is.EqualTo (0));

      set1.Add (0);
      Assert.That (set1.Count, Is.EqualTo (1));

      set1.Add (0);
      Assert.That (set1.Count, Is.EqualTo (1));

      set1.Add (12);
      Assert.That (set1.Count, Is.EqualTo (2));

      set1.Remove (12);

      set1.AddRange (new int[] {1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 7});
      Assert.That (set1.Count, Is.EqualTo (8));

      Assert.That (set1.Contains (0), Is.True);
      Assert.That (set1.Contains (1), Is.True);
      Assert.That (set1.Contains (2), Is.True);
      Assert.That (set1.Contains (3), Is.True);
      Assert.That (set1.Contains (4), Is.True);
      Assert.That (set1.Contains (5), Is.True);
      Assert.That (set1.Contains (6), Is.True);
      Assert.That (set1.Contains (7), Is.True);
      Assert.That (set1.Contains (8), Is.False);

      set1.Remove (0);
      Assert.That (set1.Count, Is.EqualTo (7));
      Assert.That (set1.Contains (0), Is.False);
      Assert.That (set1.Contains (1), Is.True);
      Assert.That (set1.Contains (2), Is.True);
      Assert.That (set1.Contains (3), Is.True);
      Assert.That (set1.Contains (4), Is.True);
      Assert.That (set1.Contains (5), Is.True);
      Assert.That (set1.Contains (6), Is.True);
      Assert.That (set1.Contains (7), Is.True);
      Assert.That (set1.Contains (8), Is.False);

      set1.Remove (6);
      Assert.That (set1.Count, Is.EqualTo (6));
      Assert.That (set1.Contains (0), Is.False);
      Assert.That (set1.Contains (1), Is.True);
      Assert.That (set1.Contains (2), Is.True);
      Assert.That (set1.Contains (3), Is.True);
      Assert.That (set1.Contains (4), Is.True);
      Assert.That (set1.Contains (5), Is.True);
      Assert.That (set1.Contains (6), Is.False);
      Assert.That (set1.Contains (7), Is.True);
      Assert.That (set1.Contains (8), Is.False);
    }

    [Test]
    public void Clear()
    {
      Set<int> set1 = new Set<int> (new int[] { 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 7 });
      Assert.That (set1.Count, Is.EqualTo (8));
      set1.Clear();
      Assert.That (set1.Count, Is.EqualTo (0));
    }

    [Test]
    public void CopyToAndToArray()
    {
      Set<int> set1 = new Set<int> (new int[] { 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 7 });
      int[] array1 = set1.ToArray();
      int[] array2 = new int[set1.Count];
      set1.CopyTo (array2, 0);

      Assert.That (array1.Length, Is.EqualTo (8));
      Assert.That (array2.Length, Is.EqualTo (8));
      for (int i = 0; i < array1.Length; ++i)
        Assert.That (array2[i], Is.EqualTo (array1[i]));

      Assert.That (array1, Has.Member (0));
      Assert.That (array1, Has.Member (1));
      Assert.That (array1, Has.Member (2));
      Assert.That (array1, Has.Member (3));
      Assert.That (array1, Has.Member (4));
      Assert.That (array1, Has.Member (5));
      Assert.That (array1, Has.Member (6));
      Assert.That (array1, Has.Member (7));

      int[] array3 = new int[] {9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9};
      set1.CopyTo (array3, 2);

      Assert.That (array3, Has.Member (0));
      Assert.That (array3, Has.Member (1));
      Assert.That (array3, Has.Member (2));
      Assert.That (array3, Has.Member (3));
      Assert.That (array3, Has.Member (4));
      Assert.That (array3, Has.Member (5));
      Assert.That (array3, Has.Member (6));
      Assert.That (array3, Has.Member (7));

      Assert.That (array3[0], Is.EqualTo (9));
      Assert.That (array3[1], Is.EqualTo (9));
      Assert.That (array3[2], Is.Not.EqualTo (9));
      Assert.That (array3[9], Is.Not.EqualTo (9));
      Assert.That (array3[10], Is.EqualTo (9));
    }

    [Test]
    [ExpectedException(typeof(ArgumentException), ExpectedMessage = "not long enough", MatchType = MessageMatch.Contains)]
    public void CopyToThrowsOnInvalidArraySize()
    {
      int[] array = new int[1];
      Set<int> set = new Set<int> (new int[] { 1, 2 });

      set.CopyTo (array, 0);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "not long enough", MatchType = MessageMatch.Contains)]
    public void CopyToThrowsOnInvalidIndex1 ()
    {
      int[] array = new int[2];
      Set<int> set = new Set<int> (new int[] { 1, 2 });

      set.CopyTo (array, 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage = "negative number", MatchType = MessageMatch.Contains)]
    public void CopyToThrowsOnInvalidIndex2 ()
    {
      int[] array = new int[2];
      Set<int> set = new Set<int> (new int[] { 1, 2 });

      set.CopyTo (array, -1);
    }

    [Test]
    public void IsReadOnlyAlwaysFalse()
    {
      ICollection<int> coll = new Set<int>();
      Assert.That (coll.IsReadOnly, Is.False);
    }

    [Test]
    public void GetEnumerator()
    {
      Set<string> set1 = new Set<string> (new string[] {"a", "b", "a", "c"});
      List<string> list = new List<string>();
      foreach (string s in set1)
        list.Add (s);
      Assert.That (list.Count, Is.EqualTo (3));
      Assert.That (list, Has.Member ("a"));
      Assert.That (list, Has.Member ("b"));
      Assert.That (list, Has.Member ("c"));

      list.Clear();

      IEnumerable enumerable = set1;

      foreach (string s in enumerable)
        list.Add (s);
      Assert.That (list.Count, Is.EqualTo (3));
      Assert.That (list, Has.Member ("a"));
      Assert.That (list, Has.Member ("b"));
      Assert.That (list, Has.Member ("c"));
    }

    public class ToStringEqualityComparer<T> : IEqualityComparer<T>
    {
      public bool Equals (T x, T y)
      {
        if (x == null)
          return y == null;
        else
          return y != null && x.ToString() == y.ToString();
      }

      public int GetHashCode (T obj)
      {
        ArgumentUtility.CheckNotNull ("obj", obj);
        return obj.ToString().GetHashCode();
      }
    }

    [Test]
    public void SpecificComparer1()
    {
      Set<object> set1 = new Set<object> (new ToStringEqualityComparer<object>());
      set1.AddRange (new object[] {1, 2, "a", "1", "2"});

      Assert.That (set1.Count, Is.EqualTo (3));
      Assert.That (set1.Contains(1), Is.True);
      Assert.That (set1.Contains (2), Is.True);
      Assert.That (set1.Contains ("a"), Is.True);
      Assert.That (set1.Contains ("1"), Is.True);
      Assert.That (set1.Contains ("2"), Is.True);

      object[] array = set1.ToArray();
      Assert.That (array.Length, Is.EqualTo (3));
      Assert.That (array, Has.Member (1));
      Assert.That (array, Has.Member (2));
      Assert.That (array, Has.Member ("a"));
    }

    [Test]
    public void SpecificComparer2 ()
    {
      Set<object> set1 = new Set<object> (new object[] { 1, 2, "a", "1", "2" }, new ToStringEqualityComparer<object> ());

      Assert.That (set1.Count, Is.EqualTo (3));
      Assert.That (set1.Contains (1), Is.True);
      Assert.That (set1.Contains (2), Is.True);
      Assert.That (set1.Contains ("a"), Is.True);
      Assert.That (set1.Contains ("1"), Is.True);
      Assert.That (set1.Contains ("2"), Is.True);

      object[] array = set1.ToArray ();
      Assert.That (array.Length, Is.EqualTo (3));
      Assert.That (array, Has.Member (1));
      Assert.That (array, Has.Member (2));
      Assert.That (array, Has.Member ("a"));
    }

    [Test]
    public void SetIsSerializable()
    {
      Assert.That (typeof (Set<int>).IsSerializable, Is.True);

      Set<int> s1 = new Set<int> (new int[] {1, 2, 3, 1, 2});
      Set<int> s2 = Serializer.SerializeAndDeserialize (s1);
      Assert.That (s2, Is.Not.SameAs (s1));
      Assert.That (s2.Count, Is.EqualTo (s1.Count));

      foreach (int i in s1)
        Assert.That (s2.Contains (i), Is.True);

      foreach (int i in s2)
        Assert.That (s1.Contains (i), Is.True);
    }

    [Test]
    public void ICollectionCopyTo()
    {
      Set<int> set = new Set<int> (1, 2, 3, 4, 5, 6, 7, 8);
      object[] targetArray = new object[10];
      ((ICollection) set).CopyTo (targetArray, 1);
      Assert.That (targetArray[0], Is.EqualTo (null));
      Assert.That (targetArray[1], Is.EqualTo (1));
      Assert.That (targetArray[2], Is.EqualTo (2));
      Assert.That (targetArray[3], Is.EqualTo (3));
      Assert.That (targetArray[4], Is.EqualTo (4));
      Assert.That (targetArray[5], Is.EqualTo (5));
      Assert.That (targetArray[6], Is.EqualTo (6));
      Assert.That (targetArray[7], Is.EqualTo (7));
      Assert.That (targetArray[8], Is.EqualTo (8));
      Assert.That (targetArray[9], Is.EqualTo (null));
    }

    [Test]
    public void ICollectionIsSynchronized()
    {
      Set<int> set = new Set<int> (1, 2, 3, 4, 5, 6, 7, 8);
      Assert.That (((ICollection) set).IsSynchronized, Is.False);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ICollectionSyncRootThrows ()
    {
      Set<int> set = new Set<int> (1, 2, 3, 4, 5, 6, 7, 8);
      object root = ((ICollection) set).SyncRoot;
    }

    [Test]
    public void GetAny()
    {
      Set<int> set = new Set<int> (1, 2, 3, 4, 5, 6, 7, 8);
      Assert.That (set.Contains (set.GetAny()), Is.True);
      set = new Set<int> (1);
      Assert.That (set.GetAny (), Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void GetAnyThrowsWhenEmpty()
    {
      Set<int> set = new Set<int> ();
      set.GetAny();
    }
  }
}
