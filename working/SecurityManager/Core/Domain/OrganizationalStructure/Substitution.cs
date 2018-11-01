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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// The <see cref="Substitution"/> type defines the association between two <see cref="User"/>s and optionally a <see cref="Role"/> where the 
  /// <see cref="SubstitutingUser"/> can act as a stand-in for the <see cref="SubstitutedUser"/> and <see cref="SubstitutedRole"/>.
  /// </summary>
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Substitution")]
  [PermanentGuid ("5F3FEEE1-38E3-4ecb-AC2F-2D74AFFE3A27")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Substitution : BaseSecurityManagerObject
  {
    public static Substitution NewObject ()
    {
      return NewObject<Substitution>();
    }

    protected Substitution ()
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      IsEnabled = true;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
    }

    [DBBidirectionalRelation ("SubstitutingFor")]
    [Mandatory]
    [SearchAvailableObjectsServiceType (typeof (UserPropertyTypeSearchService))]
    public abstract User SubstitutingUser { get; set; }

    [DBBidirectionalRelation ("SubstitutedBy")]
    [Mandatory]
    public abstract User SubstitutedUser { get; set; }

    [DBBidirectionalRelation ("SubstitutedBy")]
    [SearchAvailableObjectsServiceType (typeof (SubstitutionPropertiesSearchService))]
    public abstract Role SubstitutedRole { get; set; }

    [DateProperty]
    public abstract DateTime? BeginDate { get; set; }

    [DateProperty]
    public abstract DateTime? EndDate { get; set; }

    public abstract bool IsEnabled { get; set; }

    /// <summary>
    /// The <see cref="Substitution"/> is only active when the object is <see cref="StateType.Unchanged"/>, the <see cref="IsEnabled"/> flag is set
    /// and the present date is within the range defined by <see cref="BeginDate"/> and <see cref="EndDate"/>.
    /// </summary>
    [StorageClassNone]
    public bool IsActive
    {
      get
      {
        EnsureDataAvailable ();
        if (State != StateType.Unchanged)
          return false;

        if (!IsEnabled)
          return false;

        if (BeginDate.HasValue && BeginDate.Value.Date > DateTime.Today)
          return false;

        if (EndDate.HasValue && EndDate.Value.Date < DateTime.Today)
          return false;

        return true;
      }
    }

    public override string DisplayName
    {
      get
      {
        string userName = SubstitutedUser != null ? SubstitutedUser.DisplayName : null;
        string roleName = SubstitutedRole != null ? SubstitutedRole.DisplayName : null;

        string displayName = userName ?? "?";
        if (roleName != null)
          displayName += " (" + roleName + ")";

        return displayName;
      }
    }

    protected override void OnCommitting (DomainObjectCommittingEventArgs args)
    {
      base.OnCommitting (args);

      var substitutedUserProperty = Properties[typeof (Substitution), "SubstitutedUser"];
      if (substitutedUserProperty.GetValue<User>() != null)
        substitutedUserProperty.GetValue<User>().RegisterForCommit();
      else if (substitutedUserProperty.GetOriginalValue<User>() != null)
        substitutedUserProperty.GetOriginalValue<User>().RegisterForCommit();
    }
  }
}
