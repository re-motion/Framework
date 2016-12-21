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
using System.Globalization;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [SecurityManagerStorageGroup]
  [Serializable]
  public abstract class MetadataObject : BaseSecurityManagerObject
  {
    // types

    // static members and constants

    public static MetadataObject Find (string metadataID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("metadataID", metadataID);

      FindMetadataObjectQueryBuilder queryBuilder = new FindMetadataObjectQueryBuilder();

      var result = queryBuilder.CreateQuery (metadataID);

      return result.ToArray().SingleOrDefault();
    }

    // member fields
    private DomainObjectDeleteHandler _deleteHandler;

    // construction and disposing

    protected MetadataObject ()
    {
    }

    // methods and properties

    public abstract int Index { get; set; }

    public abstract Guid MetadataItemID { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 200)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("MetadataObject")]
    [ObjectBinding (ReadOnly = true)]
    public abstract ObjectList<LocalizedName> LocalizedNames { get; }

    public override string DisplayName
    {
      get
      {
        foreach (CultureInfo cultureInfo in CultureInfo.CurrentUICulture.GetCultureHierarchy())
        {
          LocalizedName localizedName = GetLocalizedName (cultureInfo.Name);
          if (localizedName != null)
            return localizedName.Text;
        }

        return Name;
      }
    }

    public LocalizedName GetLocalizedName (Culture culture)
    {
      ArgumentUtility.CheckNotNull ("culture", culture);

      return GetLocalizedName (culture.CultureName);
    }

    public LocalizedName GetLocalizedName (string cultureName)
    {
      ArgumentUtility.CheckNotNull ("cultureName", cultureName);

      foreach (LocalizedName localizedName in LocalizedNames)
      {
        if (localizedName.Culture.CultureName.Equals (cultureName, StringComparison.Ordinal))
          return localizedName;
      }

      return null;
    }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      //TODO: Rewrite with test
      _deleteHandler = new DomainObjectDeleteHandler (LocalizedNames);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      //TODO: Rewrite with test
      _deleteHandler.Delete();
    }
  }
}