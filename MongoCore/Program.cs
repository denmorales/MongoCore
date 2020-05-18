using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;





namespace MongoCore
{
    class Program
    {
        /// <summary>
        /// имена файлов, которые необходимо прочесть.
        /// </summary>
        private static string[] FileNames { get; } = { "books.json", "books.csv", "books.xml", "books.txt" };

        /// <summary>
        /// The logger.
        /// </summary>
        

        /// <summary>
        /// Gets the mongo collection name.
        /// </summary>
        private static string MongoCollectionName { get; } = "files";

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Start");

            try
            {
                var client = new MongoClient();
                var database = client.GetDatabase("local");
                var databaseFiles = database.GetCollection<BsonDocument>(MongoCollectionName);

                // очистим коллекцию
                long count = await databaseFiles.CountDocumentsAsync(new BsonDocument());
                if (count > 0)
                {
                    await databaseFiles.DeleteManyAsync(new BsonDocument());
                }

                foreach (string fileName in FileNames)
                {
                    string fileContent = await ReadFromFile(fileName);
                    Console.WriteLine($"Из файла {fileName} прочитано {fileContent.Length} знаков");

                    if (!string.IsNullOrWhiteSpace(fileContent))
                    {
                        string keyName = fileName.Replace(".", string.Empty);
                        BsonDocument fileDocument =
                            new BsonDocument(new Dictionary<string, object> { { keyName, fileContent } });
                        await databaseFiles.InsertOneAsync(fileDocument);
                        Console.WriteLine($"Файл {keyName} успешно добавлен");
                    }
                }

                var allDocumentsInCollection = await databaseFiles.FindAsync(new BsonDocument());
                await allDocumentsInCollection.ForEachAsync(document => { Console.WriteLine(document.ToString()); });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Finish 2");
        }

        /// <summary>
        /// The read from file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task<string> ReadFromFile(string fileName)
        {
            try
            {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(fileName))
                {
                    // Read the stream to a string, and write the string to the console.
                    string line = await sr.ReadToEndAsync();
                    Console.WriteLine(line);
                    return line;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return string.Empty;




        }
    }
    }
}
