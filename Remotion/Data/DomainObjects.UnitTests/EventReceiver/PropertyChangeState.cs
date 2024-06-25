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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public class PropertyChangeState : ChangeState
  {
    // types

    // static members and constants

    // member fields

    private readonly PropertyDefinition _propertyDefinition;
    private readonly object _oldValue;
    private readonly object _newValue;

    // construction and disposing

    public PropertyChangeState (object sender, PropertyDefinition propertyDefinition, object oldValue, object newValue, string message = null)
      : base(sender, message)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      _propertyDefinition = propertyDefinition;
      _oldValue = oldValue;
      _newValue = newValue;
    }

    // methods and properties

    public PropertyDefinition PropertyDefinition
    {
      get { return _propertyDefinition; }
    }

    public object OldValue
    {
      get { return _oldValue; }
    }

    public object NewValue
    {
      get { return _newValue; }
    }

    public override void Check (ChangeState expectedState)
    {
      base.Check(expectedState);

      PropertyChangeState propertyChangeState = (PropertyChangeState)expectedState;

      if (_propertyDefinition != propertyChangeState.PropertyDefinition)
      {
        throw CreateApplicationException(
            "Actual PropertyName '{0}' and expected PropertyName '{1}' do not match.",
            _propertyDefinition.PropertyName,
            propertyChangeState.PropertyDefinition.PropertyName);
      }

      if (!Equals(_oldValue, propertyChangeState.OldValue))
      {
        throw CreateApplicationException(
            "Actual old value '{0}' and expected old value '{1}' do not match.",
            _oldValue,
            propertyChangeState.OldValue);
      }

      if (!Equals(_newValue, propertyChangeState.NewValue))
      {
        throw CreateApplicationException(
            "Actual new value '{0}' and expected new value '{1}' do not match.",
            _newValue,
            propertyChangeState.NewValue);
      }
    }
  }
}
