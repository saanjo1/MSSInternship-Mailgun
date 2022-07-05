using MailguncApp;
using System;
using System.Collections.Generic;
using Azure.Data.Tables;
using Microsoft.Azure.Cosmos.Table;
using MailguncApp.Models;

namespace EmailConsoleApp
{
    class Program
    {

        private static string _tableName = Environment.GetEnvironmentVariable("TableName");
        private static int _sleepTime = Convert.ToInt32(Environment.GetEnvironmentVariable("SleepTime"));
        private static string _connectionString = Environment.GetEnvironmentVariable("AzureTableStorage");
        private static CloudTableClient _tableClient;
        private static CloudTable _table;


        static void Main(string[] args)
        {
            Console.WriteLine("Hello from Warranty checking service....");
            var entities = new List<WarrantyTableEntity>();
            var alerts = new List<string>();

            do
            {
                try
                {
                    Console.WriteLine("Entering try catch, initializing storage . . .");
                    CloudStorageAccount storageAccount =
                    CloudStorageAccount.Parse(_connectionString);
                    _tableClient = storageAccount.CreateCloudTableClient();
                    _table = _tableClient.GetTableReference(_tableName);
                    _table.CreateIfNotExists();

                    Console.WriteLine("Initialized table in storage.");


                    Console.WriteLine("Setting query to retrieve data . . . ");
                    var query = new TableQuery<WarrantyTableEntity>();
                    TableContinuationToken continuationToken = null;
                    do
                    {
                        var page = _table.ExecuteQuerySegmented(query, continuationToken);
                        continuationToken = page.ContinuationToken;
                        entities.AddRange(page.Results);
                    }
                    while (continuationToken != null);

                    Console.WriteLine("Query executed, entities collected.");

                    foreach (var item in entities)
                    {
                        if ((DateTime.Now - item.Duration).TotalDays < 30 && item.PossibilityToExtend == true)
                        {
                            Console.WriteLine("Added email to notify . . .");
                            alerts.Add(item.ContactMail);
                        }
                    }

                    Console.Write("Emails ready to use.");



                    Console.WriteLine("Collecting addidtional information to send email . . . ");

                    UserEmailOptions userEmailOptions = new UserEmailOptions()
                    {
                        Body = "Warranty of your device is expiring in 30 days. Please contact us.",
                        Subject = "Warranty expiration alert",
                        ToEmails = alerts
                    };

                    Console.WriteLine("Information collected, trying to send mails . . .");

                    var result = EmailService.SendEmail(userEmailOptions);

                    Console.WriteLine("Emails successfully sent.");


                    Thread.Sleep(TimeSpan.FromSeconds(_sleepTime));

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    throw ex;
                } 
            } while (true);
        }
    }
}