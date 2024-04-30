Namespace Utilerias.ObasAnalyzer

    Public Class Constantes
        'Lista de constanstes de nomenclaturas utilizadas por el sistema
        Public Shared ReadOnly nomenclaturaModelo As String = "vm"
        Public Shared ReadOnly nomenclaturaVista As String = "cshtml"
        Public Shared ReadOnly nomenclaturaControlador As String = "controller"
        Public Shared ReadOnly nomenclaturaServicio As String = "ws"
        Public Shared ReadOnly nomenclaturaRepositorio As String = "dal"
        Public Shared ReadOnly nomenclaturaEntidad As String = "ent"
        Public Shared ReadOnly nomenclaturaAccionBaseDatos As String = "vgo_conexion"
        Public Shared ReadOnly nomenclaturaAccesoBaseDatos As String = "dal"
        Public Shared ReadOnly nomenclaturaLogicaNegocio As String = "bll"

        'Lista de constanstes de límites definidos
        Public Shared ReadOnly limiteControlFlujo As Integer = 20
        Public Shared ReadOnly limiteMetodosClase As Integer = 20
        Public Shared ReadOnly limiteParametrosMetodo As Integer = 10
        Public Shared ReadOnly limiteDependenciasClase As Integer = 5
        'Public Shared ReadOnly limiteLogicaRepositorio As Integer = 2
        Public Shared ReadOnly limiteLogicaComandos As Integer = 3
        Public Shared ReadOnly limiteEntidadesRepositorio As Integer = 2
        Public Shared ReadOnly limiteAccionesBaseDatos As Integer = 1
        Public Shared ReadOnly limiteMetodosInterface As Integer = 5
        Public Shared ReadOnly limiteLineasCodigoClase As Integer = 500


        'Lista de etiquetas HTML comunes
        Public Shared ReadOnly etiquetasHtml() As String = {
            "@HTML",
            "<a ", "<abbr", "<address", "<area", "<article", "<aside", "<audio",
            "<b>", "<base", "<bdi", "<bdo", "<blockquote", "<body", "<br", "<button",
            "<canvas", "<caption", "<cite", "<code", "<col", "<colgroup",
            "<data", "<datalist", "<dd", "<del", "<details", "<dfn", "<dialog", "<div", "<dl", "<dt",
            "<em", "<embed",
            "<fieldset", "<figcaption", "<figure", "<footer", "<form",
            "<h1", "<h2", "<h3", "<h4", "<h5", "<h6", "<head", "<header", "<hgroup", "<hr", "<html",
            "<i>", "<iframe", "<img", "<input", "<ins",
            "<kbd", "<keygen",
            "<label", "<legend", "<li", "<link",
            "<main", "<map", "<mark", "<menu", "<menuitem", "<meta", "<meter",
            "<nav", "<noscript",
            "<object", "<ol", "<optgroup", "<option", "<output",
            "<p>", "<picture", "<pre", "<progress",
            "<q>",
            "<rb", "<rp", "<rt", "<rtc", "<ruby",
            "<s>", "<samp", "<script", "<search", "<section", "<small", "<source", "<span", "<strong", "<style", "<sub", "<sup", "<svg",
            "<table", "<tbody", "<td", "<template", "<textarea", "<tfoot", "<th", "<thead", "<time", "<title", "<tr", "<track",
            "<u>", "<ul",
            "<var", "<video",
            "<wbr",
            "/>"
        }

        'Lista de etiquetas HTML que se parecen a etiquetas XML de configuración
        Public Shared ReadOnly etiquetasHtmlPermitidas() As String = {
            "<select", "<summary>",
            "<param>",
            "</"
        }

        'Lista de comandos SQL comunes y comandos propios de nomenclatura del framework
        Public Shared ReadOnly comandosSQL() As String = {
            "alter ", 'comandos comunes
            "create ",
            "database", "delete from", "drop",
            "excecute",
            "grant", "group by",
            "having",
            "insert into",
            "lock",
            "order by",
            "replicate", "revoke",
            "select", "schema", "syncrhonize",
            "table", 'comandos propios de framework
            "pr_", "prd_", "pri_", "pru_", "sp_", "fc_",
            "borrarregistro",
            "ejecutarfuncionretornotabla",
            "ejecutarvista",
            "insertarregistro",
            "listarregistros",
            "listarregistroscoleccion",
            "listarregistroslista",
            "listarregistroslistacoleccion",
            "modificarregistro",
            "obtenerdatospaginacionfuncionretornotabla",
            "obtenerdatospaginacionvista",
            "obtenerregistro",
            "obtenerregistrodetallado",
            "obtenertotalregistrosconsulta"
       }

        Public Shared ReadOnly commandosLogicaSQL() As String = {
            "where", "and", "or", "join", "exists", "not", "from", "xor", "if", "else", "case", "in"
        }

        Public Shared ReadOnly commandosEjecucionSQL() As String = {
            "ejecutar", "listar", "obtener"
        }

        'Lista de comandos SQL similares a métodos aplicables a listas en Visual Basic
        Public Shared ReadOnly comandosSQLPermitidos() As String = {
            "update",
            "view",
            "where"
        }

        'Lista de referencias a librerías de acceso de datos
        Public Shared ReadOnly libreriasAccesoDatos() As String = {
            "system.data", "system.linq", "system.xml", "system.collections"
        }
    End Class

End Namespace

