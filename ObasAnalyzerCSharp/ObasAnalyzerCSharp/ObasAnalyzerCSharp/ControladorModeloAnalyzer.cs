using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System;
using Utilerias.ObasAnalyzerCSharp;


namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ControladorModeloAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001ControladorModelo = new DiagnosticDescriptor(
            "RA06001",
            "RA06-001: No se permite el uso de comandos SQL en el Controlador ",
            "El Controlador no puede contener el comando SQL '{0}'.",
            Category,
            DiagnosticSeverity.Error,
            true);

        internal static readonly DiagnosticDescriptor Regla002ControladorModelo = new DiagnosticDescriptor(
            "RA06002",
            "RA06-002: No se permite el uso de referencias a librerías de acceso a datos ",
            "El Controlador no puede contener referencias a librerías de acceso a datos'.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001ControladorModelo, Regla002ControladorModelo); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001ControladorModelo, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeRegla002ControladorModelo, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA06-001: No se permite el uso de comandos SQL en el Controlador "
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001ControladorModelo(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene nombre de la clase
            var nombreClaseContenedora = classDeclaration.SyntaxTree.FilePath;

            // Revisa si el nombre de la clase contenedora finaliza con la cadena "controller"
            if (nombreClaseContenedora.ToLower().Contains(Constantes.nomenclaturaControlador))
            {
                var textoClase = context.Node.SyntaxTree.GetText().ToString().ToLower();

                foreach (var comando in Constantes.comandosSQL)
                {
                    if (textoClase.Contains(comando))
                    {
                        var diag = Diagnostic.Create(Regla001ControladorModelo, classDeclaration.Identifier.GetLocation(), comando);
                        context.ReportDiagnostic(diag);

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA06-002: No se permite el uso de referencias a librerías de acceso a datos "
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzeRegla002ControladorModelo(SyntaxNodeAnalysisContext context)
        {
            var incumpleRegla = false;

            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene nombre de la clase
            var nombreClaseContenedora = classDeclaration.SyntaxTree.FilePath;

            // Revisa si el nombre de la clase contenedora finaliza con la cadena "controller"
            if (nombreClaseContenedora.ToLower().Contains(Constantes.nomenclaturaControlador))
            {
                // Revisa cada uno de los miembros

                foreach (var member in classDeclaration.Members)
                {
                    // Revisa el tipo del miembro
                    var memberType = context.SemanticModel.GetTypeInfo(member).Type;
                    if (memberType != null && UsaNamespaceModelo(memberType.ContainingNamespace.ToString().ToLower()))
                    {
                        incumpleRegla = true;
                        break;
                    }

                    // Revisa los atritubos de cada miembro
                    var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(member);

                    if (declaredSymbol != null)
                    {
                        var memberAttributes = declaredSymbol.GetAttributes();

                        foreach (var attribute in memberAttributes)
                        {
                            var attributeType = attribute.GetType();
                            if (attributeType != null && UsaNamespaceModelo(attributeType.Namespace.ToLower()))
                            {
                                incumpleRegla = true;
                                break;
                            }
                        }
                    }
                    if (incumpleRegla) break;
                }

                // Reporta el diagnóstico si incumple la regla
                if (incumpleRegla)
                {
                    var diagnostic = Diagnostic.Create(Regla002ControladorModelo, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.ValueText);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }


        /// <summary>
        /// Función para comprobar si la referencia existe en la lista de referencias de librerías de acceso a Datos
        /// </summary>
        /// <param name="namespaceSymbol"></param>
        /// <returns></returns>
        private static bool UsaNamespaceModelo(string nombreNamespace)
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
