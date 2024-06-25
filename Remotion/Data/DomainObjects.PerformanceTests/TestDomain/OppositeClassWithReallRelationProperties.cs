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
using Remotion.Data.DomainObjects.ObjectBinding;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class OppositeClassWithRealRelationProperties : SimpleDomainObject<OppositeClassWithRealRelationProperties>
  {
    [DBBidirectionalRelation("Virtual1", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real1 { get; set; }
    [DBBidirectionalRelation("Virtual2", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real2 { get; set; }
    [DBBidirectionalRelation("Virtual3", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real3 { get; set; }
    [DBBidirectionalRelation("Virtual4", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real4 { get; set; }
    [DBBidirectionalRelation("Virtual5", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real5 { get; set; }
    [DBBidirectionalRelation("Virtual6", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real6 { get; set; }
    [DBBidirectionalRelation("Virtual7", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real7 { get; set; }
    [DBBidirectionalRelation("Virtual8", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real8 { get; set; }
    [DBBidirectionalRelation("Virtual9", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real9 { get; set; }
    [DBBidirectionalRelation("Virtual10", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real10 { get; set; }
  }
}
