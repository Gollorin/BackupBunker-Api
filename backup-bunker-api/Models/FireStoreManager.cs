using Google.Cloud.Firestore;

namespace backup_bunker_api.Models
{
    public class FireStoreManager
    {
        public static FirestoreDb Firestore_DB { get; private set; }

        private const string PRIVATEKEY = @"
        {
              ""type"": ""service_account"",
              ""project_id"": ""backupbunker-758ce"",
              ""private_key_id"": ""eec0d113d9ad28934e9482179ff7ca2088102a6d"",
              ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDcp2SprQfm6nIe\ncfW8uUTUWpcdd2n0ltM90aWV7Nktm3MSIo5uwm5uH/JEteU2h97HepfbWPdK3C4X\nSTDaixtSCcwD6rJ3b54rhMVOjkkpqqRIyp49EAogllQZzOa2SE4W7NcSIoLLOk59\nRhqwCkq6KxGRxonlQKCbzzRu/T6U7QGD80OmCaAm625DcA16HgehtSKRLEopZpuP\nmcNl4MGXzeZi8V8TkrYAyyA4aHZMsocX7wO+K3bQFJx/0Yb6COqpQkrOArY2G5fs\n8dVGeIvLnEF6o0QE/o9fyuBLzu4HL3nOgF9/q7ZuPrjN/jQGys0dT/XPpPoFlT9F\nsF8rITAVAgMBAAECggEAEv5/3lFMSFbvKurBbhT+tU71Ci7Q2qKvHXeyCtVgHppu\njeWWoDcxfV5Xow2LZSXOChrl/pZKd1G7z/pXb8Cgwe+g9cdaxIhBs9pSh8Ac0S7k\nxHVYqMX8MTDvnWbzh0ZzHmsV2nRJEQh8YDFMM9I2+46gjQ1rqmXlTXRJPbKkE/qo\nBRuCubRRfjskAtCxTT4hPKxHRtn2rAY05ZyjlFiifSGsX+I3D4BwKzHcFKJpkemL\nL+T9BGMTjn+RaQeePOxxnHQAca8f4UcXXTsqQaog4u4KpuvZcwcf7hj9eo3eDI/X\n/CTytdHQelNUtqy6tiHd515uJ4thAMTyR/I6ou2IowKBgQDyVdUp88tDaXqWyxQu\nqHWduY8Df9QbEwo2EPCnYh6xEaz/oxyZyrLgZE7ob4kIR8gySZUm2W9hwUnYq541\nnjA+8fxLQzq2R/0VlgnN0lDGjzmQ9dgYrW0AcmxldoQV41noAWV8qZ+Bpr95JlIi\nbKkEZtPI6lqu2IOe9m2LcVXdawKBgQDpGJWWlubARj/YW2YBzvAz3t5QHZAce6Cg\nON4aLXNi9rQ4hJYJwA9KxXAFe8DQImfpDw1WPWv4TrOJGvdWsFT7cyIb30tvJ/Rq\nI2x3IwCEI3QcaBeL3dyGUWiSrWOvHxjc7M36GlcoOdXrOpvUHwSU5zS0NsmfRwcB\nZpW6mhcIfwKBgQDj2BOteAyaeyfJc3KsU7MR8o8mKR+RvRsSrma12aklrorLZ97S\nMiRwmZ8fJw7d/C+cKFbVygXREVyPHPyJSQvvgrFFShZ/n7uyefkJzT4pEix7wMnG\nmfgsPWjxFLcnpTS4z+dNiGXyDodkLsV2nFdlThC9jKpqTiOiZ+ui+ZJTtQKBgHAI\nY+VkE/Y4DXmkvEuENckIhluFSf2WoeZATufQDpRzNL2xZBe9mtv6N1Q7Xbnrv3ux\nw1w+20UWNo5gSp7802ujs7Inya7cSko1Sm2cgiQAkk8Q7LhJ4zMfr55H/hBuZtnQ\na1OZE2j/G0Ua/0idu+sBkUBI0PFgBwryTmbIpudrAoGBAM7izYcLOnb5+P/oJsSU\n1QR9mU1XJrQcVvU5kXfzkXY4hHZoowYqGSf86IkEFO5FjGGicxcA6kyeoxjBqd//\nhjMn75YLZbQsp3ZgcF7hznWN6hxhNqjNs43UoKA7F7E8CE9LJSh2eDzzVG+DW+ra\nmy6RrVpVD0wyP3Zl4zqo4+rK\n-----END PRIVATE KEY-----\n"",
              ""client_email"": ""firebase-adminsdk-5i2f6@backupbunker-758ce.iam.gserviceaccount.com"",
              ""client_id"": ""102232868908651219809"",
              ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
              ""token_uri"": ""https://oauth2.googleapis.com/token"",
              ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
              ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-5i2f6%40backupbunker-758ce.iam.gserviceaccount.com""  
        } ";

        public static void SetFirestoreDatabase()
        {
            // Make json and make pc viraible
            string json_path = Path.Combine(Path.GetTempPath(), "FirebaseDatabasePrivateKey") + ".json";

            //if (!File.Exists(json_path))
            //{
            File.WriteAllText(json_path, PRIVATEKEY);
            File.SetAttributes(json_path, FileAttributes.Hidden);

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", json_path);
            //}

            Firestore_DB = FirestoreDb.Create("backupbunker-758ce");

            File.Delete(json_path);

        }
    }
}
