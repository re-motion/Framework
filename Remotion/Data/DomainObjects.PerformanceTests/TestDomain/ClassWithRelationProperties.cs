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
  [Serializable]
  public abstract class ClassWithRelationProperties : SimpleDomainObject<ClassWithRelationProperties>
  {
    public abstract OppositeClassWithAnonymousRelationProperties Unary1 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary2 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary3 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary4 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary5 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary6 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary7 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary8 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary9 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary10 { get; set; }

    [DBBidirectionalRelation("Virtual1", ContainsForeignKey = true)]
    public abstract OppositeClassWithVirtualRelationProperties Real1 { get; set; }
    [DBBidirectionalRelation("Virtual2", ContainsForeignKey = true)]
    public abstract OppositeClassWithVirtualRelationProperties Real2 { get; set; }
    [DBBidirectionalRelation("Virtual3", ContainsForeignKey = true)]
    public abstract OppositeClassWithVirtualRelationProperties Real3 { get; set; }
    [DBBidirectionalRelation("Virtual4", ContainsForeignKey = true)]
    public abstract OppositeClassWithVirtualRelationProperties Real4 { get; set; }
    [DBBidirectionalRelation("Virtual5", ContainsForeignKey = true)]
    public abstract OppositeClassWithVirtualRelationProperties Real5 { get; set; }
    [DBBidirectionalRelation("Virtual6", ContainsForeignKey = true)]
    public abstract OppositeClassWithVirtualRelationProperties Real6 { get; set; }
    [DBBidirectionalRelation("Virtual7", ContainsForeignKey = true)]
    public abstract OppositeClassWithVirtualRelationProperties Real7 { get; set; }
    [DBBidirectionalRelation("Virtual8", ContainsForeignKey = true)]
    public abstract OppositeClassWithVirtualRelationProperties Real8 { get; set; }
    [DBBidirectionalRelation("Virtual9", ContainsForeignKey = true)]
    public abstract OppositeClassWithVirtualRelationProperties Real9 { get; set; }
    [DBBidirectionalRelation("Virtual10", ContainsForeignKey = true)]
    public abstract OppositeClassWithVirtualRelationProperties Real10 { get; set; }

    [DBBidirectionalRelation("Real1")]
    public abstract OppositeClassWithRealRelationProperties Virtual1 { get; set; }
    [DBBidirectionalRelation("Real2")]
    public abstract OppositeClassWithRealRelationProperties Virtual2 { get; set; }
    [DBBidirectionalRelation("Real3")]
    public abstract OppositeClassWithRealRelationProperties Virtual3 { get; set; }
    [DBBidirectionalRelation("Real4")]
    public abstract OppositeClassWithRealRelationProperties Virtual4 { get; set; }
    [DBBidirectionalRelation("Real5")]
    public abstract OppositeClassWithRealRelationProperties Virtual5 { get; set; }
    [DBBidirectionalRelation("Real6")]
    public abstract OppositeClassWithRealRelationProperties Virtual6 { get; set; }
    [DBBidirectionalRelation("Real7")]
    public abstract OppositeClassWithRealRelationProperties Virtual7 { get; set; }
    [DBBidirectionalRelation("Real8")]
    public abstract OppositeClassWithRealRelationProperties Virtual8 { get; set; }
    [DBBidirectionalRelation("Real9")]
    public abstract OppositeClassWithRealRelationProperties Virtual9 { get; set; }
    [DBBidirectionalRelation("Real10")]
    public abstract OppositeClassWithRealRelationProperties Virtual10 { get; set; }

    [DBBidirectionalRelation("EndOfCollection")]
    public abstract ObjectList<OppositeClassWithCollectionRelationProperties> Collection { get; }
  }
}
