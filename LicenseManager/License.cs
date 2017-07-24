using System;
using System.Text;

namespace JiaHe.LicenseManager
{
    public class License
    {
        /// <summary>
        /// 站点数
        /// </summary>
        public UInt32 SiteCount { get; set; }

        /// <summary>
        /// 证书过期时间（UTC）
        /// </summary>
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// 软件名称，不要包含空白字符，也不要为空
        /// </summary>
        public String SoftwareName { get; set; }

        /// <summary>
        /// 公司名称，不要包含空白字符，也不要为空
        /// </summary>
        public String CompanyName { get; set; }

        /// <summary>
        /// 初始化各个属性
        /// </summary>
        /// <param name="siteCount">站点数</param>
        /// <param name="expirationTime">证书过期时间</param>
        /// <param name="softwareName">软件名称</param>
        /// <param name="companyName">公司名称</param>
        public License(
            UInt32 siteCount,
            DateTime expirationTime,
            String softwareName,
            String companyName)
        {
            SiteCount = siteCount;
            ExpirationTime = expirationTime;
            SoftwareName = softwareName;
            CompanyName = companyName;
        }

        /// <summary>
        /// 使用默认值初始化一个<code>License</code>
        /// 即：站点数为0，时间为 1970-01-01 00:00，
        /// 软件名称为“演示软件”，公司名称为“演示公司”
        /// </summary>
        public License() : this(
            0,
            new DateTime(1970, 1, 1),
            "演示软件",
            "演示公司")
        { }

        //public static bool operator ==(License lhs, License rhs)
        //{
        //    if (lhs == null && rhs == null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return lhs.SiteCount == rhs.SiteCount
        //            && lhs.ExpirationTime == rhs.ExpirationTime
        //            && lhs.CompanyName == rhs.CompanyName
        //            && lhs.SoftwareName == rhs.SoftwareName;
        //    }
        //}

        //public static bool operator !=(License lhs, License rhs)
        //{
        //    return !(lhs == rhs);
        //}

        private static String dateTimeFormat = "yyyy-MM-dd/hh:mm:ss"; // constant

        /// <summary>
        /// 从加密文本解密出 License 对象。如果解密失败，返回null。
        /// </summary>
        /// <param name="execPath">license-app的可执行文件路径</param>
        /// <param name="key">键，不能超过32个字节，否则返回null</param>
        /// <param name="cipher_text">加密文本</param>
        public static License decryptFrom(String execPath, byte[] key, byte[] cipher_text)
        {
            StringBuilder keyStr = convertKeyToString(key);

            StringBuilder cipherStr = new StringBuilder("[");
            for (int i = 0; i < cipher_text.Length; i++)
            {
                cipherStr.Append(cipher_text[i]);
                cipherStr.Append(',');
            }
            cipherStr.Remove(cipherStr.Length - 1, 1);
            cipherStr.Append(']');

            String args = String.Format("dec {0} {1}", keyStr, cipherStr);

            int exitCode = 0;
            String resultStr = CmdUtils.RunCommand(execPath, args, ref exitCode);

            License result = null;
            if (exitCode == 0)
            {
                String[] fields = resultStr.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);


                if (fields.Length >= 4)
                {
                    UInt32 siteCount = UInt32.Parse(fields[0]);
                    DateTime expirationTime = DateTime.ParseExact(fields[1], dateTimeFormat, null);
                    result = new License(siteCount, expirationTime, fields[2], fields[3]);
                }
            }
            return result;
        }

        /// <summary>
        /// 生成给定的<code>License</code>对象的加密文本。如果加密失败，会返回null.
        /// 应当只在key不合规范时失败。
        /// </summary>
        /// <param name="execPath">license-app的可执行文件路径</param>
        /// <param name="key">键，不能超过32个字节</param>
        /// <param name="license">证书对象，不能为null</param>
        /// <returns></returns>
        public static byte[] encryptTo(String execPath, byte[] key, License license)
        {
            StringBuilder keyStr = convertKeyToString(key);

            StringBuilder licenseStr = new StringBuilder();
            licenseStr.Append(license.SiteCount);
            licenseStr.Append(' ');
            licenseStr.Append(license.ExpirationTime.ToString(dateTimeFormat));
            licenseStr.Append(' ');
            licenseStr.Append(license.CompanyName);
            licenseStr.Append(' ');
            licenseStr.Append(license.SoftwareName);

            String args = String.Format("enc {0} {1}", keyStr, licenseStr);

            int exitCode = 0;
            String resultStr = CmdUtils.RunCommand(execPath, args, ref exitCode);

            byte[] result = null;

            if (exitCode == 0 && resultStr != "")
            {
                string[] byteStrs = resultStr.Trim(new char[] { '[', ']', '\r', '\n', ' ' }).Split(',');
                //string[] byteStrs = resultStr.Substring(1, resultStr.Length - 2).Split(',');
                result = new byte[byteStrs.Length];
                for (int i = 0; i < byteStrs.Length; i++)
                {
                    result[i] = byte.Parse(byteStrs[i]);
                }
            }

            return result;
        }

        private static StringBuilder convertKeyToString(byte[] key)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < key.Length; i++)
            {
                result.Append((char)key[i]);
            }
            return result;
        }
    }
}
