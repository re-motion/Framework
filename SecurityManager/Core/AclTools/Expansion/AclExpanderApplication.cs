// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.ServiceLocation;
using Remotion.Tools.Console.ConsoleApplication;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderApplication : IApplicationRunner<AclExpanderApplicationSettings>
  {
    public const string CssFileName = "AclExpansion";

    private AclExpanderApplicationSettings? _settings;

    private readonly ITextWriterFactory _textWriterFactory;


    public AclExpanderApplication ()
        : this(new StreamWriterFactory())
    {
    }

    public AclExpanderApplication (ITextWriterFactory textWriterFactory)
    {
      ArgumentUtility.CheckNotNull("textWriterFactory", textWriterFactory);
      _textWriterFactory = textWriterFactory;
    }


    public AclExpanderApplicationSettings Settings
    {
      get
      {
        Assertion.IsNotNull(_settings, "Settings have not been initialized via Init(...) method.");
        return _settings;
      }
    }

    public virtual void Run (AclExpanderApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
      ArgumentUtility.CheckNotNull("settings", settings);
      ArgumentUtility.CheckNotNull("errorWriter", errorWriter);
      ArgumentUtility.CheckNotNull("logWriter", logWriter);

      Init(settings);

      var hasOriginalServiceLocator = ServiceLocator.IsLocationProviderSet;
      try
      {
        if (!hasOriginalServiceLocator)
        {
          var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
          var storageSettingsFactory = StorageSettingsFactory.CreateForSqlServer(settings.ConnectionString);
          serviceLocator.RegisterSingle(() => storageSettingsFactory);
          ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        string? cultureName = GetCultureName();

        using (new CultureScope(cultureName))
        {
          using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
          {
            List<AclExpansionEntry> aclExpansion = GetAclExpansion();

            WriteAclExpansionAsHtmlToStreamWriter(aclExpansion);
          }
        }
      }
      finally
      {
        if (!hasOriginalServiceLocator)
          ServiceLocator.SetLocatorProvider(null);
      }
    }

    public string? GetCultureName ()
    {
      string? cultureName = Settings.CultureName;
      if (String.IsNullOrEmpty(cultureName))
      {
        cultureName = null; // Passing null to CultureScope-ctor below means "keep current culture".
      }
      return cultureName;
    }

    public void Init (AclExpanderApplicationSettings settings)
    {
      ArgumentUtility.CheckNotNull("settings", settings);
      _settings = settings;
    }

    public virtual void WriteAclExpansionAsHtmlToStreamWriter (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull("aclExpansion", aclExpansion);
      if (Settings.UseMultipleFileOutput)
      {
        WriteAclExpansionAsMultiFileHtml(aclExpansion);
      }
      else
      {
        WriteAclExpansionAsSingleFileHtml(aclExpansion);
      }
    }

    private void WriteAclExpansionAsSingleFileHtml (List<AclExpansionEntry> aclExpansion)
    {
      _textWriterFactory.Extension = "html";
      string directoryUsed = Settings.Directory;
      _textWriterFactory.Directory = directoryUsed;
      using (var textWriter = _textWriterFactory.CreateTextWriter("AclExpansion_" + StringUtility.GetFileNameTimestampNow()))
      {
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(textWriter, true, CreateAclExpansionHtmlWriterSettings());
        aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      }
      WriteCssFile();
    }

    private void WriteAclExpansionAsMultiFileHtml (List<AclExpansionEntry> aclExpansion)
    {
      string directoryUsed = Path.Combine(Settings.Directory, "AclExpansion_" + StringUtility.GetFileNameTimestampNow());
      _textWriterFactory.Directory = directoryUsed;
      _textWriterFactory.Extension = "html";

      var multiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter(_textWriterFactory, true);
      multiFileHtmlWriter.DetailHtmlWriterSettings = CreateAclExpansionHtmlWriterSettings();
      multiFileHtmlWriter.WriteAclExpansion(aclExpansion);

      WriteCssFile();
    }

    private void WriteCssFile ()
    {
      Assertion.DebugIsNotNull(_textWriterFactory.Directory, "_textWriterFactory.Directory != null");
      using (var cssTextWriter = _textWriterFactory.CreateTextWriter(_textWriterFactory.Directory, CssFileName,"css"))
      {
        string resource = GetEmbeddedStringResource("AclExpansion.css");
        Assertion.IsNotNull(resource);
        cssTextWriter.Write(resource);
      }
    }


    private string GetEmbeddedStringResource (string name)
    {
      Type type = GetType();
      Assembly assembly = type.Assembly;
      using (StreamReader reader = new StreamReader(Assertion.IsNotNull(assembly.GetManifestResourceStream(type, name), "assembly.GetManifestResourceStream(type, name) != null")))
      {
        return reader.ReadToEnd();
      }
    }


    // Returns an AclExpansionHtmlWriterSettings initialized from the AclExpanderApplication Settings.
    private AclExpansionHtmlWriterSettings CreateAclExpansionHtmlWriterSettings ()
    {
      var aclExpansionHtmlWriterSettings = new AclExpansionHtmlWriterSettings();
      aclExpansionHtmlWriterSettings.OutputRowCount = Settings.OutputRowCount;
      aclExpansionHtmlWriterSettings.OutputDeniedRights = Settings.OutputDeniedRights;
      return aclExpansionHtmlWriterSettings;
    }


    public virtual List<AclExpansionEntry> GetAclExpansion ()
    {
      var aclExpander =
          new AclExpander(
              new AclExpanderUserFinder(Settings.UserFirstName, Settings.UserLastName, Settings.UserName), new AclExpanderAclFinder()
              );

      return aclExpander.GetAclExpansionEntryListSortedAndDistinct();
    }
  }
}
