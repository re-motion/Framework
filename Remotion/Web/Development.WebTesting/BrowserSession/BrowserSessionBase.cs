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
using System.Diagnostics;
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;

namespace Remotion.Web.Development.WebTesting.BrowserSession
{
  /// <summary>
  /// Wraps around <see cref="Coypu.BrowserSession"/> to handle browser specific routines.
  /// </summary>
  public abstract class BrowserSessionBase<T> : IBrowserSession
      where T : IBrowserConfiguration
  {
    private readonly TimeSpan _browserProcessesShutdownTime = TimeSpan.FromSeconds(60);

    private readonly T _browserConfiguration;
    private readonly Coypu.BrowserSession _value;
    private readonly int _driverProcessID;
    private readonly bool _headless;
    private bool _isDisposed;

    protected BrowserSessionBase (
        [NotNull] Coypu.BrowserSession value,
        [NotNull] T browserConfiguration,
        int driverProcessId,
        bool headless)
    {
      ArgumentUtility.CheckNotNull("value", value);
      ArgumentUtility.CheckNotNull("browserConfiguration", browserConfiguration);

      if (driverProcessId < 0)
        throw new ArgumentOutOfRangeException("driverProcessId", "Process id can not be smaller that zero.");

      _value = value;
      _browserConfiguration = browserConfiguration;
      _driverProcessID = driverProcessId;
      _headless = headless;
    }

    /// <inheritdoc />
    public abstract IReadOnlyCollection<BrowserLogEntry> GetBrowserLogs ();

    /// <summary>
    /// Returns the <see cref="IBrowserConfiguration"/> associated with the underlying <see cref="Coypu.BrowserSession"/>.
    /// </summary>
    protected T BrowserConfiguration
    {
      get { return _browserConfiguration; }
    }

    public void AcceptModalDialog (Options? options = null)
    {
      _value.AcceptModalDialog(options);
    }

    public IDriver Driver
    {
      get { return _value.Driver; }
    }

    public BrowserWindow Window
    {
      get { return _value; }
    }

    public bool Headless
    {
      get { return _headless; }
    }

    public BrowserWindow FindWindow (string locator, Options? options = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty("locator", locator);

      return _value.FindWindow(locator, options);
    }

    public virtual void DeleteAllCookies ()
    {
      var webDriver = (IWebDriver)_value.Driver.Native;
      webDriver.Manage().Cookies.DeleteAllCookies();
    }

    /// <summary>
    /// Disposes the underlying <see cref="Coypu.BrowserSession"/> object and makes sure neither driver nor browser are running.
    /// </summary>
    public virtual void Dispose ()
    {
      if (_isDisposed)
        return;

      _isDisposed = true;

      // Get processes for driver and main browser, as well as the sub processes of the browser
      var driverProcess = FindDriverProcess();
      var browserProcess = FindBrowserProcess();

      var processesToClose = new List<Process>();
      if (driverProcess != null)
        processesToClose.Add(driverProcess);
      if (browserProcess != null)
        processesToClose.Add(browserProcess);
      if (browserProcess != null)
        processesToClose.AddRange(FindSubProcesses(browserProcess));

      // Dispose the underlying BrowserSession
      _value.Dispose();

      //ProcessUtils.GracefulProcessShutdown(processesToClose, _browserProcessesShutdownTime);
    }

    /// <summary>
    /// Returns a <see cref="Process"/> representing the driver process.
    /// </summary>
    [CanBeNull]
    private Process? FindDriverProcess ()
    {
      var processes = Process.GetProcesses();

      return processes.FirstOrDefault(p => p.Id == _driverProcessID);
    }

    /// <summary>
    /// Returns a <see cref="Process"/> representing the main browser process.
    /// </summary>
    [CanBeNull]
    private Process? FindBrowserProcess ()
    {
      var processes = Process.GetProcesses();

      return processes.FirstOrDefault(
          p => p.ProcessName == _browserConfiguration.BrowserExecutableName
               && ProcessUtils.GetParentProcessID(p) == _driverProcessID);
    }

    /// <summary>
    /// Returns a <see cref="IEnumerable{Process}"/> representing all sub processes of a process.
    /// </summary>
    private IEnumerable<Process> FindSubProcesses (Process process)
    {
      var processes = Process.GetProcesses();

      return processes.Where(p => p.ProcessName == _browserConfiguration.BrowserExecutableName && ProcessUtils.GetParentProcessID(p) == process.Id);
    }
  }
}
