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
  public class CheckScriptService
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
      public void MethodWithoutScriptMethodAttribute ()
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
    public void Test_Valid ()
    {
      Assert.That (() => WebServiceUtility.CheckJsonService (typeof (TestScriptService), "ScriptMethod"), Throws.Nothing);
    }

    [Test]
    public void Test_BaseTypeNotWebService ()
    {
      Assert.That (
          () => WebServiceUtility.CheckJsonService (typeof (TestNotAWebService), "Method"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web service type 'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckScriptService+TestNotAWebService'"
                  + " does not derive from 'System.Web.Services.WebService'."));
    }

    [Test]
    public void Test_MissingScriptServiceAttribute ()
    {
      Assert.That (
          () => WebServiceUtility.CheckScriptService (typeof (TestWebService), "Method"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web service type 'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckScriptService+TestWebService'"
                  + " does not have the 'System.Web.Script.Services.ScriptServiceAttribute' applied."));
    }

    [Test]
    public void Test_MissingScriptMethodAttribute ()
    {
      Assert.That (
          () => WebServiceUtility.CheckScriptService (typeof (TestScriptService), "MethodWithoutScriptMethodAttribute"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web method 'MethodWithoutScriptMethodAttribute' on web service type "
                  + "'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckScriptService+TestScriptService'"
                  + " does not have the 'System.Web.Script.Services.ScriptMethodAttribute' applied."));
    }
  }
}