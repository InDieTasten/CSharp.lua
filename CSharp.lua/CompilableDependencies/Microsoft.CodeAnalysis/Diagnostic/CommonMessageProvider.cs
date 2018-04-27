// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Abstracts the ability to classify and load messages for error codes. Allows the error
    /// infrastructure to be reused between C# and VB.
    /// </summary>
    internal abstract class CommonMessageProvider
    {
        public abstract DiagnosticSeverity GetSeverity(int code);

        public abstract int FTL_InvalidInputFileName { get; }
        public abstract int ERR_FileNotFound { get; }
    }
}
