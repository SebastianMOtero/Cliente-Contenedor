using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTERO___POO___TPR2
{
    public abstract class Contenedor
    {
        List<Cliente> ListaCliente;

        #region "Contructores"
        public Contenedor()
        {
            ListaCliente = new List<Cliente>();
        }

        public Contenedor(string pId, string pDescripcion, DateTime pFechaIngreso, decimal pPeso) : this()
        {
            Id = pId;
            Descripcion = pDescripcion;
            FechaIngreso = pFechaIngreso;
            Peso = pPeso;
        }
        #endregion

        #region "Propiedades"
        public Cliente[] Clientes { get { return ListaCliente.ToArray(); } }
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaIngreso { get; set; }
        public virtual decimal Peso { get; set; }
        #endregion

        #region "Eventos"
        public event EventHandler<EventoContenedorPesadoEventsArgs> EventoContenedorPesado;
        #endregion

        #region "Métodos"
        public void ComprobarPesoMayor4000()
        {
            if (Peso > 4000)
            {
                //Desencadena evento
                EventoContenedorPesado(this, new EventoContenedorPesadoEventsArgs(Id, Descripcion, Peso, Clientes[0].Legajo, Clientes[0].Nombre, Clientes[0].Apellido, Clientes[0].RazonSocial));
            }

        }

        public void VincularCliente(Cliente pCliente)
        {
            ListaCliente.Add(pCliente);
        }
        #endregion
    }

    public class ContenedorA : Contenedor
    {
        #region "Constructores"
        public ContenedorA(string pId, string pDescripcion, DateTime pFechaIngreso, decimal pPeso) : base(pId, pDescripcion, pFechaIngreso, pPeso) { }
        #endregion
    }

    public class ContenedorB : Contenedor
    {
        #region "Constructores"
        public ContenedorB(string pId, string pDescripcion, DateTime pFechaIngreso, decimal pPeso) : base(pId, pDescripcion, pFechaIngreso, pPeso) { }
        #endregion

        #region "Propiedad"
        public override decimal Peso
        {
            get
            {
                return base.Peso;
            }
            set
            {
                base.Peso = value*11/10;
            }
        }
        #endregion
    }
}
