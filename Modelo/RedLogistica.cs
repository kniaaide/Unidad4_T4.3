using Unidad4.Logistica.Grafos;

namespace Unidad4.Logistica.Modelo
{
    /// <summary>
    /// Red de distribución de ejemplo. Los nombres son ciudades y las distancias en
    /// kilómetros son VALORES ILUSTRATIVOS para el ejercicio, no datos de una fuente oficial.
    /// </summary>
    public static class RedLogistica
    {
        public static readonly string[] Ciudades =
        {
            "CDMX", "Toluca", "Cuernavaca", "Pachuca", "Puebla", "Tlaxcala", "Querétaro"
        };

        public static GrafoPonderado Construir()
        {
            var g = new GrafoPonderado();
            foreach (string ciudad in Ciudades)
            {
                g.AgregarVertice(ciudad);
            }

            // Orden de inserción fijo: determina el orden de las listas de adyacencia.
            g.AgregarArista("CDMX", "Toluca", 65);
            g.AgregarArista("CDMX", "Cuernavaca", 85);
            g.AgregarArista("CDMX", "Pachuca", 95);
            g.AgregarArista("CDMX", "Puebla", 130);
            g.AgregarArista("Toluca", "Querétaro", 215);
            g.AgregarArista("Pachuca", "Tlaxcala", 110);
            g.AgregarArista("Puebla", "Tlaxcala", 45);
            g.AgregarArista("Cuernavaca", "Puebla", 150);
            g.AgregarArista("Pachuca", "Querétaro", 190);

            return g;
        }
    }
}
