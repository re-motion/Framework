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
using Remotion.ServiceLocation;
using Remotion.Web.Infrastructure;
using Remotion.Web.Legacy.Infrastructure;
using Remotion.Web.Legacy.UI.Controls.Rendering;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;
using Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.ListMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.SingleViewImplementation.Rendering;
using Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation.Rendering;
using Remotion.Web.UI.Controls.WebButtonImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTreeViewImplementation.Rendering;

namespace Remotion.Web.Legacy
{
  /// <summary>
  /// Provides the service configuration for the legacy rendering support of <b>Remotion.Web</b>.
  /// </summary>
  public static class LegacyServiceConfigurationService
  {
    public static IEnumerable<ServiceConfigurationEntry> GetConfiguration ()
    {
      yield return new ServiceConfigurationEntry (
          typeof (IClientScriptBehavior), CreateSingletonImplementationInfo<QuirksModeClientScriptBehavior>());

      yield return new ServiceConfigurationEntry (
          typeof (IResourceUrlFactory), CreateSingletonImplementationInfo<Factories.QuirksModeResourceUrlFactory>());

      yield return new ServiceConfigurationEntry (
          typeof (IDatePickerButtonRenderer), CreateSingletonImplementationInfo<DatePickerButtonQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IDatePickerPageRenderer), CreateSingletonImplementationInfo<DatePickerPageQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IDropDownMenuRenderer), CreateSingletonImplementationInfo<DropDownMenuQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IListMenuRenderer), CreateSingletonImplementationInfo<ListMenuQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (ISingleViewRenderer), CreateSingletonImplementationInfo<SingleViewQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (ITabbedMenuRenderer), CreateSingletonImplementationInfo<TabbedMenuQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (ITabbedMultiViewRenderer),CreateSingletonImplementationInfo<TabbedMultiViewQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IWebButtonRenderer), CreateSingletonImplementationInfo<WebButtonQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IWebTabStripRenderer), CreateSingletonImplementationInfo<WebTabStripQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IWebTreeViewRenderer), CreateSingletonImplementationInfo<WebTreeViewQuirksModeRenderer>());

      yield return new ServiceConfigurationEntry (
          typeof (IInfrastructureResourceUrlFactory), CreateSingletonImplementationInfo<QuirksModeInfrastructureResourceUrlFactory>());
    }

    private static ServiceImplementationInfo CreateSingletonImplementationInfo<T> ()
    {
      return new ServiceImplementationInfo (typeof (T), LifetimeKind.Singleton);
    }
  }
}