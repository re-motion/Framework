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
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class InheritanceHierarchyFilterTest
  {
    private class Base1Class
    {
    }

    private class Derived11Class : Base1Class
    {
    }

    private class Leaf111Class : Derived11Class
    {
    }

    private class Leaf112Class : Derived11Class
    {
    }

    private class Base2Class
    {
    }

    private class Derived21Class : Base2Class
    {
    }

    private class Derived22Class : Base2Class
    {
    }

    private class Leaf211Class : Derived21Class
    {
    }

    private class Leaf221Class : Derived22Class
    {
    }

    private class Base3Class<T>
    {
    }

    private class Leaf31Class : Base3Class<int>
    {
    }

    private class Leaf32Class<T> : Base3Class<T>
    {
    }

    [Test]
    public void name ()
    {
      Type[] types = typeof (InheritanceHierarchyFilterTest).GetNestedTypes (BindingFlags.NonPublic);
      InheritanceHierarchyFilter typeFilter = new InheritanceHierarchyFilter (types);

      Assert.That (
          typeFilter.GetLeafTypes(),
          Is.EqualTo (
              new Type[]
                  {
                      typeof (Leaf111Class), typeof (Leaf112Class),
                      typeof (Leaf211Class), typeof (Leaf221Class),
                      typeof (Leaf31Class), typeof (Leaf32Class<>)
                  }));
    }
  }
}
