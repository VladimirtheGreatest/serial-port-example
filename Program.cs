using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Linq;

namespace SerialPortExample
{
    public class Program
    {
        static bool _continue;
        static SerialPort _serialPort;
        private const string StartMachineMessage = "\x02{0}\x11\x03";
        private const string StopMachineMessage = "\x02{0}\x12\x03";
        private const string ReadBatchMachineMessage = "\x02{0}\x14\x03";
        private const int StationNumber = 1;
        private static  readonly Action<List<byte>> _processMeasurement;
        private const int SizeOfMeasurement = 4;
        private static List<byte> Data = new List<byte>();
        private static List<string> Batch = new List<string>(); //filter out start and end of the text + 8 empty zeroes after that divide every 8 digits into separate lists, start with first and check if 0 if not continue, then just sum string so 0 0 0 0 0 0 1 4 will need to equal 14 and multiply by constant denomination of the list
        //for status i can do the same only take 3rd value from the list and switch statement if R continue if error go to error screen
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

            //  _serialPort.ReadTimeout = 50000;
            //  _serialPort.WriteTimeout = 50000;
         //   Thread readThread = new Thread(Read);
            _serialPort.PortName = "COM7";
            _serialPort.ReadTimeout = -1;
            _serialPort.WriteTimeout = -1;
            _serialPort.BaudRate = 9600;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Parity = Parity.None;
             // _serialPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

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

            //to do, all comands I need
            //start machine
            //stop machine
            //reset total
            //get status
            //get amount

            // examples here then put it in the desktop application
            //serial port monitor renew trial see the way desktop app is doing that
            //received data convert it to meaningful response
            //unit test for responses ???

            //I can either use a string, probably safer to use bytes
            //string _startMachineString = "\x02\x31\x11\x03\x47";  
            //string _getMachineStatus = "\x02\x31\x05\x03\x3b";
            //string _resetError = "\x02\x31\x10\x03\x46";
            //string _clearTotal = "\x02\x31\x18\x45\x03\x99";
            //string _clearGrandTotal = "\x02\x31\x18\x48\x03\x90";
            //  string _readOutBatchTotal = "\x02\x31\x14\x03\x4A";
            //  _serialPort.Write(_startMachineString);

            //string input = "STX";
            //char[] values = input.ToCharArray();
            //foreach (char letter in values)
            //{
            //    Get the integral value of the character.
            //    int value = Convert.ToInt32(letter);
            //    Convert the integer value to a hexadecimal value in string form.
            //    Console.WriteLine($"Hexadecimal value of {letter} is {value:X}");
            //}

            //  _serialPort.Write(_clearTotal);
            //  _serialPort.Write(_startMachineString);
            // _serialPort.Write(_getMachineStatus);
            // _serialPort.Write(_stopMachineString);

            byte[] _startMachine = new byte[] { 0x02, 0x31, 0x11, 0x03, 0x47 };   //works
            _serialPort.Write(_startMachine, 0, _startMachine.Length);

            //byte[] _stopMachine = new byte[] { 0x02, 0x31, 0x12, 0x03, 0x48 };   //works
            //_serialPort.Write(_stopMachine, 0, _stopMachine.Length);

            //byte[] _clearTotal = new byte[] { 0x02 , 0x31, 0x18, 0x45, 0x03, 0x93 };   //works
            //_serialPort.Write(_clearTotal, 0, _clearTotal.Length);


            // byte[] _getDenominations = new byte[] { 0x02, 0x31, 0x01, 0x03, 0x37 };  //returning something
            //  _serialPort.Write(_getDenominations, 0, _getDenominations.Length);


          // byte[] _getBatchTotal = new byte[] { 0x02, 0x31, 0x14, 0x03, 0x4a };
           // _serialPort.Write(_getBatchTotal, 0, _getBatchTotal.Length);

            //  byte[] _getMemory = new byte[] { 0x02, 0x31, 0x13, 0x03, 0x49 };
            // _serialPort.Write(_getMemory, 0, _getMemory.Length);

            // byte[] _getStatus = new byte[] { 0x02, 0x31, 0x05, 0x03, 0x3b };
            // _serialPort.Write(_getStatus, 0, _getStatus.Length);

            //var messages = new Dictionary<string, string>
            //{
            //    {nameof(StartMachineMessage), StartMachineMessage },
            //    {nameof(StopMachineMessage), StopMachineMessage },
            //    {nameof(ReadBatchMachineMessage), ReadBatchMachineMessage },
            //};

            //foreach (var msgPair in messages)
            //{
            //    var result = GetMessageWithBcc(msgPair.Value, StationNumber);
            //    Console.WriteLine($"{msgPair.Key}: {BytesAsHexString(result)}");
            //}

            //for some reason the event is not triggered so I will readdata programatically, so I will write command to serial port and immediatelly will read the message
            var count = _serialPort.BytesToRead;
            if (/*count == 85*/true) //simple check to find out if we managed to get back batch count
            {
                var batchTotal = new BatchTotal();
                var bytes = new byte[count];

                // var test = _serialPort.ReadExisting();   test = "\u00021000000000000000000000000000000000000000000000000000000000000000000000000$00000000\u0003Z"  the other way of doing that
                _serialPort.Read(bytes, 0, count);
                var hexString = BytesAsHexString(bytes); //I can get the amount from the string, will just need to create some function to convert the string into decimals so i can save it in db
                                                         //create model to store batch values, each property will be denominations count
                                                         //ref property total value, total count(stored in db)

                var hexStringUpdated = Batch.Skip(10).ToList(); //remove first 10 items which is STX, STN  and 8 zeroes

                var hexFinal = hexStringUpdated.Take(hexStringUpdated.Count - 2).ToList();  //remove etx and bcc

                var quantityIndex = hexFinal.IndexOf("24"); //helps us to find a DOLLAR SIGN seperation for quantity

                //they will go in this order, we know the order ahead so we can do this programatically
                //0.50 
                //0.20
                //2.00
                //0.02
                //0.10
                //1.00
                //0.01
                //0.05
                //Quantities lists
                batchTotal.QuantityTotal = hexFinal.GetRange(quantityIndex + 1, 8);

                batchTotal.QuantityTotal = hexFinal.GetRange(quantityIndex + 1, 8);
                batchTotal.FiftyCents = hexFinal.GetRange(0, 8).ToList();
                batchTotal.TwentyCents = hexFinal.GetRange(8, 8).ToList();
                batchTotal.TwoPounds = hexFinal.GetRange(17, 8).ToList();
                batchTotal.TwoCents = hexFinal.GetRange(25, 8).ToList();
                batchTotal.TenCents = hexFinal.GetRange(33, 8).ToList();
                batchTotal.OnePounds = hexFinal.GetRange(41, 8).ToList();
                batchTotal.OneCents = hexFinal.GetRange(49, 8).ToList();
                batchTotal.FiveCents = hexFinal.GetRange(57, 8).ToList();

                //Total sum calculated from quantities

                batchTotal.FiftyCentsTotal = CalculateTotalForDenomination(CalculateTotal(batchTotal.FiftyCents), (decimal)0.50);
                batchTotal.TwentyCentsTotal = CalculateTotalForDenomination(CalculateTotal(batchTotal.TwentyCents), (decimal)0.20);
                batchTotal.TwoPoundsTotal = CalculateTotalForDenomination(CalculateTotal(batchTotal.TwoPounds), (decimal)2.00);
                batchTotal.TwoCentsTotal = CalculateTotalForDenomination(CalculateTotal(batchTotal.TwoCents), (decimal)0.02);
                batchTotal.TenCentsTotal = CalculateTotalForDenomination(CalculateTotal(batchTotal.TenCents), (decimal)0.10);
                batchTotal.OnePoundsTotal = CalculateTotalForDenomination(CalculateTotal(batchTotal.OnePounds), (decimal)1.00);
                batchTotal.OneCentsTotal = CalculateTotalForDenomination(CalculateTotal(batchTotal.OneCents), (decimal)0.01);
                batchTotal.FiveCentsTotal = CalculateTotalForDenomination(CalculateTotal(batchTotal.FiveCents), (decimal)0.05);
                batchTotal.SumTotal = batchTotal.FiftyCentsTotal + batchTotal.TwentyCentsTotal + batchTotal.TwoPoundsTotal + batchTotal.TwoCentsTotal + batchTotal.TenCentsTotal +
                                        batchTotal.OnePoundsTotal + batchTotal.OneCentsTotal + batchTotal.FiveCentsTotal;

                batchTotal.FiftyCentsQuantity = CalculateQuantity(batchTotal.FiftyCentsTotal, (decimal)0.50);
                batchTotal.TwentyCentsQuantity =  CalculateQuantity(batchTotal.TwentyCentsTotal, (decimal)0.20);
                batchTotal.TwoPoundsQuantity = CalculateQuantity(batchTotal.TwoPoundsTotal, (decimal)2.00);
                batchTotal.TwoCentsQuantity = CalculateQuantity(batchTotal.TwoCentsTotal, (decimal)0.02);
                batchTotal.TenCentsQuantity = CalculateQuantity(batchTotal.TenCentsTotal, (decimal)0.10);
                batchTotal.OnePoundsQuantity = CalculateQuantity(batchTotal.OnePoundsTotal, (decimal)1.00);
                batchTotal.OneCentsQuantity = CalculateQuantity(batchTotal.OneCentsTotal, (decimal)0.01);
                batchTotal.FiveCentsQuantity = CalculateQuantity(batchTotal.FiveCentsTotal, (decimal)0.05);
                batchTotal.Quantity = batchTotal.FiftyCentsQuantity + batchTotal.TwentyCentsQuantity + batchTotal.TwoPoundsQuantity + batchTotal.TwoCentsQuantity +
                                    batchTotal.TenCentsQuantity+ batchTotal.OnePoundsQuantity + batchTotal.OneCentsQuantity + batchTotal.FiveCentsQuantity;
                //after that just sum strings, remove all zeros till not zero
                //after that sum all decimal total into total sum, quantity total to decimal needs to equal sum of all denominations quantities

            }     

             byte[] _getStatus = new byte[] { 0x02, 0x31, 0x05, 0x03, 0x3b };
             _serialPort.Write(_getStatus, 0, _getStatus.Length);

            var countStatusBytes = _serialPort.BytesToRead;
            var bytesStatus = new byte[countStatusBytes];

            // var test = _serialPort.ReadExisting();   test = "\u00021000000000000000000000000000000000000000000000000000000000000000000000000$00000000\u0003Z"  the other way of doing that
            _serialPort.Read(bytesStatus, 0, countStatusBytes);
            var hexStatus = BytesAsHexString(bytesStatus);
            var test = System.Convert.ToChar(System.Convert.ToUInt32(hexStatus, 16));

            //once I have decimal total and list of denominations in decimals + quantity converted I can start implementing this

            // AddBytes(bytes);


            //readThread.Join();
            _serialPort.Close();
        }

        //this is not triggered but can read data programatically

        //public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> items,
        //                                           int numOfParts)
        //{
        //    int i = 0;
        //    return items.GroupBy(x => i++ % numOfParts);
        //}

        private static void port_DataReceived(object sender,
        SerialDataReceivedEventArgs e)
        {
            while (_serialPort.BytesToRead > 0)
            {
                var count = _serialPort.BytesToRead;
                var bytes = new byte[count];
                _serialPort.Read(bytes, 0, count);
                AddBytes(bytes);
            }
            // Show all the incoming data in the port's buffer
            var dataReceived = _serialPort.ReadExisting();
            //var test = _serialPort.ReadLine();
            //var another = _serialPort.ReadByte();
            Debug.Print("Data Received:");
            Debug.Print(dataReceived);
            //Console.WriteLine(dataReceived);
            //Console.WriteLine(test);
            //Console.WriteLine(another);
        }

        private static void AddBytes(byte[] bytes)
        {
            Data.AddRange(bytes);
            while (Data.Count > SizeOfMeasurement)
            {
                var measurementData = Data.GetRange(0, SizeOfMeasurement);
                Data.RemoveRange(0, SizeOfMeasurement);
                if (_processMeasurement != null) _processMeasurement(measurementData);
            }

        }



        public static void Read()
        {
            while (_continue)
            {
                try
                {
                   // string message = _serialPort.ReadExisting();
                    string m = _serialPort.ReadLine();
                    //var test =  GetStringFromAsciiHex(m);
                    if (!string.IsNullOrEmpty(m))
                    {
                        var test = System.Text.ASCIIEncoding.ASCII.GetBytes(m);
                        Debug.WriteLine(m);
                    }

                }
                catch (TimeoutException) { }
            }
        }

        private static byte GetBccOfBytes(byte[] bytes)
        {
            var bcc = 0;
            foreach (var byt in bytes)
            {
                bcc += byt;
            }
            return (byte)(bcc & 0xff);
        }

        public static byte[] GetMessageWithBcc(string format, int stationNumber)
        {
            var messageWithStation = string.Format(format, stationNumber);  //mix the station number into the format string
            var noBccBytes = Encoding.ASCII.GetBytes(messageWithStation);   //convert the string to an array of ASCII bytes
            var result = new byte[noBccBytes.Length + 1];                   //allocate a buffer to hold the result, note room for BCC
            for (var i = 0; i < noBccBytes.Length; ++i)                     //copy the first byte array to the result byte array
            {
                result[i] = noBccBytes[i];
            }
            result[noBccBytes.Length] = GetBccOfBytes(noBccBytes);          //calculate and append the BCC
            return result;
        }

        /// <summary>
        /// this is used to convert the bytes gibberish into hex string which can be translated into something meaningful
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string BytesAsHexString(byte[] bytes)
        {
            bool started = false;
            var buffer = new StringBuilder(bytes.Length * 3 - 1);
            foreach (var byt in bytes)
            {
                if (started)
                {
                    buffer.Append(" ");
                }
                started = true;
                buffer.Append($"{byt:x02}");
                Batch.Add($"{byt:x02}");
            }
            return buffer.ToString();
        }

        /// <summary>
        /// this method will take list of string which represents quantity received from a coint sorter
        /// and will convert it it meaningful number that can be multipled by the denomination so we can calculate the total amount of money inserted in the machine and not just a machine gibberish
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private static decimal CalculateTotal(List<string> quantity)
        {
            StringBuilder sb = new StringBuilder(null, 8);
            for (int i = 0; i < quantity.Count; i++)
            {
                sb.Append(Char.ConvertFromUtf32(Convert.ToInt32(quantity[i], 16)));
            }
            return Convert.ToDecimal(sb.ToString());
            
        }

        private static int CalculateQuantity(decimal total, decimal denomination)
        {
            return Convert.ToInt32(total / denomination);
        }
        private static decimal CalculateTotalForDenomination(decimal quantity, decimal denomination)
        {
            return Math.Round((Convert.ToDecimal(quantity.ToString("F"))) * (Convert.ToDecimal(denomination.ToString("F"))), 2);
        }

        private static string GetStringFromAsciiHex(String input)
        {
            if (input.Length % 2 != 0)
                throw new ArgumentException("input");

            byte[] bytes = new byte[input.Length / 2];

            for (int i = 0; i < input.Length; i += 2)
            {
                // Split the string into two-bytes strings which represent a hexadecimal value, and convert each value to a byte
                String hex = input.Substring(i, 2);
                bytes[i / 2] = Convert.ToByte(hex, 16);
            }

            return System.Text.ASCIIEncoding.ASCII.GetString(bytes);
        }


        //message = "\u00021100000500020020000020010010000010005\u0003"  readline returned
        //1100000500020020000020010010000010005


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

