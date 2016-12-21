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
using Remotion.Validation.Providers;

namespace Remotion.Validation.Attributes
{
  /// <summary>
  /// The <see cref="ApplyProgrammaticallyAttribute"/> can be applied to an <see cref="IComponentValidationCollector"/> to exclude it from the
  /// automatic discovery. Collectors annotated with this attribute must be registered 
  /// via a custom implemenation of the <see cref="IValidationCollectorProvider"/> interface.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class)]
  public class ApplyProgrammaticallyAttribute : Attribute
  {
  }
}