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
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Position")]
  [PermanentGuid("5BBE6C4D-DC88-4a27-8BFF-0AC62EE34333")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Position : OrganizationalStructureObject, ISupportsGetObject
  {
    private DomainObjectDeleteHandler? _deleteHandler;

    public enum Methods
    {
      //Create
      Search
    }

    internal static Position NewObject ()
    {
      return NewObject<Position>();
    }

    public static IQueryable<Position> FindAll ()
    {
      return from p in QueryFactory.CreateLinqQuery<Position>()
             orderby p.Name
             select p;
    }

    [DemandPermission(SecurityManagerAccessTypes.AssignRole)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Dummy_AssignRole ()
    {
      throw new NotImplementedException("This method is only intended for framework support and should never be called.");
    }

    //[DemandMethodPermission (GeneralAccessTypes.Create)]
    //public static Position Create ()
    //{
    //  return SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreatePosition ();
    //}

    [DemandPermission(GeneralAccessTypes.Search)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException("This method is only intended for framework support and should never be called.");
    }

    protected Position ()
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

    [ObjectBinding(Visible = false)]
    [PermanentGuid("5C31F600-88F3-4ff7-988C-0E45A857AB4B")]
    public abstract Delegation Delegation { get; set; }

    [StorageClassNone]
    public bool Delegable
    {
      get { return Delegation == Delegation.Enabled; }
      set { Delegation = value ? Delegation.Enabled : Delegation.Disabled; }
    }

    [DBBidirectionalRelation("Position")]
    public abstract ObjectList<GroupTypePosition> GroupTypes { get; }

    public override string DisplayName
    {
      get { return Name; }
    }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting(args);

      using (DefaultTransactionContext.ClientTransaction.EnterNonDiscardingScope())
      {
        var aces = QueryFactory.CreateLinqQuery<AccessControlEntry>().Where(ace => ace.SpecificPosition == this);
        var roles = QueryFactory.CreateLinqQuery<Role>().Where(r => r.Position == this);

        _deleteHandler = new DomainObjectDeleteHandler(aces, roles, GroupTypes);
      }
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted(args);

      _deleteHandler?.Delete();
    }

    protected override IDictionary<string, Enum> GetStates ()
    {
      IDictionary<string, Enum> states = base.GetStates();
      states.Add("Delegation", Delegation);

      return states;
    }
  }
}
