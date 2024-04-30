using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Utilerias.ObasAnalyzerCSharp;

namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClaseDiosAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001ClaseDios = new DiagnosticDescriptor(
            "RA16001",
            "RA16-001: La clase es muy grande",
            "La clase '{0}' tiene demasiadas líneas de código.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001ClaseDios); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001ClaseDios, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA16-001: La clase es muy grande"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001ClaseDios(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene la cantidad de líneas de código
            int conteoLineas = classDeclaration.GetLocation().GetLineSpan().EndLinePosition.Line - classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

            // Si sobrepasa el limite reporte el error
            if (conteoLineas > Constantes.limiteLineasCodigoClase)
            {
                var diagnostic = Diagnostic.Create(Regla001ClaseDios, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.ValueText);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    #endregion
}
