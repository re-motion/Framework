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

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithOneSideRelationPropertiesNotInMapping : DomainObject
  {
    protected ClassWithOneSideRelationPropertiesNotInMapping ()
    {
    }

    [DBBidirectionalRelation ("BaseBidirectionalOneToOne")]
    public abstract ClassWithManySideRelationProperties BaseBidirectionalOneToOne { get; set; }

    [DBBidirectionalRelation ("BaseBidirectionalOneToMany")]
    public abstract ObjectList<ClassWithManySideRelationProperties> BaseBidirectionalOneToMany { get; }

    [DBBidirectionalRelation ("BasePrivateBidirectionalOneToOne")]
    private ClassWithManySideRelationProperties BasePrivateBidirectionalOneToOne
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    [DBBidirectionalRelation ("BasePrivateBidirectionalOneToMany")]
    private ObjectList<ClassWithManySideRelationProperties> BasePrivateBidirectionalOneToMany
    {
      get { throw new NotImplementedException (); }
    }
  }
}
