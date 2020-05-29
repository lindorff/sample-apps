using System;

namespace InvoiceApiClientSample
{
    class InvoiceApiClientSample
    {
        static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine("InvoiceApiClientSample needs 5 arguments: ");
                Console.WriteLine("1. access token url");
                Console.WriteLine("2. client id");
                Console.WriteLine("3. client secret");
                Console.WriteLine("4. invoice api hostname");
                Console.WriteLine("5. customer id (to get invoices from)");
            }
            else
            {
                try
                {
                    var client = new InvoiceApiClient.InvoiceApiClient(args[0], args[1], args[2], "https://" + args[3]);
                    Console.WriteLine(client.GetInvoices(args[4])?.ToString());
                }
                catch (Exception e)
                {
                    Console.Write("Exception occurred: " + e.Message);
                }
                
            }
        }
    }
}
