using System;

namespace InvoiceApiClientSample
{
    class InvoiceApiClientSample
    {

        public static readonly string apiHost = "https://api.lowell.com";
        public static readonly string apiBasePath = "fin/qa/invoice/ledger";
        public static readonly string tokenEndpoint = "https://idp.lowell.io/auth/realms/test-clients/protocol/openid-connect/token";

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("InvoiceApiClientSample needs 3 arguments: ");
                Console.WriteLine("1. client id");
                Console.WriteLine("2. client secret");
                Console.WriteLine("3. customer id (to get invoices from)");
            }
            else
            {
                try
                {
                    var client = new InvoiceApiClient.InvoiceApiClient(tokenEndpoint, args[0], args[1], apiHost);
                    var customerId = args[2]?.ToString();

                    var resource = $"{apiBasePath}/v1/customers/{customerId}/invoices";

                    Console.WriteLine(client.GetInvoices(resource));
                }
                catch (Exception e)
                {
                    Console.Write("Exception occurred: " + e.Message);
                }
                
            }
        }
    }
}
