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
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class EnumWrapperTest
  {
    public enum TestEnum { One, Two, Three }
    [Flags]
    public enum TestFlags { One, Two, Three }

    [Test]
    public void Initialization_FromEnumValue ()
    {
      EnumWrapper wrapper = EnumWrapper.Get(TestEnum.One);
      Assert.That(wrapper.Name, Is.EqualTo("One|Remotion.Security.UnitTests.EnumWrapperTest+TestEnum, Remotion.Security.UnitTests"));
    }

    [Test]
    public void Initialization_FromFlags_Fails ()
    {
      Assert.That(
          () => EnumWrapper.Get(TestFlags.One),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Enumerated type 'Remotion.Security.UnitTests.EnumWrapperTest+TestFlags' "
                  + "cannot be wrapped. Only enumerated types without the System.FlagsAttribute can be wrapped.", "enumValue"));
    }

    [Test]
    public void Initialization_FromString ()
    {
      EnumWrapper wrapper = EnumWrapper.Get("123");
      Assert.That(wrapper.Name, Is.EqualTo("123"));
    }

    [Test]
    public void Initialization_FromStrings ()
    {
      EnumWrapper wrapper = EnumWrapper.Get("123", "456");
      Assert.That(wrapper.Name, Is.EqualTo("123|456"));
    }

    [Test]
    public void Equals_True ()
    {
      Assert.That(EnumWrapper.Get("123").Equals((object)EnumWrapper.Get("123")), Is.True);
    }

    [Test]
    public void Equals_False ()
    {
      Assert.That(EnumWrapper.Get("123").Equals((object)EnumWrapper.Get("321")), Is.False);
    }

    [Test]
    public void Equals_False_WithNull ()
    {
      Assert.That(EnumWrapper.Get("123").Equals(null), Is.False);
    }

    [Test]
    public void Equals_False_WithDifferentType ()
    {
      Assert.That(EnumWrapper.Get("123").Equals("123"), Is.False);
    }

    [Test]
    public void Equatable_Equals_True ()
    {
      Assert.That(EnumWrapper.Get("123").Equals(EnumWrapper.Get("123")), Is.True);
    }

    [Test]
    public void Equatable_Equals_False ()
    {
      Assert.That(EnumWrapper.Get("123").Equals(EnumWrapper.Get("321")), Is.False);
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      EnumWrapper one = EnumWrapper.Get("123");
      EnumWrapper two = EnumWrapper.Get("123");
      Assert.That(one.GetHashCode(), Is.EqualTo(two.GetHashCode()));
    }

    [Test]
    public void ConvertToString_SimpleName ()
    {
      EnumWrapper one = EnumWrapper.Get("123");
      Assert.That(one.ToString(), Is.EqualTo(one.Name));
    }

    [Test]
    public void ConvertToString_WithTypeName ()
    {
      EnumWrapper wrapper = EnumWrapper.Get("Name", "Namespace.TypeName, Assembly");

      Assert.That(wrapper.ToString(), Is.EqualTo("Name|Namespace.TypeName, Assembly"));
    }
  }
}
