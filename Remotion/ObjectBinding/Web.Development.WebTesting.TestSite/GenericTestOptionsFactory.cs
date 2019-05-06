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

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  /// <summary>
  /// Provides default initializations for common <see cref="GenericTestOptions"/>.
  /// </summary>
  public class GenericTestOptionsFactory
  {
    private ObjectBindingTestConstants TestConstants { get; } = new ObjectBindingTestConstants();
    private string DataSourceID { get; }

    public GenericTestOptionsFactory (string dataSourceID)
    {
      DataSourceID = dataSourceID;
    }

    public GenericTestOptions CreateAmbiguous ()
    {
      return new GenericTestOptions (
          TestConstants.AmbiguousControlID,
          TestConstants.AmbiguousControlHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty);
    }

    public GenericTestOptions CreateFormGrid ()
    {
      return new GenericTestOptions (
          TestConstants.ControlInFormGridID,
          TestConstants.ControlInFormGridHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty);
    }

    public GenericTestOptions CreateFormGridWithReadOnly ()
    {
      return new GenericTestOptions (
          TestConstants.ReadOnlyControlInFormGridID,
          TestConstants.ReadonlyControlInFormGridHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty,
          EnabledState.Enabled,
          ReadOnlyState.ReadOnly);
    }

    public GenericTestOptions CreateOneControlOverMultipleRows ()
    {
      return new GenericTestOptions (
          TestConstants.OneControlOverMultipleRowsFormGridID,
          TestConstants.OneControlOverMultipleRowsFormGridHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty);
    }

    public GenericTestOptions CreateShiftedColumnsFormGrid ()
    {
      return new GenericTestOptions (
          TestConstants.ColumnsShiftedControlFormGridID,
          TestConstants.ColumnsShiftedControlFormGridHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty);
    }

    public GenericTestOptions CreateFormGridMulti1 ()
    {
      return new GenericTestOptions (
          TestConstants.ControlInMultiFormGridID1,
          TestConstants.ControlInMultiFormGridHtmlID1,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty);
    }

    public GenericTestOptions CreateFormGridMulti2 ()
    {
      return new GenericTestOptions (
          TestConstants.ControlInMultiFormGridID2,
          TestConstants.ControlInMultiFormGridHtmlID2,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty);
    }

    public GenericTestOptions CreateDisabled ()
    {
      return new GenericTestOptions (
          TestConstants.DisabledControlID,
          TestConstants.DisabledHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty,
          EnabledState.Disabled);
    }

    public GenericTestOptions CreateReadOnly ()
    {
      return new GenericTestOptions (
          TestConstants.ReadOnlyControlID,
          TestConstants.ReadOnlyControlHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty,
          EnabledState.Enabled,
          ReadOnlyState.ReadOnly);
    }

    public GenericTestOptions CreateHidden ()
    {
      return new GenericTestOptions (
          TestConstants.HiddenControlID,
          TestConstants.HiddenHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty);
    }

    public GenericTestOptions CreateVisible ()
    {
      return new GenericTestOptions (
          TestConstants.VisibleControlID,
          TestConstants.VisibleHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty);
    }

    public GenericTestOptions CreateCustomValidated ()
    {
      return new GenericTestOptions (
          TestConstants.CustomValidatedControlInFormGridID,
          TestConstants.CustomValidatedControlInFormGridHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty);
    }

    public GenericTestOptions CreateMultipleValidated ()
    {
      return new GenericTestOptions (
          TestConstants.MultipleValidatedControlInFormGridID,
          TestConstants.MultipleValidatedControlInFormGridHtmlID,
          DataSourceID,
          TestConstants.CorrectDomainProperty,
          TestConstants.IncorrectDomainProperty);
    }
  }
}