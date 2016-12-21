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
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class SecurityExecutionListenerTest
  {
    private MockRepository _mockRepository;
    private IWxeSecurityAdapter _securityAdapterMock;
    private IWxeFunctionExecutionListener _innerListenerMock;
    private TestFunction _function;
    private WxeContext _wxeContext;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _wxeContext = wxeContextFactory.CreateContext (new TestFunction());

      _securityAdapterMock = _mockRepository.StrictMock<IWxeSecurityAdapter>();
      _innerListenerMock = _mockRepository.StrictMock<IWxeFunctionExecutionListener>();

      _function = new TestFunction();
    }

    [Test]
    public void Initialization ()
    {
      var securityListener = CreateSecurityListener (_securityAdapterMock);

      Assert.That (((SecurityExecutionListener) securityListener).Function, Is.SameAs (_function));
      Assert.That (((SecurityExecutionListener) securityListener).InnerListener, Is.SameAs (_innerListenerMock));
    }

    [Test]
    public void ExecutionPlay_WithAccessGranted ()
    {
      var securityListener = CreateSecurityListener (_securityAdapterMock);

      using (_mockRepository.Ordered())
      {
        _securityAdapterMock.Expect (mock => mock.CheckAccess (_function));
        _innerListenerMock.Expect (mock => mock.OnExecutionPlay (_wxeContext));
      }
      _mockRepository.ReplayAll();

      securityListener.OnExecutionPlay (_wxeContext);

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void ExecutionPlay_WithAccessDenied ()
    {
      _securityAdapterMock.Expect (mock => mock.CheckAccess (_function)).Throw (new PermissionDeniedException());
      _mockRepository.ReplayAll();

      var securityListener = CreateSecurityListener (_securityAdapterMock);

      securityListener.OnExecutionPlay (_wxeContext);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecutionPlay_WithoutWxeSecurityProvider ()
    {
      _innerListenerMock.Expect (mock => mock.OnExecutionPlay (_wxeContext));
      _mockRepository.ReplayAll();

      var securityListener = CreateSecurityListener (null);

      securityListener.OnExecutionPlay (_wxeContext);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecutionPlay_AfterExecutionStarted ()
    {
      _function.Execute (_wxeContext);
      _mockRepository.BackToRecordAll();
      _innerListenerMock.Expect (mock => mock.OnExecutionPlay (_wxeContext));
      _mockRepository.ReplayAll();

      var securityListener = CreateSecurityListener (_securityAdapterMock);

      securityListener.OnExecutionPlay (_wxeContext);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OnExecutionStop ()
    {
      _innerListenerMock.Expect (mock => mock.OnExecutionStop (_wxeContext));
      _mockRepository.ReplayAll();

      var securityListener = CreateSecurityListener (_securityAdapterMock);

      securityListener.OnExecutionStop (_wxeContext);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OnExecutionPause ()
    {
      _innerListenerMock.Expect (mock => mock.OnExecutionPause (_wxeContext));
      _mockRepository.ReplayAll();

      var securityListener = CreateSecurityListener (_securityAdapterMock);

      securityListener.OnExecutionPause (_wxeContext);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OnExecutionFail ()
    {
      Exception exception = new Exception();
      _innerListenerMock.Expect (mock => mock.OnExecutionFail (_wxeContext, exception));
      _mockRepository.ReplayAll();

      var securityListener = CreateSecurityListener (_securityAdapterMock);

      securityListener.OnExecutionFail (_wxeContext, exception);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void IsNull ()
    {
      var securityListener = CreateSecurityListener (_securityAdapterMock);

      Assert.That (securityListener.IsNull, Is.False);
    }

    private IWxeFunctionExecutionListener CreateSecurityListener (IWxeSecurityAdapter securityAdapter)
    {
      return new SecurityExecutionListener (_function, _innerListenerMock, securityAdapter);
    }
  }
}