# Tensorflow utilities for reusable actions
from __future__ import absolute_import, division, print_function, unicode_literals
import functools
import numpy as np
import tensorflow as tf
import json
import os
import h5py

'''
Creates tensorflow 2 dataset from rgb images in the image path.
Returns the created image dataset.
'''
def create_dataset_from_rgb_images(path, labels={}, batch_size=1, image_resize_x=256, image_resize_y=256, floating_point_img=True, image_format='png'):
    training_images = []

    for img in os.listdir(path):
        # Open the image file
        img = tf.io.read_file(os.path.join(path, img))
        # convert the compressed string to a 3D uint8 tensor
        if(image_format == 'png'):
            img = tf.image.decode_png(img, channels=3)
        # Use `convert_image_dtype` to convert to floats in the [0,1] range.
        if(floating_point_img):
            img = tf.image.convert_image_dtype(img, tf.float32)
        # resize the image to the desired size.
        img = tf.image.resize(img, [image_resize_x, image_resize_y])
        # add to the image dataset
        training_images.append(img.numpy())

    dataset = tf.data.Dataset.from_tensor_slices(({'input': training_images}, labels))
    dataset = dataset.batch(batch_size)
    return dataset

'''
Starts the training with the given model and tensorflow 2 dataset.
Returns the tensorflow history.
'''
def train_model(model=tf.keras.Model(inputs=None, outputs=None), optimizer=tf.keras.optimizers.Adam(learning_rate=1e-4), 
                learning_rate=1e-4, logdir='logs', checkpoint_path='model{epoch:04d}.h5', loss_funcs=[], metrics={},
                train_dataset=None, epochs=100):
    model.summary()
    tf.keras.utils.plot_model(model, 'model.png', show_shapes=True, show_layer_names=True)
    cp_callback = tf.keras.callbacks.ModelCheckpoint(filepath=checkpoint_path, save_weights_only=False, verbose=1, period=5)
    tensorboard_callback = tf.keras.callbacks.TensorBoard(log_dir=logdir)

    model.compile(optimizer=optimizer, loss=loss_funcs, metrics=metrics)
    history = model.fit(train_dataset, epochs=epochs, use_multiprocessing=True, callbacks=[cp_callback, tensorboard_callback])
    return history
    