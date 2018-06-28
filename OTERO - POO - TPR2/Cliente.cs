using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace OTERO___POO___TPR2
{
    public abstract class Cliente
    {
        List<Contenedor> ListaContenedor;

        #region "Contructores"
        public Cliente()
        {
            ListaContenedor = new List<Contenedor>();
        }
        public Cliente(string pLegajo) : this()
        {
            Legajo = pLegajo;   
        }
        #endregion

        #region "Propiedades"
        public Contenedor[] Contenedores { get { return ListaContenedor.ToArray(); } }
        public string Legajo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string RazonSocial { get; set; }
        #endregion

        #region "Metodos"
        public void QuitarContenedor(Contenedor pContenedor)
        {
            try
            {
                if (ListaContenedor.Count() > 0)
                    ListaContenedor.Remove(pContenedor);
            }
            catch (Exception)
            {
            }
        }

        public void VincularContenedor(Contenedor pContenedor)
        {
            ListaContenedor.Add(pContenedor);
        }
        #endregion
    }

    public class Persona : Cliente
    {
        #region "Constructor"
        public Persona(string pLegajo, string pNombre, string pApellido) : base(pLegajo)
        {
            Nombre = pNombre;
            Apellido = pApellido;
        }
        #endregion
    }

    public class Empresa : Cliente
    {
        #region "Constructor"
        public Empresa(string pLegajo, string pRazonSocial) : base(pLegajo)
        {
            RazonSocial = pRazonSocial;
        }
        #endregion
    }
}
