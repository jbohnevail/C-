using System;
using System.Collections.Generic;
using com.freeclimb.api;
using com.freeclimb.api.message;

namespace ListRecordings {

  class Program {
    public static string getFreeClimbAccountId () {
      return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }
    public static string getFreeClimbApiKeys () {
      return System.Environment.GetEnvironmentVariable("API_KEY");
    }

        static void Main(string[] args)
        {
            // Create FreeClimbClient object
            FreeClimbClient client = new FreeClimbClient(getFreeClimbAccountId(), getFreeClimbApiKeys());

            // Invoke get method to retrieve initial list of recording information
            MessageList messageList = client.getMessagesRequester.get();

            // Check if list is empty by checking total size of the list
            if (messageList.getTotalSize > 0)
            {
                // retrieve all recording for server
                while (messageList.getLocalSize < messageList.getTotalSize)
                {
                    messageList.loadNextPage();
                }

                // Convert current pages recording information to a linked list
                LinkedList<IFreeClimbCommon> commonList = messageList.export();

                // Loop through linked list to process recording information
                foreach (IFreeClimbCommon element in commonList)
                {
                    // Cast each element to the Recording element for processing
                    Message message = element as Message;

                    // Process recording element
                    Console.Write(message.getText);
                }
            }
        }
  }
}
