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
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class SecurityExecutionListenerTest
  {
    private Mock<IWxeSecurityAdapter> _securityAdapterMock;
    private Mock<IWxeFunctionExecutionListener> _innerListenerMock;
    private TestFunction _function;
    private WxeContext _wxeContext;

    [SetUp]
    public void SetUp ()
    {
      _wxeContext = WxeContextFactory.Create(new TestFunction());

      _securityAdapterMock = new Mock<IWxeSecurityAdapter>(MockBehavior.Strict);
      _innerListenerMock = new Mock<IWxeFunctionExecutionListener>(MockBehavior.Strict);

      _function = new TestFunction();
    }

    [Test]
    public void Initialization ()
    {
      var securityListener = CreateSecurityListener(_securityAdapterMock.Object);

      Assert.That(((SecurityExecutionListener)securityListener).Function, Is.SameAs(_function));
      Assert.That(((SecurityExecutionListener)securityListener).InnerListener, Is.SameAs(_innerListenerMock.Object));
    }

    [Test]
    public void ExecutionPlay_WithAccessGranted ()
    {
      var securityListener = CreateSecurityListener(_securityAdapterMock.Object);

      var sequence = new VerifiableSequence();
      _securityAdapterMock.InVerifiableSequence(sequence).Setup(mock => mock.CheckAccess(_function)).Verifiable();
      _innerListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionPlay(_wxeContext)).Verifiable();

      securityListener.OnExecutionPlay(_wxeContext);

      _securityAdapterMock.Verify();
      _innerListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecutionPlay_WithAccessDenied ()
    {
      _securityAdapterMock.Setup(mock => mock.CheckAccess(_function)).Throws(new PermissionDeniedException()).Verifiable();

      var securityListener = CreateSecurityListener(_securityAdapterMock.Object);

      Assert.That(
          () => securityListener.OnExecutionPlay(_wxeContext),
          Throws.InstanceOf<PermissionDeniedException>());
      _securityAdapterMock.Verify();
      _innerListenerMock.Verify();
    }

    [Test]
    public void ExecutionPlay_WithoutWxeSecurityProvider ()
    {
      _innerListenerMock.Setup(mock => mock.OnExecutionPlay(_wxeContext)).Verifiable();

      var securityListener = CreateSecurityListener(null);

      securityListener.OnExecutionPlay(_wxeContext);

      _securityAdapterMock.Verify();
      _innerListenerMock.Verify();
    }

    [Test]
    public void ExecutionPlay_AfterExecutionStarted ()
    {
      _function.Execute(_wxeContext);
      _securityAdapterMock.Reset();
      _innerListenerMock.Reset();
      _innerListenerMock.Setup(mock => mock.OnExecutionPlay(_wxeContext)).Verifiable();

      var securityListener = CreateSecurityListener(_securityAdapterMock.Object);

      securityListener.OnExecutionPlay(_wxeContext);

      _securityAdapterMock.Verify();
      _innerListenerMock.Verify();
    }

    [Test]
    public void OnExecutionStop ()
    {
      _innerListenerMock.Setup(mock => mock.OnExecutionStop(_wxeContext)).Verifiable();

      var securityListener = CreateSecurityListener(_securityAdapterMock.Object);

      securityListener.OnExecutionStop(_wxeContext);

      _securityAdapterMock.Verify();
      _innerListenerMock.Verify();
    }

    [Test]
    public void OnExecutionPause ()
    {
      _innerListenerMock.Setup(mock => mock.OnExecutionPause(_wxeContext)).Verifiable();

      var securityListener = CreateSecurityListener(_securityAdapterMock.Object);

      securityListener.OnExecutionPause(_wxeContext);

      _securityAdapterMock.Verify();
      _innerListenerMock.Verify();
    }

    [Test]
    public void OnExecutionFail ()
    {
      Exception exception = new Exception();
      _innerListenerMock.Setup(mock => mock.OnExecutionFail(_wxeContext, exception)).Verifiable();

      var securityListener = CreateSecurityListener(_securityAdapterMock.Object);

      securityListener.OnExecutionFail(_wxeContext, exception);

      _securityAdapterMock.Verify();
      _innerListenerMock.Verify();
    }

    [Test]
    public void IsNull ()
    {
      var securityListener = CreateSecurityListener(_securityAdapterMock.Object);

      Assert.That(securityListener.IsNull, Is.False);
    }

    private IWxeFunctionExecutionListener CreateSecurityListener (IWxeSecurityAdapter securityAdapter)
    {
      return new SecurityExecutionListener(_function, _innerListenerMock.Object, securityAdapter);
    }
  }
}
