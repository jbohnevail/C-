using System;
using System.Collections.Generic;
using com.freeclimb.api;
using com.freeclimb.api.queue;

namespace ListQueueMembers {
  class Program {

    static string getFreeClimbAccountId () {
      return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }
    static string getFreeClimbApiKeys () {
      return System.Environment.GetEnvironmentVariable("API_KEY");
    }
    static void Main (string[] args) {
      string queueId = "";
      // Create FreeClimbClient object
      FreeClimbClient client = new FreeClimbClient (getFreeClimbAccountId (),
        getFreeClimbApiKeys ());

      // Invoke getMembers method to retrieve initial list of queue member information
      QueueMemberList queueMemberList = client.getQueuesRequester.getMembers (queueId);

      // Check if list is empty by checking total size of the list
      if (queueMemberList.getTotalSize > 0) {
        // retrieve all queue member information from server
        while (queueMemberList.getLocalSize < queueMemberList.getTotalSize) {
          queueMemberList.loadNextPage ();
        }

        // Convert current pages queue information to a linked list
        LinkedList<IFreeClimbCommon> commonList = queueMemberList.export ();

        // Loop through linked list to process queue member information
        foreach (IFreeClimbCommon element in commonList) {
          // Cast each element to the QueueMember element for processing
          QueueMember queueMember = element as QueueMember;
          Console.WriteLine (queueMember.getCallId);
        }
      } else {
       Console.WriteLine ("No Members in queue");
      }

    }
  }
}
