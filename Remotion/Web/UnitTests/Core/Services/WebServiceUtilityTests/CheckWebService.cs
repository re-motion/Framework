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
using System.Web.Services;
using NUnit.Framework;
using Remotion.Web.Services;

namespace Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests
{
  [TestFixture]
  public class CheckWebService
  {
    [WebService]
    private class TestWebService : WebService
    {
      [WebMethod]
      public void Method ()
      {
      }

      public void MethodWithoutWebMethodAttribute ()
      {
      }

      protected void NonPublicMethod ()
      {
      }
    }

    private class TestWebServiceWithoutAttribute : WebService
    {
    }

    private class TestNotAWebService
    {
    }

    [Test]
    public void Test_Valid ()
    {
      Assert.That (() => WebServiceUtility.CheckWebService (typeof (TestWebService), "Method"), Throws.Nothing);
    }

    [Test]
    public void Test_BaseTypeNotWebService ()
    {
      Assert.That (
          () => WebServiceUtility.CheckWebService (typeof (TestNotAWebService), "Method"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web service type 'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckWebService+TestNotAWebService'"
                  + " does not derive from 'System.Web.Services.WebService'."));
    }

    [Test]
    public void Test_MissingWebServiceAttribute ()
    {
      Assert.That (
          () => WebServiceUtility.CheckScriptService (typeof (TestWebServiceWithoutAttribute), "Method"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web service type 'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckWebService+TestWebServiceWithoutAttribute'"
                  + " does not have the 'System.Web.Services.WebServiceAttribute' applied."));
    }

    [Test]
    public void Test_MissingWebMethodAttribute ()
    {
      Assert.That (
          () => WebServiceUtility.CheckScriptService (typeof (TestWebService), "MethodWithoutWebMethodAttribute"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web method 'MethodWithoutWebMethodAttribute' on web service type "
                  + "'Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckWebService+TestWebService'"
                  + " does not have the 'System.Web.Services.WebMethodAttribute' applied."));
    }

    [Test]
    public void Test_MissingWebMethod ()
    {
      Assert.That (
          () => WebServiceUtility.CheckWebService (typeof (TestWebService), "NonPublicMethod"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web method 'NonPublicMethod' was not found on the public API of web service type '"
                  + "Remotion.Web.UnitTests.Core.Services.WebServiceUtilityTests.CheckWebService+TestWebService'."));
    }
  }
}