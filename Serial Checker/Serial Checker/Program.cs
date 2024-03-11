using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Management;
using System.Threading; 
using Console = Colorful.Console;

namespace SerialChecker
{
    internal class Program
    {
        static string separator = "========================";
        static string path = $@"{Directory.GetCurrentDirectory()}\Serials\Serials_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt";

        static void Main(string[] args)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            Console.Title = "Serial Checker | Open Source";
            Console.WriteLine("Press any key to check your serial numbers", Color.BlueViolet);
            Console.ReadKey();
            Console.Clear();
            CheckSerials();
            Console.ReadLine();
        }

        static void CheckSerials()
        {
            GetSerial("BIOS", "SELECT * FROM Win32_ComputerSystemProduct", "IdentifyingNumber", "UUID");
            GetSerial("CPU", "SELECT ProcessorId, SerialNumber FROM Win32_Processor", "ProcessorId", "SerialNumber");
            GetSerial("Disk Drive", "SELECT SerialNumber FROM Win32_DiskDrive", "SerialNumber");
            GetSerial("Mainboard", "SELECT SerialNumber FROM Win32_BaseBoard", "SerialNumber");
            GetSerial("RAM", "SELECT SerialNumber FROM Win32_PhysicalMemory", "SerialNumber");
            GetMACAddress();
            GetModels();
        }

        static void GetSerial(string header, string query, params string[] properties)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                Console.WriteLine($"\n{separator}{header}{separator}", Color.Magenta);

                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine($"{separator}{header}{separator}");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        foreach (string property in properties)
                        {
                            string value = obj[property].ToString();
                            Console.WriteLine($"{property}: {value}", Color.Cyan);
                            Thread.Sleep(35);
                            writer.WriteLine($"{property}: {value}");
                        }
                    }
                    writer.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while retrieving {header} information: {e.Message}", Color.Red);
            }
        }

        static void GetMACAddress()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT MACAddress FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
                Console.WriteLine($"\n{separator}MAC Address{separator}", Color.Magenta);

                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine($"{separator}MAC Address{separator}");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string macAddress = obj["MACAddress"].ToString();
                        Console.WriteLine($"MAC Address: {macAddress}", Color.Cyan);
                        Thread.Sleep(35);
                        writer.WriteLine($"MAC Address: {macAddress}");
                    }
                    writer.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while retrieving MAC address information: {e.Message}", Color.Red);
            }
        }

        static void GetModels()
        {
            try
            {
                ManagementObjectSearcher mainboard = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                ManagementObjectSearcher gpu = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                ManagementObjectSearcher cpu = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");

                Console.WriteLine($"\n{separator}Models{separator}", Color.Magenta);

                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine($"{separator}Models{separator}");

                    Console.WriteLine("Mainboard: ", Color.Cyan);
                    writer.WriteLine("Mainboard:");
                    foreach (ManagementObject obj in mainboard.Get())
                    {
                        string manufacturer = obj["Manufacturer"].ToString();
                        string product = obj["Product"].ToString();
                        Console.WriteLine($"Manufacturer: {manufacturer}", Color.Cyan);
                        Console.WriteLine($"Product: {product}\n", Color.Cyan);
                        Thread.Sleep(35);
                        writer.WriteLine($"Manufacturer: {manufacturer}");
                        writer.WriteLine($"Product: {product}\n");
                    }

                    foreach (ManagementObject obj in gpu.Get())
                    {
                        string model = obj["Name"].ToString();
                        Console.WriteLine($"GPU: {model}\n", Color.Cyan);
                        Thread.Sleep(35);
                        writer.WriteLine($"GPU: {model}");
                    }

                    foreach (ManagementObject obj in cpu.Get())
                    {
                        string model = obj["Name"].ToString();
                        Console.WriteLine($"CPU: {model}\n", Color.Cyan);
                        Thread.Sleep(35);
                        writer.WriteLine($"CPU: {model}");
                    }

                    writer.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while retrieving models information: {e.Message}", Color.Red);
            }
        }
    }
}
