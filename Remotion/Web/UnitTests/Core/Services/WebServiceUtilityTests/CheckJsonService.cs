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
using System.Web.Script.Services;
using System.Web.Services;
using NUnit.Framework;
using Remotion.Web.Services;

namespace Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests
{
  [TestFixture]
  public class CheckJsonService
  {
    [WebService]
    [ScriptService]
    private class TestScriptService : WebService
    {
      [WebMethod]
      [ScriptMethod]
      public void ScriptMethod ()
      {
      }

      [WebMethod]
      [ScriptMethod (ResponseFormat = ResponseFormat.Json)]
      public void JsonMethod ()
      {
      }

      [WebMethod]
      public void MethodWithoutScriptMethodAttribute ()
      {
      }

      [WebMethod]
      [ScriptMethod (ResponseFormat = ResponseFormat.Xml)]
      public void MethodWithResponeFormatNotJson ()
      {
      }

      [WebMethod]
      [ScriptMethod (ResponseFormat = ResponseFormat.Json)]
      public void MethodWithParameters (int param1, bool param2)
      {
      }
    }

    [WebService]
    private class TestWebService : WebService
    {
      [WebMethod]
      public void Method()
      {
      }
    }
    
    private class TestNotAWebService
    {
    }

    [Test]
    public void CheckJsonServiceMethod_Valid ()
    {
      Assert.That (() => WebServiceUtility.CheckJsonService (typeof (TestScriptService), "JsonMethod"), Throws.Nothing);
    }
    
    [Test]
    public void Test_BaseTypeNotWebService ()
    {
      Assert.That (
          () => WebServiceUtility.CheckJsonService (typeof (TestNotAWebService), "Method"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web service type 'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckJsonService+TestNotAWebService'"
                  + " does not derive from 'System.Web.Services.WebService'."));
    }

    [Test]
    public void Test_MissingScriptServiceAttribute ()
    {
      Assert.That (
          () => WebServiceUtility.CheckJsonService (typeof (TestWebService), "Method"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web service type 'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckJsonService+TestWebService'"
                  + " does not have the 'System.Web.Script.Services.ScriptServiceAttribute' applied."));
    }

    [Test]
    public void Test_ResponseFormatNotJson ()
    {
      Assert.That (
          () => WebServiceUtility.CheckJsonService (typeof (TestScriptService), "MethodWithResponeFormatNotJson"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web method 'MethodWithResponeFormatNotJson' on web service type "
                  + "'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckJsonService+TestScriptService'"
                  + " does not have the ResponseFormat property of the ScriptMethodAttribute set to Json."));
    }

    [Test]
    public void Test_ParametersMatch ()
    {
      Assert.That (
          () => WebServiceUtility.CheckJsonService (typeof (TestScriptService), "MethodWithParameters", "param2", "param1"),
          Throws.Nothing);
    }

    [Test]
    public void Test_MissingParameter()
    {
      Assert.That (
          () => WebServiceUtility.CheckJsonService (typeof (TestScriptService), "MethodWithParameters", "param1", "param3"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web method 'MethodWithParameters' on web service type "
                  + "'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckJsonService+TestScriptService'"
                  + " does not declare the required parameter 'param3'. Parameters are matched by name and case."));
    }

    [Test]
    public void Test_UnexpectedParameter ()
    {
      Assert.That (
          () => WebServiceUtility.CheckJsonService (typeof (TestScriptService), "MethodWithParameters", "param2"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web method 'MethodWithParameters' on web service type "
                  + "'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckJsonService+TestScriptService'"
                  + " has unexpected parameter 'param1'."));
    }

    [Test]
    public void Test_ParameterWithWrongCase()
    {
      Assert.That (
          () => WebServiceUtility.CheckJsonService (typeof (TestScriptService), "MethodWithParameters", "param1", "Param2"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web method 'MethodWithParameters' on web service type "
                  + "'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckJsonService+TestScriptService'"
                  + " does not declare the required parameter 'Param2'. Parameters are matched by name and case."));
    }
  }
}