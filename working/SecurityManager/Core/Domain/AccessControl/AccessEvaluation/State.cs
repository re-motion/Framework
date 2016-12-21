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
      ArgumentUtility.CheckNotNull ("propertyHandle", propertyHandle);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);

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