using com.freeclimb.api;
using com.freeclimb.api.conference;
using com.freeclimb.percl;
using com.freeclimb.webhooks.call;
using com.freeclimb.webhooks.conference;
using Microsoft.AspNetCore.Mvc;

namespace ConnectCallerToAnotherParty.Controllers {
  [Route ("voice")]
  [ApiController]
  public class FreeClimbController : ControllerBase {

    [HttpPost ("InboundCall")]
    public ActionResult InboundCall (CallStatusCallback callStatus) {
      PerCLScript script = new PerCLScript ();
      var conferenceActionUrl = AppUrl + "/voice/ConferenceCreated";
      CreateConference cc = new CreateConference (conferenceActionUrl);
      script.Add (cc);
      return Content (script.toJson (), "application/json");
    }

    [HttpPost ("ConferenceCreated")]
    public ActionResult ConferenceCreated (ConferenceStatusCallback request) {
      // Make OutDial request once conference has been created
      PerCLScript script = new PerCLScript ();
      Say say = new Say ();
      say.setText ("Please wait while we attempt to connect you to an agent.");
      script.Add (say);
      string confId = request.getConferenceId;
      // implementation of lookupAgentPhoneNumber() is left up to the developer
      string agentPhoneNumber = lookupAgentPhoneNumber ();
      // Make OutDial request once conference has been created
      var outdialActionUrl = AppUrl + $"/voice/OutboundCallMade?conferenceId={confId}";
      var outdialConnectedUrl = AppUrl + $"/voice/OutboundCallConnected?conferenceId={confId}";
      OutDial outDial = new OutDial (agentPhoneNumber, outdialConnectedUrl);
      outDial.setCallingNumber (request.getFrom);
      outDial.setActionUrl (outdialActionUrl);
      outDial.setIfMachine (com.freeclimb.EIfMachine.Hangup);
      script.Add (outDial);
      return Content (script.toJson (), "application/json");
    }

    [HttpPost ("OutboundCallMade")]
    public ActionResult OutboundCallMade ([FromQuery (Name = "conferenceId")] string conferenceId, OutDialActionCallback request) {
      PerCLScript script = new PerCLScript ();
      // note the context of the request is the original call, not the newly created via OutDial
      AddToConference addToConference = new AddToConference (conferenceId, request.getCallId);
      var leaveConferenceUrl = AppUrl + "/voice/LeftConference";
      // set the leaveConferenceUrl for the inbound caller, so that we can terminate the conference when they hang up
      addToConference.setLeaveConferenceUrl (leaveConferenceUrl);
      script.Add (addToConference);
      return Content (script.toJson (), "application/json");
    }

    [HttpPost ("OutboundCallConnected")]
    public ActionResult OutboundCallConnected ([FromQuery (Name = "conferenceId")] string conferenceId, CallStatusCallback request) {
      PerCLScript script = new PerCLScript ();
      if (request.getDialCallStatus != com.freeclimb.ECallStatus.InProgress) {
        terminateConference (conferenceId);
        return Content (script.toJson (), "application/json");
      }
      // note context of this callback is the new call (agent). Add them to conference
      script.Add (new AddToConference (conferenceId, request.getCallId));
      return Content (script.toJson (), "application/json");
    }

    [HttpPost ("LeftConference")]
    public ActionResult LeftConference (LeaveConferenceUrlCallback request) {
      PerCLScript script = new PerCLScript ();
      // just terminate conference sonce one party left
      terminateConference (request.getConferenceId);
      return Content (script.toJson (), "application/json");
    }

    private void terminateConference (string conferenceId) {
      // your credentials information filled in here
      string acctId = getAcctId ();
      string apiKey = getApiKey ();
      FreeClimbClient client = new FreeClimbClient (acctId, apiKey);
      // terminating a conference is done by changing the status to Terminated
      ConferenceOptions options = new ConferenceOptions ();
      options.setStatus (com.freeclimb.EConferenceStatus.Terminated);
      client.getConferencesRequester.update (conferenceId, options);
    }

    public string AppUrl { get {return System.Environment.GetEnvironmentVariable("HOST"); } }
    private string getAcctId () {
      return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }

    private string getApiKey () {
      return System.Environment.GetEnvironmentVariable("API_KEY");
    }

    private string lookupAgentPhoneNumber () {
      return "";
    }
  }
}
