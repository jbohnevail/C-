using System;
using System.Collections.Generic;
using com.freeclimb;
using com.freeclimb.percl;
using com.freeclimb.webhooks.call;
using com.freeclimb.webhooks.conference;
using com.freeclimb.webhooks.percl;
using com.freeclimb.api;
using com.freeclimb.api.conference;
using Microsoft.AspNetCore.Mvc;

namespace CreateAndAddConference.Controllers {
  [Route ("voice/")]
  [ApiController]
  public class ConferencesController : ControllerBase {
    private static string[] conferenceRoomCodes = { "1", "2", "3" };
    private static IDictionary<string, ConferenceRoom> conferenceRooms = new Dictionary<string, ConferenceRoom> ();

    static ConferencesController () {
      init ();
    }

    private static void init () {
      conferenceRooms.Clear ();
      // Add three ConferenceRooms to the map on initialization
      foreach (string code in conferenceRoomCodes) {
        conferenceRooms.Add (code, new ConferenceRoom ());
      }
    }

    [HttpPost ("InboundCall")] //POST /voice/InboundCall
    public ActionResult InboundCall (CallStatusCallback callStatus) {
      PerCLScript script = new PerCLScript ();
      var getDigitsActionUrl = getAppUrl() + "/voice/GetDigitsDone";
      GetDigits getDigits = new GetDigits (getDigitsActionUrl);
      getDigits.setMaxDigits (1);
      Say say = new Say ();
      say.setText ("Hello. Welcome to the conferences tutorial, please enter your access code.");
      getDigits.setPrompts (say);
      script.Add (getDigits);
      return Content (script.toJson (), "application/json");
    }

    [HttpPost ("GetDigitsDone")] // POST /voice/GetDigitsDone
    public ActionResult GetDigitsDone (GetDigitsActionCallback request) {
      // Make OutDial request once conference has been created
      PerCLScript script = new PerCLScript ();
      string callId = request.getCallId;
      string digits = request.getDigits;
      ConferenceRoom room = conferenceRooms[digits];
      if (room == null) {
        // Handle case where no room with the given code exists
      }

      // if participants can't be added yet (actionUrl callback has not been called) notify caller and hang up
      if (room.isConferencePending) {
        Say say = new Say ();
        say.setText ("We are sorry, you cannot be added to the conference at this time. Please try again later.");
        script.Add (say);
        script.Add (new Hangup ());
      } else {
        Say say = new Say ();
        say.setText ("You will be added to the conference momentarily.");
        script.Add (say);
        script.Add (makeOrAddToConference (room, digits, callId));
      }
      return Content (script.toJson (), "application/json");
    }
    private PerCLCommand makeOrAddToConference (ConferenceRoom room, String roomCode, String callId) {
      // If a conference has not been created for this room yet, return a CreateConference PerCL command
      if (room.conferenceId == null) {
        room.isConferencePending = true;
        room.canConferenceTerminate = false;
        var conferenceActionUrl = getAppUrl() + $"/voice/ConferenceCreated?roomCode={roomCode}";
        var conferenceStatusUrl = getAppUrl() + $"/voice/ConferenceStatus?roomCode={roomCode}";
        CreateConference createConference = new CreateConference (conferenceActionUrl);
        createConference.setStatusCallbackUrl (conferenceStatusUrl);
        return createConference;
      } else {
        // If a conference has been created and the actionUrl callback has been called, return a AddToConference PerCL command
        return new AddToConference (room.conferenceId, callId);
      }
    }

    [HttpPost("ConferenceCreated")] // POST /voice/ConferenceCreated
    public ActionResult ConferenceCreated ([FromQuery (Name = "roomCode")] string roomCode, ConferenceCreateActionCallback request) {
      PerCLScript script = new PerCLScript ();
      string conferenceId = request.getConferenceId;
      string callId = request.getCallId;
      // find which conference room the newly created conference belongs to
      ConferenceRoom room = conferenceRooms[roomCode];
      if (room == null) {
        // Handle case where callback is called for a room that does not exist
      }
      room.conferenceId = conferenceId;
      room.isConferencePending = false;

      Say welcomeToConference = new Say();
      welcomeToConference.setText("You are now being added to the conference");
      script.Add(welcomeToConference);
      // Add initial caller to conference
      script.Add (new AddToConference (conferenceId, request.getCallId));
      return Content (script.toJson (), "application/json");
    }

    [HttpPost("ConferenceStatus")]
    public ActionResult ConferenceStatus ([FromQuery (Name = "roomCode")] string roomCode, ConferenceStatusCallback request) {
      PerCLScript script = new PerCLScript ();
      EConferenceStatus status = request.getStatus;
      String conferenceId = request.getConferenceId;
      // find which conference room the conference belongs to
      ConferenceRoom room = conferenceRooms[roomCode];
      if (room == null) {
        // Handle case where callback is called for a room that does not exist
      }
      if (status == EConferenceStatus.Empty && room.canConferenceTerminate) {
        try {
          terminateConference (conferenceId);
          room.conferenceId = null;
        } catch (FreeClimbException pe) {
          // Handle error when terminateConference fails
        }
      }
      // after first EMPTY status update conference can be terminated
      room.canConferenceTerminate = true;
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

    private string getAcctId () {
      return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }

    private string getApiKey () {
      return System.Environment.GetEnvironmentVariable("API_KEY");
    }

    private string getAppUrl () {
      return System.Environment.GetEnvironmentVariable("HOST");
    }
  }
}
