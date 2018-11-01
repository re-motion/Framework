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
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.WxeFunctionTests
{
  [TestFixture]
  public class Security
  {
    private MockRepository _mockRepository;
    private IWxeSecurityAdapter _mockWxeSecurityAdapter;
    private WxeContext _wxeContext;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public void SetUp ()
    {
      TestFunction rootFunction = new TestFunction ();
      WxeContextFactory contextFactory = new WxeContextFactory ();
      _wxeContext = contextFactory.CreateContext (rootFunction);
      _mockRepository = new MockRepository ();
      _mockWxeSecurityAdapter = _mockRepository.StrictMock<IWxeSecurityAdapter> ();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterMultiple<IWxeSecurityAdapter> (() => _mockWxeSecurityAdapter);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
    }

    [TearDown]
    public void TearDown ()
    {
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void ExecuteFunctionWithAccessGranted ()
    {
      TestFunction function = new TestFunction ();
      _mockWxeSecurityAdapter.CheckAccess (function);
      _mockRepository.ReplayAll ();

      function.Execute (_wxeContext);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void HasStatelessAccessGranted ()
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (typeof (TestFunction))).Return (true);
      _mockRepository.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mockRepository.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void HasStatelessAccessDenied ()
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (typeof (TestFunction))).Return (false);
      _mockRepository.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mockRepository.VerifyAll ();
      Assert.That (hasAccess, Is.False);
    }

    [Test]
    public void HasStatelessAccessGrantedWithoutWxeSecurityProvider ()
    {
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterMultiple<IWxeSecurityAdapter>();
      using (new ServiceLocatorScope (serviceLocator))
      {
        _mockRepository.ReplayAll();

        bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

        _mockRepository.VerifyAll();
        Assert.That (hasAccess, Is.True);
      }
    }
  }
}
