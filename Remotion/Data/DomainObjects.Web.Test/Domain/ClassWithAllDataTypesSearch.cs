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
using Remotion.Data.DomainObjects.Queries;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  [BindableObject]
  public class ClassWithAllDataTypesSearch
  {
    public ClassWithAllDataTypesSearch ()
    {
      EnumProperty = ClassWithAllDataTypes.EnumType.Value1;
    }

    public string StringProperty { get; set; }

    public byte? BytePropertyFrom { get; set; }

    public byte? BytePropertyTo { get; set; }

    public ClassWithAllDataTypes.EnumType EnumProperty { get; set; }

    public Color ExtensibleEnumProperty { get; set; }

    [DateProperty]
    public DateTime? DatePropertyFrom { get; set; }

    [DateProperty]
    public DateTime? DatePropertyTo { get; set; }

    public DateTime? DateTimePropertyFrom { get; set; }

    public DateTime? DateTimePropertyTo { get; set; }

    public IQuery CreateQuery ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration("QueryWithAllDataTypes");

      query.Parameters.Add("@stringProperty", StringProperty);
      query.Parameters.Add("@bytePropertyFrom", BytePropertyFrom);
      query.Parameters.Add("@bytePropertyTo", BytePropertyTo);
      query.Parameters.Add("@enumProperty", EnumProperty);
      query.Parameters.Add("@extensibleEnumProperty", ExtensibleEnumProperty);
      query.Parameters.Add("@datePropertyFrom", DatePropertyFrom);
      query.Parameters.Add("@datePropertyTo", DatePropertyTo);
      query.Parameters.Add("@dateTimePropertyFrom", DateTimePropertyFrom);
      query.Parameters.Add("@dateTimePropertyTo", DateTimePropertyTo);

      return query;
    }
  }
}
