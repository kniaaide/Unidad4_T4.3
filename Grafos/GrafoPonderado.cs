using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unidad4.Logistica.Grafos
{
    /// <summary>
    /// Grafo ponderado no dirigido representado con lista de adyacencia.
    /// Cada vértice tiene un índice entero (0..n-1) y un nombre. La matriz de
    /// adyacencia se construye a partir de las listas para mostrarla y compararla.
    /// Pesos positivos (requisito de Dijkstra).
    /// </summary>
    public sealed class GrafoPonderado
    {
        private readonly List<string> _vertices = new();
        private readonly Dictionary<string, int> _indices = new();
        private readonly List<List<Arista>> _listas = new();

        /// <summary>Nombres de los vértices en el orden de sus índices.</summary>
        public IReadOnlyList<string> Vertices => _vertices;

        /// <summary>Número de vértices.</summary>
        public int CantidadVertices => _vertices.Count;

        /// <summary>Agrega un vértice si no existe. Devuelve su índice.</summary>
        public int AgregarVertice(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del vértice no puede estar vacío.", nameof(nombre));

            if (_indices.TryGetValue(nombre, out int existente))
                return existente;

            int indice = _vertices.Count;
            _vertices.Add(nombre);
            _indices[nombre] = indice;
            _listas.Add(new List<Arista>());
            return indice;
        }

        /// <summary>Devuelve el índice de un vértice. Lanza excepción si no existe.</summary>
        public int ObtenerIndice(string nombre)
        {
            if (!_indices.TryGetValue(nombre, out int indice))
                throw new ArgumentException($"El vértice \"{nombre}\" no existe en el grafo.", nameof(nombre));
            return indice;
        }

        /// <summary>
        /// Agrega una arista no dirigida entre dos vértices. Se almacena en las dos listas
        /// de adyacencia; el peso debe ser estrictamente positivo.
        /// </summary>
        public void AgregarArista(string origen, string destino, int peso)
        {
            if (peso <= 0)
                throw new ArgumentOutOfRangeException(nameof(peso), "El peso debe ser mayor que cero.");

            int i = ObtenerIndice(origen);
            int j = ObtenerIndice(destino);
            _listas[i].Add(new Arista(j, peso));
            _listas[j].Add(new Arista(i, peso));
        }

        /// <summary>Aristas salientes del vértice indicado (lista de adyacencia).</summary>
        public IReadOnlyList<Arista> Adyacentes(int indice) => _listas[indice];

        /// <summary>
        /// Matriz de adyacencia n × n. Cero significa "sin arista"; un valor positivo es el peso.
        /// Ocupa O(n²) de memoria, aunque el grafo tenga pocas aristas.
        /// </summary>
        public int[,] ObtenerMatrizAdyacencia()
        {
            int n = CantidadVertices;
            var matriz = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                foreach (Arista a in _listas[i])
                {
                    matriz[i, a.Destino] = a.Peso;
                }
            }
            return matriz;
        }

        /// <summary>Texto tabular de la matriz de adyacencia, para mostrarla en la interfaz.</summary>
        public string TextoMatrizAdyacencia()
        {
            int n = CantidadVertices;
            int[,] m = ObtenerMatrizAdyacencia();
            var sb = new StringBuilder();

            sb.Append(new string(' ', 12));
            foreach (string v in _vertices) sb.Append(Ajustar(v, 11));
            sb.AppendLine();

            for (int i = 0; i < n; i++)
            {
                sb.Append(Ajustar(_vertices[i], 12));
                for (int j = 0; j < n; j++)
                {
                    sb.Append(Ajustar(m[i, j].ToString(), 11));
                }
                sb.AppendLine();
            }
            sb.AppendLine("(0 = sin arista; valor = peso en km)");
            return sb.ToString();
        }

        /// <summary>Texto de las listas de adyacencia, una línea por vértice.</summary>
        public string TextoListasAdyacencia()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < CantidadVertices; i++)
            {
                string vecinos = string.Join(", ",
                    _listas[i].Select(a => $"{_vertices[a.Destino]} ({a.Peso})"));
                sb.AppendLine($"{_vertices[i]} -> {(vecinos.Length == 0 ? "(sin aristas)" : vecinos)}");
            }
            return sb.ToString();
        }

        private static string Ajustar(string texto, int ancho)
        {
            return texto.Length >= ancho ? texto.Substring(0, ancho - 1) + " " : texto.PadRight(ancho);
        }
    }
}
