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

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Person : TestDomainBase
  {
    public static Person NewObject ()
    {
      return NewObject<Person>();
    }

    protected Person ()
    {
    }

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation("ContactPerson", ContainsForeignKey = false)]
    public abstract Partner AssociatedPartnerCompany { get; set; }

    [DBBidirectionalRelation("ContactPerson", ContainsForeignKey = true)]
    public abstract Customer AssociatedCustomerCompany { get; set; }

    [DBBidirectionalRelation("Reviewer")]
    public abstract IObjectList<ProductReview> Reviews { get; set; }
  }
}
