using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Unidad4.Logistica.Algoritmos;
using Unidad4.Logistica.Grafos;
using Unidad4.Logistica.Modelo;

namespace Unidad4.Logistica
{
    /// <summary>
    /// Interfaz de la Tarea 4.3. Muestra la estructura interna del grafo (matriz y listas
    /// de adyacencia), calcula el camino mínimo con Dijkstra y presenta la traza paso a paso.
    /// </summary>
    public class MainForm : Form
    {
        private readonly GrafoPonderado _grafo = RedLogistica.Construir();

        private readonly Label lblOrigen = new Label { Text = "Origen:", Location = new Point(20, 22), AutoSize = true };
        private readonly ComboBox cmbOrigen = new ComboBox { Location = new Point(80, 18), Size = new Size(160, 25), DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly Label lblDestino = new Label { Text = "Destino:", Location = new Point(260, 22), AutoSize = true };
        private readonly ComboBox cmbDestino = new ComboBox { Location = new Point(330, 18), Size = new Size(160, 25), DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly Button btnCalcular = new Button { Text = "Calcular ruta óptima", Location = new Point(510, 16), Size = new Size(170, 30) };

        private readonly Label lblMatriz = new Label { Text = "Matriz de adyacencia", Location = new Point(20, 58), AutoSize = true };
        private readonly TextBox txtMatriz = new TextBox
        {
            Location = new Point(20, 80),
            Size = new Size(360, 200),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            Font = new Font("Consolas", 9f)
        };

        private readonly Label lblListas = new Label { Text = "Listas de adyacencia", Location = new Point(395, 58), AutoSize = true };
        private readonly TextBox txtListas = new TextBox
        {
            Location = new Point(395, 80),
            Size = new Size(365, 200),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Consolas", 9f)
        };

        private readonly Label lblTraza = new Label { Text = "Traza del algoritmo (distancia provisional desde el origen, en km)", Location = new Point(20, 295), AutoSize = true };
        private readonly DataGridView dgvTraza = new DataGridView
        {
            Location = new Point(20, 316),
            Size = new Size(740, 250),
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        private readonly TextBox txtResultado = new TextBox
        {
            Location = new Point(20, 580),
            Size = new Size(740, 90),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical
        };

        public MainForm()
        {
            Text = "Unidad 4 - Tarea 4.3: Dijkstra en una red de distribución";
            ClientSize = new Size(790, 690);
            StartPosition = FormStartPosition.CenterScreen;

            Controls.AddRange(new Control[]
            {
                lblOrigen, cmbOrigen, lblDestino, cmbDestino, btnCalcular,
                lblMatriz, txtMatriz, lblListas, txtListas,
                lblTraza, dgvTraza, txtResultado
            });

            foreach (string ciudad in _grafo.Vertices)
            {
                cmbOrigen.Items.Add(ciudad);
                cmbDestino.Items.Add(ciudad);
            }
            cmbOrigen.SelectedItem = "CDMX";
            cmbDestino.SelectedItem = "Tlaxcala";

            txtMatriz.Text = _grafo.TextoMatrizAdyacencia();
            txtListas.Text = _grafo.TextoListasAdyacencia();

            btnCalcular.Click += (s, e) => Calcular();
        }

        private void Calcular()
        {
            if (cmbOrigen.SelectedItem is not string origen || cmbDestino.SelectedItem is not string destino)
            {
                MessageBox.Show(this, "Seleccione origen y destino.", "Datos incompletos");
                return;
            }

            int indiceOrigen = _grafo.ObtenerIndice(origen);
            int indiceDestino = _grafo.ObtenerIndice(destino);

            ResultadoDijkstra resultado = Dijkstra.Calcular(_grafo, indiceOrigen);
            MostrarTraza(resultado);

            var camino = resultado.ReconstruirCamino(indiceDestino);
            if (camino.Count == 0)
            {
                txtResultado.Text = $"No existe ruta desde {origen} hasta {destino}.";
                return;
            }

            txtResultado.Text =
                $"Camino más corto: {string.Join(" -> ", camino)}" + Environment.NewLine +
                $"Distancia total: {resultado.Distancias[indiceDestino]} km" + Environment.NewLine +
                "Interpretación: el algoritmo fija definitivamente la distancia de cada vértice al " +
                "elegirlo con el menor valor provisional, por lo que la ruta mostrada es de costo mínimo " +
                "cuando todos los pesos son no negativos.";
        }

        /// <summary>Muestra una fila por iteración con las distancias provisionales de todas las ciudades.</summary>
        private void MostrarTraza(ResultadoDijkstra resultado)
        {
            dgvTraza.Rows.Clear();
            dgvTraza.Columns.Clear();

            dgvTraza.Columns.Add("colPaso", "Paso");
            dgvTraza.Columns.Add("colElegido", "Vértice elegido");
            foreach (string ciudad in _grafo.Vertices)
            {
                dgvTraza.Columns.Add("col_" + ciudad, ciudad);
            }
            dgvTraza.Columns.Add("colRelax", "Relajaciones");
            dgvTraza.Columns.Add("colVisitados", "Visitados");

            foreach (PasoDijkstra paso in resultado.Pasos)
            {
                var valores = new object[2 + _grafo.CantidadVertices + 2];
                valores[0] = paso.Numero;
                valores[1] = paso.VerticeElegido;
                for (int i = 0; i < _grafo.CantidadVertices; i++)
                {
                    int d = paso.Distancias[i];
                    valores[2 + i] = d == ResultadoDijkstra.Infinito ? "∞" : d.ToString();
                }
                valores[^2] = paso.Relajaciones.Count == 0 ? "(ninguna)" : string.Join("; ", paso.Relajaciones);
                valores[^1] = string.Join(", ", paso.Visitados);
                dgvTraza.Rows.Add(valores);
            }
        }
    }
}
