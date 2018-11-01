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
using Remotion.Data.DomainObjects.DataManagement;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataManagementCommandExtensionsTest
  {
    private IDataManagementCommand _commandMock;

    [SetUp]
    public void SetUp ()
    {
      _commandMock = MockRepository.GenerateStrictMock<IDataManagementCommand> ();
    }

    [Test]
    public void NotifyAndPerform ()
    {
      using (_commandMock.GetMockRepository ().Ordered ())
      {
        _commandMock.Expect (mock => mock.Begin());
        _commandMock.Expect (mock => mock.Perform());
        _commandMock.Expect (mock => mock.End ());
      }

      _commandMock.Replay ();

      _commandMock.NotifyAndPerform ();

      _commandMock.VerifyAllExpectations ();
    }

    [Test]
    public void CanExecute_True ()
    {
      _commandMock.Stub (stub => stub.GetAllExceptions()).Return (new Exception[0]);
      Assert.That (_commandMock.CanExecute(), Is.True);
    }

    [Test]
    public void CanExecute_False ()
    {
      var exception1 = new Exception ("1");
      var exception2 = new Exception ("2");

      _commandMock.Stub (stub => stub.GetAllExceptions()).Return (new[] { exception1, exception2 });
      Assert.That (_commandMock.CanExecute(), Is.False);
    }

    [Test]
    public void EnsureCanExecute_NoExceptions ()
    {
      _commandMock.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      _commandMock.EnsureCanExecute ();
    }

    [Test]
    public void EnsureCanExecute_WithExceptions ()
    {
      var exception1 = new Exception ("1");
      var exception2 = new Exception ("2");
      _commandMock.Stub (stub => stub.GetAllExceptions ()).Return (new[] { exception1, exception2 });

      var exception = Assert.Throws<Exception> (() => _commandMock.EnsureCanExecute());
      Assert.That (exception, Is.SameAs (exception1));
    }
  }
}