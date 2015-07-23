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
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class CompoundValueEqualityComparerTest
  {
    [Test]
    public void CtorTest ()
    {
      var comparer = 
        new CompoundValueEqualityComparer<CompoundValueEqualityComparerTestClass> (x => (new object[] { x.Number, x.TestClass, x.Text, x.Text2 }));

      var y = new CompoundValueEqualityComparerTestClass { Text = "changed1" };
      Assert.That (comparer.GetEqualityParticipatingObjects(y), Is.EqualTo (new object[] { y.Number, y.TestClass, y.Text, y.Text2 }));
    }

    [Test]
    public void EqualsTest ()
    {
      var comparer = new CompoundValueEqualityComparer<CompoundValueEqualityComparerTestClass> (x => (new object[] { x.Number, x.TestClass, x.Text, x.Text2 }));
      var comparerIgnoreText = new CompoundValueEqualityComparer<CompoundValueEqualityComparerTestClass> (x => (new object[] { x.Number, x.TestClass, x.Text2 }));
      var testClass = new CompoundValueEqualityComparerTestClass ();
      var testClassCopy = new CompoundValueEqualityComparerTestClass ();
      var testClass2 = new CompoundValueEqualityComparerTestClass { Text = "changed1" };

      Assert.That (comparer.Equals (testClass, testClassCopy), Is.True);
      Assert.That (comparer.Equals (testClass, testClass2), Is.False);

      Assert.That (comparerIgnoreText.Equals (testClass, testClassCopy), Is.True);
      Assert.That (comparerIgnoreText.Equals (testClass, testClass2), Is.True);
    }


    [Test]
    public void EqualsObjectTest ()
    {
      var comparer = new CompoundValueEqualityComparer<CompoundValueEqualityComparerTestClass> (x => (new object[] { x.Number, x.TestClass, x.Text, x.Text2 }));
      var testClass = new CompoundValueEqualityComparerTestClass ();
      var testClassCopy = new CompoundValueEqualityComparerTestClass ();
      var testClass2 = new CompoundValueEqualityComparerTestClass { Text = "changed1" };
      Object obj = new Object ();
      const Object objNull = null;

      Assert.That (comparer.Equals (testClass, objNull), Is.False);
      Assert.That (comparer.Equals (testClass, obj), Is.False);
      Assert.That (comparer.Equals (testClass, (object) testClass), Is.True);
      Assert.That (comparer.Equals (testClass, (object) testClassCopy), Is.True);
      Assert.That (comparer.Equals (testClass, (object) testClass2), Is.False);
    }


    [Test]
    public void GetHashCodeConsistencyTest ()
    {
      var comparer = new CompoundValueEqualityComparer<CompoundValueEqualityComparerTestClass> (x => (new object[] { x.Number, x.TestClass, x.Text, x.Text2 }));
      var comparerIgnoreText = new CompoundValueEqualityComparer<CompoundValueEqualityComparerTestClass> (x => (new object[] { x.Number, x.TestClass, x.Text2 }));
      var testClass = new CompoundValueEqualityComparerTestClass { Number = 987654, TestClass = new ComparableTestClass(34567), Text = "the", Text2 = "quick"};
      var testClassCopy = new CompoundValueEqualityComparerTestClass { Number = 987654, TestClass = new ComparableTestClass (34567), Text = "the", Text2 = "quick" };
      var testClass2 = new CompoundValueEqualityComparerTestClass { Number = 987654, TestClass = new ComparableTestClass (34567), Text = "Wabra", Text2 = "quick" };

      Assert.That (comparer.Equals (testClass, testClassCopy), Is.True);
      Assert.That (comparer.Equals (testClass, testClass2), Is.False);
      Assert.That (comparer.GetHashCode (testClass), Is.EqualTo (comparer.GetHashCode (testClassCopy)));
      Assert.That (comparer.GetHashCode (testClass), Is.Not.EqualTo (comparer.GetHashCode (testClass2)));

      Assert.That (comparerIgnoreText.Equals (testClass, testClassCopy), Is.True);
      Assert.That (comparerIgnoreText.Equals (testClass, testClass2), Is.True);
      Assert.That (comparerIgnoreText.GetHashCode (testClass), Is.EqualTo (comparerIgnoreText.GetHashCode (testClassCopy)));
      Assert.That (comparerIgnoreText.GetHashCode (testClass), Is.EqualTo (comparerIgnoreText.GetHashCode (testClass2)));
    }

  }


  internal class CompoundValueEqualityComparerTestClass
  {
    public string Text = "1st";
    public string Text2 = "2nd";
    public int Number = 71;
    public ComparableTestClass TestClass = new ComparableTestClass (111);
  }


  internal class ComparableTestClass
  {
    public int Number;

    public ComparableTestClass () {}

    public ComparableTestClass (int number)
    {
      Number = number;
    }

    public override bool Equals (object obj)
    {
      if(obj is ComparableTestClass)
      {
        if(Object.Equals(((ComparableTestClass) obj).Number,Number))
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
