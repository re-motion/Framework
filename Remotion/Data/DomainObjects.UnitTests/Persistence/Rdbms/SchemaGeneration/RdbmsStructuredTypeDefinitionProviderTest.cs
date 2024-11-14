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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class RdbmsStructuredTypeDefinitionProviderTest : SchemaGenerationTestBase
  {
    [Test]
    public void GetTypeDefinitions_UsesSimpleStructuredTypeDefinitionRepository ()
    {
      var structuredTypeDefinitions = new[]
                                      {
                                          Mock.Of<IRdbmsStructuredTypeDefinition>(),
                                          Mock.Of<IRdbmsStructuredTypeDefinition>(),
                                          Mock.Of<IRdbmsStructuredTypeDefinition>()
                                      };

      var simpleStructuredTypeDefinitionRepositoryStub = new Mock<ISingleScalarStructuredTypeDefinitionProvider>();
      simpleStructuredTypeDefinitionRepositoryStub.Setup(_ => _.GetAllStructuredTypeDefinitions()).Returns(structuredTypeDefinitions);

      var storageObjectFactoryStub = new Mock<IRdbmsStorageObjectFactory>();
      var rdbmsProviderDefinition = new RdbmsProviderDefinition("Test", storageObjectFactoryStub.Object, "whatever", "whatever");

      storageObjectFactoryStub
          .Setup(_ => _.CreateSingleScalarStructuredTypeDefinitionProvider(rdbmsProviderDefinition))
          .Returns(simpleStructuredTypeDefinitionRepositoryStub.Object);

      var provider = new RdbmsStructuredTypeDefinitionProvider();
      var result = provider.GetTypeDefinitions(rdbmsProviderDefinition).ToArray();

      Assert.That(result, Is.EqualTo(structuredTypeDefinitions));
    }
  }
}
