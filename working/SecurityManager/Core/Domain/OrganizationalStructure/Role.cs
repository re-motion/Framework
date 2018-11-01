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
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Role")]
  [PermanentGuid ("23C68C62-5B0F-4857-8DF2-C161C0077745")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Role : OrganizationalStructureObject
  {
    private DomainObjectDeleteHandler _deleteHandler;

    public static Role NewObject ()
    {
      return NewObject<Role>();
    }

    protected Role ()
    {
    }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    [SearchAvailableObjectsServiceType (typeof (GroupPropertyTypeSearchService))]
    public abstract Group Group { get; set; }

    [Mandatory]
    [SearchAvailableObjectsServiceType (typeof (RolePropertiesSearchService))]
    public abstract Position Position { get; set; }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    [SearchAvailableObjectsServiceType (typeof (UserPropertyTypeSearchService))]
    public abstract User User { get; set; }

    [DBBidirectionalRelation ("SubstitutedRole")]
    public abstract ObjectList<Substitution> SubstitutedBy { get; }
    
    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _deleteHandler = new DomainObjectDeleteHandler (SubstitutedBy);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      _deleteHandler.Delete ();
    }

    protected override void OnCommitting (DomainObjectCommittingEventArgs args)
    {
      base.OnCommitting (args);

      var userProperty = Properties[typeof (Role), "User"];
      if (userProperty.GetValue<User> () != null)
        userProperty.GetValue<User> ().RegisterForCommit();
      else if (userProperty.GetOriginalValue<User>() != null)
        userProperty.GetOriginalValue<User>().RegisterForCommit();

      foreach (var substitution in SubstitutedBy)
        substitution.RegisterForCommit();
    }

    protected override string GetOwningTenant ()
    {
      if (User != null)
        return User.Tenant.UniqueIdentifier;

      if (Group != null)
        return Group.Tenant.UniqueIdentifier;

      return null;
    }

    public override string DisplayName
    {
      get
      {
        string positionName = Position != null ? Position.DisplayName : null;
        string groupName = Group != null ? Group.DisplayName : null;

        return string.Format ("{0} / {1}", positionName ?? "?", groupName ?? "?" );
      }
    }
  }
}
