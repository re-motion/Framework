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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Implements <see cref="IDataManagementCommand"/> by returning an exception in <see cref="GetAllExceptions"/>, and throwing that exception when 
  /// it is to be executed.
  /// </summary>
  public class ExceptionCommand : IDataManagementCommand
  {
    private readonly Exception _exception;

    public ExceptionCommand (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);
      _exception = exception;
    }

    public Exception Exception
    {
      get { return _exception; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return new[] { _exception };
    }

    public void Begin ()
    {
      throw _exception;
    }

    public void Perform ()
    {
      throw _exception;
    }

    public void End ()
    {
      throw _exception;
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }
  }
}
