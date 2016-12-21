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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering
{
  [ImplementationFor (typeof (IDatePickerPageRenderer), Lifetime = LifetimeKind.Singleton)]
  public class DatePickerPageRenderer : IDatePickerPageRenderer
  {
    private readonly IResourceUrlFactory _resourceUrlFactory;

    public DatePickerPageRenderer (IResourceUrlFactory resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("resourceUrlFactory", resourceUrlFactory);

      _resourceUrlFactory = resourceUrlFactory;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string key = typeof (DatePickerPageRenderer).FullName + "_Script";
      var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (DatePickerPageRenderer), ResourceType.Html, "DatePicker.js");
      htmlHeadAppender.RegisterJavaScriptInclude (key, scriptUrl);

      htmlHeadAppender.RegisterPageStylesheetLink();
    }

    protected IResourceUrlFactory ResourceUrlFactory
    {
      get { return _resourceUrlFactory; }
    }
  }
}