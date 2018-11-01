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
using Remotion.Data.DomainObjects.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class UnregisterDataContainerCommandTest : StandardMappingTest
  {
    private DataContainerMap _map;
    private DataContainer _dataContainer1;

    private UnregisterDataContainerCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _map = new DataContainerMap (MockRepository.GenerateStub<IClientTransactionEventSink>());
      _dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _map.Register (_dataContainer1);

      _command = new UnregisterDataContainerCommand (DomainObjectIDs.Order1, _map);
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That (_command.GetAllExceptions(), Is.Empty);
    }

    [Test]
    public void Begin_DoesNothing ()
    {
      _command.Begin();
    }

    [Test]
    public void Perform ()
    {
      Assert.That (_map[DomainObjectIDs.Order1], Is.Not.Null);

      _command.Perform();

      Assert.That (_map[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void End_DoesNothing ()
    {
      _command.End();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var result = _command.ExpandToAllRelatedObjects();

      Assert.That (result.GetNestedCommands(), Is.EqualTo (new[] { _command }));
    }
  }
}