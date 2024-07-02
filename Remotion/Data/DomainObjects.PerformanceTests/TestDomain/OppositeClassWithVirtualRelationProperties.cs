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
  public abstract class OppositeClassWithVirtualRelationProperties : SimpleDomainObject<OppositeClassWithVirtualRelationProperties>
  {
    [DBBidirectionalRelation("Real1")]
    public abstract ClassWithRelationProperties Virtual1 { get; set; }
    [DBBidirectionalRelation("Real2")]
    public abstract ClassWithRelationProperties Virtual2 { get; set; }
    [DBBidirectionalRelation("Real3")]
    public abstract ClassWithRelationProperties Virtual3 { get; set; }
    [DBBidirectionalRelation("Real4")]
    public abstract ClassWithRelationProperties Virtual4 { get; set; }
    [DBBidirectionalRelation("Real5")]
    public abstract ClassWithRelationProperties Virtual5 { get; set; }
    [DBBidirectionalRelation("Real6")]
    public abstract ClassWithRelationProperties Virtual6 { get; set; }
    [DBBidirectionalRelation("Real7")]
    public abstract ClassWithRelationProperties Virtual7 { get; set; }
    [DBBidirectionalRelation("Real8")]
    public abstract ClassWithRelationProperties Virtual8 { get; set; }
    [DBBidirectionalRelation("Real9")]
    public abstract ClassWithRelationProperties Virtual9 { get; set; }
    [DBBidirectionalRelation("Real10")]
    public abstract ClassWithRelationProperties Virtual10 { get; set; }
  }
}
