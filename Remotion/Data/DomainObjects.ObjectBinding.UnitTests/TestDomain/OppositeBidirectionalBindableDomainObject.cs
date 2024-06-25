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

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class OppositeBidirectionalBindableDomainObject : BindableDomainObject
  {
    public static OppositeBidirectionalBindableDomainObject NewObject ()
    {
      return NewObject<OppositeBidirectionalBindableDomainObject>();
    }

    [DBBidirectionalRelation("RequiredBidirectionalRelatedObjectProperty", ContainsForeignKey = true)]
    public abstract BindableDomainObjectWithProperties OppositeRequiredRelatedObject { get; set; }
    [DBBidirectionalRelation("NonRequiredBidirectionalRelatedObjectProperty", ContainsForeignKey = true)]
    public abstract BindableDomainObjectWithProperties OppositeNonRequiredRelatedObject { get; set; }

    [Mandatory]
    [DBBidirectionalRelation("RequiredBidirectionalRelatedObjectsPropertyForDomainObjectCollection")]
    public abstract BindableDomainObjectWithProperties OppositeRequiredRelatedObjectsForDomainObjectCollection { get; set; }
    [DBBidirectionalRelation("NonRequiredBidirectionalRelatedObjectsPropertyForDomainObjectCollection")]
    public abstract BindableDomainObjectWithProperties OppositeNonRequiredRelatedObjectsForDomainObjectCollection { get; set; }

    [Mandatory]
    [DBBidirectionalRelation("RequiredBidirectionalRelatedObjectsPropertyForVirtualCollection")]
    public abstract BindableDomainObjectWithProperties OppositeRequiredRelatedObjectsForVirtualCollection { get; set; }
    [DBBidirectionalRelation("NonRequiredBidirectionalRelatedObjectsPropertyForVirtualCollection")]
    public abstract BindableDomainObjectWithProperties OppositeNonRequiredRelatedObjectsForVirtualCollection { get; set; }

    [DBBidirectionalRelation("RelatedObjectProperty1", ContainsForeignKey = true)]
    public abstract SampleBindableDomainObject OppositeSampleObject { get; set; }
    [DBBidirectionalRelation("RelatedObjectProperty2")]
    public abstract ObjectList<SampleBindableDomainObject> OppositeSampleObjects { get; set; }

    [DBBidirectionalRelation("List")]
    public abstract SampleBindableMixinDomainObject OppositeSampleObjectWithMixin1 { get; set;  }
    [DBBidirectionalRelation("Relation")]
    public abstract SampleBindableMixinDomainObject OppositeSampleObjectWithMixin2 { get; set; }
  }
}
