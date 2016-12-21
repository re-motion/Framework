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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  internal static class DomainObjectExtensions
  {
    public static bool IsRelation<TDoaminObject> (this RelationChangedEventArgs args, TDoaminObject domainObject, string shortPropertyName)
        where TDoaminObject : IDomainObject
    {
      ArgumentUtility.CheckNotNull ("args", args);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("shortPropertyName", shortPropertyName);

      var properties = new PropertyIndexer (domainObject);
      var propertyAccessor = properties[typeof (TDoaminObject), shortPropertyName];
      return args.RelationEndPointDefinition == propertyAccessor.PropertyData.RelationEndPointDefinition;
    }
  }
}