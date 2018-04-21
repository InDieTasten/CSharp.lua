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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst
{
    public sealed class LuaParameterListSyntax : LuaSyntaxNode
    {
        public string OpenParenToken => Tokens.OpenParentheses;
        public string CloseParenToken => Tokens.CloseParentheses;
        public readonly LuaSyntaxList<LuaParameterSyntax> Parameters = new LuaSyntaxList<LuaParameterSyntax>();

        internal override void Render(LuaRenderer renderer)
        {
            renderer.Render(this);
        }
    }

    public sealed class LuaParameterSyntax : LuaSyntaxNode
    {
        public LuaIdentifierNameSyntax Identifier { get; }

        public LuaParameterSyntax(LuaIdentifierNameSyntax identifier)
        {
            Identifier = identifier;
        }

        internal override void Render(LuaRenderer renderer)
        {
            renderer.Render(this);
        }
    }
}
