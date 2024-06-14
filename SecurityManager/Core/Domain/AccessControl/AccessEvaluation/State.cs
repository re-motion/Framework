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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public class State
  {
    private readonly IDomainObjectHandle<StatePropertyDefinition> _propertyHandle;
    private readonly string _propertyName;
    private readonly string _value;

    public State (IDomainObjectHandle<StatePropertyDefinition> propertyHandle, string propertyName, string value)
    {
      ArgumentUtility.CheckNotNull("propertyHandle", propertyHandle);
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);
      ArgumentUtility.CheckNotNullOrEmpty("value", value);

      _propertyHandle = propertyHandle;
      _propertyName = propertyName;
      _value = value;
    }

    public IDomainObjectHandle<StatePropertyDefinition> PropertyHandle
    {
      get { return _propertyHandle; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public string Value
    {
      get { return _value; }
    }
  }
}
