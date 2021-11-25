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

namespace Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain
{
  public interface IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfDifferentInterfaces1
  {
    TestDomainObject PropertyWithMandatoryAttribute { get; set; }
  }

  public interface IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfDifferentInterfaces2
  {
    string PropertyWithMandatoryStringPropertyAttribute { get; set; }
  }

  public class MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfDifferentInterfaces
      : DomainObjectMixin<MixinTarget_AnnotatedPropertiesPartOfDifferentInterfaces>,
          IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfDifferentInterfaces1,
          IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfDifferentInterfaces2
  {
    private static readonly Type s_type = typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfDifferentInterfaces);

    [Mandatory]
    public virtual TestDomainObject PropertyWithMandatoryAttribute
    {
      get { return Properties[s_type, "PropertyWithMandatoryAttribute"].GetValue<TestDomainObject>(); }
      set { Properties[s_type, "PropertyWithMandatoryAttribute"].SetValue(value); }
    }

    [StringProperty (IsNullable = false, MaximumLength = 20)]
    public virtual string PropertyWithMandatoryStringPropertyAttribute
    {
      get { return Properties[s_type, "PropertyWithMandatoryStringPropertyAttribute"].GetValue<string>(); }
      set { Properties[s_type, "PropertyWithMandatoryStringPropertyAttribute"].SetValue(value); }
    }
  }
}