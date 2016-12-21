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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

  /// <summary>
  ///   Encapsulates the functionality required for configuring business object binding in an
  ///   <see cref="IBusinessObjectBoundWebControl"/>.
  /// </summary>
  public class BusinessObjectBinding
  {
    private readonly IBusinessObjectBoundWebControl _control;

    private IBusinessObjectDataSource _dataSource;
    private string _dataSourceControl;
    private bool _dataSourceChanged = false;

    private IBusinessObjectProperty _property;
    private string _propertyIdentifier;
    private bool _bindingChanged = false;
    /// <summary>
    ///   Set after the <see cref="DataSource"/> returned a value for the first time
    ///   in the <c>get accessor</c> of <see cref="Property"/>.
    /// </summary>
    private bool _isDesignModePropertyInitalized = false;
    /// <summary>
    ///   Set in the <c>get accessor</c> of <see cref="Property"/> when <see cref="_dataSourceChanged"/> is set.
    ///   Reset after the <see cref="Property"/> is bound.
    /// </summary>
    private bool _hasDesignModePropertyChanged = false;

    public BusinessObjectBinding (IBusinessObjectBoundWebControl control)
    {
      _control = control;
    }

    /// <summary> The <see cref="IBusinessObjectBoundWebControl"/> whose binding this instance encapsulates. </summary>
    public IBusinessObjectBoundWebControl Control
    {
      get { return _control; }
    }

    /// <summary> 
    ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> for this <see cref="BusinessObjectBinding"/>.
    /// </summary>
    /// <remarks>
    ///   Unless an <b>DataSource</b> is set, <see cref="DataSourceControl"/> is used to identify the data source.
    /// </remarks>
    /// <exception cref="ArgumentException"> Thrown if an attempt is made to set a self reference. </exception>
    public virtual IBusinessObjectDataSource DataSource
    {
      get
      {
        EnsureDataSource ();
        return _dataSource;
      }
      set
      {
        SetDataSource (value);

        Control dataSourceControl = value as Control;
        if (dataSourceControl != null)
          _dataSourceControl = dataSourceControl.ID;
        else
          _dataSourceControl = null;

        _dataSourceChanged = false;
      }
    }

    /// <summary> Uses the value of <see cref="DataSourceControl"/> to set <see cref="DataSource"/>. </summary>
    /// <remarks> 
    ///   Due to <b>Design Mode</b> behavior of Visual Studio .NET the <see cref="IComponent.Site"/> property is
    ///   utilized for finding the data source during <b>Design Mode</b>.
    /// </remarks>
    /// <exception cref="HttpException"> 
    ///   Thrown if the <see cref="DataSourceControl"/> is not <see langword="null "/> and could not be evaluated 
    ///   to a valid <see cref="IBusinessObjectDataSourceControl"/> during <b>Run Time</b>.
    /// </exception>
    public void EnsureDataSource ()
    {
      if (_dataSourceChanged)
      {
        // set _dataSource from ID in _dataSourceControl
        if (string.IsNullOrEmpty (_dataSourceControl))
        {
          SetDataSource (null);
        }
        else
        {
          bool isDesignMode = ControlHelper.IsDesignMode (_control);

          Control namingContainer = _control.NamingContainer;
          if (namingContainer == null)
          {
            if (!isDesignMode)
              throw new HttpException (string.Format ("Cannot evaluate data source because control {0} has no naming container.", _control.ID));

            //  HACK: Designmode Naming container
            //  Not completely sure that Components[0] will always be the naming container.
            if (_control.Site.Container.Components.Count > 0)
              namingContainer = (_control.Site.Container.Components[0]) as Control;
            else
              return;
          }

          Control control = ControlHelper.FindControl (namingContainer, _dataSourceControl);
          if (control == null)
          {
            if (!isDesignMode)
              throw new HttpException (string.Format ("Unable to find control id '{0}' referenced by the DataSourceControl property of '{1}'.", _dataSourceControl, _control.ID));

            foreach (IComponent component in namingContainer.Site.Container.Components)
            {
              if (component is IBusinessObjectDataSourceControl
                  && component is Control
                  && ((Control) component).ID == _dataSourceControl)
              {
                control = (Control) component;
                break;
              }
            }

            if (control == null)
            {
              SetDataSource (null);
              _dataSourceChanged = true;
              return;
            }
          }

          IBusinessObjectDataSourceControl dataSource = control as IBusinessObjectDataSourceControl;
          if (dataSource == null)
            throw new HttpException (string.Format ("The control with the id '{0}' referenced by the DataSourceControl property of '{1}' does not identify a control of type '{2}'.", _dataSourceControl, _control.ID, typeof (IBusinessObjectDataSourceControl)));

          SetDataSource (dataSource);
        }

        _dataSourceChanged = false;
      }
    }

    public void UnregisterDataSource()
    {
      if (_dataSource != null)
      {
        SetDataSource (null);
        _dataSourceChanged = true;
      }
    }

    /// <summary> Sets the new value of the <see cref="DataSource"/> property. </summary>
    /// <param name="dataSource"> The new <see cref="IBusinessObjectDataSource"/>. Can be <see langword="null"/>. </param>
    private void SetDataSource (IBusinessObjectDataSource dataSource)
    {
      if (_control == dataSource && _control is IBusinessObjectReferenceDataSource)
        throw new ArgumentException ("Assigning a reference data source as its own data source is not allowed.", "value");

      if (_dataSource == dataSource)
        return;

      if (_dataSource != null)
        _dataSource.Unregister (_control);

      _dataSource = dataSource;

      if (dataSource != null)
        dataSource.Register (_control);
      _bindingChanged = true;
    }

    /// <summary> The <b>ID</b> of the <see cref="DataSource"/>. </summary>
    /// <value> A string or <see langword="null"/> if no <see cref="DataSource"/> is set. </value>
    /// <exception cref="ArgumentException"> Thrown if an attempt is made to set a self reference. </exception>
    public string DataSourceControl
    {
      get { return _dataSourceControl; }

      set
      {
        if (_control.ID != null && _control.ID == value && _control is IBusinessObjectReferenceDataSource)
          throw new ArgumentException ("Assigning a reference data source as its own data source is not allowed.", "value");
        if (_dataSourceControl != value)
        {
          _dataSourceControl = value;
          _dataSourceChanged = true;
        }
      }
    }

    /// <summary> Gets or sets the <see cref="IBusinessObjectProperty"/> used in the business object binding. </summary>
    /// <remarks>
    ///   Unless an <b>Property</b> is set, <see cref="PropertyIdentifier"/> and <see cref="DataSource"/> are used to 
    ///   identify the property.
    /// </remarks>
    /// <exception cref="ArgumentException"> Thrown if the <see cref="Control"/> does not support the <b>Property</b>. </exception>
    /// <exception cref="InvalidOperationException"> Thrown if an invalid <b>Property</b> has been specifed by the <see cref="PropertyIdentifier"/>. </exception>
    public IBusinessObjectProperty Property
    {
      get
      {
        if (ControlHelper.IsDesignMode (_control))
        {
          if (!_isDesignModePropertyInitalized && DataSource != null)
            _isDesignModePropertyInitalized = true;
          _hasDesignModePropertyChanged |= _dataSourceChanged;
        }

        // evaluate binding
        if (_bindingChanged || _hasDesignModePropertyChanged && _isDesignModePropertyInitalized)
        {
          if (_property == null
              && DataSource != null
              && DataSource.BusinessObjectClass != null
              && !string.IsNullOrEmpty (_propertyIdentifier))
          {
            IBusinessObjectProperty property =
                DataSource.BusinessObjectClass.GetPropertyDefinition (_propertyIdentifier);
            if (property == null)
            {
              throw new InvalidOperationException (
                  string.Format ("The business object class '{0}' bound to {1} '{2}' via the DataSource " +
                          "does not support the business object property '{3}'.",
                      DataSource.BusinessObjectClass.Identifier,
                      _control.GetType ().Name,
                      _control.ID,
                      _propertyIdentifier));
            }
            if (!_control.SupportsProperty (property))
            {
              throw new InvalidOperationException (
                  string.Format ("{0} '{1}' does not support the business object property '{2}'.",
                      _control.GetType ().Name, _control.ID, _propertyIdentifier));
            }
            _property = property;
          }

          _bindingChanged = false;
          if (_isDesignModePropertyInitalized)
            _hasDesignModePropertyChanged = false;

          OnBindingChanged ();
        }

        return _property;
      }

      set
      {
        if (value != null)
        {
          if (!_control.SupportsProperty (value))
          {
            throw new ArgumentException (
                string.Format ("{0} '{1}' does not support the  business object property '{2}'.",
                    _control.GetType ().Name, _control.ID, value.Identifier),
                "value");
          }
        }

        _property = value;
        _propertyIdentifier = (value == null) ? string.Empty : value.Identifier;
        _bindingChanged = true;
      }
    }

    /// <summary> Gets or sets the string representation of the <see cref="Property"/>. </summary>
    /// <value> 
    ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for
    ///   the <see cref="IBusinessObjectProperty"/>. 
    /// </value>
    public string PropertyIdentifier
    {
      get { return _propertyIdentifier; }

      set
      {
        _propertyIdentifier = value;
        _property = null;
        _bindingChanged = true;
      }
    }

    /// <summary> Executed when the <see cref="Property"/> is assigned a new value. </summary>
    protected void OnBindingChanged ()
    {
      if (BindingChanged != null)
        BindingChanged (this, EventArgs.Empty);
    }

    /// <summary> Raised when the <see cref="Property"/> is assigned a new value. </summary>
    /// <remarks> 
    ///   Register for this event to execute code updating the <see cref="Control"/>'s state for the new binding.
    /// </remarks>
    public event EventHandler BindingChanged;

    /// <summary>Tests whether this <see cref="BusinessObjectBoundWebControl"/> can be bound to the <paramref name="property"/>.</summary>
    /// <param name="property">The <see cref="IBusinessObjectProperty"/> to be tested. Must not be <see langword="null"/>.</param>
    /// <returns>
    ///   <list type="bullet">
    ///     <item><see langword="true"/> if <see cref="IBusinessObjectBoundWebControl.SupportedPropertyInterfaces"/> is null.</item>
    ///     <item><see langword="false"/> if the <see cref="DataSource"/> is in <see cref="DataSourceMode.Search"/> mode.</item>
    ///     <item>Otherwise, <see langword="IsPropertyInterfaceSupported"/> is evaluated and returned as result.</item>
    ///   </list>
    /// </returns>
    public bool SupportsProperty (IBusinessObjectProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      if (_control.SupportedPropertyInterfaces == null)
        return true;

      bool isSearchMode = DataSource != null && DataSource.Mode == DataSourceMode.Search;
      if (!isSearchMode && !_control.SupportsPropertyMultiplicity (property.IsList))
      {
        return false;
      }

      return IsPropertyInterfaceSupported (property, _control.SupportedPropertyInterfaces);
    }

    /// <summary>Tests whether the <paramref name="property"/>'s type is part of the <paramref name="supportedPropertyInterfaces"/> array.</summary>
    /// <param name="property">The <see cref="IBusinessObjectProperty"/> to be tested. Must not be <see langword="null"/>.</param>
    /// <param name="supportedPropertyInterfaces"> 
    ///   The list of interfaces to test the <paramref name="property"/> against. Use <see langword="null"/> if no restrictions are made. Items must 
    ///   not be <see langword="null"/>.
    /// </param>
    /// <returns> 
    ///   <see langword="true"/> if the <paramref name="property"/>'s type is found in the <paramref name="supportedPropertyInterfaces"/> array. 
    /// </returns>
    public bool IsPropertyInterfaceSupported (IBusinessObjectProperty property, Type[] supportedPropertyInterfaces)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      if (supportedPropertyInterfaces == null)
        return true;
      ArgumentUtility.CheckNotNullOrItemsNull ("supportedPropertyInterfaces", supportedPropertyInterfaces);

      bool isSupportedPropertyInterface = false;
      for (int i = 0; i < supportedPropertyInterfaces.Length; i++)
      {
        Type supportedInterface = supportedPropertyInterfaces[i];
        if (supportedInterface.IsAssignableFrom (property.GetType ()))
        {
          isSupportedPropertyInterface = true;
          break;
        }
      }
      return isSupportedPropertyInterface;
    }

    /// <summary>Gets a flag specifying whether the <see cref="IBusinessObjectBoundControl"/> has a valid binding configuration.</summary>
    /// <remarks>
    ///   The configuration is considered invalid if data binding is configured for a property that is not available for the bound class or object.
    /// </remarks>
    /// <value> 
    ///   <list type="bullet">
    ///     <item><see langword="true"/> if the <see cref="DataSource"/> or the <see cref="Property"/> is see langword="null"/>. </item>
    ///     <item>The result of the <see cref="IBusinessObjectProperty.IsAccessible">IBusinessObjectProperty.IsAccessible</see> method.</item>
    ///     <item>Otherwise, <see langword="false"/> is returned.</item>
    ///   </list>
    /// </value>
    [Browsable (false)]
    public bool HasValidBinding
    {
      get
      {
        IBusinessObjectDataSource dataSource = DataSource;
        IBusinessObjectProperty property = Property;
        if (dataSource == null || property == null)
          return true;

        return property.IsAccessible (dataSource.BusinessObject);
      }
    }
  }
}
