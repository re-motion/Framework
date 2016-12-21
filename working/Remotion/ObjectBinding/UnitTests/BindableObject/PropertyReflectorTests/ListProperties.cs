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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.PropertyReflectorTests
{
  [TestFixture]
  public class ListProperties : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private BindableObjectClass _bindableObjectClass;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
      _bindableObjectClass = new BindableObjectClass (
          typeof (ClassWithReferenceType<SimpleReferenceType>),
          _businessObjectProvider,
          SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(),
          new PropertyBase[0]);
    }

    [Test]
    public void GetMetadata_WithArray ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "Array");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Array"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyArray ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ReadOnlyArray");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyArray"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithListOfT ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ListOfT");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ListOfT"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (List<SimpleReferenceType>)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyListOfT ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ReadOnlyListOfT");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyListOfT"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (List<SimpleReferenceType>)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyCollectionOfT ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ReadOnlyCollectionOfT");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyCollectionOfT"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ReadOnlyCollection<SimpleReferenceType>)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyCollectionOfTWithSetter ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ReadOnlyCollectionOfTWithSetter");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyCollectionOfTWithSetter"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ReadOnlyCollection<SimpleReferenceType>)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithArrayList ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ArrayList");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ArrayList"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ArrayList)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyArrayList ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ReadOnlyArrayList");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyArrayList"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ArrayList)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetMetadata_WithMissingItemTypeAttributeOnIList ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetMetadata_WithItemTypeAttributeOnOverride ()
    {
      // expect exception
    }
  }
}
