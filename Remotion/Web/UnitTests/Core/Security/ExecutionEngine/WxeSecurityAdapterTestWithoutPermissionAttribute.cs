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
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityAdapterTestWithoutPermissionAttribute
  {
    // types

    // static members

    // member fields

    private IWxeSecurityAdapter _securityAdapter;
    private MockRepository _mocks;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private ISecurityProvider _mockSecurityProvider;
    private IPrincipalProvider _mockPrincipalProvider;
    private ServiceLocatorScope _serviceLocatorScope;

    // construction and disposing

    public WxeSecurityAdapterTestWithoutPermissionAttribute ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new WxeSecurityAdapter ();

      _mocks = new MockRepository ();

      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider> ();
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockPrincipalProvider = _mocks.StrictMock<IPrincipalProvider> ();
      _mockFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy> ();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => _mockSecurityProvider);
      serviceLocator.RegisterSingle (() => _mockPrincipalProvider);
      serviceLocator.RegisterSingle (() => _mockFunctionalSecurityStrategy);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
    }

    [TearDown]
    public void TearDown ()
    {
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void CheckAccess_AccessGranted ()
    {
      _mocks.ReplayAll ();

      _securityAdapter.CheckAccess (new TestFunctionWithoutPermissions ());

      _mocks.VerifyAll ();
    }

    [Test]
    public void HasAccess_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (new TestFunctionWithoutPermissions ());

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void HasStatelessAccess_AccessGranted ()
    {
      _mocks.ReplayAll ();
      
      bool hasAccess = _securityAdapter.HasStatelessAccess (typeof (TestFunctionWithoutPermissions));

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }
  }
}
