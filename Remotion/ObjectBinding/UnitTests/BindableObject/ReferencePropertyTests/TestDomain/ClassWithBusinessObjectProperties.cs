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
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain
{
  [BindableObject]
  public class ClassWithBusinessObjectProperties
  {
    public ClassWithBusinessObjectProperties ()
    {
    }

    public ClassWithIdentityAndSearchServiceTypeAttribute SearchServiceFromPropertyType { get; set; }

    [SearchAvailableObjectsServiceType(typeof(ISearchServiceOnProperty))]
    public ClassWithIdentityAndSearchServiceTypeAttribute SearchServiceFromPropertyDeclaration { get; set; }

    public ClassFromOtherBusinessObjectImplementation NoSearchService { get; set; }


    public ClassWithDefaultValueServiceTypeAttribute DefaultValueServiceFromPropertyType { get; set; }

#pragma warning disable 612,618
    [DefaultValueServiceType(typeof(IDefaultValueServiceOnProperty))]
    public ClassWithDefaultValueServiceTypeAttribute DefaultValueServiceFromPropertyDeclaration { get; set; }
#pragma warning restore 612,618

    public ClassFromOtherBusinessObjectImplementation NoDefaultValueService { get; set; }


    public ClassWithDeleteObjectServiceTypeAttribute DeleteObjectServiceFromPropertyType { get; set; }

#pragma warning disable 612,618
    [DeleteObjectServiceType(typeof(IDeleteObjectServiceOnProperty))]
    public ClassWithDeleteObjectServiceTypeAttribute DeleteObjectServiceFromPropertyDeclaration { get; set; }
#pragma warning restore 612,618

    public ClassFromOtherBusinessObjectImplementation NoDeleteObjectService { get; set; }
  }
}
