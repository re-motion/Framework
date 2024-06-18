// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Formatting
{
  [TestFixture]
  public class OutputFormatterTest
  {
    private OutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _outputFormatter = new OutputFormatter();
    }

    [Test]
    public void GetShortFormattedTypeName_NormalType ()
    {
      var output = _outputFormatter.GetShortFormattedTypeName (typeof (UselessObject));

      Assert.That (output, Is.EqualTo ("UselessObject"));
    }

    [Test]
    public void GetShortFormattedTypeName_SimpeType ()
    {
      var output = _outputFormatter.GetShortFormattedTypeName (typeof (int));

      Assert.That (output, Is.EqualTo ("Int32"));
    }

    [Test]
    public void GetShortFormattedTypeName_GenericDefinition ()
    {
      var output = _outputFormatter.GetShortFormattedTypeName (typeof (GenericTarget<,>));

      Assert.That (output, Is.EqualTo ("GenericTarget<TParameter1, TParameter2>"));
    }

    [Test]
    public void GetShortFormattedTypeName_GenericType ()
    {
      var output = _outputFormatter.GetShortFormattedTypeName (typeof (GenericTarget<string, int>));

      Assert.That (output, Is.EqualTo ("GenericTarget<System.String, System.Int32>"));
    }

    public class ContainsGenericArguments<TKey> : Dictionary<TKey, int>
    {
    }

    [Test]
    public void GetShortFormattedTypeName_ContainsGenericArguments ()
    {
      var output = _outputFormatter.GetShortFormattedTypeName (typeof (ContainsGenericArguments<>).BaseType);

      Assert.That (output, Is.EqualTo ("Dictionary<TKey, System.Int32>"));
    }

    // See http://blogs.msdn.com/b/haibo_luo/archive/2006/02/17/534480.aspx for an explanation
    class G<T> { }
    class G2<T> : G<T> { }

    [Test]
    public void GetShortFormattedTypeName_TypeReturnsNullForFullName ()
    {
      var weirdType = typeof (G2<>).BaseType;
      Assert.That (weirdType.FullName, Is.Null);

      var output = _outputFormatter.GetShortFormattedTypeName (weirdType);

      Assert.That (output, Is.EqualTo ("OutputFormatterTest+G<T>"));
    }

    [Test]
    public void GetShortFormattedTypeName_Nested ()
    {
      var output = _outputFormatter.GetShortFormattedTypeName (typeof (MemberSignatureTestClass.NestedClassWithInterfaceAndInheritance));

      Assert.That (output, Is.EqualTo ("MemberSignatureTestClass+NestedClassWithInterfaceAndInheritance"));
    }

    class Nested
    {
      internal class VeryNested { }
    }

    [Test]
    public void GetShortFormattedTypeName_VeryNested ()
    {
      var output = _outputFormatter.GetShortFormattedTypeName (typeof (Nested.VeryNested));

      Assert.That (output, Is.EqualTo ("OutputFormatterTest+Nested+VeryNested"));
    }

    class Nested<T>
    {
      internal class WithGenerics<X> { }
    }

    [Test]
    public void GetShortFormattedTypeName_NestedGenericTypeDefinitions ()
    {
      var output = _outputFormatter.GetShortFormattedTypeName (typeof (Nested<>.WithGenerics<>));

      Assert.That (output, Is.EqualTo ("OutputFormatterTest+Nested<T>+WithGenerics<X>"));
    }

    [Test]
    public void CreateModifierMarkup ()
    {
      var output = _outputFormatter.CreateModifierMarkup ("attribute1 attribute2", "keyword1 keyword2");
      var expectedOutput = new XElement (
          "Modifiers",
          new XElement ("Text", "["),
          new XElement ("Type", "attribute1"),
          new XElement ("Text", "]"),
          new XElement ("Text", "["),
          new XElement ("Type", "attribute2"),
          new XElement ("Text", "]"),
          new XElement ("Keyword", "keyword1"),
          new XElement ("Keyword", "keyword2"));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }


    [Test]
    public void CreateConstructorMarkup ()
    {
      var output = _outputFormatter.CreateConstructorMarkup ("ClassName", new ParameterInfo[0]);
      var expectedOutput = new XElement ("Signature", new XElement ("Name", "ClassName"), new XElement ("Text", "("), new XElement ("Text", ")"));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void AddParameterMarkup_WithTypeAndKeyword ()
    {
      var output = new XElement ("Signature");
      var parameterInfos = typeof (MemberSignatureTestClass).GetMethod ("MethodWithParams").GetParameters();

      // int intParam, string stringParam, AssemblyBuilder assemblyBuilderParam
      _outputFormatter.AddParameterMarkup (parameterInfos, output);
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Text", "("),
          new XElement ("Type", "System.Int32", new XAttribute("languageType", "Keyword")),
          new XElement ("ParameterName", "intParam"),
          new XElement ("Text", ","),
          new XElement ("Type", "System.String", new XAttribute ("languageType", "Keyword")),
          new XElement ("ParameterName", "stringParam"),
          new XElement ("Text", ","),
          new XElement ("Type", "MixinXRef.AssemblyResolver", new XAttribute ("languageType", "Type")),
          new XElement ("ParameterName", "assemblyBuilderParam"),
          new XElement ("Text", ")")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void AddParameterMarkup_WithNestedTypeAsParameterType ()
    {
      var output = new XElement ("Signature");
      var parameterInfos = typeof (MemberSignatureTestClass).GetMethod ("MethodWithNestedType").GetParameters();

      // MemberSignatureTestClass.NestedClassWithInterfaceAndInheritance param
      _outputFormatter.AddParameterMarkup (parameterInfos, output);
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Text", "("),
          new XElement ("Type", "MixinXRef.UnitTests.TestDomain.MemberSignatureTestClass+NestedClassWithInterfaceAndInheritance", new XAttribute ("languageType", "Type")),
          new XElement ("ParameterName", "param"),
          new XElement ("Text", ")")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void AddParameterMarkup_WithNestedGenericTypeAsParameterType ()
    {
      var output = new XElement ("Signature");
      var parameterInfos = typeof (MemberSignatureTestClass).GetMethod ("MethodWithNestedGenericType").GetParameters();

      // MemberSignatureTestClass.NestedGenericType<MemberSignatureTestClass.NestedClassWithInterfaceAndInheritance> param
      _outputFormatter.AddParameterMarkup (parameterInfos, output);
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Text", "("),
          new XElement ("Type", "MixinXRef.UnitTests.TestDomain.MemberSignatureTestClass+NestedGenericType<MixinXRef.UnitTests.TestDomain.MemberSignatureTestClass+NestedClassWithInterfaceAndInheritance>", new XAttribute ("languageType", "Type")),
          new XElement ("ParameterName", "param"),
          new XElement ("Text", ")")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateMethodMarkup ()
    {
      var output = _outputFormatter.CreateMethodMarkup ("MethodName", typeof (string), new ParameterInfo[0]);
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Type", "System.String", new XAttribute ("languageType", "Keyword")),
          new XElement ("Name", "MethodName")
          );
      _outputFormatter.AddParameterMarkup (new ParameterInfo[0], expectedOutput);

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_ExplicitInterfaceMethodImplementation ()
    {
      var methodInfo =
          typeof (MemberSignatureTestClass).GetMethod (
              "MixinXRef.UnitTests.TestDomain.IExplicitInterface.Version",
              BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

      var output = _outputFormatter.CreateMethodMarkup (methodInfo.Name, methodInfo.ReturnType, methodInfo.GetParameters());

      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Type", "System.String", new XAttribute ("languageType", "Keyword")),
          new XElement ("ExplicitInterfaceName", "IExplicitInterface"),
          new XElement ("Text", "."),
          new XElement ("Name", "Version"),
          new XElement ("Text", "("),
          new XElement ("Text", ")")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateEventMarkup ()
    {
      var output = _outputFormatter.CreateEventMarkup ("EventName", typeof (ChangedEventHandler));
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Keyword", "event"),
          new XElement ("Type", "MixinXRef.UnitTests.TestDomain.ChangedEventHandler", new XAttribute ("languageType", "Type")),
          new XElement ("Name", "EventName")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateFieldMarkup ()
    {
      var output = _outputFormatter.CreateFieldMarkup ("FieldName", typeof (int));
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Type", "System.Int32", new XAttribute ("languageType", "Keyword")),
          new XElement ("Name", "FieldName")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreatePropertyMarkup ()
    {
      var output = _outputFormatter.CreatePropertyMarkup ("PropertyName", typeof (int));
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Type", "System.Int32", new XAttribute ("languageType", "Keyword")),
          new XElement ("Name", "PropertyName")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateNestedTypeMarkup_NestedEnumeration ()
    {
      var output = _outputFormatter.CreateNestedTypeMarkup (typeof (MemberSignatureTestClass.NestedEnumeration));
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Keyword", "enum"),
          new XElement ("Name", "NestedEnumeration")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateNestedTypeMarkup_NestedStruct ()
    {
      var output = _outputFormatter.CreateNestedTypeMarkup (typeof (MemberSignatureTestClass.NestedStruct));
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Keyword", "struct"),
          new XElement ("Name", "NestedStruct"),
          new XElement ("Text", ":"),
          new XElement ("Type", "System.IDisposable")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateNestedTypeMarkup_NestedInterface ()
    {
      var output = _outputFormatter.CreateNestedTypeMarkup (typeof (MemberSignatureTestClass.INestedInterface));
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Keyword", "interface"),
          new XElement ("Name", "INestedInterface"),
          new XElement ("Text", ":"),
          new XElement ("Type", "System.IDisposable"),
          new XElement ("Text", ","),
          new XElement ("Type", "System.ICloneable")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateNestedTypeMarkup_NestedClass ()
    {
      var output = _outputFormatter.CreateNestedTypeMarkup (typeof (MemberSignatureTestClass.NestedClassWithInterfaceAndInheritance));
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Keyword", "class"),
          new XElement ("Name", "NestedClassWithInterfaceAndInheritance"),
          new XElement ("Text", ":"),
          new XElement ("Type", "MixinXRef.UnitTests.TestDomain.GenericTarget<System.String, System.Int32>"),
          new XElement ("Text", ","),
          new XElement ("Type", "System.IDisposable")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}