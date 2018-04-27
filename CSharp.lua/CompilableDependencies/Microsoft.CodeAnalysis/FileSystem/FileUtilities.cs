// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslyn.Utilities
{
    internal static class FileUtilities
    {
        internal static string ResolveRelativePath(string path, string baseDirectory)
        {
            return ResolveRelativePath(path, null, baseDirectory);
        }

        internal static string ResolveRelativePath(string path, string basePath, string baseDirectory)
        {
            // Debug.Assert(baseDirectory == null || PathUtilities.IsAbsolute(baseDirectory));
            return ResolveRelativePath(PathUtilities.GetPathKind(path), path, basePath, baseDirectory);
        }

        private static string ResolveRelativePath(PathKind kind, string path, string basePath, string baseDirectory)
        {
            switch (kind)
            {
                case PathKind.Empty:
                    return null;

                case PathKind.Relative:
                    baseDirectory = GetBaseDirectory(basePath, baseDirectory);
                    if (baseDirectory == null)
                    {
                        return null;
                    }

                    // with no search paths relative paths are relative to the base directory:
                    return PathUtilities.CombinePathsUnchecked(baseDirectory, path);

                case PathKind.RelativeToCurrentDirectory:
                    baseDirectory = GetBaseDirectory(basePath, baseDirectory);
                    if (baseDirectory == null)
                    {
                        return null;
                    }

                    if (path.Length == 1)
                    {
                        // "."
                        return baseDirectory;
                    }
                    else
                    {
                        // ".\path"
                        return PathUtilities.CombinePathsUnchecked(baseDirectory, path);
                    }

                case PathKind.RelativeToCurrentParent:
                    baseDirectory = GetBaseDirectory(basePath, baseDirectory);
                    if (baseDirectory == null)
                    {
                        return null;
                    }

                    // ".."
                    return PathUtilities.CombinePathsUnchecked(baseDirectory, path);

                case PathKind.RelativeToCurrentRoot:
                    string baseRoot;
                    if (basePath != null)
                    {
                        baseRoot = PathUtilities.GetPathRoot(basePath);
                    }
                    else if (baseDirectory != null)
                    {
                        baseRoot = PathUtilities.GetPathRoot(baseDirectory);
                    }
                    else
                    {
                        return null;
                    }

                    if (string.IsNullOrEmpty(baseRoot))
                    {
                        return null;
                    }

                    //Debug.Assert(PathUtilities.IsDirectorySeparator(path[0]));
                    //Debug.Assert(path.Length == 1 || !PathUtilities.IsDirectorySeparator(path[1]));
                    return PathUtilities.CombinePathsUnchecked(baseRoot, path.Substring(1));

                case PathKind.RelativeToDriveDirectory:
                    // drive relative paths not supported, can't resolve:
                    return null;

                case PathKind.Absolute:
                    return path;

                default:
                    throw ExceptionUtilities.UnexpectedValue(kind);
            }
        }

        private static string GetBaseDirectory(string basePath, string baseDirectory)
        {
            // relative base paths are relative to the base directory:
            string resolvedBasePath = ResolveRelativePath(basePath, baseDirectory);
            if (resolvedBasePath == null)
            {
                return baseDirectory;
            }

            // Note: Path.GetDirectoryName doesn't normalize the path and so it doesn't depend on the process state.
            // Debug.Assert(PathUtilities.IsAbsolute(resolvedBasePath));
            try
            {
                return PathUtilities.GetDirectoryName(resolvedBasePath); // [Lua] This was using System.IO.Path version before :S
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
