using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Utilerias.ObasAnalyzerCSharp;

namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ViolacionArquitecturaAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001ViolacionArquitectura = new DiagnosticDescriptor(
            "RA15001",
            "RA15-001: La capa de servicios referencia acceso de datos",
            "La clase de servicio no puede referenciar directamente al objeto '{0}' de la capa de acceso a datos",
            Category,
            DiagnosticSeverity.Error,
            true);

        internal static readonly DiagnosticDescriptor Regla002ViolacionArquitectura = new DiagnosticDescriptor(
            "RA15002",
            "RA15-002: La capa de lógica referencia servicios",
            "La clase de lógica de negocios no puede referenciar directamente al objeto '{0}' de la capa de servicios",
            Category,
            DiagnosticSeverity.Error,
            true);

        internal static readonly DiagnosticDescriptor Regla003ViolacionArquitectura = new DiagnosticDescriptor(
            "RA15003",
            "RA15-003: La capa de acceso a datos referencia lógica o servicios",
            "La clase de acceso a datos no puede referenciar directamente al objeto '{0}' de la capa de servicios o lógica de negocios",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001ViolacionArquitectura, Regla002ViolacionArquitectura, Regla003ViolacionArquitectura); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001ViolacionArquitectura, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeRegla002ViolacionArquitectura, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeRegla003ViolacionArquitectura, SyntaxKind.ObjectCreationExpression);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA15-001: La capa de servicios referencia acceso de datos"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001ViolacionArquitectura(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la sintaxis de creación de objetos nuevos
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreationExpression != null)
            {
                // Obtiene el nombre de la clase
                var claseContenedora = (objectCreationExpression.FirstAncestorOrSelf<ClassDeclarationSyntax>()?.SyntaxTree?.FilePath).ToLower();

                // Comprueba si es una clase de la capa de servicios
                if (claseContenedora != null && claseContenedora.Contains(Constantes.nomenclaturaServicio))
                {
                    // Comprueba si el tipo de objeto es de la capa de acceso a datos
                    if (objectCreationExpression.Type.ToString().ToLower().Contains(Constantes.nomenclaturaAccesoBaseDatos))
                    {
                        var diagnostic = Diagnostic.Create(Regla001ViolacionArquitectura, context.Node.GetLocation(), objectCreationExpression.Type.ToString());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA15-002: La capa de lógica referencia servicios"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla002ViolacionArquitectura(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la sintaxis de creación de objetos nuevos
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreationExpression != null)
            {
                // Obtiene el nombre de la clase
                var claseContenedora = (objectCreationExpression.FirstAncestorOrSelf<ClassDeclarationSyntax>()?.SyntaxTree?.FilePath).ToLower();

                // Comprueba si es una clase de la capa de logica
                if (claseContenedora != null && claseContenedora.Contains(Constantes.nomenclaturaLogicaNegocio))
                {
                    // Comprueba si el tipo de objeto es de la capa de servicios
                    if (objectCreationExpression.Type.ToString().ToLower().Contains(Constantes.nomenclaturaServicio))
                    {
                        var diagnostic = Diagnostic.Create(Regla002ViolacionArquitectura, context.Node.GetLocation(), objectCreationExpression.Type.ToString());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA15-003: La capa de acceso a datos referencia lógica o servicios"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla003ViolacionArquitectura(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la sintaxis de creación de objetos nuevos
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreationExpression != null)
            {
                // Obtiene el nombre de la clase
                var claseContenedora = (objectCreationExpression.FirstAncestorOrSelf<ClassDeclarationSyntax>()?.SyntaxTree?.FilePath).ToLower();

                // Comprueba si es una clase de la capa de acceso a datos
                if (claseContenedora != null && claseContenedora.Contains(Constantes.nomenclaturaAccesoBaseDatos))
                {
                    // Comprueba si el tipo de objeto es de la capa de servicios o logica de negocio
                    if (objectCreationExpression.Type.ToString().ToLower().Contains(Constantes.nomenclaturaServicio) || objectCreationExpression.Type.ToString().ToLower().Contains(Constantes.nomenclaturaLogicaNegocio))
                    {
                        var diagnostic = Diagnostic.Create(Regla003ViolacionArquitectura, context.Node.GetLocation(), objectCreationExpression.Type.ToString());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }

    #endregion
}