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
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocTreeViewImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.Factories;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocTreeViewImplementation.Rendering;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.Legacy
{
  /// <summary>
  /// Provides the service configuration for the legacy rendering support of <b>Remotion.ObjectBinding.Web</b>.
  /// </summary>
  public static class BocLegacyServiceConfigurationService
  {
    public static IEnumerable<ServiceConfigurationEntry> GetConfiguration ()
    {
      yield return new ServiceConfigurationEntry (
          typeof (BocListQuirksModeCssClassDefinition), CreateSingletonImplementationInfo<BocListQuirksModeCssClassDefinition>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocIndexColumnRenderer), CreateSingletonImplementationInfo<BocIndexColumnQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocSelectorColumnRenderer), CreateSingletonImplementationInfo<BocSelectorColumnQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocRowRenderer), CreateSingletonImplementationInfo<BocRowQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocListTableBlockRenderer), CreateSingletonImplementationInfo<BocListTableBlockQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocCommandColumnRenderer), CreateSingletonImplementationInfo<BocCommandColumnQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocCompoundColumnRenderer), CreateSingletonImplementationInfo<BocCompoundColumnQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocCustomColumnRenderer), CreateSingletonImplementationInfo<BocCustomColumnQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocDropDownMenuColumnRenderer), CreateSingletonImplementationInfo<BocDropDownMenuColumnQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocListMenuBlockRenderer), CreateSingletonImplementationInfo<BocListMenuBlockQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocListNavigationBlockRenderer), CreateSingletonImplementationInfo<BocListNavigationBlockQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocListRenderer), CreateSingletonImplementationInfo<BocListQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocRowEditModeColumnRenderer), CreateSingletonImplementationInfo<BocRowEditModeColumnQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocSimpleColumnRenderer), CreateSingletonImplementationInfo<BocSimpleColumnQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocBooleanValueResourceSetFactory), CreateSingletonImplementationInfo<BocBooleanValueQuirksModeResourceSetFactory>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocEnumValueRenderer), CreateSingletonImplementationInfo<BocEnumValueQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocDateTimeValueRenderer), CreateSingletonImplementationInfo<BocDateTimeValueQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocBooleanValueRenderer), CreateSingletonImplementationInfo<BocBooleanValueQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocCheckBoxRenderer), CreateSingletonImplementationInfo<BocCheckBoxQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocTextValueRenderer), CreateSingletonImplementationInfo<BocTextValueQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocMultilineTextValueRenderer), CreateSingletonImplementationInfo<BocMultilineTextValueQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocReferenceValueRenderer), CreateSingletonImplementationInfo<BocReferenceValueQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IBocAutoCompleteReferenceValueRenderer), CreateSingletonImplementationInfo<BocAutoCompleteReferenceValueQuirksModeRenderer> ());

      yield return new ServiceConfigurationEntry (
          typeof (IBocTreeViewRenderer), CreateSingletonImplementationInfo<BocTreeViewQuirksModeRenderer> ());
    }

    private static ServiceImplementationInfo CreateSingletonImplementationInfo<T>() {
      return new ServiceImplementationInfo(typeof(T), LifetimeKind.Singleton);
    }
  }
}