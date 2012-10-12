using System;

namespace Utilities
{
    public static class Misc
    {
        public static byte[] ToBytes(String str)
        {
            var bytes = new byte[str.Length * sizeof (char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
