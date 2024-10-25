﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Melecs.OracleDataBase.FIS {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Language {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Language() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Melecs.OracleDataBase.FIS.Language", typeof(Language).Assembly);
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
        ///   Looks up a localized string similar to Die Länge der Auftragsnummer ist nicht korrekt!.
        /// </summary>
        internal static string AuftragsNummerLengthError01 {
            get {
                return ResourceManager.GetString("AuftragsNummerLengthError01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Länge des Idents ist nicht korrekt!.
        /// </summary>
        internal static string IdentLengthError01 {
            get {
                return ResourceManager.GetString("IdentLengthError01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Idents are not found.
        /// </summary>
        internal static string IdentsNotFound {
            get {
                return ResourceManager.GetString("IdentsNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The date has incorrect range. It should be specified for the last 24 hour..
        /// </summary>
        internal static string IncorrectFromDateRange {
            get {
                return ResourceManager.GetString("IncorrectFromDateRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The material number is missing!.
        /// </summary>
        internal static string MissingMaterialNumber {
            get {
                return ResourceManager.GetString("MissingMaterialNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Datenbank ist nicht verbunden!.
        /// </summary>
        internal static string NoConnection {
            get {
                return ResourceManager.GetString("NoConnection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Es wurde kein Ident eingetragen!.
        /// </summary>
        internal static string NoIdent {
            get {
                return ResourceManager.GetString("NoIdent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Es konnte kein Wert zu dem Parameter gefunden werden!.
        /// </summary>
        internal static string NoParameterValue {
            get {
                return ResourceManager.GetString("NoParameterValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Es wurde kein Datensatz empfangen!.
        /// </summary>
        internal static string NoReturnValue {
            get {
                return ResourceManager.GetString("NoReturnValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Es konnte kein offener Oberstufenauftrag gefunden werden!.
        /// </summary>
        internal static string NoSecLvlOrdNr {
            get {
                return ResourceManager.GetString("NoSecLvlOrdNr", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Es wurde keine Tracenummer eingetragen!.
        /// </summary>
        internal static string NoTracenumber {
            get {
                return ResourceManager.GetString("NoTracenumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Orders not found.
        /// </summary>
        internal static string OrdersNotFound {
            get {
                return ResourceManager.GetString("OrdersNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Konnte Abfrage auf Grund eines unbekannten Fehlers nicht durchführen!.
        /// </summary>
        internal static string UnknownError {
            get {
                return ResourceManager.GetString("UnknownError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Das Intervall der Erstmusterfreigabe wurde falsch angegeben!.
        /// </summary>
        internal static string WrongEMFType {
            get {
                return ResourceManager.GetString("WrongEMFType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die retournierte Materialnummer stimmt nicht überein!.
        /// </summary>
        internal static string WrongMatNr {
            get {
                return ResourceManager.GetString("WrongMatNr", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Der retournierte Parameter stimmt nicht überein!.
        /// </summary>
        internal static string WrongParameter {
            get {
                return ResourceManager.GetString("WrongParameter", resourceCulture);
            }
        }
    }
}
