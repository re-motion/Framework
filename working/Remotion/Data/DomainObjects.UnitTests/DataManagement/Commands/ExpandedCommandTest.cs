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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class ExpandedCommandTest
  {
    private MockRepository _mockRepository;

    private IDataManagementCommand _commandMock1;
    private IDataManagementCommand _commandMock2;
    private IDataManagementCommand _commandMock3;

    private IDataManagementCommand _nonExecutableCommandMock1;
    private IDataManagementCommand _nonExecutableCommandMock2;

    private Exception _exception1;
    private Exception _exception2;
    private Exception _exception3;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();

      _commandMock1 = _mockRepository.StrictMock<IDataManagementCommand> ();
      _commandMock1.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);

      _commandMock2 = _mockRepository.StrictMock<IDataManagementCommand> ();
      _commandMock2.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);

      _commandMock3 = _mockRepository.StrictMock<IDataManagementCommand> ();
      _commandMock3.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);

      _nonExecutableCommandMock1 = _mockRepository.StrictMock<IDataManagementCommand> ();
      _nonExecutableCommandMock2 = _mockRepository.StrictMock<IDataManagementCommand> ();

      _exception1 = new Exception ("1");
      _exception2 = new Exception ("2");
      _exception3 = new Exception ("3");

      _nonExecutableCommandMock1.Stub (stub => stub.GetAllExceptions ()).Return (new[] { _exception1, _exception2 });
      _nonExecutableCommandMock2.Stub (stub => stub.GetAllExceptions ()).Return (new[] { _exception3 });
    }

    [Test]
    public void GetNestedCommands ()
    {
      _mockRepository.ReplayAll ();

      var expandedCommand = CreateExpandedCommand ();
      Assert.That (expandedCommand.GetNestedCommands (), Is.EqualTo (new[] { _commandMock1, _commandMock2, _commandMock3 }));
    }

    [Test]
    public void GetAllExceptions_None ()
    {
      _mockRepository.ReplayAll ();

      var expandedCommand = CreateExpandedCommand ();
      Assert.That (expandedCommand.GetAllExceptions (), Is.Empty);
    }

    [Test]
    public void GetAllExceptions_Some ()
    {
      _mockRepository.ReplayAll ();

      var expandedCommand = CreateNonExecutableExpandedCommand ();
      Assert.That (expandedCommand.GetAllExceptions (), Is.EqualTo (new[] { _exception1, _exception2, _exception3 }));
    }

    [Test]
    public void Perform ()
    {
      using (_mockRepository.Ordered ())
      {
        _commandMock1.Expect (mock => mock.Perform ());
        _commandMock2.Expect (mock => mock.Perform ());
        _commandMock3.Expect (mock => mock.Perform ());
      }

      _mockRepository.ReplayAll ();

      var compositeCommand = CreateExpandedCommand ();
      compositeCommand.Perform ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Perform_NonExecutable ()
    {
      _mockRepository.ReplayAll ();

      var nonExecutableComposite = CreateNonExecutableExpandedCommand ();
      Assert.Throws<Exception> (nonExecutableComposite.Perform);

      _commandMock1.AssertWasNotCalled (mock => mock.Perform ());
      _nonExecutableCommandMock1.AssertWasNotCalled (mock => mock.Perform ());
      _nonExecutableCommandMock2.AssertWasNotCalled (mock => mock.Perform ());
    }

    [Test]
    public void Begin ()
    {
      using (_mockRepository.Ordered ())
      {
        _commandMock1.Expect (mock => mock.Begin ());
        _commandMock2.Expect (mock => mock.Begin ());
        _commandMock3.Expect (mock => mock.Begin ());
      }

      _mockRepository.ReplayAll ();

      var compositeCommand = CreateExpandedCommand ();
      compositeCommand.Begin ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Begin_NonExecutable ()
    {
      _mockRepository.ReplayAll ();

      var nonExecutableComposite = CreateNonExecutableExpandedCommand ();
      Assert.Throws<Exception> (nonExecutableComposite.Begin);

      _commandMock1.AssertWasNotCalled (mock => mock.Begin ());
      _nonExecutableCommandMock1.AssertWasNotCalled (mock => mock.Begin ());
      _nonExecutableCommandMock2.AssertWasNotCalled (mock => mock.Begin ());
    }


    [Test]
    public void End ()
    {
      using (_mockRepository.Ordered ())
      {
        _commandMock3.Expect (mock => mock.End ());
        _commandMock2.Expect (mock => mock.End ());
        _commandMock1.Expect (mock => mock.End ());
      }

      _mockRepository.ReplayAll ();

      var compositeCommand = CreateExpandedCommand ();
      compositeCommand.End ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void End_NonExecutable ()
    {
      _mockRepository.ReplayAll ();

      var nonExecutableComposite = CreateNonExecutableExpandedCommand ();
      Assert.Throws<Exception> (nonExecutableComposite.End);

      _commandMock1.AssertWasNotCalled (mock => mock.End ());
      _nonExecutableCommandMock1.AssertWasNotCalled (mock => mock.End ());
      _nonExecutableCommandMock2.AssertWasNotCalled (mock => mock.End ());
    }

    private ExpandedCommand CreateExpandedCommand ()
    {
      return new ExpandedCommand (_commandMock1, _commandMock2, _commandMock3);
    }

    private ExpandedCommand CreateNonExecutableExpandedCommand ()
    {
      return new ExpandedCommand (_commandMock1, _nonExecutableCommandMock1, _nonExecutableCommandMock2);
    }
  }
}