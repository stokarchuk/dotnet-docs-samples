﻿using Google.Apis.Storage.v1.Data;
using Google.Storage.V1;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        private static readonly string s_projectId = "YOUR-PROJECT-ID";

        private static readonly string s_usage =
                "Usage: \n" +
                "  QuickStart create [new-bucket-name]\n" +
                "  QuickStart list\n" +
                "  QuickStart list bucket-name [prefix]\n" +
                "  QuickStart upload bucket-name local-file-path\n" +
                "  QuickStart delete bucket-name\n" +
                "  QuickStart delete bucket-name object-name\n";

        // [START storage_create_bucket]
        private static void CreateBucket(string bucketName)
        {
            var storage = StorageClient.Create();
            if (bucketName == null)
                bucketName = RandomBucketName();
            storage.CreateBucket(s_projectId, new Bucket { Name = bucketName });
            Console.WriteLine($"Created {bucketName}.");
        }
        // [END storage_create_bucket]

        // [START storage_list_buckets]
        private static void ListBuckets()
        {
            var storage = StorageClient.Create();
            foreach (var bucket in storage.ListBuckets(s_projectId))
            {
                Console.WriteLine(bucket.Name);
            }
        }
        // [END storage_list_buckets]

        // [START storage_delete_bucket]
        private static void DeleteBucket(string bucketName)
        {
            var storage = StorageClient.Create();
            storage.DeleteBucket(new Bucket { Name = bucketName });
            Console.WriteLine($"Deleted {bucketName}.");
        }
        // [END storage_delete_bucket]

        // [START storage_list_files]
        private static void ListObjects(string bucketName, string prefix)
        {
            var storage = StorageClient.Create();
            foreach (var bucket in storage.ListObjects(bucketName, prefix))
            {
                Console.WriteLine(bucket.Name);
            }
        }
        // [END storage_list_files]

        // [START storage_upload_file]
        private static void UploadFile(string bucketName, string localPath)
        {
            var storage = StorageClient.Create();
            using (var f = File.OpenRead(localPath))
            {
                storage.UploadObject(new Google.Apis.Storage.v1.Data.Object
                {
                    Bucket = bucketName,
                    Name = Path.GetFileName(localPath)
                }, f);
            }
        }
        // [END storage_upload_file]

        // [START storage_delete_file]
        private static void DeleteObject(string bucketName, string objectName)
        {
            var storage = StorageClient.Create();
            storage.DeleteObject(new Google.Apis.Storage.v1.Data.Object()
            {
                Bucket = bucketName,
                Name = objectName,
            });
        }
        // [END storage_delete_file]

        public static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine(s_usage);
                return -1;
            }
            try
            {
                switch (args[0].ToLower())
                {
                    case "create":
                        CreateBucket(args.Length < 2 ? null : args[1]);
                        break;

                    case "list":
                        if (args.Length < 2)
                            ListBuckets();
                        else
                            ListObjects(args[1], args.Length < 3 ? "" : args[3]);
                        break;

                    case "delete":
                        if (args.Length < 2)
                        {
                            Console.WriteLine(s_usage);
                            return -1;
                        }
                        if (args.Length < 3)
                        {
                            DeleteBucket(args[1]);
                        }
                        else
                        {
                            DeleteObject(args[1], args[2]);
                        }
                        break;

                    case "upload":
                        if (args.Length < 3)
                        {
                            Console.WriteLine(s_usage);
                            return -1;
                        }
                        UploadFile(args[1], args[2]);
                        break;

                    default:
                        Console.WriteLine(s_usage);
                        return -1;
                }
                return 0;
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                return e.Error.Code;
            }
        }

        private static string RandomBucketName()
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                string legalChars = "abcdefhijklmnopqrstuvwxyz";
                byte[] randomByte = new byte[1];
                var randomChars = new char[12];
                int nextChar = 0;
                while (nextChar < randomChars.Length)
                {
                    rng.GetBytes(randomByte);
                    if (legalChars.Contains((char)randomByte[0]))
                        randomChars[nextChar++] = (char)randomByte[0];
                }
                return new string(randomChars);
            }
        }
    }
}
