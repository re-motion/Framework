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
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.ObjectBinding.IntegrationTests.TestDomain.Search
{
  [Instantiable]
  [DBTable]
  public abstract class SampleBindableDomainObject : BindableDomainObject, ISupportsGetObject
  {
    public static SampleBindableDomainObject NewObject ()
    {
      return NewObject<SampleBindableDomainObject>();
    }

    public static SampleBindableDomainObject NewObject (IBindableDomainObjectImplementation implementation)
    {
      return NewObject<SampleBindableDomainObject>(ParamList.Create(implementation));
    }

    protected SampleBindableDomainObject ()
    {
    }

    protected SampleBindableDomainObject (IBindableDomainObjectImplementation implementation)
    {
      PrivateInvoke.SetNonPublicField(this, "_implementation", implementation);
    }

    public abstract string Name { get; set; }
    public abstract int Int32 { get; set; }

    [StorageClassTransaction]
    [DBBidirectionalRelation("OppositeSampleObject")]
    public abstract OppositeBidirectionalBindableDomainObject RelatedObjectProperty1 { get; set; }
    [StorageClassTransaction]
    [DBBidirectionalRelation("OppositeSampleObjects")]
    public abstract OppositeBidirectionalBindableDomainObject RelatedObjectProperty2 { get; set; }

    public new void Delete ()
    {
      base.Delete();
    }
  }
}
