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
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  /// <summary>
  /// Serializable version of the <see cref="ValidationLogData"/> object.
  /// </summary>
  [Serializable]
  public class SerializableValidationLogData
  {
    private readonly int _NumberOfFailures;
    private readonly int _numberOfRulesExecuted;
    private readonly int _numberOfSuccesses;
    private readonly int _numberOfUnexpectedExceptions;
    private readonly int _numberOfWarnings;

    public SerializableValidationLogData (ValidationLogData validationLogData)
    {
      ArgumentUtility.CheckNotNull ("validationLogData", validationLogData);

      _NumberOfFailures = validationLogData.GetNumberOfFailures();
      _numberOfRulesExecuted = validationLogData.GetNumberOfRulesExecuted();
      _numberOfSuccesses = validationLogData.GetNumberOfSuccesses();
      _numberOfUnexpectedExceptions = validationLogData.GetNumberOfUnexpectedExceptions();
      _numberOfWarnings = validationLogData.GetNumberOfWarnings();
    }

    public int NumberOfRulesExecuted
    {
      get { return _numberOfRulesExecuted; }
    }

    public int NumberOfUnexpectedExceptions
    {
      get { return _numberOfUnexpectedExceptions; }
    }

    public int NumberOfFailures
    {
      get { return _NumberOfFailures; }
    }

    public int NumberOfWarnings
    {
      get { return _numberOfWarnings; }
    }

    public int NumberOfSuccesses
    {
      get { return _numberOfSuccesses; }
    }
  }
}