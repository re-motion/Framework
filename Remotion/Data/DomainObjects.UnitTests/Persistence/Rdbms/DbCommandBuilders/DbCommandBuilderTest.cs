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
using System.Data;
using System.Text;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class DbCommandBuilderTest : StandardMappingTest
  {
    private Mock<ISqlDialect> _sqlDialectStub;
    private TestableDbCommandBuilder _commandBuilder;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = new Mock<ISqlDialect>();
      _commandBuilder = new TestableDbCommandBuilder(_sqlDialectStub.Object);
    }

    [Test]
    public void AppendWhereClause ()
    {
      var statement = new StringBuilder();
      var command = new Mock<IDbCommand>();

      var specificationMock = new Mock<IComparedColumnsSpecification> (MockBehavior.Strict);
      specificationMock.Setup (mock => mock.AddParameters (command.Object, _sqlDialectStub.Object)).Verifiable();
      specificationMock
          .Setup(mock => mock.AppendComparisons(statement, command.Object, _sqlDialectStub.Object))
          .Callback(
              (StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect) =>
              {
                Assert.That(statement.ToString(), Is.EqualTo(" WHERE "));
                statement.Append("<conditions>");
              })
          .Verifiable();

      _commandBuilder.AppendWhereClause(statement, command.Object, specificationMock.Object);

      specificationMock.Verify();
    }

    [Test]
    public void AppendOrderByClause ()
    {
      var statement = new StringBuilder();
      var command = new Mock<IDbCommand>();

      var specificationMock = new Mock<IOrderedColumnsSpecification> (MockBehavior.Strict);
      specificationMock.Setup (mock => mock.IsEmpty).Returns (false).Verifiable();
      specificationMock
          .Setup(mock => mock.AppendOrderings(statement, _sqlDialectStub.Object))
          .Callback(
              (StringBuilder stringBuilder, ISqlDialect sqlDialect) =>
              {
                Assert.That(statement.ToString(), Is.EqualTo(" ORDER BY "));
                statement.Append("orders...");
              })
          .Verifiable();
      specificationMock.Object.Replay();

      _commandBuilder.AppendOrderByClause(statement, command.Object, specificationMock.Object);

      specificationMock.Verify();
      Assert.That(statement.ToString(), Is.EqualTo(" ORDER BY orders..."));
    }

  }
}
