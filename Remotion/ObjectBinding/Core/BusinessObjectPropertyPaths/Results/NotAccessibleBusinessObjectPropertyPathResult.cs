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

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results
{
  /// <summary>
  /// Implements <see cref="IBusinessObjectPropertyPathResult"/> for a property path where the <see cref="ResultObject"/> was not be accessible.
  /// </summary>
  /// <remarks><see cref="ResultProperty"/> and <see cref="ResultObject"/> will always be <see langword="null" />.</remarks>
  public sealed class NotAccessibleBusinessObjectPropertyPathResult : IBusinessObjectPropertyPathResult
  {
    private readonly IBusinessObjectProvider _businessObjectProvider;

    public NotAccessibleBusinessObjectPropertyPathResult (IBusinessObjectProvider businessObjectProvider)
    {
      _businessObjectProvider = businessObjectProvider;
    }

    public object GetValue ()
    {
      return null;
    }

    public string GetString (string format)
    {
      return _businessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder();
    }

    public IBusinessObjectProperty ResultProperty
    {
      get { return null; }
    }

    public IBusinessObject ResultObject
    {
      get { return null; }
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}