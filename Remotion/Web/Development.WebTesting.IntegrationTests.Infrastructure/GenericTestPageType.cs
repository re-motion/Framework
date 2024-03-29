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

// ReSharper disable once CheckNamespace

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Defines which element should be displayed on the generic test page.
  /// </summary>
  [Flags]
  public enum GenericTestPageType
  {
    Default = HiddenElements | VisibleElements | AmbiguousElements | DisabledElements,
    EnabledDisabled = VisibleElements | DisabledElements,
    EnabledReadOnly = VisibleElements | ReadOnlyElements,
    EnabledFormGrid = VisibleElements | FormGridElements,
    EnabledValidation = VisibleElements | ValidationElements | ReadOnlyElements,
    NonAmbiguous = HiddenElements | VisibleElements,

    /// <summary>
    /// Renders the hidden elements.
    /// </summary>
    HiddenElements = 1,

    /// <summary>
    /// Renders the visible elements.
    /// </summary>
    VisibleElements = 2,

    /// <summary>
    /// Renders the ambiguous elements.
    /// </summary>
    AmbiguousElements = 4,

    /// <summary>
    /// Renders the disabled elements.
    /// </summary>
    DisabledElements = 8,

    /// <summary>
    /// Renders the readonly elements.
    /// </summary>
    ReadOnlyElements = 16,

    /// <summary>
    /// Renders the form grid elements.
    /// </summary>
    FormGridElements = 32,

    /// <summary>
    /// Renders the validation elements.
    /// </summary>
    ValidationElements = 64,
  }
}
