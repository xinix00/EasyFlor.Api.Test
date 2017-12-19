using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Api.Test
{
    public enum WebMethod
    {
        GET,
        POST,
        PUT
    }

    public class AvgTimer
    {
        private List<long> _times;

        public class StopwatchWrapper
        {
            private AvgTimer _timer;
            private Stopwatch _stopwatch;

            public StopwatchWrapper(AvgTimer timer)
            {
                _timer = timer;
                _stopwatch = new Stopwatch();
            }

            public void Start()
            {
                _stopwatch.Start();
            }
            public void Stop()
            {
                _timer.Stop(_stopwatch.ElapsedMilliseconds);
                _stopwatch.Stop();

            }
        }

        public AvgTimer()
        {
            _times = new List<long>();
        }

        public List<long> GetTimes()
        {
            return _times;
        }

        public StopwatchWrapper StartNew()
        {
            var stopwatch = new StopwatchWrapper(this);
            stopwatch.Start();
            return stopwatch;
        }

        internal void Stop(long time)
        {
            _times.Add(time);
        }

    }

    public class ApiTester
        : IDisposable
    {
        private CookieContainer _container;
        private string _baseUrl = "<< API URL HERE >>";
        static string Date = DateTime.Now.ToString("yyyy-MM-dd");

        public ApiTester()
        {
            _container = new CookieContainer();
        }

        public void OneTimeTest(string database, string userName, string password)
        {
            using (var logger = new Logger("OneTime"))
            {
                this.Request(logger, null, "Authorization/", WebMethod.PUT, new ApiAuthorize()
                {
                    Username = userName,
                    Password = password,
                    Database = database
                });

                // Purchases
                this.Request(logger, "OneTime/Purchases", "Purchase/", WebMethod.GET, null);

                // Financial
                this.Request(logger, "OneTime/CreditInvoiceCustomItems", "CreditInvoiceCustomItem/", WebMethod.GET, null);
                this.Request(logger, "OneTime/CreditInvoiceDepositItems", "CreditInvoiceDepositItem/", WebMethod.GET, null);
                this.Request(logger, "OneTime/CreditInvoicePurchaseItems", "CreditInvoicePurchaseItem/", WebMethod.GET, null);

                this.Request(logger, "OneTime/DebetInvoiceCustomItems", "DebetInvoiceCustomItem/", WebMethod.GET, null);
                this.Request(logger, "OneTime/DebetInvoicePackagingItems", "DebetInvoicePackagingItem/", WebMethod.GET, null);
                this.Request(logger, "OneTime/DebetInvoicePurchaseItems", "DebetInvoicePurchaseItem/", WebMethod.GET, null);
                this.Request(logger, "OneTime/DebetInvoiceTrolleyItems", "DebetInvoiceTrolleyItem/", WebMethod.GET, null);

                this.Request(logger, "OneTime/PackagingInDebetInvoicePurchaseItems", "PackagingInDebetInvoicePurchaseItem/", WebMethod.GET, null);
                this.Request(logger, "OneTime/Payments", "Payment/", WebMethod.GET, null);


            }
        }

        public void DailyTest(string database, string userName, string password)
        {
            using (var logger = new Logger("Daily"))
            {
                this.Request(logger, null, "Authorization/", WebMethod.PUT, new ApiAuthorize()
                {
                    Username = userName,
                    Password = password,
                    Database = database
                });

                // Simple sets
                this.Request(logger, $"Daily/{Date}/ArticleGroups", "Articlegroup/", WebMethod.GET, null);
                this.Request(logger, $"Daily/{Date}/Countries", "Country/", WebMethod.GET, null);
                this.Request(logger, $"Daily/{Date}/Currencies", "Currency/", WebMethod.GET, null);
                this.Request(logger, $"Daily/{Date}/Remarks", "Remark/", WebMethod.GET, null);

                this.Request(logger, $"Daily/{Date}/Employee", "Employee/", WebMethod.GET, null);
                this.Request(logger, $"Daily/{Date}/Suppliers", "Supplier/", WebMethod.GET, null);

                this.Request(logger, $"Daily/{Date}/ListGroups", "ListGroup/", WebMethod.GET, null);

                // Slower sets
                this.Request(logger, $"Daily/{Date}/Debtors", "Debtor/", WebMethod.GET, null);
                this.Request(logger, $"Daily/{Date}/Articles", "Article/", WebMethod.GET, null);
                this.Request(logger, $"Daily/{Date}/Lists", "List/", WebMethod.GET, null);
            }
        }

        public void IncrementalTest(string database, string userName, string password)
        {
            using (var logger = new Logger("Incremental"))
            {
                this.Request(logger, null, "Authorization/", WebMethod.PUT, new ApiAuthorize()
                {
                    Username = userName,
                    Password = password,
                    Database = database
                });

                var date = DateTime.Now.AddDays(-2).ToString("yyyyMMdd");

                this.Request(logger, $"Incremental/{Date}/CreditInvoiceCustomItemSinceDate", "CreditInvoiceCustomItemSinceDate/" + date, WebMethod.GET, null);
                this.Request(logger, $"Incremental/{Date}/CreditInvoiceDepositItemSinceDate", "CreditInvoiceDepositItemSinceDate/" + date, WebMethod.GET, null);
                this.Request(logger, $"Incremental/{Date}/CreditInvoicePurchaseItemSinceDate", "CreditInvoicePurchaseItemSinceDate/" + date, WebMethod.GET, null);

                this.Request(logger, $"Incremental/{Date}/DebetInvoiceCustomItemSinceDate", "DebetInvoiceCustomItemSinceDate/" + date, WebMethod.GET, null);
                this.Request(logger, $"Incremental/{Date}/DebetInvoicePackagingItemSinceDate", "DebetInvoicePackagingItemSinceDate/" + date, WebMethod.GET, null);
                this.Request(logger, $"Incremental/{Date}/DebetInvoicePurchaseItemSinceDate", "DebetInvoicePurchaseItemSinceDate/" + date, WebMethod.GET, null);
                this.Request(logger, $"Incremental/{Date}/DebetInvoiceTrolleyItemSinceDate", "DebetInvoiceTrolleyItemSinceDate/" + date, WebMethod.GET, null);

                this.Request(logger, $"Incremental/{Date}/InvoiceSinceDate", "InvoiceSinceDate/" + date, WebMethod.GET, null);
                this.Request(logger, $"Incremental/{Date}/PaymentSinceDate", "PaymentSinceDate/" + date, WebMethod.GET, null);
                this.Request(logger, $"Incremental/{Date}/PurchaseSinceDate", "PurchaseSinceDate/" + date, WebMethod.GET, null);

                this.Request(logger, $"Incremental/{Date}/PackagingInDebetInvoicePurchaseItemSinceDate", "PackagingInDebetInvoicePurchaseItemSinceDate/" + date, WebMethod.GET, null);
            }
        }

        public void SpeedTest(string database, string userName, string password)
        {
            var emptyLogger = new EmptyLogger();
            using (var logger = new Logger("Speedtest"))
            {
                // Authorize
                this.Request(emptyLogger, null, "Authorization/", WebMethod.PUT, new ApiAuthorize()
                {
                    Username = userName,
                    Password = password,
                    Database = database
                });

                logger.WriteLine("Running a speedtest.");
                logger.WriteLine("1. Loading items via PurchaseSinceDate");

                // - Requesting purchases of today
                this.Request(emptyLogger, $"Temp/Purchases", $"PurchaseSinceDate/{DateTime.Now.ToString("yyyyMMdd")}", WebMethod.GET, null);

                // Reading purchase Id's from request
                var retrievedFile = string.Join("\r\n", File.ReadAllLines(@"Temp/Purchases.json"));
                var purchaseIds = JArray.Parse(retrievedFile)
                    .Where(a => a.Type == JTokenType.Object)
                    .Select(a => ((JObject)a).Value<string>("Id"))
                    .ToList();

                logger.WriteLine($" - Retrieved {purchaseIds.Count} items");

                // Fetching ListArticleSort items:
                {
                    var items = Math.Min(purchaseIds.Count, 50);
                    logger.WriteLine($"2. Fetching {items} items, 100 times, using /ListArticleSort to get complete information of {items} items (name, description etc):");
                    var avgTimer = new AvgTimer();
                    for (var x = 0; x < 100; x++)
                    {
                        var timer = avgTimer.StartNew();
                        this.Request(emptyLogger, null, "ListArticleSort", WebMethod.PUT, new ApiListArticleSort()
                        {
                            ListArticleSorts = purchaseIds.Take(items)
                        });
                        timer.Stop();
                        Console.Write(".");
                    }
                    Console.WriteLine();

                    var times = avgTimer.GetTimes();
                    logger.WriteLine($" - avg: {times.Average()}ms, max: {times.Max()}ms, min: {times.Min()}ms");
                }

                // Fetching DebtorListArticleSortPrice items:
                {
                    var items = Math.Min(purchaseIds.Count, 25);
                    logger.WriteLine($"3. Fetching {items} items, 100 times, using /DebtorListArticleSortPrice to get stock/price information:");
                    var avgTimer = new AvgTimer();
                    for (var x = 0; x < 100; x++)
                    {
                        var timer = avgTimer.StartNew();
                        this.Request(emptyLogger, null, "DebtorListArticleSortPrice", WebMethod.PUT, new ApiDebtorListArticleSortPrice()
                        {
                            DebtorId = 6340, // 000723
                            ListArticleSorts = purchaseIds.Take(items)
                        });
                        timer.Stop();
                        Console.Write(".");
                    }
                    Console.WriteLine();

                    var times = avgTimer.GetTimes();
                    logger.WriteLine($" - avg: {times.Average()}ms, max: {times.Max()}ms, min: {times.Min()}ms");
                }

            }
        }

        private void Request(ILogger logger, string name, string path, WebMethod method, object payload)
        {
            var fullUrl = _baseUrl + path;

            // - Debug information
            var payloadToJson = payload != null ? JsonConvert.SerializeObject(payload) : null;

            var stopwatch = new Stopwatch();
            logger.WriteLine("");
            logger.WriteLine("---------------- new request ----------------");
            logger.WriteLine($"Request: {fullUrl}");
            logger.WriteLine($"Method:  {method}");
            if (!string.IsNullOrEmpty(payloadToJson))
            {
                logger.WriteLine("------------------ payload ------------------");
                logger.WriteLine(payloadToJson);
                logger.WriteLine("---------------------------------------------");
            }
            else
            {
                logger.WriteLine("Without payload");
            }

            stopwatch.Start();
            // - End debug information

            // Create a WebRequest (JSON, with method)
            var request = (HttpWebRequest)WebRequest.Create(fullUrl);
            request.Method = method.ToString();
            request.CookieContainer = _container;
            request.Timeout = int.MaxValue;
            request.ReadWriteTimeout = int.MaxValue;
            request.ContinueTimeout = int.MaxValue;

            try
            {
                // Write response to stream
                if (!string.IsNullOrEmpty(payloadToJson))
                {
                    request.ContentType = "application/json; charset=UTF-8";
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(payloadToJson);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                // Receive initial headers:
                var response = (HttpWebResponse)request.GetResponse();

                // - Debug information
                logger.WriteLine($"Statuscode:      {((int)response.StatusCode)}");
                logger.WriteLine($"Elapsed time:    {stopwatch.Elapsed}");
                // - End debug information

                // Write to file when having a name
                if (!string.IsNullOrEmpty(name))
                {
                    logger.WriteLine($"-- Writing response to file: {name}.json --");

                    // Create directory
                    Directory.CreateDirectory(Path.GetDirectoryName(name));

                    using (var file = File.Create($"{name}.json"))
                    {
                        response.GetResponseStream().CopyTo(file);
                        file.Flush();
                        file.Close();
                    }
                }

                // - Debug information
                stopwatch.Stop();
                logger.WriteLine($"End time:        {stopwatch.Elapsed}");
                logger.WriteLine($"Bytes received:  {response.ContentLength}");
                // - End debug information
                response.Close();
            }
            catch (Exception e)
            {
                logger.WriteLine(e.ToString());
            }
        }

        private void Unauthorize()
        {
            this.Request(new ConsoleLogger(), null, "Unauthorization", WebMethod.PUT, null);
        }

        public void Dispose()
        {
            // To test if no unauth will cause issues
            // when done by supplier, we also don't do a unauth

            //this.Unauthorize();
        }
    }
}
