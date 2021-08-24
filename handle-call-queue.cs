using System;
using com.freeclimb;
using com.freeclimb.api;
using com.freeclimb.api.queue;
using com.freeclimb.percl;
using com.freeclimb.webhooks.call;
using com.freeclimb.webhooks.percl;
using com.freeclimb.webhooks.queue;
using Microsoft.AspNetCore.Mvc;

namespace InboundCallQueue.Controllers {
  [Route ("voice")]
  [ApiController]
  public class FreeClimbController : ControllerBase {
    public ActionResult InboundCall (CallStatusCallback freeClimbRequest) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      // Verify inbound call is in proper state
      if (freeClimbRequest.getCallStatus == ECallStatus.Ringing) {
        // Create PerCL say script with US English as the language
        Say say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Set greeting prompt
        say.setText ("Hello. Your call will be queued.");

        // Add PerCL say script to PerCL container
        script.Add (say);

        // Create PerCL pause script with a 100 millisecond pause
        script.Add (new Pause (100));

        // Create queue options with an alias
        QueueOptions options = new QueueOptions ();
        options.setAlias ("InboundCallQueue");

        // Create FreeClimbClient object
        FreeClimbClient client = new FreeClimbClient (getFreeClimbAccountId (), getFreeClimbApiKeys ());

        // Create a queue with an alias
        Queue queue = client.getQueuesRequester.create (options);

        // Create PerCL say to enqueue the call into the newly created queue with an actionUrl
        Enqueue enqueue = new Enqueue (queue.getQueueId, getAppUrl() + "/voice/InboundCallAction");
        // Add waitUrl
        enqueue.setWaitUrl (getAppUrl() + "/voice/InboundCallWait");

        // Add PerCL enqueue script to PerCL container
        script.Add (enqueue);
      }

      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }

    [HttpPost("InboundCallWait")]
    public ActionResult InboundCallWait (QueueWaitCallback queueWaitStatusCallback) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      // Create PerCL getdigits script
      string getDigitsUrl = Url.Action (getAppUrl() + "/voice/CallDequeueSelect");
      GetDigits digits = new GetDigits (getDigitsUrl); // actionUrl

      // Create PerCL say script with US English as the language
      Say say = new Say ();
      say.setLanguage (ELanguage.EnglishUS);
      // Add prompt to for queue exit
      say.setText ("Press any key to exit queue.");

      // Add say script as a prompt to getdigits
      digits.setPrompts (say);

      // Add PerCL getdigits script to PerCL container
      script.Add (digits);

      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }

    [HttpPost("CallDequeueSelect")]
    public ActionResult CallDequeueSelect (GetDigitsActionCallback getDigitsStatusCallback) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();
      if ((getDigitsStatusCallback.getDigits != null) &&
        (getDigitsStatusCallback.getDigits.Length > 0)) {
        // Create PerCL dequeue script and add to PerCL container
        script.Add (new Dequeue ());
      } else {
        // Create PerCL getdigits script
        GetDigits digits = new GetDigits (getAppUrl() + "/voice/CallDequeueSelect");

        // Create PerCL say script with US English as the language
        Say say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Add prompt to for queue exit
        say.setText ("Press any key to exit queue.");

        // Add say script as a prompt to getdigits
        digits.setPrompts (say);

        // Add PerCL getdigits script to PerCL container
        script.Add (digits);
      }
      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }

    [HttpPost("InboundCallAction")]
    public ActionResult InboundCallAction (QueueActionCallback queueActionStatusCallback) {
      // Create an empty PerCL script container
      PerCLScript script = new PerCLScript ();

        // Create PerCL say script with US English as the language
        Say say = new Say ();
        say.setLanguage (ELanguage.EnglishUS);
        // Add prompt for queue exit
        say.setText ("Call exited queue.");

        // Add PerCL say script to PerCL container
        script.Add (say);

        // Create and add PerCL hangup script to PerCL container
        script.Add (new Hangup ());

      // Convert PerCL container to JSON and append to response
      return Content (script.toJson (), "application/json");
    }

    private string getFreeClimbAccountId () {
      return "";
    }

    private string getFreeClimbApiKeys () {
      return "";
    }

    private string getAppUrl () {
      return "";
    }
  }
}
