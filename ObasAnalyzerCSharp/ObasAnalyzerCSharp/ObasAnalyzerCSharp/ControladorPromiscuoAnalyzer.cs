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
    public class ControladorPromiscuoAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001ControladorPromiscuo = new DiagnosticDescriptor(
            "RA08001",
            "RA08-001: Cantidad de acciones y rutas muy grande",
            "El Controlador no puede sobrepasar el límite definido para la cantidad de acciones y rutas que ofrece.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001ControladorPromiscuo); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001ControladorPromiscuo, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA08-001: Cantidad de acciones y rutas muy grande"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001ControladorPromiscuo(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene nombre de la clase
            var nombreClase = classDeclaration.Identifier.ValueText;

            // Revisa si el nombre de la clase contenedora finaliza con la cadena "controller"
            if (nombreClase.ToLower().Contains(Constantes.nomenclaturaControlador))
            {
                // Revisa la cantidad de acciones y rutas
                int cantidadMetodos = 0;
                int cantidadParametros = 0;
                int cantidadDependencias = 0;

                // Revisa la cantidad de dependencias en el constructor
                var constructor = classDeclaration.Members.OfType<ConstructorDeclarationSyntax>().FirstOrDefault();
                if (constructor != null)
                {
                    cantidadDependencias = constructor.ParameterList.Parameters.Count;
                }

                // Revisa la cantidad de métodos
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
                            cantidadMetodos++;
                            // Se cuentan los parámetros de los métodos
                            cantidadParametros += metodo.ParameterList.Parameters.Count;
                        }
                    }
                }

                // Genera reporte si la cantidad de acciones y rutas es mayor a lo definido en las contantes
                if (cantidadMetodos > Constantes.limiteMetodosClase || cantidadParametros > Constantes.limiteParametrosMetodo || cantidadDependencias > Constantes.limiteDependenciasClase)
                {
                    var diagnostic = Diagnostic.Create(Regla001ControladorPromiscuo, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.ValueText);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        #endregion
    }
}