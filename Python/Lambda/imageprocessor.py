# Copyright 2017 Amazon.com, Inc. or its affiliates. All Rights Reserved.
# Licensed under the Amazon Software License (the "License"). You may not use this file except in compliance with the License. A copy of the License is located at
#     http://aws.amazon.com/asl/
# or in the "license" file accompanying this file. This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, express or implied. See the License for the specific language governing permissions and limitations under the License.
# Modified by Anastasia Victor to run with Unity 


from __future__ import print_function
import base64
import datetime
import time
import decimal
import uuid
import json
import cPickle
import boto3
import pytz
from pytz import timezone
from copy import deepcopy
import io
from PIL import Image


def load_config():
    '''Load configuration from file.''' 
    with open('imageprocessor-params.json', 'r') as conf_file:
        conf_json = conf_file.read()
        return json.loads(conf_json)

def convert_ts(ts, config):
    '''Converts a timestamp to the configured timezone. Returns a localized datetime object.'''
    #lambda_tz = timezone('US/Pacific')
    tz = timezone(config['timezone'])
    utc = pytz.utc
    
    utc_dt = utc.localize(datetime.datetime.utcfromtimestamp(ts))

    localized_dt = utc_dt.astimezone(tz)

    return localized_dt


def process_image(event, context):

    #Initialize clients
    rekog_client = boto3.client('rekognition')
    kinesis_client = boto3.client('kinesis')

    #Load config
    config = load_config()

    output_kinesis_stream = config["output_kinesis_stream"]
    rekog_attributes = ['ALL']
    rekog_features_blacklist = ("Landmarks", "Emotions", "Pose", "Quality", "BoundingBox", "Confidence")
    
    #format_str = '%m/%d/%Y %H:%M:%S' # The format string for datetime string conversion in Unity
    
    #Iterate on frames fetched from Kinesis
    for record in event['Records']:
        
        try:
            #from Unity
            decoded = base64.b64decode(record['kinesis']['data'])
            frame_package =json.loads(decoded)
            
            #from python
            # frame_package_b64 = record['kinesis']['data']
            # frame_package = cPickle.loads(base64.b64decode(frame_package_b64))
    
            img_bytes = base64.decodestring(frame_package["ImageBytes"])
            #print("these are the image bytes bloog"+img_bytes)
            approx_capture_ts = frame_package["ApproximateCaptureTime"]
            #approx_capture_ts = datetime.datetime.strptime(frame_package["ApproximateCaptureTime"], format_str)

            frame_count = frame_package["FrameCount"]

            now_ts = time.time()
    
            frame_id = str(uuid.uuid4())
            processed_timestamp = decimal.Decimal(now_ts)
            #approx_capture_timestamp = decimal.Decimal(approx_capture_ts)
            approx_capture_timestamp = str(approx_capture_ts)
    
            now = convert_ts(now_ts, config)
            year = now.strftime("%Y")
            mon = now.strftime("%m")
            day = now.strftime("%d")
            hour = now.strftime("%H")
            
            
            try:    
                detectfacesresponse = rekog_client.detect_faces(
                    Image={
                        'Bytes': img_bytes
                    },
                    Attributes=rekog_attributes,
                )
            except:
                print("error with passing data to rekog client ")
    
            try:
                searchfacesresponse = rekognition.search_faces_by_image(
                    CollectionId='gray-area-event',
                    Image={'Bytes':img_bytes}                                       
                )
            except:
                print("error with search faces response")


            for match in response['FaceMatches']:
                print (match['Face']['FaceId'],match['Face']['Confidence'])
                
                face = dynamodb.get_item(
                    TableName='gray_area_collection',  
                    Key={'RekognitionId': {'S': match['Face']['FaceId']}}
                    )
                
                if 'Item' in face:
                    print (face['Item']['FullName']['S'])
                else:
                    print ('no match found in person lookup')



            item = {
                'frame_id': frame_id,
                'processed_timestamp' : str(processed_timestamp),
                'approx_capture_timestamp' : str(approx_capture_timestamp),
                'rekog_face_details' : response['FaceDetails'],
                'rekog_orientation_correction' : 
                    response['OrientationCorrection'] 
                    if 'OrientationCorrection' in response else 'ROTATE_0'
            }
    
            # Write to stream
            response = kinesis_client.put_record(
                StreamName=output_kinesis_stream,
                Data=json.dumps(item),
                PartitionKey="partitionkey"
            )
            
        except Exception as e:
            print("Error processing Frame Stream:  "+ str(e))
            

    print('Successfully processed {} records.'.format(len(event['Records'])))
    return

def handler(event, context):
    return process_image(event, context)
