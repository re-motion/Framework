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
  /// Implements <see cref="IBusinessObjectPropertyPathResult"/> for a property path that could not be evaluated up to the <see cref="ResultObject"/>,
  /// e.g. contained a <see langword="null" />-value within its path, a missing property, etc.
  /// </summary>
  /// <remarks><see cref="ResultProperty"/> and <see cref="ResultObject"/> will always be <see langword="null" />.</remarks>
  public sealed class NullBusinessObjectPropertyPathResult : IBusinessObjectPropertyPathResult
  {
    public bool IsNull
    {
      get { return true; }
    }

    public object GetValue ()
    {
      return null;
    }

    public string GetString (string format)
    {
      return string.Empty;
    }

    public IBusinessObjectProperty ResultProperty
    {
      get { return null; }
    }

    public IBusinessObject ResultObject
    {
      get { return null; }
    }
  }
}