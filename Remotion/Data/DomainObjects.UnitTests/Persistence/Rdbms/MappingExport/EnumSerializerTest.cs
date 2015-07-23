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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class EnumSerializerTest : SchemaGenerationTestBase
  {
    private EnumSerializer _enumSerializer;

    public override void SetUp ()
    {
      base.SetUp();
      _enumSerializer = new EnumSerializer();
    }

    [Test]
    public void Serialize_CreatesEnumTypeElement ()
    {
      _enumSerializer.CollectPropertyType (GetPropertyDefinition((ClassWithAllDataTypes _) => _.EnumProperty));
      var actual = _enumSerializer.Serialize ().Single();

      Assert.That (actual.Name.LocalName, Is.EqualTo ("enumType"));
    }

    [Test]
    public void Serialize_AddsTypeAttribute ()
    {
      _enumSerializer.CollectPropertyType (GetPropertyDefinition ((ClassWithAllDataTypes _) => _.EnumProperty));
      var actual = _enumSerializer.Serialize().Single();

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("type"));
      Assert.That (
          actual.Attribute ("type").Value,
          Is.EqualTo (
              "Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.ClassWithAllDataTypes+EnumType, Remotion.Data.DomainObjects.UnitTests"));
    }

    [Test]
    public void Serialize_AddsValueElementsForSimpleEnumType ()
    {
      _enumSerializer.CollectPropertyType (GetPropertyDefinition((ClassWithAllDataTypes _) => _.EnumProperty));
      var actual = _enumSerializer.Serialize ().Single();

      Assert.That (actual.Elements().Count(), Is.EqualTo (3));

      var firstValueElement = actual.Elements().ElementAt (0);
      Assert.That (firstValueElement.Name.LocalName, Is.EqualTo ("value"));
      Assert.That (firstValueElement.Attributes().Select (a => a.Name.LocalName), Contains.Item ("name"));
      Assert.That (firstValueElement.Attribute ("name").Value, Is.EqualTo ("Value0"));
      Assert.That (firstValueElement.Attributes().Select (a => a.Name.LocalName), Contains.Item ("columnValue"));
      Assert.That (firstValueElement.Attribute ("columnValue").Value, Is.EqualTo ("0"));

      var secondValueElement = actual.Elements().ElementAt (1);
      Assert.That (secondValueElement.Name.LocalName, Is.EqualTo ("value"));
      Assert.That (secondValueElement.Attributes().Select (a => a.Name.LocalName), Contains.Item ("name"));
      Assert.That (secondValueElement.Attribute ("name").Value, Is.EqualTo ("Value1"));
      Assert.That (secondValueElement.Attributes().Select (a => a.Name.LocalName), Contains.Item ("columnValue"));
      Assert.That (secondValueElement.Attribute ("columnValue").Value, Is.EqualTo ("1"));

      var thirdValueElement = actual.Elements().ElementAt (2);
      Assert.That (thirdValueElement.Name.LocalName, Is.EqualTo ("value"));
      Assert.That (thirdValueElement.Attributes().Select (a => a.Name.LocalName), Contains.Item ("name"));
      Assert.That (thirdValueElement.Attribute ("name").Value, Is.EqualTo ("Value2"));
      Assert.That (thirdValueElement.Attributes().Select (a => a.Name.LocalName), Contains.Item ("columnValue"));
      Assert.That (thirdValueElement.Attribute ("columnValue").Value, Is.EqualTo ("2"));
    }

    public void CollectPropertyType_CollectsEnumType ()
    {
      _enumSerializer.CollectPropertyType (GetPropertyDefinition ((ClassWithAllDataTypes _) => _.EnumProperty));

      Assert.That (_enumSerializer.EnumTypes, Contains.Item (typeof (ClassWithAllDataTypes.EnumType)));
    }

    [Test]
    public void CollectPropertyType_DoesNotCollectDuplicates ()
    {
      _enumSerializer.CollectPropertyType (GetPropertyDefinition ((ClassWithAllDataTypes _) => _.EnumProperty));
      _enumSerializer.CollectPropertyType (GetPropertyDefinition ((ClassWithAllDataTypes _) => _.EnumProperty));

      Assert.That (_enumSerializer.EnumTypes.Count, Is.EqualTo (1));
    }

    [Test]
    public void CollectPropertyType_DoesNotCollectNonEnumTypes ()
    {
      _enumSerializer.CollectPropertyType (GetPropertyDefinition ((ClassWithAllDataTypes _) => _.Int32Property));

      Assert.That(_enumSerializer.EnumTypes, Is.Empty);
    }
    
    [Test]
    public void CollectPropertyType_CollectsNullableEnumType ()
    {
      _enumSerializer.CollectPropertyType (GetPropertyDefinition ((ClassWithAllDataTypes _) => _.NaEnumProperty));

      Assert.That (_enumSerializer.EnumTypes, Contains.Item (typeof (ClassWithAllDataTypes.EnumType)));
    }
  }
}