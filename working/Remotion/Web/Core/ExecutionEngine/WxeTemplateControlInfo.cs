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
using System.Web;
using System.Web.UI;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{

  public class WxeTemplateControlInfo
  {
    private WxeHandler _wxeHandler;
    private WxePageStep _currentPageStep;
    private WxeUserControlStep _currentUserControlStep;
    private WxeFunction _currentPageFunction;
    private WxeFunction _currentUserControlFunction;

    private readonly IWxeTemplateControl _control;
    /// <summary> Caches the <see cref="ResourceManagerSet"/> for this control. </summary>
    private ResourceManagerSet _cachedResourceManager;
    
    public WxeTemplateControlInfo (IWxeTemplateControl control)
    {
      ArgumentUtility.CheckNotNullAndType<TemplateControl> ("control", control);
      
      _control = control;
    }

    public virtual void Initialize (HttpContext context)
    {
      if (ControlHelper.IsDesignMode (_control))
        return;
      ArgumentUtility.CheckNotNull ("context", context);

      if (_control is Page)
      {
        _wxeHandler = context.Handler as WxeHandler;
      }
      else
      {
        IWxePage wxePage = _control.Page as IWxePage;
        if (wxePage == null)
          throw new InvalidOperationException (string.Format ("'{0}' can only be added to a Page implementing the IWxePage interface.", _control.GetType ().FullName));
        _wxeHandler = wxePage.WxeHandler;
      }
      if (_wxeHandler == null)
      {
        throw new HttpException (string.Format ("No current WxeHandler found. Most likely cause of the exception: "
            + "The page '{0}' has been called directly instead of using a WXE Handler to invoke the associated WXE Function.",
            _control.Page.GetType ()));
      }


      WxeStep executingStep = _wxeHandler.RootFunction.ExecutingStep;
      if (executingStep is WxeUserControlStep)
      {
        _currentUserControlStep = (WxeUserControlStep) executingStep;
        _currentUserControlFunction = WxeStep.GetFunction (_currentUserControlStep);
        _currentPageStep = _currentUserControlStep.PageStep;
      }
      else
      {
        _currentUserControlStep = null;
        _currentUserControlFunction = null;
        _currentPageStep = (WxePageStep) executingStep;
      }

      _currentPageFunction = WxeStep.GetFunction (_currentPageStep);
    }

    public WxeHandler WxeHandler
    {
      get { return _wxeHandler; }
    }

    public WxePageStep CurrentPageStep
    {
      get { return _currentPageStep; }
    }

    public WxeUserControlStep CurrentUserControlStep
    {
      get { return _currentUserControlStep; }
    }

    public WxeFunction CurrentPageFunction
    {
      get { return _currentPageFunction; }
    }

    public WxeFunction CurrentFunction
    {
      get { return _currentUserControlFunction ?? _currentPageFunction; }
    }

    public NameObjectCollection PageVariables
    {
      get
      {
        Assertion.IsNotNull (_currentPageStep);
        return _currentPageStep.Variables;
      }
    }

    public NameObjectCollection Variables
    {
      get 
      {
        Assertion.IsNotNull (_currentPageStep);
        return ((WxeStep) _currentUserControlStep ?? _currentPageStep).Variables;
      }
    }

    public IGlobalizationService GlobalizationService
    {
      get
      {
        return SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
      }
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this control info. </summary>
    /// <param name="localResourcesType"> 
    ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it.
    ///   Typically an <b>enum</b> or the derived class itself.
    /// </param>
    protected IResourceManager GetResourceManager (Type localResourcesType)
    {
      ArgumentUtility.CheckNotNull ("localResourcesType", localResourcesType);

      //  Provider has already been identified.
      if (_cachedResourceManager != null)
        return _cachedResourceManager;

      //  Get the resource managers

      var localResourceManager = GlobalizationService.GetResourceManager (localResourcesType);
      var namingContainer = _control.NamingContainer ?? (Control) _control;
      var namingContainerResourceManager = ResourceManagerUtility.GetResourceManager (namingContainer, true);

      _cachedResourceManager = ResourceManagerSet.Create (namingContainerResourceManager, localResourceManager);

      return _cachedResourceManager;
    }
  }

}
