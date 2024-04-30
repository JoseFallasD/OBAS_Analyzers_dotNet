Imports ObasAnalyzerVB.Utilerias.ObasAnalyzer

<DiagnosticAnalyzer(LanguageNames.VisualBasic)>
Public Class ServicioEntrometidoAnalyzer
    Inherits DiagnosticAnalyzer

#Region "Reglas"
    Friend Const Category As String = "OBAS-SmellArquitectura"

    Friend Shared ReadOnly Regla001ServicioEntrometido As DiagnosticDescriptor = New DiagnosticDescriptor(
            "RA09001",
            "RA09-001: No se permite el uso de comandos SQL en la capa de Servicio ",
            "El Servicio no puede contener el comando SQL '{0}'.",
            Category,
            DiagnosticSeverity.Error,
            True)

    Friend Shared ReadOnly Regla002ServicioEntrometido As DiagnosticDescriptor = New DiagnosticDescriptor(
            "RA09002",
            "RA09-002: No se permite el uso de referencias a librerías de acceso a datos ",
            "El Servicio no puede contener referencias a librerías de acceso a datos'.",
            Category,
            DiagnosticSeverity.Error,
            True)

    Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
        Get
            Return ImmutableArray.Create(Regla001ServicioEntrometido, Regla002ServicioEntrometido)
        End Get
    End Property

    Public Overrides Sub Initialize(ByVal context As AnalysisContext)
        ' Registra el tipo de sintaxis que se analiza para cada regla
        context.RegisterSyntaxNodeAction(AddressOf AnalyzeRegla001ServicioEntrometido, SyntaxKind.ClassBlock)
        context.RegisterSyntaxNodeAction(AddressOf AnalyzeRegla002ServicioEntrometido, SyntaxKind.ClassBlock)
    End Sub
#End Region

#Region "Analyzadores"
    ''' <summary>
    ''' Valida el cumplimiento de la regla
    ''' "RA09-001: No se permite el uso de comandos SQL en la capa de Servicio"
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub AnalyzeRegla001ServicioEntrometido(ByVal context As SyntaxNodeAnalysisContext)
        ' Obtiene la declaración de la clase
        Dim classDeclaration As ClassBlockSyntax = TryCast(context.Node, ClassBlockSyntax)

        ' Obtiene nombre de la clase
        Dim nombreClaseContenedora As String = classDeclaration.SyntaxTree.FilePath

        ' Revisa si el nombre de la clase contenedora contiene la cadena "ws"
        If nombreClaseContenedora.ToLower().Contains(Constantes.nomenclaturaServicio) Then
            Dim textoClase As String = context.Node.SyntaxTree.GetText().ToString().ToLower()

            For Each comando In Constantes.comandosSQL
                If textoClase.Contains(comando) Then
                    Dim diag As Diagnostic = Diagnostic.Create(Regla001ServicioEntrometido, classDeclaration.ClassStatement.Identifier.GetLocation(), comando)
                    context.ReportDiagnostic(diag)

                    Return
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' Valida el cumplimiento de la regla
    ''' "RA09-002: No se permite el uso de referencias a librerías de acceso a datos "
    ''' </summary>
    ''' <param name="context"></param>
    Private Shared Sub AnalyzeRegla002ServicioEntrometido(ByVal context As SyntaxNodeAnalysisContext)
        Dim incumpleRegla As Boolean = False

        ' Obtiene la declaración de la clase
        Dim classDeclaration As ClassBlockSyntax = TryCast(context.Node, ClassBlockSyntax)

        ' Obtiene nombre de la clase
        Dim nombreClaseContenedora As String = classDeclaration.SyntaxTree.FilePath

        ' Revisa si el nombre de la clase contenedora contiene la cadena "ws"
        If nombreClaseContenedora.ToLower().Contains(Constantes.nomenclaturaServicio) Then
            ' Revisa cada uno de los miembros

            For Each member In classDeclaration.Members
                ' Revisa el tipo del miembro
                Dim memberType As ITypeSymbol = context.SemanticModel.GetTypeInfo(member).Type
                If memberType IsNot Nothing AndAlso UsaNamespaceAccesoDatos(memberType.ContainingNamespace.ToString().ToLower()) Then
                    incumpleRegla = True
                    Exit For
                End If

                ' Revisa los atritubos de cada miembro
                Dim declaredSymbol As ISymbol = context.SemanticModel.GetDeclaredSymbol(member)

                If declaredSymbol IsNot Nothing Then
                    Dim memberAttributes As ImmutableArray(Of AttributeData) = declaredSymbol.GetAttributes()

                    For Each attribute In memberAttributes
                        Dim attributeType As Type = attribute.GetType()
                        If attributeType IsNot Nothing AndAlso UsaNamespaceAccesoDatos(attributeType.Namespace.ToLower()) Then
                            incumpleRegla = True
                            Exit For
                        End If
                    Next
                End If
                If incumpleRegla Then Exit For
            Next

            ' Reporta el diagnóstico si incumple la regla
            If incumpleRegla Then
                Dim diagnostic As Diagnostic = Diagnostic.Create(Regla002ServicioEntrometido, classDeclaration.ClassStatement.Identifier.GetLocation(), classDeclaration.ClassStatement.Identifier.ValueText)
                context.ReportDiagnostic(diagnostic)
            End If
        End If
    End Sub


    ''' <summary>
    ''' Función para comprobar si la referencia existe en la lista de referencias de librerías de acceso a Datos
    ''' </summary>
    ''' <param name="namespaceSymbol"></param>
    ''' <returns></returns>
    Private Shared Function UsaNamespaceAccesoDatos(ByVal nombreNamespace As String) As Boolean
        If Constantes.libreriasAccesoDatos.Contains(nombreNamespace) Then
            Return True
        End If
        Return False
    End Function


#End Region

End Class
