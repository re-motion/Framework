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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Implements the <see cref="IBocValidatorFactory{T}"/> inteface and compounds all registered <see cref="IBocValidatorFactory{T}"/>.
  /// </summary>
  /// <seealso cref="IBocValidatorFactory{T}"/>
  public class CompoundValidatorFactory<T> : IBocValidatorFactory<T> where T : IBusinessObjectBoundEditableWebControl
  {
    private readonly IReadOnlyCollection<IBocValidatorFactory<T>> _innerFactories;

    public IReadOnlyCollection<IBocValidatorFactory<T>> VlidatorFactories
    {
      get { return _innerFactories; }
    }

    public CompoundValidatorFactory (IEnumerable<IBocValidatorFactory<T>> innerFactories)
    {
      ArgumentUtility.CheckNotNull ("innerFactories", innerFactories);

      _innerFactories = innerFactories.ToList().AsReadOnly();
    }

    public IEnumerable<BaseValidator> CreateValidators (T control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return _innerFactories.SelectMany (i => i.CreateValidators (control, isReadOnly));
    }
  }
}