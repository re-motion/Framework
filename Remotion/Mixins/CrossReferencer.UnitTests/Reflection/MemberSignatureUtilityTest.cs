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
  public class MemberSignatureUtilityTest
  {
    private MemberSignatureUtility _memberSignatureUtility;
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _outputFormatter = new OutputFormatter();
      _memberSignatureUtility = new MemberSignatureUtility(_outputFormatter);
    }

    [Test]
    public void GetMemberSignature_MethodWithParams ()
    {
      var methodInfo = typeof(MemberSignatureTestClass).GetMethod("MethodWithParams");
      var output = _memberSignatureUtility.GetMemberSignature(methodInfo);
      var expectedOutput = _outputFormatter.CreateMethodMarkup(methodInfo.Name, methodInfo.ReturnType, methodInfo.GetParameters());

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_Property ()
    {
      var propertyInfo = typeof(MemberSignatureTestClass).GetProperty("ProtectedProperty", BindingFlags.Instance | BindingFlags.NonPublic);
      var output = _memberSignatureUtility.GetMemberSignature(propertyInfo);
      var expectedOutput = _outputFormatter.CreatePropertyMarkup(propertyInfo.Name, propertyInfo.PropertyType);

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_Constructor ()
    {
      var constructorInfo = typeof(MemberSignatureTestClass).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
      var output = _memberSignatureUtility.GetMemberSignature(constructorInfo);
      var expectedOutput = _outputFormatter.CreateConstructorMarkup("MemberSignatureTestClass", constructorInfo.GetParameters());

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_Event ()
    {
      var eventInfo = typeof(MemberSignatureTestClass).GetEvent("ProtectedInternalEvent", BindingFlags.Instance | BindingFlags.NonPublic);
      var output = _memberSignatureUtility.GetMemberSignature(eventInfo);
      var expectedOutput = _outputFormatter.CreateEventMarkup(eventInfo.Name, eventInfo.EventHandlerType);

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_Field ()
    {
      var fieldInfo = typeof(MemberSignatureTestClass).GetField("_dictionary", BindingFlags.Instance | BindingFlags.NonPublic);
      var output = _memberSignatureUtility.GetMemberSignature(fieldInfo);
      var expectedOutput = _outputFormatter.CreateFieldMarkup(fieldInfo.Name, fieldInfo.FieldType);

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_NestedClass ()
    {
      var memberInfo = typeof(MemberSignatureTestClass).GetMember("NestedClass", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
      var output = _memberSignatureUtility.GetMemberSignature(memberInfo);
      var expectedOutput = _outputFormatter.CreateNestedTypeMarkup((Type)memberInfo);

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }
  }
}
