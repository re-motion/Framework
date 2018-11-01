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

namespace Remotion.Web.ExecutionEngine.UrlMapping
{
/// <summary> Represents the common information all configuration classes provide. </summary>
public class ConfigurationBase
{
  // types

  // static members and constants

  // member fields

  private string _applicationName;

  // construction and disposing

  /// <summary> Initializes a new instance of the <b>ConfigurationBase</b> type. </summary>
  protected ConfigurationBase (string applicationName)
  {
    _applicationName = applicationName;
  }

  /// <summary> XML Deserialization contructor. </summary>
  /// <exclude />
  protected ConfigurationBase()
  {
  }

  // methods and properties

  /// <summary> Gets the application name that is specified in the XML configuration file.  </summary>
  [XmlAttribute ("application")]
  public string ApplicationName
  {
    get { return _applicationName; }
  }
}
}
