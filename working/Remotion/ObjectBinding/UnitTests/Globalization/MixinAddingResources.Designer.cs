﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Remotion.ObjectBinding.UnitTests.Globalization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class MixinAddingResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MixinAddingResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Remotion.ObjectBinding.UnitTests.Globalization.MixinAddingResources", typeof(MixinAddingResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to overridden by mixin.
        /// </summary>
        internal static string property_PropertyForMixinOverrideTest {
            get {
                return ResourceManager.GetString("property:PropertyForMixinOverrideTest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resource from mixin for long property name.
        /// </summary>
        internal static string property_Remotion_ObjectBinding_UnitTests_Core_TestDomain_SimpleBusinessObjectClass_StringForLongPropertyName {
            get {
                return ResourceManager.GetString("property:Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectCla" +
                        "ss.StringForLongPropertyName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resource from mixin.
        /// </summary>
        internal static string property_String {
            get {
                return ResourceManager.GetString("property:String", resourceCulture);
            }
        }
    }
}
