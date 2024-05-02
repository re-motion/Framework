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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;

[TestFixture]
public class SimpleDataParameterDefinitionFactoryTest
{
  [Test]
  public void Initialize ()
  {
    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    var factory = new SimpleDataParameterDefinitionFactory(storageTypeInformationProviderStub.Object);

    Assert.That(factory.StorageTypeInformationProvider, Is.SameAs(storageTypeInformationProviderStub.Object));
  }

  [Test]
  public void CreateDataParameterDefinition_WithSimpleValue_ReturnsSimpleDataParameterDefinition ()
  {
    var dummy = "Dummy";

    var storageTypeInformation = Mock.Of<IStorageTypeInformation>();
    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    storageTypeInformationProviderStub.Setup(_ => _.GetStorageType(dummy)).Returns(storageTypeInformation);

    var dataParameterDefinitionFactory = new SimpleDataParameterDefinitionFactory(storageTypeInformationProviderStub.Object);

    var result = dataParameterDefinitionFactory.CreateDataParameterDefinition(new QueryParameter("dummy", dummy));

    Assert.That(result, Is.InstanceOf<SimpleDataParameterDefinition>());
    Assert.That(result.As<SimpleDataParameterDefinition>().StorageTypeInformation, Is.SameAs(storageTypeInformation));
  }
}
