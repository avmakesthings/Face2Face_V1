﻿//
// Copyright 2014-2015 Amazon.com, 
// Inc. or its affiliates. All Rights Reserved.
// 
// Licensed under the AWS Mobile SDK For Unity 
// Sample Application License Agreement (the "License"). 
// You may not use this file except in compliance with the 
// License. A copy of the License is located 
// in the "license" file accompanying this file. This file is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, express or implied. See the License 
// for the specific language governing permissions and 
// limitations under the License.
//

using UnityEngine;
using UnityEngine.UI;
using Amazon.Kinesis;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon;
using System.Text;
using Amazon.Kinesis.Model;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;


public struct PutRecordResponse{
	public int HttpStatusCode;	
	public string SequenceNumber;
	public ResponseMetadata ResponseMetadata;
}
public delegate void HandlePutRecordResponse(PutRecordResponse response);

public struct ListStreamsResponse{
	public int HttpStatusCode;	
	public ResponseMetadata ResponseMetadata;
	public List<string> StreamNames;
}
public delegate void HandleListStreamsResponse(ListStreamsResponse response);

public struct DescribeStreamResponse{
	public int HttpStatusCode;	
	public ResponseMetadata ResponseMetadata;
	public StreamDescription StreamDescription;
}
public delegate void HandleDescribeStreamResponse(DescribeStreamResponse response);

public struct ReadStreamResponse{
	public List<Amazon.Kinesis.Model.Record> Records;
}
public delegate void HandleReadStreamResponse(ReadStreamResponse response);


[RequireComponent(typeof(AWSConfig))]
public class AWSClient : MonoBehaviour
{

    private string IdentityPoolId;
    public Amazon.RegionEndpoint RegionEndpoint = RegionEndpoint.USWest2;


	#region private members

	private IAmazonKinesis _kinesisClient;
	private AWSCredentials _credentials;

	private AWSCredentials Credentials
	{
		get
		{
            if (_credentials == null){
                AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
                _credentials = new CognitoAWSCredentials(
                    IdentityPoolId,
                    RegionEndpoint
                ); 
                Debug.Log("Created Kinesis credentials");
            }

			return _credentials;
		}
	}

	private IAmazonKinesis Client
	{
		get
		{
			if (_kinesisClient == null)
			{
				_kinesisClient = new AmazonKinesisClient(
					Credentials, 
					RegionEndpoint
				);
                Debug.Log("Created Kinesis client");
			}
			return _kinesisClient;
		}
	}

	#endregion

    private void Awake()
    {
        IdentityPoolId = GetComponent<AWSConfig>().IdentityPoolId;

    }

    void Start()
    {

        UnityInitializer.AttachToGameObject(this.gameObject);
        Debug.Log("kinesis client =" + _kinesisClient);
        Debug.Log("kinesis client = " + _credentials);
    }


	# region Put Record
	/// <summary>
	/// Example method to demostrate Kinesis PutRecord. Puts a record with the data specified
	/// in the "Record Data" Text Input Field to the stream specified in the "Stream Name"
	/// Text Input Field.
	/// </summary>
	public void PutRecord(string record, string streamName, HandlePutRecordResponse cb)
	{	

        using (var memoryStream = new MemoryStream())
		using (var streamWriter = new StreamWriter(memoryStream))
		{
			streamWriter.Write(record);
			Client.PutRecordAsync(new PutRecordRequest
			{
				Data = memoryStream,
				PartitionKey = "partitionKey",
				StreamName = streamName
			},
			(responseObject) =>
			{
				if (responseObject.Exception == null)
				{
					cb(new PutRecordResponse(){
						ResponseMetadata = responseObject.Response.ResponseMetadata,
						HttpStatusCode = (int)responseObject.Response.HttpStatusCode,
						SequenceNumber = responseObject.Response.SequenceNumber
					});
				}
				else
				{
					Debug.LogError(responseObject.Exception);
					cb(new PutRecordResponse());
				}
			}
			);
		}
	}

	# endregion

	# region List Streams
	/// <summary>
	/// Example method to demostrate Kinesis ListStreams. Prints all of the Kinesis Streams
	/// that your Cognito Identity has access to.
	/// </summary>
	public void ListStreams( HandleListStreamsResponse cb)
	{
		Client.ListStreamsAsync(new ListStreamsRequest(),
		(responseObject) =>
		{
            if (responseObject.Exception == null)
			{
				cb(new ListStreamsResponse(){
					ResponseMetadata = responseObject.Response.ResponseMetadata,
					HttpStatusCode = (int)responseObject.Response.HttpStatusCode,
					StreamNames = responseObject.Response.StreamNames
				});
			}
			else
			{
                Debug.LogError(responseObject.Exception);
				cb(new ListStreamsResponse());
			}
		}
		);
	}

	# endregion

	# region Describe Stream
	/// <summary>
	/// Example method to demostrate Kinesis DescribeStream. Prints information about the
	/// stream specified in the "Stream Name" Text Input Field.
	/// </summary>
	public void DescribeStream(string StreamName, HandleDescribeStreamResponse cb)
	{
		Client.DescribeStreamAsync(new DescribeStreamRequest()
		{
			StreamName = StreamName
		},
		(responseObject) =>
		{
			if (responseObject.Exception == null)
			{
				cb(new DescribeStreamResponse(){
					ResponseMetadata = responseObject.Response.ResponseMetadata,
					HttpStatusCode = (int)responseObject.Response.HttpStatusCode,
					StreamDescription = responseObject.Response.StreamDescription
				});
			}
			else
			{
				Debug.LogError(responseObject.Exception);
				cb(new DescribeStreamResponse());
			}
		}
		);
	}

	# endregion

	# region ReadStream
	/// <summary>
	/// A coroutine which accepts a delegate function and calls it with new records as they arrive
	/// </summary>
	public void ReadStream(string StreamName, HandleReadStreamResponse cb)
	{
		Client.DescribeStreamAsync(new DescribeStreamRequest()
		{
			StreamName = StreamName
		},
		(responseObject) =>
		{
			if (responseObject.Exception == null)
			{
				List<Shard> shards = responseObject.Response.StreamDescription.Shards;
				string shardId = shards[0].ShardId;
				
				Client.GetShardIteratorAsync(new GetShardIteratorRequest()
					{
						StreamName = StreamName,
						ShardId = shardId,
						ShardIteratorType = ShardIteratorType.LATEST
					},
					(iteratorResponseObject)=>{
						if (responseObject.Exception == null)
						{
							string nextShardIterator = iteratorResponseObject.Response.ShardIterator;
							StartCoroutine(ReadShard(nextShardIterator, cb));
						}
						else {
							Debug.LogError(iteratorResponseObject.Exception);
							cb(new ReadStreamResponse());
						}
					}
				);
				
			}
			else
			{
				Debug.LogError(responseObject.Exception);
				cb(new ReadStreamResponse());
			}
		}
		);
	}
	

	IEnumerator ReadShard(string nextShardIterator, HandleReadStreamResponse cb)
	{
		while(true){
			Client.GetRecordsAsync(new GetRecordsRequest()
			{
				ShardIterator=nextShardIterator,
				Limit=123
			},
			(getRecordsResponseObject)=>{
				if (getRecordsResponseObject.Exception == null)
				{
					List<Record> records = getRecordsResponseObject.Response.Records;

					cb(new ReadStreamResponse(){
						Records=records
					});

					nextShardIterator = getRecordsResponseObject.Response.NextShardIterator;
				}
				else
				{
					Debug.LogError(getRecordsResponseObject.Exception);
					cb(new ReadStreamResponse());
				}
			}
		);

		// TODO: Make the duration of 'waitForSeconds'
		// contingent on response in getRecords
		// if (records.Count == 0){
		// 	System.Threading.Thread.Sleep(1000);
		// }
		yield return new WaitForSeconds(1f);
		}

	}

	# endregion
}
