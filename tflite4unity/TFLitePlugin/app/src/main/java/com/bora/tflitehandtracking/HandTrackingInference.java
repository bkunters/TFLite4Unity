package com.bora.tflitehandtracking;

import android.app.Activity;
import android.content.res.AssetFileDescriptor;
import android.util.Log;
import java.io.FileInputStream;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.MappedByteBuffer;
import java.nio.channels.FileChannel;
import java.util.Arrays;

import org.tensorflow.lite.Interpreter;
import org.tensorflow.lite.gpu.GpuDelegate;

/**
 * TODO: Port this small API to C++ for more efficient inference.
 * see https://stackoverflow.com/questions/49834875/problems-with-using-tensorflow-lite-c-api-in-android-studio-project/50332808#50332808.
 */
public class HandTrackingInference{

    private static final String TAG = "HandTrackingInterface";
    private boolean m_gpuSupport;
    private Interpreter tflite;
    private Thread inferenceThread;

    public HandTrackingInference(boolean m_gpuSupport){
        this.m_gpuSupport = m_gpuSupport;
    }

    /*
        Loads the tflite model into memory.
    */
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

            // Prepare the interpreter.
            GpuDelegate delegate = new GpuDelegate();
            Interpreter.Options options = (new Interpreter.Options()).addDelegate(delegate);
            MappedByteBuffer model = fileChannel.map(FileChannel.MapMode.READ_ONLY, startOffset, declaredLength);

            while(tflite == null){
                Log.e(TAG, "tlite is null!");

                if(m_gpuSupport) tflite = new Interpreter(model, options);
                else{
                    tflite = new Interpreter(model);
                    delegate.close();
                }
            }

            Log.d(TAG, "Model initialized.");
        }
        catch(IOException e){
            Log.e(TAG, e.toString());
        }
    }

    /*
        Runs inference using the input and the output.
     */
    public void RunInference(final byte[] inputBuffer){
        float[][] output = new float[1][63];
        InferenceThread thread = new InferenceThread(inputBuffer, output);
        thread.start();
    }

    private class InferenceThread extends Thread{

        private byte[] inputBuffer;
        private float[][] output;

        InferenceThread(byte[] inputBuffer, float[][] output){
            this.inputBuffer = inputBuffer;
            this.output = output;
        }

        @Override
        public void run() {
            tflite.run(Utils.convertRawByteDataToByteBuffer(inputBuffer), output);
        }
    }

}
