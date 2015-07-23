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

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime
{
  /// <summary>
  /// Represents a thread-static scope allowing the <see cref="DomainObject"/> constructor to access an instance of 
  /// <see cref="IObjectInitializationContext"/> for the object currently being initialized. <see cref="Dispose"/> _must_ be called for each instance
  /// constructed, otherwise the <see cref="CurrentObjectInitializationContext"/> property will contain invalid data.
  /// </summary>
  public class ObjectInititalizationContextScope : IDisposable
  {
    [ThreadStatic]
    private static IObjectInitializationContext s_currentObjectInitializationContext;

    private readonly IObjectInitializationContext _previousObjectInitializationContext;

    public static IObjectInitializationContext CurrentObjectInitializationContext
    {
      get { return s_currentObjectInitializationContext; }
    }

    public ObjectInititalizationContextScope (IObjectInitializationContext objectInitializationContext)
    {
      ArgumentUtility.CheckNotNull ("objectInitializationContext", objectInitializationContext);
      
      _previousObjectInitializationContext = s_currentObjectInitializationContext;
      s_currentObjectInitializationContext = objectInitializationContext;
    }

    public void Dispose ()
    {
      s_currentObjectInitializationContext = _previousObjectInitializationContext;
    }
  }
}