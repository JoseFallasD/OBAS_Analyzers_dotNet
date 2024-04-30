using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Utilerias.ObasAnalyzerCSharp;

namespace ObasAnalyzerCSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RepositorioGrandeAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001RepositorioGrande = new DiagnosticDescriptor(
            "RA11001",
            "RA11-001: Repositorio gestiona muchas entidades ",
            "El repositorio {0} gestiona {1} entidades y sobre pasa el límite definido para la cantidad de entidades que puede gestionar.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001RepositorioGrande); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001RepositorioGrande, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA11-001: Repositorio gestiona muchas entidades"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001RepositorioGrande(SyntaxNodeAnalysisContext context)
        {
            // Objeto para obtener entidades
            var entidades = new HashSet<INamedTypeSymbol>();

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

                    if (cuerpoMetodo != null)
                    {
                        AnalizaCuerpo(cuerpoMetodo, entidades, context);
                    }
                }

                // Si sobrepasa el limite reporte el error
                if (entidades.Count > Constantes.limiteEntidadesRepositorio)
                {
                    var diagnostic = Diagnostic.Create(Regla001RepositorioGrande, classDeclaration.Identifier.GetLocation(), classSymbol.Name, entidades.Count);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        /// <summary>
        /// Metodo que extrae los identificadores de los nodos del cuerpo de un metodo y luego
        /// compara si el nombre inicia con Ent para determinar que se refiere a una entidad
        /// </summary>
        /// <param name="cuerpo"></param>
        /// <param name="entidades"></param>
        /// <param name="context"></param>
        private static void AnalizaCuerpo(CSharpSyntaxNode cuerpo, HashSet<INamedTypeSymbol> entidades, SyntaxNodeAnalysisContext context)
        {
            // Obtiene identificadores de los nodos
            var identificadores = cuerpo.DescendantNodes().OfType<IdentifierNameSyntax>();

            // Revisa los identificadores
            foreach (var identificador in identificadores)
            {
                var simboloTipo = context.SemanticModel.GetSymbolInfo(identificador).Symbol as INamedTypeSymbol;

                if (simboloTipo != null)
                {
                    if (simboloTipo != null && simboloTipo.Name.ToLower().StartsWith(Constantes.nomenclaturaEntidad))
                    {
                        // Añade la entidad al conteo
                        entidades.Add(simboloTipo);
                    }
                }
            }
        }
    }

    #endregion
}