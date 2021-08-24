using com.freeclimb;
using com.freeclimb.percl;
using com.freeclimb.webhooks.call;
using Microsoft.AspNetCore.Mvc;

namespace PlayRecording.Controllers {
  [Route ("voice")]
  [ApiController]
  public class FreeClimbController : ControllerBase {
    [HttpPost ("PlayRecordingCallAnswered")]
    public ActionResult PlayRecordingCallAnswered (CallStatusCallback callStatusCallback) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      // Verify call is in the InProgress state
      if (callStatusCallback.getDialCallStatus == ECallStatus.InProgress) {
        // Create PerCL play script with US English as the language
        Play play = new Play (GetRecordingUrl ());

        // Add PerCL play script to PerCL container
        script.Add (play);
      }

      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }

    public ActionResult PlayRecordingCallStatus (CallStatusCallback callStatusCallback) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      return Content (script.toJson (), "application/json");
    }

    private string GetRecordingUrl () {
      return System.Environment.GetEnvironmentVariable("RECORDING_URL");
    }
  }
}
