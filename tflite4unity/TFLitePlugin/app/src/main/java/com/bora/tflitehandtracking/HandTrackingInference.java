package com.bora.tflitehandtracking;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.res.AssetFileDescriptor;
import android.graphics.Bitmap;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.Looper;
import android.util.Log;
import java.io.FileInputStream;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.MappedByteBuffer;
import java.nio.channels.FileChannel;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;

import org.tensorflow.lite.Interpreter;
import org.tensorflow.lite.gpu.GpuDelegate;
import org.tensorflow.lite.nnapi.NnApiDelegate;

/**
 * TODO: Port this small API to C++ for more efficient inference.
 * see https://stackoverflow.com/questions/49834875/problems-with-using-tensorflow-lite-c-api-in-android-studio-project/50332808#50332808.
 */
public class HandTrackingInference implements InferenceInterface<float[]>{

    private static final String TAG = "HandTrackingInterface";
    private Interpreter tflite;
    private boolean m_gpuSupport;
    public float[][] outputKeypoints = new float[1][63];  // 21 keypoint locations
    public float[][] outputHandscore = new float[1][1];  // confidence of the hand existence.

    private HandlerThread hThread;
    private Handler handler;

    public HandTrackingInference(boolean m_gpuSupport){
        this.m_gpuSupport = m_gpuSupport;

        hThread = new HandlerThread("Inference");
        hThread.start();
        Looper looper = hThread.getLooper();
        handler = new Handler(looper);
    }

    @Override
    public void loadModelFile(final Activity activity)
    {
        try{
            // Prepare the file.
            AssetFileDescriptor fileDescriptor = activity.getAssets().openFd("hand_3d.tflite");
            Log.d(TAG, "loadModelFile: " + fileDescriptor.toString());
            FileInputStream inputStream = new FileInputStream(fileDescriptor.getFileDescriptor());
            FileChannel fileChannel = inputStream.getChannel();
            long startOffset = fileDescriptor.getStartOffset();
            long declaredLength = fileDescriptor.getDeclaredLength();
            MappedByteBuffer model = fileChannel.map(FileChannel.MapMode.READ_ONLY, startOffset, declaredLength);

            while(tflite == null){
                Log.e(TAG, "tlite is null!");

                if(m_gpuSupport) {
                    // Initialize model with GPU support.
                    GpuDelegate delegate = new GpuDelegate();
                    Interpreter.Options options = (new Interpreter.Options()).addDelegate(delegate);
                    tflite = new Interpreter(model, options);
                }
                else{
                    // Initialize model with NNAPI support.
                    NnApiDelegate nnApiDelegate = new NnApiDelegate();
                    Interpreter.Options options = (new Interpreter.Options()).addDelegate(nnApiDelegate);
                    tflite = new Interpreter(model, options);
                }
            }

            Log.d(TAG, "Model initialized.");
        }
        catch(IOException e){
            Log.e(TAG, e.toString());
        }
    }

    // TODO: 24-Sep-19 Optimize inference.
    @Override
    public float[] RunInference(final Object inputBuffer){
        /*Runnable runnable = new Runnable() {
            @Override
            public void run() {
                tfliteInference(inputBuffer);
            }
        };*/
        //handler.post(runnable);

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
