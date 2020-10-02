/**
Hello! This is the file that you should add to your Unity project to start using leaderboards. There are a
bunch of things in this file, most of which you don't need to worry about. Generally, you will be using a few
functions. For instance, to add a record to your leaderboard called Higher is Better, you would do the 
following.

```
var leaderboard = new Leaderboards.HigherIsBetterLeaderboard()
var enumartor = leaderboard.AddRecord(duration, 5);
while (enumartor.MoveNext())
{
    yield return null;
}
var value = (Leaderboards.AddRecordResponsePayload) enumartor.Current;
```

Feel free to make any changes to this file, it's yours!

If you have any problems or questions please feel free to reach out to me (Anthony) at anthonymdito@gamil.com
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Leaderboards
{
    

    /**
    This is used internally, don't worry about it :)
    */
    [Serializable]
    internal class MyTopRecordRequestPayload
    {
        public string app_id;
        public string leaderboard_id;
        public string user_id;

        public MyTopRecordRequestPayload(string appId, string leaderboardId, string userId)
        {
            app_id = appId;
            leaderboard_id = leaderboardId;
            user_id = userId;
        }
    }

    /**
    This is used internally, don't worry about it :)
    */
    [Serializable]
    internal class TopRecordsRequestPayload
    {
        public string app_id;
        public string leaderboard_id;
        public int num_records;

        public TopRecordsRequestPayload(string appId, string leaderboardId, int numRecords)
        {
            app_id = appId;
            leaderboard_id = leaderboardId;
            num_records = numRecords;
        }
    }

    /**
    This is used internally, don't worry about it :)
    */
    [Serializable]
    internal class CreateRecordRequestPayload
    {
        public string app_id;
        public string leaderboard_id;
        public string user_id;
        public float value;
        public int num_records;

        public CreateRecordRequestPayload(string appId, string leaderboardId, string userId, float value, int numRecords)
        {
            app_id = appId;
            leaderboard_id = leaderboardId;
            user_id = userId;
            num_records = numRecords;
            this.value = value;
        }
    }

    /**
    This is the class that represents a record for a leaderboard, one of the numbers
    associated with an entry against that leaderboard.
    */
    [Serializable]
    public class Record
    {
        [SerializeField]
        private string user_id;
        [SerializeField]
        private string unique_id;
        [SerializeField]
        private float value;
        [SerializeField]
        private int place;

        /**
        The unique ID of the user that created this record.
        */
        public string UserId
        {
            get
            {
                return user_id;
            }
        }

        /**
        The unique ID of the record itself.
        */
        public string UniqueId
        {
            get
            {
                return unique_id;
            }
        }

        /**
        The value of the record
        */
        public float Value
        {
            get
            {
                return value;
            }
        }

        /**
        The record's place in the leaderboard. With 1 being the best.
        */
        public int Place
        {
            get
            {
                return place;
            }
        }

        public bool IsCurrentUser
        {
            get
            {
                return UserId == User.Instance().Id;
            }
        }

    }

    /**
     * Base class for all of the responses from operations. Includes a value of
     * if it was an error or not.
     */
    public class LeaderboardResponse
    {
        public bool IsError = false;
    }

    /**
     * This is the class that represents the response for getting the top records
     * of a given leaderboard.
     */
    [Serializable]
    public class TopRecordsResponsePayload : LeaderboardResponse
    {
        [SerializeField]
        private List<Record> records;

        public List<Record> Records
        {
            get
            {
                return records;
            }
        }
    }

    /**
    This is the class that is that represents the payload when you as for the
    current user's top record.
    */
    [Serializable]
    public class MyTopRecordResponsePayload : LeaderboardResponse
    {
        [SerializeField]
        private string type;
        [SerializeField]
        private Record record;

        // If the user has finished a record on the leaderboard.
        public bool HasFinished
        {
            get
            {
                return type == "finished";
            }
        }

        // If has finished is true, the user's top record.
        public Record Record
        {
            get
            {
                return record;
            }
        }
    }

    /**
    The class that represents the response to adding a record
    */
    [Serializable]
    public class AddRecordResponsePayload : LeaderboardResponse
     {
        [SerializeField]
        private string type;
        [SerializeField]
        private Record top_record;
        [SerializeField]
        private List<Record> worse;
        [SerializeField]
        private List<Record> better;
        [SerializeField]

        // If it is the user's best score for this leaderboard.
        public bool IsBestScore
        {
            get
            {
                return type == "top_record";
            }
        }

        // The top record for this user.
        public Record TopRecord
        {
            get
            {
                return top_record;
            }
        }

        // The records around the value that the user got, but the better ones.
        public List<Record> Better
        {
            get
            {
                return better;
            }
        }

        // The records around the value that the user got, but the worse ones.
        public List<Record> Worse
        {
            get
            {
                return worse;
            }
        }
     }



    /**
    Represents a user. You can use this class to get the currently logged in user id.
    */
    public class User
    {

        private static string USER_ID_KEY = "com.anthonydito.atlanta.USER_ID_KEY";

        private static User _instance = null;

        public static User Instance()
        {
            if (_instance == null)
            {
                _instance = new User();
            }
            return _instance;
        }

        public string Id { get; private set; }

        private User()
        {
            var existingId = PlayerPrefs.GetString(USER_ID_KEY);
            if (existingId == "")
            {
                Guid Id = Guid.NewGuid();
                SetId(Id.ToString());
            } else
            {
                Id = existingId;
            }

        }

        public void SetId(string Id)
        {
            this.Id = Id;
            PlayerPrefs.SetString(USER_ID_KEY, Id);
        }
    }

    /**
    This is the base class for all the leaderboards. This class
    defines all the operations that you can do on a leaderboard.
    */
    public abstract class Leaderboard
    {

        private const string NETWORK_ERROR_CODE = "com.anthonydito.atlanta.NETWORK_ERROR_CODE";

        private const string API_TOKEN = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6eyJTIjoiYW50aG9ueW1kaXRvK2RlbW9AZ21haWwuY29tIn0sIm90cCI6eyJTIjoiNTY3NzQxIn0sIm90cF9leHBpcmVfdGltZSI6eyJOIjoiMTYwMTM0MzAwMC45MDM4MjUifX0.ZJZuSzeKDwSy7IGwzbAIy7FC4vnlxAFewaMJ7P1elrg";
        private const string BASE_URL = "https://v6tbbzg231.execute-api.us-east-1.amazonaws.com/prod";
        private const string APP_ID = "com.anthonydito.leaderboarddemo";

        // The human readable name of the leaderboard.
        public abstract string Name { get; }

        // Whether a lower score is higher or not.
        public abstract bool LowerIsBetter { get;  }
        
        // The unique identifier of this leaderboard
        public abstract string Id { get; }
        // Fetches the top records of a leaderboard.
        public IEnumerator TopRecords(int numRecords)
        {
            TopRecordsRequestPayload payload = new TopRecordsRequestPayload(APP_ID, Id, numRecords);
            var iterator = SendRequest("/top-records", payload);
            while (iterator.MoveNext())
            {
                yield return null;
            }
            string text = (string)iterator.Current;
            if (text == NETWORK_ERROR_CODE)
            {
                TopRecordsResponsePayload output = new TopRecordsResponsePayload
                {
                    IsError = true
                };
                yield return output;
            } else
            {
                TopRecordsResponsePayload response = JsonUtility.FromJson<TopRecordsResponsePayload>(text);
                yield return response;
            }
        }

        // Adds a record to the leaderboard
        public IEnumerator AddRecord(float value, int numRecords) {
            var userId = User.Instance().Id;
            CreateRecordRequestPayload payload = new CreateRecordRequestPayload(APP_ID, Id, userId, value, numRecords);
            var iterator = SendRequest("/create-record", payload);
            while (iterator.MoveNext())
            {
                yield return null;
            }
            string text = (string)iterator.Current;
            if (text == NETWORK_ERROR_CODE)
            {
                AddRecordResponsePayload output = new AddRecordResponsePayload
                {
                    IsError = true
                };
                yield return output;
            } else
            {
                AddRecordResponsePayload response = JsonUtility.FromJson<AddRecordResponsePayload>(text);
                yield return response;
            }
        }

        // Gets the top record of the current user
        public IEnumerator MyTopRecord()
        {
            var userId = User.Instance().Id;
            MyTopRecordRequestPayload payload = new MyTopRecordRequestPayload(APP_ID, Id, userId);
            var iterator = SendRequest("/my-top-record", payload);
            while (iterator.MoveNext())
            {
                yield return null;
            }
            string text = (string)iterator.Current;
            if (text == NETWORK_ERROR_CODE)
            {
                MyTopRecordResponsePayload output = new MyTopRecordResponsePayload
                {
                    IsError = true
                };
                yield return output;
            } else
            {
                MyTopRecordResponsePayload response = JsonUtility.FromJson<MyTopRecordResponsePayload>(text);
                yield return response;
            }
        }

        private IEnumerator SendRequest(string path, object payload)
        {
            string json = JsonUtility.ToJson(payload);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var request = new UnityWebRequest(BASE_URL + path, "POST")
            {
                uploadHandler = new UploadHandlerRaw(bytes),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("x-token", API_TOKEN);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }
            if (request.isNetworkError || request.isHttpError)
            {
                yield return NETWORK_ERROR_CODE;
            } else
            {
                string text = request.downloadHandler.text;
                yield return text;
            }
        }
    }
    
    
    /**
    This is the class used for interacting with HigherIsBetterLeaderboard.
    With an instance of this class you can get top records, add a record, etc.

    To get an instance of this class, write the following code:
    
    var myLeaderboard = new Leaderboards.HigherIsBetterLeaderboard
    */
    public class HigherIsBetterLeaderboard : Leaderboard {
        public override string Name => "Higher is Better";
        public override bool LowerIsBetter => false;
        public override string Id => "HigherIsBetterLeaderboard";
    }
    
    
    /**
    This is the class used for interacting with LowerIsBetterLeaderboard.
    With an instance of this class you can get top records, add a record, etc.

    To get an instance of this class, write the following code:
    
    var myLeaderboard = new Leaderboards.LowerIsBetterLeaderboard
    */
    public class LowerIsBetterLeaderboard : Leaderboard {
        public override string Name => "Lower is Better";
        public override bool LowerIsBetter => true;
        public override string Id => "LowerIsBetterLeaderboard";
    }
    
}