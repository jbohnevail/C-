using System;
using com.freeclimb.api;
using com.freeclimb.api.queue;

namespace GetQueue {
  class Program {

    static string getFreeClimbAccountId () { return System.Environment.GetEnvironmentVariable("ACCOUNT_ID"); }
    static string getFreeClimbApiKeys () {return System.Environment.GetEnvironmentVariable("API_KEY"); }
    static void Main (string[] args) {
      string queueId = "";
      // Create FreeClimbClient object
      FreeClimbClient client = new FreeClimbClient (getFreeClimbAccountId (),
        getFreeClimbApiKeys ());

      // Invoke get method to retrieve queued metadata
      Queue queue = client.getQueuesRequester.get (queueId);
      Console.WriteLine(queue.getQueueId);
    }
  }
}
