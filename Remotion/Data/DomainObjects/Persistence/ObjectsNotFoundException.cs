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
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Thrown when one or multiple objects can't be found in the underlying data source.
  /// </summary>
  [Serializable]
  public class ObjectsNotFoundException : StorageProviderException
  {
    private static string BuildMessage (IEnumerable<ObjectID> ids)
    {
      return string.Format("Object(s) could not be found: {0}.", string.Join(", ", ids.Select(id => "'" + id + "'")));
    }

    private readonly ObjectID[] _ids;

    public ObjectsNotFoundException (IEnumerable<ObjectID> ids)
      : this(ids, null)
    {
    }

    public ObjectsNotFoundException (IEnumerable<ObjectID> ids, Exception? inner)
        : this(BuildMessage(ArgumentUtility.CheckNotNull("ids", ids)), ids, inner)
    {
    }

    public ObjectsNotFoundException (string message, IEnumerable<ObjectID> ids, Exception? inner)
      : base(message, inner)
    {
      ArgumentUtility.CheckNotNull("ids", ids);

      _ids = ids.ToArray();
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected ObjectsNotFoundException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
      _ids = (ObjectID[])info.GetValue("_ids", typeof(ObjectID[]))!;
    }

    public ReadOnlyCollection<ObjectID> IDs
    {
      get { return Array.AsReadOnly(_ids); }
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);

      info.AddValue("_ids", _ids);
    }
  }
}
