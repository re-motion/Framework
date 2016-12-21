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
using Remotion.Data.DomainObjects.Web.IntegrationTests.TestDomain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests
{
  [TestFixture]
  public class WxeParameterIntegrationTest
  {
    private class TestFunction : WxeFunction
    {
      public TestFunction (WxeParameterDeclaration parameterDeclaration, object parameterValue)
          : base (WxeTransactionMode<ClientTransactionFactory>.None, new[] { parameterDeclaration }, new[] { parameterValue })
      {
      }
    }

    [Test]
    public void SerializeObjectID ()
    {
      var objectID = GetObjectID();
      var parameterDeclaration = new WxeParameterDeclaration ("name", true, WxeParameterDirection.In, typeof (ObjectID));
      var function = new TestFunction (parameterDeclaration, objectID);
      function.VariablesContainer.EnsureParametersInitialized (null);

      var queryString = function.VariablesContainer.SerializeParametersForQueryString();

      Assert.That (queryString, Is.EqualTo (new NameValueCollection { { "name", objectID.ToString() } }));
    }

    [Test]
    public void DeserializeObjectID ()
    {
      var objectID = GetObjectID();
      var parameterDeclaration = new WxeParameterDeclaration ("name", true, WxeParameterDirection.In, typeof (ObjectID));
      var function = new TestFunction (parameterDeclaration, null);

      function.VariablesContainer.InitializeParameters (new NameValueCollection { { "name", objectID.ToString() } });

      Assert.That (function.Variables["name"], Is.EqualTo (objectID));
    }

    [Test]
    public void SerializeAndDeserializeObjectID ()
    {
      var objectID = GetObjectID();
      var parameterDeclaration = new WxeParameterDeclaration ("name", true, WxeParameterDirection.In, typeof (ObjectID));
      var function = new TestFunction (parameterDeclaration, objectID);
      function.VariablesContainer.EnsureParametersInitialized (null);

      var queryString = function.VariablesContainer.SerializeParametersForQueryString();
      Assert.That (queryString.Count, Is.EqualTo (1));

      var deserializedfunction = new TestFunction (parameterDeclaration, null);
      deserializedfunction.VariablesContainer.InitializeParameters (queryString);
      Assert.That (function.Variables["name"], Is.EqualTo (objectID));
    }

    [Test]
    public void SerializeIDomainObjectHandle ()
    {
      var objectID = GetObjectID();
      var domainObjectHandle = objectID.GetHandle<SampleObject>();
      var parameterDeclaration = new WxeParameterDeclaration (
          "name",
          true,
          WxeParameterDirection.In,
          typeof (IDomainObjectHandle<SampleObject>));
      var function = new TestFunction (parameterDeclaration, domainObjectHandle);
      function.VariablesContainer.EnsureParametersInitialized (null);

      var queryString = function.VariablesContainer.SerializeParametersForQueryString();
      
      Assert.That (queryString, Is.EqualTo (new NameValueCollection { { "name", objectID.ToString() } }));
    }

    [Test]
    public void DeserializeIDomainObjectHandle ()
    {
      var objectID = GetObjectID();
      var parameterDeclaration = new WxeParameterDeclaration (
          "name",
          true,
          WxeParameterDirection.In,
          typeof (IDomainObjectHandle<SampleObject>));
      var function = new TestFunction (parameterDeclaration, null);

      function.VariablesContainer.InitializeParameters (new NameValueCollection { { "name", objectID.ToString() } });

      Assert.That (function.Variables["name"], Is.InstanceOf<IDomainObjectHandle<SampleObject>> ());
      Assert.That (((IDomainObjectHandle<SampleObject>) function.Variables["name"]).ObjectID, Is.EqualTo (objectID));
    }

    [Test]
    public void SerializeAndDeserializeIDomainObjectHandle ()
    {
      var objectID = GetObjectID();
      var domainObjectHandle = objectID.GetHandle<SampleObject>();
      var parameterDeclaration = new WxeParameterDeclaration (
          "name",
          true,
          WxeParameterDirection.In,
          typeof (IDomainObjectHandle<SampleObject>));
      var function = new TestFunction (parameterDeclaration, domainObjectHandle);
      function.VariablesContainer.EnsureParametersInitialized (null);

      var queryString = function.VariablesContainer.SerializeParametersForQueryString();
      Assert.That (queryString.Count, Is.EqualTo (1));

      var deserializedfunction = new TestFunction (parameterDeclaration, null);
      deserializedfunction.VariablesContainer.InitializeParameters (queryString);
      Assert.That (function.Variables["name"], Is.InstanceOf<IDomainObjectHandle<SampleObject>>());
      Assert.That (function.Variables["name"], Is.EqualTo (domainObjectHandle));
    }

    private static ObjectID GetObjectID ()
    {
      return new ObjectID (typeof (SampleObject), Guid.NewGuid());
    }
  }
}