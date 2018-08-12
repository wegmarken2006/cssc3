using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

//git remote add origin https://github.com/wegmarken2006/cssc3.git
//git push -u origin master

namespace Cssc3
{

    static public partial class SC4
    {
    }

    static public class Osc
    {

        public static byte[] b_reverse(byte[] buf, int len)
        {
            var bout = new byte[len];
            for (var ind = 0; ind < len; ind++)
            {
                bout[ind] = buf[len - ind - 1];
            }
            return bout;
        }

        public static byte[] encode_i8(int val)
        {
            var bb = Convert.ToByte(val);
            var b1 = new byte[1];
            b1[0] = bb;
            return b1;
        }
        public static int decode_i8(byte[] buf)
        {
            var b2 = new byte[2];
            b2[0] = buf[0];
            b2[1] = 0;
            var cc = BitConverter.ToChar(b2);
            return (int)cc;
        }

        public static byte[] encode_i16(int val)
        {
            var bb = BitConverter.GetBytes(val);
            var b2 = b_reverse(bb, 2);
            return b2;
        }

        public static int decode_i16(byte[] buf)
        {
            var b2 = b_reverse(buf, 2);
            var ii = BitConverter.ToInt16(b2);
            return ii;
        }

        public static byte[] encode_i32(int val)
        {
            var bb = BitConverter.GetBytes(val);
            var b2 = b_reverse(bb, 4);
            return b2;
        }

        public static int decode_i32(byte[] buf)
        {
            var b2 = b_reverse(buf, 4);
            var ii = BitConverter.ToInt32(b2);
            return ii;
        }
        public static byte[] encode_f32(double val)
        {
            var bb = BitConverter.GetBytes(val);
            var b2 = b_reverse(bb, 4);
            return b2;
        }

        public static double decode_f32(byte[] buf)
        {
            var b2 = b_reverse(buf, 4);
            var ii = BitConverter.ToDouble(b2);
            return ii;
        }
        public static byte[] encode_str(string val)
        {
            var bb = Encoding.ASCII.GetBytes(val);
            return bb;
        }

        public static byte[] str_pstr(string val)
        {
            var stream = new MemoryStream();
            var bw = new BinaryWriter(stream);
            var bb = Encoding.ASCII.GetBytes(val);
            bw.Write((byte)val.Length);
            bw.Write(bb);
            return stream.ToArray();
        }
        public static int align(int n)
        {
            return 4 - n % 4;
        }
        public static byte[] extend_(byte[] pad, byte[] bts)
        {
            var stream = new MemoryStream();
            var bw = new BinaryWriter(stream);

            var n = align(bts.Length);
            bw.Write(bts);

            for (var ind = 0; ind < n; ind++)
            {
                bw.Write(pad);
            }
            return stream.ToArray();
        }
        public static byte[] encode_string(string str)
        {
            var eb = new byte[] { 0x0 };
            //eb[0] = 0;
            return extend_(eb, encode_str(str));
        }

        public static byte[] encode_blob(byte[] bts)
        {
            var stream = new MemoryStream();
            var bw = new BinaryWriter(stream);
            bw.Write(bts.Length);
            var eb = new byte[] { 0x0 };
            bw.Write(extend_(eb, bts));
            return stream.ToArray();
        }

        public static byte[] encode_datum(object dt)
        {
            if (dt is int dti) return encode_i32(dti);
            if (dt is double dtd) return encode_f32(dtd);
            if (dt is string dts) return encode_string(dts);
            if (dt is byte[] dtb) return encode_blob(dtb);
            throw new Exception("encodedatum");
        }

        public static string tag(object dt)
        {
            if (dt is int dti) return "i";
            if (dt is double dtd) return "f";
            if (dt is string dts) return "s";
            if (dt is byte[] dtb) return "b";
            throw new Exception("encodedatum");
        }

        public static string descriptor(object[] dt)
        {
            var sout = ",";
            foreach (var item in dt)
            {
                sout = sout + tag(item);
            }

            return sout;
        }
        public class Message
        {
            public string Name { get; set; }
            public object[] LDatum { get; set; }
        }
        public static byte[] encode_message(Message message)
        {
            var stream = new MemoryStream();
            var bw = new BinaryWriter(stream);
            bw.Write(encode_datum(message.Name));
            bw.Write(encode_datum(descriptor(message.LDatum)));

            foreach (var item in message.LDatum)
            {
                bw.Write(encode_datum(item));
            }
            return stream.ToArray();
        }

        public static void print_barray(byte[] ba)
        {
            foreach (var item in ba)
            {
                Console.Write(item);
                Console.Write(" ");
            }

        }
        public static void send_message(Message message)
        {
            var bmsg = encode_message(message);
            Console.WriteLine("\nDEBUG");
            print_barray(bmsg);
            osc_send(bmsg);
        }

        public static class PortConfig
        {
            public static string UdpIP;
            public static int UdpPort;
            public static UdpClient UdpCl;
            public static IPEndPoint Ep;
        }

        public static void sc_start()
        {
            osc_set_port();
            var msg1 = new Message { Name = "/notify", LDatum = new object[] { 1 } };
            send_message(msg1);
            msg1 = new Message { Name = "/g_new", LDatum = new object[] { 1, 1, 0 } };
            send_message(msg1);
        }
        public static void osc_set_port()
        {
            var client = new UdpClient();
            PortConfig.UdpIP = "127.0.0.1";
            PortConfig.UdpPort = 57110;
            IPEndPoint ep = new IPEndPoint(
                IPAddress.Parse(PortConfig.UdpIP), PortConfig.UdpPort);

            PortConfig.UdpCl = client;
            PortConfig.Ep = ep;

            client.Client.SendTimeout = 2000;
            client.Client.ReceiveTimeout = 10000;
            try
            {
                client.Connect(ep);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Connect error: ");
                Console.WriteLine(e.Message);
            }

        }
        public static void osc_send(byte[] message)
        {
            var client = PortConfig.UdpCl;
            var ep = PortConfig.Ep;

            try
            {
                // send data
                client.Send(message, message.Length);
                var tref = new ThreadStart(osc_receive);
                var tthread = new Thread(tref);
                tthread.Start();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Send error: ");
                Console.WriteLine(e.Message);
            }
        }

        public static void osc_receive() {
            var client = PortConfig.UdpCl;
            var ep = PortConfig.Ep;
            try
            {
                var receivedData = client.Receive(ref ep);
                Console.WriteLine("\nReceived: ");
                print_barray(receivedData);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Receive error: ");
                Console.WriteLine(e.Message);
            }
           
        }








    }


}