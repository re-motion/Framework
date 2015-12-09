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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingValidatorFactoryDecorator<T> : IBocValidatorFactory<T>
      where T : IBusinessObjectBoundEditableWebControl
  {
    private readonly SwitchingValidatorFactoryState _switchingValidatorFactoryState;
    private readonly IBocValidatorFactory<T> _withfluentValidatorFactory;
    private readonly IBocValidatorFactory<T> _withoutFluentValidatorFactory;

    public SwitchingValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<T> withfluentValidatorFactory,
        IBocValidatorFactory<T> withoutFluentValidatorFactory)
    {
      ArgumentUtility.CheckNotNull ("withfluentValidatorFactory", withfluentValidatorFactory);
      ArgumentUtility.CheckNotNull ("withoutFluentValidatorFactory", withoutFluentValidatorFactory);
      ArgumentUtility.CheckNotNull ("switchingValidatorFactoryState", switchingValidatorFactoryState);

      _switchingValidatorFactoryState = switchingValidatorFactoryState;
      _withfluentValidatorFactory = withfluentValidatorFactory;
      _withoutFluentValidatorFactory = withoutFluentValidatorFactory;
    }

    public IEnumerable<BaseValidator> CreateValidators (T control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return _switchingValidatorFactoryState.UseFluentValidatorFactory
          ? _withfluentValidatorFactory.CreateValidators (control, isReadOnly)
          : _withoutFluentValidatorFactory.CreateValidators (control, isReadOnly);
    }
  }
}