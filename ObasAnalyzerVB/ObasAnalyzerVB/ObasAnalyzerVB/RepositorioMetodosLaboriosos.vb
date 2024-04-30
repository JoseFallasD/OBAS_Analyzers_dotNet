Imports ObasAnalyzerVB.Utilerias.ObasAnalyzer

<DiagnosticAnalyzer(LanguageNames.VisualBasic)>
Public Class RepositorioMetodosLaboriosos
    Inherits DiagnosticAnalyzer

#Region "Reglas"
    Friend Const Category As String = "OBAS-SmellArquitectura"

    Friend Shared ReadOnly Regla001RepositorioMetodosLaborioso As DiagnosticDescriptor = New DiagnosticDescriptor(
            "RA12001",
            "RA12-001: Repositorio con método que ejecuta varias acciones ",
            "El método {0} del repositorio {1} ejecuta muchos llamados a acciones sobre la base de datos.",
            Category,
            DiagnosticSeverity.Error,
            True)


    Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
        Get
            Return ImmutableArray.Create(Regla001RepositorioMetodosLaborioso)
        End Get
    End Property

    Public Overrides Sub Initialize(ByVal context As AnalysisContext)
        ' Registra el tipo de sintaxis que se analiza para cada regla
        context.RegisterSyntaxNodeAction(AddressOf AnalyzeRegla001RepositorioMetodosLaborioso, SyntaxKind.ClassBlock)
    End Sub

#End Region

#Region "Analizadores"

    ''' <summary>
    ''' Valida el cumplimiento de la regla
    ''' "RA12-001: Repositorio con método que ejecuta varias acciones"
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub AnalyzeRegla001RepositorioMetodosLaborioso(ByVal context As SyntaxNodeAnalysisContext)
        ' Obtiene la declaración de la clase
        Dim classDeclaration = DirectCast(context.Node, ClassBlockSyntax)

        ' Obtiene el símbolo de la clase
        Dim classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration)

        ' Obtiene nombre de la clase
        Dim nombreClase = classDeclaration.BlockStatement.Identifier.ValueText

        ' Revisa si el nombre de la clase contenedora inicia con la cadena "dal"
        If nombreClase.ToLower().Contains(Constantes.nomenclaturaRepositorio) Then
            ' Obtiene la lista de métodos
            Dim metodos = classDeclaration.Members.OfType(Of MethodBlockSyntax)()

            ' Revisa cada método
            For Each metodo In metodos
                ' Obtiene el cuerpo del método
                Dim cuerpoMetodo = metodo.Statements

                ' Obtiene los llamados en el método
                Dim invocaciones = cuerpoMetodo.SelectMany(Function(statement) statement.DescendantNodesAndSelf().OfType(Of InvocationExpressionSyntax)())

                Dim conteoAcciones As Integer = 0


                For Each invocacion In invocaciones
                    For Each comandoEjecucion In Constantes.commandosEjecucionSQL
                        If invocacion.ToFullString().ToLower().Contains(comandoEjecucion) Then
                            conteoAcciones += 1
                        End If
                    Next
                Next

                If conteoAcciones > Constantes.limiteAccionesBaseDatos Then
                    ' Obtiene el nombre del método
                    Dim nombreMetodo = metodo.SubOrFunctionStatement.Identifier.ValueText

                    Dim diag = Diagnostic.Create(Regla001RepositorioMetodosLaborioso, classDeclaration.BlockStatement.Identifier.GetLocation(), nombreMetodo, classSymbol.Name)
                    context.ReportDiagnostic(diag)
                End If
            Next
        End If
    End Sub

#End Region


End Class
