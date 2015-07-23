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
using System.Linq;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class ExtensibleEnumerationProperty : PropertyBase, IBusinessObjectEnumerationProperty
  {
    private readonly IExtensibleEnumDefinition _definition;
    private readonly IEnumerationValueFilter _enumerationValueFilter;

    public ExtensibleEnumerationProperty (Parameters parameters)
        : base(parameters)
    {
      _definition = ExtensibleEnumUtility.GetDefinition (PropertyType);

      var filterProvider = new EnumValueFilterProvider<DisableExtensibleEnumValuesAttribute> (
          PropertyInfo,
          t => _definition.GetCustomAttributes<DisableExtensibleEnumValuesAttribute> ());
      _enumerationValueFilter = filterProvider.GetEnumerationValueFilter ();
    }

    public IEnumerationValueInfo[] GetAllValues (IBusinessObject businessObject)
    {
      return _definition.GetValueInfos ()
          .Select (info => CreateEnumerationValueInfo (info, businessObject))
          .ToArray();
    }

    public IEnumerationValueInfo[] GetEnabledValues (IBusinessObject businessObject)
    {
      return _definition.GetValueInfos ()
          .Select (info => CreateEnumerationValueInfo (info, businessObject))
          .Where (valueInfo => valueInfo.IsEnabled)
          .ToArray();
    }

    public IEnumerationValueInfo GetValueInfoByValue (object value, IBusinessObject businessObject)
    {
      var enumValue = value as IExtensibleEnum;
      if (enumValue == null)
        return null;

      IExtensibleEnumInfo extensibleEnumInfo;
      if (!_definition.TryGetValueInfoByID (enumValue.ID, out extensibleEnumInfo))
        return null;

      return CreateEnumerationValueInfo (extensibleEnumInfo, businessObject);
    }

    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier, IBusinessObject businessObject)
    {
      if (string.IsNullOrEmpty (identifier))
        return null;

      IExtensibleEnumInfo extensibleEnumInfo;
      if (!_definition.TryGetValueInfoByID (identifier, out extensibleEnumInfo))
      {
        var message = string.Format ("The identifier '{0}' does not identify a defined value for type '{1}'.", identifier, _definition.GetEnumType());
        throw new ArgumentException (message, "identifier");
      }

      return CreateEnumerationValueInfo (extensibleEnumInfo, businessObject);
    }

    public EnumerationValueInfo CreateEnumerationValueInfo (IExtensibleEnumInfo extensibleEnumInfo, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumInfo", extensibleEnumInfo);
      
      return new EnumerationValueInfo (
          extensibleEnumInfo.Value, 
          extensibleEnumInfo.Value.ID, 
          GetDisplayName (extensibleEnumInfo), 
          IsEnabled (extensibleEnumInfo.Value, businessObject));
    }

    private string GetDisplayName (IExtensibleEnumInfo extensibleEnumInfo)
    {
      return BindableObjectGlobalizationService.GetExtensibleEnumerationValueDisplayName (extensibleEnumInfo.Value);
    }

    private bool IsEnabled (IExtensibleEnum value, IBusinessObject businessObject)
    {
      return _enumerationValueFilter.IsEnabled (
          new EnumerationValueInfo (value, value.ID, null, true),
          businessObject,
          this);
    }
  }
}