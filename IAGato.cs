using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Gato_M_M
{
    class IAGato
    {
        SortedDictionary<String, int> maximos; //Contendra los maximos para diversos estado
        SortedDictionary<String, int> minimos; //Idem al anterior pero para minimos
        int niv; //La profundidad de busqueda
        Random rnd = new Random(); //Variable por si se necesitan algunos numeros aleatorios.

        public IAGato(int nivel) //Se crea la nueva IA, con el nivel pasado por defecto.
        {
            niv = nivel;
            maximos = new SortedDictionary<string, int>();
            minimos = new SortedDictionary<string, int>();
            if (File.Exists("Minimos.txt")) //Si existe el archivo cargamos su contenido.
            {
                String l;
                String[] datos;
                StreamReader sr = new StreamReader("Minimos.txt");
                while ((l = sr.ReadLine()) != null) //Mientras podamos leer una linea
                {
                    datos = l.Split(':'); //Separamos en 2 la linea mediante un :
                    if (datos.Length != 2) continue;//Si no son dos las partes continuamos en la siguiente
                    minimos.Add(datos[0], int.Parse(datos[1])); //Añadimos un minimo para el estado y el dato.
                }
                sr.Close();//Cerramos el archivo
            }
            if (File.Exists("Maximos.txt")) //Este proceso es identico al anterior solo que para maximos
            {
                String l;
                String[] datos;
                StreamReader sr = new StreamReader("Maximos.txt");
                while ((l = sr.ReadLine()) != null)
                {
                    datos = l.Split(':');
                    if (datos.Length != 2) continue;
                    maximos.Add(datos[0], int.Parse(datos[1]));
                }
                sr.Close();
            }
        }
        public void guardar() //Metodo que guarda las listas.
        {
            StreamWriter sw = new StreamWriter("Minimos.txt");//Se crea un "escritor"
            foreach (String edo in minimos.Keys) //Para cada clave(estado)
            {
                sw.WriteLine(edo + ":" + minimos[edo]);//Escribimos la clave y el valor. separado por :
            }
            sw.Flush();//Un flush para obligar a escribir
            sw.Close(); //Un close para cerrar el archivo
            sw = new StreamWriter("Maximos.txt");//Mismo procedimiento pero ahora para maximos.
            foreach (String edo in maximos.Keys)
            {
                sw.WriteLine(edo + ":" + maximos[edo]);
            }
            sw.Flush();
            sw.Close();
        }
        public void Tirar(Gato g, Button[,] botones) //Una sobrecarga para el metodo de abajo donde la profundidad es la default
        {
            Tirar(g, botones, niv);
        }
        public void Tirar(Gato g, Button[,] botones,int nivel) //Este se encarga de tirar de acuerdo al minimax Puro...
        {
            if (!g.juegoEnCurso()) return;//Si ya no se puede jugar (alguien gano o esta en empate) se retorna
            if (g.esTurnoO()) //Si les toca tirar a las O
            {
                int f = 0, c = 0; //Algo q contendra la coordenada de la mejor tirada
                int v = int.MaxValue; //Como en este caso tratamos de minimizar el resultado, empezamos con un nivel muy alto.
                int aux;
                for (int i = 0; i < 3; i++) //Estos for son para recorrer todas las casillas
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i)) //si la casilla analiza esta vacia.
                        {
                            g.tirar(j, i, 'O'); //tiramos en ella
                            aux = aplicarMaximo(g, nivel); //Sacamos el maximo  
                            if (aux < v) // Si el resultado anterior es menor q v (Minimo de maximos)
                            {
                                v = aux; //Guardamos el nuevo nimimo
                                f = i; //Guardamos las coordenadas
                                c = j;
                            }
                            g.borrar(j, i); //Borramos el movimiento.
                        }
                    }
                }
                g.tirar(c, f, 'O'); //Ok en este punto tiramos la "mejor" opcion
                botones[f, c].Text = "O"; //Actualizamos la interfaz
            }
            else //Si no tiran las X el procedimiento es parecido, solo comentare las diferencias.
            {
                int f = 0, c = 0;
                int v = int.MinValue; //En este caso se trata de maximizar el resultado... por lo tanto empieza con un nivel muy bajo.
                int aux;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i))
                        {
                            g.tirar(j, i, 'X'); //Ahora tiramos con una X
                            aux = aplicarMinimo(g, nivel); //Analizamos el minimo .
                            if (aux > v) // (Maximo de minimos)
                            {
                                v = aux;
                                f = i;
                                c = j;
                            }
                            g.borrar(j, i);
                        }
                    }
                }
                g.tirar(c, f, 'X'); //Ahora tiramos con X en la mejor opcion.
                botones[f, c].Text = "X";
            }
        }

        private int aplicarMinimo(Gato g, int niv) //Este es el metodo minimo de MINIMAX Puro. separece auna parte del metodo de arriba y solo comentare algunas cosas...
        {
            if (!g.juegoEnCurso() || niv == 0) //Si se acabo el juego o el niv enviado es=0
            {
                return g.getPuntos(); //Retornamos la puntuacion estatica para tal juego.
            }
            else
            {
                int v = int.MaxValue; // Se trata de minimizar por lo tanto empezamos con un nivel alto.
                int aux;
                for (int i = 0; i < 3; i++) 
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i)) 
                        {
                            g.tirar(j, i, 'O');
                            aux = aplicarMaximo(g, niv - 1); //Se disminulle el nivel. para poder asi limitar la profundidad de exploracion.
                            if (aux<v) v = aux; //Aqui no es necesario guardar las posiciones... puesto q no es el metodo que tira.
                            g.borrar(j, i); //Se borra el movimiento echo... por q pues no queremos q se vea el movimiento.
                        }
                    }
                }
                return v; //Se retorna el valor minimo (Minimo de maximos).
            }
        }

        private int aplicarMaximo(Gato g, int niv) //Este metodo es parecedido y a la vez contrario al anterior... indicare donde esta la diferencia.
        {
            if (!g.juegoEnCurso() || niv == 0)
            {
                return g.getPuntos();
            }
            else
            {
                int v = int.MinValue; //Como se va ha maximizar se comienza con un minimo muy bajo
                int aux;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i))
                        {
                            g.tirar(j, i, 'X'); //El tiro es inverso al del aplicarMinimo
                            aux = aplicarMinimo(g, niv - 1); //Se saca un minimo
                            if (aux>v) v = aux; //Maximo de minimos
                            g.borrar(j, i);
                        }
                    }
                }
                return v;
            }
        }
        
        //-----------------Tratando de aprender------------------------------
        //Esta parte contiene tres metodos casi identicos a los anteriores...
        //Solo cambian los nombres por q estos terminan en 2, ademas de que se supone q con esto el sistema va aprendiendo.
        //Solo comentare las direfencias.
        public void Tirar2(Gato g, Button[,] botones) //Una sobrecarga para el metodo de abajo donde la profundidad es la default
        {
            Tirar2(g, botones, niv);
        }
        public void Tirar2(Gato g, Button[,] botones,int nivel)
        {
            if (!g.juegoEnCurso()) return;
            if (g.esTurnoO()) 
            {
                int f = 0, c = 0; 
                
                int v = int.MaxValue;
                int aux;
                for (int i = 0; i < 3; i++) 
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i))
                        {
                            g.tirar(j, i, 'O'); 
                            aux = aplicarMaximo2(g, nivel); //Sacamos el maximo  notese el 2 al final
                            if (aux < v) // Si el resultado anterior es menor q v (Minimo de maximos)
                            {
                                v = aux; 
                                f = i;
                                c = j;
                            }
                            g.borrar(j, i); 
                        }
                    }
                }
                g.tirar(c, f, 'O'); 
                botones[f, c].Text = "O"; 
            }
            else //tiran las X
            {
                int f = 0, c = 0;
                
                //-------------------------------------------------------------------------------------
                //Esto se señalo entre los -------- es para que no siempre tire al iniciar en la casilla 1
                
                if (g.getCasillasVacias() == 9) //Si es el primer movimiento
                {
                    
                    f = rnd.Next(3);
                    c = rnd.Next(3);
                    if (f >= 0 && f < 3 && c >= 0&& c < 3)
                    {
                        g.tirar(c, f, 'X');
                        botones[f, c].Text = "X";
                        Console.WriteLine("Tirando al azar "+c+" "+f);
                        return;
                    }
                    
                }
                //--------------------------------------------------------------------------------------
                int v = int.MinValue; 
                int aux;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i))
                        {
                            g.tirar(j, i, 'X'); //Ahora tiramos con una X
                            aux = aplicarMinimo2(g, nivel);
                            if (aux > v) // (Maximo de minimos)
                            {
                                v = aux;
                                f = i;
                                c = j;
                            }
                            g.borrar(j, i);
                        }
                    }
                }
                g.tirar(c, f, 'X'); //Ahora tiramos con X en la mejor opcion.
                botones[f, c].Text = "X";
            }
        }

        private int aplicarMinimo2(Gato g, int niv) //Metodo min termina en 2....
        {
            if (minimos.ContainsKey(g.ToString()))//Si ya tenemos guardado un minimo para este estado... simplemente lo retornamos.
            {
                return minimos[g.ToString()];
            }
            if (!g.juegoEnCurso() || niv == 0) 
            {
                int x = g.getPuntos2(); //Notese que este metodo termina en 2... este regresara -1, 0 ,1.
                if (!g.juegoEnCurso())//Si ya termino el juego podemos agregar tranquilamente el minimo a la coleccion
                { //Dado que no habra otro valor posibe para este estado mas que ese
                    minimos.Add(g.ToString(), x);
                }
                return x; 
            }
            else
            {
                int v = int.MaxValue; // Se trata de minimizar por lo tanto empezamos con un nivel alto.
                int aux;
                bool full=true; //Si todos los estados del tablero siguientes ha este ya tienen un maximo resultado, el resultado de este no cambiara asi que lo podemos guardar en nuestra coleccion.
                for (int i = 0; i < 3; i++) 
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i)) 
                        {
                            g.tirar(j, i, 'O');
                            if (maximos.ContainsKey(g.ToString())) //Si ya esta dentro de maximos el resultado 
                            {
                                aux = maximos[g.ToString()];//Simplemente optenemos el resultado anterior.
                            }
                            else
                            {
                                aux = aplicarMaximo2(g, niv - 1); //Se busca el minimo (notese el 2 al final.
                                //Ok... si depues de optener su maximo aun no se ha terminado de explorar (no esta en maximos)
                                if (!maximos.ContainsKey(g.ToString()))  //Esta linea se puede borrar y seguira funcionando el programa solo q el aprendizaje ser "mas lento"
                                    full = false; //Seteamos el full a false para indicar que aun no se ha encontrado el valor final para este estado.
                            }
                            if (v > aux) v = aux; //Aqui no es necesario guardar las posiciones... puesto q no es el metodo que tira.
                            g.borrar(j, i); //Se borra el movimiento echo... por q pues no queremos q se vea el movimiento.
                        }
                    }
                }
                if (full) //Si ya esta completamente explorado podemos guardar su valor para futuras referencias (aprender)
                {
                    minimos.Add(g.ToString(), v);
                }
                return v; //Se retorna el valor minimo (Minimo de maximos).
            }
        }

        private int aplicarMaximo2(Gato g, int niv) //Este metodo es parecedido y a la vez contrario al anterior... indicare donde esta la diferencia.
        {
            //Las operaciones que en el anterior se hacian sobre el diccionario de minimos ahora se haran sobre el de maximos y viceversa.
            if(maximos.ContainsKey (g.ToString ()))
            {
                return maximos[g.ToString()];
            }
            if (!g.juegoEnCurso() || niv == 0)
            {
                int x = g.getPuntos2();
                if (!g.juegoEnCurso())
                {
                    maximos.Add(g.ToString(), x);
                }
                return x;
            }
            else
            {
                int v = int.MinValue; //Como se va ha maximizar se comienza con un minimo muy bajo
                int aux;
                bool full = true;//Para lo mismo que en el anterior.
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i))
                        {
                            g.tirar(j, i, 'X');
                            
                            if (minimos.ContainsKey(g.ToString()))
                            {
                                aux = minimos[g.ToString()];
                            }
                            
                            else
                            {
                                aux = aplicarMinimo2(g, niv - 1);
                                if (!minimos.ContainsKey(g.ToString()))//Se puede borrar esta linea, pues aunque mejora el aprendizare realiza una busqueda adicional.idem en el metodo anterior.
                                    full = false;
                            }

                            if (v < aux) v = aux; //Maximo de minimos
                            g.borrar(j, i);
                        }
                    }
                }
                if (full)
                {
                    maximos.Add(g.ToString(), v);
                }
                return v;
            }
        }
    }
    /**
     * Ok esta es la clase de Inteligencia basada en el algorimo minimax, que si bien ese algoritmo se 
     * basa en la exploracion de los estados del juego subsiguientes por medio de una puntuacion
     * que es ventajosa o desventajosa... para uno o para otro.
     * La serie de metodos que aprenden en realidad solo se basan en que las busquedas que se haga sobre consultas ya hechas
     * por ello que guardamos los resultado en los dos diccionarios ordenados (lo de ordenados es para agilizar las busquedas en ellos).
       * */
}
