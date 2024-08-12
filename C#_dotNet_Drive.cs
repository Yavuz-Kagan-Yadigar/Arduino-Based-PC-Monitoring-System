using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using OpenHardwareMonitor.Hardware;
using Microsoft.Win32;

namespace CPU_GPU
{
   


    public partial class Form1 : Form
    {
        private SerialPort port;
        private string com_no = "";
        private bool bagli = false;
        static double cpuTemp;
        static double ram;
        static double cpuUsage;
        static double cpuPowerDrawPackage;
        static double cpuFrequency;
        static double ramyuzde;
        static double gpuTemp;
        static double gpuUsage;


        static Computer c = new Computer()
        {
            GPUEnabled = true,
            CPUEnabled = true,
            RAMEnabled = true,
        };

        public Form1()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue("My ApplicationStartUpDemo", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");//başlangıçta otomatik açılma |auto startup at boot
            }
            InitializeComponent();
            timer1.Interval = 100;//yenileme aralığı ms cinsinden. | refresh rate in ms-----------------------------------<|
            c.Open();

            this.Show();
            
            try
            {
                port = new SerialPort("COM10", 9600);//Port ve baud hızını seçin | Choose you port and baud rate.----------------------------------------------<|
                port.Open();
                timer1.Start();
               

                bagli = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"COM port açılırken hata oluştu: {ex.Message}");
                bagli = false;

            }

        }




        static void GetSensors()
        {
            foreach (var hardware in c.Hardware)
            {

                if (hardware.HardwareType == HardwareType.CPU)
                {
                    hardware.Update();

                    foreach (var sensor in hardware.Sensors)


                         if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("CPU Package"))//benim işlemcim 12. nesil intel işlemcili olduğu ve Open Hardware Monitor desteklemediği için cpu kullanımını tam doğru olamyan bir şekilde,sıcaklığı ise alamıyoruz.siz kendi bilgisayarınıza göre kodu düzenleyeiblirsiniz | due to my pc has a 12th gen cpu and Open Hardware Monitor doesnt support it,i disabled it.You can modify code if your pc is supported.
                            {
                                cpuTemp = sensor.Value.GetValueOrDefault();

                            }
                        else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
                          {
                                cpuUsage = sensor.Value.GetValueOrDefault();

                         }
                        else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("CPU Cores"))
                        {
                           cpuPowerDrawPackage = sensor.Value.GetValueOrDefault();

                         }

                        else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("CPU Core #1"))
                        {
                            cpuFrequency = sensor.Value.GetValueOrDefault();

                        }

                }


                if (hardware.HardwareType == HardwareType.GpuAti || hardware.HardwareType == HardwareType.GpuNvidia)
                {
                    hardware.Update();

                    foreach (var sensor in hardware.Sensors)
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("GPU Core"))
                        {
                            gpuTemp = sensor.Value.GetValueOrDefault();
                        }
                        else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("GPU Core"))
                        {

                            gpuUsage = sensor.Value.GetValueOrDefault();
                        }



                }


                if (hardware.HardwareType == HardwareType.RAM)
                {
                    hardware.Update();

                    foreach (var sensor in hardware.Sensors)
                        if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("Memory"))
                        {
                            ramyuzde = sensor.Value.GetValueOrDefault();

                        }
                    ram =Math.Round(ramyuzde * 0.32,2) ;
                }


            }
        }

       

        private void timer1_Tick(object sender, EventArgs e)
        {
            GetSensors();

                port.Write("!" + Math.Round(cpuUsage,0)+"+"+ gpuUsage + "*"+ + ram + "?" + gpuTemp + "-'");//verileri paketleyip tek parça şeklinde com portu ile gönderme,decode etmek için arduino koduna bakın. | sending all datas at once.to learn how to decode check out arudino code.
         }
    }






}
