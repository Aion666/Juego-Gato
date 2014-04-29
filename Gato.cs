using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
namespace Gato_M_M
{
    class Gato
    {
        /*
         * Esta clase es la que controla el tablero del gato
         * */
        bool tX = true; //El turno es de las X??
        char[,] casillas; // La matriz de casillas
        Point[,] formasDeGanar; //Las formas de ganar
        public Gato() //Se crea un tablero nuevo??
        {
            casillas = new char[3, 3]; //Las casillas son 9,  3x3
            for (int i = 0; i < 3; i++)//Para recorrer todas las casillas 
            {
                for (int j = 0; j < 3; j++)
                {
                    casillas[i, j] = ' '; //Se coloca un espacio señalando q es una casilla vacia (al inicio todas estan vacias).
                }
            }
            formasDeGanar = new Point[8, 3];//Hay 8 formas de ganar 3 horizontales, 3 verticales, 2 diagonales
            
			formasDeGanar[0, 0] = new Point(0, 0);//Cada forma de ganar son 3 casillas en linea.
            formasDeGanar[0,1]=new Point (0,1);// Por lo tanto seria un arreglo de tamaño 8 de arreglos de tamaño 3
            formasDeGanar[0, 2] = new Point(0, 2); //Los point es para señalar la coordenada que se nesecita para ganar
            
			formasDeGanar[1,0]=new  Point (1,0); //Y son tres puntos por cada convinacion (Las tres casillas)...
            formasDeGanar[1, 1] = new Point(1, 1);//Primero agrego las horizontales
            formasDeGanar[1, 2] = new Point(1, 2);
           
			formasDeGanar[2, 0] = new Point(2, 0);
            formasDeGanar[2, 1] = new Point(2, 1);
            formasDeGanar[2, 2] = new Point(2, 2);
            
			formasDeGanar[3, 0] = new Point(0, 0);//Luego se agregan las verticales
            formasDeGanar[3, 1] = new Point(1, 0);
            formasDeGanar[3, 2] = new Point(2, 0);
            
			formasDeGanar[4, 0] = new Point(0, 1);
            formasDeGanar[4, 1] = new Point(1, 1);
            formasDeGanar[4, 2] = new Point(2, 1);
           
			formasDeGanar[5, 0] = new Point(0, 2);
            formasDeGanar[5, 1] = new Point(1, 2);
            formasDeGanar[5, 2] = new Point(2, 2);
            
			formasDeGanar[6, 0] = new Point(0, 0);//Por ultimo las diagonales.
            formasDeGanar[6, 1] = new Point(1, 1);
            formasDeGanar[6, 2] = new Point(2, 2);
            
			formasDeGanar[7, 0] = new Point(0, 2);
            formasDeGanar[7, 1] = new Point(1, 1);
            formasDeGanar[7, 2] = new Point(2, 0);

        }

        public bool estaOcupuada(int x, int y) 
        {
            return casillas[y, x] != ' ';
        }

        public bool estaVacia(int x, int y)
        {
            return casillas[y, x] == ' ';
        }

        public void  tirar(int x, int y, char jugador)
        {
            if( (jugador  =='X' && tX ) ||(jugador =='O' && !tX))//Se verifica q lo q nos mandan corresponda al turno en turno.
            {
                casillas[y, x] = jugador;
                tX = !tX;//Esto sirve para cambiar el turno
            }
        }

        public void borrar(int x, int y)
        {
            if(( casillas[y,x]=='X' && !tX )|| (casillas [y,x]=='O' && tX )) //Verifiamos q la casilla este ocupada.
            {   //Para borrarse debe de ser una casilla del turno anterior... es ahi el por q se toma en cuenta a tX
                casillas[y, x] = ' ';
                tX = !tX;
            }
        }

        public char getCasilla(int x, int y)
        {
            return casillas[y, x];
        }

        public bool esTurnoX()
        {
            return tX;
        }

        public bool esTurnoO()
        {
            return !tX;
        }

        //Este metodo determina el estado del juego, si hay ganador, es empate o aun se esta jugando.
        private int quienGana()
        {
            for (int i = 0; i < 8; i++)//Para cada forma de ganar
            {
                // Se verifica si las tres casillas son iguales
                // A la matriz casillas primero se le pasa la fila y luego la columna (primero Y y luego X) 
                
                // Debido a q al desplegar por texto se hace mas facil el proceso.
                if (casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == casillas[formasDeGanar[i, 1].Y, formasDeGanar[i, 1].X] &&
                    casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == casillas[formasDeGanar[i, 2].Y, formasDeGanar[i, 2].X])
                {
                    if (casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == 'X') //Ok si las tres casillas son iguales
                    {   //Y ademas tienen X 
                        return 1; //Retorno 1 (indica q ganan las X)
                    }
                    else if (casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == 'O') //Si no (puede darse el caso de q esten vacias y por lo tano sean las tres iguales) verifico que haya una O
                    {
                        return 2; //El 2 vendria a indicar que ganan las O
                    }
                }
            }
            for (int i = 0; i < 3; i++) //Si llegamos a este punto esq no hay ganador
            {
                for (int j = 0; j < 3; j++)//Y buscaremos una casilla vacia
                {
                    if (casillas[i, j] == ' ') return 0; //Si la hay significa q aun se puede jugar.
                }
            }
            //Si ya no se pudo jugar entonces es un empate.
            return 3;
        }

        public void resaltarGanador(Button [,] botones) //Aqui se resalta la combinacion ganadora.
        {   //El procedimiento es parecido al anterior.
            for (int i = 0; i < 8; i++)
            {
                
                if (casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == casillas[formasDeGanar[i, 1].Y, formasDeGanar[i, 1].X] &&
                    casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == casillas[formasDeGanar[i, 2].Y, formasDeGanar[i, 2].X])
                {
                    if (casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == 'X') 
                    {
                        for (int j = 0; j < 3; j++) //Para cada casilla en la forma de ganar.
                        {
                            botones[formasDeGanar[i, j].Y, formasDeGanar[i, j].X].BackColor = Color.Red;//La resaltamos
                        }
                        return; //Como solo una forma de ganar se aplica a cada juego y ya encontramos la que gano, no hace falta seguir explorando las demas
                    }
                    else if (casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == 'O') 
                    {
                        for (int j = 0; j < 3; j++) //Para cada casilla en la forma de ganar.
                        {
                            botones[formasDeGanar[i, j].Y, formasDeGanar[i, j].X].BackColor = Color.Green;//La resaltamos en otro Color.
                        }
                        return;
                    }
                }
            }
            
        }

        public int getCasillasVacias()//Cuenta cuantas casillas estan vacias
        {
            int x = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (estaVacia(j, i)) x++;
                }
            }
            return x;
        }

        public bool hayGanador() //Estos metodos funcionan dependiendo del resultado del metodo anterior.
        {
            int x = quienGana();
            return (x == 1 || x == 2);
        }

        public bool gananX()
        {
            return quienGana() == 1;
        }

        public bool gananO()
        {
            return quienGana() == 2;
        }

        public bool empate()
        {
            return quienGana() == 3;
        }

        public bool juegoEnCurso()
        {
            return quienGana() == 0;
        }        

        public int getPuntos2() //Este metodo puntua el tablero en 3 opciones -1, 0,1 (ganan O, empate o aun no se decide,ganan X respectivamente). Notece q termina en 2
         {
            if (hayGanador()) //Si hay ganador retornaremos -1 o 1
            {
                if (gananO()) return -1; //Para las O es -1
                else return 1; //Para las X es 1
            }
            else
                return 0; //retornamos 0 para lo demas.
        }

        public int getPuntos() //Este es parecido al anterior y al de quien gana y se utiliza para puntuar en el metodo minimax puro.
        {
            int x = 0; //Variable para x
            int o = 0; //variable para o
            int total = 0; //El puntaje total
            for (int i = 0; i < 8; i++) //Por cada forma de ganar
            {
                o=x = 0; //Vamos a contar cuantas casillas estan marcadas por cada quien.
                for (int j = 0; j < 3; j++) //Para cada casilla en la forma de ganar (recordar q son 3 casillas para ganar).
                {
                    if (casillas[formasDeGanar[i, j].Y, formasDeGanar[i, j].X] == 'X') //Si es = a X
                        x++; //Aumentamos el numero x
                    else if (casillas[formasDeGanar[i, j].Y, formasDeGanar[i, j].X] == 'O')//Si es = a O
                        o++; //Aumentamos el numero o
                }
                if (x == 0) //Si no hay ni una sola x en la linea
                {
                    switch (o) //Veamos q posibilidades tienen las o
                    {   //Aqui disminuimos el total (como en el caso anterior el valor para el gane de o era -1).
                        case 1: total -= 1; break; // Si es uno la posibilidad esta abierta a q en el siguiente tiro la x invalide, por lo tanto no le damos mucha importancia.
                        case 2: total -= 4; break; //Cuando son 2... las posibilidades aunmentan (se requiere solo un tiro mas y ganaria).
                        case 3: total -= 32; break; //Cuando son 3... pus ya gano... y como tal... se colocan la maxima (en realidad minima) puntuacion.
                    }
                }
                else if (o == 0) //Si no hay ninguna O
                {
                    switch (x) //Hacemos lo mismo. pero para las X y aumentando enves de disminuir
                    {
                        case 1: total += 1; break;
                        case 2: total += 4; break;
                        case 3: total += 32; break;
                    }
                }//Cuando se mezclan x y o en la misma linea... esa linea no cuenta para el puntaje
            }
            //Hay q mencionar aqui que mientras mas bajo sea el total. significa que las o tienen la ventaja
            //En cambio si el total es alto, son las x las que llevan la de ganar.
            return total;
        }

        public override string ToString()
        {
            String todo = ""; //Este metodo simplemente retorna las casillas en un solo String de una sola linea.
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    todo += casillas[i, j];
                }
            }
            return todo;
        }
    }
}
