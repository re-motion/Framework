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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: Doc
  public class BindableObjectDataSource : BusinessObjectDataSource
  {
    private IBusinessObject? _businessObject;
    private string? _typeName;
    private DataSourceMode _mode = DataSourceMode.Edit;
    private BindableObjectClass? _bindableObjectClass;

    public BindableObjectDataSource ()
    {
    }

    public override IBusinessObject? BusinessObject
    {
      get { return _businessObject; }
      set { _businessObject = value; }
    }

    public override DataSourceMode Mode
    {
      get { return _mode; }
      set { _mode = value; }
    }

    public override IBusinessObjectClass? BusinessObjectClass
    {
      get { return GetBindableObjectClass(); }
    }

    [Category("Data")]
    [DefaultValue(null)]
    [TypeConverter(typeof(TypeNameConverter))]
    public Type? Type
    {
      get
      {
        var bindableObjectClass = GetBindableObjectClass();
        if (bindableObjectClass == null)
          return null;

        return bindableObjectClass.TargetType;
      }
      set
      {
        if (value == null)
          _typeName = null;
        else
          _typeName = TypeUtility.GetPartialAssemblyQualifiedName(value);

        _bindableObjectClass = null;
      }
    }

    private new Type? GetType ()
    {
      if (_typeName == null)
        return null;

      return TypeUtility.GetType(_typeName, true);
    }

    private BindableObjectClass? GetBindableObjectClass ()
    {
      var type = GetType();
      if (type == null)
        return null;

      if (_bindableObjectClass == null)
      {
        var provider = BindableObjectProvider.GetProviderForBindableObjectType(type);
        _bindableObjectClass = provider.GetBindableObjectClass(type);
      }

      return _bindableObjectClass;
    }

    private bool IsDesignMode
    {
      get { return Site != null && Site.DesignMode; }
    }
  }
}
