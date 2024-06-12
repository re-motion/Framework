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
  public struct ValidationResultItem : IDefaultValidationResultItem
  {
    private readonly string _ruleName;
    private readonly string _message;

    public ValidationResultItem (string ruleName, string message)
    {
      ArgumentUtility.CheckNotNullOrEmpty("ruleName", ruleName);
      ArgumentUtility.CheckNotNullOrEmpty("message", message);

      _ruleName = ruleName;
      _message = message;
    }

    public string RuleName
    {
      get { return _ruleName; }
    }

    public string Message
    {
      get { return _message; }
    }
  }
}
