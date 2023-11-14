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
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Control that allows a User Control to be bound to a business object data source and property.
  /// </summary>
  public class UserControlBinding : BusinessObjectBoundEditableWebControl, IControlWithResourceManager
  {
    private string _userControlPath = string.Empty;
    private IDataEditControl? _userControl;
    private BusinessObjectReferenceDataSourceControl? _referenceDataSource;
    private ReadOnlyCollection<BaseValidator>? _validators;

    public string UserControlPath
    {
      get { return _userControlPath; }
      set { _userControlPath = value; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IDataEditControl? UserControl
    {
      get { return _userControl; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      TemplateControl control = (TemplateControl)Page!.LoadControl(_userControlPath);
      Controls.Add(control);
      _userControl = control as IDataEditControl;

      if (_userControl != null && DataSource != null)
      {
        IBusinessObjectDataSource dataSourceControl = DataSource;
        if (Property != null)
        {
          _referenceDataSource = new BusinessObjectReferenceDataSourceControl();
          _referenceDataSource.DataSource = DataSource;
          _referenceDataSource.Property = Property;
          _referenceDataSource.Mode = DataSource.Mode;
          dataSourceControl = _referenceDataSource;
          Controls.Add(_referenceDataSource);
        }

        _userControl.Mode = dataSourceControl.Mode;
        _userControl.BusinessObject = dataSourceControl.BusinessObject;
      }
    }

    protected override void OnLoad (EventArgs e)
    {
      Assertion.IsNotNull(DataSource, "DataSource must not be null.");
      Assertion.IsNotNull(_userControl, "_userControl must not be null.");

      base.OnLoad(e);
      if (_referenceDataSource != null)
        _referenceDataSource.Mode = DataSource.Mode;
      _userControl.Mode = DataSource.Mode;
    }


    public override void LoadValue (bool interim)
    {
      Assertion.IsNotNull(_userControl, "_userControl must not be null.");

      if (_referenceDataSource == null)
        throw new NotImplementedException();

      _referenceDataSource.LoadValue(interim);
      _userControl.BusinessObject = _referenceDataSource.BusinessObject;
      _userControl.LoadValues(interim);
    }

    public override bool SaveValue (bool interim)
    {
      Assertion.IsNotNull(_userControl, "_userControl must not be null.");
      Assertion.IsNotNull(_referenceDataSource, "_referenceDataSource must not be null.");

      // Validate to keep things consistent, i.e. all validators have executed during the save-operation.
      // Do not abort the save-operation because the value of the ReferenceDataSource should always be allowed to be written back into the parent.
      Validate();

      _userControl.SaveValues(interim);
      _referenceDataSource.BusinessObject = _userControl.BusinessObject;

      return _referenceDataSource.SaveValue(interim);
    }

    protected override sealed object? ValueImplementation
    {
      get
      {
        if (_referenceDataSource == null)
          throw new InvalidOperationException("Cannot get value if no property is set.");
        return _referenceDataSource.Value;
      }
      set
      {
        if (_referenceDataSource == null)
          throw new InvalidOperationException("Cannot set value if no property is set.");
        _referenceDataSource.Value = value;
      }
    }

    public override bool HasValue
    {
      get
      {
        if (_referenceDataSource == null)
          throw new InvalidOperationException("Cannot analyze value if no property is set.");
        return _referenceDataSource.HasValue;
      }
    }

    public override bool IsDirty
    {
      get
      {
        Assertion.IsNotNull(_userControl, "_userControl must not be null.");

        return _userControl.DataSource.GetBoundControlsWithValidBinding().OfType<IBusinessObjectBoundEditableWebControl>().Any(control => control.IsDirty);
      }
    }

    public override string[] GetTrackedClientIDs ()
    {
      return new string[0];
    }

    public override bool UseLabel
    {
      get { return false; }
    }

    protected override IBusinessObjectConstraintVisitor CreateBusinessObjectConstraintVisitor ()
    {
      return NullBusinessObjectConstraintVisitor.Instance;
    }

    /// <inheritdoc />
    protected override IEnumerable<BaseValidator> CreateValidators (bool isReadOnly)
    {
      var validatorFactory = ServiceLocator.GetInstance<IUserControlBindingValidatorFactory>();
      _validators = validatorFactory.CreateValidators(this, isReadOnly).ToList().AsReadOnly();
      return _validators;
    }

    public override void PrepareValidation ()
    {
      Assertion.IsNotNull(_userControl, "_userControl must not be null.");

      _userControl.PrepareValidation();
    }

    protected override Type[] SupportedPropertyInterfaces
    {
      get { return new Type[] { typeof(IBusinessObjectReferenceProperty) }; }
    }

    public IResourceManager GetResourceManager ()
    {
      return GetResourceManager(typeof(UserControlBinding));
    }
  }
}
