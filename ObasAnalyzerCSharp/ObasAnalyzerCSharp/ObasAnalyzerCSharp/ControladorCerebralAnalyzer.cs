using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using Utilerias.ObasAnalyzerCSharp;

namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ControladorCerebralAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001ControladorCerebral = new DiagnosticDescriptor(
            "RA07001",
            "RA07-001: Control de flujo muy grande",
            "El Controlador no puede sobrepasar el límite definido para el control de flujo.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001ControladorCerebral); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001ControladorCerebral, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA07-001: Control de flujo muy grande"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001ControladorCerebral(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene nombre de la clase
            var nombreClase = classDeclaration.Identifier.ValueText;

            // Revisa si el nombre de la clase contenedora finaliza con la cadena "controller"
            if (nombreClase.ToLower().Contains(Constantes.nomenclaturaControlador))
            {
                // Revisa la complejidad de cada método
                int complejidad = 0;

                foreach (var member in classDeclaration.Members)
                {
                    MethodDeclarationSyntax metodo;

                    // Revisa métodos públicos que no son constructores o sobrecargas
                    if (member is MethodDeclarationSyntax)
                    {
                        metodo = (MethodDeclarationSyntax)member;

                        if (metodo.Modifiers.Any(SyntaxKind.PublicKeyword) &&
                            !metodo.Modifiers.Any(SyntaxKind.OverrideKeyword) &&
                            metodo.Identifier.ValueText != classDeclaration.Identifier.ValueText)
                        {
                            // Se cuentan los controladores de flujo
                            complejidad += metodo.DescendantNodes().Count(n => n is IfStatementSyntax || n is SwitchStatementSyntax || n is ForStatementSyntax || n is ForEachStatementSyntax || n is WhileStatementSyntax || n is DoStatementSyntax || n is CatchClauseSyntax || n is ConditionalExpressionSyntax);
                        }
                    }
                }

                // Genera reporte si la complejidad calculada es mayor a lo definido en las contantes
                if (complejidad > Constantes.limiteControlFlujo)
                {
                    var diagnostic = Diagnostic.Create(Regla001ControladorCerebral, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.ValueText);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        #endregion
    }
}