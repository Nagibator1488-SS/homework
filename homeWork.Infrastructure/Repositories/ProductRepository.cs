using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetHomeWork.Infrastructure.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Vostok.Logging.Abstractions;

namespace DotNetHomeWork.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private const string CollectionName = "Products";
        private const string DataBaseName = "DotNetHomeWork";
        private readonly IMongoClient mongoClient;
        private readonly ILog log;

        public ProductRepository(IMongoClient mongoClient, ILog log)
        {
            this.mongoClient = mongoClient;
            this.log = log;
        }

        public void EnsureCollectionExists()
        {
            var database = mongoClient.GetDatabase(DataBaseName);
            var collectionExists = CollectionExists(database);
            if (!collectionExists)
            {
                var errorMessage = $"Collection {CollectionName} does not exist";
                log.Error(errorMessage);
                throw new Exception(errorMessage);
            }

            CreateOrgIdIndexIfNecessary();
        }

        public async Task<ProductStored> AddOrUpdateProductAsync(ProductAddModel product)
        {
            var collection = GetCollection();
            try
            {
                var update = BuildUpdateDefinition(product);
                await collection.UpdateOneAsync(
                        p => p.Name == product.Name,
                        update,
                        new UpdateOptions { IsUpsert = true })
                    .ConfigureAwait(false);
                return new ProductStored();
            }
            catch (Exception exception)
            {
                log.Error(exception);
                throw;
            }
        }

        public async Task DeleteProductAsync(ProductDeleteModel product)
        {
            var collection = GetCollection();
            try
            {
                await collection.DeleteOneAsync(
                        p => p.Name == product.Name)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                throw;
            }
        }

        public async Task<ProductStored> GetProductAsync(string name)
        {
            var collection = GetCollection();
            try
            {
                var product = await collection
                    .Find(p => p.Name == name)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                
                return product;
            }
            catch (Exception exception)
            {
                log.Error(exception);
                throw;
            }
        }

        private static UpdateDefinition<ProductStored> BuildUpdateDefinition(ProductAddModel productAddModel)
        {
            return Builders<ProductStored>.Update
                .Set(p => p.Name, productAddModel.Name)
                .Set(p => p.Price, productAddModel.Price);
        }

        private bool CollectionExists(IMongoDatabase db)
        {
            var filter = new BsonDocument("name", CollectionName);
            var collections = db.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collections.Any();
        }

        private IMongoCollection<ProductStored> GetCollection()
        {
            var database = mongoClient.GetDatabase(DataBaseName);
            return database.GetCollection<ProductStored>(
                CollectionName,
                new MongoCollectionSettings { WriteConcern = WriteConcern.WMajority, ReadPreference = ReadPreference.Primary });
        }

        private void CreateOrgIdIndexIfNecessary()
        {
            var collection = GetCollection();
            var indexes = collection.Indexes.List().ToList();
            var orgIdIndexKey = BsonValue.Create(new BsonDocument("OrgId", new BsonInt32(1)));
            if (indexes.All(index => index.GetElement("key").Value != orgIdIndexKey))
                CreateIndex(collection);
        }

        private void CreateIndex(IMongoCollection<ProductStored> collection)
        {
            var indexKeysDefinition = Builders<ProductStored>.IndexKeys.Ascending(product => product.Name);
            collection.Indexes
                .CreateOne(new CreateIndexModel<ProductStored>(indexKeysDefinition, new CreateIndexOptions { Background = true }));
        }
    }
}