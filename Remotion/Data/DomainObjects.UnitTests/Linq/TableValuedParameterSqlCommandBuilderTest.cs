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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Data.DomainObjects.UnitTests.Linq;

[TestFixture]
public class TableValuedParameterSqlCommandBuilderTest
{
  [Test]
  public void AppendCollection_UsesSingleParameter ()
  {
    var collection = new[] { 17, 4, 42 };
    var constantCollectionExpression = new ConstantCollectionExpression(collection);

    var sqlCommandBuilder = new TableValuedParameterSqlCommandBuilder();
    sqlCommandBuilder.AppendCollection(constantCollectionExpression);

    Assert.That(sqlCommandBuilder.GetCommandText(), Is.EqualTo("SELECT [Value] FROM @1"));
    Assert.That(sqlCommandBuilder.GetCommandParameters().Length, Is.EqualTo(1));

    var parameter = sqlCommandBuilder.GetCommandParameters().Single();
    Assert.That(parameter.Name, Is.EqualTo("@1"));
    Assert.That(parameter.Value, Is.SameAs(collection));
  }
}
