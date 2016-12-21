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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Metadata;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Core.Security.Domain;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityAdapterTestWithPermissionsFromInstanceMethod
  {
    // types

    // static members

    // member fields

    private IWxeSecurityAdapter _securityAdapter;
    private MockRepository _mocks;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private ISecurityProvider _mockSecurityProvider;
    private IPrincipalProvider _mockPrincipalProvider;
    private ISecurityPrincipal _stubUser;
    private ServiceLocatorScope _serviceLocatorScope;

    // construction and disposing

    public WxeSecurityAdapterTestWithPermissionsFromInstanceMethod ()
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
      _stubUser = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (_stubUser.User).Return ("user");
      _mockPrincipalProvider = _mocks.StrictMock<IPrincipalProvider> ();
      SetupResult.For (_mockPrincipalProvider.GetPrincipal ()).Return (_stubUser);

      _mockObjectSecurityStrategy = _mocks.StrictMock<IObjectSecurityStrategy> ();
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
      ExpectObjectSecurityStrategyHasAccessForSecurableObject (GeneralAccessTypes.Read, true);
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      _securityAdapter.CheckAccess (function);

      _mocks.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void CheckAccess_AccessDenied ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject (GeneralAccessTypes.Read, false);
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      _securityAdapter.CheckAccess (function);
    }

    [Test]
    public void CheckAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      using (SecurityFreeSection.Activate())
      {
        _securityAdapter.CheckAccess (function);
      }

      _mocks.VerifyAll ();
    }

    [Test]
    public void HasAccess_AccessGranted ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Read, true);
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (function);

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void HasAccess_AccessDenied ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject (GeneralAccessTypes.Read, false);
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (function);

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.False);
    }

    [Test]
    public void HasAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityAdapter.HasAccess (function);
      }

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void HasStatelessAccess_AccessGranted ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (GeneralAccessTypes.Read, true);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromInstanceMethod));

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void HasStatelessAccess_AccessDenied ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (GeneralAccessTypes.Read, false);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromInstanceMethod));

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.False);
    }

    [Test]
    public void HasStatelessAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityAdapter.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromInstanceMethod));
      }

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    private void ExpectObjectSecurityStrategyHasAccessForSecurableObject (Enum accessTypeEnum, bool returnValue)
    {
      Expect
          .Call (
              _mockObjectSecurityStrategy.HasAccess (
                  Arg.Is (_mockSecurityProvider),
                  Arg.Is (_stubUser),
                  Arg<IReadOnlyList<AccessType>>.List.Equal (new[] { AccessType.Get (accessTypeEnum) })))
          .Return (returnValue);
    }

    private void ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (Enum accessTypeEnum, bool returnValue)
    {
      Expect
          .Call (
              _mockFunctionalSecurityStrategy.HasAccess (
                  Arg.Is (typeof (SecurableObject)),
                  Arg.Is (_mockSecurityProvider),
                  Arg.Is (_stubUser),
                  Arg<IReadOnlyList<AccessType>>.List.Equal (new[] { AccessType.Get (accessTypeEnum) })))
          .Return (returnValue);
    }
  }
}
