namespace Unidad4.Logistica.Grafos
{
    /// <summary>
    /// Arista saliente de un vértice: hacia qué vértice (por índice) y con qué peso.
    /// Es el elemento de la lista de adyacencia.
    /// </summary>
    public readonly record struct Arista(int Destino, int Peso);
}
