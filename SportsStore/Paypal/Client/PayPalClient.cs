using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using PayPalCheckoutSdk.Core;
using SportsStore.Paypal.Configuration;

namespace SportsStore.Paypal.Client
{
    public class PayPalClient
    {
        static string clientId;
        static string clientSecret;

        public static PayPalEnvironment Environment()
        {
            return new SandboxEnvironment(clientId, clientSecret);
        }

        public static PayPalHttpClient Client(PayPalOptions options)
        {
            clientId = options.PayPalSandboxClientId;
            clientSecret = options.PayPalSandboxClientSecret;
            return new PayPalHttpClient(Environment());
        }

        public static PayPalHttpClient Client(string refreshToken)
        {
            return new PayPalHttpClient(Environment(), refreshToken);
        }

        internal static object client()
        {
            throw new NotImplementedException();
        }

        public static String ObjectToJSON(object serializableObject)
        {
            MemoryStream memoryStream = new MemoryStream();
            var writer = JsonReaderWriterFactory.CreateJsonWriter(memoryStream, Encoding.UTF8, true, true, "  ");

            var ser = new DataContractJsonSerializer(serializableObject.GetType(),
                new DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                });

            ser.WriteObject(writer, serializableObject);

            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);

            return sr.ReadToEnd();
        }
    }
}
