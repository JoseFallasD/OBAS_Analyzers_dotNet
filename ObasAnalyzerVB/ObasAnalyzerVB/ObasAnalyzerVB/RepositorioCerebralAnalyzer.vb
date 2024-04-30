Imports ObasAnalyzerVB.Utilerias.ObasAnalyzer

<DiagnosticAnalyzer(LanguageNames.VisualBasic)>
Public Class RepositorioCerebralAnalyzer
    Inherits DiagnosticAnalyzer

#Region "Reglas"
    Friend Const Category As String = "OBAS-SmellArquitectura"

    Friend Shared ReadOnly Regla001RepositorioCerebral As DiagnosticDescriptor = New DiagnosticDescriptor(
            "RA10001",
            "RA10-001: Lógica compleja en el repositorio",
            "La consulta {0} sobre pasa el límite definido para el control de lógica en consultas.",
            Category,
            DiagnosticSeverity.Error,
            True)


    Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
        Get
            Return ImmutableArray.Create(Regla001RepositorioCerebral)
        End Get
    End Property

    Public Overrides Sub Initialize(ByVal context As AnalysisContext)
        ' Registra el tipo de sintaxis que se analiza para cada regla
        context.RegisterSyntaxNodeAction(AddressOf AnalyzeRegla001RepositorioCerebral, SyntaxKind.ClassBlock)
    End Sub
#End Region

#Region "Analyzadores"
    ''' <summary>
    ''' Valida el cumplimiento de la regla
    ''' "RA10-001: Lógica compleja en el repositorio"
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub AnalyzeRegla001RepositorioCerebral(ByVal context As SyntaxNodeAnalysisContext)
        ' Obtiene la declaración de la clase
        Dim classDeclaration As ClassBlockSyntax = TryCast(context.Node, ClassBlockSyntax)

        ' Obtiene nombre de la clase
        Dim nombreClaseContenedora As String = classDeclaration.SyntaxTree.FilePath

        ' Revisa si el nombre de la clase contenedora contiene la cadena "dal"
        If nombreClaseContenedora.ToLower().Contains(Constantes.nomenclaturaAccesoBaseDatos) Then
            ' Obtiene la lista de metodos
            Dim metodos = classDeclaration.Members.OfType(Of MethodBlockSyntax)()

            ' Revisa cada metodo
            For Each metodo In metodos
                ' Obtiene el cuerpo del método
                Dim cuerpoMetodo = metodo.Statements

                If cuerpoMetodo.Count > 0 Then
                    AnalizaCuerpo(cuerpoMetodo, context)
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' Método que extrae las expresiones literales del cuerpo de un método y luego
    ''' separa la cadena de texto para contar la cantidad de veces que aparece un comando de lógica SQL
    ''' </summary>
    ''' <param name="cuerpo"></param>
    ''' <param name="context"></param>
    Private Shared Sub AnalizaCuerpo(cuerpo As SyntaxList(Of StatementSyntax), context As SyntaxNodeAnalysisContext)

        ' Revisa cada statement
        For Each statement As StatementSyntax In cuerpo
            ' Si es un nodo de expresión literal
            If TypeOf statement Is LocalDeclarationStatementSyntax Then
                Dim localStatement = DirectCast(statement, LocalDeclarationStatementSyntax)

                Dim textoStatement As String = localStatement.ToString().ToLower

                If Not String.IsNullOrEmpty(textoStatement) Then
                    For Each comando In Constantes.commandosLogicaSQL
                        If textoStatement.Contains(comando) Then
                            ' Separa la cadena de texto para contar la cantidad de veces que aparece un comando de lógica SQL
                            Dim palabras = textoStatement.Split(" "c, ","c, "("c, ")"c)

                            Dim conteoComandos = palabras.Count(Function(p) Constantes.commandosLogicaSQL.Contains(p))

                            If conteoComandos > 2 Then
                                Dim diag As Diagnostic = Diagnostic.Create(Regla001RepositorioCerebral, statement.GetLocation(), textoStatement)
                                context.ReportDiagnostic(diag)
                                Exit For
                            End If
                        End If
                    Next
                End If
            End If
        Next
    End Sub


#End Region

End Class
