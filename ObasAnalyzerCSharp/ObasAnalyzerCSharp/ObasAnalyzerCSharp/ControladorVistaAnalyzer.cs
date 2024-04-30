using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Utilerias.ObasAnalyzerCSharp;

namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ControladorVistaAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001ControladorVista = new DiagnosticDescriptor(
            "RA05001",
            "RA05-001: No se permite el uso de etiquetas HTML en el Controlador",
            "El Controlador no puede contener la etiqueta HTML '{0}'.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001ControladorVista); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001ControladorVista, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA01-001: No se permite el uso de etiquetas HTML en archivos",
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001ControladorVista(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene nombre de la clase
            var nombreClase = classDeclaration.Identifier.ValueText;

            // Revisa si el nombre de la clase contenedora finaliza con la cadena "controlador"
            if (nombreClase.ToLower().Contains(Constantes.nomenclaturaControlador))
            {
                var textoClase = classDeclaration.SyntaxTree.GetRoot().GetText().ToString().ToLower();

                foreach (var etiqueta in Constantes.etiquetasHtml)
                {
                    if (textoClase.Contains(etiqueta))
                    {
                        var diag = Diagnostic.Create(Regla001ControladorVista, classDeclaration.Identifier.GetLocation(), etiqueta);
                        context.ReportDiagnostic(diag);

                        return;
                    }
                }
            }
        }

        #endregion
    }
}