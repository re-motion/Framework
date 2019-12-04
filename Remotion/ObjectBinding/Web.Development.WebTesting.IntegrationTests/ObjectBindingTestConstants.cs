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
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  public class ObjectBindingTestConstants : TestConstants
  {
    public string CorrectDomainProperty => "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample";
    public string IncorrectDomainProperty => "Remotion.ObjectBinding.Sample.Job, Remotion.ObjectBinding.Sample";

    public string ControlInFormGridID => "ControlInFormGrid";
    public string ReadOnlyControlInFormGridID => "ReadOnlyControlInFormGrid";
    public string OneControlOverMultipleRowsFormGridID => "ControlInSecondRowFormGrid";
    public string ColumnsShiftedControlFormGridID => "ColumnsShiftedControlFormGrid";
    public string ControlInMultiFormGridID1 => "ControlInMultiFormGridID1";
    public string ControlInMultiFormGridID2 => "ControlInMultiFormGridID2";
    public string CustomValidatedControlInFormGridID => "CustomValidatedControlInFormGrid";
    public string CustomValidatedReadOnlyControlInFormGridID => "CustomValidatedReadOnlyControlInFormGrid";
    public string MultipleValidatedControlInFormGridID => "MultipleValidatedControlInFormGrid";
    public string ValidateButtonID => "ValidateButton";

    public string AmbiguousControlHtmlID => BodyConstant + AmbiguousControlID;
    public string ReadOnlyControlHtmlID => BodyConstant + ReadOnlyControlID;
    public string ControlInFormGridHtmlID => BodyConstant + ControlInFormGridID;
    public string ReadonlyControlInFormGridHtmlID => BodyConstant + ReadOnlyControlInFormGridID;
    public string OneControlOverMultipleRowsFormGridHtmlID => BodyConstant + OneControlOverMultipleRowsFormGridID;
    public string ColumnsShiftedControlFormGridHtmlID => BodyConstant + ColumnsShiftedControlFormGridID;
    public string ControlInMultiFormGridHtmlID1 => BodyConstant + ControlInMultiFormGridID1;
    public string ControlInMultiFormGridHtmlID2 => BodyConstant + ControlInMultiFormGridID2;
    public string CustomValidatedControlInFormGridHtmlID => BodyConstant + CustomValidatedControlInFormGridID;
    public string CustomValidatedReadOnlyControlInFormGridHtmlID => BodyConstant + CustomValidatedReadOnlyControlInFormGridID;
    public string MultipleValidatedControlInFormGridHtmlID => BodyConstant + MultipleValidatedControlInFormGridID;
    public string ValidateButtonHtmlID => BodyConstant + ValidateButtonID;
  }
}