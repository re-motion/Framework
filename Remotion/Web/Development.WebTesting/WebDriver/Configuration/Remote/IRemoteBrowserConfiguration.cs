// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using OpenQA.Selenium.Firefox;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Remote;

public interface IRemoteBrowserConfiguration : IBrowserConfiguration
{
  IWebTestRemotingSettings RemotingSettings { get; }
}
