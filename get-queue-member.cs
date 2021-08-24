using com.freeclimb.api;
using com.freeclimb.api.queue;

namespace GetQueueMember {
  class Program {

    public static string getFreeClimbAccountId()
    {
      return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }

    public static string getFreeClimbApiKeys()
    {
      return System.Environment.GetEnvironmentVariable("API_KEY");
    }

    static void Main (string[] args) {
      string queueId = "";
      string callId = "";
      // Create FreeClimbClient object
      FreeClimbClient client = new FreeClimbClient (getFreeClimbAccountId (),
        getFreeClimbApiKeys ());

      // Invoke get method to retrieve queued call metadata
      QueueMember queueMember = client.getQueuesRequester.getQueueMember (queueId, callId);
    }
  }
}
