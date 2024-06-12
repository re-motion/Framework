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
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Instantiable]
  public abstract class AccessTypeDefinition : EnumValueDefinition
  {
    public static Expression<Func<AccessTypeDefinition, IEnumerable<AccessTypeReference>>> SelectAccessTypeReferences ()
    {
      return property => property.AccessTypeReferences;
    }

    public static AccessTypeDefinition NewObject ()
    {
      return NewObject<AccessTypeDefinition>();
    }

    public static AccessTypeDefinition NewObject (Guid metadataItemID, string name, int value)
    {
      return NewObject<AccessTypeDefinition>(ParamList.Create(metadataItemID, name, value));
    }

    protected AccessTypeDefinition ()
    {
    }

    protected AccessTypeDefinition (Guid metadataItemID, string name, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      MetadataItemID = metadataItemID;
      Name = name;
      Value = value;
    }

    [DBBidirectionalRelation("AccessType")]
    protected abstract ObjectList<AccessTypeReference> AccessTypeReferences { get; }

    protected override void OnDeleting (EventArgs args)
    {
      if (AccessTypeReferences.Any())
      {
        throw new InvalidOperationException(
            string.Format("Access type '{0}' cannot be deleted because it is associated with at least one securable class definition.", Name));
      }
      base.OnDeleting(args);
    }
  }
}
