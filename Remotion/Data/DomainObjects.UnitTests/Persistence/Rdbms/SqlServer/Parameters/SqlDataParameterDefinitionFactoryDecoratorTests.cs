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
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Parameters;

[TestFixture]
public class SqlDataParameterDefinitionFactoryDecoratorTests
{
  private static IEnumerable<object> GetCauseDecorationValues ()
  {
    yield return "Dummy";
    yield return "Dummy".ToCharArray();
  }

  [Test]
  [TestCaseSource(nameof(GetCauseDecorationValues))]
  public void DecoratesWith_SqlFulltextDataParameterDefinitionDecorator (object queryParameterValue)
  {
    var queryParameter = new QueryParameter("dummy", queryParameterValue);
    var parameterDefinitionStub = new Mock<IDataParameterDefinition>();

    var factoryStub = new Mock<IDataParameterDefinitionFactory>();
    factoryStub.Setup(_ => _.CreateDataParameterDefinition(queryParameter)).Returns(parameterDefinitionStub.Object);

    var factoryDecorator = new SqlFulltextDataParameterDefinitionFactory(factoryStub.Object);

    var result = factoryDecorator.CreateDataParameterDefinition(queryParameter);
    Assert.That(result, Is.InstanceOf<SqlFulltextDataParameterDefinitionDecorator>());
    Assert.That(result.As<SqlFulltextDataParameterDefinitionDecorator>().InnerDataParameterDefinition, Is.SameAs(parameterDefinitionStub.Object));
  }

  private static IEnumerable<object> GetPreventDecorationValues ()
  {
    yield return 42;
    yield return 17.4;
    yield return new DateTime(2024, 05, 01, 14, 20, 00);
    yield return new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
  }

  [Test]
  [TestCaseSource(nameof(GetPreventDecorationValues))]
  public void DoesNotDecorate (object queryParameterValue)
  {
    var queryParameter = new QueryParameter("dummy", queryParameterValue);
    var parameterDefinitionStub = new Mock<IDataParameterDefinition>();

    var factoryStub = new Mock<IDataParameterDefinitionFactory>();
    factoryStub.Setup(_ => _.CreateDataParameterDefinition(queryParameter)).Returns(parameterDefinitionStub.Object);

    var factoryDecorator = new SqlFulltextDataParameterDefinitionFactory(factoryStub.Object);

    var result = factoryDecorator.CreateDataParameterDefinition(queryParameter);
    Assert.That(result, Is.SameAs(parameterDefinitionStub.Object));
  }
}
