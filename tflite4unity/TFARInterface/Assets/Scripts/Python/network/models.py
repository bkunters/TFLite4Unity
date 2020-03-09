import tensorflow as tf


# Models
'''
A standard custom pose and object detection model.
'''
def MobilePoseNet(output_classes):
    img_input = tf.keras.Input(shape=(224, 224, 3), name='input')
    conv_pose_1 = tf.keras.layers.Conv2D(kernel_size=(7,7), strides=2, filters=8 , activation='relu')(img_input)
    conv_pose_2 = tf.keras.layers.Conv2D(kernel_size=(7,7), strides=2, filters=16, activation='relu')(conv_pose_1)
    dropout_1   = tf.keras.layers.Dropout(rate=0.5)(conv_pose_2)
    conv_pose_3 = tf.keras.layers.Conv2D(kernel_size=(7,7), strides=2, filters=32, activation='relu')(dropout_1)
    conv_pose_4 = tf.keras.layers.Conv2D(kernel_size=(5,5), strides=2, filters=64, activation='relu')(conv_pose_3)

    conv_bbox_1 = tf.keras.layers.Conv2D(kernel_size=(3,3), filters=128 , activation='relu')(conv_pose_4)
    conv_bbox_2 = tf.keras.layers.Conv2D(kernel_size=(3,3), filters=256, activation='relu')(conv_bbox_1)
    dropout_2   = tf.keras.layers.Dropout(rate=0.5)(conv_bbox_2)
    conv_bbox_3 = tf.keras.layers.Conv2D(kernel_size=(3,3), filters=256, activation='relu')(dropout_2)
    conv_bbox_4 = tf.keras.layers.Conv2D(kernel_size=(3,3), filters=256, activation='relu')(conv_bbox_3)
    upsample_1  = tf.keras.layers.UpSampling2D(size=(48,48))(conv_bbox_4)
    conv_bbox_5 = tf.keras.layers.Conv2D(kernel_size=(3,3), filters=512 , activation='relu')(upsample_1)
    
    # MobileNetV2 is for classification
    mobile_net = tf.keras.applications.MobileNetV2(input_shape=(224, 224, 3), weights=None, classes=output_classes, include_top=True)(conv_bbox_5)
    output_id  = tf.keras.layers.Dense(output_classes, activation=tf.nn.softmax, name='class_output')(mobile_net)

    # Outputs the pose information
    flatten_pose = tf.keras.layers.Flatten()(conv_bbox_5)
    dense_pose_regression_out_1 = tf.keras.layers.Dense(1024, activation='relu')(flatten_pose)
    dense_pose_regression_out_2 = tf.keras.layers.Dense(512, activation='relu')(dense_pose_regression_out_1)
    dense_pose_regression_out_3 = tf.keras.layers.Dense(256, activation='relu')(dense_pose_regression_out_2)
    dense_pose_regression_out_4 = tf.keras.layers.Dense(128, activation='relu')(dense_pose_regression_out_3)
    dense_pose_regression_out_5 = tf.keras.layers.Dense(7, activation='relu', name='pose_output')(dense_pose_regression_out_4)

    # Outputs the bboxes
    flatten_bbox = tf.keras.layers.Flatten()(conv_bbox_5)
    dense_bbox_regression_out_1 = tf.keras.layers.Dense(1024, activation='elu')(flatten_bbox)
    dense_bbox_regression_out_2 = tf.keras.layers.Dense(512, activation='elu')(dense_bbox_regression_out_1)
    dense_bbox_regression_out_3 = tf.keras.layers.Dense(256, activation='elu')(dense_bbox_regression_out_2)
    dense_bbox_regression_out_4 = tf.keras.layers.Dense(128, activation='elu')(dense_bbox_regression_out_3)
    dropout_3                   = tf.keras.layers.Dropout(rate=0.3)(dense_bbox_regression_out_4)
    dense_bbox_regression_out_5 = tf.keras.layers.Dense(4, activation='elu', name='bbox_output')(dropout_3)

    return tf.keras.Model(inputs=[img_input], outputs=[output_id, dense_pose_regression_out_5, dense_bbox_regression_out_5])

'''
A capsule-based custom model for pose estimation.
'''
def MobilePoseCapsNet(output_classes):
    img_input = tf.keras.Input(shape=(224, 224, 3), name='input')
    #MobileNetV2 for classification
    mobile_net = tf.keras.applications.MobileNetV2(input_shape=(224, 224, 3), weights=None, classes=output_classes, include_top=True)(img_input)
    # todo: tests with 


def MobileHandKeypointNet():
    img_input = tf.keras.Input(shape=(256, 256, 3), name='input')
