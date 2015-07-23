// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Data.DomainObjects;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class LocalizedName : BaseSecurityManagerObject
  {
    public static LocalizedName NewObject (string text, Culture culture, MetadataObject metadataObject)
    {
      return NewObject<LocalizedName> (ParamList.Create (text, culture, metadataObject));
    }

    protected LocalizedName (string text, Culture culture, MetadataObject metadataObject)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("culture", culture);
      ArgumentUtility.CheckNotNull ("metadataObject", metadataObject);

      Text = text;
      Culture = culture;
      MetadataObject = metadataObject;
    }

    [StringProperty (IsNullable = false)]
    public abstract string Text { get; set; }

    [Mandatory]
    public abstract Culture Culture { get; protected set; }

    [DBBidirectionalRelation ("LocalizedNames")]
    [Mandatory]
    public abstract MetadataObject MetadataObject { get; protected set; }
  }
}
