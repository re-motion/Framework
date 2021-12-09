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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Synchronization
{
  public static class RelationInconcsistenciesTestHelper
  {
    public static ObjectID CreateObjectAndSetRelationInOtherTransaction (RelationEndPointDefinition endPointDefinition, ObjectID relatedID)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject = LifetimeService.NewObject(ClientTransaction.Current, endPointDefinition.TypeDefinition.Type, ParamList.Empty);
        SetForeignKeyProperty(domainObject, endPointDefinition, relatedID);
        ClientTransaction.Current.Commit();

        return domainObject.ID;
      }
    }

    public static ObjectID SetRelationInOtherTransaction (RelationEndPointID endPointID, ObjectID relatedID)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject = LifetimeService.GetObject(ClientTransaction.Current, endPointID.ObjectID, true);
        SetForeignKeyProperty(domainObject, (RelationEndPointDefinition)endPointID.Definition, relatedID);
        ClientTransaction.Current.Commit();

        return domainObject.ID;
      }
    }

    public static void SetForeignKeyProperty (IDomainObject domainObject, RelationEndPointDefinition endPointDefinition, ObjectID relatedID)
    {
      var relatedObject = LifetimeService.GetObjectReference(ClientTransaction.Current, relatedID);
      var properties = new PropertyIndexer(domainObject);
      properties[endPointDefinition.PropertyName].SetValue(relatedObject);
    }

    public static ObjectID CreateAndInitializeObjectAndSetRelationInOtherTransaction<TCreated, TRelated> (ObjectID relatedID, Action<TCreated, TRelated> setter)
        where TCreated : DomainObject
        where TRelated : DomainObject
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject = (TCreated)LifetimeService.NewObject(ClientTransaction.Current, typeof(TCreated), ParamList.Empty);
        setter(domainObject, relatedID != null ? (TRelated)LifetimeService.GetObject(ClientTransaction.Current, relatedID, true) : null);
        ClientTransaction.Current.Commit();

        return domainObject.ID;
      }
    }

    public static ObjectID SetRelationInOtherTransaction<TOriginating, TRelated> (ObjectID originatingID, ObjectID relatedID, Action<TOriginating, TRelated> setter)
        where TOriginating : DomainObject
        where TRelated : DomainObject
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject = (TOriginating)LifetimeService.GetObject(ClientTransaction.Current, originatingID, true);
        setter(domainObject, relatedID != null ? (TRelated)LifetimeService.GetObject(ClientTransaction.Current, relatedID, true) : null);
        ClientTransaction.Current.Commit();

        return domainObject.ID;
      }
    }
  }
}
