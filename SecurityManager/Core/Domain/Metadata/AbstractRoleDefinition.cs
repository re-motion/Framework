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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  public abstract class AbstractRoleDefinition : EnumValueDefinition, ISupportsGetObject
  {
    public static AbstractRoleDefinition NewObject ()
    {
      return NewObject<AbstractRoleDefinition>();
    }

    public static AbstractRoleDefinition NewObject (Guid metadataItemID, string name, int value)
    {
      return NewObject<AbstractRoleDefinition>(ParamList.Create(metadataItemID, name, value));
    }

    public static ObjectList<AbstractRoleDefinition> Find (IEnumerable<EnumWrapper> abstractRoles)
    {
      ArgumentUtility.CheckNotNull("abstractRoles", abstractRoles);

      var abstractRoleNames = (from abstractRole in abstractRoles select abstractRole.Name).ToArray();

      var result = from ar in QueryFactory.CreateLinqQuery<AbstractRoleDefinition>()
                   where abstractRoleNames.Contains(ar.Name)
                   orderby ar.Index
                   select ar;

      return result.ToObjectList();
    }

    public static ObjectList<AbstractRoleDefinition> FindAll ()
    {
      var result = from ar in QueryFactory.CreateLinqQuery<AbstractRoleDefinition>()
                   orderby ar.Index
                   select ar;

      return result.ToObjectList();
    }

    protected AbstractRoleDefinition ()
    {
    }

    protected AbstractRoleDefinition (Guid metadataItemID, string name, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      MetadataItemID = metadataItemID;
      Name = name;
      Value = value;
    }
  }
}
