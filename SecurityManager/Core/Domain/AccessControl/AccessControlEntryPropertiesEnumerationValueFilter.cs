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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Implementation of <see cref="IEnumerationValueFilter"/> for the <see cref="AccessControlEntry"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="AccessControlEntry.TenantCondition"/>, <see cref="AccessControlEntry.GroupCondition"/>, 
  /// and <see cref="AccessControlEntry.UserCondition"/> properties via the <see cref="DisableEnumValuesAttribute"/>.
  /// </remarks>
  public class AccessControlEntryPropertiesEnumerationValueFilter : IEnumerationValueFilter
  {
    private readonly IAccessControlSettings _accessControlSettings;

    public AccessControlEntryPropertiesEnumerationValueFilter ()
        : this(SafeServiceLocator.Current.GetInstance<IAccessControlSettings>())
    {
    }

    public AccessControlEntryPropertiesEnumerationValueFilter (IAccessControlSettings accessControlSettings)
    {
      ArgumentUtility.CheckNotNull("accessControlSettings", accessControlSettings);

      _accessControlSettings = accessControlSettings;
    }

    public bool IsEnabled (IEnumerationValueInfo value, IBusinessObject? businessObject, IBusinessObjectEnumerationProperty property)
    {
      ArgumentUtility.CheckNotNull("value", value);
      ArgumentUtility.CheckType<AccessControlEntry>("businessObject", businessObject);
      ArgumentUtility.CheckNotNull("property", property);

      AccessControlEntry? ace = (AccessControlEntry?)businessObject;
      bool isStateful = ace?.AccessControlList is StatefulAccessControlList;

      switch (property.Identifier)
      {
        case "TenantCondition":
          return value.IsEnabled && IsTenantConditionEnabled((TenantCondition)value.Value, isStateful);
        case "GroupCondition":
          return value.IsEnabled && IsGroupConditionEnabled((GroupCondition)value.Value, isStateful);
        case "UserCondition":
          return value.IsEnabled && IsUserConditionEnabled((UserCondition)value.Value, isStateful);
        default:
          throw CreateInvalidOperationException(
              "The property '{0}' is not supported by the '{1}'.",
              property.Identifier,
              typeof(AccessControlEntryPropertiesEnumerationValueFilter).GetFullNameChecked());
      }
    }

    private bool IsTenantConditionEnabled (TenantCondition value, bool isStateful)
    {
      switch (value)
      {
        case TenantCondition.None:
          return true;
        case TenantCondition.OwningTenant:
          return isStateful;
        case TenantCondition.SpecificTenant:
          return true;
        default:
          throw CreateInvalidOperationException("The value '{0}' is not a valid value for 'TenantCondition'.", value);
      }
    }

    private bool IsGroupConditionEnabled (GroupCondition value, bool isStateful)
    {
      switch (value)
      {
        case GroupCondition.None:
          return true;
        case GroupCondition.OwningGroup:
          return isStateful;
        case GroupCondition.SpecificGroup:
          return true;
        case GroupCondition.AnyGroupWithSpecificGroupType:
          return true;
        case GroupCondition.BranchOfOwningGroup:
          return isStateful;
        default:
          throw CreateInvalidOperationException("The value '{0}' is not a valid value for 'GroupCondition'.", value);
      }
    }

    private bool IsUserConditionEnabled (UserCondition value, bool isStateful)
    {
      switch (value)
      {
        case UserCondition.None:
          return true;
        case UserCondition.Owner:
          return isStateful;
        case UserCondition.SpecificUser:
          return !_accessControlSettings.DisableSpecificUser;
        case UserCondition.SpecificPosition:
          return true;
        default:
          throw CreateInvalidOperationException("The value '{0}' is not a valid value for 'UserCondition'.", value);
      }
    }

    private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
    {
      return new InvalidOperationException(string.Format(message, args));
    }
  }
}
