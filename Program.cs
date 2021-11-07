using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace SerialPortExample
{
    public class Program
    {
        static bool _continue;
        static SerialPort _serialPort;
        static void Main(string[] args)
        {
            //Baud Rate 9600 BPS
            //Stop Bits 1
            //Word Length 8 Bits
            //Parity None


            string name;
            string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
          //  Thread readThread = new Thread(Read);

           // Create a new SerialPort object with default settings.
           _serialPort = new SerialPort();


            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            // Allow the user to set the appropriate properties. I can change to my port for arduino as COM3, COM1 is a default port

            // _serialPort.PortName = SetPortName(_serialPort.PortName);
            // _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
            // _serialPort.Parity = SetPortParity(_serialPort.Parity);
            // _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            // _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            // _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);



            _serialPort.PortName = "COM4";
            _serialPort.ReadTimeout = -1;
            _serialPort.WriteTimeout = -1;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            _serialPort.Open();
            //_serialPort.Write("B");


            var isOpen = _serialPort.IsOpen;

            _continue = true;
            //  readThread.Start();

            //'STX' STN 'DC1' 'ETX' BCC   02 31 11 03 47
            //   b) The everywhere used BCC(= Block Check Character) is calculated as sum of all bytes already read

            //  4.Hex - Codes for used control characters:
            //SOH 0x01
            // STX 0x02
            // ETX 0x03
            // ENQ 0x05
            // ACK 0x06
            // DLE 0x10
            // DC1 0x11
            // DC2 0x12
            // DC3 0x13
            // DC4 0x14
            // NAK 0x15
            // CAN 0x18
            // STN 0x31

           // _serialPort.Write("B");

            //I can either use a string
            string _startMachineString = "\x02\x31\x11\x03\x47";
            _serialPort.Write(_startMachineString);

            //encoded bytes
            //byte[] _startMachine = Encoding.GetEncoding("ASCII").GetBytes("DC1");
            byte[] _mass = Encoding.GetEncoding("ASCII").GetBytes("STX STN DC1 ETX BCC");
            _serialPort.Write(_mass, 0, _mass.Length);

            //or this
            var data = new byte[] { 02, 31, 11, 03, 47};
            var dataString = new byte[] { (byte)'D', (byte)'C', (byte)'1' };


            byte[] startMachineHexBytes = new byte[] { 0x02 , 0x31, 0x11, 0x03, 0x47 };
           _serialPort.Write(startMachineHexBytes, 0, startMachineHexBytes.Length);

            var command = 0x02;
            var change_screen = BitConverter.GetBytes(command);

            //sp.Write(change_screen, 0, change_screen.Length);

            //while (_continue)
            //{
            //    message = Console.ReadLine();

            //    if (stringComparer.Equals("quit", message))
            //    {
            //        _continue = false;
            //    }
            //    else
            //    {
            //        if (stringComparer.Equals("A", message))
            //        {
            //            _serialPort.Write("A");
            //            Console.WriteLine("A presses");
            //        }
            //        else
            //        {
            //            _serialPort.Write("B");
            //            Console.WriteLine("B presses");
            //        }
            //    }
            //}

            // readThread.Join();
            _serialPort.Close();
        }
        private static void port_DataReceived(object sender,
  SerialDataReceivedEventArgs e)
        {
            // Show all the incoming data in the port's buffer
 
            var dataReceived = _serialPort.ReadExisting();
            Console.WriteLine(dataReceived);
        }


        //public static void Read()
        //{
        //    while (_continue)
        //    {
        //        try
        //        {
        //            string message = _serialPort.ReadLine();
        //            Console.WriteLine(message);
        //        }
        //        catch (TimeoutException) { }
        //    }
        //}

        public static string SetPortName(string defaultPortName)
        {
            string portName;

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("COM port({0}): ", defaultPortName);
            portName = Console.ReadLine();

            if (portName == "")
            {
                portName = defaultPortName;
            }
            return portName;
        }

        public static int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate;

            Console.Write("Baud Rate({0}): ", defaultPortBaudRate);
            baudRate = Console.ReadLine();

            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            return int.Parse(baudRate);
        }

        public static Parity SetPortParity(Parity defaultPortParity)
        {
            string parity;

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Parity({0}):", defaultPortParity.ToString());
            parity = Console.ReadLine();

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity);
        }

        public static int SetPortDataBits(int defaultPortDataBits)
        {
            string dataBits;

            Console.Write("Data Bits({0}): ", defaultPortDataBits);
            dataBits = Console.ReadLine();

            if (dataBits == "")
            {
                dataBits = defaultPortDataBits.ToString();
            }

            return int.Parse(dataBits);
        }

        public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string stopBits;

            Console.WriteLine("Available Stop Bits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Stop Bits({0}):", defaultPortStopBits.ToString());
            stopBits = Console.ReadLine();

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits), stopBits);
        }

        public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string handshake;

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Handshake({0}):", defaultPortHandshake.ToString());
            handshake = Console.ReadLine();

            if (handshake == "")
            {
                handshake = defaultPortHandshake.ToString();
            }

            return (Handshake)Enum.Parse(typeof(Handshake), handshake);
        }
    }
}

