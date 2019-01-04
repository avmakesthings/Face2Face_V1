using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


/* Example JSON 
{  
   "_capture_timestamp":"1529665475.761024951934814453125",
   "rekog_face_details":[  
      {  
         "Confidence":99.9939956665039,
         "Eyeglasses":{  
            "Confidence":99.99996185302734,
            "Value":true
         },
         "Sunglasses":{  
            "Confidence":90.12786102294922,
            "Value":false
         },
         "Gender":{  
            "Confidence":99.92919921875,
            "Value":"Male"
         },
         "Landmarks":[  
            {  
               "Y":0.1884916126728058,
               "X":0.6964147686958313,
               "Type":"eyeLeft"
            },
            {  
               "Y":0.22158221900463104,
               "X":0.7846721410751343,
               "Type":"eyeRight"
            },
            {  
               "Y":0.311414510011673,
               "X":0.7379201650619507,
               "Type":"nose"
            },
            {  
               "Y":0.3826170861721039,
               "X":0.684109091758728,
               "Type":"mouthLeft"
            },
            {  
               "Y":0.4059264361858368,
               "X":0.7546648979187012,
               "Type":"mouthRight"
            },
            {  
               "Y":0.19118919968605042,
               "X":0.7001135349273682,
               "Type":"leftPupil"
            },
            {  
               "Y":0.22685159742832184,
               "X":0.7895158529281616,
               "Type":"rightPupil"
            },
            {  
               "Y":0.14252139627933502,
               "X":0.6705896258354187,
               "Type":"leftEyeBrowLeft"
            },
            {  
               "Y":0.14325246214866638,
               "X":0.7015866041183472,
               "Type":"leftEyeBrowUp"
            },
            {  
               "Y":0.16793885827064514,
               "X":0.7302860021591187,
               "Type":"leftEyeBrowRight"
            },
            {  
               "Y":0.18048016726970673,
               "X":0.7643682360649109,
               "Type":"rightEyeBrowLeft"
            },
            {  
               "Y":0.1785452663898468,
               "X":0.7940576076507568,
               "Type":"rightEyeBrowUp"
            },
            {  
               "Y":0.19685575366020203,
               "X":0.8220421075820923,
               "Type":"rightEyeBrowRight"
            },
            {  
               "Y":0.18486985564231873,
               "X":0.680860698223114,
               "Type":"leftEyeLeft"
            },
            {  
               "Y":0.195289745926857,
               "X":0.711781919002533,
               "Type":"leftEyeRight"
            },
            {  
               "Y":0.17968061566352844,
               "X":0.697364866733551,
               "Type":"leftEyeUp"
            },
            {  
               "Y":0.19571442902088165,
               "X":0.6955580711364746,
               "Type":"leftEyeDown"
            },
            {  
               "Y":0.2188713550567627,
               "X":0.7707788348197937,
               "Type":"rightEyeLeft"
            },
            {  
               "Y":0.2265983372926712,
               "X":0.7988052368164062,
               "Type":"rightEyeRight"
            },
            {  
               "Y":0.2131195366382599,
               "X":0.7849243879318237,
               "Type":"rightEyeUp"
            },
            {  
               "Y":0.2288922518491745,
               "X":0.784299910068512,
               "Type":"rightEyeDown"
            },
            {  
               "Y":0.31934893131256104,
               "X":0.7109766602516174,
               "Type":"noseLeft"
            },
            {  
               "Y":0.3327941298484802,
               "X":0.7505560517311096,
               "Type":"noseRight"
            },
            {  
               "Y":0.374388188123703,
               "X":0.7241535186767578,
               "Type":"mouthUp"
            },
            {  
               "Y":0.43112221360206604,
               "X":0.7212003469467163,
               "Type":"mouthDown"
            }
         ],
         "Pose":{  
            "Yaw":8.713628768920898,
            "Roll":10.918479919433594,
            "Pitch":0.9100388288497925
         },
         "Emotions":[  
            {  
               "Confidence":41.106834411621094,
               "Type":"HAPPY"
            },
            {  
               "Confidence":12.424784660339355,
               "Type":"CONFUSED"
            },
            {  
               "Confidence":2.768951654434204,
               "Type":"CALM"
            }
         ],
         "AgeRange":{  
            "High":38,
            "Low":23
         },
         "EyesOpen":{  
            "Confidence":51.91926956176758,
            "Value":true
         },
         "BoundingBox":{  
            "Width":0.24687500298023224,
            "Top":0.06777777522802353,
            "Left":0.6043750047683716,
            "Height":0.4399999976158142
         },
         "Smile":{  
            "Confidence":88.3674087524414,
            "Value":true
         },
         "MouthOpen":{  
            "Confidence":98.66913604736328,
            "Value":false
         },
         "Quality":{  
            "Sharpness":99.47134399414062,
            "Brightness":27.750926971435547
         },
         "Mustache":{  
            "Confidence":99.92317199707031,
            "Value":true
         },
         "Beard":{  
            "Confidence":99.99678802490234,
            "Value":true
         }
      }
   ],
   "processed_timestamp":"1529690739.9323089122772216796875"
}
*/

namespace Rekog{

    public struct BooleanFaceDetail {
        public float Confidence;
        public bool Value;
    }

    public struct StringFaceDetail {
        public float Confidence;
        public string Value;
    }

    public struct Landmark {
        public float Y;
        public float X;
        public string Type;
    }

    public struct Pose{
        public float Yaw;        
        public float Roll;
        public float Pitch;
    }

    public struct Emotion {
        public float Confidence;
        public string Type;
    }

    public struct AgeRange{
        public int High;
        public int Low;
    }

    public struct BoundingBox{
        public float Width;
        public float Top;
        public float Left;
        public float Height;
    }

    public struct Quality{
        public float Sharpness;
        public float Brightness;
    }


    public struct Face{
        public string FaceID;
        public BoundingBox boundingBox;
        public string ImageID;
        public string ExternalImageID;
        public float Confidence;
    }


    public struct FaceMatches{
        
        public float Similarity;
        public Face face;
    }


    public struct FaceDetails{
        public float Confidence;
        public BooleanFaceDetail Eyeglasses;
        public BooleanFaceDetail Sunglasses;
        public StringFaceDetail Gender;
        public List<Landmark> Landmarks;
        public Pose Pose;
        public List<Emotion> Emotions;
        public AgeRange AgeRange;

        public BooleanFaceDetail EyesOpen;
        public BoundingBox BoundingBox;
        public BooleanFaceDetail Smile;
        public BooleanFaceDetail MouthOpen;
        public Quality Quality;
        public BooleanFaceDetail Mustache;
        public BooleanFaceDetail Beard;

    }

    [System.Serializable]
    public class Record
    {
        public string rekog_orientation_correction;
        public string frame_id;
        public string approx_capture_timestamp;
        public string processed_timestamp;
        public string dynamodb_face_match_name;
        public List<FaceDetails> rekog_face_details;
        public List<FaceMatches> rekog_face_matches;

        public static Record Deserialize(string jsonString)
        {   
            return JsonConvert.DeserializeObject<Record>(jsonString);
        }

        public string Serialize(){
            return JsonConvert.SerializeObject(this);
        }

    }
}


public class RecordTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		string sampleJSON = "{\"_capture_timestamp\":\"1529665475.761024951934814453125\",\"rekog_face_details\":[{\"Confidence\":99.9939956665039,\"Eyeglasses\":{\"Confidence\":99.99996185302734,\"Value\":true},\"Sunglasses\":{\"Confidence\":90.12786102294922,\"Value\":false},\"Gender\":{\"Confidence\":99.92919921875,\"Value\":\"Male\"},\"Landmarks\":[{\"Y\":0.1884916126728058,\"X\":0.6964147686958313,\"Type\":\"eyeLeft\"},{\"Y\":0.22158221900463104,\"X\":0.7846721410751343,\"Type\":\"eyeRight\"},{\"Y\":0.311414510011673,\"X\":0.7379201650619507,\"Type\":\"nose\"},{\"Y\":0.3826170861721039,\"X\":0.684109091758728,\"Type\":\"mouthLeft\"},{\"Y\":0.4059264361858368,\"X\":0.7546648979187012,\"Type\":\"mouthRight\"},{\"Y\":0.19118919968605042,\"X\":0.7001135349273682,\"Type\":\"leftPupil\"},{\"Y\":0.22685159742832184,\"X\":0.7895158529281616,\"Type\":\"rightPupil\"},{\"Y\":0.14252139627933502,\"X\":0.6705896258354187,\"Type\":\"leftEyeBrowLeft\"},{\"Y\":0.14325246214866638,\"X\":0.7015866041183472,\"Type\":\"leftEyeBrowUp\"},{\"Y\":0.16793885827064514,\"X\":0.7302860021591187,\"Type\":\"leftEyeBrowRight\"},{\"Y\":0.18048016726970673,\"X\":0.7643682360649109,\"Type\":\"rightEyeBrowLeft\"},{\"Y\":0.1785452663898468,\"X\":0.7940576076507568,\"Type\":\"rightEyeBrowUp\"},{\"Y\":0.19685575366020203,\"X\":0.8220421075820923,\"Type\":\"rightEyeBrowRight\"},{\"Y\":0.18486985564231873,\"X\":0.680860698223114,\"Type\":\"leftEyeLeft\"},{\"Y\":0.195289745926857,\"X\":0.711781919002533,\"Type\":\"leftEyeRight\"},{\"Y\":0.17968061566352844,\"X\":0.697364866733551,\"Type\":\"leftEyeUp\"},{\"Y\":0.19571442902088165,\"X\":0.6955580711364746,\"Type\":\"leftEyeDown\"},{\"Y\":0.2188713550567627,\"X\":0.7707788348197937,\"Type\":\"rightEyeLeft\"},{\"Y\":0.2265983372926712,\"X\":0.7988052368164062,\"Type\":\"rightEyeRight\"},{\"Y\":0.2131195366382599,\"X\":0.7849243879318237,\"Type\":\"rightEyeUp\"},{\"Y\":0.2288922518491745,\"X\":0.784299910068512,\"Type\":\"rightEyeDown\"},{\"Y\":0.31934893131256104,\"X\":0.7109766602516174,\"Type\":\"noseLeft\"},{\"Y\":0.3327941298484802,\"X\":0.7505560517311096,\"Type\":\"noseRight\"},{\"Y\":0.374388188123703,\"X\":0.7241535186767578,\"Type\":\"mouthUp\"},{\"Y\":0.43112221360206604,\"X\":0.7212003469467163,\"Type\":\"mouthDown\"}],\"Pose\":{\"Yaw\":8.713628768920898,\"Roll\":10.918479919433594,\"Pitch\":0.9100388288497925},\"Emotions\":[{\"Confidence\":41.106834411621094,\"Type\":\"HAPPY\"},{\"Confidence\":12.424784660339355,\"Type\":\"CONFUSED\"},{\"Confidence\":2.768951654434204,\"Type\":\"CALM\"}],\"AgeRange\":{\"High\":38,\"Low\":23},\"EyesOpen\":{\"Confidence\":51.91926956176758,\"Value\":true},\"BoundingBox\":{\"Width\":0.24687500298023224,\"Top\":0.06777777522802353,\"Left\":0.6043750047683716,\"Height\":0.4399999976158142},\"Smile\":{\"Confidence\":88.3674087524414,\"Value\":true},\"MouthOpen\":{\"Confidence\":98.66913604736328,\"Value\":false},\"Quality\":{\"Sharpness\":99.47134399414062,\"Brightness\":27.750926971435547},\"Mustache\":{\"Confidence\":99.92317199707031,\"Value\":true},\"Beard\":{\"Confidence\":99.99678802490234,\"Value\":true}}],\"processed_timestamp\":\"1529690739.9323089122772216796875\"}";

        Rekog.Record record = Rekog.Record.Deserialize(sampleJSON);
        //Debug.Log(record.Serialize());
	}
	
}
