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

namespace Remotion.UnitTests.Reflection.PropertyInfoExtensionsTests
{
  [TestFixture]
  public class GuessIsExplicitInterfaceProperty
  {
    public interface Interface
    {
      int Property01 { get; }
      int Property02 { get; }
      int Property03 { get; }
      int Property04 { get; }

      int Property05 { set; }
      int Property06 { set; }
      int Property07 { set; }
      int Property08 { set; }

      int Property09 { get; set; }
      int Property10 { get; set; }
      int Property11 { get; set; }
    }

    public class ClassWithInterfaceProperties : Interface
    {
      public int Property01
      {
        get { throw new NotImplementedException(); }
      }

      public int Property02
      {
        get { throw new NotImplementedException(); }
        private set { throw new NotImplementedException(); }
      }

      int Interface.Property03
      {
        get { throw new NotImplementedException(); }
      }

      public virtual int Property04
      {
        get { throw new NotImplementedException(); }
      }

      public int Property05
      {
        set { throw new NotImplementedException(); }
      }

      public int Property06
      {
        set { throw new NotImplementedException(); }
        private get { throw new NotImplementedException(); }
      }

      int Interface.Property07
      {
        set { throw new NotImplementedException(); }
      }

      public virtual int Property08
      {
        set { throw new NotImplementedException(); }
      }

      public int Property09
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      int Interface.Property10
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      public virtual int Property11
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }
    }

    public class ClassWithoutInterfaceProperties
    {
      public int Property01
      {
        get { throw new NotImplementedException(); }
      }

      public int Property02
      {
        get { throw new NotImplementedException(); }
        private set { throw new NotImplementedException(); }
      }

      private int Property03
      {
        get { throw new NotImplementedException(); }
      }

      public virtual int Property04
      {
        get { throw new NotImplementedException(); }
      }

      public int Property05
      {
        set { throw new NotImplementedException(); }
      }

      public int Property06
      {
        set { throw new NotImplementedException(); }
        private get { throw new NotImplementedException(); }
      }

      private int Property07
      {
        set { throw new NotImplementedException(); }
      }

      public virtual int Property08
      {
        set { throw new NotImplementedException(); }
      }

      public int Property09
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      private int Property10
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      public virtual int Property11
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }
    }

    [Test]
    public void NonInterfaceProperties ()
    {
      Type type = typeof (ClassWithoutInterfaceProperties);
      PropertyInfo[] properties = type.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That (properties.Length, Is.EqualTo (11));

      foreach (PropertyInfo property in properties)
        Assert.That (property.GuessIsExplicitInterfaceProperty(), Is.False);
    }

    [Test]
    public void InterfaceProperties ()
    {
      Type type = typeof (ClassWithInterfaceProperties);
      PropertyInfo[] properties = type.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That (properties.Length, Is.EqualTo (11));

      Array.Sort (properties, delegate (PropertyInfo one, PropertyInfo two)
      {
        return GetShortName (one.Name).CompareTo (GetShortName (two.Name));
      });

      Assert.That (properties[0].Name, Is.EqualTo ("Property01"));
      Assert.That (properties[1].Name, Is.EqualTo ("Property02"));
      Assert.That (properties[2].Name, Is.EqualTo ("Remotion.UnitTests.Reflection.PropertyInfoExtensionsTests.GuessIsExplicitInterfaceProperty.Interface.Property03"));

      Assert.That (properties[0].GuessIsExplicitInterfaceProperty(), Is.False);
      Assert.That (properties[1].GuessIsExplicitInterfaceProperty(), Is.False);
      Assert.That (properties[2].GuessIsExplicitInterfaceProperty(), Is.True);
      Assert.That (properties[3].GuessIsExplicitInterfaceProperty(), Is.False);
      Assert.That (properties[4].GuessIsExplicitInterfaceProperty(), Is.False);
      Assert.That (properties[5].GuessIsExplicitInterfaceProperty(), Is.False);
      Assert.That (properties[6].GuessIsExplicitInterfaceProperty(), Is.True);
      Assert.That (properties[7].GuessIsExplicitInterfaceProperty(), Is.False);
      Assert.That (properties[8].GuessIsExplicitInterfaceProperty(), Is.False);
      Assert.That (properties[9].GuessIsExplicitInterfaceProperty(), Is.True);
      Assert.That (properties[10].GuessIsExplicitInterfaceProperty(), Is.False);
    }

    private string GetShortName (string name)
    {
      return name.Substring (name.LastIndexOf ('.') + 1);
    }
  }
}
