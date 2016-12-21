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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class DbCommandBuilderTest : StandardMappingTest
  {
    private ISqlDialect _sqlDialectStub;
    private TestableDbCommandBuilder _commandBuilder;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect> ();
      _commandBuilder = new TestableDbCommandBuilder (_sqlDialectStub);
    }

    [Test]
    public void AppendWhereClause ()
    {
      var statement = new StringBuilder();
      var command = MockRepository.GenerateStub<IDbCommand>();
      
      var specificationMock = MockRepository.GenerateStrictMock<IComparedColumnsSpecification>();
      specificationMock.Expect (mock => mock.AddParameters (command, _sqlDialectStub));
      specificationMock
          .Expect (mock => mock.AppendComparisons (statement, command, _sqlDialectStub))
          .WhenCalled (
              mi =>
              {
                Assert.That (statement.ToString(), Is.EqualTo (" WHERE "));
                statement.Append ("<conditions>");
              });
      specificationMock.Replay();

      _commandBuilder.AppendWhereClause (statement, specificationMock, command);

      specificationMock.VerifyAllExpectations();
    }

    [Test]
    public void AppendOrderByClause ()
    {
      var statement = new StringBuilder ();

      var specificationMock = MockRepository.GenerateStrictMock<IOrderedColumnsSpecification> ();
      specificationMock.Expect (mock => mock.IsEmpty).Return (false);
      specificationMock
          .Expect (mock => mock.AppendOrderings(statement, _sqlDialectStub))
          .WhenCalled (
              mi =>
              {
                Assert.That (statement.ToString (), Is.EqualTo(" ORDER BY "));
                statement.Append ("orders...");
              });
      specificationMock.Replay ();

      _commandBuilder.AppendOrderByClause (statement, specificationMock);

      specificationMock.VerifyAllExpectations();
      Assert.That (statement.ToString (), Is.EqualTo(" ORDER BY orders..."));
    }
    
  }
}