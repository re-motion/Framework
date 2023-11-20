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

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// This exception is thrown when the property interception mechanism cannot be applied to a specific <see cref="DomainObject"/> type
  /// because of problems with that type's declaration.
  /// </summary>
  [Serializable]
  public class NonInterceptableTypeException : DomainObjectException
  {
    private readonly Type _type;

    public NonInterceptableTypeException (string message, Type type)
        : base(message)
    {
      _type = type;
    }

    public NonInterceptableTypeException (string message, Type type, Exception innerException)
      : base(message, innerException)
    {
      _type = type;
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected NonInterceptableTypeException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
      _type = (Type)info.GetValue("_type", typeof(Type))!;
    }

    /// <summary>
    /// The type that cannot be intercepted.
    /// </summary>
    public Type Type
    {
      get { return _type; }
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_type", _type);
    }
  }
}
