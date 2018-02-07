using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace CustomerPrinterStatusChecker
{
    class Program
    {
        public const int MOD_TG2480H = 370;
        public class Dispenser
        {

            [DllImport("CeSmLm.DLL")]
            public static extern UInt32 CePrnInitCeUsbSI(byte[] dwSysErr);
            [DllImport("CeSmLm.DLL")]
            public static extern UInt32 CePrnGetStsUsb(int intDev, byte[] dwSts, byte[] dwSysErr);
            [DllImport("CeSmLm.DLL")]
            public static extern UInt32 SetPrinterModel(UInt32 dwPrnModel);
            [DllImport("CeSmLm.DLL")]
            public static extern UInt32 CePrnGetTotPaperRemainingUsb(int intCom, byte[] dwNumCmPaper, byte[] dwSysErr);

        }
        static void Main(string[] args)
        {               
            UInt32 result;
            byte[] pBuffer = new byte[128];   

            result = Dispenser.CePrnInitCeUsbSI(pBuffer);
            if (result == 0)
            {
                Console.WriteLine("Printer:Off:Null,");
            }
            else
            {

                byte[] dwSts = new byte[20];                
                byte[] dwSysErr = new byte[20];
                
                result = Dispenser.SetPrinterModel(MOD_TG2480H);

                if (result != 0)
                {
                    Console.WriteLine("Printer:Off:Null,");
                }
                else
                {
                    result = Dispenser.CePrnGetStsUsb(0, dwSts, dwSysErr);
                    if (result != 0)
                    {
                        Console.WriteLine("Printer:Off:Null,");
                    }
                    else
                    {                        
                        string status = BitConverter.ToInt32(dwSts, 0).ToString();

                        //  Custom 2480
                        //private const uint NOPAPER          = 0x00000001;
                        //private const uint NEARPAPEREND     = 0x00000004;
                        //private const uint TICKETOUT        = 0x00000020;
                        //private const uint NOHEAD           = 0x00000100;
                        //private const uint NOCOVER          = 0x00000200;
                        //private const uint PAPERJAM         = 0x00400000;

                        //  Custom VKP800II
                        //  PAPER AT MOUTH = 2084
                        //  PAPER LOW = 2052
                        //  PRINITER BUSY = 3076
                        //  NO PAPER = 5
                        //  PAPER NORMAL = 0

                        if (status == "2052") //8388612
                        {
                            Console.WriteLine("Printer:On:Paper Low,");
                        }
                        else if (status == "5") //8388613
                        {
                            Console.WriteLine("Printer:On:Paper Empty,");
                        }
                        else if (status == "0") //8388608
                        {
                            Console.WriteLine("Printer:On:Normal,");
                        }
                        else
                        {
                            Console.WriteLine("Printer:On:Error, " + status);
                        }
                        //Console.WriteLine(status);
                    }

                }
            }
            //Console.ReadLine();
        }
    }
}
