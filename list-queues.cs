using System;
using System.Collections.Generic;
using com.freeclimb.api;
using com.freeclimb.api.queue;

namespace ListQueues
{
  class Program
  {
    public static string Alias { get; set; } = null;

    public static string getFreeClimbAccountId()
    {
      return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }
    public static string getFreeClimbApiKeys()
    {
      return System.Environment.GetEnvironmentVariable("API_KEY");
    }
    static void Main(string[] args)
    {
      // If alias provided create and populate search filter object
      QueuesSearchFilters filters = null;
      if (String.IsNullOrEmpty(Alias) == false)
      {
        filters = new QueuesSearchFilters();
        filters.setAlias(Alias);
      }

      // Create FreeClimbClient object
      FreeClimbClient client = new FreeClimbClient(getFreeClimbAccountId(),
                                           getFreeClimbApiKeys());

      // Invoke get method to retrieve initial list of queue information
      QueueList queueList = client.getQueuesRequester.get(filters);

      Console.Write($"Number of queues: {queueList.getTotalSize} \n" );
      // Check if list is empty by checking total size of the list
      if (queueList.getTotalSize > 0)
      {
        // retrieve all queue information from server
        while (queueList.getLocalSize < queueList.getTotalSize)
        {
          queueList.loadNextPage();
        }

        // Convert current pages queue information to a linked list
        LinkedList<IFreeClimbCommon> commonList = queueList.export();
        // Loop through linked list to process queue information
        foreach (IFreeClimbCommon element in commonList)
        {
          // Cast each element to the Queue element for processing
          Queue queue = element as Queue;

          // Process queue element
        }
      }
    }
  }
}
