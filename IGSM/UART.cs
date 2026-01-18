using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Ports;
using System.Text;
using System.Windows;

namespace IGSM
{
    internal class UART
    {
        private SerialPort serialPort;
        private string COM_Port;
        private Thread readThread;
        private string ID = null;
        private string _prefixbuffer;
        private string _databuffer;

        public UART(string COM_Name)
        {
            this.COM_Port = COM_Name;
            serialPort = new SerialPort(COM_Port, 115200);
        }

        public event Action<string, string> DataReceived;

        private void ReadID()
        {
            while (true)
            {
                serialPort.WriteLine("SUC");

                if (serialPort.BytesToRead>0)
                {
                    ID = serialPort.ReadLine();
                }

                if (ID != null)
                {
                    readThread = new Thread(ReadData);
                    readThread.Start();
                    break;
                }

                Thread.Sleep(500);
            }
        }


        private void ReadData()
        {
            string prefixbuffer = "";
            string databuffer = "";
            bool timeoutreached = false;
            string readdata = "";

            while (serialPort.IsOpen || !timeoutreached)
            {

                try
                {
                    if (serialPort.BytesToRead>0)
                    {

                        readdata = serialPort.ReadLine();

                        for (int i = 0; i<2; i++)
                        {
                            prefixbuffer += readdata[i];
                        }

                        for (int i = 0; i<readdata.Length+2; i++)
                        {
                            if (i+2 >= readdata.Length)
                                break;

                            databuffer += readdata[i+2];
                        }


                        _prefixbuffer = prefixbuffer.ToString();
                        _databuffer = databuffer.ToString();

                        DataReceived.Invoke(prefixbuffer, databuffer);

                        prefixbuffer = "";
                        databuffer = "";
                    }
                }
                catch (TimeoutException)
                {
                    timeoutreached = true;
                }
            }
        }

        public string GetPrefix() { return _prefixbuffer; }

        public string GetData() {  return _databuffer; }

        public string GetID() { return ID; }

        public void Connect()
        {
            try
            {
                serialPort.Open();
                readThread = new Thread(ReadID);
                readThread.Start();
            }
            catch (Exception e)
            {
               MessageBox.Show(e.Message);
            }
        }
    }
}
