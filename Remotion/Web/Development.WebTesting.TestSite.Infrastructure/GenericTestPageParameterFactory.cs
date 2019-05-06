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

namespace Remotion.Web.Development.WebTesting.TestSite.Infrastructure
{
  public class GenericTestPageParameterFactory
  {
    private TestConstants TestConstants { get; } = new TestConstants();

    public GenericTestPageParameter CreateHtmlIDSelector ()
    {
      return new GenericTestPageParameter (
          TestConstants.HtmlIDSelectorID,
          TestConstants.VisibleHtmlID,
          TestConstants.HiddenHtmlID);
    }

    public GenericTestPageParameter CreateLocalIdSelector ()
    {
      return new GenericTestPageParameter (
          TestConstants.LocalIDSelectorID,
          TestConstants.VisibleControlID,
          TestConstants.HiddenControlID,
          TestConstants.VisibleHtmlID);
    }

    public GenericTestPageParameter CreateFirstSelector ()
    {
      return new GenericTestPageParameter (TestConstants.FirstSelectorID, TestConstants.VisibleHtmlID);
    }

    public GenericTestPageParameter CreateSingleSelector ()
    {
      return new GenericTestPageParameter (TestConstants.SingleSelectorID, TestConstants.VisibleHtmlID);
    }

    public GenericTestPageParameter CreateDisabledTests ()
    {
      return new GenericTestPageParameter (
          TestConstants.DisabledTestsID,
          TestConstants.VisibleHtmlID,
          TestConstants.DisabledHtmlID);
    }

    public GenericTestPageParameter CreateIndexSelector ()
    {
      return new GenericTestPageParameter (
          TestConstants.IndexSelectorID,
          TestConstants.VisibleIndex,
          TestConstants.HiddenIndex,
          TestConstants.VisibleHtmlID);
    }
  }
}