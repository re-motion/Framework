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
using System.Xml.Serialization;

namespace Remotion.Web.Configuration
{

/// <summary> Configuration section entry for configuring the <b>Remotion.Web.ExecutionEngine</b>. </summary>
/// <include file='..\doc\include\Configuration\ExecutionEngineConfiguration.xml' path='ExecutionEngineConfiguration/Class/*' />
[XmlType(Namespace = WebConfiguration.SchemaUri)]
public class ExecutionEngineConfiguration
{
  private int _functionTimeout = 20;
  private bool _enableSessionManagement = true;
  private int _refreshInterval = 10;

  /// <summary> Gets or sets the default timeout for individual functions within one session. </summary>
  /// <value> The timeout in minutes. Defaults to 20 minutes. Must be greater than zero. </value>
  [XmlAttribute("functionTimeout")]
  public int FunctionTimeout
  {
    get
    {
      return _functionTimeout;
    }
    set
    {
      if (value < 1)
        throw new ArgumentException("The FunctionTimeout must be greater than zero.");
      _functionTimeout = value;
    }
  }

  /// <summary> Gets or sets a flag that determines whether session management is employed. </summary>
  /// <include file='..\doc\include\Configuration\ExecutionEngineConfiguration.xml' path='ExecutionEngineConfiguration/EnableSessionManagement/*' />
  [XmlAttribute("enableSessionManagement")]
  public bool EnableSessionManagement
  {
    get { return _enableSessionManagement; }
    set { _enableSessionManagement = value; }
  }

  /// <summary> Gets or sets the default refresh interval for a function. </summary>
  /// <include file='..\doc\include\Configuration\ExecutionEngineConfiguration.xml' path='ExecutionEngineConfiguration/RefreshInterval/*' />
  [XmlAttribute("refreshInterval")]
  public int RefreshInterval
  {
    get
    {
      return _refreshInterval;
    }
    set
    {
      if (value < 0)
        throw new ArgumentException("The RefreshInterval must not be a negative number.");
      _refreshInterval = value;
    }
  }
}

}
