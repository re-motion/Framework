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
using System.Collections;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  public class WxeExceptionHandler
  {
    private bool _catchExceptions;
    private Type[]? _catchExceptionTypes;
    private Exception? _exception;

    public Exception? Exception
    {
      get { return _exception; }
    }

    /// <summary> 
    ///   If this is <see langword="true" />, exceptions are caught and returned in the <see cref="Exception"/> property.
    /// </summary>
    public bool CatchExceptions
    {
      get { return _catchExceptions; }
      set { _catchExceptions = value; }
    }

    /// <summary>
    ///   Sets <see cref="CatchExceptions"/> to <see langword="true" /> and limits the types of exceptions that are caught.
    /// </summary>
    /// <param name="exceptionTypes"> Exceptions of these types or sub classes will be caught, all other
    ///     exceptions will be rethrown. </param>
    public void SetCatchExceptionTypes (params Type[] exceptionTypes)
    {
      _catchExceptions = true;
      _catchExceptionTypes = exceptionTypes;
    }

    /// <summary>
    ///   Joins the passed exceptions types with those already assigned.
    /// </summary>
    /// <param name="exceptionTypes"> 
    ///   Exceptions of these types or sub classes will be caught, all other exceptions will be rethrown. 
    /// </param>
    public void AppendCatchExceptionTypes (params Type[] exceptionTypes)
    {
      if (_catchExceptionTypes != null)
      {
        ArrayList exceptionTypeList = new ArrayList(_catchExceptionTypes);
        for (int idxNewTypes = 0; idxNewTypes < exceptionTypes.Length; idxNewTypes++)
        {
          bool isRegistered = false;
          for (int idxRegisteredTypes = 0; idxRegisteredTypes < _catchExceptionTypes.Length; idxRegisteredTypes++)
          {
            if (_catchExceptionTypes[idxRegisteredTypes] == exceptionTypes[idxNewTypes])
            {
              isRegistered = true;
              break;
            }
          }
          if (!isRegistered)
            exceptionTypeList.Add(exceptionTypes[idxNewTypes]);
        }
        exceptionTypes = (Type[])exceptionTypeList.ToArray(typeof(Type));
      }
      SetCatchExceptionTypes(exceptionTypes);
    }

    public Type[] GetCatchExceptionTypes ()
    {
      if (_catchExceptionTypes == null)
        return new Type[0];
      return (Type[])_catchExceptionTypes.Clone();
    }

    public bool Catch (Exception exception)
    {
      ArgumentUtility.CheckNotNull("exception", exception);

      bool match = false;
      if (_catchExceptions && _catchExceptionTypes != null)
        match = _catchExceptionTypes.Any(exceptionType => exceptionType.IsInstanceOfType(exception));

      if (!_catchExceptions || !match)
        return false;

      _exception = exception;

      return true;
    }
  }
}
