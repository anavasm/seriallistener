using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SerialListener
{
    public partial class SerialListener : ServiceBase
    {
        SerialPort sp;
        //log
        static EventLog eventLog1;

        public SerialListener()
        {
            InitializeComponent();
            eventLog1 = new EventLog();
            this.AutoLog = false;
            // create an event source, specifying the name of a log that 
            // does not currently exist to create a new, custom log 
            if (!System.Diagnostics.EventLog.SourceExists("SerialListener"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "SerialListener", "SerialListenerService");
            }
            // configure the event log instance to use this source name
            eventLog1.Source = "SerialListener";
        }

        protected override void OnStart(string[] args)
        {
            //Abre conexión conexión puerto serie
            try
            {
                sp = new SerialPort("COM2");
                sp.BaudRate = 9600;
                sp.Parity = Parity.None;
                sp.StopBits = StopBits.One;
                sp.DataBits = 8;
                sp.Handshake = Handshake.None;

                //agrega evento de escucha al puerto serie
                sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                //abre conexión con el puerto serie
                sp.Open();
                eventLog1.WriteEntry("SerialListener se ha iniciado correctamente", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Error producido en el servicio SerialListener: " + ex.Message, EventLogEntryType.Error);
                this.Stop();
            }
            
        }

        protected override void OnStop()
        {
            sp.Close();
            eventLog1.WriteEntry("Servicio SerialListener detenido correctamente", EventLogEntryType.Information);
        }

        /// <summary>
        /// Método que recibe datos por el puerto serie
        /// </summary>
        /// <param name="sender">Objeto que lanza el evento (SerialPort)</param>
        /// <param name="e">Argumentos del evento</param>
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            
            SerialPort sPort = (SerialPort)sender;           
            //enviar datos a servidor UDP 5557
            UdpClient udpClient = new UdpClient(11000);
            try
            {
                //conectamos con el servidor UDP, puerto 5557
                udpClient.Connect("127.0.0.1", 5557);
                //leemos los datos recibidos por el puerto serie
                int bytes = sPort.BytesToRead;
                byte[] byteBuffer = new byte[bytes];
                sPort.Read(byteBuffer, 0, bytes);
                //enviamos los datos por UDP
                udpClient.Send(byteBuffer, byteBuffer.Length);
                //cerramos conexión con el servidor UDP
                udpClient.Close();

            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Error producido en el servicio SerialListener: " + ex.Message, EventLogEntryType.Error);
            }

        }
    }
}
