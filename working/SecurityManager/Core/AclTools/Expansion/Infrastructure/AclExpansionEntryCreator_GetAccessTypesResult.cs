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
