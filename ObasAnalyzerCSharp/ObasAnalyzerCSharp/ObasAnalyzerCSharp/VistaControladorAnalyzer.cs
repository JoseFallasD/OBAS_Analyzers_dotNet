using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Utilerias.ObasAnalyzerCSharp;

namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VistaControladorAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001VistaControlador = new DiagnosticDescriptor(
            "RA04001",
            "RA04-001: No se permite el acceso variables Post-Get en la Vista",
            "La Vista no puede acceder a variables Post-Get.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001VistaControlador); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001VistaControlador, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA04-001: No se permite el acceso variables Post-Get en la Vista"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001VistaControlador(SyntaxNodeAnalysisContext context)
        {

            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene nombre de la clase
            var nombreClaseContenedora = classDeclaration.SyntaxTree.FilePath;

            // Revisa si el nombre de la clase contenedora finaliza con la cadena "cshtml"
            if (nombreClaseContenedora.ToLower().Contains(Constantes.nomenclaturaVista))
            {
                // Revisa cada elemento mientro de la clase
                foreach (var member in classDeclaration.Members)
                {
                    // Revisa los atritubos de cada miembro
                    var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(member);

                    if (declaredSymbol != null)
                    {
                        var memberAttributes = declaredSymbol.GetAttributes();

                        foreach (var attribute in memberAttributes)
                        {
                            if (attribute.AttributeClass != null && (attribute.AttributeClass.Name == "HttpPost" || attribute.AttributeClass.Name == "HttpGet"))
                            {
                                var diagnostic = Diagnostic.Create(Regla001VistaControlador, member.GetLocation(), classDeclaration.Identifier.ValueText);
                                context.ReportDiagnostic(diagnostic);
                                return;
                            }
                        }
                    }
                }

            }
        }

        #endregion
    }
}