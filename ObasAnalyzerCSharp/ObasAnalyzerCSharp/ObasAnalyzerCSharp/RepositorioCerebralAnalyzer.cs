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
    public class RepositorioCerebralAnalyzer : DiagnosticAnalyzer
    {
        #region "Reglas"
        internal const string Category = "OBAS-SmellArquitectura-MVC";

        internal static readonly DiagnosticDescriptor Regla001RepositorioCerebral = new DiagnosticDescriptor(
            "RA10001",
            "RA10-001: Lógica compleja en el repositorio",
            "La consulta {0} sobre pasa el límite definido para el control de lógica en consultas.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Regla001RepositorioCerebral); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            // Registra el tipo de sintaxis que se analiza para cada regla
            context.RegisterSyntaxNodeAction(AnalyzeRegla001RepositorioCerebral, SyntaxKind.ClassDeclaration);
        }
        #endregion

        #region "Analizadores"

        /// <summary>
        /// Valida el cumplimiento de la regla
        /// "RA10-001: Lógica compleja en el repositorio"
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeRegla001RepositorioCerebral(SyntaxNodeAnalysisContext context)
        {
            // Obtiene la declaración de la clase
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

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
                        AnalizaCuerpo(cuerpoMetodo, context);
                    }
                }
            }
        }

        /// <summary>
        /// Metodo que extrae las expresiones literales del cuerpo de un metodo y luego
        /// separa la cadena de texto para contar la cantidad de veces que aparece un comando de lógica SQL
        /// </summary>
        /// <param name="cuerpo"></param>
        /// <param name="classSymbol"></param>
        /// <param name="context"></param>
        private static void AnalizaCuerpo(CSharpSyntaxNode cuerpo, SyntaxNodeAnalysisContext context)
        {
            // Extrae las expresiones literales
            var literales = cuerpo.DescendantNodes().OfType<LiteralExpressionSyntax>();

            // Revisa cada literal
            foreach (var literal in literales)
            {
                var literalValue = context.SemanticModel.GetConstantValue(literal).Value.ToString().ToLower();

                if (!String.IsNullOrEmpty(literalValue))
                {
                    foreach (var comando in Constantes.commandosLogicaSQL)
                    {
                        if (literalValue.Contains(comando))
                        {
                            // Separa la cadena de texto para contar la cantidad de veces que aparece un comando de lógica SQL
                            var palabras = literalValue.Split(' ', ',', '(', ')');

                            int conteoComandos = palabras.Count(p => Constantes.commandosLogicaSQL.Contains(p));

                            if (conteoComandos > Constantes.limiteLogicaComandos)
                            {
                                var diagnostic = Diagnostic.Create(Regla001RepositorioCerebral, literal.GetLocation(), literalValue);
                                context.ReportDiagnostic(diagnostic);
                                break;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}