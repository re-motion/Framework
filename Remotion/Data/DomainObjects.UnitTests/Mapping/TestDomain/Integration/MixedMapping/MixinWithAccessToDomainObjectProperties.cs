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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping
{
  [CLSCompliant(false)]
  [Extends(typeof(ClassWithAllDataTypes), MixinTypeArguments = new Type[] { typeof(ClassWithAllDataTypes) })]
  public class MixinWithAccessToDomainObjectProperties<TDomainObject> : DomainObjectMixin<TDomainObject>
      where TDomainObject : DomainObject
  {
    public bool OnDomainObjectCreatedCalled;
    public bool OnDomainObjectLoadedCalled;
    public LoadMode OnDomainObjectLoadedLoadMode;
    public bool OnDomainObjectReferenceInitializingCalled;

    [StorageClassNone]
    public new ObjectID ID
    {
      get { return base.ID; }
    }

    public new Type GetPublicDomainObjectType ()
    {
      return base.GetPublicDomainObjectType();
    }

    [StorageClassNone]
    public new DomainObjectState State
    {
      get { return base.State; }
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    [StorageClassNone]
    public new TDomainObject Target
    {
      get { return base.Target; }
    }

    protected override void OnDomainObjectReferenceInitializing ()
    {
      OnDomainObjectReferenceInitializingCalled = true;
    }

    protected override void OnDomainObjectCreated ()
    {
      OnDomainObjectCreatedCalled = true;
    }

    protected override void OnDomainObjectLoaded (LoadMode loadMode)
    {
      OnDomainObjectLoadedCalled = true;
      OnDomainObjectLoadedLoadMode = loadMode;
    }
  }
}
