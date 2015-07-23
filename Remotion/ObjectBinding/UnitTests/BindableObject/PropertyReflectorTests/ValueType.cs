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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.PropertyReflectorTests
{
  [TestFixture]
  public class ValueType : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private BindableObjectClass _bindableObjectClass;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();
      _bindableObjectClass = new BindableObjectClass (
          typeof (ClassWithReferenceType<SimpleReferenceType>),
          _businessObjectProvider,
          SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(),
          new PropertyBase[0]);
    }

    [Test]
    public void GetMetadata_WithScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "Scalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleValueType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Scalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.True);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithNullableScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "NullableScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleValueType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("NullableScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType?)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithUndefinedEnum ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithUndefinedEnumValue), "Scalar");
      PropertyReflector propertyReflector = PropertyReflector.Create (IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (EnumWithUndefinedValue)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (EnumerationProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Scalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (EnumWithUndefinedValue)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "ReadOnlyScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleValueType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.True);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyNonPublicSetterScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "ReadOnlyNonPublicSetterScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleValueType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyNonPublicSetterScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.True);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyAttributeScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "ReadOnlyAttributeScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleValueType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyAttributeScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.True);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithArray ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "Array");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleValueType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Array"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleValueType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithNullableArray ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "NullableArray");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleValueType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("NullableArray"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType?[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleValueType?)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }
  }
}
