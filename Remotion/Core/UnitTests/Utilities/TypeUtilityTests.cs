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
using Remotion.Utilities;

internal class RootType
{
  internal class RootNestedType
  {
  }
}

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class TypeUtilityTests
  {
    private class NestedType
    {
      internal class NestedNestedType
      {
      }
    }

    // ReSharper disable once UnusedTypeParameter
    private class NestedGenericType<T>
    {
      internal class NestedGenericNestedType
      {
      }

      internal class NestedGenericNestedGenericType<TQ>
      {
        // _x introduced to get rid of R# warning
        // ReSharper disable once UnusedMember.Local
        private Type _x = typeof(TQ);
      }
    }

    [Test]
    public void TestAbbreviatedTypeName ()
    {
      AssertTransformation(
          "Remotion.UnitTests::Utilities.TypeUtilityTests",
          "Remotion.UnitTests.Utilities.TypeUtilityTests, Remotion.UnitTests");
    }

    [Test]
    public void TestAbbreviatedTypeName_WithNestedType ()
    {
      AssertTransformation(
          "Remotion.UnitTests::Utilities.TypeUtilityTests+NestedType",
          "Remotion.UnitTests.Utilities.TypeUtilityTests+NestedType, Remotion.UnitTests");
    }

    [Test]
    public void TestAbbreviatedOpenGenericTypeName ()
    {
      AssertTransformation(
          "A.B::C.D`2",
          "A.B.C.D`2, A.B");
    }

    [Test]
    public void TestAbbreviatedClosedGenericTypeName ()
    {
      AssertTransformation(
          "A.B::C.D`2[a.b::c.d,System.String]",
          "A.B.C.D`2[[a.b.c.d, a.b],System.String], A.B");
    }

    [Test]
    public void TestAbbreviatedTypeArgumentWithOptionalBrackets ()
    {
      AssertTransformation(
          "Dictionary`2[[a.b::c.d],System.String]",
          "Dictionary`2[[a.b.c.d, a.b],System.String]");
    }

    [Test]
    public void TestClosedGenericTypeNameWithAbbreviatedTypeArgument ()
    {
      AssertTransformation(
          "A.B.C.D`2[a.b::c.d,System.String], A.B",
          "A.B.C.D`2[[a.b.c.d, a.b],System.String], A.B");
    }

    [Test]
    public void TestPartiallyAbbreviatedClosedGenericTypeName ()
    {
      AssertTransformation(
          "A.B::C.D`2[a.b::c.d,System.String], Version=1.0.0.0",
          "A.B.C.D`2[[a.b.c.d, a.b],System.String], A.B, Version=1.0.0.0");
    }

    [Test]
    public void TestAbbreviatedClosedGenericTypeWithPartiallyAbbreviatedTypeArgument ()
    {
      AssertTransformation(
          "A.B::C.D`2[[a.b::c.d, Version=1.0.0.0],System.String]",
          "A.B.C.D`2[[a.b.c.d, a.b, Version=1.0.0.0],System.String], A.B");
    }

    [Test]
    public void TestGetType ()
    {
      Type t = TypeUtility.GetType("Remotion.UnitTests::Utilities.TypeUtilityTests", true);
      Assert.That(t, Is.EqualTo(typeof(TypeUtilityTests)));
    }

    [Test]
    public void TestGetType_WithNestedNestedType ()
    {
      Type t = TypeUtility.GetType("Remotion.UnitTests::Utilities.TypeUtilityTests+NestedType+NestedNestedType", true);
      Assert.That(t, Is.EqualTo(typeof(NestedType.NestedNestedType)));
    }

    [Test]
    public void TestGetType_WithRootNestedType ()
    {
      Type t = TypeUtility.GetType("RootType+RootNestedType, Remotion.UnitTests", true);
      Assert.That(t, Is.EqualTo(typeof(RootType.RootNestedType)));
    }

    [Test]
    public void TestGetType_WithGenericAbbreviatedClosedNestedGenericType ()
    {
      Type t = TypeUtility.GetType(
          "Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1[Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1[[NUnit.Framework.TestAttribute, nunit.framework]]]",
          true);
      Assert.That(t, Is.EqualTo(typeof(NestedGenericType<NestedGenericType<TestAttribute>>)));
    }

    [Test]
    public void TestGetType_WithNestedGenericNestedType ()
    {
      Type t = TypeUtility.GetType("Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1+NestedGenericNestedType", true);
      Assert.That(t, Is.EqualTo(typeof(NestedGenericType<>.NestedGenericNestedType)));
    }

    [Test]
    public void TestGetType_WithNestedGenericNestedGenericType ()
    {
      Type t = TypeUtility.GetType("Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1+NestedGenericNestedGenericType`1", true);
      Assert.That(t, Is.EqualTo(typeof(NestedGenericType<>.NestedGenericNestedGenericType<>)));
    }

    [Test]
    public void TestGetType_WithClosedNestedGenericNestedGenericType ()
    {
      Type t = TypeUtility.GetType(
          "Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1+NestedGenericNestedGenericType`1[[NUnit.Framework.TestAttribute, nunit.framework], [NUnit.Framework.TestCaseData, nunit.framework]]",
          true);
      Assert.That(t, Is.EqualTo(typeof(NestedGenericType<TestAttribute>.NestedGenericNestedGenericType<TestCaseData>)));
    }

    [Test]
    public void TestNestedUnqualified ()
    {
      AssertTransformation("a::b[c::d,e::f,g::h]",
                            "a.b[[c.d, c],[e.f, e],[g.h, g]], a");
    }

    [Test]
    public void TestNestedOptionalBrackets ()
    {
      AssertTransformation("a::b[[c::d],[e::f],[g::h]]",
                            "a.b[[c.d, c],[e.f, e],[g.h, g]], a");
    }

    [Test]
    public void TestNestedQualified ()
    {
      AssertTransformation("a::b[[c::d, ver=1],[e::f, ver=2],[g::h, ver=3]], ver=4",
                            "a.b[[c.d, c, ver=1],[e.f, e, ver=2],[g.h, g, ver=3]], a, ver=4");
    }


    [Test]
    public void TestNestedWithArgsUnqualified ()
    {
      AssertTransformation("a::b[c::d[...],e::f[...],g::h[...]]",
                            "a.b[[c.d[...], c],[e.f[...], e],[g.h[...], g]], a");
    }

    [Test]
    public void TestNestedWithArgsOptionalBrackets ()
    {
      AssertTransformation("a::b[[c::d[...]],[e::f[...]],[g::h[...]]]",
                            "a.b[[c.d[...], c],[e.f[...], e],[g.h[...], g]], a");
    }

    [Test]
    public void TestNestedWithArgsQualified ()
    {
      AssertTransformation("a::b[[c::d[...], ver=1],[e::f[...], ver=2],[g::h[...], ver=3]], ver=4",
                            "a.b[[c.d[...], c, ver=1],[e.f[...], e, ver=2],[g.h[...], g, ver=3]], a, ver=4");
    }

    [Test]
    public void TestDeepNestedWithArgsUnqualified ()
    {
      AssertTransformation("a::b[c::d[e::f[g::h[...]]]]",
                            "a.b[[c.d[[e.f[[g.h[...], g]], e]], c]], a");
    }

    [Test]
    public void TestDeepNestedWithArgsOptionalBrackets ()
    {
      AssertTransformation("a::b[[c::d[[e::f[[g::h[...]]]]]]]",
                            "a.b[[c.d[[e.f[[g.h[...], g]], e]], c]], a");
    }

    [Test]
    public void TestDeepNestedWithArgsQualified ()
    {
      AssertTransformation("a::b[[c::d[[e::f[[g::h[...], ver=1]], ver=2]], ver=3]], ver=4",
                            "a.b[[c.d[[e.f[[g.h[...], g, ver=1]], e, ver=2]], c, ver=3]], a, ver=4");
    }

    [Test]
    public void TestDeepNestedWithArgsWithStrongName ()
    {
      AssertTransformation("a::b[[c::d[[e::f[[g::h[...], ver=1, token=2]], ver=2, token=3]], ver=3, token=4]], ver=4, token=5",
                            "a.b[[c.d[[e.f[[g.h[...], g, ver=1, token=2]], e, ver=2, token=3]], c, ver=3, token=4]], a, ver=4, token=5");
    }

    [Test]
    public void GetAbbreviatedTypeName_WithoutSubNamespaceAndWithoutVersionAndCulture ()
    {
      var type = typeof(Moq.Capture);
      string name = TypeUtility.GetAbbreviatedTypeName(type, false);
      Assert.That(name, Is.EqualTo("Moq::Capture"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithoutSubNamespaceAndWithVersionAndCulture ()
    {
      var type = typeof(Moq.Capture);
      string name = TypeUtility.GetAbbreviatedTypeName(type, true);
      Assert.That(name, Is.EqualTo("Moq::Capture" + type.Assembly.FullName.Replace("Moq", string.Empty)));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithSubNamespaceAndWithoutVersionAndCulture ()
    {
      var type = typeof(Moq.Language.ICallback);
      string name = TypeUtility.GetAbbreviatedTypeName(type, false);
      Assert.That(name, Is.EqualTo("Moq::Language.ICallback"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithSubNamespaceAndWithVersionAndCulture ()
    {
      var type = typeof(Moq.Language.ICallback);
      string name = TypeUtility.GetAbbreviatedTypeName(type, true);
      Assert.That(name, Is.EqualTo("Moq::Language.ICallback" + type.Assembly.FullName.Replace("Moq", string.Empty)));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithoutAbbreviate ()
    {
      var type = typeof(NUnit.Framework.Constraints.AllOperator);
      string name = TypeUtility.GetAbbreviatedTypeName(type, false);
      Assert.That(name, Is.EqualTo("NUnit.Framework.Constraints.AllOperator, nunit.framework"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithNestedType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(NestedType), false);
      Assert.That(name, Is.EqualTo("Remotion.UnitTests::Utilities.TypeUtilityTests+NestedType"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithNestedNestedType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(NestedType.NestedNestedType), false);
      Assert.That(name, Is.EqualTo("Remotion.UnitTests::Utilities.TypeUtilityTests+NestedType+NestedNestedType"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithRootType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(RootType), false);
      Assert.That(name, Is.EqualTo("RootType, Remotion.UnitTests"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithRootNestedType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(RootType.RootNestedType), false);
      Assert.That(name, Is.EqualTo("RootType+RootNestedType, Remotion.UnitTests"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithNestedGenericType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(NestedGenericType<>), false);
      Assert.That(name, Is.EqualTo("Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithClosedNestedGenericType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(NestedGenericType<TestAttribute>), false);
      Assert.That(name, Is.EqualTo("Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1[[NUnit.Framework.TestAttribute, nunit.framework]]"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithGenericAbbreviatedClosedNestedGenericType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(NestedGenericType<NestedGenericType<TestAttribute>>), false);
      Assert.That(
          name,
          Is.EqualTo(
              "Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1[Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1[[NUnit.Framework.TestAttribute, nunit.framework]]]"));
    }


    [Test]
    public void GetAbbreviatedTypeName_WithNestedGenericNestedType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(NestedGenericType<>.NestedGenericNestedType), false);
      Assert.That(name, Is.EqualTo("Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1+NestedGenericNestedType"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithClosedNestedGenericNestedType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(NestedGenericType<TestAttribute>.NestedGenericNestedType), false);
      Assert.That(name, Is.EqualTo("Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1+NestedGenericNestedType[[NUnit.Framework.TestAttribute, nunit.framework]]"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithNestedGenericNestedGenericType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(NestedGenericType<>.NestedGenericNestedGenericType<>), false);
      Assert.That(name, Is.EqualTo("Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1+NestedGenericNestedGenericType`1"));
    }

    [Test]
    public void GetAbbreviatedTypeName_WithClosedNestedGenericNestedGenericType ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName(typeof(NestedGenericType<TestAttribute>.NestedGenericNestedGenericType<TestCaseData>), false);
      Assert.That(
          name,
          Is.EqualTo(
              "Remotion.UnitTests::Utilities.TypeUtilityTests+NestedGenericType`1+NestedGenericNestedGenericType`1[[NUnit.Framework.TestAttribute, nunit.framework], [NUnit.Framework.TestCaseData, nunit.framework]]"));
    }

    private void AssertTransformation (string abbreviatedName, string fullName)
    {
      string result = TypeUtility.ParseAbbreviatedTypeName(abbreviatedName);
      Assert.That(result, Is.EqualTo(fullName));
    }
  }
}
