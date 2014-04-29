using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gato_M_M
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Button[,] btns; //Una matriz para los botones para majarlos mas facil con for.
        Gato gato; //El tablero q se esta jugando
        IAGato ia; //La IA del gato.
        private void Form1_Load(object sender, EventArgs e)
        {
            btns = new Button[3, 3]; //La matriz de botones es de 3x3
            btns[0, 0] = button1; //Se agregan en orden por linea.
            btns[0, 1] = button2;
            btns[0, 2] = button3;
            btns[1, 0] = button4;
            btns[1, 1] = button5;
            btns[1, 2] = button6;
            btns[2, 0] = button7;
            btns[2, 1] = button8;
            btns[2, 2] = button9;
            reiniciar();
            ia = new IAGato(1); //Se crea la IA con un nivel por default(No se utiliza este nivel durante el juego pero se podria llegar a usar si se modifica el codigo del formulario)
            tableLayoutPanel1.Enabled = false; //Se desactiva el panel  impidiendo jugar 
            button10.Enabled = true; //Se activa el boton que comienza el juego.
        }
        private void reiniciar() //Este metodo prepara todo para que comienze el juego
        {
            for (int i = 0; i < 3; i++)//Se recorren los botones
            {
                for (int j = 0; j < 3; j++)
                {
                    btns[i, j].Text = ""; //Se limpia su texto
                    btns[i, j].BackColor = Color.White; //Se establece blanco como fondo
                    btns[i, j].ForeColor = Color.Black; //Negro como letras
                    btns[i, j].Font = new Font("Arial", 75); //La letra arial y un poco grande
                    btns[i, j].Click += new EventHandler(btn_Click); //Se asocian al mismo manejador.
                }
            }
            gato = new Gato();//Se crea un nuego juego (tablero)
            tableLayoutPanel1.Enabled = true; //Se activan todos los botones.
            button10.Enabled = false; //Se desactiva el boton de jugar.
        }

        void btn_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button b = (Button)sender; //Para cada boton en la matriz
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (btns[i, j] == b) //Si es igual al que se pulso (b)
                        {
                            if (gato.estaVacia(j, i)) // Si la posicon en el gato esta vacia
                            {
                                gato.tirar(j, i, (gato.esTurnoX()) ? 'X' : 'O'); //Tiramos de acuerdo al turno q corresponda
                                btns[i, j].Text = "" + gato.getCasilla(j, i);//cambiamos el texto
                                juegaCompu(); //Y pedimos a la compu q haga el movimiento.
                            }
                            i = 3; j = 3; continue;//No hace falta seguir buscando asi q rompemos los dos ciclos.
                        }
                    }
                }
                if (!gato.juegoEnCurso()) //Si ya se termino de jugar
                {
                    tableLayoutPanel1.Enabled = false;//Se desactivan las casillas (por si se pudice tirar por tirar)
                    button10.Enabled = true;//Se activa el boton jugar
                    gato.resaltarGanador(btns); // Se resalta el ganador.
                }
            }
        }

        private void juegaCompu()
        {
            if (thh.Checked) return;//Si se esta jugando hombre vs hombre no hago nada.
            if (aprendiz.Checked) //Si se juega con el algoritmo aprendiz
            {
                ia.Tirar2(gato, btns, (int)numericUpDown1.Value);//EL metodo termina en 2
            }
            else//Si no pues no termina en dos
            {
                ia.Tirar(gato, btns, (int)numericUpDown1.Value);
            }
            //Notar que se manda el numero de niveles a explorar
        }

        private void button10_Click(object sender, EventArgs e)
        {
            reiniciar();//El boton de jugar... reinicia todo
            if (tmh.Checked) juegaCompu(); //Y si juega la compu primero se manda a traer el metodo correspondiente
        }


        private void tmh_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
   
