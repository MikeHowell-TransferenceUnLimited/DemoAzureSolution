using System;
using System.Collections.Immutable;
using System.Xml.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using System.Net;


namespace CosmosDBSQL
{
    class Program
    {
        private const string EndpointUri = "https://mchcosmosdb-sql.documents.azure.com:443";
        private const string Key = "8DmbzvPkofTz2N8ssRPRvdY4sz9YNziBC6zWxHiEtj8VlmN4OANC3Ui8jNWCSgZ3J0HPRtNPFmQUEKL7kPa2Kg==";
        private CosmosClient client;
        private Database database;
        private Container container;

        static void Main(string[] args)
        {
            try
            {
                Program demo = new Program();
                demo.StartDemo().Wait();
            }
            catch (CosmosException ce)
            {
                Exception baseException = ce.GetBaseException();
                System.Console.WriteLine($"{ce.StatusCode} error occured: {ce.Message}, Message: {baseException.Message}");
            }
            catch (Exception ex)
            {
                Exception baseException = ex.GetBaseException();
                Console.WriteLine($"Error ocurred: {ex.Message}, Message: {baseException.Message}");
            }
        }

        private async Task StartDemo()
        {
            Console.WriteLine("Starting Cosmos DB SQL API Demo!");

            string databaseName = "demoDB_" + Guid.NewGuid().ToString().Substring(0, 5);
            this.SendMessageToConsoleAndWait($"Creating database {databaseName}...");

            this.client = new CosmosClient(EndpointUri, Key);
            this.database = await this.client.CreateDatabaseIfNotExistsAsync(databaseName);

            string containerName = "collection_" + Guid.NewGuid().ToString().Substring(0, 5);

            this.SendMessageToConsoleAndWait($"Creating collection demo {containerName}...");

            this.container = await this.database.CreateContainerIfNotExistsAsync(containerName, "/LastName");

            Person person1 = new Person
            {
                Id = "Person.1",
                FirstName = "Santiago",
                LastName = "Fernandez",
                Devices = new Device[]
                {
                    new Device {OperatingSystem ="iOS", CameraMegaPixels=7,Ram=16,Usage="Personal"},
                    new Device {OperatingSystem = "Android", CameraMegaPixels = 12, Ram = 64, Usage = "Work"}
                },
                Gender = "Male",
                Address = new Address
                {
                    City = "Seville",
                    Country = "Spain",
                    PostalCode = "28973",
                    Street = "Diagonal",
                    State = "Andalucia"
                },
                IsRegistered = true
            };

            await this.CreateDocumentIfNotExistsAsync(databaseName, containerName, person1);

            Person person2 = new Person
            {
                Id = "Person.2",
                FirstName = "Agatha",
                LastName = "Smith",
                Devices = new Device[]
               {
                    new Device {OperatingSystem ="iOS", CameraMegaPixels=12,Ram=32,Usage="Work"},
                    new Device {OperatingSystem = "Windows", CameraMegaPixels = 12, Ram = 64, Usage = "Personal"}
               },
                Gender = "Female",
                Address = new Address
                {
                    City = "Laguna Beach",
                    Country = "United States",
                    PostalCode = "12345",
                    Street = "Main",
                    State = "CA"
                },
                IsRegistered = true
            };

            await this.CreateDocumentIfNotExistsAsync(databaseName, containerName, person2);

            this.SendMessageToConsoleAndWait($"Getting documents from the collection {containerName}...");

            IQueryable<Person> queryablePeople = this.container.GetItemLinqQueryable<Person>(true).Where(p => p.Gender == "Male");

            Console.WriteLine("Running LINQ query for finding men...");
            
            foreach (Person foundPerson in queryablePeople)
            {
                Console.WriteLine($"\tPerson: {foundPerson}");
            }

            // Both the property name and value are case sensitive.
            var sqlQuery = "select * from person where person.Gender='Female'";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
            FeedIterator<Person> peopleResultSetIterator = this.container.GetItemQueryIterator<Person>(queryDefinition);

            Console.WriteLine("Running SQL query for finding women...");
            while (peopleResultSetIterator.HasMoreResults)
            {
                FeedResponse<Person> currentResultSet = await peopleResultSetIterator.ReadNextAsync();
                foreach (Person foundPerson in currentResultSet)
                {
                    Console.WriteLine($"\tPerson: {foundPerson}");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            this.SendMessageToConsoleAndWait($"Updating documents in the collection {containerName}...");

            person2.FirstName = "Mathew";
            person2.Gender = "Male";

            await this.container.UpsertItemAsync(person2);
            this.SendMessageToConsoleAndWait($"Document modified {person2}");

            PartitionKey partitionKey = new PartitionKey(person1.LastName);
            await this.container.DeleteItemAsync<Person>(person1.Id, partitionKey);
            this.SendMessageToConsoleAndWait($"Document deleted {person1}");

            this.SendMessageToConsoleAndWait("Cleaning-up your Cosmos DB account...");
            await this.database.DeleteAsync();
        }

        private void SendMessageToConsoleAndWait(string msg)
        {
            Console.WriteLine(msg);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        private async Task CreateDocumentIfNotExistsAsync(string databsae, string collection, Person person)
        {
            try
            {
                await this?.container.ReadItemAsync<Person>(person.Id, new PartitionKey(person.LastName));
                this.SendMessageToConsoleAndWait($"Document {person.Id} already exists in collection {collection}");
            }
            catch (CosmosException dce)
            {
                if (dce.StatusCode == HttpStatusCode.NotFound)
                {
                    await this?.container.CreateItemAsync<Person>(person, new PartitionKey(person.LastName));

                    this.SendMessageToConsoleAndWait($"Created new document {person.Id} in collection {collection}");
                }
            }
        }
    }
}

