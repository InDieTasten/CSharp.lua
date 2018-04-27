// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Represents a diagnostic, such as a compiler error or a warning, along with the location where it occurred.
    /// </summary>
    // [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public abstract partial class Diagnostic
    {
        internal const string CompilerDiagnosticCategory = "Compiler";

        /// <summary>
        /// Highest valid warning level for non-error diagnostics.
        /// </summary>
        internal const int HighestValidWarningLevel = 4;

        /// <summary>
        /// Creates a <see cref="Diagnostic"/> instance.
        /// </summary>
        /// <param name="descriptor">A <see cref="DiagnosticDescriptor"/> describing the diagnostic</param>
        /// <param name="location">An optional primary location of the diagnostic. If null, <see cref="Location"/> will return <see cref="Location.None"/>.</param>
        /// <param name="messageArgs">Arguments to the message of the diagnostic</param>
        /// <returns>The <see cref="Diagnostic"/> instance.</returns>
        public static Diagnostic Create(
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            return Create(descriptor, location, null, null, messageArgs);
        }

        /// <summary>
        /// Creates a <see cref="Diagnostic"/> instance.
        /// </summary>
        /// <param name="descriptor">A <see cref="DiagnosticDescriptor"/> describing the diagnostic.</param>
        /// <param name="location">An optional primary location of the diagnostic. If null, <see cref="Location"/> will return <see cref="Location.None"/>.</param>
        /// <param name="properties">
        /// An optional set of name-value pairs by means of which the analyzer that creates the diagnostic
        /// can convey more detailed information to the fixer. If null, <see cref="Properties"/> will return
        /// <see cref="ImmutableDictionary{TKey, TValue}.Empty"/>.
        /// </param>
        /// <param name="messageArgs">Arguments to the message of the diagnostic.</param>
        /// <returns>The <see cref="Diagnostic"/> instance.</returns>
        public static Diagnostic Create(
            DiagnosticDescriptor descriptor,
            Location location,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            return Create(descriptor, location, null, properties, messageArgs);
        }

        /// <summary>
        /// Creates a <see cref="Diagnostic"/> instance.
        /// </summary>
        /// <param name="descriptor">A <see cref="DiagnosticDescriptor"/> describing the diagnostic.</param>
        /// <param name="location">An optional primary location of the diagnostic. If null, <see cref="Location"/> will return <see cref="Location.None"/>.</param>
        /// <param name="additionalLocations">
        /// An optional set of additional locations related to the diagnostic.
        /// Typically, these are locations of other items referenced in the message.
        /// If null, <see cref="AdditionalLocations"/> will return an empty list.
        /// </param>
        /// <param name="messageArgs">Arguments to the message of the diagnostic.</param>
        /// <returns>The <see cref="Diagnostic"/> instance.</returns>
        public static Diagnostic Create(
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            params object[] messageArgs)
        {
            return Create(descriptor, location, additionalLocations, properties: null, messageArgs: messageArgs);
        }

        /// <summary>
        /// Creates a <see cref="Diagnostic"/> instance.
        /// </summary>
        /// <param name="descriptor">A <see cref="DiagnosticDescriptor"/> describing the diagnostic.</param>
        /// <param name="location">An optional primary location of the diagnostic. If null, <see cref="Location"/> will return <see cref="Location.None"/>.</param>
        /// <param name="additionalLocations">
        /// An optional set of additional locations related to the diagnostic.
        /// Typically, these are locations of other items referenced in the message.
        /// If null, <see cref="AdditionalLocations"/> will return an empty list.
        /// </param>
        /// <param name="properties">
        /// An optional set of name-value pairs by means of which the analyzer that creates the diagnostic
        /// can convey more detailed information to the fixer. If null, <see cref="Properties"/> will return
        /// <see cref="ImmutableDictionary{TKey, TValue}.Empty"/>.
        /// </param>
        /// <param name="messageArgs">Arguments to the message of the diagnostic.</param>
        /// <returns>The <see cref="Diagnostic"/> instance.</returns>
        public static Diagnostic Create(
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            throw new NotImplementedException(); // [Lua]
            //return Create(descriptor, location, effectiveSeverity: descriptor.DefaultSeverity, additionalLocations, properties, messageArgs);
        }

        internal static Diagnostic Create(CommonMessageProvider messageProvider, int errorCode, params object[] arguments)
        {
            return Create(new DiagnosticInfo(messageProvider, errorCode, arguments));
        }

        internal static Diagnostic Create(DiagnosticInfo info)
        {
            return new DiagnosticWithInfo(info, Location.None);
        }

        /// <summary>
        /// Gets the diagnostic descriptor, which provides a description about a <see cref="Diagnostic"/>.
        /// </summary>
        public abstract DiagnosticDescriptor Descriptor { get; }

        /// <summary>
        /// Gets the diagnostic identifier. For diagnostics generated by the compiler, this will be a numeric code with a prefix such as "CS1001".
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Gets the category of diagnostic. For diagnostics generated by the compiler, the category will be "Compiler".
        /// </summary>
        internal virtual string Category { get { return this.Descriptor.Category; } }

        /// <summary>
        /// Get the culture specific text of the message.
        /// </summary>
        public abstract string GetMessage(IFormatProvider formatProvider = null);

        /// <summary>
        /// Gets the default <see cref="DiagnosticSeverity"/> of the diagnostic's <see cref="DiagnosticDescriptor"/>.
        /// </summary>
        /// <remarks>
        /// To get the effective severity of the diagnostic, use <see cref="Severity"/>.
        /// </remarks>
        public virtual DiagnosticSeverity DefaultSeverity { get { throw new NotImplementedException(); } } // [Lua] return this.Descriptor.DefaultSeverity; } }

        /// <summary>
        /// Gets the effective <see cref="DiagnosticSeverity"/> of the diagnostic.
        /// </summary>
        /// <remarks>
        /// To get the default severity of diagnostic's <see cref="DiagnosticDescriptor"/>, use <see cref="DefaultSeverity"/>.
        /// To determine if this is a warning treated as an error, use <see cref="IsWarningAsError"/>.
        /// </remarks>
        public abstract DiagnosticSeverity Severity { get; }

        /// <summary>
        /// Gets the warning level. This is 0 for diagnostics with severity <see cref="DiagnosticSeverity.Error"/>,
        /// otherwise an integer between 1 and 4.
        /// </summary>
        public abstract int WarningLevel { get; }

        /// <summary>
        /// Returns true if the diagnostic has a source suppression, i.e. an attribute or a pragma suppression.
        /// </summary>
        public abstract bool IsSuppressed { get; }

        /// <summary>
        /// Returns true if this diagnostic is enabled by default by the author of the diagnostic.
        /// </summary>
        internal virtual bool IsEnabledByDefault { get { return this.Descriptor.IsEnabledByDefault; } }

        /// <summary>
        /// Returns true if this is a warning treated as an error; otherwise false.
        /// </summary>
        /// <remarks>
        /// True implies <see cref="DefaultSeverity"/> = <see cref="DiagnosticSeverity.Warning"/>
        /// and <see cref="Severity"/> = <see cref="DiagnosticSeverity.Error"/>.
        /// </remarks>
        public bool IsWarningAsError
        {
            get
            {
                return this.DefaultSeverity == DiagnosticSeverity.Warning &&
                    this.Severity == DiagnosticSeverity.Error;
            }
        }

        /// <summary>
        /// Gets the primary location of the diagnostic, or <see cref="Location.None"/> if no primary location.
        /// </summary>
        public abstract Location Location { get; }

        /// <summary>
        /// Gets an array of additional locations related to the diagnostic.
        /// Typically these are the locations of other items referenced in the message.
        /// </summary>
        public abstract IReadOnlyList<Location> AdditionalLocations { get; }

        /// <summary>
        /// Gets custom tags for the diagnostic.
        /// </summary>
        internal virtual IReadOnlyList<string> CustomTags { get { return (IReadOnlyList<string>)this.Descriptor.CustomTags; } }

        /// <summary>
        /// Gets property bag for the diagnostic. it will return <see cref="ImmutableDictionary{TKey, TValue}.Empty"/> 
        /// if there is no entry. This can be used to put diagnostic specific information you want 
        /// to pass around. for example, to corresponding fixer.
        /// </summary>
        public virtual ImmutableDictionary<string, string> Properties
            => ImmutableDictionary<string, string>.Empty;

        public override string ToString()
        {
            throw new NotImplementedException(); // [Lua]
            //return DiagnosticFormatter.Instance.Format(this, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Gets the default warning level for a diagnostic severity. Warning levels are used with the <c>/warn:N</c>
        /// command line option to suppress diagnostics over a severity of interest. When N is 0, only error severity
        /// messages are produced by the compiler. Values greater than 0 indicated that warnings up to and including
        /// level N should also be included.
        /// </summary>
        /// <remarks>
        /// <see cref="DiagnosticSeverity.Info"/> and <see cref="DiagnosticSeverity.Hidden"/> are treated as warning
        /// level 1. In other words, these diagnostics which typically interact with editor features are enabled unless
        /// the special <c>/warn:0</c> option is set.
        /// </remarks>
        /// <param name="severity">A <see cref="DiagnosticSeverity"/> value.</param>
        /// <returns>The default compiler warning level for <paramref name="severity"/>.</returns>
        internal static int GetDefaultWarningLevel(DiagnosticSeverity severity)
        {
            switch (severity)
            {
                case DiagnosticSeverity.Error:
                    return 0;

                case DiagnosticSeverity.Warning:
                default:
                    return 1;
            }
        }
    }
}
