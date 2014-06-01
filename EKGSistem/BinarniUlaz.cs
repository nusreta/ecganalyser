/*
 * BinarniUlaz.cs
 * Nedim Srndic
 * Biomedicinski signali i sistemi
 * Elektrotehnicki fakultet Univerziteta u Sarajevu
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace signalreader
{
    class BinarniUlaz : Ulaz
    {
        string fileName;
        BackgroundWorker worker;
        short channel;
        FileStream file;
        BinaryReader reader;
        int sleepInterval;

        public BinarniUlaz(string fileName, int sleepInterval)
        {
            this.fileName = fileName;
            worker = new BackgroundWorker();
            this.sleepInterval = sleepInterval;
        }

        public override void Read(int channel)
        {
            this.channel = Convert.ToInt16(channel);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            long fileLength = 0;
            try
            {
                file = new FileStream(fileName, FileMode.Open);
                reader = new BinaryReader(file);
            }
            catch (Exception)
            {
                // vec je otvoren fajl, sve je OK
            }
            fileLength = file.Length;
            short flag = 0;
            long low = 0, high = 0;
            byte[] buf = { 0, 0, 0 };

            for (int i = 0; i < fileLength / 3; i++)
            {
                for (short j = 1; j <= 2; j++)
                {
                    switch (flag)
                    {
                        case 0:
                            try
                            {
                              
                                    buf = reader.ReadBytes(3);
                                
                            }
                            catch (Exception)
                            {
                                return;
                            }
                            low = buf[1] & 0x0F;
                            high = buf[1] & 0xF0;
                            if (channel == j)
                                if (low > 7)
                                    while (!UlazniBuffer.Write(Convert.ToDouble((buf[0] + (low << 8) - 4096))))
                                        System.Threading.Thread.Sleep(sleepInterval);
                                else
                                    while (!UlazniBuffer.Write(Convert.ToDouble((buf[0] + (low << 8) ) )))
                                        System.Threading.Thread.Sleep(sleepInterval);
                            flag = 1;
                            break;
                        case 1:
                            if (channel == j)
                                if (high > 127)
                                    while (!UlazniBuffer.Write(Convert.ToDouble(buf[2] + (high << 4) - 4096)))
                                        System.Threading.Thread.Sleep(sleepInterval);
                                else
                                    while (!UlazniBuffer.Write(Convert.ToDouble((buf[2] + (high << 4)))))
                                        System.Threading.Thread.Sleep(sleepInterval);
                            flag = 0;
                            break;
                    }
                }
            }

            while (!UlazniBuffer.Write(Double.PositiveInfinity))
                System.Threading.Thread.Sleep(sleepInterval);

            Stop();
        }

        public override void Stop()
        {
            reader.Close();
            file.Close();
            worker.WorkerSupportsCancellation = true;
            worker.CancelAsync();
        }
    }
}
