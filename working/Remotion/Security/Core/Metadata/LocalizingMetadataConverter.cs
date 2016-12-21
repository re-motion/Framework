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
using System.Globalization;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class LocalizingMetadataConverter : IMetadataConverter
  {
    private delegate LocalizedName CreateLocalizedName<T> (T item, string text);

    private CultureInfo[] _cultures;
    private IMetadataConverter _metadataConverter;
    private IMetadataLocalizationConverter _localizationConverter;

    public LocalizingMetadataConverter (IMetadataLocalizationConverter localizationConverter, CultureInfo[] cultures)
    {
      ArgumentUtility.CheckNotNull ("localizationConverter", localizationConverter);
      ArgumentUtility.CheckNotNullOrItemsNull ("cultures", cultures);

      _localizationConverter = localizationConverter;
      _cultures = cultures;
    }

    public IMetadataConverter MetadataConverter
    {
      get { return _metadataConverter; }
      set { _metadataConverter = value; }
    }

    public CultureInfo[] Cultures
    {
      get { return _cultures; }
    }

    public void ConvertAndSave (MetadataCache cache, string filename)
    {
      if (_metadataConverter != null)
        _metadataConverter.ConvertAndSave (cache, filename);

      foreach (CultureInfo culture in _cultures)
        _localizationConverter.ConvertAndSave (GetLocalizedNames (cache, culture), culture, filename);
    }

    private LocalizedName[] GetLocalizedNames (MetadataCache cache, CultureInfo culture)
    {
      List<LocalizedName> localizedNames = new List<LocalizedName> ();

      AddNames (localizedNames, cache.GetSecurableClassInfos (), CreateLocalizedNameFromClassInfo);
      AddNames (localizedNames, cache.GetAbstractRoles (), CreateLocalizedNameFromEnumValueInfo);
      AddNames (localizedNames, cache.GetAccessTypes (), CreateLocalizedNameFromEnumValueInfo);
      AddStateNames (localizedNames, cache.GetStatePropertyInfos ());

      return localizedNames.ToArray ();
    }

    private void AddNames<T> (List<LocalizedName> localizedNames, List<T> items, CreateLocalizedName<T> createLocalizedNameDelegate) where T : MetadataInfo
    {
      foreach (T item in items)
        localizedNames.Add (createLocalizedNameDelegate (item, string.Empty));
    }

    private LocalizedName CreateLocalizedNameFromClassInfo (SecurableClassInfo classInfo, string text)
    {
      return new LocalizedName (classInfo.ID, classInfo.Name, classInfo.Description);
    }

    private LocalizedName CreateLocalizedNameFromEnumValueInfo (EnumValueInfo enumValueInfo, string text)
    {
      EnumWrapper enumWrapper = EnumWrapper.Get(enumValueInfo.Name, enumValueInfo.TypeName);
      return new LocalizedName (enumValueInfo.ID, enumWrapper.ToString (), enumValueInfo.Name);
    }

    private LocalizedName CreateLocalizedNameFromStatePropertyInfo (StatePropertyInfo propertyInfo, string text)
    {
      return new LocalizedName (propertyInfo.ID, propertyInfo.Name, propertyInfo.Description);
    }

    private LocalizedName CreateLocalizedNameForState (StatePropertyInfo property, EnumValueInfo state, string text)
    {
      string description = property.Name + "|" + state.Name;
      return new LocalizedName (property.ID + "|" + state.Value, property.Name + "|" + state.Name, description);
    }

    private void AddStateNames (List<LocalizedName> localizedNames, List<StatePropertyInfo> properties)
    {
      foreach (StatePropertyInfo property in properties)
      {
        localizedNames.Add (CreateLocalizedNameFromStatePropertyInfo (property, string.Empty));

        foreach (EnumValueInfo stateInfo in property.Values)
          localizedNames.Add (CreateLocalizedNameForState (property, stateInfo, string.Empty));
      }
    }
  }
}
