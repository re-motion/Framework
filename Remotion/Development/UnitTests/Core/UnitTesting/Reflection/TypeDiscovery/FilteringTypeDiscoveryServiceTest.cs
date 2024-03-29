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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Reflection.TypeDiscovery
{
  [TestFixture]
  public class FilteringTypeDiscoveryServiceTest
  {
    [Test]
    public void CreateFromNamespaceWhitelist ()
    {
      var types = new[]
                  {
                      typeof(Color),
                      typeof(Brush),
                      typeof(DateTime)
                  };

      var decoratedTypeDiscoveryServiceMock = new Mock<ITypeDiscoveryService>();

      decoratedTypeDiscoveryServiceMock.Setup(service => service.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(types);

      var filteringTypeDiscoveryService =
          FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist(decoratedTypeDiscoveryServiceMock.Object, typeof(Color).Namespace);
      var filteredTypes = filteringTypeDiscoveryService.GetTypes(null, false);

      Assert.That(
          filteredTypes.Cast<Type>().ToArray(),
          Is.EqualTo(
              new[]
              {
                typeof(Color),
                typeof(Brush)
              }));
    }

    [Test]
    public void CreateFromNamespaceWhitelist_IncludesSubNamespaces ()
    {
      var types = new[]
                  {
                      typeof(Color),
                      typeof(Brush),
                      typeof(DateTime),
                      typeof(ImageFlags)
                  };

      var decoratedTypeDiscoveryServiceMock = new Mock<ITypeDiscoveryService>();

      decoratedTypeDiscoveryServiceMock.Setup(service => service.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(types);

      var filteringTypeDiscoveryService =
          FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist(decoratedTypeDiscoveryServiceMock.Object, typeof(Color).Namespace);
      var filteredTypes = filteringTypeDiscoveryService.GetTypes(null, false);

      Assert.That(
          filteredTypes.Cast<Type>().ToArray(),
          Is.EqualTo(
              new[]
              {
                typeof(Color),
                typeof(Brush),
                typeof(ImageFlags)
              }));
    }

    [Test]
    public void CreateFromNamespaceWhitelist_AllowsTypesWithoutNamespace_Exclusion ()
    {
      var types = new[] { typeof(TypeWithNullNamespace) };

      var decoratedTypeDiscoveryServiceMock = new Mock<ITypeDiscoveryService>();

      decoratedTypeDiscoveryServiceMock.Setup(service => service.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(types);

      var filteringTypeDiscoveryService =
          FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist(decoratedTypeDiscoveryServiceMock.Object, typeof(Color).Namespace);
      var filteredTypes = filteringTypeDiscoveryService.GetTypes(null, false);

      Assert.That(filteredTypes, Is.Empty);
    }

    [Test]
    public void CreateFromNamespaceWhitelist_AllowsTypesWithoutNamespace_Inclusion ()
    {
      var types = new[] { typeof(TypeWithNullNamespace) };

      var decoratedTypeDiscoveryServiceMock = new Mock<ITypeDiscoveryService>();

      decoratedTypeDiscoveryServiceMock.Setup(service => service.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(types);

      var filteringTypeDiscoveryService =
          FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist(decoratedTypeDiscoveryServiceMock.Object, "");
      var filteredTypes = filteringTypeDiscoveryService.GetTypes(null, false);

      Assert.That(filteredTypes, Is.EqualTo(types));
    }

    [Test]
    public void CreateFromNamespaceBlacklist ()
    {
      var types = new[]
                  {
                      typeof(Color),
                      typeof(Brush),
                      typeof(DateTime)
                  };

      var decoratedTypeDiscoveryServiceMock = new Mock<ITypeDiscoveryService>();

      decoratedTypeDiscoveryServiceMock.Setup(service => service.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(types);

      var filteringTypeDiscoveryService =
          FilteringTypeDiscoveryService.CreateFromNamespaceBlacklist(decoratedTypeDiscoveryServiceMock.Object, typeof(Color).Namespace);
      var filteredTypes = filteringTypeDiscoveryService.GetTypes(null, false);

      Assert.That(
          filteredTypes.Cast<Type>().ToArray(),
          Is.EqualTo(
              new[]
              {
                typeof(DateTime)
              }));
    }

    [Test]
    public void CreateFromNamespaceBlacklist_IncludesSubNamespaces ()
    {
      var types = new[]
                  {
                      typeof(Color),
                      typeof(Brush),
                      typeof(DateTime),
                      typeof(ImageFlags)
                  };

      var decoratedTypeDiscoveryServiceMock = new Mock<ITypeDiscoveryService>();

      decoratedTypeDiscoveryServiceMock.Setup(service => service.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(types);

      var filteringTypeDiscoveryService =
          FilteringTypeDiscoveryService.CreateFromNamespaceBlacklist(decoratedTypeDiscoveryServiceMock.Object, typeof(Color).Namespace);
      var filteredTypes = filteringTypeDiscoveryService.GetTypes(null, false);

      Assert.That(
          filteredTypes.Cast<Type>().ToArray(),
          Is.EqualTo(
              new[]
              {
                typeof(DateTime),
              }));
    }

    [Test]
    public void CreateFromNamespaceBlacklist_AllowsTypesWithoutNamespace_Exclusion ()
    {
      var types = new[] { typeof(TypeWithNullNamespace) };

      var decoratedTypeDiscoveryServiceMock = new Mock<ITypeDiscoveryService>();

      decoratedTypeDiscoveryServiceMock.Setup(service => service.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(types);

      var filteringTypeDiscoveryService =
          FilteringTypeDiscoveryService.CreateFromNamespaceBlacklist(decoratedTypeDiscoveryServiceMock.Object, "");
      var filteredTypes = filteringTypeDiscoveryService.GetTypes(null, false);

      Assert.That(filteredTypes, Is.Empty);
    }

    [Test]
    public void CreateFromNamespaceBlacklist_AllowsTypesWithoutNamespace_Inclusion ()
    {
      var types = new[] { typeof(TypeWithNullNamespace) };

      var decoratedTypeDiscoveryServiceMock = new Mock<ITypeDiscoveryService>();

      decoratedTypeDiscoveryServiceMock.Setup(service => service.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(types);

      var filteringTypeDiscoveryService =
          FilteringTypeDiscoveryService.CreateFromNamespaceBlacklist(decoratedTypeDiscoveryServiceMock.Object, typeof(Color).Namespace);
      var filteredTypes = filteringTypeDiscoveryService.GetTypes(null, false);

      Assert.That(filteredTypes, Is.EqualTo(types));
    }
  }
}

public class TypeWithNullNamespace
{
}
