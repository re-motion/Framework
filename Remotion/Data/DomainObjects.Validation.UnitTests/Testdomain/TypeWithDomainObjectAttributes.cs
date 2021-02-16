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
  [DBTable]
  public class TypeWithDomainObjectAttributes : DomainObject
  {
    public virtual string PropertyWithoutAttribute { get; set; }

    [Mandatory]
    public virtual TestDomainObject PropertyWithMandatoryAttribute { get; set; }

    [StringProperty (IsNullable = true, MaximumLength = 10)]
    public virtual string PropertyWithNullableStringPropertyAttribute { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 20)]
    public virtual string PropertyWithMandatoryStringPropertyAttribute { get; set; }

    [BinaryProperty (IsNullable = false)]
    public virtual byte[] BinaryProperty { get; set; }

    [StorageClassNone]
    [Mandatory]
    public virtual ObjectList<TestDomainObject> DomainObjectCollectionProperty { get; set; }
 
    [StorageClassNone]
    [Mandatory]
    public virtual IObjectList<TestDomainObject> VirtualCollectionProperty { get; set; }
  }
}