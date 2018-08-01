using System;
using System.Collections.Generic;
using System.Linq;


//git remote add origin https://github.com/wegmarken2006/cssc3.git
//git push -u origin master

namespace Cssc3
{

    static public partial class SC4
    {
    }

    static public class Osc
    {

        public static byte[] b_reverse(byte[] buf, int len) {
            var bout = new byte[len];
            for (var ind = 0; ind < len; ind++) {
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


    }


}