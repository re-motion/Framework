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

namespace Remotion.Security.Metadata
{

  public class SecurableClassInfo : MetadataInfo
  {
    // types

    // static members

    // member fields

    private List<StatePropertyInfo> _properties = new List<StatePropertyInfo>();
    private List<EnumValueInfo> _accessTypes = new List<EnumValueInfo>();
    private SecurableClassInfo _baseClass;
    private List<SecurableClassInfo> _derivedClasses = new List<SecurableClassInfo> ();

    // construction and disposing

    public SecurableClassInfo ()
    {
    }

    // methods and properties

    public List<StatePropertyInfo> Properties
    {
      get { return _properties; }
    }

    public List<EnumValueInfo> AccessTypes
    {
      get { return _accessTypes; }
    }

    public SecurableClassInfo BaseClass
    {
      get { return _baseClass; }
      set { _baseClass = value; }
    }

    public List<SecurableClassInfo> DerivedClasses
    {
      get { return _derivedClasses; }
    }
	
  }
}
