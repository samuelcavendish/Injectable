using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Injectable;

internal class InjectableSyntaxReceiver : ISyntaxContextReceiver
{
    public List<ClassDeclarationSyntax> Candidates { get; } = [];
    public List<InterfaceDeclarationSyntax> InterfaceCandidates { get; } = [];

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        // Collect all class and interface declarations
        if (context.Node is ClassDeclarationSyntax classDecl)
        {
            Candidates.Add(classDecl);
        }
        else if (context.Node is InterfaceDeclarationSyntax interfaceDecl)
        {
            InterfaceCandidates.Add(interfaceDecl);
        }
    }
}
