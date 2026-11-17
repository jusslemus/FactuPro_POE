using MySqlConnector;

public static class Conexion
{
    private static string? cadena;

    public static MySqlConnection Obtener()
    {
        if (cadena == null)
        {
            string ruta = Path.Combine(AppContext.BaseDirectory, "conexion.txt");
            if (!File.Exists(ruta))
                throw new FileNotFoundException("Falta el archivo conexion.txt con la cadena de conexión.");
            cadena = File.ReadAllText(ruta).Trim();
        }
        return new MySqlConnection(cadena);
    }

    public static bool Probar(out string error)
    {
        try
        {
            using var cn = Obtener();
            cn.Open();
            error = "";
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}