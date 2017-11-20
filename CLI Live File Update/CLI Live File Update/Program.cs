using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CLI_Live_File_Update
{
    class Program
    {
        static void Main(string[] args){
            String From = "";
            String LastModFrom = DateTime.Now.ToString();
            Boolean KeepLast = false;
            Boolean PastMax = false;
            List<string> To = new List<string>();
            Boolean AllowedLargeFiles = false;

            if (args.Length == 0) {
                Console.WriteLine("No Params Given....");
                Environment.Exit(2);
            }

            for (int runs = 0; runs < args.Count(); runs++) {
                if (args[runs] == "-m" || args[runs] == "--master") {
                    if (From == "") {
                        runs++;
                        From = args[runs];

                        if (!File.Exists(From)) {
                            Console.WriteLine("The Master file has to exist to copy from!");
                        }
                    }
                    else {
                        Console.WriteLine("You can only Define One Master File.");
                    }
                }
                else if (args[runs] == "-a" || args[runs] == "--add" || args[runs] == "-t" || args[runs] == "--target") {
                    runs++;
                    To.Add(args[runs]);

                    if (!File.Exists(args[runs])) {
                        File.Create(args[runs]).Dispose();
                    }
                }
                else if (args[runs] == "-l" || args[runs] == "--large-file" || args[runs] == "--large-files") {
                    AllowedLargeFiles = true;
                }
                else if (args[runs] == "-u" || args[runs] == "--no-update") {
                    KeepLast = true;
                }
                else if (args[runs] == "-b" || args[runs] == "--bypass-max") {
                    PastMax = true;
                }
                else {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Unknown Switch: " + args[runs]);
                    Console.ResetColor();
                }
            }

            if (To.Count == 0) {
                Console.WriteLine("No Target Files Found.");
                Environment.Exit(3);
            }
            else if (To.Count > 10 && PastMax == false) {
                Console.WriteLine("Default Max of 10. Use -b to Bypass the Default Max");
                Environment.Exit(3);
            }

            // Get the Main CopyFrom File Data
            FileInfo Main = new FileInfo(From);
            if (KeepLast == true) {
                LastModFrom = Main.LastWriteTime.ToString();
            }

            // 31457280 = 1024 MB = 1GB
            if (Main.Length > 31457280 && AllowedLargeFiles == false) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("MASTER FILE SIZE TOO LARGE! CAN NOT PROCESS FAST LARGE FILES!");
                Console.WriteLine("Use -l or --large-file to Continue");
                Console.ResetColor();
                Environment.Exit(1);
            }
            else if(Main.Length > 31457280 && AllowedLargeFiles == true) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Allowing the Use of Large Files.");
                Console.WriteLine("You may notice Performance issues with the Large Files");
                Console.ResetColor();
            }

            while (true) {

                if (!File.Exists(From)) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("MASTER FILE IS GONE!");
                    Console.ResetColor();
                    Environment.Exit(4);
                }

                Main = new FileInfo(From);
                while (LastModFrom != Main.LastWriteTime.ToString()) {
                    Console.Write("\n");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("New Update " + DateTime.Now);

                    Console.WriteLine("    Files are Being Updated:");
                    // Next Goal: is to make it have a Seperate Theread to Copy the Data
                    foreach (String SFile in To)
                    {
                        Console.WriteLine("        " + SFile);
                        Main.CopyTo(SFile, true);
                    }
                    LastModFrom = Main.LastWriteTime.ToString();
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1000);
                }
                Console.Write(".");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}