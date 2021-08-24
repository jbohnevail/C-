using System;
using com.freeclimb;
using com.freeclimb.percl;
using com.freeclimb.webhooks;
using com.freeclimb.webhooks.percl;
using com.freeclimb.webhooks.message;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace SendMessagePercl.Controllers {
  [Route ("/voice]")]
  [ApiController]
  public class FreeClimbController : ControllerBase {

    public string FromPhoneNumber { get { return ""; } }
    public string AppUrl { get { return System.Environment.GetEnvironmentVariable("HOST"); } }

    [HttpPost] // POST /voice
    public ActionResult voice (VoiceRequest freeClimbRequest) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      // Verify inbound call is in proper state
      if (freeClimbRequest.getCallStatus == ECallStatus.Ringing) {
        // Create PerCL say script with US English as the language
        Say say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Set greeting prompt
        say.setText ("Hello");
        // Add PerCL say script to PerCL container
        script.Add (say);
        // Create PerCL pause script with a 100 millisecond pause
        Pause pause = new Pause (100);
        // Add PerCL pause script to PerCL container
        script.Add (pause);
        // Create PerCL getdigits script
        string getDigitsUrl = AppUrl + "PhoneNumDone";
        GetDigits getDigits = new GetDigits (getDigitsUrl);
        // Set the max and min number of expected digits to 1
        getDigits.setMaxDigits (10);
        getDigits.setMinDigits (10);
        // Set the DTMF buffer flush to false
        getDigits.setFlushBuffer (EBool.False);
        // Create PerCL say script with US English as the language
        say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Set color selection menu prompt
        say.setText ("Please enter the 10 digits phone number to send the text message to.");
        // Add main selection menu prompt to the getdigits script
        getDigits.setPrompts (say);
        // Add PerCL getdigits script to PerCL container
        script.Add (getDigits);
      }
      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }

    [HttpPost ("GetDigits")] // /voice/GetDigits
    public ActionResult GetDigits (GetDigitsActionCallback getDigitsStatusCallback) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      // Verify the getdigits contains a single digit response
      if ((getDigitsStatusCallback.getDigits != null) && (getDigitsStatusCallback.getDigits.Length == 10)) {
        // create properly formatted phone num
        string phoneNum = "+1" + getDigitsStatusCallback.getDigits;
        // create and add PerCL sms script to PerCL container
        Sms sms = new Sms (FromPhoneNumber, phoneNum, "Hello from FreeClimb SMS");
        // add a notification URL so we can track status of the message
        sms.setNotificationUrl(AppUrl + "MessageStatusCallback");
        script.Add (sms);
        // Create PerCL say script with US English as the language
        Say say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Set color selected prompt
        say.setText ("We'll send the text message now. Goodbye.");
        // Add PerCL say script to PerCL container
        script.Add (say);

        // Create PerCL hangup script and add to the container
        script.Add (new Hangup ());
      }
      // unexpected getdigit response
      else {
        // Create PerCL say script with US English as the language
        Say say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Set error selection prompt
        say.setText ("There was an error retrieving your selection. Goodbye.");
        // Add PerCL say script to PerCL container
        script.Add (say);
        // Create PerCL hangup script and add to the container
        script.Add (new Hangup ());
      }
      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }

    public ActionResult MessageStatusCallback(MessageStatus status)
    {
      // note this returns no PerCL; it's just a status message informating us of a change in status of a message we sent
      // Read the entire FreeClimb request JSON context
      // just log for tutorial. your app may need to take action based on status change
      Console.WriteLine ("Message Status Callback. Message ID: " + status.getMessageId + ". Message Status: " + status.getStatus.ToString ());
      // just return OK
      return Ok();
    }
  }
}
