using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Utilerias.ObasAnalyzerCSharp;

namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModeloControladorAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001ModeloControlador = new DiagnosticDescriptor(
            "RA02001",
            "RA02-001: No se permite el acceso variables Post-Get en el Modelo",
            "El Modelo no puede acceder a variables Post-Get.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001ModeloControlador); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001ModeloControlador, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA02-001: No se permite el acceso variables Post-Get en el Modelo"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001ModeloControlador(SyntaxNodeAnalysisContext context)
        {

            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene nombre de la clase
            var nombreClase = classDeclaration.Identifier.ValueText;

            // Revisa si el nombre de la clase contenedora finaliza con la cadena "vm"
            if (nombreClase.ToLower().Contains(Constantes.nomenclaturaVista))
            {

                // Revisa cada elemento mientro de la clase
                foreach (var member in classDeclaration.Members)
                {
                    // Revisa los atritubos de cada miembro
                    var memberAttributes = context.SemanticModel.GetDeclaredSymbol(member).GetAttributes();

                    foreach (var attribute in memberAttributes)
                    {
                        if (attribute.AttributeClass != null && (attribute.AttributeClass.Name == "HttpPost" || attribute.AttributeClass.Name == "HttpGet"))
                        {
                            var diagnostic = Diagnostic.Create(Regla001ModeloControlador, member.GetLocation(), classDeclaration.Identifier.ValueText);
                            context.ReportDiagnostic(diagnostic);
                            return;
                        }
                    }
                }

            }
        }

        #endregion
    }
}