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
using System.Configuration.Internal;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Configuration
{
  /// <summary>
  /// Fake implementation of the <see cref="IInternalConfigSystem"/> interface. Used by the <see cref="ConfigSystemHelper"/> to fake the 
  /// configuration.
  /// </summary>
  public class FakeInternalConfigSystem: IInternalConfigSystem
  {
    private Dictionary<string, object> _sections = new Dictionary<string, object>();

    public FakeInternalConfigSystem()
    {
    }

    public object GetSection (string configKey)
    {
      object value;
      if (_sections.TryGetValue (configKey, out value))
        return value;
      return null;
    }

    public void AddSection (string configKey, object section)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("configKey", configKey);
      ArgumentUtility.CheckNotNull ("section", section);

      _sections.Add (configKey, section);
    }

    public void RefreshConfig (string sectionName)
    {
      throw new NotSupportedException();
    }

    public bool SupportsUserConfig
    {
      get { return false; }
    }
  }
}
