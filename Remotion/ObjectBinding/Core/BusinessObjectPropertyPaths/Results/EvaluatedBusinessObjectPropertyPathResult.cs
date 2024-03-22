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

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results
{
  /// <summary>
  /// Implements <see cref="IBusinessObjectPropertyPathResult"/> for a successfully evaluated property path.
  /// </summary>
  /// <remarks>
  /// <see cref="GetValue"/> and <see cref="GetString"/> may still not return the actual value if the <see cref="ResultProperty"/> is not accessible
  /// for the <see cref="ResultObject"/>.
  /// </remarks>
  public sealed class EvaluatedBusinessObjectPropertyPathResult : IBusinessObjectPropertyPathResult
  {
    private readonly IBusinessObject _resultObject;
    private readonly IBusinessObjectProperty _resultProperty;

    public EvaluatedBusinessObjectPropertyPathResult (IBusinessObject resultObject, IBusinessObjectProperty resultProperty)
    {
      ArgumentUtility.CheckNotNull("resultObject", resultObject);
      ArgumentUtility.CheckNotNull("resultProperty", resultProperty);

      _resultObject = resultObject;
      _resultProperty = resultProperty;
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    public object? GetValue ()
    {
      if (!_resultProperty.IsAccessible(_resultObject))
        return null;

      try
      {
        return _resultObject.GetProperty(_resultProperty);
      }
      catch (BusinessObjectPropertyAccessException)
      {
        return null;
      }
    }

    public string GetString (string? format)
    {
      if (!_resultProperty.IsAccessible(_resultObject))
        return _resultObject.BusinessObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder();

      try
      {
        return _resultObject.GetPropertyString(_resultProperty, format);
      }
      catch (BusinessObjectPropertyAccessException)
      {
        return _resultObject.BusinessObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder();
      }
    }

    public IBusinessObjectProperty ResultProperty
    {
      get { return _resultProperty; }
    }

    public IBusinessObject ResultObject
    {
      get { return _resultObject; }
    }
  }
}
