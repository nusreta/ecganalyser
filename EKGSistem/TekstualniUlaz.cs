/*
 * TekstualniUlaz.cs
 * Nedim Srndic
 * Biomedicinski signali i sistemi
 * Elektrotehnicki fakultet Univerziteta u Sarajevu
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.IO;
using System.ComponentModel;
using System.Collections;

namespace signalreader
{
    public class TekstualniUlaz : Ulaz
    {
        String fileName;
        int channel;
        BackgroundWorker worker = null;
        StreamReader file;
        int sleepInterval;

        public TekstualniUlaz(string fileName, int sleepInterval)
        {
            this.fileName = fileName;
            worker = new BackgroundWorker();
            this.sleepInterval = sleepInterval;
        }
        public override void Read(int channel)
        {
            this.channel = channel;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string line = "";
            double signal;
            try
            {
                try
                {
                    file = new StreamReader(fileName);
                }
                catch (Exception)
                {
                    // vec je otvoren fajl
                }

                line = file.ReadLine();

                while (!(line.Contains("0.000")))
                {
                    if (line == null)
                        break;
                    try
                    {
                        line = file.ReadLine();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }

                do
                {
                    signal = (double.Parse(line.Split('\t')[channel]));
                    signal = signal / 1000;
                    while (!UlazniBuffer.Write(signal))
                        System.Threading.Thread.Sleep(sleepInterval);
                } while ((line = file.ReadLine()) != null);

                while (!UlazniBuffer.Write(double.PositiveInfinity))
                {
                    System.Threading.Thread.Sleep(sleepInterval);
                }
            }
            catch
            {
            }
            Stop();
        }

        public override void Stop()
        {
            file.Close();
            worker.WorkerSupportsCancellation = true;
            worker.CancelAsync();
        }
    }
}
