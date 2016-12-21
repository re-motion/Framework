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
using System.Linq;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting.ObjectMothers;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ComponentwiseEqualsAndHashcodeWrapperTest
  {
    [Test]
    public void CtorTest ()
    {
      var array = new object[0];
      var wrapper = new ComponentwiseEqualsAndHashcodeWrapper<object> (array);
      Assert.That (wrapper.Enumerable,Is.EqualTo(array));
    }


    [Test]
    public void FactoryTest ()
    {
      var array = new object[0];
      var wrapper = ComponentwiseEqualsAndHashcodeWrapper.New (array);
      Assert.That (wrapper.Enumerable, Is.EqualTo (array));
    }


    [Test]
    public void EnumerableTest ()
    {
      var enumerable = ListObjectMother.New (9, 7, 22, 1);
      var enumerableWrapped = NewEnumerableEqualsWrapper (enumerable);
      Assert.That (enumerableWrapped.Cast<object> ().SequenceEqual (enumerable.Cast<object> ()));
    }


    [Test]
    public void EqualsAndGetHashCodeTest ()
    {
      var sequence0 = new [] {  new ComparableTestClass_EnumerableEqualsWrapper (7), new ComparableTestClass_EnumerableEqualsWrapper (11), new ComparableTestClass_EnumerableEqualsWrapper (13) };
      var sequence1 = new [] {  new ComparableTestClass_EnumerableEqualsWrapper (7), new ComparableTestClass_EnumerableEqualsWrapper (11), new ComparableTestClass_EnumerableEqualsWrapper (13) };
      var sequence2 = new [] {  new ComparableTestClass_EnumerableEqualsWrapper (7), new ComparableTestClass_EnumerableEqualsWrapper (11), new ComparableTestClass_EnumerableEqualsWrapper (13), new ComparableTestClass_EnumerableEqualsWrapper (17) };
      var sequence3 = new [] {  new ComparableTestClass_EnumerableEqualsWrapper (7), new ComparableTestClass_EnumerableEqualsWrapper (11), new ComparableTestClass_EnumerableEqualsWrapper (14) };
      var sequence4 = new [] {  new ComparableTestClass_EnumerableEqualsWrapper (7), new ComparableTestClass_EnumerableEqualsWrapper (11), new ComparableTestClass_EnumerableEqualsWrapper (13), new ComparableTestClass_EnumerableEqualsWrapper (17) };

      Assert.That (NewEnumerableEqualsWrapper (sequence0).Equals (null), Is.False);

      var sequenceWrapped0 = NewEnumerableEqualsWrapper (sequence0);
      var sequenceWrapped1 = NewEnumerableEqualsWrapper (sequence1);
      var sequenceWrapped2 = NewEnumerableEqualsWrapper (sequence2);
      var sequenceWrapped3 = NewEnumerableEqualsWrapper (sequence3);
      var sequenceWrapped4 = NewEnumerableEqualsWrapper (sequence4);

      var numbers0 = new[] { 7, 11, 13 };

      // Even though the ComparableTestClass incorrectly implements IEquatable<int> 
      // (IEquatable<> must only be used in the combination "class T : IEquatable<T>"; seeMSDN help) 
      // the sequences should symetrically not compare equal.

      Assert.That (sequenceWrapped0.Equals (numbers0), Is.False);
      Assert.That (NewEnumerableEqualsWrapper (numbers0).Equals (sequence0), Is.False);

      Assert.That (sequenceWrapped0.Equals (new[] { 7, 12, 13 }), Is.False);

      Assert.That (sequenceWrapped0.Equals (sequence0), Is.False);

      Assert.That (sequenceWrapped0.Equals (sequenceWrapped0), Is.True);

      Assert.That (sequenceWrapped0.Equals (sequence1), Is.False);

      Assert.That (sequenceWrapped0.Equals (sequenceWrapped2), Is.False);
      Assert.That (sequenceWrapped0.GetHashCode (), Is.EqualTo (sequenceWrapped1.GetHashCode ()));

      Assert.That (sequenceWrapped3.Equals (sequenceWrapped0), Is.False);
      Assert.That (sequenceWrapped0.Equals (sequenceWrapped3), Is.False);

      Assert.That (sequenceWrapped2.Equals (sequence4), Is.False);
      Assert.That (sequenceWrapped4.Equals (sequence2), Is.False);

      Assert.That (sequenceWrapped2.Equals (sequenceWrapped4), Is.True);
      Assert.That (sequenceWrapped4.Equals (sequenceWrapped2), Is.True);
      Assert.That (sequenceWrapped2.GetHashCode (), Is.EqualTo (sequenceWrapped4.GetHashCode ()));
    }

    private ComponentwiseEqualsAndHashcodeWrapper<T> NewEnumerableEqualsWrapper<T> (IEnumerable<T> elements)
    {
      return new ComponentwiseEqualsAndHashcodeWrapper<T> (elements);
    }
  }

  internal class ComparableTestClass_EnumerableEqualsWrapper : IEquatable<int>
  {
    public int Number;

    public ComparableTestClass_EnumerableEqualsWrapper () { }

    public ComparableTestClass_EnumerableEqualsWrapper (int number)
    {
      Number = number;
    }

    public bool Equals (int other)
    {
      return Number == other;
    }

    public override bool Equals (object obj)
    {
      if (obj is ComparableTestClass_EnumerableEqualsWrapper)
      {
        if (Object.Equals (((ComparableTestClass_EnumerableEqualsWrapper) obj).Number, Number))
        {
          return true;
        }
      }
    
      return false;
    }


    public override int GetHashCode ()
    {
      return Number;
    }
  }


}
