/*
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
*/

using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst
{
    public abstract class LuaLiteralExpressionSyntax : LuaExpressionSyntax
    {
        public abstract string Text { get; }
    }

    public sealed class LuaIdentifierLiteralExpressionSyntax : LuaLiteralExpressionSyntax
    {
        public LuaIdentifierNameSyntax Identifier { get; }

        public LuaIdentifierLiteralExpressionSyntax(string text) : this(new LuaIdentifierNameSyntax(text))
        {
        }

        public LuaIdentifierLiteralExpressionSyntax(LuaIdentifierNameSyntax identifier)
        {
            Identifier = identifier;
        }

        public override string Text
        {
            get
            {
                return Identifier.ValueText;
            }
        }

        internal override void Render(LuaRenderer renderer)
        {
            renderer.Render(this);
        }

        public static readonly LuaIdentifierLiteralExpressionSyntax Nil = new LuaIdentifierLiteralExpressionSyntax(LuaIdentifierNameSyntax.Nil);
        public static readonly LuaIdentifierLiteralExpressionSyntax Zero = new LuaIdentifierLiteralExpressionSyntax("0");
        public static readonly LuaIdentifierLiteralExpressionSyntax ZeroFloat = new LuaIdentifierLiteralExpressionSyntax("0.0");
        public static readonly LuaIdentifierLiteralExpressionSyntax True = new LuaIdentifierLiteralExpressionSyntax(LuaIdentifierNameSyntax.True);
        public static readonly LuaIdentifierLiteralExpressionSyntax False = new LuaIdentifierLiteralExpressionSyntax(LuaIdentifierNameSyntax.False);
    }

    public sealed class LuaStringLiteralExpressionSyntax : LuaLiteralExpressionSyntax
    {
        public string OpenParenToken => Tokens.Quote;
        public LuaIdentifierNameSyntax Identifier { get; }
        public string CloseParenToken => Tokens.Quote;

        public LuaStringLiteralExpressionSyntax(LuaIdentifierNameSyntax identifier)
        {
            Identifier = identifier;
        }

        public override string Text
        {
            get
            {
                return Identifier.ValueText;
            }
        }

        internal override void Render(LuaRenderer renderer)
        {
            renderer.Render(this);
        }

        public static readonly LuaStringLiteralExpressionSyntax Empty = new LuaStringLiteralExpressionSyntax(LuaIdentifierNameSyntax.Empty);
    }

    public sealed class LuaVerbatimStringLiteralExpressionSyntax : LuaLiteralExpressionSyntax
    {
        public override string Text { get; }
        public int EqualsCount { get; }
        public string OpenBracket => Tokens.OpenBracket;
        public string CloseBracket => Tokens.CloseBracket;

        public LuaVerbatimStringLiteralExpressionSyntax(string value, bool checkNewLine = true)
        {
            const string closeBracket = Tokens.CloseBracket;
            char equals = Tokens.Equals[0];
            int count = 0;
            while (true)
            {
                string closeToken = closeBracket + new string(equals, count) + closeBracket;
                if (!value.Contains(closeToken))
                {
                    break;
                }
                ++count;
            }
            if (checkNewLine)
            {
                if (value[0] == '\n')
                {
                    value = '\n' + value;
                }
            }
            Text = value;
            EqualsCount = count;
        }

        internal override void Render(LuaRenderer renderer)
        {
            renderer.Render(this);
        }
    }

    public class LuaConstLiteralExpression : LuaLiteralExpressionSyntax
    {
        public LuaLiteralExpressionSyntax Value { get; }
        public string OpenComment => Tokens.OpenLongComment;
        public string IdentifierToken { get; }
        public string CloseComment => Tokens.CloseDoubleBrace;

        public LuaConstLiteralExpression(string value, string identifierToken) : this(new LuaIdentifierLiteralExpressionSyntax(value), identifierToken)
        {
        }

        public LuaConstLiteralExpression(LuaLiteralExpressionSyntax value, string identifierToken)
        {
            Value = value;
            IdentifierToken = identifierToken;
        }

        public override string Text
        {
            get
            {
                return Value.Text;
            }
        }

        internal override void Render(LuaRenderer renderer)
        {
            renderer.Render(this);
        }
    }

    public sealed class LuaCharacterLiteralExpression : LuaConstLiteralExpression
    {
        public LuaCharacterLiteralExpression(char character) : base(((int)character).ToString(), GetIdentifierToken(character))
        {
        }

        private static string GetIdentifierToken(char character)
        {
            return SyntaxFactory.Literal(character).Text;
        }
    }
}