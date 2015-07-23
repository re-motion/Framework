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
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Extensions.UnitTests.Utilities
{
  [TestFixture]
  public class PostActionDisposableDecoratorTest
  {
    private IDisposable _innerDisposableMock;
    private IAction _actionMock;

    private PostActionDisposableDecorator _decorator;

    [SetUp]
    public void SetUp ()
    {
      _innerDisposableMock = MockRepository.GenerateStrictMock<IDisposable>();
      _actionMock = MockRepository.GenerateStrictMock<IAction>();
      _decorator = new PostActionDisposableDecorator (_innerDisposableMock, _actionMock.Invoke);
    }

    [Test]
    public void Dispose ()
    {
      var sequence = 0;
      _innerDisposableMock
          .Expect (mock => mock.Dispose())
          .WhenCalled (mi => Assert.That (sequence++, Is.EqualTo (0), "Inner Dispose must be called first."));
      _actionMock
          .Expect (mock => mock.Invoke ())
          .WhenCalled (mi => Assert.That (sequence++, Is.EqualTo (1), "Action must be called second."));

      _decorator.Dispose();

      _innerDisposableMock.VerifyAllExpectations();
      _actionMock.VerifyAllExpectations();
    }

    [Test]
    public void Dispose_Twice ()
    {
      _innerDisposableMock
          .Expect (mock => mock.Dispose ())
          .Repeat.Twice();
      _actionMock
          .Expect (mock => mock.Invoke ())
          .Repeat.Once();

      _decorator.Dispose ();
      _decorator.Dispose ();

      _innerDisposableMock.VerifyAllExpectations ();
      _actionMock.VerifyAllExpectations ();
    }

    [Test]
    public void Dispose_PostActionIsExecuted_EvenWhenAnExceptionIsThrown ()
    {
      var exception = new Exception();

      _innerDisposableMock
          .Expect (mock => mock.Dispose ())
          .Throw (exception);
      _actionMock.Expect (mock => mock.Invoke ());

      Assert.That (() =>_decorator.Dispose (), Throws.Exception.SameAs (exception));

      _innerDisposableMock.VerifyAllExpectations ();
      _actionMock.VerifyAllExpectations ();
    }

    [Test]
    public void Dispose_PostAction_DoesNotSwallowOriginalException ()
    {
      var exception = new Exception ();
      var exception2 = new Exception ();

      _innerDisposableMock
          .Expect (mock => mock.Dispose ())
          .Throw (exception);
      _actionMock
          .Expect (mock => mock.Invoke ())
          .Throw (exception2);

      Assert.That (() => _decorator.Dispose (), Throws.Exception.SameAs (exception));

      _innerDisposableMock.VerifyAllExpectations ();
      _actionMock.VerifyAllExpectations ();
    }

    [Test]
    public void Dispose_PostAction_CanThrowExceptions ()
    {
      var exception = new Exception ();

      _innerDisposableMock.Expect (mock => mock.Dispose ());
      _actionMock
          .Expect (mock => mock.Invoke ())
          .Throw (exception);

      Assert.That (() => _decorator.Dispose (), Throws.Exception.SameAs (exception));

      _innerDisposableMock.VerifyAllExpectations ();
      _actionMock.VerifyAllExpectations ();
    }

    public interface IAction
    {
      void Invoke ();
    }
  }
}