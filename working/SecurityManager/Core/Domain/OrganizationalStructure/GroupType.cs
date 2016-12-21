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
using System.ComponentModel;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Globalization;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.GroupType")]
  [PermanentGuid ("BDBB9696-177B-4b73-98CF-321B2FBEAD0C")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class GroupType : OrganizationalStructureObject, ISupportsGetObject
  {
    private DomainObjectDeleteHandler _deleteHandler;

    public enum Methods
    {
      Search
    }

    public static GroupType NewObject ()
    {
      return NewObject<GroupType> ();
    }

    public static IQueryable<GroupType> FindAll ()
    {
      return from g in QueryFactory.CreateLinqQuery<GroupType>()
             orderby g.Name
             select g;
    }

    [DemandPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    protected GroupType ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("GroupType")]
    public abstract ObjectList<GroupTypePosition> Positions { get; }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);
     
      using (DefaultTransactionContext.ClientTransaction.EnterNonDiscardingScope())
      {
        if (QueryFactory.CreateLinqQuery<Group>().Where (g => g.GroupType == this).Any())
        {
          throw new InvalidOperationException (
              string.Format (
                  "The GroupType '{0}' is still assigned to at least one group. Please update or delete the dependent groups before proceeding.", Name));
        }

        var aces = QueryFactory.CreateLinqQuery<AccessControlEntry> ().Where (ace => ace.SpecificGroupType == this);
        
        _deleteHandler = new DomainObjectDeleteHandler (aces, Positions);
      }
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      _deleteHandler.Delete();
    }

    public override string DisplayName
    {
      get { return Name; }
    }
  }
}
