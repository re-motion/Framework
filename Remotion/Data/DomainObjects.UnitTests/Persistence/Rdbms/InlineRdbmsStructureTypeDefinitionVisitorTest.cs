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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class InlineRdbmsStructureTypeDefinitionVisitorTest : StandardMappingTest
  {
    public interface IVisitorCallReceiver
    {
      void HandleTableTypeDefinition (TableTypeDefinition table, Action<IRdbmsStructuredTypeDefinition> continuation);
    }

    private TableTypeDefinition _tableTypeDefinition;
    private Mock<IVisitorCallReceiver> _voidReceiverMock;

    public override void SetUp ()
    {
      base.SetUp();

      _tableTypeDefinition = TableTypeDefinitionObjectMother.Create();

      _voidReceiverMock = new Mock<IVisitorCallReceiver>(MockBehavior.Strict);
    }

    [Test]
    public void Visit_WithoutResult_TableTypeDefinition ()
    {
      _voidReceiverMock.Setup(mock => mock.HandleTableTypeDefinition(_tableTypeDefinition, It.IsAny<Action<IRdbmsStructuredTypeDefinition>>())).Verifiable();

      InlineRdbmsStructuredTypeDefinitionVisitor.Visit(
          _tableTypeDefinition,
          _voidReceiverMock.Object.HandleTableTypeDefinition);

      _voidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithoutResult_Continuation_TableTypeDefinition ()
    {
      var secondTypeDef = TableTypeDefinitionObjectMother.Create("Second");

      _voidReceiverMock
          .Setup(_ => _.HandleTableTypeDefinition(_tableTypeDefinition, It.IsAny<Action<IRdbmsStructuredTypeDefinition>>()))
          .Callback((TableTypeDefinition _, Action<IRdbmsStructuredTypeDefinition> continuation) => continuation(secondTypeDef))
          .Verifiable();

      _voidReceiverMock
          .Setup(_ => _.HandleTableTypeDefinition(secondTypeDef, It.IsAny<Action<IRdbmsStructuredTypeDefinition>>()))
          .Verifiable();

      InlineRdbmsStructuredTypeDefinitionVisitor.Visit(
          _tableTypeDefinition,
          _voidReceiverMock.Object.HandleTableTypeDefinition);

      _voidReceiverMock.Verify();
    }
  }
}
