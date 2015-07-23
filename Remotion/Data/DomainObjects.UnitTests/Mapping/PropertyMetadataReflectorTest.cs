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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class PropertyMetadataReflectorTest
  {
    [Test]
    public void GetStorageClass_WithAttribute_ReturnsStorageClass ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttribute<StorageClassAttribute> (false))
          .Return (new StorageClassAttribute (StorageClass.Transaction));

      var reflector = new PropertyMetadataReflector();
      var result = reflector.GetStorageClass (propertyInformationStub);

      Assert.That (result, Is.EqualTo (StorageClass.Transaction));
    }

    [Test]
    public void GetStorageClass_WithDerivedAttribute_ReturnsStorageClass ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttribute<StorageClassAttribute> (false))
          .Return (new StorageClassNoneAttribute());

      var reflector = new PropertyMetadataReflector();
      var result = reflector.GetStorageClass (propertyInformationStub);

      Assert.That (result, Is.EqualTo (StorageClass.None));
    }

    [Test]
    public void GetStorageClass_WithoutAttribute_ReturnsNull ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttribute<StorageClassAttribute> (false))
          .Return (null);

      var reflector = new PropertyMetadataReflector();
      var result = reflector.GetStorageClass (propertyInformationStub);

      Assert.That (result, Is.Null);
    }
  }
}