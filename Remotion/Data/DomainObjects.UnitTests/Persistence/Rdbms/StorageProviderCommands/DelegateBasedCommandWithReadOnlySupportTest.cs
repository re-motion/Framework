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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class DelegateBasedCommandWithReadOnlySupportTest
  {
    [Test]
    public void Execute ()
    {
      var executionContext = new Mock<IRdbmsProviderCommandExecutionContext>();
      var innerCommandStub = new Mock<IRdbmsProviderCommandWithReadOnlySupport<string>>();
      var delegateBasedCommand = new DelegateBasedCommandWithReadOnlySupport<string, int>(innerCommandStub.Object, s => s.Length);

      innerCommandStub.Setup(stub => stub.Execute(executionContext.Object)).Returns("Test1");

      var result = delegateBasedCommand.Execute(executionContext.Object);

      Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public void Create ()
    {
      var innerCommandStub = new Mock<IRdbmsProviderCommandWithReadOnlySupport<string>>();
      Func<string, int> operation = s => s.Length;
      var instance = DelegateBasedCommand.CreateForReadOnly(innerCommandStub.Object, operation);

      Assert.That(instance, Is.TypeOf(typeof(DelegateBasedCommandWithReadOnlySupport<string, int>)));
      Assert.That(instance.Command, Is.SameAs(innerCommandStub.Object));
      Assert.That(instance.Operation, Is.SameAs(operation));
    }
  }
}
