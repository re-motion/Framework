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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators
{
  /// <summary>
  /// Implements the property path parsing logic as specified by <see cref="IBusinessObjectPropertyPathPropertyEnumerator"/> 
  /// but allows for customization when handling invalid property path information via the template methods <see cref="HandlePropertyNotFound"/> and
  /// <see cref="HandlePropertyNotLastPropertyAndNotReferenceProperty"/>.
  /// </summary>
  public abstract class BusinessObjectPropertyPathPropertyEnumeratorBase : IBusinessObjectPropertyPathPropertyEnumerator
  {
    private string _remainingPropertyPathIdentifier;
    private bool _isEnumerationFinished;
    private IBusinessObjectProperty _currentProperty;
    private bool _isEnumerationStarted;

    protected BusinessObjectPropertyPathPropertyEnumeratorBase (string propertyPathIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);

      _remainingPropertyPathIdentifier = propertyPathIdentifier;
    }

    protected abstract void HandlePropertyNotFound (
        IBusinessObjectClass businessObjectClass,
        string propertyIdentifier);

    protected abstract void HandlePropertyNotLastPropertyAndNotReferenceProperty (
        IBusinessObjectClass businessObjectClass,
        IBusinessObjectProperty property);

    public IBusinessObjectProperty Current
    {
      get
      {
        if (!_isEnumerationStarted)
          throw new InvalidOperationException ("Enumeration has not started. Call MoveNext.");

        if (_isEnumerationFinished)
          throw new InvalidOperationException ("Enumeration already finished.");

        return _currentProperty; }
    }

    public bool HasNext
    {
      get { return _remainingPropertyPathIdentifier.Length > 0; }
    }

    public bool MoveNext (IBusinessObjectClass currentClass)
    {
      ArgumentUtility.CheckNotNull ("currentClass", currentClass);

      _isEnumerationStarted = true;
      _currentProperty = null;

      if (!HasNext)
      {
        _isEnumerationFinished = true;
        return false;
      }

      var propertyIdentifierAndRemainder =
          _remainingPropertyPathIdentifier.Split (
              new[] { currentClass.BusinessObjectProvider.GetPropertyPathSeparator() }, 2, StringSplitOptions.None);

      if (propertyIdentifierAndRemainder.Length == 2)
        _remainingPropertyPathIdentifier = propertyIdentifierAndRemainder[1];
      else
        _remainingPropertyPathIdentifier = string.Empty;

      var propertyIdentifier = propertyIdentifierAndRemainder[0];
      var property = currentClass.GetPropertyDefinition (propertyIdentifier);

      if (property == null)
        HandlePropertyNotFound (currentClass, propertyIdentifier);
      else if (HasNext && ! (property is IBusinessObjectReferenceProperty))
        HandlePropertyNotLastPropertyAndNotReferenceProperty (currentClass, property);
      else
        _currentProperty = property;

      return true;
    }
  }
}