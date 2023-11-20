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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using Remotion.Development.Web.ResourceHosting;
using Remotion.Logging;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Sample.ReferenceDataSourceTestDomain;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.Configuration;
using Remotion.Web.Infrastructure;

namespace OBWTest
{
  public class Global : HttpApplication
  {
    private const string c_writeReflectionBusinessObjectToDiskAppSettingName = "WriteBusinessObjectsToDisk";

    private WaiConformanceLevel _waiConformanceLevelBackup;
    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    public Global ()
    {
      //  Initialize Logger
      LogManager.GetLogger(typeof(Global));
      InitializeComponent();
    }

    public static bool PreferQuirksModeRendering { get; private set; }

    public XmlReflectionBusinessObjectStorageProvider XmlReflectionBusinessObjectStorageProvider
    {
      get { return XmlReflectionBusinessObjectStorageProvider.Current; }
    }

    protected void Application_Start (Object sender, EventArgs e)
    {
      LogManager.Initialize();

      string objectPath = Server.MapPath("~/objects");
      if (!Directory.Exists(objectPath))
        Directory.CreateDirectory(objectPath);
      var defaultServiceLocator = DefaultServiceLocator.Create();

      //defaultServiceLocator.RegisterSingle<ResourceTheme>(() => new ResourceTheme.NovaGray());

      ServiceLocator.SetLocatorProvider(() => defaultServiceLocator);

      IReflectionBusinessObjectStorageProvider reflectionBusinessObjectStorageProvider;
      var writeReflectionBusinessObjectToDiskSetting = WebConfigurationManager.AppSettings[c_writeReflectionBusinessObjectToDiskAppSettingName];
      if (StringComparer.OrdinalIgnoreCase.Equals(writeReflectionBusinessObjectToDiskSetting, true.ToString()))
      {
        reflectionBusinessObjectStorageProvider = new FileSystemReflectionBusinessObjectStorageProvider(objectPath);
      }
      else
      {
        var httpContextProvider = SafeServiceLocator.Current.GetInstance<IHttpContextProvider>();
        reflectionBusinessObjectStorageProvider = new SessionStateReflectionBusinessObjectStorageProvider(
            httpContextProvider,
            new InMemoryWithFileSystemReadFallbackReflectionBusinessObjectStorageProviderFactory(objectPath));
      }

      var resourceUrlFactory = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>();
      XmlReflectionBusinessObjectStorageProvider provider = new XmlReflectionBusinessObjectStorageProvider(reflectionBusinessObjectStorageProvider);
      XmlReflectionBusinessObjectStorageProvider.SetCurrent(provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService(typeof(IGetObjectService), provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()
                            .AddService(typeof(ISearchAvailableObjectsService), new BindableXmlObjectSearchService());
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()
                            .AddService(typeof(IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService(resourceUrlFactory));

      BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute>().AddService(new ReferenceDataSourceTestDefaultValueService());
      BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute>().AddService(new ReferenceDataSourceTestDeleteObjectService());

      var developmentResourcesExistTestPath = Path.Combine(Server.MapPath("~/"), @"..\Web.ClientScript");
      var isResourceMappingRequired = Directory.Exists(developmentResourcesExistTestPath);
      if (isResourceMappingRequired)
      {
#if DEBUG
        const string configuration = "Debug";
#else
        const string configuration = "Release";
#endif
        _resourceVirtualPathProvider = new ResourceVirtualPathProvider(
            new[]
            {
                new ResourcePathMapping("Remotion.Web/Html", @$"..\..\Web\ClientScript\bin\{configuration}\dist"),
                new ResourcePathMapping("Remotion.Web/Image", @"..\..\Web\Core\res\Image"),
                new ResourcePathMapping("Remotion.Web/Themes", @"..\..\Web\Core\res\Themes"),
                new ResourcePathMapping("Remotion.Web/UI", @"..\..\Web\Core\res\UI"),
                new ResourcePathMapping("Remotion.ObjectBinding.Sample/Image", @"..\..\ObjectBinding\Sample\res\Image"),
                new ResourcePathMapping("Remotion.ObjectBinding.Web/Html", @$"..\..\ObjectBinding\Web.ClientScript\bin\{configuration}\dist"),
                new ResourcePathMapping("Remotion.ObjectBinding.Web/Themes", @"..\..\ObjectBinding\Web\res\Themes"),
                new ResourcePathMapping("Remotion.Web.Preview", @"..\..\Web\Preview\res"),
                new ResourcePathMapping("Remotion.ObjectBinding.Web.Preview", @"..\..\ObjectBinding\Web.Preview\res")
            },
            FileExtensionHandlerMapping.Default);
        _resourceVirtualPathProvider.Register();
      }
      else
      {
        _resourceVirtualPathProvider = new ResourceVirtualPathProvider(new ResourcePathMapping[0], new FileExtensionHandlerMapping[0]);
      }

      //var bundle = new Bundle ("~/bundles/css");
      //foreach (var resourcePathMapping in _resourceVirtualPathProvider.Mappings)
      //  BundleCssFilesRecursively ((ResourceVirtualDirectory) _resourceVirtualPathProvider.GetDirectory(resourcePathMapping.VirtualPath), bundle);

      //BundleTable.Bundles.Add (bundle);
    }

    //private void BundleCssFilesRecursively (ResourceVirtualDirectory virtualDirectory, Bundle bundle)
    //{
    //  foreach (ResourceVirtualDirectory directory in virtualDirectory.Directories)
    //    BundleCssFilesRecursively (directory, bundle);

    //  bundle.IncludeDirectory (virtualDirectory.AppRelativeVirtualPath, "*.css");
    //}

    protected void Session_Start (Object sender, EventArgs e)
    {
    }

    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
      _resourceVirtualPathProvider.HandleBeginRequest();
    }

    protected void Application_AuthenticateRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_PreRequestHandlerExecute (Object sender, EventArgs e)
    {
      _waiConformanceLevelBackup = WebConfiguration.Current.Wcag.ConformanceLevel;

      try
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {
      }
      try
      {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {
      }
    }

    protected void Application_PostRequestHandlerExecute (Object sender, EventArgs e)
    {
      WebConfiguration.Current.Wcag.ConformanceLevel = _waiConformanceLevelBackup;
    }

    protected void Application_EndRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_Error (Object sender, EventArgs e)
    {
    }

    protected void Session_End (Object sender, EventArgs e)
    {
    }

    protected void Application_End (Object sender, EventArgs e)
    {
    }

    #region Web Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
    }

    #endregion
  }
}
