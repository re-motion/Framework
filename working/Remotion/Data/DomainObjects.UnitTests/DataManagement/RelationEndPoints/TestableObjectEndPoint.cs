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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  public class TestableObjectEndPoint : ObjectEndPoint
  {
    public TestableObjectEndPoint (ClientTransaction clientTransaction, RelationEndPointID id)
        : base (clientTransaction, id)
    {
    }

    public TestableObjectEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
    }

    public override bool HasChanged
    {
      get { throw new NotImplementedException (); }
    }

    public override bool HasBeenTouched
    {
      get { throw new NotImplementedException (); }
    }

    public override bool IsDataComplete
    {
      get { throw new NotImplementedException(); }
    }

    public override bool? IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }

    public override void EnsureDataComplete ()
    {
      throw new NotImplementedException ();
    }

    public override void Synchronize ()
    {
      throw new NotImplementedException();
    }

    public override void Touch ()
    {
      throw new NotImplementedException();
    }

    public override void Commit ()
    {
      throw new NotImplementedException();
    }

    public override void Rollback ()
    {
      throw new NotImplementedException();
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      throw new NotImplementedException();
    }

    public override ObjectID OppositeObjectID
    {
      get { throw new NotImplementedException (); }
    }

    public override ObjectID OriginalOppositeObjectID
    {
      get { throw new NotImplementedException (); }
    }

    public override DomainObject GetOppositeObject ()
    {
      throw new NotImplementedException ();
    }

    public override DomainObject GetOriginalOppositeObject ()
    {
      throw new NotImplementedException ();
    }

    public override IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      throw new NotImplementedException ();
    }

    public void CallSetOppositeObjectDataFromSubTransaction (IObjectEndPoint sourceObjectEndPoint)
    {
      SetOppositeObjectDataFromSubTransaction (sourceObjectEndPoint);
    }

    protected override void SetOppositeObjectDataFromSubTransaction (IObjectEndPoint sourceObjectEndPoint)
    {
      throw new NotImplementedException ();
    }
  }
}