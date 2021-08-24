using com.freeclimb.api;
using com.freeclimb.api.conference;
using System.Collections.Generic;
using System;

namespace ListConferences
{
  class Program
  {

    static string getAcctId()
    {
      return System.Environment.GetEnvironmentVariable("ACCOUNT_ID");
    }

    static string getApiKey()
    {
      return System.Environment.GetEnvironmentVariable("API_KEY");
    }

    public static IList<Conference> GetConferencesList()
    {
      string acctId = getAcctId();
      string apiKey = getApiKey();
      IList<Conference> conferences = new List<Conference>();
      FreeClimbClient client = new FreeClimbClient(acctId, apiKey);
      // you can pass an option ConferenceSearchFilters class to the getConferences method to filter by certain criteria (e.g. alias, create date)
      ConferenceList conferenceList = client.getConferencesRequester.getConferences();
      if (conferenceList.getTotalSize > 0)
      {
        // Retrieve all pages of results
        while (conferenceList.getLocalSize < conferenceList.getTotalSize)
        {
          conferenceList.loadNextPage();
        }
        foreach (IFreeClimbCommon item in conferenceList.export())
        {
          Conference conf = item as Conference;
          // do whatever you need to do with conferene object which contains all the conference properties
          conferences.Add(conf);
        }
      }
      return conferences;
    }

    static void Main(string[] args)
    {
      IList<Conference> conferenceList = GetConferencesList();

      foreach (Conference conference in conferenceList)
      {
        Console.WriteLine(conference.getConferenceId);
      }
    }
  }
}
