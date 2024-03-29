﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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

namespace Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain
{
  public interface IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface
  {
    string PropertyWithoutAttribute { get; set; }

    TestDomainObject PropertyWithMandatoryAttribute { get; set; }

    TestDomainObject BidirectionalPropertyWithMandatoryAttribute { get; set; }

    ObjectList<TestDomainObject> BidirectionalDomainObjectCollectionPropertyWithMandatoryAttribute { get; set; }

    IObjectList<TestDomainObject> BidirectionalVirtualCollectionPropertyWithMandatoryAttribute { get; set; }

    string PropertyWithNullableStringPropertyAttribute { get; set; }

    string PropertyWithMandatoryStringPropertyAttribute { get; set; }

    int IntProperty { get; set; }
  }

  public class MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface
      : DomainObjectMixin<MixinTarget_AnnotatedPropertiesPartOfInterface>, IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface
  {
    private static readonly Type s_type = typeof(MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface);

    public virtual string PropertyWithoutAttribute
    {
      get { return Properties[s_type, "PropertyWithoutAttribute"].GetValue<string>(); }
      set { Properties[s_type, "PropertyWithoutAttribute"].SetValue(value); }
    }

    [Mandatory]
    public virtual TestDomainObject PropertyWithMandatoryAttribute
    {
      get { return Properties[s_type, "PropertyWithMandatoryAttribute"].GetValue<TestDomainObject>(); }
      set { Properties[s_type, "PropertyWithMandatoryAttribute"].SetValue(value); }
    }

    [DBBidirectionalRelation("OppositeRequiredObject")]
    [Mandatory]
    public virtual TestDomainObject BidirectionalPropertyWithMandatoryAttribute
    {
      get { return Properties[s_type, "BidirectionalPropertyWithMandatoryAttribute"].GetValue<TestDomainObject>(); }
      set { Properties[s_type, "BidirectionalPropertyWithMandatoryAttribute"].SetValue(value); }
    }

    [Mandatory]
    [DBBidirectionalRelation("OppositeObjectForDomainObjectCollectionProperty")]
    public virtual ObjectList<TestDomainObject> BidirectionalDomainObjectCollectionPropertyWithMandatoryAttribute
    {
      get { return Properties[s_type, "BidirectionalDomainObjectCollectionPropertyWithMandatoryAttribute"].GetValue<ObjectList<TestDomainObject>>(); }
      set { Properties[s_type, "BidirectionalDomainObjectCollectionPropertyWithMandatoryAttribute"].SetValue(value); }
    }

    [Mandatory]
    [DBBidirectionalRelation("OppositeObjectForVirtualCollectionProperty")]
    public virtual IObjectList<TestDomainObject> BidirectionalVirtualCollectionPropertyWithMandatoryAttribute
    {
      get { return Properties[s_type, "BidirectionalVirtualCollectionPropertyWithMandatoryAttribute"].GetValue<IObjectList<TestDomainObject>>(); }
      set { Properties[s_type, "BidirectionalVirtualCollectionPropertyWithMandatoryAttribute"].SetValue(value); }
    }

    [StringProperty(IsNullable = true, MaximumLength = 10)]
    public virtual string PropertyWithNullableStringPropertyAttribute
    {
      get { return Properties[s_type, "PropertyWithNullableStringPropertyAttribute"].GetValue<string>(); }
      set { Properties[s_type, "PropertyWithNullableStringPropertyAttribute"].SetValue(value); }
    }

    [StringProperty(IsNullable = false, MaximumLength = 20)]
    public virtual string PropertyWithMandatoryStringPropertyAttribute
    {
      get { return Properties[s_type, "PropertyWithMandatoryStringPropertyAttribute"].GetValue<string>(); }
      set { Properties[s_type, "PropertyWithMandatoryStringPropertyAttribute"].SetValue(value); }
    }

    public virtual int IntProperty
    {
      get { return Properties[s_type, "IntProperty"].GetValue<int>(); }
      set { Properties[s_type, "IntProperty"].SetValue(value); }
    }
  }
}
