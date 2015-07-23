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
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.SecurityManager.Domain;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public abstract class BaseEditPage<T> : BasePage
      where T : BaseSecurityManagerObject, ISupportsGetObject
  {
    private readonly List<DataEditUserControl> _dataEditUserControls = new List<DataEditUserControl>();

    protected new FormFunction<T> CurrentFunction
    {
      get { return (FormFunction<T>) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      foreach (var control in _dataEditUserControls)
        control.DataSource.BusinessObject = CurrentFunction.CurrentObject;

      //Split to support IFormGridRowProvider
      if (IsPostBack)
        LoadValuesInternal (true);
    }

    protected override void OnLoadComplete (EventArgs e)
    {
      //Split to support IFormGridRowProvider
      if (!IsPostBack)
        LoadValuesInternal (false);

      base.OnLoadComplete (e);
    }

    private void LoadValuesInternal (bool interim)
    {
      foreach (var control in _dataEditUserControls)
        control.LoadValues (interim);
      LoadValues (interim);
    }

    protected virtual void LoadValues (bool interim)
    {
    }

    protected void SaveButton_Click (object sender, EventArgs e)
    {
      bool isValid = true;

      PrepareValidation();

      foreach (DataEditUserControl dataEditUserControl in _dataEditUserControls)
        isValid &= dataEditUserControl.Validate();
      isValid &= ValidatePage();

      if (isValid)
      {
        foreach (DataEditUserControl dataEditUserControl in _dataEditUserControls)
          dataEditUserControl.SaveValues (false);
        SaveValues (false);

        if (ValidatePagePostSaveValues())
        {
          ClientTransaction.Current.Commit();
          ExecuteNextStep();
        }
        else
          ShowErrors();
      }
      else
        ShowErrors();
    }

    protected virtual void ShowErrors ()
    {
    }

    protected virtual bool ValidatePage ()
    {
      return true;
    }

    protected virtual void SaveValues (bool interim)
    {
    }

    protected virtual bool ValidatePagePostSaveValues ()
    {
      return true;
    }

    protected override object SaveControlState ()
    {
      foreach (DataEditUserControl control in _dataEditUserControls)
        control.SaveValues (true);

      return base.SaveControlState ();
    }

    protected void RegisterDataEditUserControl (DataEditUserControl dataEditUserControl)
    {
      _dataEditUserControls.Add (dataEditUserControl);
    }
  }
}