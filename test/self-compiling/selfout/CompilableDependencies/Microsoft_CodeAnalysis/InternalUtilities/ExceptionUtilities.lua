-- Generated by CSharp.lua Compiler 1.1.0
-- Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
local System = System
System.namespace("Roslyn.Utilities", function (namespace) 
  namespace.class("ExceptionUtilities", function (namespace) 
    local UnexpectedValue, getUnreachable
    UnexpectedValue = function (o) 
      local default
      if (o ~= nil) then
        default = o:GetType():getFullName()
      else
        default = "<unknown>"
      end
      local output = System.String.Format("Unexpected value '{0}' of type '{1}'", o, default)
      --Debug.Assert(false, output);

      -- We do not throw from here because we don't want all Watson reports to be bucketed to this call.
      return System.InvalidOperationException(output)
    end
    getUnreachable = function () 
      return System.InvalidOperationException("This program location is thought to be unreachable.")
    end
    return {
      UnexpectedValue = UnexpectedValue, 
      getUnreachable = getUnreachable
    }
  end)
end)
