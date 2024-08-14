using Google.Cloud.Firestore;
using DotNetEnv;
using Google.Cloud.Firestore.V1;

namespace Services
{
    public class FirestoreDB
    {
        private FirestoreDb _db;
        public FirestoreDB()
        {
            // Load environment variables from .env.local file
            DotNetEnv.Env.Load(".env.local");

            // Get the path to the credentials file from the environment variable
            string? credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            if (string.IsNullOrEmpty(credentialsPath))
            {
                throw new InvalidOperationException("GOOGLE_APPLICATION_CREDENTIALS environment variable is not set.");
            }

            // Set the environment variable for Google Cloud credentials
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

            // Get the project ID and database ID from environment variables
            string? projectId = Environment.GetEnvironmentVariable("PROJECT_ID");
            string? databaseId = Environment.GetEnvironmentVariable("DATABASE_ID");

            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(databaseId))
            {
                throw new InvalidOperationException("PROJECT_ID or DATABASE_ID environment variable is not set.");
            }

            _db = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                DatabaseId = databaseId
            }.Build();
        }

        public async Task<List<Dictionary<string, object>>> GetCollection(string collection)
        {
            Query query = _db.Collection(collection);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();
                list.Add(data);
            }
            return list;
        }
    }
}