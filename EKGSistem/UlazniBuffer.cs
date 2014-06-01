/*
 * UlazniBuffer.cs
 * Nedim Srndic
 * Biomedicinski signali i sistemi
 * Elektrotehnicki fakultet Univerziteta u Sarajevu
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace signalreader
{
    public enum EKGFileType {
        BINARY,
        TEXT
    };

    public static class UlazniBuffer
    {
        private static int MAX_BUFFER_SIZE = 75;
        private static double[] buffer = new double[MAX_BUFFER_SIZE];

        private static int startIndex = 0;
        private static int bufferSize = 0;

        private static object mutex = new object();
        private static Ulaz ulaz;

        public static void Open(string filename, short channel, EKGFileType filetype)
        {
            if (!System.IO.File.Exists(filename))
                throw new System.IO.FileNotFoundException("Fajl nije pronadjen", filename);
            if (filetype == EKGFileType.BINARY)
                ulaz = new BinarniUlaz(filename, 30);
            else if (filetype == EKGFileType.TEXT)
                ulaz = new TekstualniUlaz(filename, 30);

            ulaz.Read(channel);
        }

        public static bool ReadOne(out double value)
        {
            lock (mutex)
            {
                // data available
                if (bufferSize > 0)
                {
                    value = buffer[startIndex];
                    startIndex = (startIndex + 1) % MAX_BUFFER_SIZE;
                    bufferSize--;
                    //Console.WriteLine("r+ {0}", value);
                    return true;
                }
                else // no data available
                {
                    value = 0;
                    //Console.Write("r");
                    return false;
                }
            }
        }

        public static bool ReadMany(out double[] value, int count)
        {
            lock (mutex)
            {
                // data available
                if (count > 0 && bufferSize >= count)
                {
                    value = new double[count];
                    for (int i = 0; i < count; i++)
                    {
                        value[i] = buffer[startIndex];
                        startIndex = (startIndex + 1) % MAX_BUFFER_SIZE;
                    }
                    bufferSize -= count;
                    //Console.WriteLine("r+ {0}", value);
                    return true;
                }
                else // no data available
                {
                    value = new double[1];
                    //Console.Write("r");
                    return false;
                }
            }
        }

        public static bool Write(double value)
        {
            lock (mutex) {
                if (bufferSize < MAX_BUFFER_SIZE)
                {
                    buffer[(startIndex + bufferSize) % MAX_BUFFER_SIZE] = value;
                    bufferSize++;
                    //Console.WriteLine("w+ {0}", value);
                    return true;
                }
                else {
                    //Console.Write("w");
                    return false;
                }
            }            
        }

        public static void Clear()
        {
            lock (mutex)
            {
                startIndex = 0;
                bufferSize = 0;
            }
        }
        public static void Close()
        {
            if(ulaz!=null)
            ulaz.Stop();
        }
    }
}
