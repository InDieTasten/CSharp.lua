// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class MessageProvider : CommonMessageProvider
    {
        public static readonly MessageProvider Instance = new MessageProvider();

        //static MessageProvider()
        //{
        //    ObjectBinder.RegisterTypeReader(typeof(MessageProvider), r => Instance);
        //}

        private MessageProvider()
        {
        }

        public override DiagnosticSeverity GetSeverity(int code)
        {
            return ErrorFacts.GetSeverity((ErrorCode)code);
        }

        public override int FTL_InvalidInputFileName => (int)ErrorCode.FTL_InvalidInputFileName;
        public override int ERR_FileNotFound => (int)ErrorCode.ERR_FileNotFound;
    }
}
