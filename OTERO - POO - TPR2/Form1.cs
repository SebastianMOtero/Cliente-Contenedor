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
using System.Runtime.InteropServices;

namespace OTERO___POO___TPR2
{
    public partial class Form1 : Form
    {
        #region "Constructor"
        public Form1()
        {
            InitializeComponent();
            pcbSalirActivo.Visible = false;
            pcbSalir.Visible = false;
        }
        #endregion

        #region "Mover ventana con la barra"
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lpParam);
        #endregion

        public Administrador A;
        
        #region "Eventos"
        //Evento personalizado para peso mayor a 4000 kg del contenedor
        private void FuncionEventoContenedorPesado(object sender, EventoContenedorPesadoEventsArgs e)
        {
            try
            {
                string datosContenedor = "     ID: " + e.Id + "\n     Descripcion: " + e.Descripcion + "\n     Peso: " + e.Peso;
                string datosCliente;
                if (e.RazonSocial == null)
                {
                    datosCliente = "     Nombre: " + e.Nombre + "\n     Apellido: " + e.Apellido;
                }
                else
                {
                    datosCliente = "     Razon Social: " + e.RazonSocial;
                }

                MessageBox.Show("Contenedor\n" + datosContenedor + "\n\nCliente\n" + datosCliente,"Peso superior a 4000 Kg!",MessageBoxButtons.OK);

            }
            catch (Exception)
            {

            }
        }

        //Carga del form1
        private void Form1_Load(object sender, EventArgs e)
        {
            A = new Administrador();
            cmbTipoCliente.SelectedIndex = 0;
            NormalizarPanel();
        }

        //Evento para mover la ventana
        private void pBarraHeader_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        //Evento para detectar fecha incorrecta
        private void dtpFechaIngreso_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFechaIngreso.Value > DateTime.Today)
            {
                MessageBox.Show("No se puede ingresar una fecha futura","Error con la fecha seleccionada");
                dtpFechaIngreso.Value = DateTime.Today;
            }

        }
        #endregion

        public void LimpiarTxt()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtRazonSocial.Clear();
            txtLegajo.Clear();

            txtDescripcion.Clear();
            txtIdLetras.Clear();
            txtIdNum.Clear();
            txtPeso.Clear();
        }

        private void NormalizarPanel()
        {
            pAgregarModificarContenedor.Visible = false;
            pAgregarModificadorCliente.Visible = false;
            pContenedoresDeClientes.Visible = true;
            dgvClientes.Enabled = true;
        }

        private void ActualizarDGV(DataGridView pDGV, object pObject)
        {
            pDGV.DataSource = null;
            pDGV.DataSource = pObject;
        }

        #region "Data Grid View"
        private void dgvClientes_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Cliente Cli = ((ClienteVisto)(dgvClientes.SelectedRows[0].DataBoundItem)).Origen();
                ActualizarDGV(dgvContenedoresDeCliente, Cli.Contenedores);
                txtLegajo.Text = Cli.Legajo;
                if (Cli is Persona)
                {
                    lblDGVContenedoresDeClientes.Text = "Contenedores de " + Cli.Nombre + " " + Cli.Apellido;
                    cmbTipoCliente.SelectedIndex = 0;
                    txtNombre.Text = Cli.Nombre;
                    txtApellido.Text = Cli.Apellido;
                }   
                else
                {
                    lblDGVContenedoresDeClientes.Text = "Contenedores de " + Cli.RazonSocial;
                    cmbTipoCliente.SelectedIndex = 1;
                    txtRazonSocial.Text = Cli.RazonSocial;
                }     
            }
            catch (Exception)
            {

            }
        }

        private void dgvContenedores_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Contenedor C = ((Contenedor)(dgvContenedores.SelectedRows[0].DataBoundItem));
                string[] vectorId = C.Id.Split('-');
                txtIdLetras.Text = vectorId[1];
                txtIdNum.Text = vectorId[0];
                txtDescripcion.Text = C.Descripcion;
                txtPeso.Text = C.Peso.ToString();
                dtpFechaIngreso.Value = C.FechaIngreso;

                if (C is ContenedorA)
                    cmbTipoContenedor.SelectedIndex = 0;
                else
                    cmbTipoContenedor.SelectedIndex = 1;
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region "Botones de paneles secundarios Cliente y Contenedor interfaz"
        //Crea cliente con los datos ingresados
        private void btnAgregarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                Cliente C;

                //Dependiendo de la seleccion del combo, crea una persona o empresa
                if (cmbTipoCliente.SelectedItem.ToString() == "Persona")
                {
                    C = new Persona(txtLegajo.Text, txtNombre.Text, txtApellido.Text);
                }
                else
                {
                    C = new Empresa(txtLegajo.Text, txtRazonSocial.Text);
                }

                //Agregar Persona
                A.AgregarCliente(C);

                //Comprueba si se agrego el cliente, para proceder con actualizar dgv y normalizar interfaz
                if (A.Cliente.Contains(C))
                {
                    ActualizarDGV(dgvClientes, ClienteVisto.RetornaClientesVisto(A.Cliente));
                    NormalizarPanel();
                    LimpiarTxt();
                    dgvClientes.Enabled = true;
                }
            }
            catch (Exception)
            {
            }
        }

        //Segun seleccion del combobox muestra textbox de empresa o de persona
        private void cmbTipoCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTipoCliente.SelectedIndex == 0)
            {
                txtNombre.Visible = true;
                txtApellido.Visible = true;
                txtRazonSocial.Visible = false;
                lblNombre.Visible = true;
                lblApellido.Visible = true;
                lblRazonSocial.Visible = false;
            }
            else
            {
                txtNombre.Visible = false;
                txtApellido.Visible = false;
                txtRazonSocial.Visible = true;
                lblNombre.Visible = false;
                lblApellido.Visible = false;
                lblRazonSocial.Visible = true;
            }
        }

        //Modifica cliente con los datos ingresados
        private void btnModificarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                //Obtiene el cliente seleccionado en la DGV
                Cliente C = ((ClienteVisto)(dgvClientes.SelectedRows[0].DataBoundItem)).Origen();

                //Crea un objeto Z y copia los valores a ingresar en C
                Cliente Z;
                if (C is Persona)
                    Z = new Persona(txtLegajo.Text, txtNombre.Text, txtApellido.Text);
                else
                    Z = new Empresa(txtLegajo.Text, txtRazonSocial.Text);

                //Comprueba si los valores a ingresar en Z cumplen formato
                if (A.ControlarNombreRazon(Z) != null)
                    return;
                
                //Si cumplen, agregar los nuevos valores al objeto
                if (C is Persona)
                {
                    C.Nombre = txtNombre.Text;
                    C.Apellido = txtApellido.Text;
                }
                else
                    C.RazonSocial = txtRazonSocial.Text;

                //Comprueba si se agrego el cliente, para proceder con actualizar dgv y normalizar interfaz
                if (A.Cliente.Contains(C))
                {
                    ActualizarDGV(dgvClientes, ClienteVisto.RetornaClientesVisto(A.Cliente));
                    NormalizarPanel();
                    LimpiarTxt();
                }
            }
            catch (Exception)
            {
            }
        }

        //Cancelar agregar/modificar cliente
        private void btnCancelarCliente_Click(object sender, EventArgs e)
        {
            NormalizarPanel();
        }

        //Crea contenedor con los datos ingresados
        private void btnAgregarContenedor_Click(object sender, EventArgs e)
        {
            try
            {
                Contenedor C;
                string id = txtIdNum.Text + "-" + txtIdLetras.Text;
                if (cmbTipoContenedor.SelectedItem.ToString() == "A")
                {
                    C = new ContenedorA(id, txtDescripcion.Text, Convert.ToDateTime(dtpFechaIngreso.Value), Convert.ToDecimal(txtPeso.Text));
                }
                else
                {
                    C = new ContenedorB(id, txtDescripcion.Text, Convert.ToDateTime(dtpFechaIngreso.Value), Convert.ToDecimal(txtPeso.Text));
                }

                //Agregar contenedor
                A.AgregarContenedor(C);

                //Comprueba si se agrego el contenedor, para proceder con vinculacion, evento, actualizar dgv y normalizar interfaz
                if (A.Contenedor.Contains(C))
                {
                    //Realiza la vinculacion entre contenedor y cliente
                    Cliente Cli = ((ClienteVisto)(dgvClientes.SelectedRows[0].DataBoundItem)).Origen();
                    Cli.VincularContenedor(C);
                    C.VincularCliente(Cli);

                    //Suscripcion evento
                    C.EventoContenedorPesado += FuncionEventoContenedorPesado;

                    //Comprobar peso
                    C.ComprobarPesoMayor4000();

                    ActualizarDGV(dgvContenedores, A.Contenedor);
                    NormalizarPanel();
                    LimpiarTxt();
                    //dgvClientes.Enabled = true;
                }
            }
            catch (Exception)
            {
            }
        }

        //Modifica contenedor con los datos ingresados
        private void btnModificarContenedor_Click(object sender, EventArgs e)
        {
            try
            {
                Contenedor C = ((Contenedor)(dgvContenedores.SelectedRows[0].DataBoundItem));

                C.Descripcion = txtDescripcion.Text;
                C.FechaIngreso = Convert.ToDateTime(dtpFechaIngreso.Text);
                C.Peso = Convert.ToDecimal(txtPeso.Text);

                ActualizarDGV(dgvContenedores, A.Contenedor);

                C.ComprobarPesoMayor4000();
                ActualizarDGV(dgvContenedores, A.Contenedor);

                NormalizarPanel();
            }
            catch (Exception)
            {
            }
        }

        //Cancelar agregar/modificar contenedor
        private void btnCancelarContenedor_Click(object sender, EventArgs e)
        {
            NormalizarPanel();
        }
        #endregion

        #region "Paneles y botones del menu interfaz"
        //Abre el panel para agregar contenedor
        private void btnAbrirAgregarContenedor_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                cmbTipoContenedor.SelectedIndex = 0;
                pAgregarModificarContenedor.Visible = true;
                pAgregarModificadorCliente.Visible = false;
                pContenedoresDeClientes.Visible = false;
                btnAgregarContenedor.Visible = true;
                btnModificarContenedor.Visible = false;
                txtIdNum.Enabled = true;
                txtIdLetras.Enabled = true;
                cmbTipoContenedor.Enabled = true;
                lblAgregarContenedor.Text = "Agregar contenedor";
                lblAgregarContenedor.Visible = true;
                lblModificarContenedor.Visible = false;
            }
            else
                MessageBox.Show("Seleccione un cliente de la lista para agregar un nuevo contenedor", "Sin seleccion");
        }

        //Abre el panel para modificar contenedor
        private void btnAbrirModificarContenedor_Click(object sender, EventArgs e)
        {
            if (dgvContenedores.SelectedRows.Count > 0)
            {
                pAgregarModificarContenedor.Visible = true;
                pAgregarModificadorCliente.Visible = false;
                pContenedoresDeClientes.Visible = false;
                btnAgregarContenedor.Visible = false;
                btnModificarContenedor.Visible = true;
                txtIdNum.Enabled = false;
                txtIdLetras.Enabled = false;
                cmbTipoContenedor.Enabled = false;
                lblAgregarContenedor.Text = "Modificar contenedor";
                lblAgregarContenedor.Visible = false;
                lblModificarContenedor.Visible = true;
            }
            else
                MessageBox.Show("Seleccione un contenedor de la lista para modificarlo", "Sin seleccion");
        }

        //Borrar contenedor seleccionado
        private void btnBorrarContenedor_Click(object sender, EventArgs e)
        {
            try
            {
                Contenedor C = ((Contenedor)(dgvContenedores.SelectedRows[0].DataBoundItem));

                A.BorrarContenedor(C);

                if (A.Contenedor.Count() == 0)
                {
                    dgvContenedores.DataSource = null;
                }
                else
                {
                    ActualizarDGV(dgvContenedores, A.Contenedor); 
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Seleccione un contenedor de la lista para eliminarlo", "Sin seleccion");
            }
        }

        //Abrir el panel para agregar cliente
        private void btnAbrirAgregarCliente_Click(object sender, EventArgs e)
        {
            LimpiarTxt();
            cmbTipoCliente.SelectedIndex = 0;
            pAgregarModificarContenedor.Visible = false;
            pAgregarModificadorCliente.Visible = true;
            pContenedoresDeClientes.Visible = false;
            btnAgregarCliente.Visible = true;
            btnModificarCliente.Visible = false;
            cmbTipoCliente.Enabled = true;
            lblAgregarCliente.Visible = true;
            lblModificarCliente.Visible = false;
            txtLegajo.Enabled = true;
        }

        //Abrir el panel para modificar cliente
        private void btnAbrirModificarCliente_Click(object sender, EventArgs e)
        {
            //Activa el panel si hay una fila seleccionada
            if (dgvClientes.SelectedRows.Count > 0)
            {
                LimpiarTxt();
                pAgregarModificarContenedor.Visible = false;
                pAgregarModificadorCliente.Visible = true;
                pContenedoresDeClientes.Visible = false;
                btnAgregarCliente.Visible = false;
                btnModificarCliente.Visible = true;
                cmbTipoCliente.Enabled = false;
                lblAgregarCliente.Visible = false;
                lblModificarCliente.Visible = true;
                txtLegajo.Enabled = false;
            }
            else
                MessageBox.Show("Seleccione un usuario de la lista para modificarlo", "Sin seleccion");
        }

        //Borrar cliente seleccionado
        private void btnBorrarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                Cliente C = ((ClienteVisto)(dgvClientes.SelectedRows[0].DataBoundItem)).Origen();

                A.BorrarCliente(C);

                if (A.Cliente.Count() == 0)
                {
                    dgvClientes.DataSource = null;
                    dgvContenedores.DataSource = null;
                }
                else
                {
                    ActualizarDGV(dgvClientes, ClienteVisto.RetornaClientesVisto(A.Cliente));
                    ActualizarDGV(dgvContenedores, A.Contenedor);
                }
                LimpiarTxt();
            }
            catch (Exception)
            {
                MessageBox.Show("Seleccione un usuario de la lista para eliminarlo", "Sin seleccion");
            }
        }

        //Boton para salir del programa
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Boton de la barra superior para salir del programa
        private void pcbSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region "Efectos de los botones"
        private void btnAbrirAgregarContenedor_MouseEnter(object sender, EventArgs e)
        {
            this.btnAbrirAgregarContenedor.BackColor = Color.FromArgb(42, 191, 159);
        }

        private void btnAbrirModificarContenedor_MouseEnter(object sender, EventArgs e)
        {
            this.btnAbrirModificarContenedor.BackColor = Color.FromArgb(42, 191, 159);
        }

        private void btnBorrarContenedor_MouseEnter(object sender, EventArgs e)
        {
            this.btnBorrarContenedor.BackColor = Color.FromArgb(42, 191, 159);
        }

        private void btnAbrirAgregarCliente_MouseEnter(object sender, EventArgs e)
        {
            this.btnAbrirAgregarCliente.BackColor = Color.FromArgb(42, 191, 159);
        }

        private void btnAbrirModificarCliente_MouseEnter(object sender, EventArgs e)
        {
            this.btnAbrirModificarCliente.BackColor = Color.FromArgb(42, 191, 159);
        }

        private void btnBorrarCliente_MouseEnter(object sender, EventArgs e)
        {
            this.btnBorrarCliente.BackColor = Color.FromArgb(42, 191, 159);
        }

        private void btnSalir_MouseEnter(object sender, EventArgs e)
        {
            this.btnSalir.BackColor = Color.FromArgb(42, 191, 159);
        }

        private void btnAbrirAgregarContenedor_MouseLeave(object sender, EventArgs e)
        {
            this.btnAbrirAgregarContenedor.BackColor = Color.FromArgb(129, 134, 140);
        }

        private void btnAbrirModificarContenedor_MouseLeave(object sender, EventArgs e)
        {
            this.btnAbrirModificarContenedor.BackColor = Color.FromArgb(129, 134, 140);
        }

        private void btnBorrarContenedor_MouseLeave(object sender, EventArgs e)
        {
            this.btnBorrarContenedor.BackColor = Color.FromArgb(129, 134, 140);
        }

        private void btnAbrirAgregarCliente_MouseLeave(object sender, EventArgs e)
        {
            this.btnAbrirAgregarCliente.BackColor = Color.FromArgb(129, 134, 140);
        }

        private void btnAbrirModificarCliente_MouseLeave(object sender, EventArgs e)
        {
            this.btnAbrirModificarCliente.BackColor = Color.FromArgb(129, 134, 140);
        }

        private void btnBorrarCliente_MouseLeave(object sender, EventArgs e)
        {
            this.btnBorrarCliente.BackColor = Color.FromArgb(129, 134, 140);
        }

        private void btnSalir_MouseLeave(object sender, EventArgs e)
        {
            this.btnSalir.BackColor = Color.FromArgb(129, 134, 140);
        }

        private void pcbSalir_MouseEnter(object sender, EventArgs e)
        {
            pcbSalirActivo.Visible = true;
            pcbSalir.Visible = false;
        }

        private void pcbSalirActivo_MouseLeave(object sender, EventArgs e)
        {
            pcbSalir.Visible = true;
            pcbSalirActivo.Visible = false;
        }
        #endregion
    }

    public class ClienteVisto
    {
        Cliente ListaCliente;

        #region "Contructores"
        public ClienteVisto(string pLegajo, string pDescripcion, Cliente pClienteOrigen)
        {
            Legajo = pLegajo;
            Descripcion = pDescripcion;
            ListaCliente = pClienteOrigen;
        }
        #endregion

        #region "Propiedades"
        public string Legajo { get; set; }
        public string Descripcion { get; set; }
        #endregion

        #region "Metodos"
        public static ClienteVisto[] RetornaClientesVisto(Cliente[] pClientes)
        {
            List<ClienteVisto> ListaClienteVisto = new List<ClienteVisto>();
            string D;
            foreach (Cliente C in pClientes)
            {

                if (C is Persona)
                {
                    D = C.Nombre + " " + C.Apellido;
                }
                else
                {
                    D = C.RazonSocial;
                }

                ListaClienteVisto.Add(new ClienteVisto(C.Legajo, D, C));
            }
            return ListaClienteVisto.ToArray();
        }

        public Cliente Origen()
        {
            return ListaCliente;
        }
        #endregion
    }

    public class Administrador
    {
        #region "Listas"
        List<Cliente> ListaCliente;
        List<Contenedor> ListaContenedor;
        #endregion

        #region "Constructor"
        public Administrador()
        {
            ListaCliente = new List<Cliente>();
            ListaContenedor = new List<Contenedor>();
        }
        #endregion

        #region "Propiedades"
        public Cliente[] Cliente
        {
            get
            {
                return ListaCliente.ToArray();
            }
        }

        public Contenedor[] Contenedor
        {
            get
            {
                return ListaContenedor.ToArray();
            }
        }
        #endregion

        #region "Métodos
        //Agrega contenedor a la lista
        public void AgregarContenedor(Contenedor pContenedor)
        {
            try
            {
                if (ControlarId(pContenedor) != null)
                    return;

                if (ControlarDescripcion(pContenedor) != null)
                    return;

                if (!ListaContenedor.Exists(x => x.Id == pContenedor.Id))
                    ListaContenedor.Add(pContenedor);
                else
                    MessageBox.Show("Ya existe un contenedor identificado con esa ID", "Error con el contenedor ingresado");
            }
            catch(Exception)
            {

            }
        }

        //Agrega cliente a la lista
        public void AgregarCliente(Cliente pCliente)
        {
            try
            {
                if (ControlarLegajo(pCliente) != null)
                    return;

                if (ControlarNombreRazon(pCliente) != null)
                    return;

                if (!ListaCliente.Exists(x => x.Legajo == pCliente.Legajo))
                    ListaCliente.Add(pCliente);
                else
                    MessageBox.Show("Ya existe un cliente identificado con ese legajo", "Error con el cliente ingresado");
            }
            catch(Exception)
            {
            }
        }

        //Controla legajo
        public object ControlarLegajo(Cliente pCliente)
        {
            int i = 0;

            int.TryParse(pCliente.Legajo, out i);

            if (i < 1 || i > 9999)
                return MessageBox.Show("Ingresar un numero entre 0 y 10000", "Error con el legajo");

            return null;
        }

        //Controla formato del nombre, apellido y razon social
        public object ControlarNombreRazon(Cliente pCliente)
        {
            if (pCliente.RazonSocial == null)
            {
                if (pCliente.Nombre.Length < 3)
                    return MessageBox.Show("No se permiten nombres menores a 3 caracteres", "Error con el nombre");

                if (pCliente.Apellido.Length < 3)
                    return MessageBox.Show("No se permiten apellidos menores a 3 caracteres", "Error con el apellido");
            }
            else
                if (pCliente.RazonSocial.Length < 3)
                    return MessageBox.Show("No se permite una razon social menor a 3 caracteres", "Error con la razon social");
            return null;
        }

        //Controla formato del ID: 999-AAAA
        public object ControlarId(Contenedor pContenedor)
        {
            char[] vector = pContenedor.Id.ToCharArray();

            if (pContenedor.Id.Count() != 8)
                return MessageBox.Show("Respete el formato \"999-AAAA\"", "Error con el ID");

            if (!(vector[3] == '-'))
                return MessageBox.Show("Respete el formato \"999-AAAA\"", "Error con el ID");

            for (int i = 0; i < 3; i++)
            {
                if (!char.IsNumber(vector[i]))
                    return MessageBox.Show("Respete el formato \"999-AAAA\"", "Error con el ID");
            }

            for (int i = 4; i < 8; i++)
            {
                if (!char.IsUpper(vector[i]))
                    return MessageBox.Show("Respete el formato \"999-AAAA\"", "Error con el ID");
            }
            return null;
        }

        //Controla descripcion
        public object ControlarDescripcion(Contenedor pContenedor)
        {
            if (pContenedor.Descripcion.Length < 3)
                return MessageBox.Show("No se permiten descripciones menores a 3 caracteres", "Error con la descripcion");

            return null;
        }

        //Borra contenedor recibido de la lista
        public void BorrarContenedor(Contenedor pContenedor)
        {
            try
            {
                if (ListaContenedor.Count() > 0)
                {
                    ListaContenedor.Remove(pContenedor);
                    foreach (Cliente Cli in ListaCliente)
                    {
                        Cli.QuitarContenedor(pContenedor);
                    }
                }
            }
            catch(Exception)
            {
            }
        }

        //Borra cliente recibido de la lista
        public void BorrarCliente(Cliente pCliente)
        {
            try
            {
                if (ListaCliente.Count() > 0)
                {
                    
                    for (int i = 0; i < ListaContenedor.Count(); i++)
                    {
                        if (ListaContenedor[i].Clientes[0] == pCliente)
                        {
                            ListaContenedor.Remove(ListaContenedor[i]);
                            i--;
                        }
                    }
                    ListaCliente.Remove(pCliente);
                }
            }
            catch(Exception)
            {
            }
        }
        #endregion
    }

    #region "Eventos"
    //Clase del argumentos del evento personalizado
    public class EventoContenedorPesadoEventsArgs : EventArgs
    {
        public EventoContenedorPesadoEventsArgs(string pId, string pDescripcion, decimal pPeso, string pLegajo, string pNombre, string pApellido, string pRazonSocial)
        {
            Id = pId;
            Descripcion = pDescripcion;
            Peso = pPeso.ToString();
            Legajo = pLegajo;
            Nombre = pNombre;
            Apellido = pApellido;
            RazonSocial = pRazonSocial;
        }

        public string Id { get; set; }
        public string Descripcion { get; set; }
        public string Peso { get; set; }
        public string Legajo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string RazonSocial { get; set; }
        //ACA PONER CLASE EVENTO  2
        /*public class SuperiorA1000EventArgs : EventArgs
            {
                public SuperiorA1000EventArgs(decimal pMonto) { Importe = pMonto; }
                public decimal Importe { get; set; }
            }
            */
    }
    #endregion
}