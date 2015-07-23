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
  [PermanentGuid ("DF0A8DB4-943C-4bd1-8B3B-276C8AA16BDB")]
  [Instantiable]
  [DBTable]
  [DBStorageGroup]
  public abstract class FileItem : BaseSecurableObject, ISupportsGetObject
  {
    public static FileItem NewObject (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return DomainObject.NewObject<FileItem> ();
      }
    }

    protected FileItem ()
    {
    }

    [Mandatory]
    public abstract Tenant Tenant { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Files")]
    [Mandatory]
    public abstract File File { get; set; }

    public override User GetOwner ()
    {
      if (File == null)
        return null;
      return File.GetOwner();
    }

    public override Group GetOwnerGroup ()
    {
      if (File == null)
        return null;
      return File.GetOwnerGroup ();
    }

    public override Tenant GetOwnerTenant ()
    {
      if (File == null)
        return null;
      return File.GetOwnerTenant ();
    }
  }
}
