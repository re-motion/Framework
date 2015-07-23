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
using JetBrains.Annotations;
using Remotion.ObjectBinding.Sample;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  [UsedImplicitly]
  public class ControlTestFunction : WxeFunction
  {
    private Person _person;

    public ControlTestFunction ()
        : base (new NoneTransactionMode())
    {
    }

    [WxeParameter (1, false, WxeParameterDirection.In)]
    public string UserControl
    {
      get { return (string) Variables["UserControl"]; }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("UserControl", value);
        Variables["UserControl"] = value;
      }
    }

    public Person Person
    {
      get { return _person; }
    }

    // Steps
    private void Step1 ()
    {
      ExceptionHandler.AppendCatchExceptionTypes (typeof (WxeUserCancelException));
    }

    private void Step2 ()
    {
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();

      var personID = new Guid (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
      _person = Person.GetObject (personID);
    }

    private WxeStep Step3 = new WxePageStep ("ControlTestForm.aspx");

    private void Step4 ()
    {
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();
    }
  }
}