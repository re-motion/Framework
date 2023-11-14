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
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  public class DomainObjectDeleteHandler
  {
    private BaseSecurityManagerObject[]? _objectsToBeDeleted;

    public DomainObjectDeleteHandler (params IEnumerable[] lists)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("lists", lists);

      _objectsToBeDeleted = lists.SelectMany(objects => objects.Cast<BaseSecurityManagerObject>()).ToArray();
    }

    [MemberNotNullWhen(false, nameof(_objectsToBeDeleted))]
    public bool IsDeleted
    {
      get { return _objectsToBeDeleted == null; }
    }

    public void Delete ()
    {
      if (IsDeleted)
        throw new InvalidOperationException("The Delete operation my only be performed once.");

      foreach (BaseSecurityManagerObject domainObject in _objectsToBeDeleted.Where(o =>!o.State.IsInvalid))
        domainObject.Delete();

      _objectsToBeDeleted = null;
    }
  }
}
