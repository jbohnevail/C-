using System;
using System.Collections.Generic;
using com.freeclimb;
using com.freeclimb.api;
using com.freeclimb.api.call;
using com.freeclimb.percl;
using com.freeclimb.webhooks.application;
using com.freeclimb.webhooks.call;
using com.freeclimb.webhooks.percl;
using Microsoft.AspNetCore.Mvc;

namespace SpeechRecognition.Controllers {
  [Route("/voice")]
  [ApiController]
  public class FreeClimbontroller : ControllerBase {

    public string AppUrl {
      get {
        return System.Environment.GetEnvironmentVariable("HOST");
      }
    }

    [HttpPost ("/CreateCall")]
    public void CreateCall ([FromBody] string value) {
      string accountId = System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
      string apiKey = System.Environment.GetEnvironmentVariable("API_KEY");

      // Set up Call Details
      string applicationId = System.Environment.GetEnvironmentVariable("APPLICATION_ID");
      string phoneNumber = "+" + value;
      string freeclimbPhoneNumber = "Your FreeClimb Number Here";

      try {
        // Create the FreeClimbClient
        FreeClimbClient client = new FreeClimbClient (accountId, apiKey);
        // Create a Call
        Call call = client.getCallsRequester.create (phoneNumber, // To
          freeclimbPhoneNumber, // From,
          applicationId); // Application to Handle the call
      } catch (FreeClimbException ex) {
        System.Console.Write (ex.Message);
      }
    }

    [HttpPost] // POST /voice
    public ActionResult SelectColorCallAnswered (CallStatusCallback callStatusCallback) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      // Verify call is in the InProgress state
      if (callStatusCallback.getDialCallStatus == ECallStatus.InProgress) {
        // Create PerCL get speech script (see grammar file content below)
        string actionUrl = AppUrl + "/voice/SelectColorDone";
        string grammarFile = AppUrl + "/grammars/FreeClimbColor.xml";
        GetSpeech getSpeech = new GetSpeech (actionUrl, grammarFile);
        // Set location and type of grammar as well as the grammar rule
        getSpeech.setGrammarType (EGrammarType.Url);
        getSpeech.setGrammarRule ("FreeClimbColor");

        // Create PerCL say script with US English as the language
        Say say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Set prompt for color selection
        say.setText ("Please select a color. Select green, red or yellow.");

        // Add PerCL say script to PerCL get speech prompt list
        getSpeech.setPrompts (say);

        // Add PerCL get speech script to PerCL container
        script.Add (getSpeech);
      }

      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }

    [HttpPost("SelectColorDone")]
    public ActionResult SelectColorDone (GetSpeechActionCallback getSpeechStatusCallback) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();

      // Check if recognition was successful
      if (getSpeechStatusCallback.getReason == ESpeechTermReason.Recognition) {
        // Create PerCL say script with US English as the language
        Say say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Set prompt to speak the selected color
        say.setText (string.Format ("Selected color was {0}", (getSpeechStatusCallback.getRecognitionResult).ToLower ()));

        // Add PerCL say script to PerCL container
        script.Add (say);
      } else {
        // Create PerCL say script with US English as the language
        Say say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Set prompt to indicated selection error
        say.setText ("There was an error in selecting a color.");

        // Add PerCL say script to PerCL container
        script.Add (say);
      }

      // Create PerCL pause script with a duration of 100 milliseconds
      Pause pause = new Pause (100);

      // Add PerCL pause script to PerCL container
      script.Add (pause);

      // Create PerCL say script with US English as the language
      Say sayGoodbye = new Say ();
      sayGoodbye.setLanguage (ELanguage.EnglishUS);
      // Set prompt
      sayGoodbye.setText("Goodbye");
      // Add PerCL say script to PerCL container
      script.Add (sayGoodbye);

      // Create PerCL hangup script
      Hangup hangup = new Hangup ();

      // Add PerCL hangup script to PerCL container
      script.Add (new Hangup ());

      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }
  }
}
