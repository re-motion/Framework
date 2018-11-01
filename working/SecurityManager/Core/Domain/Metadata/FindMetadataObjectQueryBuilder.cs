// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  public class FindMetadataObjectQueryBuilder
  {
    private struct MetadataID
    {
      public static MetadataID Parse (string metadataID)
      {
        try
        {
          if (metadataID.Contains ("|"))
          {
            string[] metadataIDParts = metadataID.Split (new char[] { '|' }, 2);
            Guid metadataItemID = new Guid (metadataIDParts[0]);
            int stateValue = int.Parse (metadataIDParts[1]);

            return new MetadataID (metadataItemID, stateValue);
          }

          return new MetadataID (new Guid (metadataID), null);
        }
        catch (FormatException exception)
        {
          throw new ArgumentException (string.Format ("The metadata ID '{0}' is invalid.", metadataID), "metadataID", exception);
        }
      }

      public readonly Guid MetadataItemID;
      public readonly int? StateValue;

      public MetadataID (Guid metadataItemID, int? stateValue)
      {
        MetadataItemID = metadataItemID;
        StateValue = stateValue;
      }
    }

    public IQueryable<MetadataObject> CreateQuery (string metadataReference)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("metadataReference", metadataReference);

      MetadataID metadataID = MetadataID.Parse (metadataReference);

      if (metadataID.StateValue.HasValue)
      {
        return (from state in QueryFactory.CreateLinqQuery<StateDefinition>()
               where state.StateProperty.MetadataItemID == metadataID.MetadataItemID && state.Value == metadataID.StateValue
               select state).Cast<MetadataObject>();
      }
      else
      {
        return from m in QueryFactory.CreateLinqQuery<MetadataObject>()
               where m.MetadataItemID == metadataID.MetadataItemID
               select m;
      }
    }
  }
}
