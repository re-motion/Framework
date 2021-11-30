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
using Moq;
using NUnit.Framework;
using Remotion.Web.Infrastructure;
using Remotion.Web.Services;

namespace Remotion.Web.UnitTests.Core.Services.WebServiceFactoryTests
{
  [TestFixture]
  public class CreateWebService
  {
    private interface IInvalidInterface
    {
    }

    private class InvalidBaseType
    {
    }

    private interface IValidWebService
    {
      void WebMethod ();

      void MethodWithParameters (int param1, bool param2);
    }

    private interface IInvalidWebServiceWithMissingWebMethodAttribute
    {
      void MethodWithoutWebMethodAttribute ();
    }

    [WebService]
    [ScriptService]
    private class TestScriptService : WebService, IValidWebService, IInvalidWebServiceWithMissingWebMethodAttribute
    {
      [WebMethod]
      public void WebMethod ()
      {
      }

      [WebMethod]
      public void MethodWithParameters (int param1, bool param2)
      {
      }

      public void MethodWithoutWebMethodAttribute ()
      {
      }
    }

    private Mock<IBuildManager> _buildManagerStub;
    private WebServiceFactory _webServiceFactory;

    [SetUp]
    public void SetUp ()
    {
      _buildManagerStub = new Mock<IBuildManager>();
      _webServiceFactory = new WebServiceFactory(_buildManagerStub.Object);
    }

    [Test]
    public void Test ()
    {
      _buildManagerStub.Setup(stub => stub.GetCompiledType("~/VirtualServicePath")).Returns(typeof(TestScriptService));

      var service = _webServiceFactory.CreateWebService<IValidWebService>("~/VirtualServicePath");

      Assert.That(service, Is.InstanceOf<TestScriptService>());
    }

    [Test]
    public void Test_InterfaceNotImplemented ()
    {
      _buildManagerStub.Setup(stub => stub.GetCompiledType("~/VirtualServicePath")).Returns(typeof(TestScriptService));

      Assert.That(
          () => _webServiceFactory.CreateWebService<IInvalidInterface>("~/VirtualServicePath"),
          Throws.ArgumentException.And.Message.EqualTo(
              "Web service '~/VirtualServicePath' does not implement mandatory interface "
              + "'Remotion.Web.UnitTests.Core.Services.WebServiceFactoryTests.CreateWebService+IInvalidInterface'."));
    }

    [Test]
    public void Test_BaseTypeNotImplemented ()
    {
      _buildManagerStub.Setup(stub => stub.GetCompiledType("~/VirtualServicePath")).Returns(typeof(TestScriptService));

      Assert.That(
          () => _webServiceFactory.CreateWebService<InvalidBaseType>("~/VirtualServicePath"),
          Throws.ArgumentException.And.Message.EqualTo(
              "Web service '~/VirtualServicePath' is not based on type "
              + "'Remotion.Web.UnitTests.Core.Services.WebServiceFactoryTests.CreateWebService+InvalidBaseType'."));
    }

    [Test]
    public void Test_FactoryChecksWebServiceDeclaration ()
    {
      _buildManagerStub.Setup(stub => stub.GetCompiledType("~/VirtualServicePath")).Returns(typeof(TestScriptService));

      Assert.That(
          () => _webServiceFactory.CreateWebService<IInvalidWebServiceWithMissingWebMethodAttribute>("~/VirtualServicePath"),
          Throws.ArgumentException.And.Message.Contains(
              " does not have the 'System.Web.Services.WebMethodAttribute' applied."));
    }

    [Test]
    public void Test_VirtualPathCannotBeCompiled ()
    {
      _buildManagerStub.Setup(stub => stub.GetCompiledType("~/VirtualServicePath")).Returns((Type)null);

      Assert.That(
          () => _webServiceFactory.CreateWebService<IInvalidInterface>("~/VirtualServicePath"),
          Throws.InvalidOperationException.And.Message.EqualTo("Web service '~/VirtualServicePath' could not be compiled."));
    }
  }
}
