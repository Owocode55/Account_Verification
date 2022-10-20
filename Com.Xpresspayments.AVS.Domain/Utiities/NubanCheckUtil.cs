using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Common.Utiities
{
    public static class NubanCheckUtil
    {
        public static bool isNubanAccount(string accountNumber, string bankCode)
        {
            string seed = "373373373373"; // this is a constant
            string cipher = string.Empty;
            sbyte check = 0;
            int calfig = 0;
            bool isNuban = false;
            if (accountNumber.Length == 10)
            {
                try
                {
                    cipher = bankCode + accountNumber.Substring(0, accountNumber.Length - 1);
                    for (int ex = 0; ex < seed.Length; ++ex)
                    {
                        calfig += int.Parse(string.Empty + seed[ex]) * int.Parse(string.Empty + cipher[ex]);
                    }
                    calfig %= 10;
                    calfig = 10 - calfig;
                    if (calfig == 10)
                    {
                        calfig = 0;
                    }



                    if ((calfig + string.Empty).Equals((accountNumber.Substring(accountNumber.Length - 1, accountNumber.Length))))
                    {
                        check = 1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.Write(ex.StackTrace);
                    //log.error("====> {}", ex.Message);
                    //throw new NubanCheckException(AvsResponseCode.NUBAN_CHECK.Description);
                }
            }
            else
            {
                check = 0;
            }



            return check == 1;
        }

        public static bool isNuban(string bankCode, string accountNo)
        {
            bool isnuban = false;
            try
            {
                bankCode = bankCode.Length > 3 ? bankCode.Substring(bankCode.Length - 3) : bankCode;

                var nChecker = new int[] { 3, 7, 3, 3, 7, 3, 3, 7, 3, 3, 7, 3 };
                if (accountNo.Length != 10)
                    return false;

                var accarray = accountNo.ToCharArray();
                var bcarray = bankCode.ToCharArray();

                int sum = (int)Char.GetNumericValue(bcarray[0]) * nChecker[0] + (int)Char.GetNumericValue(bcarray[1]) * nChecker[1] + (int)Char.GetNumericValue(bcarray[2]) * nChecker[2] + (int)Char.GetNumericValue(accarray[0]) * nChecker[3]
                         + (int)Char.GetNumericValue(accarray[1]) * nChecker[4] + (int)Char.GetNumericValue(accarray[2]) * nChecker[5] + (int)Char.GetNumericValue(accarray[3]) * nChecker[6] + (int)Char.GetNumericValue(accarray[4]) * nChecker[7]
                         + (int)Char.GetNumericValue(accarray[5]) * nChecker[8] + (int)Char.GetNumericValue(accarray[6]) * nChecker[9] + (int)Char.GetNumericValue(accarray[7]) * nChecker[10] + (int)Char.GetNumericValue(accarray[8]) * nChecker[11];

                int reminder = sum % 10;
                var lastDigit = accountNo.Substring(accountNo.Length - 1);
                if (reminder == 0)
                {
                    isnuban = lastDigit == reminder.ToString() ? true : false;
                    return isnuban;
                }

                isnuban = (10 - reminder).ToString() == lastDigit ? true : false;
            }
            catch { }


            return isnuban;
        }
    }
}
