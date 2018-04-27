-- Generated by CSharp.lua Compiler 1.1.0
-- Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
local System = System
local SystemCollectionsImmutable = System.Collections.Immutable
local MicrosoftCodeAnalysis
System.usingDeclare(function (global) 
  MicrosoftCodeAnalysis = Microsoft.CodeAnalysis
end)
System.namespace("Microsoft.CodeAnalysis", function (namespace) 
  -- [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
  -- <summary>
  -- Represents a diagnostic, such as a compiler error or a warning, along with the location where it occurred.
  -- </summary>
  namespace.class("Diagnostic", function (namespace) 
    local Create, Create1, Create2, Create3, Create4, Create5, getCategory, getDefaultSeverity, 
    getIsEnabledByDefault, getIsWarningAsError, getCustomTags, getProperties, ToString, GetDefaultWarningLevel
    -- <summary>
    -- Creates a <see cref="Diagnostic"/> instance.
    -- </summary>
    -- <param name="descriptor">A <see cref="DiagnosticDescriptor"/> describing the diagnostic</param>
    -- <param name="location">An optional primary location of the diagnostic. If null, <see cref="Location"/> will return <see cref="Location.None"/>.</param>
    -- <param name="messageArgs">Arguments to the message of the diagnostic</param>
    -- <returns>The <see cref="Diagnostic"/> instance.</returns>
    Create = function (descriptor, location, messageArgs) 
      return Create3(descriptor, location, nil, nil, messageArgs)
    end
    -- <summary>
    -- Creates a <see cref="Diagnostic"/> instance.
    -- </summary>
    -- <param name="descriptor">A <see cref="DiagnosticDescriptor"/> describing the diagnostic.</param>
    -- <param name="location">An optional primary location of the diagnostic. If null, <see cref="Location"/> will return <see cref="Location.None"/>.</param>
    -- <param name="properties">
    -- An optional set of name-value pairs by means of which the analyzer that creates the diagnostic
    -- can convey more detailed information to the fixer. If null, <see cref="Properties"/> will return
    -- <see cref="ImmutableDictionary{TKey, TValue}.Empty"/>.
    -- </param>
    -- <param name="messageArgs">Arguments to the message of the diagnostic.</param>
    -- <returns>The <see cref="Diagnostic"/> instance.</returns>
    Create1 = function (descriptor, location, properties, messageArgs) 
      return Create3(descriptor, location, nil, properties, messageArgs)
    end
    -- <summary>
    -- Creates a <see cref="Diagnostic"/> instance.
    -- </summary>
    -- <param name="descriptor">A <see cref="DiagnosticDescriptor"/> describing the diagnostic.</param>
    -- <param name="location">An optional primary location of the diagnostic. If null, <see cref="Location"/> will return <see cref="Location.None"/>.</param>
    -- <param name="additionalLocations">
    -- An optional set of additional locations related to the diagnostic.
    -- Typically, these are locations of other items referenced in the message.
    -- If null, <see cref="AdditionalLocations"/> will return an empty list.
    -- </param>
    -- <param name="messageArgs">Arguments to the message of the diagnostic.</param>
    -- <returns>The <see cref="Diagnostic"/> instance.</returns>
    Create2 = function (descriptor, location, additionalLocations, messageArgs) 
      return Create3(descriptor, location, additionalLocations, nil, messageArgs)
    end
    -- <summary>
    -- Creates a <see cref="Diagnostic"/> instance.
    -- </summary>
    -- <param name="descriptor">A <see cref="DiagnosticDescriptor"/> describing the diagnostic.</param>
    -- <param name="location">An optional primary location of the diagnostic. If null, <see cref="Location"/> will return <see cref="Location.None"/>.</param>
    -- <param name="additionalLocations">
    -- An optional set of additional locations related to the diagnostic.
    -- Typically, these are locations of other items referenced in the message.
    -- If null, <see cref="AdditionalLocations"/> will return an empty list.
    -- </param>
    -- <param name="properties">
    -- An optional set of name-value pairs by means of which the analyzer that creates the diagnostic
    -- can convey more detailed information to the fixer. If null, <see cref="Properties"/> will return
    -- <see cref="ImmutableDictionary{TKey, TValue}.Empty"/>.
    -- </param>
    -- <param name="messageArgs">Arguments to the message of the diagnostic.</param>
    -- <returns>The <see cref="Diagnostic"/> instance.</returns>
    Create3 = function (descriptor, location, additionalLocations, properties, messageArgs) 
      System.throw(System.NotImplementedException())
      -- [Lua]
      --return Create(descriptor, location, effectiveSeverity: descriptor.DefaultSeverity, additionalLocations, properties, messageArgs);
    end
    Create4 = function (messageProvider, errorCode, arguments) 
      return Create5(MicrosoftCodeAnalysis.DiagnosticInfo:new(2, messageProvider, errorCode, arguments))
    end
    Create5 = function (info) 
      return MicrosoftCodeAnalysis.DiagnosticWithInfo(info, MicrosoftCodeAnalysis.Location.getNone(), false)
    end
    getCategory = function (this) 
      return this:getDescriptor():getCategory()
    end
    getDefaultSeverity = function (this) 
      System.throw(System.NotImplementedException())
    end
    getIsEnabledByDefault = function (this) 
      return this:getDescriptor():getIsEnabledByDefault()
    end
    getIsWarningAsError = function (this) 
      return this:getDefaultSeverity() == 2 --[[DiagnosticSeverity.Warning]] and this:getSeverity() == 3 --[[DiagnosticSeverity.Error]]
    end
    getCustomTags = function (this) 
      return System.cast(System.IReadOnlyList_1(System.String), this:getDescriptor():getCustomTags())
    end
    getProperties = function (this) 
      return SystemCollectionsImmutable.ImmutableDictionary_2(System.String, System.String).Empty
    end
    ToString = function (this) 
      System.throw(System.NotImplementedException())
      -- [Lua]
      --return DiagnosticFormatter.Instance.Format(this, CultureInfo.CurrentUICulture);
    end
    -- <summary>
    -- Gets the default warning level for a diagnostic severity. Warning levels are used with the <c>/warn:N</c>
    -- command line option to suppress diagnostics over a severity of interest. When N is 0, only error severity
    -- messages are produced by the compiler. Values greater than 0 indicated that warnings up to and including
    -- level N should also be included.
    -- </summary>
    -- <remarks>
    -- <see cref="DiagnosticSeverity.Info"/> and <see cref="DiagnosticSeverity.Hidden"/> are treated as warning
    -- level 1. In other words, these diagnostics which typically interact with editor features are enabled unless
    -- the special <c>/warn:0</c> option is set.
    -- </remarks>
    -- <param name="severity">A <see cref="DiagnosticSeverity"/> value.</param>
    -- <returns>The default compiler warning level for <paramref name="severity"/>.</returns>
    GetDefaultWarningLevel = function (severity) 
      repeat
        local default = severity
        if default == 3 --[[DiagnosticSeverity.Error]] then
          return 0
        else
          return 1
        end
      until 1
    end
    return {
      Create = Create, 
      Create1 = Create1, 
      Create2 = Create2, 
      Create3 = Create3, 
      Create4 = Create4, 
      Create5 = Create5, 
      getCategory = getCategory, 
      getDefaultSeverity = getDefaultSeverity, 
      getIsEnabledByDefault = getIsEnabledByDefault, 
      getIsWarningAsError = getIsWarningAsError, 
      getCustomTags = getCustomTags, 
      getProperties = getProperties, 
      ToString = ToString, 
      GetDefaultWarningLevel = GetDefaultWarningLevel
    }
  end)
end)