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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace OBWTest.Design
{

public class DesignTestFunction: WxeFunction
{
  public DesignTestFunction ()
    : base(new NoneTransactionMode())
  {
    ReturnUrl = "StartForm.aspx";
  }

  // steps
  private WxeStep Step1 = new WxePageStep("Design/DesignTestEnumValueForm.aspx");//Enum

  private WxeStep Step2 = new WxePageStep("Design/DesignTestBooleanValueForm.aspx");
  private WxeStep Step3 = new WxePageStep("Design/DesignTestCheckBoxForm.aspx");
  private WxeStep Step4 = new WxePageStep("Design/DesignTestDateTimeValueForm.aspx");
  private WxeStep Step5 = new WxePageStep("Design/DesignTestDateValueForm.aspx");
  private WxeStep Step6 = new WxePageStep("Design/DesignTestEnumValueForm.aspx");
  private WxeStep Step7 = new WxePageStep("Design/DesignTestMultilineTextValueForm.aspx");
  private WxeStep Step8 = new WxePageStep("Design/DesignTestReferenceValueForm.aspx");
  private WxeStep Step9 = new WxePageStep("Design/DesignTestTextValueForm.aspx");
  private WxeStep Step10 = new WxePageStep("Design/DesignTestListForm.aspx");
  private WxeStep Step11 = new WxePageStep("Design/DesignTestTreeViewForm.aspx");
}

}
