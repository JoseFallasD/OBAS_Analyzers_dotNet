using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Utilerias.ObasAnalyzerCSharp;

namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModeloVistaAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001ModeloVista = new DiagnosticDescriptor(
            "RA01001",
            "RA01-001: No se permite el uso de etiquetas HTML en el Modelo",
            "El Modelo no puede contener la etiqueta HTML '{0}'.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001ModeloVista); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001ModeloVista, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA01-001: No se permite el uso de etiquetas HTML en el Modelo"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001ModeloVista(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene nombre de la clase
            var nombreClase = classDeclaration.Identifier.ValueText;

            // Revisa si el nombre de la clase contenedora finaliza con la cadena "vm"
            if (nombreClase.ToLower().EndsWith("vm"))
            {

                // Obtiene el código fuente de la clase
                var textoClase = classDeclaration.SyntaxTree.GetRoot().GetText().ToString().ToLower();

                // Revisa si alguna etiqueta HTML está presente en el código
                foreach (var etiqueta in Constantes.etiquetasHtml)
                {
                    if (textoClase.Contains(etiqueta))
                    {
                        var diag = Diagnostic.Create(Regla001ModeloVista, classDeclaration.Identifier.GetLocation(), etiqueta);
                        context.ReportDiagnostic(diag);
                    }
                }
            }
        }

        #endregion
    }
}