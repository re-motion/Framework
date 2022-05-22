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

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.IntegrationTests
{
  [TestFixture]
  public class AttributesOnUnsupportedTypesIntegrationTests : ValidationIntegrationTestBase
  {
    //MappingAttributesAreSupportedForPropertyTypeValidationRule
    [Test]
    public void AttributesOnUnsupportedTypes ()
    {
      Assert.That(
          () => ValidateMapping("AttributesOnUnsupportedTypes"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The 'StringPropertyAttribute' may be only applied to properties of type 'String'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.AttributesOnUnsupportedTypes.ClassWithAttributesOnUnsupportedTypes\r\n"
                  + "Property: IntPropertyWithStringPropertyAttribute\r\n"
                  + "----------\r\n"
                  + "The 'BinaryPropertyAttribute' may be only applied to properties of type 'Byte[]'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.AttributesOnUnsupportedTypes.ClassWithAttributesOnUnsupportedTypes\r\n"
                  + "Property: BoolPropertyWithBinaryPropertyAttribute\r\n"
                  + "----------\r\n"
                  + "The 'ExtensibleEnumPropertyAttribute' may be only applied to properties of type 'IExtensibleEnum'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.AttributesOnUnsupportedTypes.ClassWithAttributesOnUnsupportedTypes\r\n"
                  + "Property: StringPropertyWithExtensibleEnumPropertyAttribute\r\n"
                  + "----------\r\n"
                  + "The 'MandatoryAttribute' may be only applied to properties assignable to types 'IDomainObject', 'ObjectList`1', or 'IObjectList`1'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.AttributesOnUnsupportedTypes.ClassWithAttributesOnUnsupportedTypes\r\n"
                  + "Property: StringPropertyWithMandatoryPropertyAttribute\r\n"
                  + "----------\r\n"
                  + "The 'DBBidirectionalRelationAttribute' may be only applied to properties assignable to types 'IDomainObject', 'ObjectList`1', or 'IObjectList`1'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.AttributesOnUnsupportedTypes.ClassWithAttributesOnUnsupportedTypes\r\n"
                  + "Property: StringPropertyWithBidirectionalRelationAttribute"));
    }
  }
}
