using System;
using System.Collections.Generic;
using System.Text;

namespace Utilerias.ObasAnalyzerCSharp
{
    class Constantes
    {
        //Lista de constanstes de nomenclaturas utilizadas por el sistema
        public static readonly string nomenclaturaModelo = "vm";
        public static readonly string nomenclaturaVista = "cshtml";
        public static readonly string nomenclaturaControlador = "controller";
        public static readonly string nomenclaturaServicio = "wsr";
        public static readonly string nomenclaturaRepositorio = "dal";
        public static readonly string nomenclaturaEntidad = "ent";
        public static readonly string nomenclaturaAccionBaseDatos = "vgo_conexion";
        public static readonly string nomenclaturaAccesoBaseDatos = "dal";
        public static readonly string nomenclaturaLogicaNegocio = "bll";

        //Lista de constanstes de límites definidos
        //Límite de cantidad de If, Switch, For, Is, etc que puede tener un método de un Controller
        public static readonly int limiteControlFlujo = 20;
        //Límite de métodos que puede tener una clase
        public static readonly int limiteMetodosClase = 20;
        //Límite de parámetros que puede recibir un método
        public static readonly int limiteParametrosMetodo = 10;
        //Límite de clases referenciadas en un método
        public static readonly int limiteDependenciasClase = 5;
        
        //public static readonly int limiteLogicaRepositorio = 2;

        //Límite de comandos de lógica SQL que puede tener un método ("where", "and", "or", "join", "exists", "not", "from", "xor", "if", "else", "case", "in")
        public static readonly int limiteLogicaComandos = 3;
        //Límite de Entidades que son referenciadas en un mismo Repositorio (Dal)
        public static readonly int limiteEntidadesRepositorio = 3;
        //Límite de veces que se invoca al objeto de conexión a base de datos (vgo_conexion) en un Repositorio (Dal)
        public static readonly int limiteAccionesBaseDatos = 50;
        //Límite de métodos que puede tener una Interface
        public static readonly int limiteMetodosInterface = 20;
        //Límite de líneas de código de una clase
        public static readonly int limiteLineasCodigoClase = 500;

        //Lista de etiquetas HTML comunes
        public static readonly string[] etiquetasHtml = {
            "@html",
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
            "<ol", "<optgroup", "<option", "<output",
            "<p>", "<picture", "<pre", "<progress",
            "<q>",
            "<rb", "<rp", "<rt", "<rtc", "<ruby",
            "<s>", "<samp", "<script", "<search", "<section", "<small", "<source", "<span", "<strong", "<style", "<sub", "<sup", "<svg",
            "<table", "<tbody", "<td", "<template", "<textarea", "<tfoot", "<th", "<thead", "<time", "<title", "<tr", "<track",
            "<u>", "<ul",
            "<var", "<video",
            "<wbr",
            "/>"
        };

        //Lista de etiquetas HTML que se parecen a etiquetas XML de configuración
        public static readonly string[] etiquetasHtmlPermitidas = {
            "<select","<summary>",
            "<object",
            "<param>",
            "</"
        };

        //Lista de comandos SQL comunes y comandos propios de nomenclatura del framework
        public static readonly string[] comandosSQL = {
            //comandos comunes
            "alter ",
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
            "table",
            //comandos propios de framework
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
       };

        public static readonly string[] commandosLogicaSQL =
        {
            "where", "and", "or", "join", "exists", "not", "from", "xor", "if", "else", "case", "in"
        };

        //Lista de comandos SQL similares a métodos aplicables a listas en Visual Basic
        public static readonly string[] comandosSQLPermitidos = {
            "update",
            "view",
            "where"
        };

        //Lista de referencias a librerías de acceso de datos
        public static readonly string[] libreriasAccesoDatos = {
            "System.Data", "System.Linq", "System.Xml", "System.Collections"
        };
    }
}