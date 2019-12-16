package com.bora.tflitehandtracking;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.res.AssetFileDescriptor;
import android.graphics.Bitmap;
import android.util.Log;
import java.io.FileInputStream;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.MappedByteBuffer;
import java.nio.channels.FileChannel;
import java.util.HashMap;
import java.util.Map;
import org.tensorflow.lite.Interpreter;
import org.tensorflow.lite.gpu.GpuDelegate;

/**
 * Represents the inference class for 3d hand tracking from rgb image.
 */
public class HandTrackingInference3D implements InferenceInterface<float[]>{

    private static final String TAG = "HandTrackingInterface3d";
    private final String MODEL_NAME = "hand_3d.tflite";
    private Interpreter tflite;
    public float[][] outputKeypoints = new float[1][63];  // 21 keypoint vectors.
    public float[][] outputHandscore = new float[1][1];   // confidence of the hand existence.

    /**
     * TODO: remove later.
     * @param m_gpuSupport
     */
    public HandTrackingInference3D(boolean m_gpuSupport){}

    @Override
    public void loadModelFile(final Activity activity)
    {
        try{
            // Prepare the file.
            AssetFileDescriptor fileDescriptor = activity.getAssets().openFd(MODEL_NAME);
            Log.d(TAG, "loadModelFile: " + fileDescriptor.toString());
            FileInputStream inputStream = new FileInputStream(fileDescriptor.getFileDescriptor());
            FileChannel fileChannel = inputStream.getChannel();
            long startOffset = fileDescriptor.getStartOffset();
            long declaredLength = fileDescriptor.getDeclaredLength();
            MappedByteBuffer model = fileChannel.map(FileChannel.MapMode.READ_ONLY, startOffset, declaredLength);

            while(tflite == null){
                Log.e(TAG, "tflite is null!");

                // Initialize model with GPU support.
                GpuDelegate delegate = new GpuDelegate();
                Interpreter.Options options = (new Interpreter.Options()).addDelegate(delegate);
                tflite = new Interpreter(model, options);
            }

            Log.d(TAG, "Model initialized.");
        }
        catch(IOException e){
            Log.e(TAG, e.toString());
        }
    }

    @Override
    public float[] RunInference(final Object inputBuffer){

        ByteBuffer buffer = null;
        if(inputBuffer instanceof byte[] || inputBuffer instanceof Byte[]){
            buffer = Utils.convertRawByteDataToByteBuffer((byte[])inputBuffer);
        }
        else if(inputBuffer instanceof Bitmap){
            buffer = Utils.convertBitmapToByteBuffer((Bitmap)inputBuffer);
        }
        else if(inputBuffer instanceof Long){
            buffer = Utils.convert2DTextureToByteBuffer((Long)inputBuffer);
        }

        Object[] imgData = {buffer};

        @SuppressLint("UseSparseArrays")
        Map<Integer, Object> outputMap = new HashMap<>();
        outputMap.put(0, outputKeypoints);
        outputMap.put(1, outputHandscore);
        tflite.runForMultipleInputsOutputs(imgData, outputMap);

        return null;
    }

    /**
     * @return Hand existence confidence.
     */
    public float getHandscore(){
        return outputHandscore[0][0];
    }

    /**
     * @return Keypoint locations.
     */
    public float[] getKeypoints(){
        return outputKeypoints[0];
    }

}
