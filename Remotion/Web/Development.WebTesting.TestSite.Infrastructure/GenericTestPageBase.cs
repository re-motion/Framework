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
using System.Text.Json;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Development.WebTesting.TestSite.Infrastructure
{
  public abstract class GenericTestPageBase<TOptions> : WxePage
  {
    private readonly Dictionary<string, IGenericTestPage<TOptions>> _pages = new Dictionary<string, IGenericTestPage<TOptions>>();

    protected GenericTestPageBase ()
    {
    }

    [NotNull]
    protected abstract TOptions AmbiguousControlOptions { get; }

    [NotNull]
    protected abstract Control AmbiguousControlPanel { get; }

    [NotNull]
    protected abstract TOptions DisabledControlOptions { get; }

    [NotNull]
    protected abstract Control DisabledControlPanel { get; }

    [NotNull]
    protected abstract TOptions HiddenControlOptions { get; }

    [NotNull]
    protected abstract Control HiddenControlPanel { get; }

    [NotNull]
    protected abstract Dictionary<string, GenericTestPageParameter> Parameters { get; }

    [NotNull]
    protected abstract TOptions VisibleControlOptions { get; }

    [NotNull]
    protected abstract Control VisibleControlPanel { get; }

    /// <summary>
    /// Renders the specified <paramref name="information"/> onto the web page so that the client can process it.
    /// </summary>
    protected abstract void SetTestInformation ([NotNull] string information);

    /// <summary>
    /// Registers a <paramref name="testPage"/> under a specific <paramref name="key"/>.
    /// </summary>
    protected void Register ([NotNull] string key, [NotNull] IGenericTestPage<TOptions> testPage)
    {
      ArgumentUtility.CheckNotNull("key", key);
      ArgumentUtility.CheckNotNull("testPage", testPage);

      if (_pages.ContainsKey(key))
        throw new InvalidOperationException("A generic test page with that name is already registered.");

      _pages[key] = testPage;
    }

    /// <inheritdoc />
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      var control = Request.Params["control"];
      var type = Request.Params["type"];

      IGenericTestPage<TOptions> testPage;
      if (control == null || !_pages.TryGetValue(control, out testPage))
      {
        SetTestInformation(new GenericTestPageParameterDto(GenericTestPageStatus.Failed, null));
        return;
      }

      int typeInt;
      GenericTestPageType pageType;
      if (type != null && int.TryParse(type, out typeInt))
        pageType = (GenericTestPageType) typeInt;
      else
        pageType = GenericTestPageType.Default;

      AddControlsOnInit(pageType, testPage);

      var parameters = new Dictionary<string, GenericTestPageParameter>(Parameters);
      testPage.AddParameters(parameters, VisibleControlOptions);

      SetTestInformation(new GenericTestPageParameterDto(GenericTestPageStatus.Ok, parameters));
    }

    protected virtual void AddControlsOnInit (GenericTestPageType pageType, IGenericTestPage<TOptions> testPage)
    {
      if (pageType.HasFlag(GenericTestPageType.VisibleElements))
        VisibleControlPanel.Controls.Add(testPage.CreateControl(VisibleControlOptions));

      if (pageType.HasFlag(GenericTestPageType.HiddenElements))
        HiddenControlPanel.Controls.Add(testPage.CreateControl(HiddenControlOptions));

      if (pageType.HasFlag(GenericTestPageType.AmbiguousElements))
        AmbiguousControlPanel.Controls.Add(testPage.CreateControl(AmbiguousControlOptions));

      if (pageType.HasFlag(GenericTestPageType.DisabledElements))
        DisabledControlPanel.Controls.Add(testPage.CreateControl(DisabledControlOptions));
    }

    private void SetTestInformation (GenericTestPageParameterDto information)
    {
      var serializerOptions = new JsonSerializerOptions { Converters = { GenericTestParameterConverter.Instance } };
      var serialized = JsonSerializer.Serialize(information, serializerOptions);
      SetTestInformation(serialized);
    }
  }
}
