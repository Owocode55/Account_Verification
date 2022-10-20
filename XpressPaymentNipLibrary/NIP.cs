using nfp.ssm.core;
using System;

namespace XpressPaymentNipLibrary
{
    public class NIP
    {
        public static void GenerateKeyPairs(string UserName, string SecreteKey, string PrivateKey, string PublicKey)
        {
            SSMLib ssmlib = new SSMLib(PublicKey, PrivateKey);

            ssmlib.generateKeyPair(UserName, SecreteKey);
        }

        public static string EncryptData(string data, string privateKey, string publicKey)
        {
            try
            {
                SSMLib ssmLibery = new SSMLib(publicKey, privateKey);
                var resp = ssmLibery.encryptMessage(data);
                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string DecryptData(string data, string privateKey, string publicKey, string secretekey)
        {
            try
            {
                SSMLib ssmLibery = new SSMLib(publicKey, privateKey);
                var resp = ssmLibery.decryptFile(data, secretekey);
                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
