using Meadow.Foundation.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class JsonTest<T> : ITest<T>
        where T : IDeviceUnderTest<IMeadowDevice>
    {
        public Task<bool> RunTest(T device)
        {
            try
            {
                // create dummy data
                InsertDummyData();

                // retrieve data
                RetrieveData();

                // update a data field
                UpdateData();

                RetrieveData();

                // retrieve a single item by some field (primary key equivalent)
                RetrieveByKey();

                // delete a record
                DeleteData();

            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Problem: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Resolver.Log.Error($"Inner exception: {ex.InnerException.Message}");
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(true);
        }

        void InsertDummyData()
        {
            // Creating a list of dummy objects to serialize
            var dummyData = new List<SimpleObject>
            {
                new SimpleObject { Name = "Object 1", Value = 42 },
                new SimpleObject { Name = "Object 2", Value = 100 },
                new SimpleObject { Name = "Object 3", Value = 77 }
            };

            // Serializing using MicroJson and logging it
            var json = MicroJson.Serialize(dummyData);
            Resolver.Log.Info("Inserted Dummy Data: ");
            Resolver.Log.Info(json);
        }

        void RetrieveData()
        {
            // Retrieve JSON data and deserialize it
            var json = "[{\"name\":\"Object 1\",\"value\":42},{\"name\":\"Object 2\",\"value\":100},{\"name\":\"Object 3\",\"value\":77}]";

            Resolver.Log.Info("Retrieving Data...");

            var result = MicroJson.Deserialize<List<SimpleObject>>(json);
            foreach (var item in result)
            {
                Resolver.Log.Info($"Retrieved: Name={item.Name}, Value={item.Value}");
            }
        }

        void UpdateData()
        {
            // Simulating an update by deserializing, modifying, and re-serializing the data
            var json = "[{\"name\":\"Object 1\",\"value\":42}]";
            var item = MicroJson.Deserialize<List<SimpleObject>>(json)[0];

            // Modify the object
            item.Value = 84;

            // Serialize the updated object
            var updatedJson = MicroJson.Serialize(item);

            Resolver.Log.Info($"Updated Data: {updatedJson}");
        }

        void RetrieveByKey()
        {
            // Simulate retrieval by a key (similar to primary key)
            var json = "{\"name\":\"Object 1\",\"value\":84}";
            var item = MicroJson.Deserialize<SimpleObject>(json);

            Resolver.Log.Info($"Retrieved by key: Name={item.Name}, Value={item.Value}");
        }

        void DeleteData()
        {
            // Simulate deleting data by removing an item from the list
            var json = "[{\"name\":\"Object 1\",\"value\":42},{\"name\":\"Object 2\",\"value\":100}]";
            var data = MicroJson.Deserialize<List<SimpleObject>>(json);

            // Remove the first item
            data.RemoveAt(0);

            // Serialize the remaining data
            var updatedJson = MicroJson.Serialize(data);

            Resolver.Log.Info($"After deletion: {updatedJson}");
        }
    }

    // Simple object class used for testing JSON serialization
    public class SimpleObject
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
