package com.bora.tflitehandtracking;

import android.app.Activity;

/**
 * Inference interface for tensorflow lite models.
 * @param <T> The output type.
 */
public interface InferenceInterface<T> {

    /**
     * Maps a model file into memory.
     * @param activity current activity.
     */
    void loadModelFile(final Activity activity);

    /**
     * Runs inference on the model.
     * @param input The input data.
     * @return Output data.
     */
    T RunInference(final Object input);

}
