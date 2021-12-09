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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class StorageEntityBasedStorageProviderDefinitionFinderTest : StandardMappingTest
  {
    private StorageEntityBasedStorageProviderDefinitionFinder _finder;

    public override void SetUp ()
    {
      base.SetUp();

      _finder = new StorageEntityBasedStorageProviderDefinitionFinder();
    }

    [Test]
    public void GetStorageProviderDefinition ()
    {
      var classDefinition = Configuration.GetTypeDefinition(typeof(Order));

      var provider = _finder.GetStorageProviderDefinition(classDefinition, null);

      Assert.That(provider, Is.SameAs(TestDomainStorageProviderDefinition));
    }

    [Test]
    public void GetStorageProviderDefinition_NoStorageEntity ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order), baseClass: null);

      Assert.That(
          () => _finder.GetStorageProviderDefinition(typeDefinition, null),
          Throws.InvalidOperationException.With.Message.EqualTo("Cannot obtain storage provider for TypeDefinitions without storage entities. "));
      Assert.That(
          () => _finder.GetStorageProviderDefinition(typeDefinition, "Context"),
          Throws.InvalidOperationException.With.Message.EqualTo("Cannot obtain storage provider for TypeDefinitions without storage entities. Context"));
    }
  }
}
