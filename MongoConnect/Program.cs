using MongoDB.Driver;
using System;

namespace MongoConnect
{
    public class Program
    { 
        static void Main(string[] args)
        {
            var connectionString = "mongodb://root:admin@localhost:27017";
            
            var database = "testDb";

            var client = new MongoClient(connectionString);
            
            var db = client.GetDatabase(database);
             
            var collection = db.GetCollection<Book>("book");

            var documents = collection.CountDocuments(FilterDefinition<Book>.Empty);
            
            Console.Write($"number of documents - {documents}");

            //var book = new Book
            //{
            //    Author = "Stephen King",
            //    Title = "The Shining",
            //    Year = 1977
            //};

            //collection.InsertOne(book);

            //Console.WriteLine($"{book.Title} inserted");    

        }
    }

    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
    }
}
