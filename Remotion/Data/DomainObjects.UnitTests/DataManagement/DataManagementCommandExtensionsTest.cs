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
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataManagementCommandExtensionsTest
  {
    private Mock<IDataManagementCommand> _commandMock;

    [SetUp]
    public void SetUp ()
    {
      _commandMock = new Mock<IDataManagementCommand>(MockBehavior.Strict);
    }

    [Test]
    public void NotifyAndPerform ()
    {
      var sequence = new MockSequence();
      _commandMock.InSequence(sequence).Setup(mock => mock.Begin()).Verifiable();
      _commandMock.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _commandMock.InSequence(sequence).Setup(mock => mock.End()).Verifiable();

      DataManagementCommandExtensions.NotifyAndPerform(_commandMock.Object);

      _commandMock.Verify();
    }

    [Test]
    public void CanExecute_True ()
    {
      _commandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      Assert.That(DataManagementCommandExtensions.CanExecute(_commandMock.Object), Is.True);
    }

    [Test]
    public void CanExecute_False ()
    {
      var exception1 = new Exception("1");
      var exception2 = new Exception("2");

      _commandMock.Setup(stub => stub.GetAllExceptions()).Returns(new[] { exception1, exception2 });
      Assert.That(DataManagementCommandExtensions.CanExecute(_commandMock.Object), Is.False);
    }

    [Test]
    public void EnsureCanExecute_NoExceptions ()
    {
      _commandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      DataManagementCommandExtensions.EnsureCanExecute(_commandMock.Object);
    }

    [Test]
    public void EnsureCanExecute_WithExceptions ()
    {
      var exception1 = new Exception("1");
      var exception2 = new Exception("2");
      _commandMock.Setup(stub => stub.GetAllExceptions()).Returns(new[] { exception1, exception2 });

      var exception = Assert.Throws<Exception>(() => DataManagementCommandExtensions.EnsureCanExecute(_commandMock.Object));
      Assert.That(exception, Is.SameAs(exception1));
    }
  }
}
