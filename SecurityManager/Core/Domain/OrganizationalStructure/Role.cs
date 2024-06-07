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
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Role")]
  [PermanentGuid("23C68C62-5B0F-4857-8DF2-C161C0077745")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Role : OrganizationalStructureObject
  {
    private DomainObjectDeleteHandler? _deleteHandler;

    public static Role NewObject ()
    {
      return NewObject<Role>();
    }

    protected Role ()
    {
    }

    [DBBidirectionalRelation("Roles")]
    [Mandatory]
    [SearchAvailableObjectsServiceType(typeof(GroupPropertyTypeSearchService))]
    public abstract Group? Group { get; set; }

    [Mandatory]
    [SearchAvailableObjectsServiceType(typeof(RolePropertiesSearchService))]
    public abstract Position? Position { get; set; }

    [DBBidirectionalRelation("Roles")]
    [Mandatory]
    [SearchAvailableObjectsServiceType(typeof(UserPropertyTypeSearchService))]
    public abstract User? User { get; set; }

    [DBBidirectionalRelation("SubstitutedRole")]
    public abstract ObjectList<Substitution> SubstitutedBy { get; }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting(args);

      _deleteHandler = new DomainObjectDeleteHandler(SubstitutedBy);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted(args);

      _deleteHandler?.Delete();
    }

    protected override void OnCommitting (DomainObjectCommittingEventArgs args)
    {
      base.OnCommitting(args);

      var userProperty = Properties[typeof(Role), "User"];
      var currentUser = userProperty.GetValue<User>();
      var originalUser = userProperty.GetOriginalValue<User>();
      if (currentUser != null)
        currentUser.RegisterForCommit();
      else if (originalUser != null)
        originalUser.RegisterForCommit();

      foreach (var substitution in SubstitutedBy)
        substitution.RegisterForCommit();
    }

    protected override string? GetOwningTenant ()
    {
      if (User != null)
        return User.Tenant?.UniqueIdentifier;

      if (Group != null)
        return Group.Tenant?.UniqueIdentifier;

      return null;
    }

    public override string DisplayName
    {
      get
      {
        string? positionName = Position != null ? Position.DisplayName : null;
        string? groupName = Group != null ? Group.DisplayName : null;

        return string.Format("{0} / {1}", positionName ?? "?", groupName ?? "?" );
      }
    }
  }
}
