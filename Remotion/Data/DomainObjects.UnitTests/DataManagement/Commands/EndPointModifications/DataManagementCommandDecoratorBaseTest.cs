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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class DataManagementCommandDecoratorBaseTest
  {
    private Mock<IDataManagementCommand> _decoratedCommandMock;
    private TestableDataManagementCommandDecoratorBase _decorator;

    [SetUp]
    public void SetUp ()
    {
      _decoratedCommandMock = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      _decorator = new TestableDataManagementCommandDecoratorBase(_decoratedCommandMock.Object);
    }

    [Test]
    public void GetAllExceptions ()
    {
      var exception1 = new Exception("Test1");
      var exception2 = new Exception("Test2");
      var fakeExceptions = new[] { exception1, exception2 };
      _decoratedCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(fakeExceptions);

      Assert.That(_decorator.GetAllExceptions(), Is.EqualTo(fakeExceptions));
    }

    [Test]
    public void Begin ()
    {
      _decoratedCommandMock.Setup(mock => mock.Begin()).Verifiable();

      _decorator.Begin();

      _decoratedCommandMock.Verify();
    }

    [Test]
    public void Perform ()
    {
      _decoratedCommandMock.Setup(mock => mock.Perform()).Verifiable();

      _decorator.Perform();

      _decoratedCommandMock.Verify();
    }

    [Test]
    public void End ()
    {
      _decoratedCommandMock.Setup(mock => mock.End()).Verifiable();

      _decorator.End();

      _decoratedCommandMock.Verify();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var fakeExpanded = new ExpandedCommand();
      _decoratedCommandMock.Setup(stub => stub.ExpandToAllRelatedObjects()).Returns(fakeExpanded);

      var result = _decorator.ExpandToAllRelatedObjects();

      var nestedCommands = result.GetNestedCommands();
      Assert.That(nestedCommands, Has.Count.EqualTo(1));
      var nestedCommand = nestedCommands.Single();
      Assert.That(nestedCommand, Is.TypeOf<TestableDataManagementCommandDecoratorBase>());
      Assert.That(((TestableDataManagementCommandDecoratorBase)nestedCommand).DecoratedCommand, Is.SameAs(fakeExpanded));
    }
  }

  public class TestableDataManagementCommandDecoratorBase : DataManagementCommandDecoratorBase
  {
    public TestableDataManagementCommandDecoratorBase (IDataManagementCommand decoratedCommand)
        : base(decoratedCommand)
    {
    }

    protected override IDataManagementCommand Decorate (IDataManagementCommand decoratedCommand)
    {
      return new TestableDataManagementCommandDecoratorBase(decoratedCommand);
    }
  }
}
