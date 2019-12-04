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
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  /// <summary>
  /// Provides default initializations for common <see cref="GenericTestPageParameter"/>.
  /// </summary>
  public class ObjectBindingGenericTestPageParameterFactory : GenericTestPageParameterFactory
  {
    private ObjectBindingTestConstants TestConstants { get; } = new ObjectBindingTestConstants();

    public GenericTestPageParameter CreateValidationErrorTests ()
    {
      return new GenericTestPageParameter (
          TestConstants.ValidationErrorTestsID,
          TestConstants.ValidateButtonHtmlID,
          TestConstants.CustomValidatedControlInFormGridHtmlID,
          TestConstants.CustomValidatedReadOnlyControlInFormGridHtmlID,
          TestConstants.MultipleValidatedControlInFormGridHtmlID,
          TestConstants.VisibleHtmlID,
          TestConstants.ControlInFormGridHtmlID,
          TestConstants.ReadOnlyControlHtmlID);
    }

    public GenericTestPageParameter CreateLabelTests ()
    {
      return new GenericTestPageParameter (
          TestConstants.LabelTestsID,
          TestConstants.ControlInFormGridHtmlID,
          TestConstants.ReadonlyControlInFormGridHtmlID,
          TestConstants.OneControlOverMultipleRowsFormGridHtmlID,
          TestConstants.ColumnsShiftedControlFormGridHtmlID,
          TestConstants.ControlInMultiFormGridHtmlID1,
          TestConstants.ControlInMultiFormGridHtmlID2,
          TestConstants.VisibleHtmlID);
    }

    public GenericTestPageParameter CreateReadOnlyTests ()
    {
      return new GenericTestPageParameter (
          TestConstants.ReadOnlyTestsID,
          TestConstants.VisibleHtmlID,
          TestConstants.ReadOnlyControlHtmlID);
    }
  }
}