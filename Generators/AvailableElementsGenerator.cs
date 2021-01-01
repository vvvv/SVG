﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Svg.Generators
{
    [Generator]
    public class AvailableElementsGenerator : ISourceGenerator
    {
        private const string AttributeText = @"// <auto-generated />
#nullable disable
using System;

namespace Svg
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ElementFactoryAttribute : Attribute
    {
    }
}";

        public void Initialize(GeneratorInitializationContext context)
        {
            // System.Diagnostics.Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("ElementFactoryAttribute", SourceText.From(AttributeText, Encoding.UTF8));

            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
            {
                return;
            }

            var options = (context.Compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
            var compilation = context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(AttributeText, Encoding.UTF8), options));

            var elementFactoryAttribute = compilation.GetTypeByMetadataName("Svg.ElementFactoryAttribute");
            if (elementFactoryAttribute is null)
            {
                return;
            }

            var svgElementSymbol = compilation.GetTypeByMetadataName("Svg.SvgElement");
            if (svgElementSymbol is null)
            {
                return;
            }

            List<INamedTypeSymbol> elementFactorySymbols = new();
            List<INamedTypeSymbol> svgElementSymbols = new();

            foreach (var candidateClass in receiver.CandidateClasses)
            {
                var semanticModel = compilation.GetSemanticModel(candidateClass.SyntaxTree);
                var namedTypeSymbol = semanticModel.GetDeclaredSymbol(candidateClass);
                if (namedTypeSymbol is null)
                {
                    continue;
                }

                var attributes = namedTypeSymbol.GetAttributes();
                if (attributes.Any(ad => ad?.AttributeClass?.Equals(elementFactoryAttribute, SymbolEqualityComparer.Default) ?? false))
                {
                    elementFactorySymbols.Add(namedTypeSymbol);
                }
                else
                {
                    if (!namedTypeSymbol.IsAbstract && !namedTypeSymbol.IsGenericType)
                    {
                        var baseType = namedTypeSymbol.BaseType;
                        while (true)
                        {
                            if (baseType is null)
                            {
                                break;
                            }
                            if (SymbolEqualityComparer.Default.Equals(baseType, svgElementSymbol))
                            {
                                svgElementSymbols.Add(namedTypeSymbol);
                                break;
                            }
                            baseType = baseType.BaseType;
                        }
                    }
                }
            }

            foreach (var elementFactorySymbol in elementFactorySymbols)
            {
                var classSource = ProcessClass(compilation, elementFactorySymbol, svgElementSymbols);
                if (classSource is not null)
                {
                    context.AddSource($"{elementFactorySymbol.Name}_ElementFactory.cs", SourceText.From(classSource, Encoding.UTF8));
                }
            }
        }

        private static string? ProcessClass(Compilation compilation, INamedTypeSymbol elementFactorySymbol, List<INamedTypeSymbol> svgElementSymbols)
        {
            if (!elementFactorySymbol.ContainingSymbol.Equals(elementFactorySymbol.ContainingNamespace,
                SymbolEqualityComparer.Default))
            {
                return null;
            }
            var svgElementAttribute = compilation.GetTypeByMetadataName("Svg.SvgElementAttribute");
            if (svgElementAttribute is null)
            {
                return null;
            }

            string namespaceElementFactory = elementFactorySymbol.ContainingNamespace.ToDisplayString();

            var format = new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeTypeConstraints | SymbolDisplayGenericsOptions.IncludeVariance
            );

            string classElementFactory = elementFactorySymbol.ToDisplayString(format);

            var source = new StringBuilder($@"// <auto-generated />
#nullable disable
using System;
using System.Collections.Generic;

namespace {namespaceElementFactory}
{{
    internal partial class {classElementFactory}
    {{");
            Dictionary<string, List<string>> elements = new();
   
            // elements

            foreach (var svgElementSymbol in svgElementSymbols)
            {
                string namespaceSvgElement = svgElementSymbol.ContainingNamespace.ToDisplayString();
                string classNameSvgElement = $"{namespaceSvgElement}.{svgElementSymbol.ToDisplayString(format)}";

                var attributes = svgElementSymbol.GetAttributes();
                if (attributes.Length == 0)
                {
                    continue;
                }

                var attributeData = attributes.FirstOrDefault(ad => ad?.AttributeClass?.Equals(svgElementAttribute, SymbolEqualityComparer.Default) ?? false);
                if (attributeData is null || attributeData.ConstructorArguments.Length != 1)
                {
                    continue;
                }

                var elementName = (string?) attributeData.ConstructorArguments[0].Value;
                if (elementName is null)
                {
                    continue;
                }

                if (elements.TryGetValue(elementName, out var classNames))
                {
                    classNames.Add(classNameSvgElement);
                }
                else
                {
                    elements.Add(elementName, new List<string> { classNameSvgElement });
                }
            }

            // availableElements

            source.Append($@"
        private static readonly List<ElementInfo> availableElements = new()
        {{
");
            foreach (var element in elements)
            {
                var elementName = element.Key;
                var className = element.Value.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(className))
                {
                    continue;
                }

                source.AppendLine($@"            new ElementInfo {{ ElementName = ""{elementName}"", ElementType = typeof({className}), CreateInstance = () => new {className}() }},");
            }
            source.Append($@"        }};");

            // availableElementsWithoutSvg

            source.Append($@"

        private static readonly Dictionary<string, ElementInfo> availableElementsWithoutSvg = new()
        {{
");
            foreach (var element in elements)
            {
                var elementName = element.Key;
                var className = element.Value.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(className))
                {
                    continue;
                }
                if (elementName.Equals("svg", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                source.AppendLine($@"            [""{elementName}""] = new ElementInfo {{ ElementName = ""{elementName}"", ElementType = typeof({className}), CreateInstance = () => new {className}() }},");
            }

            source.Append($@"        }};");

            // availableElementsDictionary

            source.Append($@"

        private static readonly Dictionary<string, List<Type>> availableElementsDictionary = new()
        {{
");

            foreach (var element in elements)
            {
                var elementName = element.Key;
                var classNames = element.Value;
 
                source.Append($@"            [""{elementName}""] = new List<Type> {{ ");

                for (var i = 0; i < classNames.Count; i++)
                {
                    var className = classNames[i];
                    if (!string.IsNullOrWhiteSpace(className))
                    {
                        source.Append($"typeof({className}){((i < classNames.Count && classNames.Count > 1) ? ", " : "")}");
                    }
                }

                source.AppendLine($" }},");
            }

            source.Append($@"        }};");

            // end of class and namespace

            source.Append($@"
    }}
}}");

            return source.ToString();
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    CandidateClasses.Add(classDeclarationSyntax);
                }
            }
        }
    }
}
