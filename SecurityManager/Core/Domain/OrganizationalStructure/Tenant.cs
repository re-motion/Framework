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
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Tenant")]
  [PermanentGuid("BD8FB1A4-E300-4663-AB1E-D6BD7B106619")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Tenant : OrganizationalStructureObject, ISupportsGetObject
  {
    public enum Methods
    {
      Search
    }

    internal static Tenant NewObject ()
    {
      return NewObject<Tenant>();
    }

    public static IQueryable<Tenant> FindAll ()
    {
      return from t in QueryFactory.CreateLinqQuery<Tenant>()
             orderby t.Name
             select t;
    }

    public static Tenant? FindByUnqiueIdentifier (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty("uniqueIdentifier", uniqueIdentifier);

      var result = from t in QueryFactory.CreateLinqQuery<Tenant>()
                   where t.UniqueIdentifier == uniqueIdentifier
                   select t;

      return result.SingleOrDefault();
    }

    [DemandPermission(GeneralAccessTypes.Search)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException("This method is only intended for framework support and should never be called.");
    }

    protected Tenant ()
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

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string UniqueIdentifier { get; set; }

    public abstract bool IsAbstract { get; set; }

    [DBBidirectionalRelation("Children")]
    [SearchAvailableObjectsServiceType(typeof(TenantPropertyTypeSearchService))]
    public abstract Tenant? Parent { get; set; }

    [DBBidirectionalRelation("Parent")]
    public abstract ObjectList<Tenant> Children { get; }

    public override string DisplayName
    {
      get { return Name; }
    }

    protected override string GetOwningTenant ()
    {
      return UniqueIdentifier;
    }

    /// <summary>
    /// Gets all the <see cref="Tenant"/> objects in the <see cref="Parent"/> hierarchy, 
    /// provided the user has read access for the respective parent-object.
    /// </summary>
    /// <remarks>
    ///   <para>If the user does not have read access for the respective <see cref="Tenant"/>, the parent hierarchy evaluation stops at this point.</para>
    ///   <para>If the user does not have read access on the current object, an empty sequence is returned.</para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the parent hierarchy contains a circular reference.
    /// </exception>
    [DemandPermission(GeneralAccessTypes.Read)]
    public IEnumerable<Tenant> GetParents ()
    {
      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      if (!securityClient.HasMethodAccess(this, "GetParents"))
        return Enumerable.Empty<Tenant>();

      return Parent.CreateSequenceWithCycleCheck(
          t => t.Parent,
          t => t != null && securityClient.HasAccess(t, AccessType.Get(GeneralAccessTypes.Read)),
          null,
          t => new InvalidOperationException(
              string.Format("The parent hierarchy for tenant '{0}' cannot be resolved because a circular reference exists.", ID))
          );
    }

    /// <summary>
    /// Gets the <see cref="Tenant"/> and all of its <see cref="Children"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     If the user does not have read access for the respective <see cref="Tenant"/>, the hierarchy evaluation stops at this point for the evaluated branch.
    ///   </para>
    ///   <para>If the user does not have read access on the current object, an empty sequence is returned.</para>para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the hierarchy contains a circular reference.
    /// </exception>
    public IEnumerable<Tenant> GetHierachy ()
    {
      return GetHierarchyWithSecurityCheck(this);
    }

    /// <summary>
    /// Resolves the hierarchy for the current <see cref="Tenant"/> as long as the user has <see cref="GeneralAccessTypes.Read"/> permissions 
    /// on the current object.
    /// </summary>
    private IEnumerable<Tenant> GetHierarchyWithSecurityCheck (Tenant startPoint)
    {
      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      if (!securityClient.HasAccess(this, AccessType.Get(GeneralAccessTypes.Read)))
        return Enumerable.Empty<Tenant>();

      return new[] { this }.Concat(Children.SelectMany(c => c.GetHierarchyWithCircularReferenceCheck(startPoint)));
    }

    /// <summary>
    /// Resolves the hierarchy for the current <see cref="Tenant"/> as long as the current object is not equal to the <param name="startPoint"/>, 
    /// which would indicate a circular reference.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the current object equals the <paramref name="startPoint"/>.
    /// </exception>
    private IEnumerable<Tenant> GetHierarchyWithCircularReferenceCheck (Tenant startPoint)
    {
      if (this == startPoint)
      {
        throw new InvalidOperationException(
            string.Format("The hierarchy for tenant '{0}' cannot be resolved because a circular reference exists.", startPoint.ID));
      }

      return GetHierarchyWithSecurityCheck(startPoint);
    }
  }
}
