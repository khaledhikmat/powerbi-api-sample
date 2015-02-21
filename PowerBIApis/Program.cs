using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using PowerBIApis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIApis
{
    public class Program
    {
        // PowerBI Stuff
        // NOTE: Never store keys in source code! In a production environment, please 
        // use a configuraion manager

        // Native Client
        private static readonly string PowerBIClientID = "<your client id goes here>";
        private static readonly string PowerBIClientRedirectUrl = "<your client redirect URL goes here>";

        // PowerBI Service
        private static string PowerBIAuthority = "https://login.windows.net/common/oauth2/authorize";
        private static string PowerBIResourceUri = "https://analysis.windows.net/powerbi/api";
        
        // Test Dataset names
        private static string PowerBIDatasetName = "Acme";

        public static void Main(string[] args)
        {
            try
            {
                // Get all existing data sets
                var dsTasks = GetPowerBIDataSets();
                Task.WaitAll(new Task[] { dsTasks }, 60000);
                DatasetsList datasetsList = dsTasks.Result;

                // Create a new data set
                Dataset ds = new Dataset();
                ds.Name = PowerBIDatasetName;
                ds.Tables = new List<Table>() {
                    new Table() {Name = "Sales", Columns = new List<Column> () {
                        new Column() {Name = "TransactionDate", DataType = "DateTime" },
                        new Column() {Name = "Pens", DataType = "Int64" },
                        new Column() {Name = "Books", DataType = "Int64" },
                        new Column() {Name = "Calculators", DataType = "Int64" },
                    }},
                    new Table() {Name = "Table Bookings", Columns = new List<Column> () {
                        new Column() {Name = "TransactionDate", DataType = "DateTime" },
                        new Column() {Name = "Bookings", DataType = "Int64" },
                        new Column() {Name = "Canx", DataType = "Int64" },
                        new Column() {Name = "Confirmations", DataType = "Int64" },
                        new Column() {Name = "NoShows", DataType = "Int64" },
                    }}
                };

                CreatePowerBIDataSet(ds).Wait();

                // Re-get all existing data sets
                dsTasks = GetPowerBIDataSets();
                Task.WaitAll(new Task[] { dsTasks }, 60000);
                datasetsList = dsTasks.Result;

                // Add rows to the new dataset
                Dataset acmeDs = datasetsList.Datasets.Where(d => d.Name == PowerBIDatasetName).FirstOrDefault();
                if (acmeDs != null)
                {
                    // I wish the dataset collection included the tables!!
                    //Table table = acmeDs.Tables.Where(t => t.Name == "Sales").FirstOrDefault();
                    //if (table != null)
                    {
                        TableRows<Sales> sales = new TableRows<Sales>();
                        sales.Rows.Add(new Sales() { TransactionDate = DateTime.Now, Pens = 1, Books = 0, Calculators = 0 });
                        sales.Rows.Add(new Sales() { TransactionDate = DateTime.Now.AddHours(-1), Pens = 1, Books = 1, Calculators = 2 });
                        sales.Rows.Add(new Sales() { TransactionDate = DateTime.Now.AddHours(-3), Pens = 0, Books = 3, Calculators = 1 });
                        sales.Rows.Add(new Sales() { TransactionDate = DateTime.Now.AddHours(-5), Pens = 4, Books = 2, Calculators = 2 });

                        CreatePowerBITableRows(acmeDs.Id, "Sales", sales).Wait();
                    }
                }
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        /*
         * NOTE: the code below is not a demonstration of best practices in HTTP coding! 
         * It is meant to show the simplest way to interact with the PowerBI APIs
         */

        private static async Task<DatasetsList> GetPowerBIDataSets()
        {
            DateTime now = DateTime.Now;
            HttpClient client = null;
            DatasetsList list = new DatasetsList();

            try
            {
                AuthenticationContext authContext = new AuthenticationContext(PowerBIAuthority);
                string token = authContext.AcquireToken(PowerBIResourceUri, PowerBIClientID, new Uri(PowerBIClientRedirectUrl)).AccessToken.ToString();

                client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("https://api.powerbi.com/beta/myorg/datasets");

                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Datasets JSON: " + responseString);

                list = JsonConvert.DeserializeObject<DatasetsList>(responseString);
                foreach (Dataset dataset in list.Datasets)
                {
                    Console.WriteLine("Id: {0} - Name: {1}", dataset.Id, dataset.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetPowerBIDataSets - exception: " + ex.Message);
                Console.WriteLine("GetPowerBIDataSets - stack trace: " + ex.StackTrace);
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }

            return list;
        }

        private static async Task CreatePowerBIDataSet(Dataset dataset)
        {
            DateTime now = DateTime.Now;
            HttpClient client = null;

            try
            {
                AuthenticationContext authContext = new AuthenticationContext(PowerBIAuthority);
                string token = authContext.AcquireToken(PowerBIResourceUri, PowerBIClientID, new Uri(PowerBIClientRedirectUrl)).AccessToken.ToString();

                client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var postData = JsonConvert.SerializeObject(dataset,
                                                      new JsonSerializerSettings()
                                                      {
                                                          NullValueHandling = NullValueHandling.Ignore
                                                      });
                HttpContent httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://api.powerbi.com/beta/myorg/datasets", httpContent);

                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Dataset creation JSON: " + responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreatePowerBIDataSet - exception: " + ex.Message);
                Console.WriteLine("CreatePowerBIDataSet - stack trace: " + ex.StackTrace);
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        private static async Task CreatePowerBITableRows(string datasetId, string tableName, TableRows<Sales> rows)
        {
            DateTime now = DateTime.Now;
            HttpClient client = null;

            try
            {
                AuthenticationContext authContext = new AuthenticationContext(PowerBIAuthority);
                string token = authContext.AcquireToken(PowerBIResourceUri, PowerBIClientID, new Uri(PowerBIClientRedirectUrl)).AccessToken.ToString();

                client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var postData = JsonConvert.SerializeObject(rows,
                                                      new JsonSerializerSettings()
                                                      {
                                                          NullValueHandling = NullValueHandling.Ignore
                                                      });
                HttpContent httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://api.powerbi.com/beta/myorg/datasets/" + datasetId+ "/tables/" + tableName + "/rows", httpContent);

                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Rows creation JSON: " + responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreatePowerBITableRows - exception: " + ex.Message);
                Console.WriteLine("CreatePowerBITableRows - stack trace: " + ex.StackTrace);
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }
    }
}
