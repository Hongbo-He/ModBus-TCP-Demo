using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace modbusTcpDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }


        Boolean flag = true;
        private void button1_Click(object sender, EventArgs e)
        {
            con();
            timer1.Enabled = true;


        }

        private Socket clientSocket;

        public void con() {
            int port = Convert.ToInt32("502");

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                clientSocket.Connect(endPoint);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public void recvSth() {
            while (true)
            {
                byte[] data = new byte[1024];

                int len = clientSocket.Receive(data);
                while (len == 0) {
                    continue;
                }
                int dataSize = data[8];

                byte[] data2 = new byte[dataSize];
                Console.WriteLine(dataSize);
                for (int i = 0; i < dataSize; i++) {
                    data2[i] = data[i + 9];
                }
                String re = BitConverter.ToString(data2);
                //label2.Text = re + "/r/n" + DateTime.Now;
                updateUI2(re + "  时间" + DateTime.Now);
                Console.WriteLine(re);


            }

        }
        delegate void changeText(String msg);

        private void delegateChangeLB2(String msg) {
            label2.Text = msg;
        }

        private void delegateChangeLB1(String msg)
        {
            label1.Text = msg;
        }


        private void updateUI(String msg) {
            changeText change = new changeText(delegateChangeLB1);
            this.groupBox1.Invoke(change,msg);
        }
        private void updateUI2(String msg)
        {
            changeText change = new changeText(delegateChangeLB2);
            this.groupBox1.Invoke(change, msg);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            timer1.Interval = 1000;
            if (clientSocket.Connected)
            {
                byte[] data = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x06, 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A };
                //label1.Text = BitConverter.ToString(data);
                updateUI(BitConverter.ToString(data));
                clientSocket.Send(data);
                Console.WriteLine("发送完毕");
            }
            else
            {
                Console.WriteLine("未连接");
                flag = true;
            }
            if (flag)
            {
                ThreadStart threadStart = new ThreadStart(recvSth);
                Thread thread = new Thread(threadStart);
                thread.IsBackground = true;
                thread.Start();

                flag = false;
            }
        }
    }
}
