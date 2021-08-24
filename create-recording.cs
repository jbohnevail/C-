using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using com.freeclimb.api;
using com.freeclimb.percl;
using com.freeclimb.webhooks.call;
using com.freeclimb.api.call;
using com.freeclimb;

namespace MakeARecording.Controllers
{
    [Route("voice")]
    [ApiController]
    public class FreeClimbController : ControllerBase
    {
        public string AppUrl { get { return System.Environment.GetEnvironmentVariable("HOST");} }

        // POST /voice/
        [HttpPost]
        public ActionResult Post(CallStatusCallback callStatusCallback)
        {
            PerCLScript script = new PerCLScript();
            // Verify call is in the InProgress state
            if (callStatusCallback.getDialCallStatus == ECallStatus.InProgress) {
                // Create PerCL say script with US English as the language
                Say say = new Say();
                say.setLanguage(ELanguage.EnglishUS);
                // Set prompt to record message
                say.setText("Hello. Please leave a message after the beep, then press one or hangup");
                // Add PerCL say script to PerCL container
                script.Add(say);
                // Create PerCL record utterance script
                string messageDoneUrl = AppUrl + "/voice/MakeRecordMessageDone";
                RecordUtterance recordUtterance = new RecordUtterance(messageDoneUrl);
                // Set indication that audible 'beep' should be used to signal start of recording
                recordUtterance.setPlayBeep(EBool.True);
                // Set indication that end of recording is touch tone key 0ne
                recordUtterance.setFinishOnKey(EFinishOnKey.One);

                // Add PerCL record utterance script to PerCL container
                script.Add(recordUtterance);
            }
            // Convert PerCL container to JSON and append to response
            return Content(script.toJson(), "application/json");
        }

        // PUT api/values/5
        // RPC call To start a call
        [HttpPut]
        public void Put([FromBody] string value)
        {
          // Set up App Credentails
          string accountId = System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
          string apiKey = System.Environment.GetEnvironmentVariable("API_KEY");

          // Set up Call Details
          string applicationId = System.Environment.GetEnvironmentVariable("APPLICATION_ID");
          string phoneNumber = "+" + value;
          string freeClimbPhoneNumber = "+Your FreeClimb Number";

          try {
            // Create the PersyClient
            FreeClimbClient client = new FreeClimbClient(accountId, apiKey);
            // Create a Call
            Call call = client.getCallsRequester.create(phoneNumber, // To
                                                        freeClimbPhoneNumber, // From,
                                                        applicationId); // Application to Handle the call
          } catch(FreeClimbException ex) {
            System.Console.Write(ex.Message);
          }
        }


        [HttpPost("MakeRecordMessageDone")]
        public ActionResult MakeRecordMessageDone(RecordingUtteranceActionCallback recordingUtteranceStatusCallback) {
          // Create an empty PerCL script container
          PerCLScript script = new PerCLScript();

          if (Request != null) {
            // Check if recording was successful by checking if a recording identifier was provided
            if (recordingUtteranceStatusCallback.getRecordingId != null) {
              // Recording was successful as recording identifier present in response

              // Create PerCL say script with US English as the language
              Say say = new Say();
              say.setLanguage(ELanguage.EnglishUS);
              // Set prompt to indicate message has been recorded
              say.setText("Thanks. The message has been recorded.");

              // Add PerCL say script to PerCL container
              script.Add(say);
            }
            else {
              // Recording was failed as there is no recording identifier present in response

              // Create PerCL say script with US English as the language
              Say say = new Say();
              say.setLanguage(ELanguage.EnglishUS);
              // Set prompt to indicate message recording failed
              say.setText("Sorry we weren't able to record the message.");

              // Add PerCL say script to PerCL container
              script.Add(say);
            }

            // Create PerCL pause script with a duration of 100 milliseconds
            Pause pause = new Pause(100);

            // Add PerCL pause script to PerCL container
            script.Add(pause);

            // Create PerCL say script with US English as the language
            Say sayGoodbye = new Say();
            sayGoodbye.setLanguage(ELanguage.EnglishUS);
            // Set prompt sayGoodbye.setText("Goodbye");

            // Add PerCL say script to PerCL container
            script.Add(sayGoodbye);

            // Create PerCL hangup script
            Hangup hangup = new Hangup();

            // Add PerCL hangup script to PerCL container
            script.Add(new Hangup());
          }

          // Convert PerCL container to JSON and append to response
          return Content(script.toJson(), "application/json");
      }
  }
}
