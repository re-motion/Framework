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
using Remotion.Data.DomainObjects.DataManagement.Commands;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class ExceptionCommandTest
  {
    private Exception _exception;
    private ExceptionCommand _command;

    [SetUp]
    public void SetUp ()
    {
      _exception = new Exception ("Test");
      _command = new ExceptionCommand (_exception);
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That (_command.GetAllExceptions(), Is.EqualTo (new[] { _exception }));
    }

    [Test]
    public void Begin ()
    {
      var exception = Assert.Throws<Exception> (_command.Begin);
      Assert.That (exception, Is.SameAs (_exception));
    }

    [Test]
    public void Perform ()
    {
      var exception = Assert.Throws<Exception> (_command.Perform);
      Assert.That (exception, Is.SameAs (_exception));
    }

    [Test]
    public void End ()
    {
      var exception = Assert.Throws<Exception> (_command.End);
      Assert.That (exception, Is.SameAs (_exception));
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var result = _command.ExpandToAllRelatedObjects();

      Assert.That (result.GetNestedCommands(), Is.EqualTo (new[] { _command }));
    }
  }
}