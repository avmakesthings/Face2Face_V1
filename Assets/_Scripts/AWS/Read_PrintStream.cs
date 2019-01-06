using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

[RequireComponent(typeof(AWSClient))]
public class Read_PrintStream : MonoBehaviour {

    private AWSClient awsClient;
    private string streamName;
    private Coroutine co;
    bool stopPrinting;

    public Text ageText;
    public Text genderText;
	public Text emotionText;
    public Text mustacheText;
    public Text glassesText;
    public Text beardText;
    string emotionStr;


	// Use this for initialization
	void Start() {
        resetDataFields("");
        awsClient = this.GetComponent<AWSClient>();
        streamName = "AmazonRekognitionStreamOut";
	}

	void HandleError(Exception e){
		Debug.LogError(e);
	}



    IEnumerator ReadKinesisStream(){
		yield return null;

        try{
			
            awsClient.ReadStream(streamName, (response)=>{
				List<Amazon.Kinesis.Model.Record> records = response.Records;
				foreach(Amazon.Kinesis.Model.Record awsRecord in records){
                    if (stopPrinting)
                    {
                        return;
                    }
					try{
						string recordString = Encoding.ASCII.GetString(awsRecord.Data.ToArray());
						Rekog.Record record = Rekog.Record.Deserialize(recordString);

						if(record.rekog_face_details.Count > 0){
							printAge(record.rekog_face_details[0].AgeRange.Low, record.rekog_face_details[0].AgeRange.High);
							printGender(record.rekog_face_details[0].Gender.Value, record.rekog_face_details[0].Gender.Confidence);
							
							emotionStr = "";
							foreach(Rekog.Emotion emotion in record.rekog_face_details[0].Emotions){
								string s = printEmotion(emotion.Type,emotion.Confidence);
								emotionStr = emotionStr + s;
							}

							emotionText.text = emotionStr;
							printConfidence(record.rekog_face_details[0].Mustache.Confidence, record.rekog_face_details[0].Mustache.Value,mustacheText);
							printConfidence(record.rekog_face_details[0].Beard.Confidence,record.rekog_face_details[0].Beard.Value ,beardText);
							printConfidence(record.rekog_face_details[0].Eyeglasses.Confidence, record.rekog_face_details[0].Eyeglasses.Value, glassesText);

						}
					} catch(Exception e){
						HandleError(e);
					}
				}
			});
		}catch(Exception e){
			HandleError(e);
        }

	}


	void printAge(float lowAge, float highAge ){
		string s = String.Format("{0} - {1}", (int)lowAge, (int)highAge);
		ageText.text = s;
	}


	void printGender(string myGender, float confidence){
		string s = String.Format("{0}% {1}", (int)confidence, myGender);
		genderText.text = s;
	}


	void printConfidence(float confidence, bool value, Text text){
		string s;
		if(value){
			s = confidence.ToString("n2")+ "% Yes";
		}else{
			s = confidence.ToString("n2") + "% No";
		}
		text.text = s;
	}


	string printEmotion(string myEmotion, float confidence){
		string s = String.Format("{0}% {1} \n", confidence.ToString("n2"), myEmotion);
		return s;
	}

    public void resetDataFields(string resetString){
        
        emotionText.text = resetString;
        mustacheText.text = resetString;
        glassesText.text = resetString;
        beardText.text = resetString;
        ageText.text= resetString;
        genderText.text = resetString;
    }

    public void activate()
    {
        resetDataFields("");
        stopPrinting = false;
        co = StartCoroutine(ReadKinesisStream());
    }


    public void deactivate()
    {
        stopPrinting = true;
        StopCoroutine(co);
        resetDataFields("");

    }

}
