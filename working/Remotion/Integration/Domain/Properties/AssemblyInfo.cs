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

using System.Reflection;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Globalization.Mixins;
using Remotion.ObjectBinding.Security;
using Remotion.Validation.Globalization;
using Remotion.Validation.Mixins;

[assembly: AssemblyTitle ("re-motion Integration Library for Application Domains")]
[assembly: AssemblyDescription ("This assembly can be linked to ensure that all domain-layer-related dependencies are integrated in the build output of a project.")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: EnsureGlobalizationSupportForMixins]
[assembly: EnsureValidationSupportForDomainObjects]
[assembly: EnsureValidationSupportForMixins]
[assembly: EnsureValidationSupportForGlobalization]
[assembly: EnsureSecuritySupportForObjectBinding]
