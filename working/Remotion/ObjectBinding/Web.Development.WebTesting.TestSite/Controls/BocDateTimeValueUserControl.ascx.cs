﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocDateTimeValueUserControl : DataEditUserControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput();
    }

    private void SetTestOutput ()
    {
      TestOutput.SetCurrentValueNormal (DateOfBirthField_Normal.Value != null ? DateOfBirthField_Normal.Value.Value.ToString() : "invalid");
      TestOutput.SetCurrentValueNoAutoPostBack (
          DateOfBirthField_NoAutoPostBack.Value != null ? DateOfBirthField_NoAutoPostBack.Value.Value.ToString() : "invalid");
      TestOutput.SetCurrentValueDateOnly (DateOfBirthField_DateOnly.Value != null ? DateOfBirthField_DateOnly.Value.Value.ToString() : "invalid");
      TestOutput.SetCurrentValueWithSeconds (
          DateOfBirthField_WithSeconds.Value != null ? DateOfBirthField_WithSeconds.Value.Value.ToString() : "invalid");
    }

    private BocDateTimeValueUserControlTestOutput TestOutput
    {
      get { return (BocDateTimeValueUserControlTestOutput) ((Layout) Page.Master).GetTestOutputControl(); }
    }
  }
}