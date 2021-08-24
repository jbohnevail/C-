using com.freeclimb.api;
using com.freeclimb.api.queue;
namespace CreateQueue
{
  class Program
  {
    static string getFreeClimbAccountId()
    {
      return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }

    static string getFreeClimbApiKeys()
    {
      return System.Environment.GetEnvironmentVariable("API_KEY");
    }

    static void Main(string[] args)
    {
      string alias = "My_First_Queue";

      QueueOptions options = new QueueOptions();
      options.setAlias(alias); // Set the optional alias

      // Create FreeClimbClient object
      FreeClimbClient client = new FreeClimbClient(getFreeClimbAccountId(),
                                           getFreeClimbApiKeys());

      // Invoke method to create queue metadata
      Queue queue = client.getQueuesRequester.create(options);
    }
  }
}
