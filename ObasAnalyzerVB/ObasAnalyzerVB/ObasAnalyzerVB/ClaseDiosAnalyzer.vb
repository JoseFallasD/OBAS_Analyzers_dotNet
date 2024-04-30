Imports ObasAnalyzerVB.Utilerias.ObasAnalyzer

<DiagnosticAnalyzer(LanguageNames.VisualBasic)>
Public Class ClaseDiosAnalyzer
    Inherits DiagnosticAnalyzer

#Region "Reglas"
    Friend Const Category As String = "OBAS-SmellArquitectura"

    Friend Shared ReadOnly Regla001ClaseDios As DiagnosticDescriptor = New DiagnosticDescriptor(
            "RA16001",
            "RA16-001: La clase es muy grande ",
            "La clase '{0}' tiene demasiadas líneas de código.",
            Category,
            DiagnosticSeverity.Error,
            True)


    Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
        Get
            Return ImmutableArray.Create(Regla001ClaseDios)
        End Get
    End Property

    Public Overrides Sub Initialize(ByVal context As AnalysisContext)
        ' Registra el tipo de sintaxis que se analiza para cada regla
        context.RegisterSyntaxNodeAction(AddressOf AnalyzeRegla001ClaseDios, SyntaxKind.ClassBlock)
    End Sub
#End Region

#Region "Analizadores"

    ''' <summary>
    ''' Valida el cumplimiento de la regla
    ''' "RA16-001: La clase es muy grande"
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub AnalyzeRegla001ClaseDios(ByVal context As SyntaxNodeAnalysisContext)
        ' Obtiene la declaración de la clase
        Dim classDeclaration = DirectCast(context.Node, ClassBlockSyntax)

        ' Obtiene la posición de inicio y fin del bloque de clase
        Dim startLine As Integer = classDeclaration.BlockStatement.GetLocation().GetLineSpan().StartLinePosition.Line
        Dim endLine As Integer = classDeclaration.EndBlockStatement.GetLocation().GetLineSpan().EndLinePosition.Line

        ' Obtiene la cantidad de líneas de código
        Dim conteoLineas As Integer = endLine - startLine + 1

        ' Si sobrepasa el límite, reporte el error
        If conteoLineas > Constantes.limiteLineasCodigoClase Then
            Dim diag = Diagnostic.Create(Regla001ClaseDios, classDeclaration.ClassStatement.Identifier.GetLocation(), classDeclaration.ClassStatement.Identifier.ValueText)
            context.ReportDiagnostic(diag)
        End If
    End Sub

#End Region


End Class

