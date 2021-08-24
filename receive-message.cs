using com.freeclimb;
using com.freeclimb.percl;
using com.freeclimb.webhooks.call;
using System;
using Microsoft.AspNetCore.Mvc;
using com.freeclimb.api;

namespace ReceiveMessage.Controllers {
  [Route ("inboundSms")]
  [ApiController]
  public class FreeClimbController : ControllerBase {

    [HttpPost]
    public ActionResult smsConnect (CallStatusCallback freeClimbRequest) {
        string acctId = getAcctId ();
        string apiKey = getApiKey ();
        FreeClimbClient client = new FreeClimbClient (acctId, apiKey);
        string to = freeClimbRequest.getFrom;
        string from = "";
        client.getMessagesRequester.create(from, to, "Hello from the C# SDK!");
       
        return Ok();
    }

    private string getAcctId () {
        return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }

    private string getApiKey () {
        return System.Environment.GetEnvironmentVariable("API_KEY");
    }
  }
}
