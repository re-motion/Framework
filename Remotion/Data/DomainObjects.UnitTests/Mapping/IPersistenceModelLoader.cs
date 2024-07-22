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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class IPersistenceModelLoaderTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      _serviceLocator.RegisterSingle(() => Mock.Of<IStorageSettings>());
    }

    [Test]
    public void GetInstance_Once ()
    {
      var domainModelConstraintProvider = _serviceLocator.GetInstance<IPersistenceModelLoader>();

      Assert.That(domainModelConstraintProvider, Is.TypeOf<PersistenceModelLoader>());
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var propertyDefaultValueProvider1 = _serviceLocator.GetInstance<IPersistenceModelLoader>();
      var propertyDefaultValueProvider2 = _serviceLocator.GetInstance<IPersistenceModelLoader>();

      Assert.That(propertyDefaultValueProvider1, Is.SameAs(propertyDefaultValueProvider2));
    }
  }
}
