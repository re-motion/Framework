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
using Remotion.Web.Infrastructure;
using Remotion.Web.Services;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Services.WebServiceFactoryTests
{
  [TestFixture]
  public class CreateJsonService
  {
    private interface IInvalidInterface
    {
    }

    private class InvalidBaseType
    {
    }

    private interface IValidJsonService
    {
      void JsonMethod ();

      void MethodWithParameters (int param1, bool param2);
    }

    private interface IInvalidJsonServiceWithWrongResponseFormat
    {
      void XmlMethod ();
    }

    [WebService]
    [ScriptService]
    private class TestScriptService : WebService, IValidJsonService, IInvalidJsonServiceWithWrongResponseFormat
    {
      [WebMethod]
      [ScriptMethod (ResponseFormat = ResponseFormat.Json)]
      public void JsonMethod ()
      {
      }

      [WebMethod]
      [ScriptMethod (ResponseFormat = ResponseFormat.Json)]
      public void MethodWithParameters (int param1, bool param2)
      {
      }

      [WebMethod]
      [ScriptMethod (ResponseFormat = ResponseFormat.Xml)]
      public void XmlMethod ()
      {
      }
    }

    private IBuildManager _buildManagerStub;
    private WebServiceFactory _webServiceFactory;

    [SetUp]
    public void SetUp ()
    {
      _buildManagerStub = MockRepository.GenerateStub<IBuildManager>();
      _webServiceFactory = new WebServiceFactory (_buildManagerStub);
    }

    [Test]
    public void Test ()
    {
      _buildManagerStub.Stub (stub => stub.GetCompiledType ("~/VirtualServicePath")).Return (typeof (TestScriptService));

      var service = _webServiceFactory.CreateJsonService<IValidJsonService> ("~/VirtualServicePath");

      Assert.That (service, Is.InstanceOf<TestScriptService>());
    }

    [Test]
    public void Test_InterfaceNotImplemented ()
    {
      _buildManagerStub.Stub (stub => stub.GetCompiledType ("~/VirtualServicePath")).Return (typeof (TestScriptService));

      Assert.That (
          () => _webServiceFactory.CreateJsonService<IInvalidInterface> ("~/VirtualServicePath"),
          Throws.ArgumentException.And.Message.EqualTo (
              "Web service '~/VirtualServicePath' does not implement mandatory interface "
              + "'Remotion.Web.UnitTests.Core.Services.WebServiceFactoryTests.CreateJsonService+IInvalidInterface'."));
    }

    [Test]
    public void Test_BaseTypeNotImplemented ()
    {
      _buildManagerStub.Stub (stub => stub.GetCompiledType ("~/VirtualServicePath")).Return (typeof (TestScriptService));

      Assert.That (
          () => _webServiceFactory.CreateJsonService<InvalidBaseType> ("~/VirtualServicePath"),
          Throws.ArgumentException.And.Message.EqualTo (
              "Web service '~/VirtualServicePath' is not based on type "
              + "'Remotion.Web.UnitTests.Core.Services.WebServiceFactoryTests.CreateJsonService+InvalidBaseType'."));
    }

    [Test]
    public void Test_FactoryChecksJsonServiceDeclaration ()
    {
      _buildManagerStub.Stub (stub => stub.GetCompiledType ("~/VirtualServicePath")).Return (typeof (TestScriptService));

      Assert.That (
          () => _webServiceFactory.CreateJsonService<IInvalidJsonServiceWithWrongResponseFormat> ("~/VirtualServicePath"),
          Throws.ArgumentException.And.Message.ContainsSubstring (
              " does not have the ResponseFormat property of the ScriptMethodAttribute set to Json."));
    }

    [Test]
    public void Test_VirtualPathCannotBeCompiled ()
    {
      _buildManagerStub.Stub (stub => stub.GetCompiledType ("~/VirtualServicePath")).Return (null);

      Assert.That (
          () => _webServiceFactory.CreateWebService<IInvalidInterface> ("~/VirtualServicePath"),
          Throws.InvalidOperationException.And.Message.EqualTo ("Web service '~/VirtualServicePath' could not be compiled."));
    }
  }
}