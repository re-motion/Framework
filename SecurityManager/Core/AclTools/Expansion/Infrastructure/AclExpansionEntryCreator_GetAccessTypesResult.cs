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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  public class AclExpansionEntryCreator_GetAccessTypesResult
  {
    public AclProbe AclProbe { get; private set; }
    public AccessTypeStatistics AccessTypeStatistics { get; private set; }
    public AccessInformation AccessInformation { get; private set; }

    public AclExpansionEntryCreator_GetAccessTypesResult (AccessInformation accessInformation, AclProbe aclProbe, AccessTypeStatistics accessTypeStatistics)
    {
      AclProbe = aclProbe;
      AccessTypeStatistics = accessTypeStatistics;
      AccessInformation = accessInformation;
    }
  }
}
