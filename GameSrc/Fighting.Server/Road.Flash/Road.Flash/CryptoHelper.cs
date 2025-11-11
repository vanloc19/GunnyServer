using System;
using System.Security.Cryptography;
using System.Text;

namespace Road.Flash
{
	public class CryptoHelper
	{
		public static RSACryptoServiceProvider GetRSACrypto(string privateKey)
		{
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters
			{
				Flags = CspProviderFlags.UseMachineKeyStore
			});
			rSACryptoServiceProvider.FromXmlString(privateKey);
			return rSACryptoServiceProvider;
		}

		public static string RsaDecrypt(string privateKey, string src)
		{
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters
			{
				Flags = CspProviderFlags.UseMachineKeyStore
			});
			rSACryptoServiceProvider.FromXmlString(privateKey);
			return RsaDecrypt(rSACryptoServiceProvider, src);
		}

		public static string RsaDecrypt(RSACryptoServiceProvider rsa, string src)
		{
			byte[] srcData = Convert.FromBase64String(src);
			byte[] destData = rsa.Decrypt(srcData, fOAEP: false);
			return Encoding.UTF8.GetString(destData);
		}

		public static byte[] RsaDecryt2(RSACryptoServiceProvider rsa, string src)
		{
			byte[] srcData = Convert.FromBase64String(src);
			return rsa.Decrypt(srcData, fOAEP: false);
		}
	}
}