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
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Reflection
{
  [TestFixture]
  public class TypeModifierUtilityTest
  {
    private TypeModifierUtility _typeModifierUtility;

    [SetUp]
    public void SetUp ()
    {
      _typeModifierUtility = new TypeModifierUtility();
    }


    [Test]
    public void GetTypeModifiers_Visibility ()
    {
      Assert.That(_typeModifierUtility.GetTypeModifiers(typeof(TypeModifierTestClass)), Is.EqualTo("public"));
      Assert.That(
          _typeModifierUtility.GetTypeModifiers(typeof(TypeModifierTestClass).GetNestedType("ProtectedClass", BindingFlags.Instance | BindingFlags.NonPublic)),
          Is.EqualTo("protected"));
      Assert.That(_typeModifierUtility.GetTypeModifiers(typeof(TypeModifierTestClass.ProtectedInternalClass)), Is.EqualTo("protected internal"));
      Assert.That(_typeModifierUtility.GetTypeModifiers(typeof(TypeModifierTestClass.InternalClass)), Is.EqualTo("internal"));
      Assert.That(
          _typeModifierUtility.GetTypeModifiers(typeof(TypeModifierTestClass).GetNestedType("PrivateClass", BindingFlags.Instance | BindingFlags.NonPublic)),
          Is.EqualTo("private"));
      Assert.That(_typeModifierUtility.GetTypeModifiers(typeof(TopLevelInternalClass)), Is.EqualTo("internal"));
    }

    [Test]
    public void GetTypeModifiers_Sealed ()
    {
      Assert.That(_typeModifierUtility.GetTypeModifiers(typeof(PublicSealedClass)), Is.EqualTo("public sealed"));
      // struct is sealed by default
      Assert.That(_typeModifierUtility.GetTypeModifiers(typeof(TypeModifierTestClass.PublicStruct)), Is.EqualTo("public sealed"));
    }

    [Test]
    public void GetTypeModifiers_Abstract ()
    {
      Assert.That(_typeModifierUtility.GetTypeModifiers(typeof(PublicAbstractClass)), Is.EqualTo("public abstract"));
    }
  }
}
