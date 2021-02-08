using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSBackend.Util
{
    public class RandomIdGenerator
    {

        private static char[] _base62chars =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
                .ToCharArray();

        private static Random _random = new Random();

        private static string GetYearCode()
        {
            return _base62chars[(DateTime.Now.Year + 10) % 36] + "";
        }

        /// <summary>
        /// Generates a random string (all uppercase) of defined length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Generate(int length)
        {
            var sb = new StringBuilder(length);

            for (int i = 0; i < length - 1; i++)
                sb.Append(_base62chars[_random.Next(36)]);

            sb.Append(GetYearCode());

            return sb.ToString();
        }
    }
}
