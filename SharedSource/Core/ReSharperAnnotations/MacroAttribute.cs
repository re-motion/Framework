// NOTE: This file was originally generated via JetBrains ReSharper
// and is part of the JetBrains.Annotations for ReSharper. 
// It has since been modified for use in the re-motion framework development.
//
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
// Original license header by JetBrains:
// MIT License
// 
// Copyright (c) 2016 JetBrains http://www.jetbrains.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 

#nullable disable

using System;

#pragma warning disable 1591
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace

namespace JetBrains.Annotations
{
  /// <summary>
  /// Allows specifying a macro for a parameter of a <see cref="SourceTemplateAttribute">source template</see>.
  /// </summary>
  /// <remarks>
  /// You can apply the attribute on the whole method or on any of its additional parameters. The macro expression
  /// is defined in the <see cref="MacroAttribute.Expression"/> property. When applied on a method, the target
  /// template parameter is defined in the <see cref="MacroAttribute.Target"/> property. To apply the macro silently
  /// for the parameter, set the <see cref="MacroAttribute.Editable"/> property value = -1.
  /// </remarks>
  /// <example>
  /// Applying the attribute on a source template method:
  /// <code>
  /// [SourceTemplate, Macro(Target = "item", Expression = "suggestVariableName()")]
  /// public static void forEach&lt;T&gt;(this IEnumerable&lt;T&gt; collection) {
  ///   foreach (var item in collection) {
  ///     //$ $END$
  ///   }
  /// }
  /// </code>
  /// Applying the attribute on a template method parameter:
  /// <code>
  /// [SourceTemplate]
  /// public static void something(this Entity x, [Macro(Expression = "guid()", Editable = -1)] string newguid) {
  ///   /*$ var $x$Id = "$newguid$" + x.ToString();
  ///   x.DoSomething($x$Id); */
  /// }
  /// </code>
  /// </example>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true)]
  sealed partial class MacroAttribute : Attribute
  {
    /// <summary>
    /// Allows specifying a macro that will be executed for a <see cref="SourceTemplateAttribute">source template</see>
    /// parameter when the template is expanded.
    /// </summary>
    [CanBeNull]
    public string Expression { get; set; }

    /// <summary>
    /// Allows specifying which occurrence of the target parameter becomes editable when the template is deployed.
    /// </summary>
    /// <remarks>
    /// If the target parameter is used several times in the template, only one occurrence becomes editable;
    /// other occurrences are changed synchronously. To specify the zero-based index of the editable occurrence,
    /// use values >= 0. To make the parameter non-editable when the template is expanded, use -1.
    /// </remarks>>
    public int Editable { get; set; }

    /// <summary>
    /// Identifies the target parameter of a <see cref="SourceTemplateAttribute">source template</see> if the
    /// <see cref="MacroAttribute"/> is applied on a template method.
    /// </summary>
    [CanBeNull]
    public string Target { get; set; }
  }
}

#nullable restore
