using System;
using System.Collections.Generic;
using System.Linq;
using Unidad4.Logistica.Grafos;

namespace Unidad4.Logistica.Algoritmos
{
    /// <summary>
    /// Estado del algoritmo tras una iteración: vértice elegido, distancias vigentes,
    /// vértices ya definitivos y relajaciones realizadas en esa iteración.
    /// </summary>
    public sealed record PasoDijkstra(
        int Numero,
        string VerticeElegido,
        int[] Distancias,
        IReadOnlyList<string> Visitados,
        IReadOnlyList<string> Relajaciones);

    /// <summary>Resultado de Dijkstra desde un origen: distancias, predecesores y traza.</summary>
    public sealed class ResultadoDijkstra
    {
        /// <summary>Valor que representa distancia infinita (vértice no alcanzable).</summary>
        public const int Infinito = int.MaxValue;

        public GrafoPonderado Grafo { get; }
        public int Origen { get; }
        public int[] Distancias { get; }
        public int[] Previos { get; }
        public IReadOnlyList<PasoDijkstra> Pasos { get; }

        public ResultadoDijkstra(GrafoPonderado grafo, int origen, int[] distancias, int[] previos,
                                 IReadOnlyList<PasoDijkstra> pasos)
        {
            Grafo = grafo;
            Origen = origen;
            Distancias = distancias;
            Previos = previos;
            Pasos = pasos;
        }

        /// <summary>
        /// Reconstruye el camino desde el origen hasta el destino siguiendo los predecesores
        /// hacia atrás y luego invirtiendo la lista. Devuelve lista vacía si no hay camino.
        /// </summary>
        public List<string> ReconstruirCamino(int destino)
        {
            var camino = new List<string>();
            if (Distancias[destino] == Infinito) return camino;

            for (int actual = destino; actual != -1; actual = Previos[actual])
            {
                camino.Add(Grafo.Vertices[actual]);
            }
            camino.Reverse();
            return camino;
        }
    }

    /// <summary>
    /// Algoritmo de Dijkstra para caminos mínimos desde un origen en un grafo con pesos no negativos.
    /// Implementación con selección lineal del mínimo: O(V²) de tiempo, adecuada para grafos
    /// de tamaño moderado y para mostrar la traza. Desempate: el vértice de menor índice.
    /// </summary>
    public static class Dijkstra
    {
        public static ResultadoDijkstra Calcular(GrafoPonderado grafo, int origen)
        {
            int n = grafo.CantidadVertices;
            if (origen < 0 || origen >= n)
                throw new ArgumentOutOfRangeException(nameof(origen));

            var distancias = new int[n];
            var previos = new int[n];
            var visitado = new bool[n];
            Array.Fill(distancias, ResultadoDijkstra.Infinito);
            Array.Fill(previos, -1);
            distancias[origen] = 0;

            var pasos = new List<PasoDijkstra>();

            for (int numero = 1; numero <= n; numero++)
            {
                // 1. Seleccionar el vértice no visitado con menor distancia provisional.
                int u = -1;
                for (int i = 0; i < n; i++)
                {
                    if (!visitado[i] && distancias[i] != ResultadoDijkstra.Infinito
                        && (u == -1 || distancias[i] < distancias[u]))
                    {
                        u = i;
                    }
                }

                // Si no quedan vértices alcanzables, el algoritmo termina.
                if (u == -1) break;

                // 2. Marcar u como definitivo: su distancia ya es mínima.
                visitado[u] = true;

                // 3. Relajar las aristas salientes de u hacia vértices no definitivos.
                var relajaciones = new List<string>();
                foreach (Arista arista in grafo.Adyacentes(u))
                {
                    int v = arista.Destino;
                    if (visitado[v]) continue;

                    long candidato = (long)distancias[u] + arista.Peso;   // long evita desbordamiento
                    if (candidato < distancias[v])
                    {
                        distancias[v] = (int)candidato;
                        previos[v] = u;
                        relajaciones.Add($"{grafo.Vertices[v]} = {candidato} (vía {grafo.Vertices[u]})");
                    }
                }

                // 4. Registrar el estado de la iteración para la traza.
                pasos.Add(new PasoDijkstra(
                    numero,
                    grafo.Vertices[u],
                    (int[])distancias.Clone(),
                    Enumerable.Range(0, n).Where(i => visitado[i]).Select(i => grafo.Vertices[i]).ToList(),
                    relajaciones));
            }

            return new ResultadoDijkstra(grafo, origen, distancias, previos, pasos);
        }
    }
}
