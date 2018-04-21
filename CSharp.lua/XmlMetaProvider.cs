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
using System.Diagnostics.Contracts;
using System.IO;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;

namespace CSharpLua
{
    public sealed class XmlMetaProvider
    {
        [XmlRoot("meta")]
        public sealed class XmlMetaModel
        {
            public sealed class TemplateModel
            {
                [XmlAttribute]
                public string Template;
            }

            public sealed class PropertyModel
            {
                [XmlAttribute]
                public string name;
                [XmlAttribute]
                public string Name;
                [XmlElement]
                public TemplateModel set;
                [XmlElement]
                public TemplateModel get;
                [XmlAttribute]
                public string IsField;

                public bool? CheckIsField
                {
                    get
                    {
                        if (IsField != null)
                        {
                            if (IsField.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                            if (IsField.Equals(bool.FalseString, StringComparison.OrdinalIgnoreCase))
                            {
                                return false;
                            }
                        }
                        return null;
                    }
                }
            }

            public sealed class FieldModel
            {
                [XmlAttribute]
                public string name;
                [XmlAttribute]
                public string Template;
            }

            public sealed class ArgumentModel
            {
                [XmlAttribute]
                public string type;
            }

            public sealed class MethodModel
            {
                [XmlAttribute]
                public string name;
                [XmlAttribute]
                public string Name;
                [XmlAttribute]
                public string Template;
                [XmlAttribute]
                public int ArgCount = -1;
                [XmlElement("arg")]
                public ArgumentModel[] Args;
                [XmlAttribute]
                public string RetType;
                [XmlAttribute]
                public int GenericArgCount = -1;
                [XmlAttribute]
                public bool IgnoreGeneric;
            }

            public sealed class ClassModel
            {
                [XmlAttribute]
                public string name;
                [XmlAttribute]
                public string Name;
                [XmlElement("property")]
                public PropertyModel[] Propertys;
                [XmlElement("field")]
                public FieldModel[] Fields;
                [XmlElement("method")]
                public MethodModel[] Methods;
                [XmlAttribute]
                public string Import;
            }

            public sealed class NamespaceModel
            {
                [XmlAttribute]
                public string name;
                [XmlAttribute]
                public string Name;
                [XmlElement("class")]
                public ClassModel[] Classes;
            }

            public sealed class AssemblyModel
            {
                [XmlElement("namespace")]
                public NamespaceModel[] Namespaces;
            }

            public sealed class ExportModel
            {
                public sealed class AttributeModel
                {
                    [XmlAttribute("name")]
                    public string Name;
                }
                [XmlElement("attribute")]
                public AttributeModel[] Attributes;
            }

            [XmlElement("assembly")]
            public AssemblyModel Assembly;

            [XmlElement("export")]
            public ExportModel Export;
        }

        private enum MethodMetaType
        {
            Name,
            CodeTemplate,
            IgnoreGeneric,
        }

        private sealed class MethodMetaInfo
        {
            private List<XmlMetaModel.MethodModel> models_ = new List<XmlMetaModel.MethodModel>();
            private bool isSingleModel_;

            public void Add(XmlMetaModel.MethodModel model)
            {
                models_.Add(model);
                CheckIsSingleModel();
            }

            private void CheckIsSingleModel()
            {
                bool isSingle = false;
                if (models_.Count == 1)
                {
                    var model = models_.First();
                    if (model.ArgCount == -1 && model.Args == null && model.RetType == null && model.GenericArgCount == -1)
                    {
                        isSingle = true;
                    }
                }
                isSingleModel_ = isSingle;
            }

            private bool IsTypeMatch(ITypeSymbol symbol, string typeString)
            {
                INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
                string namespaceName = typeSymbol.ContainingNamespace.ToString();
                string name;
                if (typeSymbol.TypeArguments.Length == 0)
                {
                    name = $"{namespaceName}.{symbol.Name}";
                }
                else
                {
                    name = $"{namespaceName}.{symbol.Name}^{typeSymbol.TypeArguments.Length}";
                }
                return name == typeString;
            }

            private bool IsMethodMatch(XmlMetaModel.MethodModel model, IMethodSymbol symbol)
            {
                if (model.name != symbol.Name)
                {
                    return false;
                }

                if (model.ArgCount != -1)
                {
                    if (symbol.Parameters.Length != model.ArgCount)
                    {
                        return false;
                    }
                }

                if (model.GenericArgCount != -1)
                {
                    if (symbol.TypeArguments.Length != model.GenericArgCount)
                    {
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(model.RetType))
                {
                    if (!IsTypeMatch(symbol.ReturnType, model.RetType))
                    {
                        return false;
                    }
                }

                if (model.Args != null)
                {
                    if (symbol.Parameters.Length != model.Args.Length)
                    {
                        return false;
                    }

                    int index = 0;
                    foreach (var parameter in symbol.Parameters)
                    {
                        var parameterModel = model.Args[index];
                        if (!IsTypeMatch(parameter.Type, parameterModel.type))
                        {
                            return false;
                        }
                        ++index;
                    }
                }

                return true;
            }

            private string GetName(IMethodSymbol symbol)
            {
                if (isSingleModel_)
                {
                    return models_.First().Name;
                }

                var methodModel = models_.Find(i => IsMethodMatch(i, symbol));
                return methodModel?.Name;
            }

            private string GetCodeTemplate(IMethodSymbol symbol)
            {
                if (isSingleModel_)
                {
                    return models_.First().Template;
                }

                var methodModel = models_.Find(i => IsMethodMatch(i, symbol));
                return methodModel?.Template;
            }

            private string GetIgnoreGeneric(IMethodSymbol symbol)
            {
                bool isIgnoreGeneric = false;
                if (isSingleModel_)
                {
                    isIgnoreGeneric = models_.First().IgnoreGeneric;
                }
                else
                {
                    var methodModel = models_.Find(i => IsMethodMatch(i, symbol));
                    if (methodModel != null)
                    {
                        isIgnoreGeneric = methodModel.IgnoreGeneric;
                    }
                }
                return isIgnoreGeneric ? bool.TrueString : bool.FalseString;
            }

            public string GetMetaInfo(IMethodSymbol symbol, MethodMetaType type)
            {
                switch (type)
                {
                    case MethodMetaType.Name:
                        {
                            return GetName(symbol);
                        }
                    case MethodMetaType.CodeTemplate:
                        {
                            return GetCodeTemplate(symbol);
                        }
                    case MethodMetaType.IgnoreGeneric:
                        {
                            return GetIgnoreGeneric(symbol);
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
        }

        private sealed class TypeMetaInfo
        {
            private XmlMetaModel.ClassModel _model;
            private Dictionary<string, XmlMetaModel.FieldModel> _fields = new Dictionary<string, XmlMetaModel.FieldModel>();
            private Dictionary<string, XmlMetaModel.PropertyModel> _properties = new Dictionary<string, XmlMetaModel.PropertyModel>();
            private Dictionary<string, MethodMetaInfo> _methods = new Dictionary<string, MethodMetaInfo>();

            public TypeMetaInfo(XmlMetaModel.ClassModel model)
            {
                _model = model;
                Field();
                Property();
                Method();
            }

            public XmlMetaModel.ClassModel Model
            {
                get
                {
                    return _model;
                }
            }

            private void Field()
            {
                if (_model.Fields != null)
                {
                    foreach (var fieldModel in _model.Fields)
                    {
                        if (string.IsNullOrEmpty(fieldModel.name))
                        {
                            throw new ArgumentException($"type [{_model.name}]'s field name mustn't be empty");
                        }

                        if (_fields.ContainsKey(fieldModel.name))
                        {
                            throw new ArgumentException($"type [{_model.name}]'s field [{fieldModel.name}] already exists.");
                        }
                        _fields.Add(fieldModel.name, fieldModel);
                    }
                }
            }

            private void Property()
            {
                if (_model.Propertys != null)
                {
                    foreach (var propertyModel in _model.Propertys)
                    {
                        if (string.IsNullOrEmpty(propertyModel.name))
                        {
                            throw new ArgumentException($"type [{_model.name}]'s property name mustn't be empty.");
                        }

                        if (_fields.ContainsKey(propertyModel.name))
                        {
                            throw new ArgumentException($"type [{_model.name}]'s property [{propertyModel.name}] already exists.");
                        }
                        _properties.Add(propertyModel.name, propertyModel);
                    }
                }
            }

            private void Method()
            {
                if (_model.Methods != null)
                {
                    foreach (var methodModel in _model.Methods)
                    {
                        if (string.IsNullOrEmpty(methodModel.name))
                        {
                            throw new ArgumentException($"type [{_model.name}]'s method name mustn't be empty.");
                        }

                        var info = _methods.GetOrDefault(methodModel.name);
                        if (info == null)
                        {
                            info = new MethodMetaInfo();
                            _methods.Add(methodModel.name, info);
                        }
                        info.Add(methodModel);
                    }
                }
            }

            public XmlMetaModel.FieldModel GetFieldModel(string name)
            {
                return _fields.GetOrDefault(name);
            }

            public XmlMetaModel.PropertyModel GetPropertyModel(string name)
            {
                return _properties.GetOrDefault(name);
            }

            public MethodMetaInfo GetMethodMetaInfo(string name)
            {
                return _methods.GetOrDefault(name);
            }
        }

        private Dictionary<string, string> _namespaceNameMaps = new Dictionary<string, string>();
        private Dictionary<string, TypeMetaInfo> _typeMetas = new Dictionary<string, TypeMetaInfo>();
        private HashSet<string> _exportAttributes = new HashSet<string>();

        public XmlMetaProvider(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlMetaModel));
                try
                {
                    using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        XmlMetaModel model = (XmlMetaModel)xmlSerializer.Deserialize(stream);
                        var assembly = model.Assembly;
                        if (assembly != null && assembly.Namespaces != null)
                        {
                            foreach (var namespaceModel in assembly.Namespaces)
                            {
                                LoadNamespace(namespaceModel);
                            }
                        }
                        var export = model.Export;
                        if (export != null)
                        {
                            if (export.Attributes != null)
                            {
                                foreach (var attribute in export.Attributes)
                                {
                                    if (string.IsNullOrEmpty(attribute.Name))
                                    {
                                        throw new ArgumentException("attribute's name mustn't be empty.");
                                    }
                                    _exportAttributes.Add(attribute.Name);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Loading xml files encountered an issue at {file}.", ex);
                }
            }
        }

        private void LoadNamespace(XmlMetaModel.NamespaceModel model)
        {
            string namespaceName = model.name;
            if (string.IsNullOrEmpty(namespaceName))
            {
                throw new ArgumentException("namespace's name mustn't be empty.");
            }

            if (!string.IsNullOrEmpty(model.Name))
            {
                if (_namespaceNameMaps.ContainsKey(namespaceName))
                {
                    throw new ArgumentException($"namespace [{namespaceName}] already exists.");
                }
                _namespaceNameMaps.Add(namespaceName, model.Name);
            }

            if (model.Classes != null)
            {
                string name = !string.IsNullOrEmpty(model.Name) ? model.Name : namespaceName;
                LoadType(name, model.Classes);
            }
        }

        private void LoadType(string namespaceName, XmlMetaModel.ClassModel[] classes)
        {
            foreach (var classModel in classes)
            {
                string className = classModel.name;
                if (string.IsNullOrEmpty(className))
                {
                    throw new ArgumentException($"namespace [{namespaceName}]'s class name mustn't be empty.");
                }

                string classesfullName = namespaceName + '.' + className;
                classesfullName = classesfullName.Replace('^', '_');
                if (_typeMetas.ContainsKey(classesfullName))
                {
                    throw new ArgumentException($"type [{classesfullName}] already exists.");
                }
                TypeMetaInfo info = new TypeMetaInfo(classModel);
                _typeMetas.Add(classesfullName, info);
            }
        }

        public string GetNamespaceMapName(INamespaceSymbol symbol, string original)
        {
            return _namespaceNameMaps.GetOrDefault(original);
        }

        internal bool MayHaveCodeMeta(ISymbol symbol)
        {
            return symbol.DeclaredAccessibility == Accessibility.Public && !symbol.IsFromCode();
        }

        private string GetTypeShortString(ISymbol symbol)
        {
            INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
            return typeSymbol.GetTypeShortName(GetNamespaceMapName);
        }

        internal string GetTypeMapName(ISymbol symbol, string shortName)
        {
            if (MayHaveCodeMeta(symbol))
            {
                TypeMetaInfo info = _typeMetas.GetOrDefault(shortName);
                return info?.Model.Name;
            }
            return null;
        }

        private TypeMetaInfo GetTypeMetaInfo(ISymbol memberSymbol)
        {
            string typeName = GetTypeShortString(memberSymbol.ContainingType);
            return _typeMetas.GetOrDefault(typeName);
        }

        public bool? IsPropertyField(IPropertySymbol symbol)
        {
            if (MayHaveCodeMeta(symbol))
            {
                var info = GetTypeMetaInfo(symbol)?.GetPropertyModel(symbol.Name);
                return info?.CheckIsField;
            }
            return null;
        }

        public string GetFieldCodeTemplate(IFieldSymbol symbol)
        {
            if (MayHaveCodeMeta(symbol))
            {
                return GetTypeMetaInfo(symbol)?.GetFieldModel(symbol.Name)?.Template;
            }
            return null;
        }

        public string GetProertyCodeTemplate(IPropertySymbol symbol, bool isGet)
        {
            if (MayHaveCodeMeta(symbol))
            {
                var info = GetTypeMetaInfo(symbol)?.GetPropertyModel(symbol.Name);
                if (info != null)
                {
                    return isGet ? info.get?.Template : info.set?.Template;
                }
            }
            return null;
        }

        private string GetInternalMethodMetaInfo(IMethodSymbol symbol, MethodMetaType metaType)
        {
            Contract.Assert(symbol != null);
            if (symbol.DeclaredAccessibility != Accessibility.Public)
            {
                return null;
            }

            string codeTemplate = null;
            if (!symbol.IsFromCode())
            {
                codeTemplate = GetTypeMetaInfo(symbol)?.GetMethodMetaInfo(symbol.Name)?.GetMetaInfo(symbol, metaType);
            }

            if (codeTemplate == null)
            {
                if (symbol.IsOverride)
                {
                    if (symbol.OverriddenMethod != null)
                    {
                        codeTemplate = GetInternalMethodMetaInfo(symbol.OverriddenMethod, metaType);
                    }
                }
                else
                {
                    var interfaceImplementations = symbol.InterfaceImplementations();
                    if (interfaceImplementations != null)
                    {
                        foreach (IMethodSymbol interfaceMethod in interfaceImplementations)
                        {
                            codeTemplate = GetInternalMethodMetaInfo(interfaceMethod, metaType);
                            if (codeTemplate != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return codeTemplate;
        }

        private string GetMethodMetaInfo(IMethodSymbol symbol, MethodMetaType metaType)
        {
            Utility.CheckMethodDefinition(ref symbol);
            return GetInternalMethodMetaInfo(symbol, metaType);
        }

        public string GetMethodMapName(IMethodSymbol symbol)
        {
            return GetMethodMetaInfo(symbol, MethodMetaType.Name);
        }

        public string GetMethodCodeTemplate(IMethodSymbol symbol)
        {
            return GetMethodMetaInfo(symbol, MethodMetaType.CodeTemplate);
        }

        public bool IsMethodIgnoreGeneric(IMethodSymbol symbol)
        {
            return GetMethodMetaInfo(symbol, MethodMetaType.IgnoreGeneric) == bool.TrueString;
        }

        public bool IsExportAttribute(INamedTypeSymbol attributeTypeSymbol)
        {
            return _exportAttributes.Count > 0 && _exportAttributes.Contains(attributeTypeSymbol.ToString());
        }
    }
}
