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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [MultiLingualResources("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.User")]
  [PermanentGuid("759DA370-E2C4-4221-B878-BE378C916042")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class User : OrganizationalStructureObject, ISupportsGetObject
  {
    public enum Methods
    {
      //Create
      Search
    }

    public static Expression<Func<User, IEnumerable<Substitution>>> SelectSubstitutions ()
    {
      return user => user.SubstitutingFor;
    }

    internal static User NewObject ()
    {
      return NewObject<User>();
    }

    public static User? FindByUserName (string userName)
    {
      ArgumentUtility.CheckNotNull("userName", userName);

      var result = from u in QueryFactory.CreateLinqQuery<User>()
                   where u.UserName == userName
                   select u;

      return result.SingleOrDefault();
    }

    public static IQueryable<User> FindByTenant (IDomainObjectHandle<Tenant> tenantHandle)
    {
      ArgumentUtility.CheckNotNull("tenantHandle", tenantHandle);

      return from u in QueryFactory.CreateLinqQuery<User>()
             where u.Tenant!.ID == tenantHandle.ObjectID
             orderby u.LastName, u.FirstName
             select u;
    }

    //[DemandMethodPermission (GeneralAccessTypes.Create)]
    //public static User Create ()
    //{
    //  return SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateUser ();
    //}

    [DemandPermission(GeneralAccessTypes.Search)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException("This method is only intended for framework support and should never be called.");
    }

    private DomainObjectDeleteHandler? _deleteHandler;

    protected User ()
    {
    }

    [StringProperty(MaximumLength = 100)]
    public abstract string? Title { get; set; }

    [StringProperty(MaximumLength = 100)]
    public abstract string? FirstName { get; set; }

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string LastName { get; set; }

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string UserName { get; set; }

    [DBBidirectionalRelation("User")]
    public abstract ObjectList<Role> Roles { get; [DemandPermission(SecurityManagerAccessTypes.AssignRole)] protected set; }

    [Mandatory]
    public abstract Tenant? Tenant { get; set; }

    [Mandatory]
    [SearchAvailableObjectsServiceType(typeof(GroupPropertyTypeSearchService))]
    public abstract Group? OwningGroup { get; set; }

    [DBBidirectionalRelation("SubstitutingUser")]
    protected abstract ObjectList<Substitution> SubstitutingFor { get; }

    [DBBidirectionalRelation("SubstitutedUser")]
    public abstract ObjectList<Substitution> SubstitutedBy { get; [DemandPermission(SecurityManagerAccessTypes.AssignSubstitute)] protected set; }

    public IEnumerable<Substitution> GetActiveSubstitutions ()
    {
      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      if (!securityClient.HasAccess(this, AccessType.Get(GeneralAccessTypes.Read)))
        return new Substitution[0];

      return SubstitutingFor.Where(s => s.IsActive);
    }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting(args);

      using (DefaultTransactionContext.ClientTransaction.EnterNonDiscardingScope())
      {
        var aces = QueryFactory.CreateLinqQuery<AccessControlEntry>().Where(ace => ace.SpecificUser == this);

        _deleteHandler = new DomainObjectDeleteHandler(aces, Roles, SubstitutingFor, SubstitutedBy);
      }
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted(args);

      _deleteHandler?.Delete();
    }

    public override string DisplayName
    {
      get { return GetFormattedName(); }
    }

    private string GetFormattedName ()
    {
      string formattedName = LastName;

      if (!string.IsNullOrEmpty(FirstName))
        formattedName += " " + FirstName;

      if (!string.IsNullOrEmpty(Title))
        formattedName += ", " + Title;

      return formattedName;
    }

    protected override string GetOwner ()
    {
      return UserName;
    }

    protected override string? GetOwningTenant ()
    {
      return Tenant == null ? null : Tenant.UniqueIdentifier;
    }

    protected override string? GetOwningGroup ()
    {
      return OwningGroup == null ? null : OwningGroup.UniqueIdentifier;
    }
  }
}
