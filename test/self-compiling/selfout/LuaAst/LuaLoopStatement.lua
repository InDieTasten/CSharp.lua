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
local CSharpLua
local CSharpLuaLuaAst
System.usingDeclare(function (global) 
  CSharpLua = global.CSharpLua
  CSharpLuaLuaAst = CSharpLua.LuaAst
end)
System.namespace("CSharpLua.LuaAst", function (namespace) 
  namespace.class("LuaForInStatementSyntax", function (namespace) 
    local getForKeyword, getInKeyword, getPlaceholder, Render, __init__, __ctor__
    __init__ = function (this) 
      this.Body = System.create(CSharpLuaLuaAst.LuaBlockSyntax(), function (default) 
        default.OpenBraceToken = "do" --[[Keyword.Do]]
        default.CloseBraceToken = "end" --[[Keyword.End]]
      end)
    end
    __ctor__ = function (this, identifier, expression) 
      __init__(this)
      this.__base__.__ctor__(this)
      if expression == nil then
        System.throw(CSharpLua.ArgumentNullException("expression" --[[nameof(expression)]]))
      end
      this.Identifier = identifier or System.throw(CSharpLua.ArgumentNullException("identifier" --[[nameof(identifier)]]))
      this.Expression = CSharpLuaLuaAst.LuaInvocationExpressionSyntax:new(2, CSharpLuaLuaAst.LuaIdentifierNameSyntax.Each, expression)
    end
    getForKeyword = function (this) 
      return "for" --[[Keyword.For]]
    end
    getInKeyword = function (this) 
      return "in" --[[Keyword.In]]
    end
    getPlaceholder = function (this) 
      return CSharpLuaLuaAst.LuaIdentifierNameSyntax.Placeholder
    end
    Render = function (this, renderer) 
      renderer:Render45(this)
    end
    return {
      __inherits__ = function (global) 
        return {
          global.CSharpLua.LuaAst.LuaStatementSyntax
        }
      end, 
      getForKeyword = getForKeyword, 
      getInKeyword = getInKeyword, 
      getPlaceholder = getPlaceholder, 
      Render = Render, 
      __ctor__ = __ctor__
    }
  end)

  namespace.class("LuaNumericalForStatementSyntax", function (namespace) 
    local getForKeyword, getEqualsToken, Render, __init__, __ctor__
    __init__ = function (this) 
      this.Body = System.create(CSharpLuaLuaAst.LuaBlockSyntax(), function (default) 
        default.OpenBraceToken = "do" --[[Keyword.Do]]
        default.CloseBraceToken = "end" --[[Keyword.End]]
      end)
    end
    __ctor__ = function (this, identifier, startExpression, limitExpression, stepExpression) 
      __init__(this)
      this.__base__.__ctor__(this)
      this.Identifier = identifier or System.throw(CSharpLua.ArgumentNullException("identifier" --[[nameof(identifier)]]))
      this.StartExpression = startExpression or System.throw(CSharpLua.ArgumentNullException("startExpression" --[[nameof(startExpression)]]))
      this.LimitExpression = limitExpression or System.throw(CSharpLua.ArgumentNullException("limitExpression" --[[nameof(limitExpression)]]))
      this.StepExpression = stepExpression
    end
    getForKeyword = function (this) 
      return "for" --[[Keyword.For]]
    end
    getEqualsToken = function (this) 
      return "=" --[[Tokens.Equals]]
    end
    Render = function (this, renderer) 
      renderer:Render46(this)
    end
    return {
      __inherits__ = function (global) 
        return {
          global.CSharpLua.LuaAst.LuaStatementSyntax
        }
      end, 
      getForKeyword = getForKeyword, 
      getEqualsToken = getEqualsToken, 
      Render = Render, 
      __ctor__ = __ctor__
    }
  end)

  namespace.class("LuaWhileStatementSyntax", function (namespace) 
    local getWhileKeyword, Render, __init__, __ctor__
    __init__ = function (this) 
      this.Body = System.create(CSharpLuaLuaAst.LuaBlockSyntax(), function (default) 
        default.OpenBraceToken = "do" --[[Keyword.Do]]
        default.CloseBraceToken = "end" --[[Keyword.End]]
      end)
    end
    __ctor__ = function (this, condition) 
      __init__(this)
      this.__base__.__ctor__(this)
      this.Condition = condition or System.throw(CSharpLua.ArgumentNullException("condition" --[[nameof(condition)]]))
    end
    getWhileKeyword = function (this) 
      return "while" --[[Keyword.While]]
    end
    Render = function (this, renderer) 
      renderer:Render47(this)
    end
    return {
      __inherits__ = function (global) 
        return {
          global.CSharpLua.LuaAst.LuaStatementSyntax
        }
      end, 
      getWhileKeyword = getWhileKeyword, 
      Render = Render, 
      __ctor__ = __ctor__
    }
  end)

  namespace.class("LuaRepeatStatementSyntax", function (namespace) 
    local getRepeatKeyword, getUntilKeyword, Render, __init__, __ctor__
    __init__ = function (this) 
      this.Body = CSharpLuaLuaAst.LuaBlockSyntax()
    end
    __ctor__ = function (this, condition) 
      __init__(this)
      this.__base__.__ctor__(this)
      this.Condition = condition or System.throw(CSharpLua.ArgumentNullException("condition" --[[nameof(condition)]]))
    end
    getRepeatKeyword = function (this) 
      return "repeat" --[[Keyword.Repeat]]
    end
    getUntilKeyword = function (this) 
      return "until" --[[Keyword.Until]]
    end
    Render = function (this, renderer) 
      renderer:Render48(this)
    end
    return {
      __inherits__ = function (global) 
        return {
          global.CSharpLua.LuaAst.LuaStatementSyntax
        }
      end, 
      getRepeatKeyword = getRepeatKeyword, 
      getUntilKeyword = getUntilKeyword, 
      Render = Render, 
      __ctor__ = __ctor__
    }
  end)
end)
