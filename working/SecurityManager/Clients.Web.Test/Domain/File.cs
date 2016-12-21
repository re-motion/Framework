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
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Clients.Web.Test.Domain
{
  [PermanentGuid ("BAA77408-32E6-4979-9914-8A12B71808F2")]
  [Instantiable]
  [DBTable]
  [DBStorageGroup]
  public abstract class File : BaseSecurableObject, ISupportsGetObject
  {
    public enum Method
    {
      CreateFileItem
    }

    public static File NewObject (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return DomainObject.NewObject<File> ();
      }
    }

    protected File ()
    {
    }

    [Mandatory]
    public abstract Tenant Tenant { get; set; }

    //[DemandPropertyGetterPermission (DomainAccessTypes.ReadName)]
    //[DemandPropertySetterPermission (DomainAccessTypes.WriteName)]
    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [PermanentGuid ("4B073E2B-C56D-419c-8358-808FDEF669EF")]
    public abstract Confidentiality Confidentiality { get; set; }

    [Mandatory]
    public abstract User Creator { get; set; }

    public abstract User Clerk { get; set; }

    public abstract Group Group { get; set; }

    [DBBidirectionalRelation ("File")]
    public abstract ObjectList<FileItem> Files { get; set; }

    public override User GetOwner ()
    {
      return Clerk;
    }

    public override Group GetOwnerGroup ()
    {
      if (Clerk == null)
        return null;
      return Clerk.OwningGroup;
    }

    public override Tenant GetOwnerTenant ()
    {
      return Tenant;
    }
  }
}
