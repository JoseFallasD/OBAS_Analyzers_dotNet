Imports ObasAnalyzerVB.Utilerias.ObasAnalyzer

<DiagnosticAnalyzer(LanguageNames.VisualBasic)>
Public Class InterfaceAmbiguaAnalyzer
    Inherits DiagnosticAnalyzer

#Region "Reglas"
    Friend Const Category As String = "OBAS-SmellArquitectura"

    Friend Shared ReadOnly Regla001InterfaceAmbigua As DiagnosticDescriptor = New DiagnosticDescriptor(
            "RA14001",
            "RA14-001: Interface Ambigua ",
            "La interface {0} tiene demasiados métodos.",
            Category,
            DiagnosticSeverity.Error,
            True)


    Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
        Get
            Return ImmutableArray.Create(Regla001InterfaceAmbigua)
        End Get
    End Property

    Public Overrides Sub Initialize(ByVal context As AnalysisContext)
        ' Registra el tipo de sintaxis que se analiza para cada regla
        context.RegisterSyntaxNodeAction(AddressOf AnalyzeRegla001InterfaceAmbigua, SyntaxKind.InterfaceBlock)
    End Sub
#End Region

#Region "Analizadores"

    ''' <summary>
    ''' Valida el cumplimiento de la regla
    ''' "RA14-001: Interface Ambigua"
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub AnalyzeRegla001InterfaceAmbigua(ByVal context As SyntaxNodeAnalysisContext)
        Dim interfaceDeclaration = DirectCast(context.Node, InterfaceBlockSyntax)

        ' Cuenta la cantidad de métodos presentes
        Dim cantidadMetodos As Integer = 0
        For Each miembro In interfaceDeclaration.Members
            If TypeOf miembro Is MethodStatementSyntax Then
                cantidadMetodos += 1
            End If
        Next

        ' Si sobrepasa el límite, reporte el error
        If cantidadMetodos > Constantes.limiteMetodosInterface Then
            Dim diag = Diagnostic.Create(Regla001InterfaceAmbigua, interfaceDeclaration.InterfaceStatement.Identifier.GetLocation(), interfaceDeclaration.InterfaceStatement.Identifier.Text)
            context.ReportDiagnostic(diag)
        End If
    End Sub

#End Region


End Class

