import json
import numpy

file_path = "dataset/sample_data.json"
training_images = []
training_labels = []

# Opens the json file which contains the annotations.
def extract_data():
    # todo : append the training images as numpy data. 

    # save the training labels in a list.
    training_labels = []
    with open(file_path, "r") as file:
        json_data = json.loads(file.read())
        for i in json_data:
            training_labels.append(json_data[i]['regions']['0']['region_attributes'])

def structure_data():
    #todo : parse the necessary data separately( hand classification / gesture classification )
    hand_labels = []
    gesture_labels = []

#extract_data()
