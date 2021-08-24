using System;
using com.freeclimb.api;

namespace DeleteRecording {
  class Program {
    static void Main (string[] args) {
      try {
        string freeClimbAccountId = System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
        string freeClimbApiKey = System.Environment.GetEnvironmentVariable("API_KEY");
        string recordingId = "";

        // Create FreeClimbClient object
        FreeClimbClient client = new FreeClimbClient (freeClimbAccountId, freeClimbApiKey);

        // Invoke deleted method to delete recording Url
        client.getRecordingsRequester.delete (recordingId);
      } catch (FreeClimbException ex) {
        // Exception throw upon failure
        System.Console.WriteLine(ex.Message);
      }
    }
  }
}
