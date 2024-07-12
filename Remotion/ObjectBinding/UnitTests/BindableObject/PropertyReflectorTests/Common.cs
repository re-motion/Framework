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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.PropertyReflectorTests
{
  [TestFixture]
  public class Common : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void Initialize ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithAllDataTypes), "String");

      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That(propertyReflector.PropertyInfo, Is.SameAs(propertyInfo));
      Assert.That(propertyReflector.BusinessObjectProvider, Is.SameAs(_businessObjectProvider));
    }

    [Test]
    public void Initialize_WithMixin ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithAllDataTypes), "String");

      using (MixinConfiguration.BuildNew().ForClass(typeof(PropertyReflector)).AddMixin<MixinStub>().EnterScope())
      {
        PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

        Assert.That(propertyReflector.PropertyInfo, Is.SameAs(propertyInfo));
        Assert.That(propertyReflector.BusinessObjectProvider, Is.SameAs(_businessObjectProvider));
        Assert.That(propertyReflector, Is.InstanceOf(typeof(IMixinTarget)));
      }
    }

    [Test]
    public void GetMetadata_WithBoolean ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Boolean");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(BooleanProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Boolean"));
    }

    [Test]
    public void GetMetadata_WithByte ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Byte");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(ByteProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Byte"));
    }

    [Test]
    public void GetMetadata_WithDate ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Date");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(DateProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Date"));
    }

#if NET6_0_OR_GREATER
    [Test]
    public void GetMetadata_WithDateOnly ()
    {
      var businessObjectProperty = GetMetadataFromPropertyReflector("DateOnly");

      Assert.That(businessObjectProperty, Is.TypeOf<DateOnlyProperty>());
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("DateOnly"));
    }
#endif

    [Test]
    public void GetMetadata_WithDateTime ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("DateTime");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(DateTimeProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("DateTime"));
    }

    [Test]
    public void GetMetadata_WithDecimal ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Decimal");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(DecimalProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Decimal"));
    }

    [Test]
    public void GetMetadata_WithDouble ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Double");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(DoubleProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Double"));
    }

    [Test]
    public void GetMetadata_WithEnum ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Enum");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(EnumerationProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Enum"));
    }

    [Test]
    public void GetMetadata_WithFlags ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Flags");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(NotSupportedProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Flags"));
    }

    [Test]
    public void GetMetadata_WithExtensibleEnum ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("ExtensibleEnum");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(ExtensibleEnumerationProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("ExtensibleEnum"));
    }

    [Test]
    public void GetMetadata_WithGuid ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Guid");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(GuidProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Guid"));
    }

    [Test]
    public void GetMetadata_WithEnumBase ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo(typeof(ClassWithReferenceType<Enum>), "Scalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(NotSupportedProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Scalar"));
    }

    [Test]
    public void GetMetadata_WithInt16 ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Int16");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(Int16Property)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Int16"));
    }

    [Test]
    public void GetMetadata_WithInt32 ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Int32");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(Int32Property)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Int32"));
    }

    [Test]
    public void GetMetadata_WithInt64 ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Int64");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(Int64Property)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Int64"));
    }

    [Test]
    public void GetMetadata_WithBusinessObject ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("BusinessObject");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(ReferenceProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("BusinessObject"));
    }

    [Test]
    public void GetMetadata_WithSingle ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("Single");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(SingleProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("Single"));
    }

    [Test]
    public void GetMetadata_WithString ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("String");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(StringProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("String"));
    }

    [Test]
    public void GetMetadata_WithIEnumerable ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector("IEnumerable");

      Assert.That(businessObjectProperty, Is.TypeOf(typeof(NotSupportedProperty)));
      Assert.That(businessObjectProperty.Identifier, Is.EqualTo("IEnumerable"));
    }

    [Test]
    [Ignore("TODO: test")]
    public void GetMetadata_WithRequiredStringAttribute ()
    {
    }

    [Test]
    [Ignore("TODO: test")]
    public void GetMetadata_WithMaxLengthStringAttribute ()
    {
    }

    private IBusinessObjectProperty GetMetadataFromPropertyReflector (string propertyName)
    {
      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithAllDataTypes), propertyName);
      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      return propertyReflector.GetMetadata();
    }
  }
}
