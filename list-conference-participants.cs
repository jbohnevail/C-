using System;
using System.Collections.Generic;
using com.freeclimb.api;
using com.freeclimb.api.conference;

namespace ListConferenceParticipants {
  class Program {
    static void Main (string[] args) {
      string conferenceId = "";
      IList<Participant> participants = GetConferenceParticipantsList(conferenceId);

      if(participants.Count == 0) {
        Console.WriteLine($"There are no participants in the conference: {conferenceId}");
      } else {
        foreach (Participant particpant in participants) {
          Console.WriteLine(particpant.getCallId);
        }
      }
    }

    public static IList<Participant> GetConferenceParticipantsList (string conferenceId) {
      string acctId = getAcctId ();
      string apiKey = getApiKey ();
      IList<Participant> ret = new List<Participant> ();
      FreeClimbClient client = new FreeClimbClient (acctId, apiKey);
      // the last two parameters are to filter on the participants talk and listen properties respectively
      ParticipantList participantList = client.getConferencesRequester.getParticipants (conferenceId);
      if (participantList.getTotalSize > 0) {
        // Retrieve all pages of results
        while (participantList.getLocalSize < participantList.getTotalSize) {
          participantList.loadNextPage ();
        }
        foreach (IFreeClimbCommon item in participantList.export ()) {
          Participant participant = item as Participant;
          // do whatever you need to do with participant object which contains all the conference properties
          ret.Add (participant);
        }
      }
      return ret;
    }

    private static string getAcctId () {
      return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }
    private static string getApiKey () {
      return System.Environment.GetEnvironmentVariable("API_KEY");
    }
  }
}
