using System.Collections.Generic;
using com.freeclimb;
using com.freeclimb.api;
using com.freeclimb.percl;
using com.freeclimb.webhooks.call;
using com.freeclimb.webhooks.percl;
using Microsoft.AspNetCore.Mvc;

namespace CollectDigits.Controllers {
  [Route ("voice/")]
  [ApiController]
  public class FreeClimbController : ControllerBase {

    public string AppUrl { get { return System.Environment.GetEnvironmentVariable("HOST"); } }

    [HttpPost ("InboundCall")] // POST voice/InboundCall
    public ActionResult InboundCall (CallStatusCallback freeClimbRequest) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      // Verify inbund call is in proper state
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
        string getDigitsUrl = AppUrl + "/voice/ColorSelectionDone";

        GetDigits getDigits = new GetDigits (getDigitsUrl);
        // Set the max and min number of expected digits to 1
        getDigits.setMaxDigits (1);
        getDigits.setMinDigits (1);
        // Set the DTMF buffer flush to false
        getDigits.setFlushBuffer (EBool.False);

        // Create PerCL say script with US English as the language
        say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Set color selection menu prompt
        say.setText ("Please select a color. Enter one for green, two for red or three for yellow.");

        // Add main selection menu prompt to the getdigits script
        getDigits.setPrompts (say);

        // Add PerCL getdigits script to PerCL container
        script.Add (getDigits);
      }

      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }

    [HttpPost("ColorSelectionDone")] // POST voice/ColorSelectionDone
    public ActionResult ColorSelectionDone (GetDigitsActionCallback getDigitsStatusCallback) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      // Verify the getdigits contains a single digit response
      if ((getDigitsStatusCallback.getDigits != null) && (getDigitsStatusCallback.getDigits.Length == 1)) {
        // Verify digit one selected
        if (string.Equals (getDigitsStatusCallback.getDigits, "1") == true) {
          // Create PerCL say script with US English as the language
          Say say = new Say ();
          say.setLanguage (ELanguage.EnglishUS);
          // Set color selected prompt
          say.setText ("You selected green. Goodbye.");

          // Add PerCL say script to PerCL container
          script.Add (say);

          // Create PerCL hangup script and add to the container
          script.Add (new Hangup ());
        }
        // Verify digit two selected
        else if (string.Equals (getDigitsStatusCallback.getDigits, "2") == true) {
          // Create PerCL say script with US English as the language
          Say say = new Say ();
          say.setLanguage (ELanguage.EnglishUS);
          // Set color selected prompt
          say.setText ("You selected red. Goodbye.");

          // Add PerCL say script to PerCL container
          script.Add (say);

          // Create PerCL hangup script and add to the container
          script.Add (new Hangup ());
        }
        // Verify digit three selected
        else if (string.Equals (getDigitsStatusCallback.getDigits, "3") == true) {
          // Create PerCL say script with US English as the language
          Say say = new Say ();
          say.setLanguage (ELanguage.EnglishUS);
          // Set color selected prompt
          say.setText ("You selected yellow. Goodbye.");

          // Add PerCL say script to PerCL container
          script.Add (say);

          // Create PerCL hangup script and add to the container
          script.Add (new Hangup ());
        }
        // Invalid selection
        else {
          // Create PerCL say script with US English as the language
          Say say = new Say ();
          say.setLanguage (ELanguage.EnglishUS);
          // Set invalid selection prompt
          say.setText ("Invalid selection. Goodbye.");

          // Add PerCL say script to PerCL container
          script.Add (say);

          // Create PerCL hangup script and add to the container
          script.Add (new Hangup ());
        }
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
  }
}
