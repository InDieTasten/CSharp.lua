-- Generated by CSharp.lua Compiler 1.1.0
-- Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
local System = System
local RoslynUtilities
System.usingDeclare(function (global) 
  RoslynUtilities = Roslyn.Utilities
end)
System.namespace("Roslyn.Utilities", function (namespace) 
  -- Contains path parsing utilities.
  -- We need our own because System.IO.Path is insufficient for our purposes
  -- For example we need to be able to work with invalid paths or paths containing wildcards
  namespace.class("PathUtilities", function (namespace) 
    local getIsUnixLikePlatform, DirectorySeparatorChar, DirectorySeparatorStr, IsDirectorySeparator, GetDirectoryName, GetFileName, GetExtension, GetDirectoryName1, 
    GetPathRoot, GetUnixRoot, GetWindowsRoot, ConsumeDirectorySeparators, IsAbsolute, IsDriveRootedAbsolutePath, GetPathKind, CombinePathsUnchecked, 
    GetPathRoot1, __staticCtor__
    __staticCtor__ = function (this) 
      this.IsDirectorySeparator = IsDirectorySeparator
      this.GetDirectoryName = GetDirectoryName
      this.GetFileName = GetFileName
      this.GetExtension = GetExtension
      this.GetDirectoryName1 = GetDirectoryName1
      this.IsAbsolute = IsAbsolute
      this.GetPathKind = GetPathKind
      this.CombinePathsUnchecked = CombinePathsUnchecked
      this.GetPathRoot1 = GetPathRoot1
      DirectorySeparatorChar = getIsUnixLikePlatform() and 47 --[['/']] or 92 --[['\\']]
      DirectorySeparatorStr = System.String(DirectorySeparatorChar, 1)
      this.DirectorySeparatorChar, this.DirectorySeparatorStr = DirectorySeparatorChar, DirectorySeparatorStr
    end
    getIsUnixLikePlatform = function () 
      return false
    end
    IsDirectorySeparator = function (c) 
      return c == DirectorySeparatorChar or c == 47 --[['/']]
    end
    -- <summary>
    -- Get directory name from path.
    -- </summary>
    -- <remarks>
    -- Unlike <see cref="System.IO.Path.GetDirectoryName"/> it doesn't check for invalid path characters
    -- </remarks>
    -- <returns>Prefix of path that represents a directory</returns>
    GetDirectoryName = function (path) 
      return GetDirectoryName1(path, getIsUnixLikePlatform())
    end
    GetFileName = function (path, includeExtension) 
      return RoslynUtilities.FileNameUtilities.GetFileName(path, includeExtension)
    end
    GetExtension = function (path) 
      return RoslynUtilities.FileNameUtilities.GetExtension(path)
    end
    GetDirectoryName1 = function (path, isUnixLike) 
      if path ~= nil then
        local rootLength = #GetPathRoot(path, isUnixLike)
        if #path > rootLength then
          local i = #path
          while i > rootLength do
            local continue
            repeat
              i = i - 1
              if IsDirectorySeparator(path:get(i)) then
                if i > 0 and IsDirectorySeparator(path:get(i - 1)) then
                  continue = true
                  break
                end

                break
              end
              continue = true
            until 1
            if not continue then
              break
            end
          end

          return path:Substring(0, i)
        end
      end

      return nil
    end
    GetPathRoot = function (path, isUnixLike) 
      if path == nil then
        return nil
      end

      if isUnixLike then
        return GetUnixRoot(path)
      else
        return GetWindowsRoot(path)
      end
    end
    GetUnixRoot = function (path) 
      -- either it starts with "/" and thus has "/" as the root.  Or it has no root.
      local default
      if #path > 0 and IsDirectorySeparator(path:get(0)) then
        default = path:Substring(0, 1)
      else
        default = ""
      end
      return default
    end
    GetWindowsRoot = function (path) 
      -- Windows
      local length = #path
      if length >= 1 and IsDirectorySeparator(path:get(0)) then
        if length < 2 or not IsDirectorySeparator(path:get(1)) then
          --  It was of the form:
          --          \     
          --          \f
          -- in this case, just return \ as the root.
          return path:Substring(0, 1)
        end

        -- First consume all directory separators.
        local i = 2
        i = ConsumeDirectorySeparators(path, length, i)

        -- We've got \\ so far.  If we have a path of the form \\x\y\z
        -- then we want to return "\\x\y" as the root portion.
        local hitSeparator = false
        while true do
          local continue
          repeat
            if i == length then
              -- We reached the end of the path. The entire path is
              -- considered the root.
              return path
            end

            if not IsDirectorySeparator(path:get(i)) then
              -- We got a non separator character.  Just keep consuming.
              i = i + 1
              continue = true
              break
            end

            if not hitSeparator then
              -- This is the first separator group we've hit after some server path.  
              -- Consume them and keep going.
              hitSeparator = true
              i = ConsumeDirectorySeparators(path, length, i)
              continue = true
              break
            end

            -- We hit the second separator.  The root is the path up to this point.
            return path:Substring(0, i)
            continue = true
          until 1
          if not continue then
            break
          end
        end
      elseif length >= 2 and path:get(1) == 58 --[[':']] then
        -- handles c: and c:\
        local default
        if length >= 3 and IsDirectorySeparator(path:get(2)) then
          default = path:Substring(0, 3)
        else
          default = path:Substring(0, 2)
        end
        return default
      else
        -- No path root.
        return ""
      end
    end
    ConsumeDirectorySeparators = function (path, length, i) 
      while i < length and IsDirectorySeparator(path:get(i)) do
        i = i + 1
      end

      return i
    end
    -- <summary>
    -- True if the path is an absolute path (rooted to drive or network share)
    -- </summary>
    IsAbsolute = function (path) 
      if System.String.IsNullOrEmpty(path) then
        return false
      end

      if getIsUnixLikePlatform() then
        return path:get(0) == DirectorySeparatorChar
      end

      -- "C:\"
      if IsDriveRootedAbsolutePath(path) then
        -- Including invalid paths (e.g. "*:\")
        return true
      end

      -- "\\machine\share"
      -- Including invalid/incomplete UNC paths (e.g. "\\goo")
      return #path >= 2 and IsDirectorySeparator(path:get(0)) and IsDirectorySeparator(path:get(1))
    end
    -- <summary>
    -- Returns true if given path is absolute and starts with a drive specification ("C:\").
    -- </summary>
    IsDriveRootedAbsolutePath = function (path) 
      -- Debug.Assert(!IsUnixLikePlatform);
      return #path >= 3 and path:get(1) == 58 --[[':']] and IsDirectorySeparator(path:get(2))
    end
    -- <summary>
    -- Gets the specific kind of relative or absolute path.
    -- </summary>
    GetPathKind = function (path) 
      if System.String.IsNullOrWhiteSpace(path) then
        return 0 --[[PathKind.Empty]]
      end

      -- "C:\"
      -- "\\machine" (UNC)
      -- "/etc"      (Unix)
      if IsAbsolute(path) then
        return 6 --[[PathKind.Absolute]]
      end

      -- "."
      -- ".."
      -- ".\"
      -- "..\"
      if #path > 0 and path:get(0) == 46 --[['.']] then
        if #path == 1 or IsDirectorySeparator(path:get(1)) then
          return 2 --[[PathKind.RelativeToCurrentDirectory]]
        end

        if path:get(1) == 46 --[['.']] then
          if #path == 2 or IsDirectorySeparator(path:get(2)) then
            return 3 --[[PathKind.RelativeToCurrentParent]]
          end
        end
      end

      if not getIsUnixLikePlatform() then
        -- "\"
        -- "\goo"
        if #path >= 1 and IsDirectorySeparator(path:get(0)) then
          return 4 --[[PathKind.RelativeToCurrentRoot]]
        end

        -- "C:goo"

        if #path >= 2 and path:get(1) == 58 --[[':']] and (#path <= 2 or not IsDirectorySeparator(path:get(2))) then
          return 5 --[[PathKind.RelativeToDriveDirectory]]
        end
      end

      -- "goo.dll"
      return 1 --[[PathKind.Relative]]
    end
    CombinePathsUnchecked = function (root, relativePath) 
      -- Debug.Assert(!string.IsNullOrEmpty(root));

      local c = root:get(#root - 1)
      if not IsDirectorySeparator(c) and c ~= 58 --[[':']] then
        return (root .. DirectorySeparatorStr) .. relativePath
      end

      return root .. relativePath
    end
    -- <summary>
    -- Gets the root part of the path.
    -- </summary>
    GetPathRoot1 = function (path) 
      return GetPathRoot(path, getIsUnixLikePlatform())
    end
    return {
      getIsUnixLikePlatform = getIsUnixLikePlatform, 
      __staticCtor__ = __staticCtor__
    }
  end)
end)