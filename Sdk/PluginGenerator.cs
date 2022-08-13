using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Application.Sdk;

[Generator]
public class PluginGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        #if DEBUG
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
        #endif
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var syntaxTrees = context.Compilation.SyntaxTrees;
        var handlers = syntaxTrees.Where(n => n.GetText().ToString().Contains("IJSOperation"));

        foreach (var handler in handlers)
        {
            var classDeclarationsList = handler.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            if (!classDeclarationsList.Any() || !ImplementsInterface(classDeclarationsList.First(), "IJSOperation")) { continue; }

            var classDeclaration = classDeclarationsList.First();

            var properties = classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>();

            var outputProperty = properties.Where(p => p.AttributeLists.Any(pp => pp.ToString().StartsWith("[Output")));
            if (outputProperty.Count() > 1)
            {
                var desc = new DiagnosticDescriptor(
                    "PLUGIN001",
                    "At most one output parameter allowed",
                    "At most one property can be decorated with the [Output] attribute",
                    "JSOperation Plugin Framework",
                    DiagnosticSeverity.Error,
                    true);

                context.ReportDiagnostic(Diagnostic.Create(desc, classDeclaration.GetLocation()));
            }
            
            var inputProperties = properties.Where(p => p.AttributeLists.Any(pp => pp.ToString().StartsWith("[Input"))); 
            if (! inputProperties.Any())
            {
                var desc = new DiagnosticDescriptor(
                    "PLUGIN002",
                    "There are no input properties here",
                    "This is just a test to see if things are detected correctly",
                    "Plugin Framework",
                    DiagnosticSeverity.Warning,
                    true);

                context.ReportDiagnostic(Diagnostic.Create(desc, classDeclaration.GetLocation()));
            }

            StringBuilder initializationStatements = new();
            List<string> parameters = new();
            List<string> signature = new();
            if (inputProperties.Count() > 0)
            {
                foreach (var propertySyntax in inputProperties)
                {
                    var propertyName = propertySyntax.Identifier.ToString();
                    var propertyType = propertySyntax.Type.ToString();
                    signature.Add(propertyType);
                    
                    var parameterName = propertyName + "_param";
                    parameters.Add($"{propertyType} {parameterName}");

                    initializationStatements.Append($"{propertyName}={parameterName};");
                }
            }

            if (outputProperty.Count() == 1)
            {
                var sourceCodeBuilder = new StringBuilder();
            
                var usingDirectives = handler.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
                var usingDirectivesString = string.Join(Environment.NewLine, usingDirectives);
                sourceCodeBuilder.Append(usingDirectivesString);

                var namespaceDefinition = GetNamespace(classDeclaration); //handler.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>().First();

                var property = outputProperty.First();
                var propertyName = property.Identifier.ToString();
                var returnType = property.Type.ToString();

                var className = classDeclaration.Identifier.ToString();

                // TODO: Refactor this
                if (inputProperties.Count() > 0)
                {
                    sourceCodeBuilder.Append(
                    @$"
                    namespace {namespaceDefinition};
                    public partial class {className}
                    {{
                        public Func<{string.Join(",", signature)},{returnType}> GetDelegate()
                        {{
                            return ({string.Join(",", parameters)}) => {{ {initializationStatements} Execute(); return {propertyName}; }};
                        }}
                    }}");
                }
                else
                {
                    sourceCodeBuilder.Append(
                    @$"
                    namespace {namespaceDefinition};
                    public partial class {className}
                    {{
                        public Func<{returnType}> GetDelegate()
                        {{
                            return () => {{ Execute(); return {propertyName}; }};
                        }}
                    }}");
                }

                context.AddSource($"{className}.g.cs", sourceCodeBuilder.ToString());
            }
        }
    }
    
    private bool ImplementsInterface(ClassDeclarationSyntax classDeclaration, string interfaceName)
    {
        return classDeclaration.BaseList?.Types.Any(type => type.ToString() == interfaceName) ?? false;
    }
    
    /// <summary>
    /// Credit to: https://andrewlock.net/creating-a-source-generator-part-5-finding-a-type-declarations-namespace-and-type-hierarchy/
    /// </summary>
    static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        string nameSpace = string.Empty;

        SyntaxNode? potentialNamespaceParent = syntax.Parent;

        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }
        
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            nameSpace = namespaceParent.Name.ToString();

            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }
                
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }

        return nameSpace;
    }
}