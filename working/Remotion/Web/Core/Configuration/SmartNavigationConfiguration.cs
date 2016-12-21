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
/// <include file='..\doc\include\Configuration\SmartNavigationConfiguration.xml' path='SmartNavigationConfiguration/Class/*' />
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class SmartNavigationConfiguration
{
  private bool _enableScrolling = true;
  private bool _enableFocusing = true;

  /// <summary> Gets or sets a flag that determines whether to enable smart scrolling. </summary>
  /// <value> <see langword="true"/> to enable smart scrolling. Defaults to <see langword="true"/>. </value>
  [XmlAttribute ("enableScrolling")]
  public bool EnableScrolling
  {
    get { return _enableScrolling; }
    set { _enableScrolling = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to enable smart focus. </summary>
  /// <value> <see langword="true"/> to enable smart focusing. Defaults to <see langword="true"/>. </value>
  [XmlAttribute ("enableFocusing")]
  public bool EnableFocusing
  {
    get { return _enableFocusing; }
    set { _enableFocusing = value; }
  }
}

}
