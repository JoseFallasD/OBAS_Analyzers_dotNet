Imports ObasAnalyzerVB.Utilerias.ObasAnalyzer

<DiagnosticAnalyzer(LanguageNames.VisualBasic)>
Public Class ViolacionArquitecturaAnalyzer
    Inherits DiagnosticAnalyzer

#Region "Reglas"
    Friend Const Category As String = "OBAS-SmellArquitectura-MVC"

    Friend Shared ReadOnly Regla001ViolacionArquitectura As DiagnosticDescriptor = New DiagnosticDescriptor(
        "RA15001",
        "RA15-001: La capa de servicios referencia acceso de datos",
        "La clase de servicio no puede referenciar directamente al objeto '{0}' de la capa de acceso a datos",
        Category,
        DiagnosticSeverity.Error,
        True)

    Friend Shared ReadOnly Regla002ViolacionArquitectura As DiagnosticDescriptor = New DiagnosticDescriptor(
        "RA15002",
        "RA15-002: La capa de lógica referencia servicios",
        "La clase de lógica de negocios no puede referenciar directamente al objeto '{0}' de la capa de servicios",
        Category,
        DiagnosticSeverity.Error,
        True)

    Friend Shared ReadOnly Regla003ViolacionArquitectura As DiagnosticDescriptor = New DiagnosticDescriptor(
        "RA15003",
        "RA15-003: La capa de acceso a datos referencia lógica o servicios",
        "La clase de acceso a datos no puede referenciar directamente al objeto '{0}' de la capa de servicios o lógica de negocios",
        Category,
        DiagnosticSeverity.Error,
        True)

    Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
        Get
            Return ImmutableArray.Create(Regla001ViolacionArquitectura, Regla002ViolacionArquitectura, Regla003ViolacionArquitectura)
        End Get
    End Property

    Public Overrides Sub Initialize(ByVal context As AnalysisContext)
        ' Registra el tipo de sintaxis que se analiza para cada regla
        context.RegisterSyntaxNodeAction(AddressOf AnalyzeRegla001ViolacionArquitectura, SyntaxKind.ObjectCreationExpression)
        context.RegisterSyntaxNodeAction(AddressOf AnalyzeRegla002ViolacionArquitectura, SyntaxKind.ObjectCreationExpression)
        context.RegisterSyntaxNodeAction(AddressOf AnalyzeRegla003ViolacionArquitectura, SyntaxKind.ObjectCreationExpression)
    End Sub
#End Region

#Region "Analizadores"

    ''' <summary>
    ''' Valida el cumplimiento de la regla
    ''' "RA15-001: La capa de servicios referencia acceso de datos"
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub AnalyzeRegla001ViolacionArquitectura(ByVal context As SyntaxNodeAnalysisContext)
        ' Obtiene la sintaxis de creación de objetos nuevos
        Dim objectCreationExpression = DirectCast(context.Node, ObjectCreationExpressionSyntax)

        If objectCreationExpression IsNot Nothing Then
            ' Obtiene el nombre de la clase
            Dim claseContenedora = objectCreationExpression.FirstAncestorOrSelf(Of ClassBlockSyntax)?.SyntaxTree?.FilePath?.ToLower()

            ' Comprueba si es una clase de la capa de servicios
            If claseContenedora IsNot Nothing AndAlso claseContenedora.Contains(Constantes.nomenclaturaServicio) Then
                ' Comprueba si el tipo de objeto es de la capa de acceso a datos
                If objectCreationExpression.Type.ToString().ToLower().Contains(Constantes.nomenclaturaRepositorio) Then
                    Dim diag = Diagnostic.Create(Regla001ViolacionArquitectura, context.Node.GetLocation(), objectCreationExpression.Type.ToString())
                    context.ReportDiagnostic(diag)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Valida el cumplimiento de la regla
    ''' "RA15-002: La capa de lógica referencia servicios"
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub AnalyzeRegla002ViolacionArquitectura(ByVal context As SyntaxNodeAnalysisContext)
        ' Obtiene la sintaxis de creación de objetos nuevos
        Dim objectCreationExpression = DirectCast(context.Node, ObjectCreationExpressionSyntax)

        If objectCreationExpression IsNot Nothing Then
            ' Obtiene el nombre de la clase
            Dim claseContenedora = objectCreationExpression.FirstAncestorOrSelf(Of ClassBlockSyntax)?.SyntaxTree?.FilePath?.ToLower()

            ' Comprueba si es una clase de la capa de lógica
            If claseContenedora IsNot Nothing AndAlso claseContenedora.Contains(Constantes.nomenclaturaLogicaNegocio) Then
                ' Comprueba si el tipo de objeto es de la capa de servicios
                If objectCreationExpression.Type.ToString().ToLower().Contains(Constantes.nomenclaturaServicio) Then
                    Dim diag = Diagnostic.Create(Regla002ViolacionArquitectura, context.Node.GetLocation(), objectCreationExpression.Type.ToString())
                    context.ReportDiagnostic(diag)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Valida el cumplimiento de la regla
    ''' "RA15-003: La capa de acceso a datos referencia lógica o servicios"
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub AnalyzeRegla003ViolacionArquitectura(ByVal context As SyntaxNodeAnalysisContext)
        ' Obtiene la sintaxis de creación de objetos nuevos
        Dim objectCreationExpression = DirectCast(context.Node, ObjectCreationExpressionSyntax)

        If objectCreationExpression IsNot Nothing Then
            ' Obtiene el nombre de la clase
            Dim claseContenedora = objectCreationExpression.FirstAncestorOrSelf(Of ClassBlockSyntax)?.SyntaxTree?.FilePath?.ToLower()

            ' Comprueba si es una clase de la capa de acceso a datos
            If claseContenedora IsNot Nothing AndAlso claseContenedora.Contains(Constantes.nomenclaturaRepositorio) Then
                ' Comprueba si el tipo de objeto es de la capa de servicios o lógica de negocios
                If objectCreationExpression.Type.ToString().ToLower().Contains(Constantes.nomenclaturaServicio) OrElse objectCreationExpression.Type.ToString().ToLower().Contains(Constantes.nomenclaturaLogicaNegocio) Then
                    Dim diag = Diagnostic.Create(Regla003ViolacionArquitectura, context.Node.GetLocation(), objectCreationExpression.Type.ToString())
                    context.ReportDiagnostic(diag)
                End If
            End If
        End If
    End Sub
#End Region


End Class

