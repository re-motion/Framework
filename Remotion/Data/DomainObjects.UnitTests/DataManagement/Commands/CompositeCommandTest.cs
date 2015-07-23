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
  public class CompositeCommandTest
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
      _mockRepository = new MockRepository();

      _commandMock1 = _mockRepository.StrictMock<IDataManagementCommand>();
      _commandMock1.Stub (stub => stub.GetAllExceptions()).Return (new Exception[0]);

      _commandMock2 = _mockRepository.StrictMock<IDataManagementCommand>();
      _commandMock2.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);

      _commandMock3 = _mockRepository.StrictMock<IDataManagementCommand> ();
      _commandMock3.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);

      _nonExecutableCommandMock1 = _mockRepository.StrictMock<IDataManagementCommand>();
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
      _mockRepository.ReplayAll();

      var compositeCommand = CreateComposite();
      Assert.That (compositeCommand.GetNestedCommands(), Is.EqualTo (new[] { _commandMock1, _commandMock2, _commandMock3 }));
    }

    [Test]
    public void GetAllExceptions_None ()
    {
      _mockRepository.ReplayAll ();

      var compositeCommand = CreateComposite ();
      Assert.That (compositeCommand.GetAllExceptions (), Is.Empty);
    }

    [Test]
    public void GetAllExceptions_Some ()
    {
      _mockRepository.ReplayAll();

      var nonExecutableComposite = CreateNonExecutableComposite();
      Assert.That (nonExecutableComposite.GetAllExceptions (), Is.EqualTo (new[] { _exception1, _exception2, _exception3 }));
    }

    [Test]
    public void Perform ()
    {
      using (_mockRepository.Ordered ())
      {
        _commandMock1.Expect (mock => mock.Perform());
        _commandMock2.Expect (mock => mock.Perform());
        _commandMock3.Expect (mock => mock.Perform());
      }

      _mockRepository.ReplayAll();

      var compositeCommand = CreateComposite();
      compositeCommand.Perform();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Perform_NonExecutable ()
    {
      _mockRepository.ReplayAll ();

      var nonExecutableComposite = CreateNonExecutableComposite();
      Assert.Throws<Exception> (nonExecutableComposite.Perform);

      _commandMock1.AssertWasNotCalled (mock => mock.Perform ());
      _nonExecutableCommandMock1.AssertWasNotCalled (mock => mock.Perform());
      _nonExecutableCommandMock2.AssertWasNotCalled (mock => mock.Perform());
    }

    [Test]
    public void Begin ()
    {
      using (_mockRepository.Ordered ())
      {
        _commandMock1.Expect (mock => mock.Begin());
        _commandMock2.Expect (mock => mock.Begin());
        _commandMock3.Expect (mock => mock.Begin());
      }

      _mockRepository.ReplayAll();

      var compositeCommand = CreateComposite();
      compositeCommand.Begin();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Begin_NonExecutable ()
    {
      _mockRepository.ReplayAll ();

      var nonExecutableComposite = CreateNonExecutableComposite();
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
        _commandMock3.Expect (mock => mock.End());
        _commandMock2.Expect (mock => mock.End());
        _commandMock1.Expect (mock => mock.End());
      }

      _mockRepository.ReplayAll();

      var compositeCommand = CreateComposite();
      compositeCommand.End();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void End_NonExecutable ()
    {
      _mockRepository.ReplayAll ();

      var nonExecutableComposite = CreateNonExecutableComposite();
      Assert.Throws<Exception> (nonExecutableComposite.End);

      _commandMock1.AssertWasNotCalled (mock => mock.End ());
      _nonExecutableCommandMock1.AssertWasNotCalled (mock => mock.End ());
      _nonExecutableCommandMock2.AssertWasNotCalled (mock => mock.End ());
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var expandedStub1A = MockRepository.GenerateStub<IDataManagementCommand>();
      expandedStub1A.Stub (stub => stub.GetAllExceptions()).Return (new Exception[0]);
      var expandedStub1B = MockRepository.GenerateStub<IDataManagementCommand>();
      expandedStub1B.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      var expandedStub2A = MockRepository.GenerateStub<IDataManagementCommand> ();
      expandedStub2A.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      var expandedStub2B = MockRepository.GenerateStub<IDataManagementCommand> ();
      expandedStub2B.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      var expandedStub3A = MockRepository.GenerateStub<IDataManagementCommand> ();
      expandedStub3A.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      var expandedStub3B = MockRepository.GenerateStub<IDataManagementCommand> ();
      expandedStub3B.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);

      _commandMock1.Expect (mock => mock.ExpandToAllRelatedObjects()).Return (new ExpandedCommand (expandedStub1A, expandedStub1B));
      _commandMock2.Expect (mock => mock.ExpandToAllRelatedObjects()).Return (new ExpandedCommand (expandedStub2A, expandedStub2B));
      _commandMock3.Expect (mock => mock.ExpandToAllRelatedObjects()).Return (new ExpandedCommand (expandedStub3A, expandedStub3B));

      _mockRepository.ReplayAll ();

      var compositeCommand = CreateComposite();
      var result = compositeCommand.ExpandToAllRelatedObjects();

      _mockRepository.VerifyAll ();
      Assert.That (
          result.GetNestedCommands(),
          Is.EqualTo (new[] { expandedStub1A, expandedStub1B, expandedStub2A, expandedStub2B, expandedStub3A, expandedStub3B }));
    }
    
    [Test]
    public void CombineWith ()
    {
      _mockRepository.ReplayAll ();

      var otherCommandStub = MockRepository.GenerateStub<IDataManagementCommand> ();
      otherCommandStub.Stub (stub => stub.GetAllExceptions()).Return (new Exception[0]);
      
      var compositeCommand = CreateComposite();
      var result = compositeCommand.CombineWith (otherCommandStub);

      Assert.That (result, Is.Not.SameAs (compositeCommand));
      Assert.That (result.GetNestedCommands(), Is.EqualTo (new[] { _commandMock1, _commandMock2, _commandMock3, otherCommandStub }));
      Assert.That (compositeCommand.GetNestedCommands(), Is.EqualTo (new[] { _commandMock1, _commandMock2, _commandMock3 }));
    }

    [Test]
    public void CombineWith_NonExecutable ()
    {
      _mockRepository.ReplayAll ();
      var compositeCommand = CreateComposite ();
      var result = compositeCommand.CombineWith (_commandMock1, _nonExecutableCommandMock1, _nonExecutableCommandMock2);

      Assert.That (result, Is.Not.SameAs (compositeCommand));
      Assert.That (
          result.GetNestedCommands(),
          Is.EqualTo (new[] { _commandMock1, _commandMock2, _commandMock3, _commandMock1, _nonExecutableCommandMock1, _nonExecutableCommandMock2 }));
      Assert.That (result.GetAllExceptions (), Is.EqualTo (new[] { _exception1, _exception2, _exception3 }));
    }

    [Test]
    public void NotifyAndPerform_IntegrationTest ()
    {
      using (_mockRepository.Ordered())
      {
        _commandMock1.Expect (mock => mock.Begin());
        _commandMock2.Expect (mock => mock.Begin());
        _commandMock3.Expect (mock => mock.Begin());

        _commandMock1.Expect (mock => mock.Perform());
        _commandMock2.Expect (mock => mock.Perform());
        _commandMock3.Expect (mock => mock.Perform());

        _commandMock3.Expect (mock => mock.End());
        _commandMock2.Expect (mock => mock.End());
        _commandMock1.Expect (mock => mock.End());
      }

      _mockRepository.ReplayAll();

      var compositeCommand = CreateComposite();
      compositeCommand.NotifyAndPerform();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void NotifyAndPerform_NonExecutable_IntegrationTest ()
    {
      _mockRepository.ReplayAll ();

      var nonExecutableComposite = CreateNonExecutableComposite();
      var exception = Assert.Throws<Exception> (nonExecutableComposite.NotifyAndPerform);

      Assert.That (exception, Is.SameAs (_exception1));

      _commandMock1.AssertWasNotCalled (mock => mock.Begin ());
      _nonExecutableCommandMock1.AssertWasNotCalled (mock => mock.Begin ());
      _nonExecutableCommandMock2.AssertWasNotCalled (mock => mock.Begin ());

      _commandMock1.AssertWasNotCalled (mock => mock.Begin ());
      _nonExecutableCommandMock1.AssertWasNotCalled (mock => mock.Begin ());
      _nonExecutableCommandMock2.AssertWasNotCalled (mock => mock.Begin ());

      _commandMock1.AssertWasNotCalled (mock => mock.Perform ());
      _nonExecutableCommandMock1.AssertWasNotCalled (mock => mock.Perform ());
      _nonExecutableCommandMock2.AssertWasNotCalled (mock => mock.Perform ());

      _nonExecutableCommandMock2.AssertWasNotCalled (mock => mock.End ());
      _nonExecutableCommandMock1.AssertWasNotCalled (mock => mock.End ());
      _commandMock1.AssertWasNotCalled (mock => mock.End ());

      _nonExecutableCommandMock2.AssertWasNotCalled (mock => mock.End ());
      _nonExecutableCommandMock1.AssertWasNotCalled (mock => mock.End ());
      _commandMock1.AssertWasNotCalled (mock => mock.End ());
    }

    private CompositeCommand CreateComposite ()
    {
      return new CompositeCommand (_commandMock1, _commandMock2, _commandMock3);
    }

    private CompositeCommand CreateNonExecutableComposite ()
    {
      return new CompositeCommand (_commandMock1, _nonExecutableCommandMock1, _nonExecutableCommandMock2);
    }
  }
}