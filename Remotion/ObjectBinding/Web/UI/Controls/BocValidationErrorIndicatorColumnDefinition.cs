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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// A column definition for displaying a validation error indicator for a row.
  /// </summary>
  public class BocValidationErrorIndicatorColumnDefinition : BocColumnDefinition, IBocSortableColumnDefinition
  {
    public BocValidationErrorIndicatorColumnDefinition ()
    {
    }

    /// <inheritdoc />
    protected override IBocColumnRenderer GetRendererInternal (IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull("serviceLocator", serviceLocator);

      return serviceLocator.GetInstance<IBocValidationErrorIndicatorColumnRenderer>();
    }

    /// <inheritdoc />
    protected override string DisplayedTypeName => "ValidationErrorIndicatorColumnDefinition";

    /// <summary> Gets or sets a flag that determines whether to enable sorting for this column. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("A flag determining whether to enable sorting for this column.")]
    [DefaultValue(true)]
    [NotifyParentProperty(true)]
    public bool IsSortable { get; set; } = true;

    public IComparer<BocListRow> CreateCellValueComparer ()
    {
      Assertion.IsTrue(OwnerControl is IBocList);
      var validationFailureRepository = ((IBocList)OwnerControl).ValidationFailureRepository;

      return new BocListRowWithValidationFailureComparer(validationFailureRepository);
    }
  }
}
