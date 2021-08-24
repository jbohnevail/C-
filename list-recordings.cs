using System;
using System.Collections.Generic;
using com.freeclimb.api;
using com.freeclimb.api.recording;

namespace ListRecordings {

  class Program {
    public static string getFreeClimbAccountId () {
      return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }
    public static string getFreeClimbApiKeys () {
      return System.Environment.GetEnvironmentVariable("API_KEY");
    }

    static void Main (string[] args) {
      // Create FreeClimbClient object
      FreeClimbClient client = new FreeClimbClient (getFreeClimbAccountId (), getFreeClimbApiKeys ());

      // Invoke get method to retrieve initial list of recording information
      RecordingList recordingList = client.getRecordingsRequester.getMeta ();

      // Check if list is empty by checking total size of the list
      if (recordingList.getTotalSize > 0) {
        // retrieve all recording for server
        while (recordingList.getLocalSize < recordingList.getTotalSize) {
          recordingList.loadNextPage ();
        }

        // Convert current pages recording information to a linked list
        LinkedList<IFreeClimbCommon> commonList = recordingList.export ();

        // Loop through linked list to process recording information
        foreach (IFreeClimbCommon element in commonList) {
          // Cast each element to the Recording element for processing
          Recording recording = element as Recording;

          // Process recording element
          Console.Write (recording.getRecordingId);
        }
      }
    }
  }
}
