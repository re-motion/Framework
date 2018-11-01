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
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.GenericTestPageParameters;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestPageParameters
{
  /// <summary>
  /// <see cref="IGenericTestPageParameter"/> for <see cref="DomainPropertyControlSelectorTestCaseFactory{TControlSelector,TControl}"/>.
  /// </summary>
  public class DomainPropertyGenericTestPageParameter : GenericTestPageParameterBase
  {
    private const int c_parameterCount = 5;

    private string _correctDomainClass;
    private string _foundControlID;
    private string _hiddenDomainProperty;
    private string _incorrectDomainClass;
    private string _visibleDomainProperty;

    public DomainPropertyGenericTestPageParameter ()
        : base (TestConstants.DomainPropertySelectorID, c_parameterCount)
    {
    }

    /// <summary>
    /// Domain class of the correct control.
    /// </summary>
    public string CorrectDomainClass
    {
      get { return _correctDomainClass; }
    }

    /// <summary>
    /// HTML id of the element with <see cref="VisibleDomainProperty"/> set to <see cref="CorrectDomainClass"/>.
    /// </summary>
    public string FoundControlID
    {
      get { return _foundControlID; }
    }

    /// <summary>
    /// Domain property of the hidden control.
    /// </summary>
    public string HiddenDomainProperty
    {
      get { return _hiddenDomainProperty; }
    }

    /// <summary>
    /// Domain class of the incorrect control.
    /// </summary>
    public string IncorrectDomainClass
    {
      get { return _incorrectDomainClass; }
    }

    /// <summary>
    /// Domain property of the visible control.
    /// </summary>
    public string VisibleDomainProperty
    {
      get { return _visibleDomainProperty; }
    }

    /// <inheritdoc />
    public override void Apply (GenericTestPageParameter data)
    {
      base.Apply (data);

      _visibleDomainProperty = data[0];
      _hiddenDomainProperty = data[1];
      _foundControlID = data[2];
      _correctDomainClass = data[3];
      _incorrectDomainClass = data[4];
    }
  }
}