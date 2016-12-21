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
using System.Runtime.Serialization;
using System.Web;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  [Serializable]
  public sealed class WxeHttpExceptionPreservingException : WxeException
  {
    [CanBeNull]
    public static Exception GetUnwrappedException ([NotNull] Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);
      
      var unwrappedException = exception;
      while (unwrappedException is HttpException || unwrappedException is WxeHttpExceptionPreservingException)
        unwrappedException = unwrappedException.InnerException;
      return unwrappedException;
    }

    public WxeHttpExceptionPreservingException (HttpException exception)
        : base (string.Format ("{0} was thrown.", exception), ArgumentUtility.CheckNotNull("exception", exception))
    {
    }

    private WxeHttpExceptionPreservingException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    public HttpException HttpException
    {
      get { return (HttpException) InnerException; }
    }
  }
}