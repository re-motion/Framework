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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  /// <summary>
  /// <see cref="UserSubstitedByListEditableRowControlFactory"/> overrides <see cref="EditableRowAutoCompleteControlFactory"/> 
  /// and provides special logic for editing the <b>SubstitutedBy</b> <see cref="BocList"/> on the <see cref="EditUserControl"/>.
  /// </summary>
  /// <remarks>
  /// The <see cref="UserSubstitedByListEditableRowControlFactory"/> instance is retrieved form the <see cref="IServiceLocator"/> using the type
  /// <see cref="UserSubstitedByListEditableRowControlFactory"/> as key.
  /// </remarks>
  [ImplementationFor(typeof(UserSubstitedByListEditableRowControlFactory), Lifetime = LifetimeKind.Singleton)]
  public class UserSubstitedByListEditableRowControlFactory : EditableRowAutoCompleteControlFactory
  {
    public UserSubstitedByListEditableRowControlFactory ()
    {
    }

    protected override IBusinessObjectBoundEditableWebControl? CreateFromPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull("propertyPath", propertyPath);

      if (propertyPath.Identifier == "SubstitutedRole")
        return CreateControlForSubstitutedRole(propertyPath);
      else
        return base.CreateFromPropertyPath(propertyPath);
    }

    protected virtual BocReferenceValue CreateBocReferenceValue (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull("propertyPath", propertyPath);

      return new BocReferenceValue();
    }

    private IBusinessObjectBoundEditableWebControl CreateControlForSubstitutedRole (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull("propertyPath", propertyPath);

      var control = CreateBocReferenceValue(propertyPath);
      control.PreRender += HandleSubstitutedRolePreRender;
      control.EnableSelectStatement = false;
      return control;
    }

    private void HandleSubstitutedRolePreRender (object? sender, EventArgs e)
    {
      var substitutedRoleReferenceValue = ArgumentUtility.CheckNotNullAndType<BocReferenceValue>("sender", sender!);

      Assertion.IsNotNull(substitutedRoleReferenceValue.DataSource, "BocReferenceValue{{{0}}}.DataSource != null", substitutedRoleReferenceValue.ID);
      Assertion.IsNotNull(substitutedRoleReferenceValue.DataSource.BusinessObject, "BocReferenceValue{{{0}}}.DataSource.BusinessObject != null", substitutedRoleReferenceValue.ID);
      Assertion.IsNotNull(substitutedRoleReferenceValue.Property, "BocReferenceValue{{{0}}}.Property != null", substitutedRoleReferenceValue.ID);

      var substitution = substitutedRoleReferenceValue.DataSource.BusinessObject;
      var roles = substitutedRoleReferenceValue.Property.SearchAvailableObjects(substitution, new DefaultSearchArguments(null));
      substitutedRoleReferenceValue.SetBusinessObjectList(roles);
    }
  }
}
