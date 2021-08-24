using com.freeclimb;
using com.freeclimb.percl;
using com.freeclimb.webhooks.call;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MakeOutboundCall.Controllers {
  [Route ("connect")]
  [ApiController]
  public class FreeClimbController : ControllerBase {

    [HttpPost]
    public ActionResult CallConnect (CallStatusCallback freeClimbRequest) {
        // Create an empty PerCL script container
        PerCLScript script = new PerCLScript ();
        Say say = new Say();
        say.setText("You just got called by the C sharp S D K!");
        Console.WriteLine(freeClimbRequest.getCallStatus);
       
        script.Add (say);

        // Convert PerCL container to JSON and append to response
        return Content (script.toJson (), "application/json");
    }
  }
}
