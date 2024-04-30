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
    public class RepositorioMetodosLaboriosos : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001RepositorioMetodosLaborioso = new DiagnosticDescriptor(
            "RA12001",
            "RA12-001: Repositorio con método que ejecuta varias acciones",
            "El método {0} del repositorio {1} ejecuta muchos llamados a acciones sobre la base de datos.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001RepositorioMetodosLaborioso); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001RepositorioGordo, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA12-001: Repositorio con método que ejecuta varias acciones"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001RepositorioGordo(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Obtiene el simbolo de la clase
            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

            // Obtiene nombre de la clase
            var nombreClase = classDeclaration.Identifier.ValueText;

            // Revisa si el nombre de la clase contenedora inicia con la cadena "dal"
            if (nombreClase.ToLower().Contains(Constantes.nomenclaturaRepositorio))
            {
                // Obtiene la lista de metodos
                var metodos = classDeclaration.Members.OfType<BaseMethodDeclarationSyntax>();

                // Revisa cada metodo
                foreach (var metodo in metodos)
                {
                    // Obtiene el cuerpo del método
                    var cuerpoMetodo = metodo.Body;

                    // Obtiene los llamados en el método

                    var invocaciones = cuerpoMetodo.DescendantNodes().OfType<InvocationExpressionSyntax>();

                    int conteoAcciones = 0;

                    foreach (var invocacion in invocaciones)
                    {
                        if (invocacion.ToFullString().ToLower().Contains("vgo_conexion"))
                        {
                            conteoAcciones++;
                        }
                    }
                    if (conteoAcciones > Constantes.limiteAccionesBaseDatos)
                    {
                        // Intenta convertir el método a MethodDeclarationSyntax
                        var methodDeclaration = metodo as MethodDeclarationSyntax;
                        // Obtiene el nombre del método
                        string nombreMetodo;

                        // Si la conversión es exitosa obtiene el nombre
                        if (methodDeclaration != null)
                        {
                            nombreMetodo = methodDeclaration.Identifier.ValueText;

                            var diagnostic = Diagnostic.Create(Regla001RepositorioMetodosLaborioso, classDeclaration.Identifier.GetLocation(), nombreMetodo, classSymbol.Name);
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }
    }

    #endregion
}