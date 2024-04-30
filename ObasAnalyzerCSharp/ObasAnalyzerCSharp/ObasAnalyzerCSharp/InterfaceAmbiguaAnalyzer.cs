using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Utilerias.ObasAnalyzerCSharp;

namespace Analyzer1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterfaceAmbiguaAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001InterfaceAmbigua = new DiagnosticDescriptor(
            "RA14001",
            "RA14-001: Interface Ambigua",
            "La interface {0} tiene demasiados métodos",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001InterfaceAmbigua); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001InterfaceAmbigua, SyntaxKind.InterfaceDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA14-001: Interface Ambigua"
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzeRegla001InterfaceAmbigua(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            // Cuenta la cantidad de métodos presentes
            int cantidadMetodos = 0;
            foreach (var miembro in interfaceDeclaration.Members)
            {
                if (miembro is MethodDeclarationSyntax)
                {
                    cantidadMetodos++;
                }
            }

            // Si sobrepasa el limite reporte el error
            if (cantidadMetodos > Constantes.limiteMetodosInterface)
            {
                var diagnostic = Diagnostic.Create(Regla001InterfaceAmbigua, interfaceDeclaration.Identifier.GetLocation(), interfaceDeclaration.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }
        #endregion
    }
}