Imports ObasAnalyzerVB.Utilerias.ObasAnalyzer

<DiagnosticAnalyzer(LanguageNames.VisualBasic)>
Public Class RepositorioGrandeAnalyzer
    Inherits DiagnosticAnalyzer

#Region "Reglas"
    Friend Const Category As String = "OBAS-SmellArquitectura"

    Friend Shared ReadOnly Regla001RepositorioGrande As DiagnosticDescriptor = New DiagnosticDescriptor(
            "RA11001",
            "RA11-001: Repositorio gestiona muchas entidades ",
            "El repositorio {0} gestiona {1} entidades y sobre pasa el límite definido para la cantidad de entidades que puede gestionar.",
            Category,
            DiagnosticSeverity.Error,
            True)


    Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
        Get
            Return ImmutableArray.Create(Regla001RepositorioGrande)
        End Get
    End Property

    Public Overrides Sub Initialize(ByVal context As AnalysisContext)
        ' Registra el tipo de sintaxis que se analiza para cada regla
        context.RegisterSyntaxNodeAction(AddressOf AnalyzeRegla001RepositorioGrande, SyntaxKind.ClassBlock)
    End Sub
#End Region

#Region "Analyzadores"
    ''' <summary>
    ''' Valida el cumplimiento de la regla
    ''' "RA11-001: Repositorio gestiona muchas entidades"
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub AnalyzeRegla001RepositorioGrande(ByVal context As SyntaxNodeAnalysisContext)
        ' Objeto para obtener entidades
        Dim entidades As New HashSet(Of TypeSyntax)()

        ' Obtiene la declaración de la clase
        Dim classDeclaration = DirectCast(context.Node, ClassBlockSyntax)

        ' Obtiene el símbolo de la clase
        Dim classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration)

        ' Obtiene nombre de la clase
        Dim nombreClase = classDeclaration.BlockStatement.Identifier.ValueText

        ' Revisa si el nombre de la clase contenedora inicia con la cadena "dal"
        If nombreClase.ToLower().Contains(Constantes.nomenclaturaAccesoBaseDatos) Then
            ' Obtiene la lista de métodos
            Dim metodos = classDeclaration.Members.OfType(Of MethodBlockSyntax)()

            ' Revisa cada método
            For Each metodo In metodos
                ' Obtiene el cuerpo del método
                Dim cuerpoMetodo = metodo.Statements

                If cuerpoMetodo.Count > 0 Then
                    AnalizaCuerpo(cuerpoMetodo, entidades, context)
                End If
            Next

            ' Si sobrepasa el límite, reporte el error
            If entidades.Count > Constantes.limiteEntidadesRepositorio Then
                Dim diag As Diagnostic = Diagnostic.Create(Regla001RepositorioGrande, classDeclaration.BlockStatement.Identifier.GetLocation(), classSymbol.Name, entidades.Count)
                context.ReportDiagnostic(diag)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Metodo que extrae los identificadores de los nodos del cuerpo de un metodo y luego
    ''' compara si el nombre inicia con Ent para determinar que se refiere a una entidad
    ''' </summary>
    ''' <param name="cuerpo"></param>
    '''  <param name="entidades"></param>
    ''' <param name="context"></param>
    Private Shared Sub AnalizaCuerpo(cuerpo As SyntaxList(Of StatementSyntax), entidades As HashSet(Of TypeSyntax), context As SyntaxNodeAnalysisContext)
        ' Revisa cada statement
        For Each statement As StatementSyntax In cuerpo
            ' Si es un nodo de expresión literal
            If TypeOf statement Is LocalDeclarationStatementSyntax Then
                Dim tipoVariable = DirectCast(statement, LocalDeclarationStatementSyntax).Declarators.FirstOrDefault().AsClause.Type

                If (tipoVariable IsNot Nothing) AndAlso (tipoVariable.ToString.ToLower().StartsWith(Constantes.nomenclaturaEntidad)) Then
                    If Not entidades.Contains(tipoVariable) Then
                        entidades.Add(tipoVariable)
                    End If
                End If
            End If
        Next
    End Sub



#End Region

End Class
