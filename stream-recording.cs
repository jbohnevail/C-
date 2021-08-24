using System;
using System.IO;
using com.freeclimb.api;

namespace StreamRecording
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        string FreeClimbAccountId = System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
        string FreeClimbApiKey = System.Environment.GetEnvironmentVariable("API_KEY");
        string RecordingId = "";

        // Create FreeClimbClient object
        FreeClimbClient client = new FreeClimbClient(FreeClimbAccountId, FreeClimbApiKey);

        // Invoke get method with AudioReturn format of Stream to obtain IO stream of audio
        Stream stream = client.getRecordingsRequester.stream(RecordingId);
      }
      catch (FreeClimbException ex)
      {
        // Exception throw upon failure
        Console.Write(ex.Message);
      }
    }
  }
}
