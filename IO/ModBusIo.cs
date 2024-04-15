using FluentModbus;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace PLPTVision
{
    internal class ModbusIO
    {
        public event EventHandler<IOChageArgs>? OnIOChange;
        short[] inData = new short[16];
        bool init = false;
        object lockObj = new object();

        ModbusRtuClient client = new ModbusRtuClient
        {
            BaudRate = 115200,
            Parity = Parity.None,
            ReadTimeout = 1000,
        };

        bool started = false;
        public void Open(string portName)
        {
            client.Connect(portName, ModbusEndianness.BigEndian);
            client.WriteTimeout = 1000;
        }

        public void StartRead()
        {
            if (started)
                return;
            started = true;
            Task.Run(() =>
            {
                while (started)
                {
                    Span<short> readData = new Span<short>();

                    lock (lockObj)
                    {
                        readData = client.ReadInputRegisters<short>(1, 0, 4);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        if (inData[i] != readData[i])
                        {
                            OnIOChange?.Invoke(this, new IOChageArgs { port = i, value = readData[i] == 1 ? true : false });
                            inData[i] = readData[i];
                        }
                    }
                    Thread.Sleep(20);
                }
            });
        }

        public void On(int port, bool value)
        {

            var pin = port switch
            {
                2 => 10,
                3 => 14,
                _ => -1
            };
            if (pin != -1)
            {
                lock (lockObj)
                {
                    client.WriteSingleRegister(1, pin, (short)(value ? 1 : 0));
                }
            }
        }
    }

    public class IOChageArgs : EventArgs
    {
        public int port;
        public bool value;
    }
}
