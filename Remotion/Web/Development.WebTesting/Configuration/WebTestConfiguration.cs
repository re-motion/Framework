using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  public class WebTestConfiguration : IWebTestConfiguration
  {
#if !NETFRAMEWORK
    internal static IWebTestConfiguration Current
    {
      get { return }

    }
#endif
#if NETFRAMEWORK
    internal static Lazy<WebTestConfigurationSection> Current
    {
      get { return new Lazy<WebTestConfigurationSection>(
          () =>
          {
            var configuration = (WebTestConfigurationSection)ConfigurationManager.GetSection("remotion.webTesting");
            Assertion.IsNotNull(configuration, "Configuration section 'remotion.webTesting' missing.");
            return configuration;
          });}
    }
#endif

    public string Name { get; }
    public string Type { get; }
    public NameValueCollection Parameters { get; }
    public string RootPath { get; }
    public IReadOnlyList<string> Resources { get; }
    public string Browser { get; }
    public TimeSpan SearchTimeout { get; }
    public TimeSpan DownloadStartedTimeout { get; }
    public TimeSpan DownloadUpdatedTimeout { get; }
    public TimeSpan RetryInterval { get; }
    public TimeSpan AsyncJavaScriptTimeout { get; }
    public bool Headless { get; }
    public string WebApplicationRoot { get; }
    public string ScreenshotDirectory { get; }
    public string LogsDirectory { get; }
    public bool CloseBrowserWindowsOnSetUpAndTearDown { get; }
    public bool CleanUpUnmatchedDownloadedFiles { get; }
    public string RequestErrorDetectionStrategy { get; }
  }
}
