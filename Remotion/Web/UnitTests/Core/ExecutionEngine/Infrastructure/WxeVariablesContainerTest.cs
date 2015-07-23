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
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class WxeVariablesContainerTest
  {
    [Test]
    public void GetParameterDeclarations_ParametersDeclaredOnType_ReturnsParametersSortedByIndex ()
    {
      var parameters = WxeVariablesContainer.GetParameterDeclarations (typeof (TestBaseFunctionWithParameters));
      Assert.That (parameters.Length, Is.EqualTo (2));
      Assert.That (parameters[0].Name, Is.EqualTo ("Parameter1"));
      Assert.That (parameters[1].Name, Is.EqualTo ("Parameter2"));
    }

    [Test]
    public void GetParameterDeclarations_ParametersDeclaredOnTypeAndBaseType_ReturnsParametersSortedByHierarchyAndIndex ()
    {
      var parameters = WxeVariablesContainer.GetParameterDeclarations (typeof (TestDerivedFunctionWithParameters));
      Assert.That (parameters.Length, Is.EqualTo (4));
      Assert.That (parameters[0].Name, Is.EqualTo ("Parameter1"));
      Assert.That (parameters[1].Name, Is.EqualTo ("Parameter2"));
      Assert.That (parameters[2].Name, Is.EqualTo ("Parameter3"));
      Assert.That (parameters[3].Name, Is.EqualTo ("Parameter4"));
    }

    [Test]
    public void GetParameterDeclarations_ParametersDeclaredOnOverride_ThrowsWxeException ()
    {
      Assert.That (
          () => WxeVariablesContainer.GetParameterDeclarations (typeof (TestFunctionWithOverriddenParameter)),
          Throws.TypeOf<WxeException>()
              .With.Message.EqualTo (
                  "Property 'Parameter1', overridden by 'Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions.TestFunctionWithOverriddenParameter', has a WxeParameterAttribute applied. "
                  + "The WxeParameterAttribute may only be applied to the original declaration of a property."));
    }

    [Test]
    public void GetParameterDeclarations_DuplciateParameterIndex_ThrowsWxeException ()
    {
      Assert.That (
          () => WxeVariablesContainer.GetParameterDeclarations (typeof (TestFunctionWithDuplicateParameterIndices)),
          Throws.TypeOf<WxeException>().With.Message.EqualTo (
              "'Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions.TestFunctionWithDuplicateParameterIndices' declares WxeParameters 'Parameter1' and 'Parameter2' with the same index. "
              + "The index of a WxeParameter must be unique within a type."));
    }

    [Test]
    public void SerializeParameters ()
    {
      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters ("Hello World", null, 1);
      NameValueCollection parameters = function.VariablesContainer.SerializeParametersForQueryString();
      Assert.That (parameters.Count, Is.EqualTo (3));
      Assert.That (parameters["StringValue"], Is.EqualTo ("Hello World"));
      Assert.That (parameters["NaInt32Value"], Is.EqualTo (""));
      Assert.That (parameters["IntValue"], Is.EqualTo ("1"));
    }

    [Test]
    public void SerializeParametersWithInt32BeingNull ()
    {
      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.Variables["StringValue"] = "Hello World";
      function.Variables["NaInt32Value"] = 1;
      function.Variables["Int32Value"] = null;

      NameValueCollection parameters = function.VariablesContainer.SerializeParametersForQueryString();

      Assert.That (parameters.Count, Is.EqualTo (2));
      Assert.That (parameters["StringValue"], Is.EqualTo ("Hello World"));
      Assert.That (parameters["NaInt32Value"], Is.EqualTo ("1"));
    }

    [Test]
    public void InitializeParameters ()
    {
      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      NameValueCollection parameters = new NameValueCollection();
      parameters.Add ("StringValue", "Hello World");
      parameters.Add ("NaInt32Value", "");
      parameters.Add ("IntValue", "1");

      function.VariablesContainer.InitializeParameters (parameters);

      Assert.That (function.StringValue, Is.EqualTo ("Hello World"));
      Assert.That (function.NaInt32Value, Is.EqualTo (null));
      Assert.That (function.IntValue, Is.EqualTo (1));
    }

    [Test]
    public void InitializeParameters_WithStringBeingEmpty ()
    {
      NameValueCollection parameters = new NameValueCollection();
      parameters.Add ("StringValue", "");
      parameters.Add ("NaInt32Value", "2");
      parameters.Add ("IntValue", "1");

      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.VariablesContainer.InitializeParameters (parameters);

      Assert.That (function.StringValue, Is.EqualTo (""));
      Assert.That (function.NaInt32Value, Is.EqualTo (2));
      Assert.That (function.IntValue, Is.EqualTo (1));
    }

    [Test]
    public void InitializeParameters_WithNaInt32BeingEmpty ()
    {
      NameValueCollection parameters = new NameValueCollection();
      parameters.Add ("StringValue", "Hello World");
      parameters.Add ("NaInt32Value", "");
      parameters.Add ("IntValue", "1");

      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.VariablesContainer.InitializeParameters (parameters);

      Assert.That (function.StringValue, Is.EqualTo ("Hello World"));
      Assert.That (function.NaInt32Value, Is.EqualTo (null));
      Assert.That (function.IntValue, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (ApplicationException))]
    public void InitializeParameters_WitInt32BeingEmpty ()
    {
      NameValueCollection parameters = new NameValueCollection();
      parameters.Add ("StringValue", "Hello World");
      parameters.Add ("NaInt32Value", "2");
      parameters.Add ("IntValue", "");

      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.VariablesContainer.InitializeParameters (parameters);
    }
  }
}
