using System.Text.RegularExpressions;

namespace OtpAgentForms.Helpers
{
    internal static class ImeiHelper
    {
        private static List<string> Tac;

        public static string GenerateRandomImei()
        {
            if (Tac == null || !Tac.Any())
            {
                return null;
            }
            var random = new Random();
            var tacIndex = random.Next(0, Tac.Count);
            var tac = Tac[tacIndex];
            var serial = GetRandomSerial();
            var checkSum = LuhnCheckSum($"{tac}{serial}");
            return $"{tac}{serial}{checkSum}";
        }

        private static string GetRandomSerial(int length = 6)
        {
            Random rnd = new();
            var ch = "0123456789";
            char[] letters = ch.ToCharArray();
            string s = "";
            for (int i = 0; i < length; i++)
            {
                s += letters[rnd.Next(letters.Length)].ToString();
            }
            return s;
        }

        private static long LuhnCheckSum(string n)
        {
            int[] numbers = n.Select(x => int.Parse(x.ToString())).ToArray();
            int len = numbers.Count();

            int digit = 0;
            int check_digit = 0;
            for (var i = len - 1; i >= 0; i--)
            {
                digit = numbers[i];
                if (i % 2 != 0)
                {
                    digit *= 2;

                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                check_digit += digit;
            }
            return 10 - check_digit % 10;
        }

        public static void ReadTac()
        {
            Tac = new();
            string path = "TAC";
            FileStream fileStream = new(path, FileMode.Open);
            StreamReader streamReader = new(fileStream, System.Text.Encoding.Default);
            try
            {
                string s = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(s))
                {
                    var tac = Regex.Matches(s, @"\d+")
                                        .Select(x => x.Value)
                                        .Where(x => x.Length == 8)
                                        .FirstOrDefault();
                    if (tac == null)
                    {
                        s = streamReader.ReadLine();
                        continue;
                    }
                    Tac.Add(tac);
                    s = streamReader.ReadLine();
                }
            }
            finally
            {
                streamReader.Close();
                fileStream.Close();
            }
        }
    }
}