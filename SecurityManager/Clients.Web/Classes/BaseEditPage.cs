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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.SecurityManager.Domain;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public abstract class BaseEditPage<T> : BasePage
      where T : BaseSecurityManagerObject, ISupportsGetObject
  {
    private readonly List<DataEditUserControl> _dataEditUserControls = new List<DataEditUserControl>();

    protected new FormFunction<T> CurrentFunction
    {
      get { return (FormFunction<T>)base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      foreach (var control in _dataEditUserControls)
        control.DataSource.BusinessObject = CurrentFunction.CurrentObject;

      //Split to support IFormGridRowProvider
      if (IsPostBack)
        LoadValuesInternal(true);
    }

    protected override void OnLoadComplete (EventArgs e)
    {
      //Split to support IFormGridRowProvider
      if (!IsPostBack)
        LoadValuesInternal(false);

      base.OnLoadComplete(e);
    }

    private void LoadValuesInternal (bool interim)
    {
      foreach (var control in _dataEditUserControls)
        control.LoadValues(interim);
      LoadValues(interim);
    }

    protected virtual void LoadValues (bool interim)
    {
    }

    protected void SaveButton_Click (object sender, EventArgs e)
    {
      Assertion.IsNotNull(ClientTransaction.Current, "ClientTransaction.Current !=  when executing page lifecycle events.");
      bool isValid = true;

      PrepareValidation();

      foreach (DataEditUserControl dataEditUserControl in _dataEditUserControls)
        isValid &= dataEditUserControl.Validate();
      isValid &= ValidatePage();

      if (isValid)
      {
        foreach (DataEditUserControl dataEditUserControl in _dataEditUserControls)
          dataEditUserControl.SaveValues(false);
        SaveValues(false);

        if (ValidatePagePostSaveValues())
        {
          try
          {
            ClientTransaction.Current.Commit();
          }
          catch (Exception ex)
          {
            if (IsValidationErrorException(ex))
            {
              ShowErrors();
              return;
            }
            else
            {
              throw;
            }
          }
          ExecuteNextStep();
        }
        else
        {
          ShowErrors();
        }
      }
      else
      {
        ShowErrors();
      }
    }

    protected virtual bool ValidatePage ()
    {
      return true;
    }

    protected virtual bool IsValidationErrorException ([NotNull] Exception exception)
    {
      return false;
    }

    protected virtual void ShowErrors ()
    {
    }

    protected virtual void SaveValues (bool interim)
    {
    }

    protected virtual bool ValidatePagePostSaveValues ()
    {
      return true;
    }

    protected override object? SaveControlState ()
    {
      foreach (DataEditUserControl control in _dataEditUserControls)
        control.SaveValues(true);

      return base.SaveControlState();
    }

    protected void RegisterDataEditUserControl (DataEditUserControl dataEditUserControl)
    {
      _dataEditUserControls.Add(dataEditUserControl);
    }
  }
}
