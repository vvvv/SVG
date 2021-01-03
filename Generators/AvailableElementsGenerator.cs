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
    /// <summary>
    /// Generates available elements ElementInfo metadata for SvgElementFactory.
    /// </summary>
    [Generator]
    public class AvailableElementsGenerator : ISourceGenerator
    {
        /// <summary>
        /// The ElementFactory attribute is used for decorating target SvgElementFactory class.
        /// </summary>
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

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
            // NOTE: Uncomment the next line to enable source generator debugging (build project to trigger debugger to be attached).
            // System.Diagnostics.Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            // Add the ElementFactory attribute source to compilation. 
            context.AddSource("ElementFactoryAttribute", SourceText.From(AttributeText, Encoding.UTF8));

            // Check is we have our SyntaxReceiver object used to filter compiled code.
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

            var svgElementBaseSymbol = compilation.GetTypeByMetadataName("Svg.SvgElement");
            if (svgElementBaseSymbol is null)
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

                // Find classes with ElementFactory attribute.
                if (namedTypeSymbol.GetAttributes().Any(ad => ad?.AttributeClass?.Equals(elementFactoryAttribute, SymbolEqualityComparer.Default) ?? false))
                {
                    elementFactorySymbols.Add(namedTypeSymbol);
                    continue;
                }

                // Find classes derived from SvgElement.
                if (!namedTypeSymbol.IsAbstract && !namedTypeSymbol.IsGenericType && HasBaseType(namedTypeSymbol, svgElementBaseSymbol))
                {
                    svgElementSymbols.Add(namedTypeSymbol);
                }
            }

            // Generate code for each class marked with ElementFactor attribute.
            foreach (var elementFactorySymbol in elementFactorySymbols)
            {
                var classSource = ProcessClass(compilation, elementFactorySymbol, svgElementSymbols, svgElementBaseSymbol);
                if (classSource is not null)
                {
                    context.AddSource($"{elementFactorySymbol.Name}_ElementFactory.cs", SourceText.From(classSource, Encoding.UTF8));
                }
            }
        }

        /// <summary>
        /// Check if symbol has target base class
        /// </summary>
        /// <param name="namedTypeSymbol">The candidate class symbol.</param>
        /// <param name="targetBaseType">The base type class symbol</param>
        /// <returns>True is candidate class derives from base type, otherwise false.</returns>
        private static bool HasBaseType(INamedTypeSymbol namedTypeSymbol, INamedTypeSymbol targetBaseType)
        {
            var baseType = namedTypeSymbol.BaseType;
            while (true)
            {
                if (baseType is null)
                {
                    break;
                }

                // We need to use SymbolEqualityComparer for symbol comparison.
                if (SymbolEqualityComparer.Default.Equals(baseType, targetBaseType))
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Get symbol base types until provided type.
        /// </summary>
        /// <param name="namedTypeSymbol">The candidate class symbol.</param>
        /// <param name="targetBaseType">The base type class symbol</param>
        /// <returns>Returns a list of all base class symbol until target base type is reached.</returns>
        private static IEnumerable<INamedTypeSymbol> GetBaseTypes(INamedTypeSymbol namedTypeSymbol, INamedTypeSymbol targetBaseType)
        {
            var baseType = namedTypeSymbol.BaseType;
            while (true)
            {
                if (baseType is null)
                {
                    break;
                }

                // We need to use SymbolEqualityComparer for symbol comparison.
                if (SymbolEqualityComparer.Default.Equals(baseType, targetBaseType))
                {
                    yield return baseType;
                    break;
                }

                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        private static string? GetTypeConverter(Compilation compilation, IPropertySymbol propertySymbol)
        {
            var typeConverterAttribute = compilation.GetTypeByMetadataName("System.ComponentModel.TypeConverterAttribute");
            if (typeConverterAttribute is null)
            {
                return null;
            }

            var propertySymbolTypeConverter = GetTypeConverter(propertySymbol, typeConverterAttribute);
            if (propertySymbolTypeConverter is not null)
            {
                return propertySymbolTypeConverter;
            }
            
            var propertySymbolTypeTypeConverter = GetTypeConverter(propertySymbol.Type, typeConverterAttribute);
            if (propertySymbolTypeTypeConverter is not null)
            {
                return propertySymbolTypeTypeConverter;
            }

            // TODO: This does not work as it uses reflection.
            // var type = Type.GetType(propertySymbol.Type.ToDisplayString());
            // if (type is not null)
            // {
            //     return TypeDescriptor.GetConverter(type).ToString();
            // }
            
            return null;
        }

        private static string? GetTypeConverter(ISymbol symbol, INamedTypeSymbol typeConverterAttribute)
        {
            var attributes = symbol.GetAttributes();
            if (attributes.Length == 0)
            {
                return null;
            }

            // Find typeConverterAttribute attribute data. We need only first constructor argument for attribute type.
            var attributeData = attributes.FirstOrDefault(ad => ad?.AttributeClass?.Equals(typeConverterAttribute, SymbolEqualityComparer.Default) ?? false);
            if (attributeData is null || attributeData.ConstructorArguments.Length < 1)
            {
                return null;
            }

            // The Type is set in attribute by providing constructor argument.
  
            return attributeData.ConstructorArguments[0].Value?.ToString();
        }

        private static IEnumerable<Property> GetElementProperties(Compilation compilation, INamedTypeSymbol svgElementSymbol, INamedTypeSymbol svgElementBaseSymbol, INamedTypeSymbol svgAttributeAttribute)
        {
            var types = GetBaseTypes(svgElementSymbol, svgElementBaseSymbol).Prepend(svgElementSymbol);

            foreach (var type in types)
            {
                var members = type.GetMembers();
                foreach (var member in members)
                {
                    if (member is not IPropertySymbol propertySymbol)
                    {
                        continue;
                    }
                    
                    var attributes = propertySymbol.GetAttributes();
                    if (attributes.Length == 0)
                    {
                        continue;
                    }

                    // Find svgAttributeAttribute attribute data. We need only first constructor argument for attribute 'name'.
                    var attributeData = attributes.FirstOrDefault(ad => ad?.AttributeClass?.Equals(svgAttributeAttribute, SymbolEqualityComparer.Default) ?? false);
                    if (attributeData is null || attributeData.ConstructorArguments.Length < 1)
                    {
                        continue;
                    }

                    // The Name is set in attribute by providing constructor argument.
                    var name = (string?) attributeData.ConstructorArguments[0].Value;
                    if (name is null)
                    {
                        continue;
                    }

                    var property = new Property(
                        propertySymbol,
                        name,
                        GetTypeConverter(compilation, propertySymbol)
                    );
 
                    yield return property;
                }
            }
        }

        private class Property
        {
            public IPropertySymbol Symbol { get; set; }
            public string Name { get; set; }
            public string? Converter { get; set; }
  
            public Property(IPropertySymbol symbol, string name, string? converter)
            {
                Symbol = symbol;
                Name = name;
                Converter = converter;
            }
        }
        
        private class Element
        {
            public INamedTypeSymbol Symbol { get; set; }
            public string ElementName { get; set; }
            public List<string> ClassNames { get; set; }
            public List<Property> Properties { get; set; }

            public Element(INamedTypeSymbol symbol, string elementName, List<string> classNames, List<Property> properties)
            {
                Symbol = symbol;
                ElementName = elementName;
                ClassNames = classNames;
                Properties = properties;
            }
        }

        private static string? ProcessClass(Compilation compilation, INamedTypeSymbol elementFactorySymbol, List<INamedTypeSymbol> svgElementSymbols, INamedTypeSymbol svgElementBaseSymbol)
        {
            // Get the containing namespace for ElementFactory class.
            if (!elementFactorySymbol.ContainingSymbol.Equals(elementFactorySymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null;
            }

            // Get SvgElementAttribute symbol using for later attribute retrieval.
            var svgElementAttribute = compilation.GetTypeByMetadataName("Svg.SvgElementAttribute");
            if (svgElementAttribute is null)
            {
                return null;
            }

            // Get SvgAttributeAttribute symbol using for later attribute retrieval.
            var svgAttributeAttribute = compilation.GetTypeByMetadataName("Svg.SvgAttributeAttribute");
            if (svgAttributeAttribute is null)
            {
                return null;
            }

            // Convert symbol to proper display string.
            string namespaceElementFactory = elementFactorySymbol.ContainingNamespace.ToDisplayString();

            // We need to format properly symbols to support generic types.
            var format = new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeTypeConstraints | SymbolDisplayGenericsOptions.IncludeVariance
            );

            string classElementFactory = elementFactorySymbol.ToDisplayString(format);

            var source = new StringBuilder($@"// <auto-generated />
using System;
using System.Collections.Generic;

namespace {namespaceElementFactory}
{{");

            // Key: ElementName
            SortedDictionary<string, Element> items = new();

            // Get all classes with SvgElementAttribute attribute set.
            foreach (var svgElementSymbol in svgElementSymbols)
            {
                string namespaceSvgElement = svgElementSymbol.ContainingNamespace.ToDisplayString();
                string classNameSvgElement = $"{namespaceSvgElement}.{svgElementSymbol.ToDisplayString(format)}";

                var attributes = svgElementSymbol.GetAttributes();
                if (attributes.Length == 0)
                {
                    continue;
                }

                // Find SvgElementAttribute attribute data. The SvgElementAttribute has constructor with one argument of type string.
                var attributeData = attributes.FirstOrDefault(ad => ad?.AttributeClass?.Equals(svgElementAttribute, SymbolEqualityComparer.Default) ?? false);
                if (attributeData is null || attributeData.ConstructorArguments.Length != 1)
                {
                    continue;
                }

                // The ElementName is set in attribute by providing constructor argument.
                var elementName = (string?) attributeData.ConstructorArguments[0].Value;
                if (elementName is null)
                {
                    continue;
                }

                if (items.TryGetValue(elementName, out var element))
                {
                    element.ClassNames.Add(classNameSvgElement);
                }
                else
                {
                    element = new Element(
                        svgElementSymbol,
                        elementName,
                        new List<string> { classNameSvgElement },
                        GetElementProperties(compilation, svgElementSymbol, svgElementBaseSymbol, svgAttributeAttribute).ToList()
                    );
                    items.Add(elementName, element);
                }
            }

            // TODO:
            
#if DEBUG
            source.AppendLine($"");
            foreach (var item in items)
            {
                var element = item.Value;
                source.AppendLine($"    // {element.Symbol.ToDisplayString(format)}");
                foreach (var property in element.Properties)
                {
                    source.AppendLine($"    // - {property.Symbol}, '{property.Name}', {property.Symbol.Type}, {property.Converter ?? "<INVALID>"}");
                }
            }
#endif
            
            // Start ElementFactory class.

            source.Append($@"    internal partial class {classElementFactory}
    {{");
            // Generate availableElements list.

            source.Append($@"
        private static readonly List<ElementInfo> availableElements = new List<ElementInfo>()
        {{
");
            foreach (var element in items)
            {
                var elementName = element.Key;
                var className = element.Value.ClassNames.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(className))
                {
                    continue;
                }

                source.AppendLine($@"            new ElementInfo() {{ ElementName = ""{elementName}"", ElementType = typeof({className}), CreateInstance = () => new {className}() }},");
            }
            source.Append($@"        }};");

            // Generate availableElementsWithoutSvg dictionary.

            source.Append($@"

        private static readonly Dictionary<string, ElementInfo> availableElementsWithoutSvg = new Dictionary<string, ElementInfo>()
        {{
");
            foreach (var element in items)
            {
                var elementName = element.Key;
                var className = element.Value.ClassNames.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(className))
                {
                    continue;
                }
                if (elementName.Equals("svg", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                source.AppendLine($@"            [""{elementName}""] = new ElementInfo() {{ ElementName = ""{elementName}"", ElementType = typeof({className}), CreateInstance = () => new {className}() }},");
            }

            source.Append($@"        }};");

            // Generate availableElementsDictionary dictionary.

            source.Append($@"

        private static readonly Dictionary<string, List<Type>> availableElementsDictionary = new Dictionary<string, List<Type>>()
        {{
");

            foreach (var element in items)
            {
                var elementName = element.Key;
                var classNames = element.Value.ClassNames;
 
                source.Append($@"            [""{elementName}""] = new List<Type>() {{ ");

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

            // Generate end of class and namespace.

            source.Append($@"
    }}
}}");

            return source.ToString();
        }

        /// <summary>
        /// The SyntaxReceiver is used to filter compiled code. This enable quick and easy way to filter compiled code.
        /// </summary>
        private class SyntaxReceiver : ISyntaxReceiver
        {
            /// <summary>
            /// Gets the list of all candidate class.
            /// </summary>
            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

            /// <inheritdoc/>
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
