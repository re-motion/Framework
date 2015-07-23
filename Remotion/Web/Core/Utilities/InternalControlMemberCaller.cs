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
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.Utilities
{
  /// <summary>
  /// Default implementation of the <seealso cref="IInternalControlMemberCaller"/> interface.
  /// </summary>
  [ImplementationFor (typeof (IInternalControlMemberCaller), Lifetime = LifetimeKind.Singleton)]
  public class InternalControlMemberCaller : IInternalControlMemberCaller
  {
    private const BindingFlags c_bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
    public static readonly Type InternalControlStateType = typeof (Control).Assembly.GetType ("System.Web.UI.ControlState", true, false);

    //  private System.Web.UI.UpdatePanel._rendered
    private static readonly FieldInfo s_updatePanelRenderedFieldInfo = typeof (UpdatePanel).GetField ("_rendered", c_bindingFlags);

    private static readonly Lazy<Action<Control, object>> s_set_ControlState = new Lazy<Action<Control, object>> (
        () =>
        {
          var methodInfo = typeof (Control).GetMethod (
              "set_ControlState",
              c_bindingFlags,
              null,
              new[] { InternalControlStateType },
              new ParameterModifier[0]);
          return MethodInfoAdapter.Create (methodInfo).GetFastInvoker<Action<Control, object>>();
        });

    private static readonly Lazy<Func<Control, int>> s_get_ControlState = GetLazyMethod<Func<Control, int>> ("get_ControlState");

    private static readonly Lazy<Action<Control, Control>> s_InitRecursive = GetLazyMethod<Action<Control, Control>> ("InitRecursive");

    private static readonly Lazy<Action<Control, object>> s_LoadViewStateRecursive = GetLazyMethod<Action<Control, object>> ("LoadViewStateRecursive");

    private static readonly Lazy<Func<Control, ViewStateMode, object>> s_SaveViewStateRecursive = 
        GetLazyMethod<Func<Control, ViewStateMode, object>> ("SaveViewStateRecursive");

    private static readonly Lazy<Action<Page>> s_SaveAllState = GetLazyMethod<Action<Page>> ("SaveAllState");

    private static readonly Lazy<Func<Control, object>> s_SaveControlStateInternal = GetLazyMethod<Func<Control, object>> ("SaveControlStateInternal");

    private static readonly Lazy<Action<Control>> s_ClearChildControlState = GetLazyMethod<Action<Control>> ("ClearChildControlState");

    private static readonly Lazy<Func<Page, PageStatePersister>> s_get_PageStatePersister =
        GetLazyMethod<Func<Page, PageStatePersister>> ("get_PageStatePersister");

    private static readonly Lazy<Func<ControlCollection, string, string>> s_SetCollectionReadOnly =
        GetLazyMethod<Func<ControlCollection, string, string>> ("SetCollectionReadOnly");

    private static readonly Lazy<Action<Control, HtmlTextWriter, ICollection>> s_RenderChildrenInternal =
        GetLazyMethod<Action<Control, HtmlTextWriter, ICollection>> ("RenderChildrenInternal");

    public void SetControlState (Control control, ControlState value)
    {
      object internalValue = Enum.ToObject (InternalControlStateType, value);
      s_set_ControlState.Value (control, internalValue);
    }

    public ControlState GetControlState (Control control)
    {
      int internalValue = s_get_ControlState.Value (control);
      return (ControlState) internalValue;
    }

    public void InitRecursive (Control control, Control namingContainer)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("namingContainer", namingContainer);

      //  internal void System.Web.UI.Control.InitRecursive (Control)
      s_InitRecursive.Value (control, namingContainer);
    }

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s LoadViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be restored. </param>
    /// <param name="viewState"> The view state object used for restoring. </param>
    public void LoadViewStateRecursive (Control target, object viewState)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      //  internal void System.Web.UI.Control.LoadViewStateRecursive (object)
      s_LoadViewStateRecursive.Value (target, viewState);
    }

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s SaveViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be saved. </param>
    /// <returns> The view state object for <paramref name="target"/>. </returns>
    public object SaveViewStateRecursive (Control target)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      var inheritedViewState = target.CreateSequence (c => c.Parent)
                                     .Select (c => (ViewStateMode?) c.ViewStateMode)
                                     .FirstOrDefault (m => m != ViewStateMode.Inherit) ?? ViewStateMode.Enabled;

      //  internal object System.Web.UI.Control.SaveViewStateRecursive(ViewStateMode)
      return s_SaveViewStateRecursive.Value (target, inheritedViewState);
    }

    /// <summary>Encapsulates the invocation of <see cref="Page"/>'s SaveAllState method.</summary>
    /// <param name="page">The <see cref="Page"/> for which SaveAllState will be invoked. Must not be <see langword="null" />.</param>
    public void SaveAllState (Page page)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      //  private void System.Web.UI.Page.SaveAllState()
      s_SaveAllState.Value (page);
    }

    /// <summary>Encapsulates the invocation of <see cref="Control"/>'s SaveChildControlState method.</summary>
    /// <param name="control">The <see cref="Control"/> for which SaveChildControlState will be invoked. Must not be <see langword="null" />.</param>
    public IDictionary SaveChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer: Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      //  private ControlSet System.Web.UI.Page._registeredControlsRequiringControlState
      var registeredControlsRequiringControlStateFieldInfo = typeof (Page).GetField ("_registeredControlsRequiringControlState", c_bindingFlags);
      var registeredControlsRequiringControlState = (ICollection) registeredControlsRequiringControlStateFieldInfo.GetValue (control.Page);

      //LosFormatter only supports Hashtable and HybridDictionary without using native serialization
      var dictionary = new HybridDictionary ();
      if (registeredControlsRequiringControlState != null)
      {
        foreach (Control registeredControl in registeredControlsRequiringControlState)
        {
          if (registeredControl.UniqueID.StartsWith (control.UniqueID) && registeredControl != control)
          {
            object controlState = SaveControlStateInternal (registeredControl);
            if (controlState != null)
              dictionary.Add (registeredControl.UniqueID, controlState);
          }
        }
      }

      if (dictionary.Count == 0)
        return null;
      return dictionary;
    }

    /// <summary>Encapsulates the invocation of <see cref="Control"/>'s SaveControlStateInternal method.</summary>
    /// <param name="control">The <see cref="Control"/> for which SaveControlStateInternal will be invoked. Must not be <see langword="null" />.</param>
    public object SaveControlStateInternal (Control control)
    {
      //  protected object System.Web.UI.Page.SaveControlStateInternal
      return s_SaveControlStateInternal.Value (control);
    }

    /// <summary>Returns the control states for all controls that are child-controls of the passed <see cref="Control"/>.</summary>
    public IDictionary GetChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer: Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      //LosFormatter only supports Hashtable and HybridDictionary without using native serialization
      var childControlState = new HybridDictionary ();

      var pageStatePersister = GetPageStatePersister (control.Page);
      var controlStates = (IDictionary) pageStatePersister.ControlState;

      var parentPrefix = control.UniqueID + control.Page.IdSeparator;
      foreach (string key in controlStates.Keys)
      {
        if (key.StartsWith (parentPrefix))
          childControlState.Add (key, controlStates[key]);
      }

      if (childControlState.Count == 0)
        return null;
      return childControlState;
    }

    /// <summary>Sets the control states for the child control of the passed <see cref="Control"/>.</summary>
    public void SetChildControlState<TNamingContainer> (TNamingContainer control, IDictionary newControlState)
        where TNamingContainer: Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (newControlState == null)
        return;

      var pageStatePersister = GetPageStatePersister (control.Page);
      var controlState = (IDictionary) pageStatePersister.ControlState;

      foreach (string key in newControlState.Keys)
        controlState[key] = newControlState[key];
    }

    /// <summary>Sets the control states for the child control of the passed <see cref="Control"/>.</summary>
    public void ClearChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer: Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      //  protected void System.Web.UI.Control.ClearChildControlState
      s_ClearChildControlState.Value (control);
    }

    /// <summary>Encapsulates the get-access the the <see cref="Page"/>'s PageStatePersister property.</summary>
    public PageStatePersister GetPageStatePersister (Page page)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      //  protected PageStatePersister System.Web.UI.Page.PageStatePersister
      return s_get_PageStatePersister.Value (page);
    }

    public string SetCollectionReadOnly (ControlCollection collection, string exceptionMessage)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      //  internal void System.Web.UI.ControlCollection.SetCollectionReadOnly
      return s_SetCollectionReadOnly.Value (collection, exceptionMessage);
    }

    /// <summary>Calls the <b>RenderChildrenInternal</b> method of the <see cref="Control"/>.</summary>
    public void RenderChildrenInternal (Control control, HtmlTextWriter writer, ICollection controls)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("controls", controls);

      //  internal void System.Web.UI.Control.RenderChildrenInternal
      s_RenderChildrenInternal.Value (control, writer, controls);
    }

    /// <summary>Sets the <b>_rendered</b> flag of the <see cref="UpdatePanel"/>.</summary>
    public void SetUpdatePanelRendered (UpdatePanel updatePanel, bool value)
    {
      ArgumentUtility.CheckNotNull ("updatePanel", updatePanel);

      s_updatePanelRenderedFieldInfo.SetValue (updatePanel, value);
    }

    private static Lazy<T> GetLazyMethod<T> (string name)
    {
      return new Lazy<T> (
          () =>
          {
            var invokeMethod = typeof (T).GetMethod ("Invoke");
            var targetType = invokeMethod.GetParameters().First().ParameterType;
            var parameterTypes = invokeMethod.GetParameters().Skip (1).Select (p => p.ParameterType).ToArray();

            var methodInfo = targetType.GetMethod (name, c_bindingFlags, null, parameterTypes, new ParameterModifier[0]);
            Assertion.IsNotNull (
                methodInfo,
                "Type '{0}' does not contain method {1} ({2}).",
                targetType,
                name,
                string.Join<Type> (",", parameterTypes));

            return (T) (object) methodInfo.CreateDelegate (typeof (T));
          });
    }
  }
}
