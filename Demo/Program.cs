using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JiaHe.LicenseManager;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            String execPath = "license-app.exe";

            String keyStr = "ufida2016";
            byte[] key = new byte[keyStr.Length];
            for (int i = 0; i < key.Length; i++)
            {
                key[i] = (byte) keyStr[i];
            }

            License license = new License(42, DateTime.UtcNow, "测试软件", "测试公司");
            byte[] cipher_text = License.encryptTo(execPath, key, license);
            Console.WriteLine(cipher_text);

            License license2 = License.decryptFrom(execPath, key, cipher_text);

            License license3 = License.decryptFrom(execPath, new byte[] { 1 }, cipher_text);
            Console.WriteLine(license3 == null);
        }
    }
}
