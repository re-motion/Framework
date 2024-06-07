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
