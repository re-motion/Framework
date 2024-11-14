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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [MultiLingualResources("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Group")]
  [PermanentGuid("AA1761A4-226C-4ebe-91F0-8FFF4974B175")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Group : OrganizationalStructureObject, ISupportsGetObject
  {
    // types

    public enum Methods
    {
      //Create
      Search
    }

    // static members and constants

    internal static Group NewObject ()
    {
      return NewObject<Group>();
    }

    public static IQueryable<Group> FindByTenant (IDomainObjectHandle<Tenant> tenantHandle)
    {
      ArgumentUtility.CheckNotNull("tenantHandle", tenantHandle);

      return from g in QueryFactory.CreateLinqQuery<Group>()
                   where g.Tenant!.ID == tenantHandle.ObjectID
                   orderby g.Name, g.ShortName
                   select g;
    }

    public static Group? FindByUnqiueIdentifier (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty("uniqueIdentifier", uniqueIdentifier);

      var result = from g in QueryFactory.CreateLinqQuery<Group>()
                   where g.UniqueIdentifier == uniqueIdentifier
                   select g;

      return result.SingleOrDefault();
    }

    //[DemandMethodPermission (GeneralAccessTypes.Create)]
    //public static Group Create ()
    //{
    //  return SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateGroup ();
    //}

    [DemandPermission(GeneralAccessTypes.Search)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException("This method is only intended for framework support and should never be called.");
    }

    // member fields

    private DomainObjectDeleteHandler? _deleteHandler;

    // construction and disposing

    protected Group ()
    {
      // The SecurityFreeSection is reduntant if the security strategy created by the OrganizationalStructureObject uses no security for new objects.
      // Since creating the security strategy represents an extension point, separate precaution for setting the UniqueIdentifier in the ctor must be taken.
      using (SecurityFreeSection.Activate())
      {
        // ReSharper disable DoNotCallOverridableMethodsInConstructor
        UniqueIdentifier = Guid.NewGuid().ToString();
        // ReSharper restore DoNotCallOverridableMethodsInConstructor
      }
    }

    // methods and properties

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty(MaximumLength = 20)]
    public abstract string? ShortName { get; set; }

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string UniqueIdentifier { get; set; }

    [Mandatory]
    public abstract Tenant? Tenant { get; set; }

    [DBBidirectionalRelation("Children")]
    [SearchAvailableObjectsServiceType(typeof(GroupPropertyTypeSearchService))]
    public abstract Group? Parent { get; set; }

    [DBBidirectionalRelation("Parent")]
    public abstract ObjectList<Group> Children { get; }

    [SearchAvailableObjectsServiceType(typeof(GroupTypePropertyTypeSearchService))]
    public abstract GroupType? GroupType { get; set; }


    [DBBidirectionalRelation("Group")]
    public abstract ObjectList<Role> Roles { get; [DemandPermission(SecurityManagerAccessTypes.AssignRole)] protected set; }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting(args);

      using (DefaultTransactionContext.ClientTransaction.EnterNonDiscardingScope())
      {
        var aces = QueryFactory.CreateLinqQuery<AccessControlEntry>().Where(ace => ace.SpecificGroup == this);

        _deleteHandler = new DomainObjectDeleteHandler(aces, Roles);
      }
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted(args);

      _deleteHandler?.Delete();
    }

    public override string DisplayName
    {
      get
      {
        if (string.IsNullOrEmpty(ShortName))
          return Name;
        else
          return string.Format("{0} ({1})", ShortName, Name);
      }
    }

    protected override string? GetOwningTenant ()
    {
      return Tenant == null ? null : Tenant.UniqueIdentifier;
    }

    protected override string GetOwningGroup ()
    {
      return UniqueIdentifier;
    }

    /// <summary>
    /// Gets all the <see cref="Group"/> objects in the <see cref="Parent"/> hierarchy, 
    /// provided the user has read access for the respective parent-object.
    /// </summary>
    /// <remarks>
    ///   <para>If the user does not have read access for the respective <see cref="Group"/>, the parent hierarchy evaluation stops at this point.</para>
    ///   <para>If the user does not have read access on the current object, an empty sequence is returned.</para>para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the parent hierarchy contains a circular reference.
    /// </exception>
    [DemandPermission(GeneralAccessTypes.Read)]
    public IEnumerable<Group> GetParents ()
    {
      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      if (!securityClient.HasMethodAccess(this, "GetParents"))
        return Enumerable.Empty<Group>();

      return Parent.CreateSequenceWithCycleCheck(
          g => g.Parent,
          g => g != null && securityClient.HasAccess(g, AccessType.Get(GeneralAccessTypes.Read)),
          null,
          g => new InvalidOperationException(
              string.Format("The parent hierarchy for group '{0}' cannot be resolved because a circular reference exists.", ID)));
    }

    /// <summary>
    /// Gets the <see cref="Group"/> and all of its <see cref="Children"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     If the user does not have read access for the respective <see cref="Group"/>, the hierarchy evaluation stops at this point for the evaluated branch.
    ///   </para>
    ///   <para>If the user does not have read access on the current object, an empty sequence is returned.</para>para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the hierarchy contains a circular reference.
    /// </exception>
    public IEnumerable<Group> GetHierachy ()
    {
      return GetHierarchyWithSecurityCheck(this);
    }

    /// <summary>
    /// Resolves the hierarchy for the current <see cref="Group"/> as long as the user has <see cref="GeneralAccessTypes.Read"/> permissions
    /// on the current object.
    /// </summary>
    private IEnumerable<Group> GetHierarchyWithSecurityCheck (Group startPoint)
    {
      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      if (!securityClient.HasAccess(this, AccessType.Get(GeneralAccessTypes.Read)))
        return Enumerable.Empty<Group>();

      return new[] { this }.Concat(Children.SelectMany(c => c.GetHierarchyWithCircularReferenceCheck(startPoint)));
    }

    /// <summary>
    /// Resolves the hierarchy for the current <see cref="Group"/> as long as the current object is not equal to the <param name="startPoint"/>, 
    /// which would indicate a circular reference.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the current object equals the <paramref name="startPoint"/>.
    /// </exception>
    private IEnumerable<Group> GetHierarchyWithCircularReferenceCheck (Group startPoint)
    {
      if (this == startPoint)
      {
        throw new InvalidOperationException(
            string.Format("The hierarchy for group '{0}' cannot be resolved because a circular reference exists.", startPoint.ID));
      }

      return GetHierarchyWithSecurityCheck(startPoint);
    }

    protected override void OnCommitting (DomainObjectCommittingEventArgs args)
    {
      base.OnCommitting(args);

      CheckParentHierarchy();
    }

    private void CheckParentHierarchy ()
    {
      if (!Properties[typeof(Group), "Parent"].HasChanged)
        return;

      var parents = GetParentObjectReference().CreateSequenceWithCycleCheck(
          g => g.GetParentObjectReference(),
          g => new InvalidOperationException(
              string.Format("Group '{0}' cannot be committed because it would result in a circular parent hierarchy.", ID)));

      foreach (var group in parents)
        group.RegisterForCommit();
    }

    private Group? GetParentObjectReference ()
    {
      var parentID = Properties[typeof(Group), "Parent"].GetRelatedObjectID();
      if (parentID == null)
        return null;

      var parent = (Group)LifetimeService.GetObjectReference(DefaultTransactionContext.ClientTransaction, parentID);
      UnloadService.TryUnloadData(DefaultTransactionContext.ClientTransaction, parent.ID);

      return parent;
    }
  }
}
