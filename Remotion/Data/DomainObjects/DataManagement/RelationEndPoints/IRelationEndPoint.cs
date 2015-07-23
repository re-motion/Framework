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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Provides a common interface for objects representing one side of a relation between <see cref="DomainObject"/> instances.
  /// </summary>
  public interface IRelationEndPoint : INullObject, IFlattenedSerializable
  {
    RelationEndPointID ID { get; }
    ClientTransaction ClientTransaction { get; }

    ObjectID ObjectID { get; }
    IRelationEndPointDefinition Definition { get; }
    RelationDefinition RelationDefinition { get; }

    bool HasChanged { get; }
    bool HasBeenTouched { get; }

    DomainObject GetDomainObject ();
    DomainObject GetDomainObjectReference ();

    bool IsDataComplete { get; }
    void EnsureDataComplete ();

    bool? IsSynchronized { get; }
    void Synchronize ();

    void Touch ();
    void Commit ();
    void Rollback ();
    IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject);
    IDataManagementCommand CreateDeleteCommand ();
    void ValidateMandatory ();

    IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ();
    void SetDataFromSubTransaction (IRelationEndPoint source);
  }
}