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
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class AccessInformation
  {
    private readonly AccessTypeDefinition[] _allowedAccessTypes;
    private readonly AccessTypeDefinition[] _deniedAccessTypes;

    public AccessInformation (AccessTypeDefinition[] allowedAccessTypes, AccessTypeDefinition[] deniedAccessTypes)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("allowedAccessTypes", allowedAccessTypes);
      ArgumentUtility.CheckNotNullOrItemsNull("deniedAccessTypes", deniedAccessTypes);

      _allowedAccessTypes = allowedAccessTypes;
      _deniedAccessTypes = deniedAccessTypes;
    }

    public AccessTypeDefinition[] AllowedAccessTypes
    {
      get { return _allowedAccessTypes; }
    }

    public AccessTypeDefinition[] DeniedAccessTypes
    {
      get { return _deniedAccessTypes; }
    }
  }
}
