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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.WebTreeViewImplementation;
using Remotion.Web.UI.Controls.WebTreeViewImplementation.Rendering;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary> A tree view. </summary>
  /// <include file='..\..\doc\include\UI\Controls\WebTreeView.xml' path='WebTreeView/Class/*' />
  [ToolboxData ("<{0}:WebTreeView runat=server></{0}:WebTreeView>")]
  [DefaultEvent ("Click")]
  public class WebTreeView : WebControl, IWebTreeView, IPostBackEventHandler, IResourceDispatchTarget
  {
    // constants

    #region private const string c_nodeIcon...

    private const string c_nodeIconF = "TreeViewF.gif";
    private const string c_nodeIconFMinus = "TreeViewFMinus.gif";
    private const string c_nodeIconFPlus = "TreeViewFPlus.gif";
    private const string c_nodeIconI = "TreeViewI.gif";
    private const string c_nodeIconL = "TreeViewL.gif";
    private const string c_nodeIconLMinus = "TreeViewLMinus.gif";
    private const string c_nodeIconLPlus = "TreeViewLPlus.gif";
    private const string c_nodeIconMinus = "TreeViewMinus.gif";
    private const string c_nodeIconPlus = "TreeViewPlus.gif";
    private const string c_nodeIconR = "TreeViewR.gif";
    private const string c_nodeIconRMinus = "TreeViewRMinus.gif";
    private const string c_nodeIconRPlus = "TreeViewRPlus.gif";
    private const string c_nodeIconT = "TreeViewT.gif";
    private const string c_nodeIconTMinus = "TreeViewTMinus.gif";
    private const string c_nodeIconTPlus = "TreeViewTPlus.gif";
    private const string c_nodeIconWhite = "TreeViewWhite.gif";

    #endregion

    /// <summary> The separator used for the node path. </summary>
    private const char c_pathSeparator = '\t';

    /// <summary> The prefix for the expansion command. </summary>
    private const string c_expansionCommandPrefix = "Expand=";

    /// <summary> The prefix for the click command. </summary>
    private const string c_clickCommandPrefix = "Click=";

    // statics
    private static readonly object s_clickEvent = new object();
    private static readonly object s_selectionChangedEvent = new object();

    // types

    // fields
    // The URL resolved icon paths.

    #region private IconInfo _resolvedNodeIcon...

    private IconInfo _resolvedNodeIconF;
    private IconInfo _resolvedNodeIconFMinus;
    private IconInfo _resolvedNodeIconFPlus;
    private IconInfo _resolvedNodeIconI;
    private IconInfo _resolvedNodeIconL;
    private IconInfo _resolvedNodeIconLMinus;
    private IconInfo _resolvedNodeIconLPlus;
    private IconInfo _resolvedNodeIconMinus;
    private IconInfo _resolvedNodeIconPlus;
    private IconInfo _resolvedNodeIconR;
    private IconInfo _resolvedNodeIconRMinus;
    private IconInfo _resolvedNodeIconRPlus;
    private IconInfo _resolvedNodeIconT;
    private IconInfo _resolvedNodeIconTMinus;
    private IconInfo _resolvedNodeIconTPlus;
    private IconInfo _resolvedNodeIconWhite;

    #endregion

    /// <summary> The nodes in this tree view. </summary>
    private readonly WebTreeNodeCollection _nodes;

    private Triplet[] _nodesControlState;
    private bool _isLoadControlStateCompleted;
    private bool _enableTopLevelExpander = true;
    private bool _enableLookAheadEvaluation;

    private bool _enableScrollBars;
    private bool _enableWordWrap;
    private bool _showLines = true;
    private bool _enableTreeNodeControlState = true;
    private bool _hasTreeNodesCreated;
    private bool _requiresSynchronousPostBack;
    private WebTreeNode _selectedNode;
    private WebTreeViewMenuItemProvider _menuItemProvider;
    private readonly Dictionary<WebTreeNode, DropDownMenu> _menus = new Dictionary<WebTreeNode, DropDownMenu>();
    private readonly PlaceHolder _menuPlaceHolder;
    private bool _hasTreeNodeMenusCreated;
    private int _menuCounter;

    private readonly IRenderingFeatures _renderingFeatures;

    /// <summary>
    ///   The delegate called before a node with <see cref="WebTreeNode.IsEvaluated"/> set to <see langword="false"/>
    ///   is expanded.
    /// </summary>
    private EvaluateWebTreeNode _evaluateTreeNode;

    private InitializeRootWebTreeNodes _initializeRootTreeNodes;
    private WebTreeNodeRenderMethod _treeNodeRenderMethod;
    private IPage _page;
    private IInfrastructureResourceUrlFactory _infrastructureResourceUrlFactory;

    //  construction and destruction

    /// <summary> Initalizes a new instance. </summary>
    public WebTreeView (IControl ownerControl)
    {
      _nodes = new WebTreeNodeCollection (ownerControl);
      _nodes.SetParent (this, null);
      _menuPlaceHolder = new PlaceHolder();
      _renderingFeatures = SafeServiceLocator.Current.GetInstance<IRenderingFeatures>();
    }

    /// <summary> Initalizes a new instance. </summary>
    public WebTreeView ()
        : this (null)
    {
    }

    protected override void CreateChildControls ()
    {
      base.CreateChildControls();

      _menuPlaceHolder.ID = ID + "_Menus";
      _menuPlaceHolder.EnableViewState = false;
      Controls.Add (_menuPlaceHolder);
    }

    //  methods and properties

    //  public void RaisePostDataChangedEvent()
    //  {
    //  }
    //
    //  public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
    //  {
    //    return false;
    //  }

    /// <summary> Implementation of the <see cref="IPostBackEventHandler"/> interface. </summary>
    /// <param name="eventArgument"> &lt;command prefix&gt;&lt;node path&gt;</param>
    void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);
      EnsureTreeNodesCreated();

      eventArgument = eventArgument.Trim();
      if (eventArgument.StartsWith (c_expansionCommandPrefix))
        HandleExpansionCommandEvent (eventArgument.Substring (c_expansionCommandPrefix.Length));
      else if (eventArgument.StartsWith (c_clickCommandPrefix))
        HandleClickCommandEvent (eventArgument.Substring (c_clickCommandPrefix.Length));
      else
        throw new ArgumentException ("Argument 'eventArgument' has unknown prefix: '" + eventArgument + "'.");
    }

    /// <summary> Handles the expansion command (i.e. expands/collapses the clicked tree node). </summary>
    /// <param name="eventArgument"> The path to the clicked tree node. </param>
    private void HandleExpansionCommandEvent (string eventArgument)
    {
      string[] pathSegments;
      WebTreeNode clickedNode = ParseNodePath (eventArgument, out pathSegments);
      if (clickedNode != null)
      {
        if (clickedNode.IsEvaluated)
        {
          clickedNode.IsExpanded = !clickedNode.IsExpanded;
          if (clickedNode.IsExpanded && EnableLookAheadEvaluation)
            clickedNode.EvaluateChildren();
        }
        else
        {
          clickedNode.EvaluateExpand();
          if (EnableLookAheadEvaluation)
            clickedNode.EvaluateChildren();
        }

        if (clickedNode.IsExpanded)
          InitializeTreeNodeMenus (clickedNode.Children);
      }
    }

    /// <summary> Handles the click command. </summary>
    /// <param name="eventArgument"> The path to the clicked tree node. </param>
    private void HandleClickCommandEvent (string eventArgument)
    {
      string[] pathSegments;
      WebTreeNode clickedNode = ParseNodePath (eventArgument, out pathSegments);
      bool isSelectionChanged = _selectedNode != clickedNode;
      SetSelectedNode (clickedNode);
      OnClick (clickedNode, pathSegments);
      if (isSelectionChanged)
        OnSelectionChanged (clickedNode);
    }

    /// <summary> Fires the <see cref="Click"/> event. </summary>
    protected virtual void OnClick (WebTreeNode node, string[] path)
    {
      WebTreeNodeClickEventHandler handler = (WebTreeNodeClickEventHandler) Events[s_clickEvent];
      if (handler != null)
      {
        WebTreeNodeClickEventArgs e = new WebTreeNodeClickEventArgs (node, path);
        handler (this, e);
      }
    }

    /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
    protected virtual void OnSelectionChanged (WebTreeNode node)
    {
      WebTreeNodeEventHandler handler = (WebTreeNodeEventHandler) Events[s_selectionChangedEvent];
      if (handler != null)
      {
        WebTreeNodeEventArgs e = new WebTreeNodeEventArgs (node);
        handler (this, e);
      }
    }

    public void SetEvaluateTreeNodeDelegate (EvaluateWebTreeNode evaluateTreeNode)
    {
      _evaluateTreeNode = evaluateTreeNode;
    }

    public void SetInitializeRootTreeNodesDelegate (InitializeRootWebTreeNodes initializeRootTreeNodes)
    {
      _initializeRootTreeNodes = initializeRootTreeNodes;
    }

    public void SetTreeNodeRenderMethodDelegate (WebTreeNodeRenderMethod treeNodeRenderMethod)
    {
      _treeNodeRenderMethod = treeNodeRenderMethod;
    }

    //  /// <summary> Collapses all nodes of this tree view. Only the root nodes will remain visible. </summary>
    //  public void CollapseAll()
    //  {
    //    _nodes.SetExpansion (false);
    //  }
    //    
    //  /// <summary> Expands all nodes of this tree view.</summary>
    //  public void ExpandAll()
    //  {
    //    _nodes.SetExpansion (true);
    //  }

    protected void EnsureTreeNodesCreated ()
    {
      if (_hasTreeNodesCreated)
        return;

      _hasTreeNodesCreated = true;

      if (_initializeRootTreeNodes != null)
        _initializeRootTreeNodes();

      if (_nodesControlState != null)
        LoadNodesControlStateRecursive (_nodesControlState, _nodes);

      if (_nodes.Count == 0)
        _hasTreeNodesCreated = false;
    }

    /// <summary>
    ///   Calles the delegate set using <see cref="SetEvaluateTreeNodeDelegate"/> with the passed <paramref name="node"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">
    ///   Thrown if no method is registered for this delegate but a node with 
    ///   <see cref="WebTreeNode.IsEvaluated"/> set to <see langword="false"/> is going to be expanded.
    /// </exception>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if the registered method has not set the <see cref="WebTreeNode.IsEvaluated"/> flag.
    /// </exception>
    protected internal void EvaluateTreeNodeInternal (WebTreeNode node)
    {
      ArgumentUtility.CheckNotNull ("node", node);

      if (_evaluateTreeNode == null)
        throw new NullReferenceException ("EvaluateTreeNode has no method registered but tree node '" + node.ItemID + "' is not evaluated.");
      _evaluateTreeNode (node);
      if (!node.IsEvaluated)
        throw new InvalidOperationException ("EvaluateTreeNode called for tree node '" + node.ItemID + "' but did not evaluate the tree node.");
    }

    public void ResetNodes ()
    {
      Nodes.Clear();
      _menuPlaceHolder.Controls.Clear();
      _menus.Clear();
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      if (!IsDesignMode)
        Page.RegisterRequiresControlState (this);
      if (Page != null && !Page.IsPostBack)
        _isLoadControlStateCompleted = true;

      if (!IsDesignMode)
        RegisterHtmlHeadContents (HtmlHeadAppender.Current, Context);
    }


    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;

      base.LoadControlState (values[0]);
      if (_enableTreeNodeControlState)
        _nodesControlState = (Triplet[]) values[1];
      else
        _nodesControlState = null;
      _menuCounter = (int) values[2];

      _isLoadControlStateCompleted = true;
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[3];

      values[0] = base.SaveControlState();
      if (_enableTreeNodeControlState)
        values[1] = SaveNodesControlStateRecursive (_nodes);
      values[2] = _menuCounter;

      return values;
    }

    /// <summary> Loads the settings of the <paramref name="nodes"/> from <paramref name="nodesState"/>. </summary>
    private void LoadNodesControlStateRecursive (Triplet[] nodesState, WebTreeNodeCollection nodes)
    {
      for (int i = 0; i < nodesState.Length; i++)
      {
        Triplet nodeState = nodesState[i];
        string nodeID = (string) nodeState.First;
        WebTreeNode node = nodes.Find (nodeID);
        if (node != null)
        {
          object[] values = (object[]) nodeState.Second;
          node.IsExpanded = (bool) values[0];
          if (!node.IsEvaluated)
          {
            bool isEvaluated = (bool) values[1];
            if (isEvaluated)
              EvaluateTreeNodeInternal (node);
          }
          bool isSelected = (bool) values[2];
          if (isSelected)
            node.IsSelected = true;
          node.MenuID = (string) values[3];
          LoadNodesControlStateRecursive ((Triplet[]) nodeState.Third, node.Children);
        }
      }
    }

    /// <summary> Saves the settings of the  <paramref name="nodes"/> and returns this view state </summary>
    private Triplet[] SaveNodesControlStateRecursive (WebTreeNodeCollection nodes)
    {
      EnsureTreeNodesCreated();
      Triplet[] nodesState = new Triplet[nodes.Count];
      for (int i = 0; i < nodes.Count; i++)
      {
        WebTreeNode node = nodes[i];
        Triplet nodeControlState = new Triplet();
        nodeControlState.First = node.ItemID;
        object[] values = new object[4];
        values[0] = node.IsExpanded;
        values[1] = node.IsEvaluated;
        values[2] = node.IsSelected;
        values[3] = node.MenuID;
        nodeControlState.Second = values;
        nodeControlState.Third = SaveNodesControlStateRecursive (node.Children);
        nodesState[i] = nodeControlState;
      }
      return nodesState;
    }


    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    void IResourceDispatchTarget.Dispatch (IDictionary values)
    {
      ArgumentUtility.CheckNotNull ("values", values);
      Dispatch (values);
    }

    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    protected virtual void Dispatch (IDictionary values)
    {
      //  Dispatch simple properties
      ResourceDispatcher.DispatchGeneric (this, values);
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected virtual void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);

      if (ControlHelper.IsDesignMode (this))
        return;

      string key = ResourceManagerUtility.GetGlobalResourceKey (AccessKey);
      if (!string.IsNullOrEmpty (key))
        AccessKey = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
      if (!string.IsNullOrEmpty (key))
        ToolTip = resourceManager.GetString (key);

      Nodes.LoadResources (resourceManager, globalizationService);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      EnsureTreeNodesCreated();
      EnsureChildControls();
      InitializeTreeNodeMenus (_nodes);
      _hasTreeNodeMenusCreated = true;
    }

    public virtual void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, HttpContextBase httpContext)
    {
      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents (htmlHeadAppender);
    }

    protected virtual IWebTreeViewRenderer CreateRenderer ()
    {
      return SafeServiceLocator.Current.GetInstance<IWebTreeViewRenderer>();
    }

    /// <summary> Overrides the parent control's <c>OnPreRender</c> method. </summary>
    protected override void OnPreRender (EventArgs e)
    {
      EnsureTreeNodesCreated();

      base.OnPreRender (e);

      var resourceManager = ResourceManagerUtility.GetResourceManager (this, true);
      var globalizationService = SafeServiceLocator.Current.GetInstance<IGlobalizationService>();

      LoadResources (resourceManager, globalizationService);

      if (_requiresSynchronousPostBack)
      {
        var scriptManager = ScriptManager.GetCurrent (Page);
        if (scriptManager != null)
        {
          bool hasUpdatePanelAsParent = false;
          for (Control current = Parent; current != null && !(current is Page); current = current.Parent)
          {
            if (current is UpdatePanel)
            {
              hasUpdatePanelAsParent = true;
              break;
            }
          }

          if (hasUpdatePanelAsParent)
          {
            ISmartPage smartPage = Page as ISmartPage;
            if (smartPage == null)
            {
              throw new InvalidOperationException (
                  string.Format (
                      "{0}: WebTreeViews with RequiresSynchronousPostBack set to true are only supported on pages implementing ISmartPage when used within an UpdatePanel.",
                      ID));
            }
            RegisterTreeNodesForSynchronousPostback (smartPage, _nodes);
          }
        }
      }

      PreRenderTreeNodeMenus();
    }

    private void RegisterTreeNodesForSynchronousPostback (ISmartPage smartPage, WebTreeNodeCollection nodes)
    {
      foreach (WebTreeNode node in nodes)
      {
        smartPage.RegisterCommandForSynchronousPostBack (this, GetClickCommandArgument (FormatNodePath (node)));
        RegisterTreeNodesForSynchronousPostback (smartPage, node.Children);
      }
    }

    /// <summary> Overrides the parent control's <c>TagKey</c> property. </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Div; }
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      base.AddAttributesToRender (writer);
      if (_enableScrollBars)
        writer.AddStyleAttribute ("overflow", "auto");
      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        IControlWithDiagnosticMetadata controlWithDiagnosticMetadata = this;
        writer.AddAttribute (DiagnosticMetadataAttributes.ControlType, controlWithDiagnosticMetadata.ControlType);
      }
    }

    /// <summary> Overrides the parent control's <c>RenderContents</c> method. </summary>
    protected override void RenderContents (HtmlTextWriter writer)
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
        WcagHelper.Instance.HandleError (1, this);

      ResolveNodeIcons();
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassRoot);
      writer.RenderBeginTag (HtmlTextWriterTag.Ul); // Begin child nodes
      RenderNodes (writer, _nodes, true);
      writer.RenderEndTag();
      if (IsDesignMode && _nodes.Count == 0)
        RenderDesignModeContents (writer);
      foreach (DropDownMenu menu in _menuPlaceHolder.Controls)
        menu.RenderAsContextMenu (writer);
    }

    /// <summary> Renders the <paremref name="nodes"/> onto the <paremref name="writer"/>. </summary>
    private void RenderNodes (HtmlTextWriter writer, WebTreeNodeCollection nodes, bool isTopLevel)
    {
      for (int i = 0; i < nodes.Count; i++)
      {
        WebTreeNode node = nodes[i];
        bool isFirstNode = i == 0;
        bool isLastNode = i + 1 == nodes.Count;

        if (_renderingFeatures.EnableDiagnosticMetadata)
        {
          if (!string.IsNullOrEmpty (node.ItemID))
            writer.AddAttribute (DiagnosticMetadataAttributes.ItemID, node.ItemID);
          if (!string.IsNullOrEmpty (node.Text))
            writer.AddAttribute (DiagnosticMetadataAttributes.Content, HtmlUtility.StripHtmlTags (node.Text));
          if (node.IsSelected)
            writer.AddAttribute (DiagnosticMetadataAttributes.WebTreeViewIsSelectedNode, "true");
          writer.AddAttribute (DiagnosticMetadataAttributes.WebTreeViewNumberOfChildren, node.Children.Count.ToString());
          writer.AddAttribute (DiagnosticMetadataAttributes.IndexInCollection, (i + 1).ToString());
        }

        writer.RenderBeginTag (HtmlTextWriterTag.Li); // Begin node block

        bool hasExpander = !isTopLevel || _enableTopLevelExpander;

        RenderNode (writer, node, isFirstNode, isLastNode, hasExpander);
        bool hasChildren = node.Children.Count > 0;
        if (!hasExpander)
          node.IsExpanded = true;
        if (hasChildren && node.IsExpanded)
          RenderNodeChildren (writer, node, isLastNode, hasExpander);

        writer.RenderEndTag(); // End node block
      }
    }

    /// <summary> Renders the <paramref name="node"/> onto the <paremref name="writer"/>. </summary>
    private void RenderNode (
        HtmlTextWriter writer,
        WebTreeNode node,
        bool isFirstNode,
        bool isLastNode,
        bool hasExpander)
    {
      DropDownMenu menu;
      bool isMenuVisible = false;
      if (_menus.TryGetValue (node, out menu))
      {
        for (int i = 0; i < menu.MenuItems.Count; i++)
        {
          if (menu.MenuItems[i].IsVisible)
          {
            isMenuVisible = true;
            break;
          }
        }
      }

      if (!_enableWordWrap)
        writer.AddStyleAttribute ("white-space", "nowrap");

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNode);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      string nodePath = FormatNodePath (node);

      if (hasExpander)
        RenderNodeExpander (writer, node, nodePath, isFirstNode, isLastNode);

      if (isMenuVisible)
      {
        writer.AddAttribute ("oncontextmenu", "return false;");
        writer.AddAttribute ("name", menu.ClientID);
      }
      RenderNodeHead (writer, node, nodePath);

      writer.RenderEndTag();
    }

    /// <summary> Renders the <paramref name="node"/>'s expander (i.e. +/-) onto the <paremref name="writer"/>. </summary>
    private void RenderNodeExpander (
        HtmlTextWriter writer,
        WebTreeNode node,
        string nodePath,
        bool isFirstNode,
        bool isLastNode)
    {
      IconInfo nodeIcon = GetNodeIcon (node, isFirstNode, isLastNode);
      bool hasChildren = node.Children.Count > 0;
      bool isEvaluated = node.IsEvaluated;
      bool hasExpansionLink = hasChildren || !isEvaluated;
      if (hasExpansionLink)
      {
        string argument = c_expansionCommandPrefix + nodePath;
        string postBackEventReference = Page.ClientScript.GetPostBackEventReference (this, argument);
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEventReference);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        if (_renderingFeatures.EnableDiagnosticMetadata)
        {
          writer.AddAttribute (DiagnosticMetadataAttributes.TriggersPostBack, "true");
          writer.AddAttribute (
              DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
              !node.IsExpanded
                  ? DiagnosticMetadataAttributeValues.WebTreeViewWellKnownExpandAnchor
                  : DiagnosticMetadataAttributeValues.WebTreeViewWellKnownCollapseAnchor);
        }
        writer.RenderBeginTag (HtmlTextWriterTag.A);
      }

      nodeIcon.Render (writer, this);
      if (hasExpansionLink)
        writer.RenderEndTag();
    }

    /// <summary> Renders the <paramref name="node"/>'s head (i.e. icon and text) onto the <paremref name="writer"/>. </summary>
    private void RenderNodeHead (HtmlTextWriter writer, WebTreeNode node, string nodePath)
    {
      if (node.IsSelected)
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNodeHeadSelected);
      else
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNodeHead);
      if (!string.IsNullOrEmpty (node.ToolTip))
        writer.AddAttribute (HtmlTextWriterAttribute.Title, node.ToolTip);

      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      string argument = GetClickCommandArgument (nodePath);
      string postBackEventReference = Page.ClientScript.GetPostBackEventReference (this, argument);
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEventReference);
      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        writer.AddAttribute (DiagnosticMetadataAttributes.TriggersPostBack, "true");
        writer.AddAttribute (
            DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
            DiagnosticMetadataAttributeValues.WebTreeViewWellKnownSelectAnchor);
      }
      writer.RenderBeginTag (HtmlTextWriterTag.A);
      if (_treeNodeRenderMethod == null)
      {
        if (node.Icon != null && node.Icon.HasRenderingInformation)
        {
          node.Icon.Render (writer, this);
          writer.Write ("&nbsp;");
        }
        if (!string.IsNullOrEmpty (node.Text))
          writer.WriteEncodedText (node.Text);
      }
      else
        _treeNodeRenderMethod.Invoke (writer, node);
      writer.RenderEndTag();

      writer.RenderEndTag();
    }

    private string GetClickCommandArgument (string nodePath)
    {
      return c_clickCommandPrefix + nodePath;
    }

    /// <summary> Renders the <paramref name="node"/>'s children onto the <paremref name="writer"/>. </summary>
    private void RenderNodeChildren (HtmlTextWriter writer, WebTreeNode node, bool isLastNode, bool hasExpander)
    {
      if (!hasExpander)
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTopLevelNodeChildren);
      else if (isLastNode || !_showLines)
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNodeChildrenNoLines);
      else
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNodeChildren);
      writer.RenderBeginTag (HtmlTextWriterTag.Ul); // Begin child nodes

      RenderNodes (writer, node.Children, false);

      writer.RenderEndTag(); // End child nodes
    }

    /// <summary> Renders a dummy tree for design mode. </summary>
    private void RenderDesignModeContents (HtmlTextWriter writer)
    {
      WebTreeNodeCollection designModeNodes = new WebTreeNodeCollection (null);
      designModeNodes.SetParent (this, null);
      WebTreeNodeCollection nodes = designModeNodes;
      nodes.Add (new WebTreeNode ("node0", "Node 0"));
      nodes.Add (new WebTreeNode ("node1", "Node 1"));
      nodes.Add (new WebTreeNode ("node2", "Node 2"));
      RenderNodes (writer, designModeNodes, true);
    }

    /// <summary> Generates the string representation of the <paramref name="node"/>'s path. </summary>
    /// <remarks> ...&lt;node.Parent.Parent.ItemID&gt;|&lt;node.Parent.ItemID&gt;|&lt;ItemID&gt; </remarks>
    public string FormatNodePath (WebTreeNode node)
    {
      if (node == null)
        return string.Empty;

      string parentPath = string.Empty;
      if (node.ParentNode != null)
      {
        parentPath = FormatNodePath (node.ParentNode);
        parentPath += c_pathSeparator;
      }
      return parentPath + node.ItemID;
    }

    /// <summary>
    ///   Parses the string generated by <see cref="FormatNodePath"/> and returns the node to which it points.
    /// </summary>
    /// <remarks> If the path cannot be resolved completly, the last valid node in the path is returned. </remarks>
    /// <param name="path"> The path to be parsed. </param>
    /// <param name="pathSegments"> Returns the IDs that comprised the path. </param>
    /// <returns> The <see cref="WebTreeNode"/> to which <paramref name="path"/> pointed. </returns>
    public WebTreeNode ParseNodePath (string path, out string[] pathSegments)
    {
      pathSegments = path.Split (c_pathSeparator);
      WebTreeNode currentNode = null;
      WebTreeNodeCollection currentNodes = _nodes;
      for (int i = 0; i < pathSegments.Length; i++)
      {
        string nodeID = pathSegments[i];
        WebTreeNode node = currentNodes.Find (nodeID);
        if (node == null)
          return currentNode;
        currentNode = node;
        currentNodes = currentNode.Children;
      }
      return currentNode;
    }

    /// <summary> Returns the URL of the node icon for the <paramref name="node"/>. </summary>
    private IconInfo GetNodeIcon (WebTreeNode node, bool isFirstNode, bool isLastNode)
    {
      bool hasChildren = node.Children.Count > 0;
      bool hasParent = node.ParentNode != null;
      bool isOnlyNode = isFirstNode && isLastNode;
      bool isExpanded = node.IsExpanded;
      bool isEvaluated = node.IsEvaluated;

      char expander;
      char type;

      if (!isEvaluated)
        expander = '+';
      else if (hasChildren)
      {
        if (isExpanded)
          expander = '-';
        else
          expander = '+';
      }
      else
        expander = ' ';

      if (hasParent)
      {
        if (isLastNode)
          type = 'L';
        else
          type = 'T';
      }
      else
      {
        if (isOnlyNode)
          type = 'r';
        else if (isFirstNode)
          type = 'F';
        else if (isLastNode)
          type = 'L';
        else
          type = 'T';
      }

      if (_showLines)
      {
        if (expander == ' ' && type == 'F')
          return _resolvedNodeIconF;
        else if (expander == '-' && type == 'F')
          return _resolvedNodeIconFMinus;
        else if (expander == '+' && type == 'F')
          return _resolvedNodeIconFPlus;
        else if (expander == ' ' && type == 'L')
          return _resolvedNodeIconL;
        else if (expander == '-' && type == 'L')
          return _resolvedNodeIconLMinus;
        else if (expander == '+' && type == 'L')
          return _resolvedNodeIconLPlus;
        else if (expander == ' ' && type == 'r')
          return _resolvedNodeIconR;
        else if (expander == '-' && type == 'r')
          return _resolvedNodeIconRMinus;
        else if (expander == '+' && type == 'r')
          return _resolvedNodeIconRPlus;
        else if (expander == ' ' && type == 'T')
          return _resolvedNodeIconT;
        else if (expander == '-' && type == 'T')
          return _resolvedNodeIconTMinus;
        else if (expander == '+' && type == 'T')
          return _resolvedNodeIconTPlus;
      }
      else
      {
        if (expander == ' ')
          return _resolvedNodeIconWhite;
        else if (expander == '-')
          return _resolvedNodeIconMinus;
        else if (expander == '+')
          return _resolvedNodeIconPlus;
      }

      return _resolvedNodeIconWhite;
    }

    /// <summary> Resolves the URLs for the node icons. </summary>
    private void ResolveNodeIcons ()
    {
      _resolvedNodeIconF = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconF).GetUrl());
      _resolvedNodeIconFMinus = new IconInfo (
          InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconFMinus).GetUrl());
      _resolvedNodeIconFPlus = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconFPlus).GetUrl());
      _resolvedNodeIconI = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconI).GetUrl());
      _resolvedNodeIconL = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconL).GetUrl());
      _resolvedNodeIconLMinus = new IconInfo (
          InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconLMinus).GetUrl());
      _resolvedNodeIconLPlus = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconLPlus).GetUrl());
      _resolvedNodeIconMinus = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconMinus).GetUrl());
      _resolvedNodeIconPlus = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconPlus).GetUrl());
      _resolvedNodeIconR = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconR).GetUrl());
      _resolvedNodeIconRMinus = new IconInfo (
          InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconRMinus).GetUrl());
      _resolvedNodeIconRPlus = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconRPlus).GetUrl());
      _resolvedNodeIconT = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconT).GetUrl());
      _resolvedNodeIconTMinus = new IconInfo (
          InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconTMinus).GetUrl());
      _resolvedNodeIconTPlus = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconTPlus).GetUrl());
      _resolvedNodeIconWhite = new IconInfo (InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Image, c_nodeIconWhite).GetUrl());
    }

    public new HttpContextBase Context
    {
      get { return ((IControl) this).Page.Context; }
    }

    private IInfrastructureResourceUrlFactory InfrastructureResourceUrlFactory
    {
      get
      {
        if (_infrastructureResourceUrlFactory == null)
          _infrastructureResourceUrlFactory = SafeServiceLocator.Current.GetInstance<IInfrastructureResourceUrlFactory>();
        return _infrastructureResourceUrlFactory;
      }
    }

    /// <summary> Sets the selected tree node. </summary>
    internal void SetSelectedNode (WebTreeNode node)
    {
      if (node != null && node.TreeView != this)
        throw new InvalidOperationException ("Only tree nodes that are part of this tree can be selected.");
      if (_selectedNode != node)
      {
        if ((_selectedNode != null) && _selectedNode.IsSelected)
          _selectedNode.SetSelected (false);
        _selectedNode = node;
        if ((_selectedNode != null) && !_selectedNode.IsSelected)
          _selectedNode.SetSelected (true);
      }
    }

    /// <summary> Gets the tree nodes displayed by this tree view. </summary>
    [PersistenceMode (PersistenceMode.InnerProperty)]
    [ListBindable (false)]
    [MergableProperty (false)]
    //  Default category
    [Description ("The tree nodes displayed by this tree view.")]
    [DefaultValue ((string) null)]
    public virtual WebTreeNodeCollection Nodes
    {
      get
      {
        if (_isLoadControlStateCompleted)
          EnsureTreeNodesCreated();
        return _nodes;
      }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines whether to show the top level expander and automatically expand the 
    ///   child nodes if the expander is hidden.
    /// </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("If cleared, the top level expender will be hidden and the child nodes expanded for the top level nodes.")]
    [DefaultValue (true)]
    public bool EnableTopLevelExpander
    {
      get { return _enableTopLevelExpander; }
      set { _enableTopLevelExpander = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to evaluate the child nodes when expanding a tree node. </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("If set, the child nodes will be evaluated when a node is expanded.")]
    [DefaultValue (false)]
    public bool EnableLookAheadEvaluation
    {
      get { return _enableLookAheadEvaluation; }
      set { _enableLookAheadEvaluation = value; }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines whether to show scroll bars. Requires also a width for the tree view.
    /// </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("If set, the tree view shows srcoll bars. Requires a witdh in addition to this setting to actually enable the scrollbars.")]
    [DefaultValue (false)]
    public bool EnableScrollBars
    {
      get { return _enableScrollBars; }
      set { _enableScrollBars = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to enable word wrapping. </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("If set, word wrap will be enabled for the tree node's text.")]
    [DefaultValue (false)]
    public bool EnableWordWrap
    {
      get { return _enableWordWrap; }
      set { _enableWordWrap = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to show the connection lines between the nodes. </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("If cleared, the tree nodes will not be connected by lines.")]
    [DefaultValue (true)]
    public bool ShowLines
    {
      get { return _showLines; }
      set { _showLines = value; }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines whether the tree node's state information will be saved in the view state.
    /// </summary>
    /// <remarks>
    ///   If cleared, the tree view's owner control will have to save the <see cref="WebTreeNode.IsEvaluated"/> and
    ///   <see cref="WebTreeNode.IsExpanded"/> flags to provide a consistent user expierence.
    /// </remarks>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public bool EnableTreeNodeControlState
    {
      get { return _enableTreeNodeControlState; }
      set { _enableTreeNodeControlState = value; }
    }

    /// <summary>
    /// Gets or sets a flag that determines whether the post back from a node click must be executed synchronously when the tree is rendered within 
    /// an <see cref="UpdatePanel"/>.
    /// </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("True to require a synchronous postback for node clicks within Ajax Update Panels.")]
    [DefaultValue (false)]
    public bool RequiresSynchronousPostBack
    {
      get { return _requiresSynchronousPostBack; }
      set { _requiresSynchronousPostBack = value; }
    }

    /// <summary> Occurs when a node is clicked. </summary>
    [Category ("Action")]
    [Description ("Occurs when a node is clicked.")]
    public event WebTreeNodeClickEventHandler Click
    {
      add { Events.AddHandler (s_clickEvent, value); }
      remove { Events.RemoveHandler (s_clickEvent, value); }
    }

    /// <summary> Occurs when the selected node is changed. </summary>
    [Category ("Action")]
    [Description ("Occurs when the selected node is changed.")]
    public event WebTreeNodeEventHandler SelectionChanged
    {
      add { Events.AddHandler (s_selectionChangedEvent, value); }
      remove { Events.RemoveHandler (s_selectionChangedEvent, value); }
    }

    /// <summary> Gets the currently selected tree node. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public WebTreeNode SelectedNode
    {
      get
      {
        if (_isLoadControlStateCompleted)
          EnsureTreeNodesCreated();
        return _selectedNode;
      }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public WebTreeViewMenuItemProvider MenuItemProvider
    {
      get { return _menuItemProvider; }
      set { _menuItemProvider = value; }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public bool IsDesignMode
    {
      get { return ControlHelper.IsDesignMode (this); }
    }

    IPage IControl.Page
    {
      get
      {
        if (_page == null)
          _page = PageWrapper.CastOrCreate (Page);
        return _page;
      }
    }

    private bool IsTreeNodeReachable (WebTreeNode node)
    {
      node = node.ParentNode;
      bool isReachable = true;
      while (node != null)
      {
        if (!node.IsExpanded)
        {
          isReachable = false;
          break;
        }
        node = node.ParentNode;
      }
      return isReachable;
    }

    private void InitializeTreeNodeMenus (WebTreeNodeCollection nodes)
    {
      if (_menuItemProvider == null)
        return;

      for (int i = 0; i < nodes.Count; i++)
      {
        WebTreeNode node = nodes[i];
        InitializeTreeNodeMenu (node);
        if (node.IsExpanded)
          InitializeTreeNodeMenus (node.Children);
      }
    }

    private void InitializeTreeNodeMenu (WebTreeNode node)
    {
      if (_menuItemProvider == null)
        return;

      if (_menus.ContainsKey (node))
        return;

      DropDownMenu menu = new DropDownMenu();
      if (string.IsNullOrEmpty (node.MenuID))
      {
        node.MenuID = ID + "_Menu_" + _menuCounter;
        _menuCounter++;
      }
      menu.ID = node.MenuID;
      menu.MenuItems.AddRange (_menuItemProvider.InitalizeMenuItems (node));
      menu.EventCommandClick += Menu_EventCommandClick;
      menu.WxeFunctionCommandClick += Menu_WxeFunctionCommandClick;
      menu.Mode = MenuMode.ContextMenu;

      _menus.Add (node, menu);
      _menuPlaceHolder.Controls.Add (menu);
    }

    internal void EnsureTreeNodeMenuInitialized (WebTreeNode node)
    {
      if (!_hasTreeNodeMenusCreated)
        return;

      if (_menuItemProvider == null)
        return;

      if (IsTreeNodeReachable (node))
        InitializeTreeNodeMenu (node);
    }

    private void PreRenderTreeNodeMenus ()
    {
      string key = ClientID + "_BindContextMenus";

      DropDownMenu anyNodeContextMenu = null;
      if (_menus.Values.Count > 0)
        anyNodeContextMenu = _menus.Values.First (menu => (menu != null), () => new Exception());

      if (anyNodeContextMenu != null)
      {
        string script =
            string.Format (
                @"$(document).ready( function(){{ 
  $('#{0}').find('span.treeViewNodeHead, span.treeViewNodeHeadSelected').each(
    function() {{
      var menuID = $(this).attr('name');
      if (menuID != null && menuID.length > 0)
        {1}
    }}
  ); 
}} );",
                ClientID,
                anyNodeContextMenu.GetBindOpenEventScript ("this", "menuID", true));
        ((IControl) this).Page.ClientScript.RegisterStartupScriptBlock (this, typeof (WebTreeView), key, script);
      }

      List<WebTreeNode> unreachableNodes = new List<WebTreeNode>();
      foreach (KeyValuePair<WebTreeNode, DropDownMenu> entry in _menus)
      {
        if (IsTreeNodeReachable (entry.Key))
          PreRenderTreeNodeMenus (entry.Key, entry.Value.MenuItems);
        else
        {
          _menuPlaceHolder.Controls.Remove (entry.Value);
          unreachableNodes.Add (entry.Key);
        }
      }

      foreach (WebTreeNode node in unreachableNodes)
        _menus.Remove (node);
    }

    private void PreRenderTreeNodeMenus (WebTreeNode node, WebMenuItemCollection menuItems)
    {
      _menuItemProvider.PreRenderMenuItems (node, menuItems);
    }

    private void Menu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      DropDownMenu menu = (DropDownMenu) sender;
      foreach (KeyValuePair<WebTreeNode, DropDownMenu> entry in _menus)
      {
        if (entry.Value == menu)
        {
          _menuItemProvider.OnMenuItemEventCommandClick (e.Item, entry.Key);
          return;
        }
      }
    }

    private void Menu_WxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      DropDownMenu menu = (DropDownMenu) sender;
      foreach (KeyValuePair<WebTreeNode, DropDownMenu> entry in _menus)
      {
        if (entry.Value == menu)
        {
          _menuItemProvider.OnMenuItemWxeFunctionCommandClick (e.Item, entry.Key);
          return;
        }
      }
    }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return "WebTreeView"; }
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/> node. </summary>
    /// <remarks> Class: <c>treeViewNode</c> </remarks>
    protected virtual string CssClassNode
    {
      get { return "treeViewNode"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s node head. </summary>
    /// <remarks> Class: <c>treeViewNodeHead</c> </remarks>
    protected virtual string CssClassNodeHead
    {
      get { return "treeViewNodeHead"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s node head if it is selected. </summary>
    /// <remarks> Class: <c>treeViewNodeHeadSelected</c> </remarks>
    protected virtual string CssClassNodeHeadSelected
    {
      get { return "treeViewNodeHeadSelected"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s node children. </summary>
    /// <remarks> Class: <c>treeViewNodeChildren</c> </remarks>
    protected virtual string CssClassNodeChildren
    {
      get { return "treeViewNodeChildren"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s last node's children. </summary>
    /// <remarks> Class: <c>treeViewNodeChildrenNoLines</c> </remarks>
    protected virtual string CssClassNodeChildrenNoLines
    {
      get { return "treeViewNodeChildrenNoLines"; }
    }

    /// <summary> 
    ///   Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s top level node's children if the expander is 
    ///   hidden.
    /// </summary>
    /// <remarks> Class: <c>treeViewTopLevelNodeChildren</c> </remarks>
    protected virtual string CssClassTopLevelNodeChildren
    {
      get { return "treeViewTopLevelNodeChildren"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s root element. </summary>
    /// <remarks> Class: <c>treeViewRoot</c> </remarks>
    protected virtual string CssClassRoot
    {
      get { return "treeViewRoot"; }
    }

    #endregion
  }

  /// <summary>
  ///   Represents the method called before a <see cref="WebTreeNode"/> with <see cref="WebTreeNode.IsEvaluated"/>
  ///   set to <see langword="false"/> is expanded.
  /// </summary>
  public delegate void EvaluateWebTreeNode (WebTreeNode expandingNode);

  public delegate void InitializeRootWebTreeNodes ();

  public delegate void WebTreeNodeRenderMethod (HtmlTextWriter writer, WebTreeNode node);
}