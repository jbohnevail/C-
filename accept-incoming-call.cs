using com.freeclimb;
using com.freeclimb.percl;
using com.freeclimb.webhooks.call;
using Microsoft.AspNetCore.Mvc;

namespace IncomingCall.Controllers {
  [Route ("voice")]
  [ApiController]
  public class FreeClimbController : ControllerBase {

    [HttpPost("InboundCall")]
    public ActionResult InboundCall (CallStatusCallback freeClimbRequest) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      // Verify inbound call is in proper state
      if (freeClimbRequest.getCallStatus == ECallStatus.Ringing) {
        // Create PerCL say script with US English as the language
        Say say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Set prompt to record message
        say.setText ("Hello. Thank you for invoking the accept incoming call tutorial.");

        // Add PerCL say script to PerCL container
        script.Add (say);

        // Create PerCL pause script with a duration of 100 milliseconds
        Pause pause = new Pause (100);

        // Add PerCL pause script to PerCL container
        script.Add (pause);

        // Create PerCL say script with US English as the language
        Say sayGoodbye = new Say ();
        sayGoodbye.setLanguage (ELanguage.EnglishUS);
        // Set prompt
        sayGoodbye.setText ("Goodbye.");

        // Add PerCL say script to PerCL container
        script.Add (sayGoodbye);

        // Create PerCL hangup script
        Hangup hangup = new Hangup ();

        // Add PerCL hangup script to PerCL container
        script.Add (new Hangup ());
      }
      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }
  }
}
