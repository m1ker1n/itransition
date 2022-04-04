using System;
using System.Text;
using System.Security.Cryptography;

namespace Rock_Paper_Scissors
{
    internal class Verifier
    {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        private Byte[] _key;

        public string Key
        {
            get { return Convert.ToHexString(_key); }
        }

        public Verifier()
        {
            _key = new byte[32];
            rngCsp.GetBytes(_key);
        }

        public Verifier(Byte[] key)
        {
            _key = key;
        }

        public string GetHMAC(string msg)
        {
            byte[] byteMsg = Encoding.Default.GetBytes(msg);

            using (var hmac_sha256 = new HMACSHA256(_key))
            {
                byte[] byteHMAC = hmac_sha256.ComputeHash(byteMsg);
                return Convert.ToHexString(byteHMAC);
            }
        }
    }
}
