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

namespace Remotion.SecurityManager.Domain
{
  [Serializable]
  public sealed class GuidRevisionValue : IRevisionValue
  {
    private readonly DateTime _timestamp;
    private readonly Guid _revision;

    public GuidRevisionValue (Guid revision)
    {
      _timestamp = DateTime.UtcNow;
      _revision = revision;
    }

    public bool IsCurrent (IRevisionValue reference)
    {
      var referenceRevision = reference as GuidRevisionValue;
      if (referenceRevision == null)
        return false;

      if (_revision == referenceRevision._revision)
        return true;
      if (_timestamp > referenceRevision._timestamp)
        return true;
      return false;
    }
  }
}