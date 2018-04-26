// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Globalization;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    }
}
