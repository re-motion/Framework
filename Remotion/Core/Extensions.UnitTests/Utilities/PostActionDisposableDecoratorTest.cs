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
using Remotion.Utilities;

namespace Remotion.Extensions.UnitTests.Utilities
{
  [TestFixture]
  public class PostActionDisposableDecoratorTest
  {
    private Mock<IDisposable> _innerDisposableMock;
    private Mock<IAction> _actionMock;

    private PostActionDisposableDecorator _decorator;

    [SetUp]
    public void SetUp ()
    {
      _innerDisposableMock = new Mock<IDisposable>(MockBehavior.Strict);
      _actionMock = new Mock<IAction>(MockBehavior.Strict);
      _decorator = new PostActionDisposableDecorator(_innerDisposableMock.Object, _actionMock.Object.Invoke);
    }

    [Test]
    public void Dispose ()
    {
      var sequence = 0;
      _innerDisposableMock
          .Setup(mock => mock.Dispose())
          .Callback(() => Assert.That(sequence++, Is.EqualTo(0), "Inner Dispose must be called first."))
          .Verifiable();
      _actionMock
          .Setup(mock => mock.Invoke())
          .Callback(() => Assert.That(sequence++, Is.EqualTo(1), "Action must be called second."))
          .Verifiable();

      _decorator.Dispose();

      _innerDisposableMock.Verify();
      _actionMock.Verify();
    }

    [Test]
    public void Dispose_Twice ()
    {
      _innerDisposableMock
          .Setup(mock => mock.Dispose())
          .Verifiable();
      _actionMock
          .Setup(mock => mock.Invoke())
          .Verifiable();

      _decorator.Dispose();
      _decorator.Dispose();

      _innerDisposableMock.Verify(mock => mock.Dispose(), Times.Exactly(2));
      _actionMock.Verify(mock => mock.Invoke(), Times.Once());
    }

    [Test]
    public void Dispose_PostActionIsExecuted_EvenWhenAnExceptionIsThrown ()
    {
      var exception = new Exception();

      _innerDisposableMock
          .Setup(mock => mock.Dispose())
          .Throws(exception)
          .Verifiable();
      _actionMock.Setup(mock => mock.Invoke()).Verifiable();

      Assert.That(() =>_decorator.Dispose(), Throws.Exception.SameAs(exception));

      _innerDisposableMock.Verify();
      _actionMock.Verify();
    }

    [Test]
    public void Dispose_PostAction_DoesNotSwallowOriginalException ()
    {
      var exception = new Exception();
      var exception2 = new Exception();

      _innerDisposableMock
          .Setup(mock => mock.Dispose())
          .Throws(exception)
          .Verifiable();
      _actionMock
          .Setup(mock => mock.Invoke())
          .Throws(exception2)
          .Verifiable();

      Assert.That(() => _decorator.Dispose(), Throws.Exception.SameAs(exception));

      _innerDisposableMock.Verify();
      _actionMock.Verify();
    }

    [Test]
    public void Dispose_PostAction_CanThrowExceptions ()
    {
      var exception = new Exception();

      _innerDisposableMock.Setup(mock => mock.Dispose()).Verifiable();
      _actionMock
          .Setup(mock => mock.Invoke())
          .Throws(exception)
          .Verifiable();

      Assert.That(() => _decorator.Dispose(), Throws.Exception.SameAs(exception));

      _innerDisposableMock.Verify();
      _actionMock.Verify();
    }

    public interface IAction
    {
      void Invoke ();
    }
  }
}