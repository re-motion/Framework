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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Globalization;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.WebTreeViewImplementation;
using Remotion.Web.UI.Controls.WebTreeViewImplementation.Rendering;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary> A tree view. </summary>
  [ToolboxData("<{0}:WebTreeView runat=server></{0}:WebTreeView>")]
  [DefaultEvent("Click")]
  public class WebTreeView : WebControl, IWebTreeView, IPostBackEventHandler, IResourceDispatchTarget
  {
    // constants

    #region private const string c_nodeIcon...

    private const string c_nodeIconF = "sprite.svg#TreeViewF";
    private const string c_nodeIconFMinus = "sprite.svg#TreeViewFMinus";
    private const string c_nodeIconFPlus = "sprite.svg#TreeViewFPlus";
    private const string c_nodeIconI = "sprite.svg#TreeViewI";
    private const string c_nodeIconL = "sprite.svg#TreeViewL";
    private const string c_nodeIconLMinus = "sprite.svg#TreeViewLMinus";
    private const string c_nodeIconLPlus = "sprite.svg#TreeViewLPlus";
    private const string c_nodeIconMinus = "sprite.svg#TreeViewMinus";
    private const string c_nodeIconPlus = "sprite.svg#TreeViewPlus";
    private const string c_nodeIconR = "sprite.svg#TreeViewR";
    private const string c_nodeIconRMinus = "sprite.svg#TreeViewRMinus";
    private const string c_nodeIconRPlus = "sprite.svg#TreeViewRPlus";
    private const string c_nodeIconT = "sprite.svg#TreeViewT";
    private const string c_nodeIconTMinus = "sprite.svg#TreeViewTMinus";
    private const string c_nodeIconTPlus = "sprite.svg#TreeViewTPlus";
    private const string c_nodeIconWhite = "sprite.svg#TreeViewWhite";

    #endregion

    /// <summary> The separator used for the node path. </summary>
    private const char c_pathSeparator = '/';

    /// <summary> The prefix for the expansion command. </summary>
    private const string c_expansionCommandPrefix = "Expand=";

    /// <summary> The prefix for the click command. </summary>
    private const string c_clickCommandPrefix = "Click=";

    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.Web.Globalization.WebTreeView")]
    public enum ResourceIdentifier
    {
      /// <summary> Additional text for improved screen reader support in Internet Explorer.</summary>
      ScreenReaderNodeSelectedLabelText
    }

    // statics
    private static readonly object s_clickEvent = new object();
    private static readonly object s_selectionChangedEvent = new object();

    // fields
    // The URL resolved icon paths.

    #region private IconInfo _resolvedNodeIcon...

    private IconInfo? _resolvedNodeIconF;
    private IconInfo? _resolvedNodeIconFMinus;
    private IconInfo? _resolvedNodeIconFPlus;
    private IconInfo? _resolvedNodeIconI;
    private IconInfo? _resolvedNodeIconL;
    private IconInfo? _resolvedNodeIconLMinus;
    private IconInfo? _resolvedNodeIconLPlus;
    private IconInfo? _resolvedNodeIconMinus;
    private IconInfo? _resolvedNodeIconPlus;
    private IconInfo? _resolvedNodeIconR;
    private IconInfo? _resolvedNodeIconRMinus;
    private IconInfo? _resolvedNodeIconRPlus;
    private IconInfo? _resolvedNodeIconT;
    private IconInfo? _resolvedNodeIconTMinus;
    private IconInfo? _resolvedNodeIconTPlus;
    private IconInfo? _resolvedNodeIconWhite;

    #endregion

    /// <summary> The nodes in this tree view. </summary>
    private readonly WebTreeNodeCollection _nodes;

    private Triplet[]? _nodesControlState;
    private bool _isLoadControlStateCompleted;
    private bool _enableTopLevelExpander = true;
    private bool _enableLookAheadEvaluation;
    private bool _enableTopLevelGrouping;

    private bool _enableScrollBars;
    private bool _enableWordWrap;
    private bool _showLines = true;
    private bool _enableTreeNodeControlState = true;
    private bool _hasTreeNodesCreated;
    private bool _requiresSynchronousPostBack;
    private WebTreeNode? _selectedNode;
    private WebTreeNode? _focusededNode;
    private WebTreeViewMenuItemProvider? _menuItemProvider;
    private readonly Dictionary<WebTreeNode, DropDownMenu> _menus = new Dictionary<WebTreeNode, DropDownMenu>();
    private readonly PlaceHolder _menuPlaceHolder;
    private bool _hasTreeNodeMenusCreated;
    private int _menuCounter;
    private string? _assignedLabelID;

    private readonly IRenderingFeatures _renderingFeatures;

    /// <summary>
    ///   The delegate called before a node with <see cref="WebTreeNode.IsEvaluated"/> set to <see langword="false"/>
    ///   is expanded.
    /// </summary>
    private EvaluateWebTreeNode? _evaluateTreeNode;

    private InitializeRootWebTreeNodes? _initializeRootTreeNodes;
    private WebTreeNodeRenderMethod? _treeNodeRenderMethod;
    private WebTreeNodeMenuRenderMethod? _treeNodeMenuRenderMethod;
    private IPage? _page;
    private IInfrastructureResourceUrlFactory? _infrastructureResourceUrlFactory;
    private readonly ILabelReferenceRenderer _labelReferenceRenderer;
    private readonly IFallbackNavigationUrlProvider _fallbackNavigationUrlProvider;

    /// <summary> Caches the <see cref="ResourceManagerSet"/> for this <see cref="WebTreeView"/>. </summary>
    private ResourceManagerSet? _cachedResourceManager;

    //  construction and destruction

    /// <summary> Initalizes a new instance. </summary>
    public WebTreeView (IControl? ownerControl)
    {
      _nodes = new WebTreeNodeCollection(ownerControl);
      _nodes.SetParent(this, null);
      _menuPlaceHolder = new PlaceHolder();
      _renderingFeatures = SafeServiceLocator.Current.GetInstance<IRenderingFeatures>();
      _fallbackNavigationUrlProvider = SafeServiceLocator.Current.GetInstance<IFallbackNavigationUrlProvider>();
      _labelReferenceRenderer = SafeServiceLocator.Current.GetInstance<ILabelReferenceRenderer>();
    }

    /// <summary> Initalizes a new instance. </summary>
    public WebTreeView ()
        : this(null)
    {
    }

    protected override void CreateChildControls ()
    {
      base.CreateChildControls();

      _menuPlaceHolder.ID = ID + "_Menus";
      _menuPlaceHolder.EnableViewState = false;
      Controls.Add(_menuPlaceHolder);
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
      ArgumentUtility.CheckNotNullOrEmpty("eventArgument", eventArgument);
      EnsureTreeNodesCreated();

      eventArgument = eventArgument.Trim();
      if (eventArgument.StartsWith(c_expansionCommandPrefix))
        HandleExpansionCommandEvent(eventArgument.Substring(c_expansionCommandPrefix.Length));
      else if (eventArgument.StartsWith(c_clickCommandPrefix))
        HandleClickCommandEvent(eventArgument.Substring(c_clickCommandPrefix.Length));
      else
        throw new ArgumentException("Argument 'eventArgument' has unknown prefix: '" + eventArgument + "'.");
    }

    /// <summary> Handles the expansion command (i.e. expands/collapses the clicked tree node). </summary>
    /// <param name="eventArgument"> The path to the clicked tree node. </param>
    private void HandleExpansionCommandEvent (string eventArgument)
    {
      string[] pathSegments;
      WebTreeNode? clickedNode = ParseNodePath(eventArgument, out pathSegments);
      if (clickedNode != null)
      {
        _focusededNode = clickedNode;
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
          InitializeTreeNodeMenus(clickedNode.Children);
      }
    }

    /// <summary> Handles the click command. </summary>
    /// <param name="eventArgument"> The path to the clicked tree node. </param>
    private void HandleClickCommandEvent (string eventArgument)
    {
      string[] pathSegments;
      WebTreeNode? clickedNode = ParseNodePath(eventArgument, out pathSegments);
      bool isSelectionChanged = _selectedNode != clickedNode;
      SetSelectedNode(clickedNode);
      OnClick(clickedNode, pathSegments);
      if (isSelectionChanged)
        OnSelectionChanged(clickedNode);
    }

    /// <summary> Fires the <see cref="Click"/> event. </summary>
    protected virtual void OnClick (WebTreeNode? node, string[] path)
    {
      WebTreeNodeClickEventHandler? handler = (WebTreeNodeClickEventHandler?)Events[s_clickEvent];
      if (handler != null)
      {
        WebTreeNodeClickEventArgs e = new WebTreeNodeClickEventArgs(node, path);
        handler(this, e);
      }
    }

    /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
    protected virtual void OnSelectionChanged (WebTreeNode? node)
    {
      WebTreeNodeEventHandler? handler = (WebTreeNodeEventHandler?)Events[s_selectionChangedEvent];
      if (handler != null)
      {
        WebTreeNodeEventArgs e = new WebTreeNodeEventArgs(node);
        handler(this, e);
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

    public void SetTreeNodeMenuRenderMethodDelegate (WebTreeNodeMenuRenderMethod treeNodeMenuRenderMethod)
    {
      _treeNodeMenuRenderMethod = treeNodeMenuRenderMethod;
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
        LoadNodesControlStateRecursive(_nodesControlState, _nodes);

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
      ArgumentUtility.CheckNotNull("node", node);

      if (_evaluateTreeNode == null)
        throw new NullReferenceException("EvaluateTreeNode has no method registered but tree node '" + node.ItemID + "' is not evaluated.");
      _evaluateTreeNode(node);
      if (!node.IsEvaluated)
        throw new InvalidOperationException("EvaluateTreeNode called for tree node '" + node.ItemID + "' but did not evaluate the tree node.");
    }

    public void ResetNodes ()
    {
      Nodes.Clear();
      _menuPlaceHolder.Controls.Clear();
      _menus.Clear();
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      Page!.RegisterRequiresControlState(this);
      if (Page != null && !Page.IsPostBack)
        _isLoadControlStateCompleted = true;

      RegisterHtmlHeadContents(HtmlHeadAppender.Current, Context);

      ScriptUtility.Instance.RegisterJavaScriptInclude(this, HtmlHeadAppender.Current);
    }


    protected override void LoadControlState (object? savedState)
    {
      object?[] values = (object?[])savedState!;

      base.LoadControlState(values[0]);
      if (_enableTreeNodeControlState)
        _nodesControlState = (Triplet[])values[1]!;
      else
        _nodesControlState = null;
      _menuCounter = (int)values[2]!;

      _isLoadControlStateCompleted = true;
    }

    protected override object SaveControlState ()
    {
      object?[] values = new object?[3];

      values[0] = base.SaveControlState();
      if (_enableTreeNodeControlState)
        values[1] = SaveNodesControlStateRecursive(_nodes);
      values[2] = _menuCounter;

      return values;
    }

    /// <summary> Loads the settings of the <paramref name="nodes"/> from <paramref name="nodesState"/>. </summary>
    private void LoadNodesControlStateRecursive (Triplet[] nodesState, WebTreeNodeCollection nodes)
    {
      for (int i = 0; i < nodesState.Length; i++)
      {
        Triplet nodeState = nodesState[i];
        string nodeID = (string)nodeState.First!; // TODO RM-8118: not null assertion
        WebTreeNode? node = nodes.Find(nodeID);
        if (node != null)
        {
          object[] values = (object[])nodeState.Second!; // TODO RM-8118: not null assertion
          node.IsExpanded = (bool)values[0];
          if (!node.IsEvaluated)
          {
            bool isEvaluated = (bool)values[1];
            if (isEvaluated)
              EvaluateTreeNodeInternal(node);
          }
          bool isSelected = (bool)values[2];
          if (isSelected)
            node.IsSelected = true;
          node.MenuID = (string)values[3];
          LoadNodesControlStateRecursive((Triplet[])nodeState.Third!, node.Children); // TODO RM-8118: not null assertion
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
        nodeControlState.Third = SaveNodesControlStateRecursive(node.Children);
        nodesState[i] = nodeControlState;
      }
      return nodesState;
    }


    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    void IResourceDispatchTarget.Dispatch (IDictionary<string, WebString> values)
    {
      ArgumentUtility.CheckNotNull("values", values);
      Dispatch(values);
    }

    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    protected virtual void Dispatch (IDictionary<string, WebString> values)
    {
      //  Dispatch simple properties
      ResourceDispatcher.DispatchGeneric(this, values);
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected virtual void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      string? key = ResourceManagerUtility.GetGlobalResourceKey(AccessKey);
      if (!string.IsNullOrEmpty(key))
        AccessKey = resourceManager.GetString(key);

      key = ResourceManagerUtility.GetGlobalResourceKey(ToolTip);
      if (!string.IsNullOrEmpty(key))
        ToolTip = resourceManager.GetString(key);

      Nodes.LoadResources(resourceManager, globalizationService);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      EnsureTreeNodesCreated();
      EnsureChildControls();
      InitializeTreeNodeMenus(_nodes);
      _hasTreeNodeMenusCreated = true;
    }

    public virtual void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, HttpContextBase? httpContext)
    {
      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender);
    }

    protected virtual IWebTreeViewRenderer CreateRenderer ()
    {
      return SafeServiceLocator.Current.GetInstance<IWebTreeViewRenderer>();
    }

    /// <summary> Overrides the parent control's <c>OnPreRender</c> method. </summary>
    protected override void OnPreRender (EventArgs e)
    {
      EnsureTreeNodesCreated();

      base.OnPreRender(e);

      var resourceManager = ResourceManagerUtility.GetResourceManager(this, true);
      var globalizationService = SafeServiceLocator.Current.GetInstance<IGlobalizationService>();

      LoadResources(resourceManager, globalizationService);

      if (_requiresSynchronousPostBack)
      {
        var scriptManager = ScriptManager.GetCurrent(Page!);
        if (scriptManager != null)
        {
          bool hasUpdatePanelAsParent = false;
          for (Control? current = Parent; current != null && !(current is Page); current = current.Parent)
          {
            if (current is UpdatePanel)
            {
              hasUpdatePanelAsParent = true;
              break;
            }
          }

          if (hasUpdatePanelAsParent)
          {
            ISmartPage? smartPage = Page as ISmartPage;
            if (smartPage == null)
            {
              throw new InvalidOperationException(
                  string.Format(
                      "{0}: WebTreeViews with RequiresSynchronousPostBack set to true are only supported on pages implementing ISmartPage when used within an UpdatePanel.",
                      ID));
            }
            RegisterTreeNodesForSynchronousPostback(smartPage, _nodes);
          }
        }
      }

      PreRenderTreeNodeMenus();
    }

    private void RegisterTreeNodesForSynchronousPostback (ISmartPage smartPage, WebTreeNodeCollection nodes)
    {
      foreach (WebTreeNode node in nodes)
      {
        smartPage.RegisterCommandForSynchronousPostBack(this, GetClickCommandArgument(FormatNodePath(node)));
        RegisterTreeNodesForSynchronousPostback(smartPage, node.Children);
      }
    }

    /// <summary> Overrides the parent control's <c>TagKey</c> property. </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Div; }
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      base.AddAttributesToRender(writer);
      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        IControlWithDiagnosticMetadata controlWithDiagnosticMetadata = this;
        writer.AddAttribute(DiagnosticMetadataAttributes.ControlType, controlWithDiagnosticMetadata.ControlType);
      }
    }

    protected override void Render (HtmlTextWriter writer)
    {
      var cssClassBackup = CssClass;

      if (_enableScrollBars)
        CssClass += " " + CssClassDefinition.Themed + " " + CssClassDefinition.Scrollable;

      base.Render(writer);

      CssClass = cssClassBackup;
    }

    /// <summary> Overrides the parent control's <c>RenderContents</c> method. </summary>
    protected override void RenderContents (HtmlTextWriter writer)
    {
      ((IControl)this).Page!.ClientScript.RegisterStartupScriptBlock(
          this,
          typeof(WebTreeView),
          Guid.NewGuid().ToString(),
          string.Format("WebTreeView.Initialize ('#{0}');", ClientID));

      ResolveNodeIcons();

      var rootCssClass = CssClassRoot;
      if (!_enableWordWrap)
        rootCssClass += " whitespaceNoWrap";

      writer.AddAttribute(HtmlTextWriterAttribute.Class, rootCssClass);
      writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Tree);

      var labelIDs = GetLabelIDs().ToArray();
      _labelReferenceRenderer.AddLabelsReference(writer, labelIDs);

      writer.RenderBeginTag(HtmlTextWriterTag.Ul); // Begin child nodes
      if (_focusededNode == null)
        _focusededNode = _selectedNode;
      using (var nodeIDAlgorithm = CreateNodeIDAlgorithm())
      {
        RenderNodes(writer, _nodes, true, nodeIDAlgorithm, 0);
      }
      writer.RenderEndTag();
      foreach (DropDownMenu menu in _menuPlaceHolder.Controls)
        menu.RenderAsContextMenu(writer);
    }

    /// <summary> Renders the <paremref name="nodes"/> onto the <paremref name="writer"/>. </summary>
    private void RenderNodes (HtmlTextWriter writer, WebTreeNodeCollection webTreeNodes, bool isTopLevel, HashAlgorithm nodeIDAlgorithm, int nestingDepth)
    {
      var nodes = EnableTopLevelGrouping && isTopLevel
          ? WebTreeNodeCollection.GroupByCategory(webTreeNodes)
          : webTreeNodes.Cast<WebTreeNode>().ToArray();

      for (int i = 0; i < nodes.Count; i++)
      {
        WebTreeNode node = nodes[i];
        bool isFirstNode = i == 0;
        bool isLastNode = i + 1 == nodes.Count;
        bool hasChildren = node.Children.Count > 0;
        bool hasExpander = !isTopLevel || _enableTopLevelExpander;
        if (!hasExpander)
          node.IsExpanded = true;
        string nodePath = FormatNodePath(node);
        string nodeID = CreateNodeID(nodeIDAlgorithm, nodePath);

        if (_renderingFeatures.EnableDiagnosticMetadata)
        {
          if (!string.IsNullOrEmpty(node.ItemID))
            writer.AddAttribute(DiagnosticMetadataAttributes.ItemID, node.ItemID);
          if (!node.Text.IsEmpty)
            HtmlUtility.ExtractPlainText(node.Text).AddAttributeTo(writer, DiagnosticMetadataAttributes.Content);
          if (node.IsSelected)
            writer.AddAttribute(DiagnosticMetadataAttributes.WebTreeViewIsSelectedNode, "true");
          if (node.IsEvaluated)
            writer.AddAttribute(DiagnosticMetadataAttributes.WebTreeViewNumberOfChildren, node.Children.Count.ToString());
          else
            writer.AddAttribute(DiagnosticMetadataAttributes.WebTreeViewNumberOfChildren, DiagnosticMetadataAttributes.Null);
          writer.AddAttribute(DiagnosticMetadataAttributes.WebTreeViewIsExpanded, node.IsExpanded ? "true" : "false");
          writer.AddAttribute(DiagnosticMetadataAttributes.IndexInCollection, (i + 1).ToString());
          if (node.Badge is { Value: { IsEmpty: false } })
          {
            HtmlUtility.ExtractPlainText(node.Badge.Value).AddAttributeTo(writer, DiagnosticMetadataAttributes.WebTreeViewBadgeValue);
            if (!node.Badge.Description.IsEmpty)
              node.Badge.Description.AddAttributeTo(writer, DiagnosticMetadataAttributes.WebTreeViewBadgeDescription);
          }
          if (!string.IsNullOrEmpty(node.Category))
            writer.AddAttribute(DiagnosticMetadataAttributes.WebTreeViewNodeCategory, node.Category);
        }

        writer.AddAttribute(HtmlTextWriterAttribute.Id, "Node_" + nodeID);

        if (EnableTopLevelGrouping && isTopLevel && !isLastNode && nodes[i].Category != nodes[i + 1].Category)
          writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassNodeCategorySeparator);

        writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.None);

        writer.RenderBeginTag(HtmlTextWriterTag.Li); // Begin node block

        if (!isTopLevel && !string.IsNullOrEmpty(node.Category))
          throw new InvalidOperationException("Only root nodes may have a category set.");

        RenderNodeHead(writer, node, isFirstNode, isLastNode, hasExpander, node.IsSelected, nodePath, nodeID, nestingDepth);
        if (hasChildren && node.IsExpanded)
          RenderNodeChildren(writer, node, isLastNode, hasExpander, nodeID, nodeIDAlgorithm, nestingDepth);

        writer.RenderEndTag(); // End node block
      }
    }

    /// <summary> Renders the <paramref name="node"/> onto the <paremref name="writer"/>. </summary>
    private void RenderNodeHead (
        HtmlTextWriter writer,
        WebTreeNode node,
        bool isFirstNode,
        bool isLastNode,
        bool hasExpander,
        bool isSelected,
        string nodePath,
        string nodeID,
        int nestingDepth)
    {
      bool isMenuVisible = false;
      if (_menus.TryGetValue(node, out var menu))
      {
        if (_treeNodeMenuRenderMethod != null)
          _treeNodeMenuRenderMethod(writer, node, menu);

        for (int i = 0; i < menu.MenuItems.Count; i++)
        {
          if (menu.MenuItems[i].IsVisible)
          {
            isMenuVisible = true;
            break;
          }
        }
      }

      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassNode);
      writer.AddStyleAttribute("--nesting-depth", nestingDepth.ToString());
      writer.RenderBeginTag(HtmlTextWriterTag.Span);

      if (hasExpander)
        RenderNodeExpander(writer, node, nodePath, nodeID, isFirstNode, isLastNode);

      if (isMenuVisible)
      {
        writer.AddAttribute("oncontextmenu", "return false;");
        writer.AddAttribute("id", menu!.ClientID);
      }
      RenderNodeLabel(writer, node, nodePath, nodeID);

      if (isSelected)
      {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassScreenReaderText);
        writer.RenderBeginTag(HtmlTextWriterTag.Span);

        var resourceManager= GetResourceManager();
        resourceManager.GetText(ResourceIdentifier.ScreenReaderNodeSelectedLabelText).WriteTo(writer);

        writer.RenderEndTag();
      }

      RenderNodeBadge(writer, node);

      writer.RenderEndTag();
    }

    /// <summary> Renders the <paramref name="node"/>'s expander (i.e. +/-) onto the <paremref name="writer"/>. </summary>
    private void RenderNodeExpander (
        HtmlTextWriter writer,
        WebTreeNode node,
        string nodePath,
        string nodeID,
        bool isFirstNode,
        bool isLastNode)
    {
      IconInfo nodeIcon = GetNodeIcon(node, isFirstNode, isLastNode)!;
      bool hasChildren = node.Children.Count > 0;
      bool isEvaluated = node.IsEvaluated;
      bool hasExpansionLink = hasChildren || !isEvaluated;
      if (hasExpansionLink)
      {
        string argument = c_expansionCommandPrefix + nodePath;
        string postBackEventReference = Page!.ClientScript.GetPostBackEventReference(this, argument) + ";return false;";
        writer.AddAttribute(HtmlTextWriterAttribute.Onclick, postBackEventReference);
        writer.AddAttribute(HtmlTextWriterAttribute.Href, _fallbackNavigationUrlProvider.GetURL());
        if (_renderingFeatures.EnableDiagnosticMetadata)
          writer.AddAttribute(DiagnosticMetadataAttributes.TriggersPostBack, "true");
      }
      writer.AddAttribute("tabindex", "-1");
      writer.AddAttribute(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);
      writer.RenderBeginTag(HtmlTextWriterTag.A);

      nodeIcon.Render(writer, this);
      writer.RenderEndTag();
    }

    /// <summary> Renders the <paramref name="node"/>'s head (i.e. icon and text) onto the <paremref name="writer"/>. </summary>
    private void RenderNodeLabel (HtmlTextWriter writer, WebTreeNode node, string nodePath, string nodeID)
    {
      if (node.IsSelected)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassNodeHeadSelected);
      else
        writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassNodeHead);

      if (!node.ToolTip.IsEmpty)
        node.ToolTip.AddAttributeTo(writer, HtmlTextWriterAttribute.Title);

      writer.RenderBeginTag(HtmlTextWriterTag.Span);

      string argument = GetClickCommandArgument(nodePath);
      string postBackEventReference = Page!.ClientScript.GetPostBackEventReference(this, argument) + ";return false;";
      writer.AddAttribute(HtmlTextWriterAttribute.Onclick, postBackEventReference);
      writer.AddAttribute(HtmlTextWriterAttribute.Href, _fallbackNavigationUrlProvider.GetURL());
      if (_renderingFeatures.EnableDiagnosticMetadata)
        writer.AddAttribute(DiagnosticMetadataAttributes.TriggersPostBack, "true");
      writer.AddAttribute(HtmlTextWriterAttribute.Id, "Head_" + nodeID);
      writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.TreeItem);
      writer.AddAttribute("tabindex", _focusededNode == node ? "0" : "-1");

      if(node.IsSelected)
        writer.AddAttribute(HtmlTextWriterAttribute2.AriaSelected, HtmlAriaSelectedAttributeValue.True);

      bool hasChildren = node.Children.Count > 0;
      bool isEvaluated = node.IsEvaluated;
      if (hasChildren || !isEvaluated)
      {
        writer.AddAttribute(
            HtmlTextWriterAttribute2.AriaExpanded,
            node.IsExpanded ? HtmlAriaExpandedAttributeValue.True : HtmlAriaExpandedAttributeValue.False);
      }
      if (node.IsExpanded)
        writer.AddAttribute(HtmlTextWriterAttribute2.AriaOwns, CreateNodeChildrenID(nodeID));

      writer.RenderBeginTag(HtmlTextWriterTag.A);
      if (_treeNodeRenderMethod == null)
      {
        if (node.Icon != null && node.Icon.HasRenderingInformation)
        {
          node.Icon.Render(writer, this);
          writer.Write("&nbsp;");
        }
        if (!node.Text.IsEmpty)
        {
          writer.RenderBeginTag(HtmlTextWriterTag.Span);
          node.Text.WriteTo(writer);
          writer.RenderEndTag();
        }

        RenderNodeLabelScreenReaderText(writer, node);
      }
      else
        _treeNodeRenderMethod.Invoke(writer, node);
      writer.RenderEndTag();

      writer.RenderEndTag();
    }

    /// <summary> Renders the <paramref name="node"/>'s screenReader text (if there is one) onto the <paremref name="writer"/>. </summary>
    private void RenderNodeLabelScreenReaderText (HtmlTextWriter writer, WebTreeNode node)
    {
      var badge = node.Badge;
      if (badge == null || badge.Value.IsEmpty || badge.Description.IsEmpty)
        return;

      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassScreenReaderText);
      writer.RenderBeginTag(HtmlTextWriterTag.Span);
      writer.Write(".");
      badge.Description.WriteTo(writer);
      writer.RenderEndTag();
    }

    /// <summary> Renders the <paramref name="node"/>'s badge (if there is one) onto the <paremref name="writer"/>. </summary>
    private void RenderNodeBadge (HtmlTextWriter writer, WebTreeNode node)
    {
      var badge = node.Badge;
      if (badge == null || badge.Value.IsEmpty)
        return;

      badge.Description.AddAttributeTo(writer, HtmlTextWriterAttribute.Title);
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassNodeBadge);
      //The screenreader text has been moved  inside the <a> element, as that is the treeitem which is read out to the screenreader.
      writer.AddAttribute(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);
      writer.RenderBeginTag(HtmlTextWriterTag.Span);

      writer.RenderBeginTag(HtmlTextWriterTag.Span);
      badge.Value.WriteTo(writer);
      writer.RenderEndTag();

      writer.RenderEndTag();
    }

    private string GetClickCommandArgument (string nodePath)
    {
      return c_clickCommandPrefix + nodePath;
    }

    /// <summary> Renders the <paramref name="node"/>'s children onto the <paremref name="writer"/>. </summary>
    private void RenderNodeChildren (HtmlTextWriter writer, WebTreeNode node, bool isLastNode, bool hasExpander, string nodeID, HashAlgorithm nodeIDAlgorithm, int nestingDepth)
    {
      if (!hasExpander)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTopLevelNodeChildren);
      else if (isLastNode || !_showLines)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassNodeChildrenNoLines);
      else
        writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassNodeChildren);
      writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Group);
      writer.AddAttribute(HtmlTextWriterAttribute.Id, CreateNodeChildrenID(nodeID));
      writer.RenderBeginTag(HtmlTextWriterTag.Ul); // Begin child nodes

      RenderNodes(writer, node.Children, false, nodeIDAlgorithm, nestingDepth + 1);

      writer.RenderEndTag(); // End child nodes
    }

    private string CreateNodeChildrenID (string nodeID)
    {
      return $"Node_{nodeID}_Children";
    }

    /// <summary> Renders a dummy tree for design mode. </summary>
    private void RenderDesignModeContents (HtmlTextWriter writer)
    {
      WebTreeNodeCollection designModeNodes = new WebTreeNodeCollection(null);
      designModeNodes.SetParent(this, null);
      WebTreeNodeCollection nodes = designModeNodes;
      nodes.Add(new WebTreeNode("node0", WebString.CreateFromText("Node 0")));
      nodes.Add(new WebTreeNode("node1", WebString.CreateFromText("Node 1")));
      nodes.Add(new WebTreeNode("node2", WebString.CreateFromText("Node 2")));
      using (var nodeIDAlgorithm = CreateNodeIDAlgorithm())
      {
        RenderNodes(writer, designModeNodes, true, nodeIDAlgorithm, 0);
      }
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this <see cref="WebTreeView"/>. </summary>
    protected IResourceManager GetResourceManager ()
    {
      //  Provider has already been identified.
      if (_cachedResourceManager != null)
        return _cachedResourceManager;

      //  Get the resource managers

      IResourceManager localResourceManager = GlobalizationService.GetResourceManager(typeof(ResourceIdentifier));
      IResourceManager namingContainerResourceManager = ResourceManagerUtility.GetResourceManager(NamingContainer, true);
      _cachedResourceManager = ResourceManagerSet.Create(namingContainerResourceManager, localResourceManager);

      return _cachedResourceManager;
    }

    private IGlobalizationService GlobalizationService
    {
      get { return SafeServiceLocator.Current.GetInstance<IGlobalizationService>(); }
    }

    private HashAlgorithm CreateNodeIDAlgorithm ()
    {
      return Assertion.IsNotNull(MD5.Create(), "HashAlgorithm.Create('MD5') != null");
    }

    private string CreateNodeID (HashAlgorithm nodeIDAlgorithm, string nodePath)
    {
      return BitConverter.ToString(nodeIDAlgorithm.ComputeHash(Encoding.Unicode.GetBytes(ClientID + ":" + nodePath))).Replace("-", "");
    }

    /// <summary> Generates the string representation of the <paramref name="node"/>'s path. </summary>
    /// <remarks> ...&lt;node.Parent.Parent.ItemID&gt;|&lt;node.Parent.ItemID&gt;|&lt;ItemID&gt; </remarks>
    public string FormatNodePath (WebTreeNode? node)
    {
      if (node == null)
        return string.Empty;

      string parentPath = string.Empty;
      if (node.ParentNode != null)
      {
        parentPath = FormatNodePath(node.ParentNode);
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
    public WebTreeNode? ParseNodePath (string path, out string[] pathSegments)
    {
      pathSegments = path.Split(c_pathSeparator);
      WebTreeNode? currentNode = null;
      WebTreeNodeCollection currentNodes = _nodes;
      for (int i = 0; i < pathSegments.Length; i++)
      {
        string nodeID = pathSegments[i];
        WebTreeNode? node = currentNodes.Find(nodeID);
        if (node == null)
          return currentNode;
        currentNode = node;
        currentNodes = currentNode.Children;
      }
      return currentNode;
    }

    /// <summary> Returns the URL of the node icon for the <paramref name="node"/>. </summary>
    private IconInfo? GetNodeIcon (WebTreeNode node, bool isFirstNode, bool isLastNode)
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
    [MemberNotNull(nameof(_resolvedNodeIconF))]
    [MemberNotNull(nameof(_resolvedNodeIconFMinus))]
    [MemberNotNull(nameof(_resolvedNodeIconFPlus))]
    [MemberNotNull(nameof(_resolvedNodeIconI))]
    [MemberNotNull(nameof(_resolvedNodeIconL))]
    [MemberNotNull(nameof(_resolvedNodeIconLMinus))]
    [MemberNotNull(nameof(_resolvedNodeIconLPlus))]
    [MemberNotNull(nameof(_resolvedNodeIconMinus))]
    [MemberNotNull(nameof(_resolvedNodeIconPlus))]
    [MemberNotNull(nameof(_resolvedNodeIconR))]
    [MemberNotNull(nameof(_resolvedNodeIconRMinus))]
    [MemberNotNull(nameof(_resolvedNodeIconRPlus))]
    [MemberNotNull(nameof(_resolvedNodeIconT))]
    [MemberNotNull(nameof(_resolvedNodeIconTMinus))]
    [MemberNotNull(nameof(_resolvedNodeIconTPlus))]
    [MemberNotNull(nameof(_resolvedNodeIconWhite))]
    private void ResolveNodeIcons ()
    {
      _resolvedNodeIconF = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconF).GetUrl());
      _resolvedNodeIconFMinus = new IconInfo(
          InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconFMinus).GetUrl());
      _resolvedNodeIconFPlus = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconFPlus).GetUrl());
      _resolvedNodeIconI = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconI).GetUrl());
      _resolvedNodeIconL = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconL).GetUrl());
      _resolvedNodeIconLMinus = new IconInfo(
          InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconLMinus).GetUrl());
      _resolvedNodeIconLPlus = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconLPlus).GetUrl());
      _resolvedNodeIconMinus = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconMinus).GetUrl());
      _resolvedNodeIconPlus = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconPlus).GetUrl());
      _resolvedNodeIconR = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconR).GetUrl());
      _resolvedNodeIconRMinus = new IconInfo(
          InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconRMinus).GetUrl());
      _resolvedNodeIconRPlus = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconRPlus).GetUrl());
      _resolvedNodeIconT = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconT).GetUrl());
      _resolvedNodeIconTMinus = new IconInfo(
          InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconTMinus).GetUrl());
      _resolvedNodeIconTPlus = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconTPlus).GetUrl());
      _resolvedNodeIconWhite = new IconInfo(InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_nodeIconWhite).GetUrl());
    }

    public new HttpContextBase? Context
    {
      get { return ((IControl)this).Page!.Context; }
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
    internal void SetSelectedNode (WebTreeNode? node)
    {
      if (node != null && node.TreeView != this)
        throw new InvalidOperationException("Only tree nodes that are part of this tree can be selected.");
      if (_selectedNode != node)
      {
        if ((_selectedNode != null) && _selectedNode.IsSelected)
          _selectedNode.SetSelected(false);
        _selectedNode = node;
        if ((_selectedNode != null) && !_selectedNode.IsSelected)
          _selectedNode.SetSelected(true);
      }
    }

    /// <summary> Gets the tree nodes displayed by this tree view. </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [ListBindable(false)]
    [MergableProperty(false)]
    //  Default category
    [Description("The tree nodes displayed by this tree view.")]
    [DefaultValue((string?)null)]
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
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("If cleared, the top level expender will be hidden and the child nodes expanded for the top level nodes.")]
    [DefaultValue(true)]
    public bool EnableTopLevelExpander
    {
      get { return _enableTopLevelExpander; }
      set { _enableTopLevelExpander = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to evaluate the child nodes when expanding a tree node. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("If set, the child nodes will be evaluated when a node is expanded.")]
    [DefaultValue(false)]
    public bool EnableLookAheadEvaluation
    {
      get { return _enableLookAheadEvaluation; }
      set { _enableLookAheadEvaluation = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to group the root nodes by their category. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("If set, the root nodes will be grouped by their category attribute. The order of the child nodes will remains unchanged.")]
    [DefaultValue(false)]
    public bool EnableTopLevelGrouping
    {
      get { return _enableTopLevelGrouping; }
      set { _enableTopLevelGrouping = value; }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines whether to show scroll bars. Requires also a width for the tree view.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("If set, the tree view shows srcoll bars. Requires a witdh in addition to this setting to actually enable the scrollbars.")]
    [DefaultValue(false)]
    public bool EnableScrollBars
    {
      get { return _enableScrollBars; }
      set { _enableScrollBars = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to enable word wrapping. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Appearance")]
    [Description("If set, word wrap will be enabled for the tree node's text.")]
    [DefaultValue(false)]
    public bool EnableWordWrap
    {
      get { return _enableWordWrap; }
      set { _enableWordWrap = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to show the connection lines between the nodes. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Appearance")]
    [Description("If cleared, the tree nodes will not be connected by lines.")]
    [DefaultValue(true)]
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
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public bool EnableTreeNodeControlState
    {
      get { return _enableTreeNodeControlState; }
      set { _enableTreeNodeControlState = value; }
    }

    /// <summary>
    /// Gets or sets a flag that determines whether the post back from a node click must be executed synchronously when the tree is rendered within 
    /// an <see cref="UpdatePanel"/>.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("True to require a synchronous postback for node clicks within Ajax Update Panels.")]
    [DefaultValue(false)]
    public bool RequiresSynchronousPostBack
    {
      get { return _requiresSynchronousPostBack; }
      set { _requiresSynchronousPostBack = value; }
    }

    /// <summary> Occurs when a node is clicked. </summary>
    [Category("Action")]
    [Description("Occurs when a node is clicked.")]
    public event WebTreeNodeClickEventHandler Click
    {
      add { Events.AddHandler(s_clickEvent, value); }
      remove { Events.RemoveHandler(s_clickEvent, value); }
    }

    /// <summary> Occurs when the selected node is changed. </summary>
    [Category("Action")]
    [Description("Occurs when the selected node is changed.")]
    public event WebTreeNodeEventHandler SelectionChanged
    {
      add { Events.AddHandler(s_selectionChangedEvent, value); }
      remove { Events.RemoveHandler(s_selectionChangedEvent, value); }
    }

    /// <summary> Gets the currently selected tree node. </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public WebTreeNode? SelectedNode
    {
      get
      {
        if (_isLoadControlStateCompleted)
          EnsureTreeNodesCreated();
        return _selectedNode;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public WebTreeViewMenuItemProvider? MenuItemProvider
    {
      get { return _menuItemProvider; }
      set { _menuItemProvider = value; }
    }

    IPage? IControl.Page
    {
      get
      {
        if (_page == null)
          _page = PageWrapper.CastOrCreate(Page);
        return _page;
      }
    }

    public void AssignLabel (string labelID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("labelID", labelID);

      _assignedLabelID = labelID;
    }

    protected virtual IEnumerable<string> GetLabelIDs ()
    {
      if (string.IsNullOrEmpty(_assignedLabelID))
        return Enumerable.Empty<string>();
      return EnumerableUtility.Singleton(_assignedLabelID);
    }

    IEnumerable<string> IControlWithLabel.GetLabelIDs ()
    {
      return GetLabelIDs();
    }

    private bool IsTreeNodeReachable (WebTreeNode node)
    {
      node = node.ParentNode!;
      bool isReachable = true;
      while (node != null) // TODO RM-8118: don't reuse variable
      {
        if (!node.IsExpanded)
        {
          isReachable = false;
          break;
        }
        node = node.ParentNode!;
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
        InitializeTreeNodeMenu(node);
        if (node.IsExpanded)
          InitializeTreeNodeMenus(node.Children);
      }
    }

    private void InitializeTreeNodeMenu (WebTreeNode node)
    {
      if (_menuItemProvider == null)
        return;

      if (_menus.ContainsKey(node))
        return;

      DropDownMenu menu = new DropDownMenu();
      if (string.IsNullOrEmpty(node.MenuID))
      {
        node.MenuID = ID + "_Menu_" + _menuCounter;
        _menuCounter++;
      }
      menu.ID = node.MenuID;
      menu.MenuItems.AddRange(_menuItemProvider.InitalizeMenuItems(node));
      menu.EventCommandClick += Menu_EventCommandClick;
      menu.WxeFunctionCommandClick += Menu_WxeFunctionCommandClick;
      menu.Mode = MenuMode.ContextMenu;

      _menus.Add(node, menu);
      _menuPlaceHolder.Controls.Add(menu);
    }

    internal void EnsureTreeNodeMenuInitialized (WebTreeNode node)
    {
      if (!_hasTreeNodeMenusCreated)
        return;

      if (_menuItemProvider == null)
        return;

      if (IsTreeNodeReachable(node))
        InitializeTreeNodeMenu(node);
    }

    private void PreRenderTreeNodeMenus ()
    {
      string key = ClientID + "_BindContextMenus";

      DropDownMenu? anyNodeContextMenu = null;
      if (_menus.Values.Count > 0)
        anyNodeContextMenu = _menus.Values.First(menu => (menu != null), () => new Exception());

      if (anyNodeContextMenu != null)
      {
        string script =
            string.Format(
                @"document.getElementById('{0}').querySelectorAll('span.treeViewNodeHead, span.treeViewNodeHeadSelected').forEach(
  el => {{
    var menuID = el.id;
    if (menuID != null && menuID.length > 0)
      {1}
  }}
);",
                ClientID,
                anyNodeContextMenu.GetBindOpenEventScript("el", "menuID", true));
        ((IControl)this).Page!.ClientScript.RegisterStartupScriptBlock(this, typeof(WebTreeView), key, script);
      }

      List<WebTreeNode> unreachableNodes = new List<WebTreeNode>();
      foreach (KeyValuePair<WebTreeNode, DropDownMenu> entry in _menus)
      {
        if (IsTreeNodeReachable(entry.Key))
          PreRenderTreeNodeMenus(entry.Key, entry.Value.MenuItems);
        else
        {
          _menuPlaceHolder.Controls.Remove(entry.Value);
          unreachableNodes.Add(entry.Key);
        }
      }

      foreach (WebTreeNode node in unreachableNodes)
        _menus.Remove(node);
    }

    private void PreRenderTreeNodeMenus (WebTreeNode node, WebMenuItemCollection menuItems)
    {
      _menuItemProvider!.PreRenderMenuItems(node, menuItems);
    }

    private void Menu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      DropDownMenu menu = (DropDownMenu)sender;
      foreach (KeyValuePair<WebTreeNode, DropDownMenu> entry in _menus)
      {
        if (entry.Value == menu)
        {
          _menuItemProvider!.OnMenuItemEventCommandClick(e.Item, entry.Key);
          return;
        }
      }
    }

    private void Menu_WxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      DropDownMenu menu = (DropDownMenu)sender;
      foreach (KeyValuePair<WebTreeNode, DropDownMenu> entry in _menus)
      {
        if (entry.Value == menu)
        {
          _menuItemProvider!.OnMenuItemWxeFunctionCommandClick(e.Item, entry.Key);
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

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s node badge. </summary>
    /// <remarks> Class: <c>treeViewNodeBadge</c> </remarks>
    protected virtual string CssClassNodeBadge
    {
      get { return "treeViewNodeBadge"; }
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

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s last node in a category. </summary>
    /// <remarks> Class: <c>treeViewNodeCategorySeparator</c> </remarks>
    protected virtual string CssClassNodeCategorySeparator
    {
      get { return "treeViewNodeCategorySeparator"; }
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

    /// <summary> Gets the CSS-Class applied to elements only visible to screen readers. </summary>
    /// <remarks> Class: <c>screenReaderText</c> </remarks>
    protected string CssClassScreenReaderText
    {
      get { return CssClassDefinition.ScreenReaderText; }
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

  public delegate void WebTreeNodeMenuRenderMethod (HtmlTextWriter writer, WebTreeNode node, DropDownMenu menu);
}
