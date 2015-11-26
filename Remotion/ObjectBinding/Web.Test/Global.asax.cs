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
using Microsoft.Practices.ServiceLocation;
using OBWTest.ValidatorFactoryDecorators;
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
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Decorators;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.Configuration;

namespace OBWTest
{
  public class Global : HttpApplication
  {
    private WaiConformanceLevel _waiConformanceLevelBackup;
    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    public Global ()
    {
      //  Initialize Logger
      LogManager.GetLogger (typeof (Global));
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
      
      string objectPath = Server.MapPath ("~/objects");
      if (!Directory.Exists (objectPath))
        Directory.CreateDirectory (objectPath);
      var defaultServiceLocator = DefaultServiceLocator.Create ();

      RegisterSwitchingValidatorFactories(defaultServiceLocator);

      ServiceLocator.SetLocatorProvider (() => defaultServiceLocator);

      XmlReflectionBusinessObjectStorageProvider provider = new XmlReflectionBusinessObjectStorageProvider (objectPath);
      XmlReflectionBusinessObjectStorageProvider.SetCurrent (provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (IGetObjectService), provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()
                            .AddService (typeof (ISearchAvailableObjectsService), new BindableXmlObjectSearchService());
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()
                            .AddService (typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService());

      BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute>().AddService (new ReferenceDataSourceTestDefaultValueService());
      BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute>().AddService (new ReferenceDataSourceTestDeleteObjectService());

      _resourceVirtualPathProvider = new ResourceVirtualPathProvider (
          new[]
          {
              new ResourcePathMapping ("Remotion.Web", @"..\..\Web\Core\res"),
              new ResourcePathMapping ("Remotion.ObjectBinding.Web", @"..\..\ObjectBinding\Web\res"),
              new ResourcePathMapping ("Remotion.Web.Legacy", @"..\..\Web\Legacy\res"),
              new ResourcePathMapping ("Remotion.ObjectBinding.Web.Legacy", @"..\..\ObjectBinding\Web.Legacy\res")
          },
          FileExtensionHandlerMapping.Default);
      _resourceVirtualPathProvider.Register ();

      //var bundle = new Bundle ("~/bundles/css");
      //foreach (var resourcePathMapping in _resourceVirtualPathProvider.Mappings)
      //  BundleCssFilesRecursively ((ResourceVirtualDirectory) _resourceVirtualPathProvider.GetDirectory(resourcePathMapping.VirtualPath), bundle);

      //BundleTable.Bundles.Add (bundle);
    }

    private static void RegisterSwitchingValidatorFactories (DefaultServiceLocator defaultServiceLocator)
    {
      var compoundBocAutoCompleteReferenceValueValidatorFactory =
          new CompoundBocAutoCompleteReferenceValueValidatorFactory (
              new IBocAutoCompleteReferenceValueValidatorFactory[] { new BocAutoCompleteReferenceValueValidatorFactory(), new BocValidatorFactory() });
      var bocAutoCompleteReferenceValueValidatorFactory = new SwitchingBocAutoCompleteReferenceValueValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringBocAutoCompleteReferenceValueValidatorFactoryDecorator (
              compoundBocAutoCompleteReferenceValueValidatorFactory),
          new BocAutoCompleteReferenceValueValidatorFactory());
      defaultServiceLocator.RegisterSingle<IBocAutoCompleteReferenceValueValidatorFactory> (() => bocAutoCompleteReferenceValueValidatorFactory);

      var compoundBocBooleanValueValidatorFactory = new CompoundBocBooleanValueValidatorFactory (
          new IBocBooleanValueValidatorFactory[] { new BocBooleanValueValidatorFactory(), new BocValidatorFactory() });
      var bocBooleanValueValidatorFactory = new SwitchingBocBooleanValueValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringBocBooleanValueValidatorFactoryDecorator (
              compoundBocBooleanValueValidatorFactory), // Fluent
          new BocBooleanValueValidatorFactory ()); // Not Fluent
      defaultServiceLocator.RegisterSingle<IBocBooleanValueValidatorFactory> (() => bocBooleanValueValidatorFactory);

      var compoundBocCheckBoxValidatorFactory = new CompoundBocCheckBoxValidatorFactory (
          new IBocCheckBoxValidatorFactory[] { new BocValidatorFactory() });
      var bocCheckBoxValidatorFactory = new SwitchingBocCheckBoxValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringBocCheckBoxValidatorFactoryDecorator (
              compoundBocCheckBoxValidatorFactory),
          new CompoundValidatorFactory<IBocCheckBox>(Enumerable.Empty<IBocValidatorFactory<IBocCheckBox>>()));
      defaultServiceLocator.RegisterSingle<IBocCheckBoxValidatorFactory> (() => bocCheckBoxValidatorFactory);

      var compoundBocDateTimeValueValidatorFactory = new CompoundBocDateTimeValueValidatorFactory (
          new IBocDateTimeValueValidatorFactory[] { new BocDateTimeValueValidatorFactory(), new BocValidatorFactory() });
      var bocDateTimeValueValidatorFactory = new SwitchingBocDateTimeValueValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringBocDateTimeValueValidatorFactoryDecorator (
              compoundBocDateTimeValueValidatorFactory),
          new BocDateTimeValueValidatorFactory ());
      defaultServiceLocator.RegisterSingle<IBocDateTimeValueValidatorFactory> (() => bocDateTimeValueValidatorFactory);

      var compoundBocEnumValueValidatorFactory = new CompoundBocEnumValueValidatorFactory (
          new IBocEnumValueValidatorFactory[] { new BocEnumValueValidatorFactory(), new BocValidatorFactory() });
      var bocEnumValueValidatorFactory = new SwitchingBocEnumValueValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringBocEnumValueValidatorFactoryDecorator (
              compoundBocEnumValueValidatorFactory),
          new BocEnumValueValidatorFactory ());
      defaultServiceLocator.RegisterSingle<IBocEnumValueValidatorFactory> (() => bocEnumValueValidatorFactory);

      var compoundBocListValidatorFactory = new CompoundBocListValidatorFactory (
          new IBocListValidatorFactory[] { new BocListValidatorFactory(), new BocListValidatorValidatorFactory() });
      var bocListValidatorFactory = new SwitchingBocListValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringBocListValidatorFactoryDecorator (
              compoundBocListValidatorFactory),
          new BocListValidatorFactory ());
      defaultServiceLocator.RegisterSingle<IBocListValidatorFactory> (() => bocListValidatorFactory);

      var compoundBocMultilineTextValueValidatorFactory = new CompoundBocMultilineTextValueValidatorFactory (
          new IBocMultilineTextValueValidatorFactory[] { new BocMultilineTextValueValidatorFactory(), new BocValidatorFactory() });
      var bocMultilineTextValueValidatorFactory = new SwitchingBocMultilineTextValueValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringBocMultilineTextValueValidatorFactoryDecorator (
              compoundBocMultilineTextValueValidatorFactory),
          new BocMultilineTextValueValidatorFactory ());
      defaultServiceLocator.RegisterSingle<IBocMultilineTextValueValidatorFactory> (() => bocMultilineTextValueValidatorFactory);

      var compoundBocReferenceValueValidatorFactory = new CompoundBocReferenceValueValidatorFactory (
          new IBocReferenceValueValidatorFactory[] { new BocReferenceValueValidatorFactory(), new BocValidatorFactory() });
      var bocReferenceValueValidatorFactory = new SwitchingBocReferenceValueValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringBocReferenceValueValidatorFactoryDecorator (
              compoundBocReferenceValueValidatorFactory),
          new BocReferenceValueValidatorFactory ());
      defaultServiceLocator.RegisterSingle<IBocReferenceValueValidatorFactory> (() => bocReferenceValueValidatorFactory);

      var compoundBocTextValueValidatorFactory = new CompoundBocTextValueValidatorFactory (
          new IBocTextValueValidatorFactory[] { new BocTextValueValidatorFactory(), new BocValidatorFactory() });
      var bocTextValueValidatorFactory = new SwitchingBocTextValueValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringBocTextValueValidatorFactoryDecorator (
              compoundBocTextValueValidatorFactory),
          new BocTextValueValidatorFactory ());
      defaultServiceLocator.RegisterSingle<IBocTextValueValidatorFactory> (() => bocTextValueValidatorFactory);

      var compoundBusinessObjectReferenceDataSourceControlValidatorFactory = new CompoundBusinessObjectReferenceDataSourceControlValidatorFactory (
          new IBusinessObjectReferenceDataSourceControlValidatorFactory[] { new BocReferenceDataSourceValidatorFactory() });
      var businessObjectReferenceDataSourceControlValidatorFactory = new SwitchingBusinessObjectReferenceDataSourceControlValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringBusinessObjectReferenceDataSourceControlValidatorFactoryDecorator (
              compoundBusinessObjectReferenceDataSourceControlValidatorFactory),
          new CompoundValidatorFactory<BusinessObjectReferenceDataSourceControl> (Enumerable.Empty<IBocValidatorFactory<BusinessObjectReferenceDataSourceControl>> ()));
      defaultServiceLocator.RegisterSingle<IBusinessObjectReferenceDataSourceControlValidatorFactory> (
          () => businessObjectReferenceDataSourceControlValidatorFactory);

      var compoundUserControlBindingValidatorFactory = new CompoundUserControlBindingValidatorFactory (
          new IUserControlBindingValidatorFactory[] { new UserControlBindingValidatorValidatorFactory() });
      var userControlBindingValidatorFactory = new SwitchingUserControlBindingValidatorFactoryDecorator (
          SwitchingValidatorFactoryState.Instance,
          new FilteringUserControlBindingValidatorFactoryDecorator (
              compoundUserControlBindingValidatorFactory),
          new CompoundValidatorFactory<UserControlBinding>(Enumerable.Empty<IBocValidatorFactory<UserControlBinding>>()));
      defaultServiceLocator.RegisterSingle<IUserControlBindingValidatorFactory> (() => userControlBindingValidatorFactory);
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
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture (Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {
      }
      try
      {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo (Request.UserLanguages[0]);
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