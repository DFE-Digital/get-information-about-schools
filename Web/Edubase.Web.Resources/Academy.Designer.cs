﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Edubase.Web.Resources {
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
    internal class Academy {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Academy() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Edubase.Web.Resources.Academy", typeof(Academy).Assembly);
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
        ///   Looks up a localized string similar to This should reflect the age range stated in the academy&apos;s funding agreement. Please follow the statutory guidancewhen updating this information..
        /// </summary>
        internal static string AgeRange {
            get {
                return ResourceManager.GetString("AgeRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://www.gov.uk/government/publications/making-significant-changes-to-an-existing-academy.
        /// </summary>
        internal static string AgeRangeLink {
            get {
                return ResourceManager.GetString("AgeRangeLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The capacity of the school is the number of pupil places it can accommodate. This should reflect the age range stated in the academy&apos;s funding agreement. Please follow the statutory guidance when updating this information..
        /// </summary>
        internal static string SchoolCapacity {
            get {
                return ResourceManager.GetString("SchoolCapacity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://www.gov.uk/government/publications/making-significant-changes-to-an-existing-academy.
        /// </summary>
        internal static string SchoolCapacityLink {
            get {
                return ResourceManager.GetString("SchoolCapacityLink", resourceCulture);
            }
        }
    }
}
