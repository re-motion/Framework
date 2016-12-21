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

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping
{
  [DBTable ("MixedDomains_RelationTarget")]
  [Instantiable]
  [TestDomain]
  public abstract class RelationTargetForPersistentMixin : DomainObject
  {
    [DBBidirectionalRelation ("RelationProperty")]
    public abstract TargetClassForPersistentMixin RelationProperty1 { get; set; }

    [DBBidirectionalRelation ("VirtualRelationProperty", ContainsForeignKey = true)]
    public abstract TargetClassForPersistentMixin RelationProperty2 { get; set; }

    [DBBidirectionalRelation ("CollectionProperty1Side")]
    public abstract TargetClassForPersistentMixin RelationProperty3 { get; set; }

    [DBBidirectionalRelation ("CollectionPropertyNSide")]
    public abstract ObjectList<TargetClassForPersistentMixin> RelationProperty4 { get; }

    [DBBidirectionalRelation ("PrivateBaseRelationProperty", ContainsForeignKey = false)]
    public abstract TargetClassForPersistentMixin RelationProperty5 { get; set; }
  }
}
