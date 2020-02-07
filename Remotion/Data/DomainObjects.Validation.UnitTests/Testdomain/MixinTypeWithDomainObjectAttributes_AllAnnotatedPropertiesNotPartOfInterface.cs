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
  public interface IMixinTypeWithDomainObjectAttributes_AllAnnotatedPropertiesNotPartOfInterface
  {
    string PropertyWithoutAttribute { get; set; }
  }

  public class MixinTypeWithDomainObjectAttributes_AllAnnotatedPropertiesNotPartOfInterface
      : DomainObjectMixin<MixinTarget_AllAnnotatedPropertiesNotPartOfInterface>,
          IMixinTypeWithDomainObjectAttributes_AllAnnotatedPropertiesNotPartOfInterface
  {
    public string PropertyWithoutAttribute { get; set; }

    [Mandatory]
    public TestDomainObject PropertyWithMandatoryAttribute { get; set; }

    [StringProperty (IsNullable = true, MaximumLength = 10)]
    public string PropertyWithNullableStringPropertyAttribute { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 20)]
    public string PropertyWithMandatoryStringPropertyAttribute { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 20)]
    public string PropertyWithMandatoryStringPropertyAttributePartOfInterface { get; set; }
  }
}