import tensorflow as tf
from tensorflow.keras.layers import Dense, Conv2D, Input
from tensorflow.keras.models import Model

# Creates the functional model.
def create_model():
    input = Input(shape=(256, 256, 3), name='input')
    conv1 = Conv2D(filters=8,  strides=(2,2), kernel_size=7, data_format="channels_last", activation='relu', name='conv1')(input)
    conv2 = Conv2D(filters=16,  strides=(2,2), kernel_size=5, activation='relu', name='conv2')(conv1)
    conv3 = Conv2D(filters=32, strides=(2,2), kernel_size=3, activation='relu', name='conv3')(conv2)

    # submodel 1
    # todo:add dropouts depending on the learning accuracy
    conv_sub1_1 = Conv2D(filters=64, strides=(2,2) ,kernel_size=7, activation='relu', name='hand_conv1')(conv3)
    conv_sub1_2 = Conv2D(filters=128, strides=(2,2), kernel_size=5, activation='relu', name='hand_conv2')(conv_sub1_1)
    dense_sub1  =  Dense(units=2,  activation='softmax', name='hand_output')(conv_sub1_2)

    # submodel 2
    # todo:add dropouts depending on the learning accuracy
    conv_sub2_1 = Conv2D(filters=64, strides=(2,2), kernel_size=7, activation='relu', name='gesture_conv1')(conv3)
    conv_sub2_2 = Conv2D(filters=128, strides=(2,2), kernel_size=5, activation='relu', name='gesture_conv2')(conv_sub2_1)
    dense_sub2  =  Dense(units=6, activation='softmax', name='gesture_output')(conv_sub2_2)

    model = Model([input], [dense_sub1, dense_sub2])
    tf.keras.utils.plot_model(model, show_shapes=True)
    return model
