using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AbstraccionSinDesacopleAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001AbstraccionSinDesacople = new DiagnosticDescriptor(
            "RA13001",
            "RA13-001: Clase con abstracción sin desacople",
            "La clase {0} utiliza la interface {1} y accede al miembro {2} de la clase {3}.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001AbstraccionSinDesacople); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001AbstraccionSinDesacople, SyntaxKind.SimpleMemberAccessExpression);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA13-001: Clase con abstracción sin desacople"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001AbstraccionSinDesacople(SyntaxNodeAnalysisContext context)
        {
            // Obtiene el miembro
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            // Obtiene la información del miembro
            var infoSimbolo = context.SemanticModel.GetSymbolInfo(memberAccess);

            // Si el miembro es una propiedad obtiene el tipo
            if (infoSimbolo.Symbol != null && infoSimbolo.Symbol.Kind == SymbolKind.Property)
            {
                var propertySymbol = (IPropertySymbol)infoSimbolo.Symbol;

                var containingType = propertySymbol.ContainingType;

                if (containingType.TypeKind == TypeKind.Class)
                {
                    var classSymbol = (INamedTypeSymbol)containingType;

                    // Obtiene todas las interfaces utilizadas
                    var implementedInterfaces = classSymbol.AllInterfaces;

                    var expression = memberAccess.Expression;

                    var typeInfo = context.SemanticModel.GetTypeInfo(expression);

                    var typeKind = typeInfo.Type.TypeKind;

                    if (!implementedInterfaces.IsEmpty)
                    {



                        if (typeInfo.Type != null && typeInfo.Type.TypeKind == TypeKind.Interface)
                        {
                            var interfaceSymbol = (INamedTypeSymbol)typeInfo.Type;

                            if (implementedInterfaces.Contains(interfaceSymbol))
                            {
                                var diagnostic = Diagnostic.Create(Regla001AbstraccionSinDesacople, memberAccess.GetLocation(), classSymbol.Name, interfaceSymbol.Name, propertySymbol.Name, containingType.Name);

                                context.ReportDiagnostic(diagnostic);
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion
}