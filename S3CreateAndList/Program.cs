using System;
using System.Threading.Tasks;

// To interact with Amazon S3.
using Amazon.S3;
using Amazon.S3.Model;

namespace S3CreateAndList
{
	class Program
	{
		// Main method
		static async Task Main(string[] args)
		{
			// Before running this app:
			// - Credentials must be specified in an AWS profile. If you use a profile other than
			//   the [default] profile, also set the AWS_PROFILE environment variable.
			// - An AWS Region must be specified either in the [default] profile
			//   or by setting the AWS_REGION environment variable.

			// Create an S3 client object.
			var s3Client = new AmazonS3Client();

			// Parse the command line arguments for the bucket name.
			if (GetBucketName(args, out String bucketName))
			{
				// If a bucket name was supplied, create the bucket.
				// Call the API method directly
				try
				{
					Console.WriteLine($"\nCreating bucket {bucketName}...");
					var createResponse = await s3Client.PutBucketAsync(bucketName);
					Console.WriteLine($"Result: {createResponse.HttpStatusCode.ToString()}");
				}
				catch (Exception e)
				{
					Console.WriteLine("Caught exception when creating a bucket:");
					Console.WriteLine(e.Message);
				}
			}

			// List the buckets owned by the user.
			// Call a class method that calls the API method.
			Console.WriteLine("\nGetting a list of your buckets...");
			var listResponse = await MyListBucketsAsync(s3Client);
			Console.WriteLine($"Number of buckets: {listResponse.Buckets.Count}");
			foreach (S3Bucket b in listResponse.Buckets)
			{
				Console.WriteLine(b.BucketName);
			}

			Console.WriteLine("Would you like to delete a bucket?");
			string stringDeleteBucketDecision = Console.ReadLine().ToUpper();
			if (stringDeleteBucketDecision is not null && stringDeleteBucketDecision.Length == 1 && stringDeleteBucketDecision == "Y")
			{
				Console.WriteLine("\n");
				Console.WriteLine(stringDeleteBucketDecision);
				string S3BucketToDelete = GetBucketNameToDelete();
				Console.WriteLine($"Bucket to be deleted: {S3BucketToDelete}");
				try
				{
					await DeleteS3BucketAsync(s3Client, S3BucketToDelete);
				}
				catch (Exception e)
				{
					Console.Write("There was an issue with creating the bucket: ");
					Console.Write("\n");
					Console.WriteLine(e.Message);
				}
			}
		}


		// 
		// Method to parse the command line.
		private static Boolean GetBucketName(string[] args, out String bucketName)
		{
			Boolean retval = false;
			bucketName = String.Empty;
			if (args.Length == 0)
			{
				Console.WriteLine("\nNo arguments specified. Will simply list your Amazon S3 buckets." +
				  "\nIf you wish to create a bucket, supply a valid, globally unique bucket name.");
				bucketName = String.Empty;
				retval = false;
			}
			else if (args.Length == 1)
			{
				bucketName = args[0];
				retval = true;
			}
			else
			{
				Console.WriteLine("\nToo many arguments specified." +
				  "\n\ndotnet_tutorials - A utility to list your Amazon S3 buckets and optionally create a new one." +
				  "\n\nUsage: S3CreateAndList [bucket_name]" +
				  "\n - bucket_name: A valid, globally unique bucket name." +
				  "\n - If bucket_name isn't supplied, this utility simply lists your buckets.");
				Environment.Exit(1);
			}
			return retval;
		}


		//
		// Async method to get a list of Amazon S3 buckets.
		private static async Task<ListBucketsResponse> MyListBucketsAsync(IAmazonS3 s3Client)
		{
			return await s3Client.ListBucketsAsync();
		}

		// Get user entry
		public static string GetBucketNameToDelete()
		{
			Console.WriteLine("Please enter the name of the bucket that you would like to delete.");
			Console.WriteLine("\n Also, please note - if the bucket you are looking to delete has contents in it - you will not be able to delete it.");
			Console.WriteLine("\n Make your entry below:");
			string bucketToDelete = Console.ReadLine();
			return bucketToDelete;
		}

		public static async Task<bool> DeleteS3BucketAsync(IAmazonS3 client, string S3BucketName)
		{
			var request = new DeleteBucketRequest
			{
				BucketName = S3BucketName,
			};
			var response = await client.DeleteBucketAsync(request);
			return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
		}

	}
}
