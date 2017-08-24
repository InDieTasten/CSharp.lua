-- Generated by CSharp.lua Compiler 1.1.0
--[[
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
]]
local System = System
local Linq = System.Linq.Enumerable
local MicrosoftCodeAnalysis = Microsoft.CodeAnalysis
local MicrosoftCodeAnalysisCSharp = Microsoft.CodeAnalysis.CSharp
local MicrosoftCodeAnalysisCSharpSyntax = Microsoft.CodeAnalysis.CSharp.Syntax
local SystemIO = System.IO
local SystemLinq = System.Linq
local SystemText = System.Text
local SystemTextRegularExpressions = System.Text.RegularExpressions
local SystemThreading = System.Threading
local CSharpLua
local CSharpLuaLuaAst
System.usingDeclare(function (global) 
  CSharpLua = global.CSharpLua
  CSharpLuaLuaAst = CSharpLua.LuaAst
end)
System.namespace("CSharpLua", function (namespace) 
  namespace.class("CmdArgumentException", function (namespace) 
    local __ctor__
    __ctor__ = function (this, message) 
      this.__base__.__ctor__(this, message)
    end
    return {
      __inherits__ = function (global) 
        return {
          global.System.Exception
        }
      end, 
      __ctor__ = __ctor__
    }
  end)

  namespace.class("CompilationErrorException", function (namespace) 
    local __ctor1__, __ctor2__
    __ctor1__ = function (this, message) 
      this.__base__.__ctor__(this, message)
    end
    __ctor2__ = function (this, node, message) 
      this.__base__.__ctor__(this, ("{0}: {1}, please refactor your code."):Format(CSharpLua.Utility.GetLocationString(node), message))
    end
    return {
      __inherits__ = function (global) 
        return {
          global.System.Exception
        }
      end, 
      __ctor__ = {
        __ctor1__, 
        __ctor2__
      }
    }
  end)

  namespace.class("ArgumentNullException", function (namespace) 
    local __ctor__
    __ctor__ = function (this, paramName) 
      this.__base__.__ctor__(this, paramName)
      assert(false)
    end
    return {
      __inherits__ = function (global) 
        return {
          global.System.ArgumentNullException
        }
      end, 
      __ctor__ = __ctor__
    }
  end)

  namespace.class("InvalidOperationException", function (namespace) 
    local __ctor__
    __ctor__ = function (this) 
      this.__base__.__ctor__(this)
      assert(false)
    end
    return {
      __inherits__ = function (global) 
        return {
          global.System.InvalidOperationException
        }
      end, 
      __ctor__ = __ctor__
    }
  end)

  namespace.class("Utility", function (namespace) 
    local First, Last, GetOrDefault, GetOrDefault1, TryAdd, AddAt, IndexOf, TrimEnd, 
    GetCommondLines, GetArgument, GetCurrentDirectory, Split, IsPrivate, IsPrivate1, IsStatic, IsAbstract, 
    IsReadOnly, IsConst, IsParams, IsPartial, IsOutOrRef, IsStringType, IsDelegateType, IsIntegerType, 
    IsNullableType, IsImmutable, IsInterfaceImplementation, InterfaceImplementations, IsFromCode, IsOverridable, OverriddenSymbol, IsOverridden, 
    IsPropertyField, IsEventFiled, HasStaticCtor, IsStaticLazy, IsAssignment, systemLinqEnumerableType_, IsSystemLinqEnumerable, GetLocationString, 
    IsSubclassOf, IsImplementInterface, IsBaseNumberType, IsNumberTypeAssignableFrom, IsAssignableFrom, CheckSymbolDefinition, CheckMethodDefinition, CheckOriginalDefinition, 
    IsMainEntryPoint, IsExtendSelf, IsTimeSpanType, IsGenericIEnumerableType, IsExplicitInterfaceImplementation, IsExportSyntaxTrivia, IsTypeDeclaration, GetIEnumerableElementType, 
    DynamicGetProperty, GetTupleElementTypes, GetTupleElementIndex, GetTupleElementCount, identifierRegex_, IsIdentifierIllegal, ToBase63, EncodeToIdentifier, 
    FillExternalTypeName, GetTypeShortName, GetNewIdentifierName, InternalGetAllNamespaces, GetAllNamespaces, __staticCtor__
    __staticCtor__ = function (this) 
      this.First = First
      this.Last = Last
      this.GetOrDefault = GetOrDefault
      this.GetOrDefault1 = GetOrDefault1
      this.TryAdd = TryAdd
      this.AddAt = AddAt
      this.IndexOf = IndexOf
      this.TrimEnd = TrimEnd
      this.GetCommondLines = GetCommondLines
      this.GetArgument = GetArgument
      this.GetCurrentDirectory = GetCurrentDirectory
      this.Split = Split
      this.IsPrivate = IsPrivate
      this.IsPrivate1 = IsPrivate1
      this.IsStatic = IsStatic
      this.IsAbstract = IsAbstract
      this.IsReadOnly = IsReadOnly
      this.IsConst = IsConst
      this.IsParams = IsParams
      this.IsPartial = IsPartial
      this.IsOutOrRef = IsOutOrRef
      this.IsStringType = IsStringType
      this.IsDelegateType = IsDelegateType
      this.IsIntegerType = IsIntegerType
      this.IsNullableType = IsNullableType
      this.IsImmutable = IsImmutable
      this.IsInterfaceImplementation = IsInterfaceImplementation
      this.InterfaceImplementations = InterfaceImplementations
      this.IsFromCode = IsFromCode
      this.IsOverridable = IsOverridable
      this.OverriddenSymbol = OverriddenSymbol
      this.IsOverridden = IsOverridden
      this.IsPropertyField = IsPropertyField
      this.IsEventFiled = IsEventFiled
      this.HasStaticCtor = HasStaticCtor
      this.IsStaticLazy = IsStaticLazy
      this.IsAssignment = IsAssignment
      this.IsSystemLinqEnumerable = IsSystemLinqEnumerable
      this.GetLocationString = GetLocationString
      this.IsSubclassOf = IsSubclassOf
      this.IsAssignableFrom = IsAssignableFrom
      this.CheckMethodDefinition = CheckMethodDefinition
      this.CheckOriginalDefinition = CheckOriginalDefinition
      this.IsMainEntryPoint = IsMainEntryPoint
      this.IsExtendSelf = IsExtendSelf
      this.IsTimeSpanType = IsTimeSpanType
      this.IsGenericIEnumerableType = IsGenericIEnumerableType
      this.IsExplicitInterfaceImplementation = IsExplicitInterfaceImplementation
      this.IsExportSyntaxTrivia = IsExportSyntaxTrivia
      this.IsTypeDeclaration = IsTypeDeclaration
      this.GetIEnumerableElementType = GetIEnumerableElementType
      this.GetTupleElementTypes = GetTupleElementTypes
      this.GetTupleElementIndex = GetTupleElementIndex
      this.GetTupleElementCount = GetTupleElementCount
      this.IsIdentifierIllegal = IsIdentifierIllegal
      this.GetTypeShortName = GetTypeShortName
      this.GetNewIdentifierName = GetNewIdentifierName
      this.InternalGetAllNamespaces = InternalGetAllNamespaces
      this.GetAllNamespaces = GetAllNamespaces
      identifierRegex_ = SystemTextRegularExpressions.Regex([[^[a-zA-Z_][a-zA-Z0-9_]*$]], 8 --[[RegexOptions.Compiled]])
    end
    First = function (list, T) 
      return list:get(0)
    end
    Last = function (list, T) 
      return list:get(list:getCount() - 1)
    end
    GetOrDefault = function (list, index, v, T) 
      local default
      if index >= 0 and index < list:getCount() then
        default = list:get(index)
      else
        default = v
      end
      return default
    end
    GetOrDefault1 = function (dict, key, t, K, T) 
      local v
      local default
      default, v = dict:TryGetValue(key)
      if default then
        return v
      end
      return t
    end
    TryAdd = function (dict, key, value, K, V) 
      local set = GetOrDefault1(dict, key, nil, K, System.HashSet(V))
      if set == nil then
        set = System.HashSet(V)()
        dict:Add(key, set)
      end
      return set:Add(value)
    end
    AddAt = function (list, index, v, T) 
      if index < list:getCount() then
        list:set(index, v)
      else
        local count = index - list:getCount()
        do
          local i = 0
          while i < count do
            list:Add(System.default(T))
            i = i + 1
          end
        end
        list:Add(v)
      end
    end
    IndexOf = function (source, match, T) 
      local index = 0
      for _, item in System.each(source) do
        if match(item) then
          return index
        end
        index = index + 1
      end
      return - 1
    end
    TrimEnd = function (s, end_) 
      if s:EndsWith(end_) then
        return s:Remove(#s - #end_, #end_)
      end
      return s
    end
    GetCommondLines = function (args) 
      local cmds = System.Dictionary(System.String, System.Array(System.String))()

      local key = ""
      local values = System.List(System.String)()

      for _, arg in System.each(args) do
        local i = arg:Trim()
        if i:StartsWith("-") then
          if not System.String.IsNullOrEmpty(key) then
            cmds:Add(key, values:ToArray())
            key = ""
            values:Clear()
          end
          key = i
        else
          values:Add(i)
        end
      end

      if not System.String.IsNullOrEmpty(key) then
        cmds:Add(key, values:ToArray())
      end
      return cmds
    end
    GetArgument = function (args, name, isOption) 
      local values = GetOrDefault1(args, name, nil, System.String, System.Array(System.String))
      if values == nil or #values == 0 then
        if isOption then
          return nil
        end
        System.throw(CSharpLua.CmdArgumentException(name .. " is not found"))
      end
      return values:get(0)
    end
    GetCurrentDirectory = function (path) 

      if path:StartsWith("~/" --[[CurrentDirectorySign1]]) then
        return SystemIO.Path.Combine(System.AppDomain.getCurrentDomain():getBaseDirectory(), path:Substring(#("~/" --[[CurrentDirectorySign1]])))
      elseif path:StartsWith("~\\" --[[CurrentDirectorySign2]]) then
        return SystemIO.Path.Combine(System.AppDomain.getCurrentDomain():getBaseDirectory(), path:Substring(#("~\\" --[[CurrentDirectorySign2]])))
      end

      return SystemIO.Path.Combine(System.Environment.getCurrentDirectory(), path)
    end
    Split = function (s, isPath) 
      local list = System.HashSet(System.String)()
      if not System.String.IsNullOrEmpty(s) then
        local array = s:Split(59 --[[';']], 0)
        for _, i in System.each(array) do
          local default
          if isPath then
            default = GetCurrentDirectory(i)
          else
            default = i
          end
          list:Add(default)
        end
      end
      return Linq.ToArray(list)
    end
    IsPrivate = function (symbol) 
      return symbol:getDeclaredAccessibility() == 1 --[[Accessibility.Private]]
    end
    IsPrivate1 = function (modifiers) 
      for _, modifier in System.each(modifiers) do
        repeat
          local default = MicrosoftCodeAnalysisCSharp.CSharpExtensions.Kind(modifier)
          if default == 8344 --[[SyntaxKind.PrivateKeyword]] then
            do
              return true
            end
          elseif default == 8343 --[[SyntaxKind.PublicKeyword]] or default == 8345 --[[SyntaxKind.InternalKeyword]] or default == 8346 --[[SyntaxKind.ProtectedKeyword]] then
            do
              return false
            end
          end
        until 1
      end
      return true
    end
    IsStatic = function (modifiers) 
      return Linq.Any(modifiers, function (i) 
        return MicrosoftCodeAnalysis.CSharpExtensions.IsKind(i, 8347 --[[SyntaxKind.StaticKeyword]])
      end)
    end
    IsAbstract = function (modifiers) 
      return Linq.Any(modifiers, function (i) 
        return MicrosoftCodeAnalysis.CSharpExtensions.IsKind(i, 8356 --[[SyntaxKind.AbstractKeyword]])
      end)
    end
    IsReadOnly = function (modifiers) 
      return Linq.Any(modifiers, function (i) 
        return MicrosoftCodeAnalysis.CSharpExtensions.IsKind(i, 8348 --[[SyntaxKind.ReadOnlyKeyword]])
      end)
    end
    IsConst = function (modifiers) 
      return Linq.Any(modifiers, function (i) 
        return MicrosoftCodeAnalysis.CSharpExtensions.IsKind(i, 8350 --[[SyntaxKind.ConstKeyword]])
      end)
    end
    IsParams = function (modifiers) 
      return Linq.Any(modifiers, function (i) 
        return MicrosoftCodeAnalysis.CSharpExtensions.IsKind(i, 8365 --[[SyntaxKind.ParamsKeyword]])
      end)
    end
    IsPartial = function (modifiers) 
      return Linq.Any(modifiers, function (i) 
        return MicrosoftCodeAnalysis.CSharpExtensions.IsKind(i, 8406 --[[SyntaxKind.PartialKeyword]])
      end)
    end
    IsOutOrRef = function (modifiers) 
      return Linq.Any(modifiers, function (i) 
        return MicrosoftCodeAnalysis.CSharpExtensions.IsKind(i, 8361 --[[SyntaxKind.OutKeyword]]) or MicrosoftCodeAnalysis.CSharpExtensions.IsKind(i, 8360 --[[SyntaxKind.RefKeyword]])
      end)
    end
    IsStringType = function (type) 
      return type:getSpecialType() == 20 --[[SpecialType.System_String]]
    end
    IsDelegateType = function (type) 
      return type:getTypeKind() == 3 --[[TypeKind.Delegate]]
    end
    IsIntegerType = function (type) 
      if IsNullableType(type) then
        type = First((System.cast(MicrosoftCodeAnalysis.INamedTypeSymbol, type)):getTypeArguments(), MicrosoftCodeAnalysis.ITypeSymbol)
      end
      return type:getSpecialType() >= 9 --[[SpecialType.System_SByte]] and type:getSpecialType() <= 16 --[[SpecialType.System_UInt64]]
    end
    IsNullableType = function (type) 
      return type:getOriginalDefinition():getSpecialType() == 32 --[[SpecialType.System_Nullable_T]]
    end
    IsImmutable = function (type) 
      local isImmutable = (type:getIsValueType() and type:getIsDefinition()) or IsStringType(type) or IsDelegateType(type)
      return isImmutable
    end
    IsInterfaceImplementation = function (symbol, T) 
      if not symbol:getIsStatic() then
        local type = symbol:getContainingType()
        if type ~= nil then
          local interfaceSymbols = Linq.SelectMany(type:getAllInterfaces(), function (i) 
            return i:GetMembers():OfType(T)
          end, T)
          return Linq.Any(interfaceSymbols, function (i) 
            return symbol:Equals(type:FindImplementationForInterfaceMember(i))
          end)
        end
      end
      return false
    end
    InterfaceImplementations = function (symbol, T) 
      if not symbol:getIsStatic() then
        local type = symbol:getContainingType()
        if type ~= nil then
          local interfaceSymbols = Linq.SelectMany(type:getAllInterfaces(), function (i) 
            return i:GetMembers():OfType(T)
          end, T)
          return Linq.Where(interfaceSymbols, function (i) 
            return symbol:Equals(type:FindImplementationForInterfaceMember(i))
          end)
        end
      end
      return System.Array.Empty(T)
    end
    IsFromCode = function (symbol) 
      return not symbol:getDeclaringSyntaxReferences():getIsEmpty()
    end
    IsOverridable = function (symbol) 
      return not symbol:getIsStatic() and (symbol:getIsAbstract() or symbol:getIsVirtual() or symbol:getIsOverride())
    end
    OverriddenSymbol = function (symbol) 
      repeat
        local default = symbol:getKind()
        if default == 9 --[[SymbolKind.Method]] then
          do
            local methodSymbol = System.cast(MicrosoftCodeAnalysis.IMethodSymbol, symbol)
            return methodSymbol:getOverriddenMethod()
          end
        elseif default == 15 --[[SymbolKind.Property]] then
          do
            local propertySymbol = System.cast(MicrosoftCodeAnalysis.IPropertySymbol, symbol)
            return propertySymbol:getOverriddenProperty()
          end
        elseif default == 5 --[[SymbolKind.Event]] then
          do
            local eventSymbol = System.cast(MicrosoftCodeAnalysis.IEventSymbol, symbol)
            return eventSymbol:getOverriddenEvent()
          end
        end
      until 1
      return nil
    end
    IsOverridden = function (symbol, superSymbol) 
      while true do
        local overriddenSymbol = OverriddenSymbol(symbol)
        if overriddenSymbol ~= nil then
          overriddenSymbol = CheckOriginalDefinition(overriddenSymbol)
          if overriddenSymbol:Equals(superSymbol) then
            return true
          end
          symbol = overriddenSymbol
        else
          return false
        end
      end
    end
    IsPropertyField = function (symbol) 
      if not IsFromCode(symbol) or IsOverridable(symbol) then
        return false
      end

      local syntaxReference = SystemLinq.ImmutableArrayExtensions.FirstOrDefault(symbol:getDeclaringSyntaxReferences(), MicrosoftCodeAnalysis.SyntaxReference)
      if syntaxReference ~= nil then
        local node = syntaxReference:GetSyntax(System.default(SystemThreading.CancellationToken))
        repeat
          local default = MicrosoftCodeAnalysisCSharp.CSharpExtensions.Kind(node)
          if default == 8892 --[[SyntaxKind.PropertyDeclaration]] then
            do
              local property = System.cast(MicrosoftCodeAnalysisCSharpSyntax.PropertyDeclarationSyntax, node)
              local hasGet = false
              local hasSet = false
              if property:getAccessorList() ~= nil then
                for _, accessor in System.each(property:getAccessorList():getAccessors()) do
                  if accessor:getBody() ~= nil then
                    if MicrosoftCodeAnalysis.CSharpExtensions.IsKind(accessor, 8896 --[[SyntaxKind.GetAccessorDeclaration]]) then
                      assert(not hasGet)
                      hasGet = true
                    else
                      assert(not hasSet)
                      hasSet = true
                    end
                  end
                end
              else
                assert(not hasGet)
                hasGet = true
              end
              local isField = not hasGet and not hasSet
              if isField then
                if IsInterfaceImplementation(symbol, MicrosoftCodeAnalysis.IPropertySymbol) then
                  isField = false
                end
              end
              return isField
            end
          elseif default == 8894 --[[SyntaxKind.IndexerDeclaration]] then
            do
              return false
            end
          elseif default == 8647 --[[SyntaxKind.AnonymousObjectMemberDeclarator]] then
            do
              return true
            end
          else
            do
              System.throw(CSharpLua.InvalidOperationException())
            end
          end
        until 1
      end
      return false
    end
    IsEventFiled = function (symbol) 
      if not IsFromCode(symbol) or IsOverridable(symbol) then
        return false
      end

      local syntaxReference = SystemLinq.ImmutableArrayExtensions.FirstOrDefault(symbol:getDeclaringSyntaxReferences(), MicrosoftCodeAnalysis.SyntaxReference)
      if syntaxReference ~= nil then
        local isField = MicrosoftCodeAnalysis.CSharpExtensions.IsKind(syntaxReference:GetSyntax(System.default(SystemThreading.CancellationToken)), 8795 --[[SyntaxKind.VariableDeclarator]])
        if isField then
          if IsInterfaceImplementation(symbol, MicrosoftCodeAnalysis.IEventSymbol) then
            isField = false
          end
        end
        return isField
      end
      return false
    end
    HasStaticCtor = function (typeSymbol) 
      return SystemLinq.ImmutableArrayExtensions.Any(typeSymbol:getConstructors(), function (i) 
        return i:getIsStatic()
      end, MicrosoftCodeAnalysis.IMethodSymbol)
    end
    IsStaticLazy = function (symbol) 
      local success = symbol:getIsStatic() and not IsPrivate(symbol)
      if success then
        local typeSymbol = symbol:getContainingType()
        return HasStaticCtor(typeSymbol)
      end
      return success
    end
    IsAssignment = function (kind) 
      return kind >= 8714 --[[SyntaxKind.SimpleAssignmentExpression]] and kind <= 8724 --[[SyntaxKind.RightShiftAssignmentExpression]]
    end
    IsSystemLinqEnumerable = function (symbol) 
      if systemLinqEnumerableType_ ~= nil then
        return symbol == systemLinqEnumerableType_
      else
        local success = symbol:ToString() == CSharpLuaLuaAst.LuaIdentifierNameSyntax.SystemLinqEnumerable.ValueText
        if success then
          systemLinqEnumerableType_ = symbol
        end
        return success
      end
    end
    GetLocationString = function (node) 
      local location = node:getSyntaxTree():GetLocation(node:getSpan())
      local methodInfo = location:GetType():GetMethod("GetDebuggerDisplay", 36 --[[BindingFlags.Instance | BindingFlags.NonPublic]])
      return System.cast(System.String, methodInfo:Invoke(location, nil))
    end
    IsSubclassOf = function (child, parent) 
      if parent:getSpecialType() == 1 --[[SpecialType.System_Object]] then
        return true
      end

      local p = child
      if p == parent then
        return false
      end

      while p ~= nil do
        if p == parent then
          return true
        end
        p = p:getBaseType()
      end
      return false
    end
    IsImplementInterface = function (implementType, interfaceType) 
      local t = implementType
      while t ~= nil do
        local interfaces = implementType:getAllInterfaces()
        for _, i in System.each(interfaces) do
          if i == interfaceType or IsImplementInterface(i, interfaceType) then
            return true
          end
        end
        t = t:getBaseType()
      end
      return false
    end
    IsBaseNumberType = function (specialType) 
      return specialType >= 8 --[[SpecialType.System_Char]] and specialType <= 19 --[[SpecialType.System_Double]]
    end
    IsNumberTypeAssignableFrom = function (left, right) 
      if IsBaseNumberType(left:getSpecialType()) and IsBaseNumberType(right:getSpecialType()) then
        local begin
        repeat
          local default = right:getSpecialType()
          if default == 8 --[[SpecialType.System_Char]] or default == 9 --[[SpecialType.System_SByte]] or default == 10 --[[SpecialType.System_Byte]] then
            do
              begin = 11 --[[SpecialType.System_Int16]]
              break
            end
          elseif default == 11 --[[SpecialType.System_Int16]] or default == 12 --[[SpecialType.System_UInt16]] then
            do
              begin = 13 --[[SpecialType.System_Int32]]
              break
            end
          elseif default == 13 --[[SpecialType.System_Int32]] or default == 14 --[[SpecialType.System_UInt32]] then
            do
              begin = 15 --[[SpecialType.System_Int64]]
              break
            end
          elseif default == 15 --[[SpecialType.System_Int64]] or default == 16 --[[SpecialType.System_UInt64]] then
            do
              begin = 17 --[[SpecialType.System_Decimal]]
              break
            end
          else
            do
              begin = 19 --[[SpecialType.System_Double]]
              break
            end
          end
        until 1
        local end_ = 19 --[[SpecialType.System_Double]]
        return left:getSpecialType() >= begin and left:getSpecialType() <= end_
      end
      return false
    end
    IsAssignableFrom = function (left, right) 
      if left == right then
        return true
      end

      if IsNumberTypeAssignableFrom(left, right) then
        return true
      end

      if IsSubclassOf(right, left) then
        return true
      end

      if left:getTypeKind() == 7 --[[TypeKind.Interface]] then
        return IsImplementInterface(right, left)
      end

      return false
    end
    CheckSymbolDefinition = function (symbol, T) 
      local originalDefinition = System.cast(T, symbol:getOriginalDefinition())
      if originalDefinition ~= symbol then
        symbol = originalDefinition
      end
      return symbol
    end
    CheckMethodDefinition = function (symbol) 
      if symbol:getIsExtensionMethod() then
        if symbol:getReducedFrom() ~= nil and symbol:getReducedFrom() ~= symbol then
          symbol = symbol:getReducedFrom()
        end
      else
        symbol = CheckSymbolDefinition(symbol, MicrosoftCodeAnalysis.IMethodSymbol)
      end
      return symbol
    end
    CheckOriginalDefinition = function (symbol) 
      if symbol:getKind() == 9 --[[SymbolKind.Method]] then
        local methodSymbol = System.cast(MicrosoftCodeAnalysis.IMethodSymbol, symbol)
        methodSymbol = CheckMethodDefinition(methodSymbol)
        if methodSymbol ~= symbol then
          symbol = methodSymbol
        end
      else
        symbol = CheckSymbolDefinition(symbol, MicrosoftCodeAnalysis.ISymbol)
      end
      return symbol
    end
    IsMainEntryPoint = function (symbol) 
      if symbol:getIsStatic() and symbol:getMethodKind() == 10 --[[MethodKind.Ordinary]] and symbol:getTypeArguments():getIsEmpty() and symbol:getContainingType():getTypeArguments():getIsEmpty() and symbol:getName() == "Main" then
        if symbol:getReturnsVoid() or symbol:getReturnType():getSpecialType() == 13 --[[SpecialType.System_Int32]] then
          if symbol:getParameters():getIsEmpty() then
            return true
          elseif symbol:getParameters():getLength() == 1 then
            local parameterType = symbol:getParameters():get(0):getType()
            if parameterType:getTypeKind() == 1 --[[TypeKind.Array]] then
              local arrayType = System.cast(MicrosoftCodeAnalysis.IArrayTypeSymbol, parameterType)
              if arrayType:getElementType():getSpecialType() == 20 --[[SpecialType.System_String]] then
                return true
              end
            end
          end
        end
      end
      return false
    end
    IsExtendSelf = function (typeSymbol, baseTypeSymbol) 
      if baseTypeSymbol:getIsGenericType() then
        for _, baseTypeArgument in System.each(baseTypeSymbol:getTypeArguments()) do
          if baseTypeSymbol:getKind() ~= 17 --[[SymbolKind.TypeParameter]] then
            if not baseTypeArgument:Equals(typeSymbol) then
              if IsAssignableFrom(typeSymbol, baseTypeArgument) then
                return true
              end
            end
          end
        end
      end
      return false
    end
    IsTimeSpanType = function (typeSymbol) 
      return typeSymbol:getContainingNamespace():getName() == "System" and typeSymbol:getName() == "TimeSpan"
    end
    IsGenericIEnumerableType = function (typeSymbol) 
      return typeSymbol:getOriginalDefinition():getSpecialType() == 25 --[[SpecialType.System_Collections_Generic_IEnumerable_T]]
    end
    IsExplicitInterfaceImplementation = function (symbol) 
      repeat
        local default = symbol:getKind()
        if default == 15 --[[SymbolKind.Property]] then
          do
            local property = System.cast(MicrosoftCodeAnalysis.IPropertySymbol, symbol)
            if property:getGetMethod() ~= nil then
              if property:getGetMethod():getMethodKind() == 8 --[[MethodKind.ExplicitInterfaceImplementation]] then
                return true
              end
              if property:getSetMethod() ~= nil then
                if property:getSetMethod():getMethodKind() == 8 --[[MethodKind.ExplicitInterfaceImplementation]] then
                  return true
                end
              end
            end
            break
          end
        elseif default == 9 --[[SymbolKind.Method]] then
          do
            local method = System.cast(MicrosoftCodeAnalysis.IMethodSymbol, symbol)
            if method:getMethodKind() == 8 --[[MethodKind.ExplicitInterfaceImplementation]] then
              return true
            end
            break
          end
        end
      until 1
      return false
    end
    IsExportSyntaxTrivia = function (syntaxTrivia, rootNode) 
      repeat
        local default = MicrosoftCodeAnalysisCSharp.CSharpExtensions.Kind(syntaxTrivia)
        if default == 8541 --[[SyntaxKind.SingleLineCommentTrivia]] or default == 8542 --[[SyntaxKind.MultiLineCommentTrivia]] or default == 8552 --[[SyntaxKind.RegionDirectiveTrivia]] or default == 8553 --[[SyntaxKind.EndRegionDirectiveTrivia]] then
          local span = MicrosoftCodeAnalysis.CSharpExtensions.IsKind(rootNode, 8840 --[[SyntaxKind.CompilationUnit]]) and rootNode:getFullSpan() or rootNode:getSpan()
          return span:Contains(syntaxTrivia:getSpan())
        else
          return false
        end
      until 1
    end
    IsTypeDeclaration = function (kind) 
      return kind >= 8855 --[[SyntaxKind.ClassDeclaration]] and kind <= 8858 --[[SyntaxKind.EnumDeclaration]]
    end
    GetIEnumerableElementType = function (symbol) 
      local default
      if IsGenericIEnumerableType(symbol) then
        default = System.cast(MicrosoftCodeAnalysis.INamedTypeSymbol, symbol)
      else
        default = SystemLinq.ImmutableArrayExtensions.FirstOrDefault(symbol:getAllInterfaces(), function (i) 
          return IsGenericIEnumerableType(i)
        end, MicrosoftCodeAnalysis.INamedTypeSymbol)
      end
      local interfaceType = default
      local extern = interfaceType
      if extern ~= nil then
        extern = First(extern.getTypeArguments(), MicrosoftCodeAnalysis.ITypeSymbol)
      end
      return extern
    end
    DynamicGetProperty = function (symbol, name, T) 
      return System.cast(T, symbol:GetType():GetProperty(name):GetValue(symbol))
    end
    GetTupleElementTypes = function (typeSymbol) 
      assert(typeSymbol:getIsTupleType())
      return DynamicGetProperty(typeSymbol, "TupleElementTypes", System.IReadOnlyCollection_1(MicrosoftCodeAnalysis.ITypeSymbol))
    end
    GetTupleElementIndex = function (fieldSymbol) 
      assert(fieldSymbol:getContainingType():getIsTupleType())
      return DynamicGetProperty(fieldSymbol, "TupleElementIndex", System.Int) + 1
    end
    GetTupleElementCount = function (typeSymbol) 
      return GetTupleElementTypes(typeSymbol):getCount()
    end
    IsIdentifierIllegal = function (identifierName) 
      if not identifierRegex_:IsMatch(identifierName) then
        identifierName = EncodeToIdentifier(identifierName)
        return true, identifierName
      end
      return false, identifierName
    end
    ToBase63 = function (number) 
      local kAlphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_"
      local basis = #kAlphabet
      local n = number
      local sb = SystemText.StringBuilder()
      while n > 0 do
        local ch = kAlphabet:get(n % basis)
        sb:Append(ch)
        n = n // basis
      end
      return sb:ToString()
    end
    EncodeToIdentifier = function (name) 
      local sb = SystemText.StringBuilder()
      for _, c in System.each(name) do
        if c < 127 then
          sb:Append(c)
        else
          local base63 = ToBase63(c)
          sb:Append(base63)
        end
      end
      if System.Char.IsNumber(sb:get(0)) then
        sb:Insert(0, 95 --[['_']])
      end
      return sb:ToString()
    end
    FillExternalTypeName = function (sb, typeSymbol, funcOfTypeName) 
      local externalType = typeSymbol:getContainingType()
      if externalType ~= nil then
        FillExternalTypeName(sb, externalType, funcOfTypeName)
        local default = funcOfTypeName
        if default ~= nil then
          default = default(typeSymbol)
        end
        local typeName = default or externalType:getName()
        sb:Append(typeName)
        local typeParametersCount = externalType:getTypeParameters():getLength()
        if typeParametersCount > 0 then
          sb:Append(95 --[['_']])
          sb:Append(typeParametersCount)
        end
        sb:Append(46 --[['.']])
      end
    end
    GetTypeShortName = function (typeSymbol, funcOfNamespace, funcOfTypeName) 
      local sb = SystemText.StringBuilder()
      local namespaceName
      local namespaceSymbol = typeSymbol:getContainingNamespace()
      if namespaceSymbol:getIsGlobalNamespace() then
        namespaceName = ""
      else
        namespaceName = namespaceSymbol:ToString()
        local default = funcOfNamespace
        if default ~= nil then
          default = default(namespaceSymbol, namespaceName)
        end
        local newName = default
        if newName ~= nil then
          namespaceName = newName
        end
      end
      if #namespaceName > 0 then
        sb:Append(namespaceName)
        sb:Append(46 --[['.']])
      end
      FillExternalTypeName(sb, typeSymbol, funcOfTypeName)
      local extern = funcOfTypeName
      if extern ~= nil then
        extern = extern(typeSymbol)
      end
      local typeName = extern or typeSymbol:getName()
      sb:Append(typeName)
      local typeParametersCount = typeSymbol:getTypeParameters():getLength()
      if typeParametersCount > 0 then
        sb:Append(95 --[['_']])
        sb:Append(typeParametersCount)
      end
      return sb:ToString()
    end
    GetNewIdentifierName = function (name, index) 
      repeat
        local default = index
        if default == 0 then
          return name
        elseif default == 1 then
          return name .. "_"
        elseif default == 2 then
          return "_" .. name
        else
          return name .. (index - 2)
        end
      until 1
    end
    InternalGetAllNamespaces = function (symbol) 
      return System.yieldIEnumerable(function (symbol) 
        repeat
          System.yieldReturn(symbol)
          symbol = symbol:getContainingNamespace()
        until not (not symbol:getIsGlobalNamespace())
      end, MicrosoftCodeAnalysis.INamespaceSymbol, symbol)
    end
    GetAllNamespaces = function (symbol) 
      return Linq.Reverse(InternalGetAllNamespaces(symbol))
    end
    return {
      __staticCtor__ = __staticCtor__
    }
  end)
end)
