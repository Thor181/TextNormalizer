using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextNormalizer
{
    internal class Crypter
    {
        internal string Encrypt(string encryptableString)
        {
            StringBuilder str = new StringBuilder(encryptableString);
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = SwapBits(str[i]);
            }
            return str.ToString();
        }
        internal string Decrypt(string decryptableString)
        {
            StringBuilder str = new StringBuilder(decryptableString);
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = SwapBits(str[i]);
            }
            return str.ToString();
        }
        private static char SwapBits(char x)
        {
            return (char)((x & 0x0F) << 4 | (x & 0xF0) >> 4);
        }
    }
}
