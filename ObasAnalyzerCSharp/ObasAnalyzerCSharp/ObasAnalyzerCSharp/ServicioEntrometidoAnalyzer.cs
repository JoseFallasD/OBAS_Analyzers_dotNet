using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;
using Utilerias.ObasAnalyzerCSharp;


namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ServicioEntrometidoAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001ServicioEntrometido = new DiagnosticDescriptor(
            "RA09001",
            "RA09-001: No se permite el uso de comandos SQL en la capa de Servicio ",
            "El Servicio no puede contener el comando SQL '{0}'.",
            Category,
            DiagnosticSeverity.Error,
            true);

        internal static readonly DiagnosticDescriptor Regla002ServicioEntrometido = new DiagnosticDescriptor(
            "RA09002",
            "RA09-002: No se permite el uso de referencias a librerías de acceso a datos ",
            "El Servicio no puede contener referencias a la librería de acceso a datos {0}'.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001ServicioEntrometido, Regla002ServicioEntrometido); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001ServicioEntrometido, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeRegla002ServicioEntrometido, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA09-001: No se permite el uso de comandos SQL en la capa de Servicio "
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001ServicioEntrometido(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene nombre de la clase
            var nombreClaseContenedora = classDeclaration.SyntaxTree.FilePath;

            // Revisa si el nombre de la clase contenedora finaliza con la cadena "wsr"
            if (nombreClaseContenedora.ToLower().Contains(Constantes.nomenclaturaServicio))
            {
                var textoClase = context.Node.SyntaxTree.GetText().ToString().ToLower();

                foreach (var comando in Constantes.comandosSQL)
                {
                    if (textoClase.Contains(comando))
                    {
                        var diag = Diagnostic.Create(Regla001ServicioEntrometido, classDeclaration.Identifier.GetLocation(), comando);
                        context.ReportDiagnostic(diag);

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA09-002: No se permite el uso de referencias a librerías de acceso a datos "
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzeRegla002ServicioEntrometido(SyntaxNodeAnalysisContext context)
        {
            var incumpleRegla = false;

            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene nombre de la clase
            var nombreClaseContenedora = classDeclaration.SyntaxTree.FilePath;

            // Revisa si el nombre de la clase contenedora finaliza con la cadena "wsr"
            if (nombreClaseContenedora.ToLower().Contains(Constantes.nomenclaturaServicio))
            {
                // Revisa la referencia de librerías
                foreach (var referencia in classDeclaration.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>())
                {
                    if (UsaNamespaceAccesoDatos(referencia.Name.ToString().ToLower()))
                    {
                        var diagnostic = Diagnostic.Create(Regla002ServicioEntrometido, classDeclaration.Identifier.GetLocation(), referencia.Name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }


        /// <summary>
        /// Función para comprobar si la referencia existe en la lista de referencias de librerías de acceso a Datos
        /// </summary>
        /// <param name="namespaceSymbol"></param>
        /// <returns></returns>
        private static bool UsaNamespaceAccesoDatos(string nombreNamespace)
        {
            if (Constantes.libreriasAccesoDatos.Contains(nombreNamespace))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}