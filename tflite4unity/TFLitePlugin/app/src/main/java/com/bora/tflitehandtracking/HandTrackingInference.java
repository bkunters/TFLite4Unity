package com.bora.tflitehandtracking;

import android.app.Activity;
import android.content.res.AssetFileDescriptor;
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

import org.tensorflow.lite.Interpreter;
import org.tensorflow.lite.gpu.GpuDelegate;
import org.tensorflow.lite.nnapi.NnApiDelegate;

/**
 * TODO: Port this small API to C++ for more efficient inference.
 * see https://stackoverflow.com/questions/49834875/problems-with-using-tensorflow-lite-c-api-in-android-studio-project/50332808#50332808.
 */
public class HandTrackingInference{

    private static final String TAG = "HandTrackingInterface";
    private boolean m_gpuSupport;
    private Interpreter tflite;
    private float[][] output;
    private HandlerThread hThread;
    private Handler handler;

    public HandTrackingInference(boolean m_gpuSupport){
        this.m_gpuSupport = m_gpuSupport;

        hThread = new HandlerThread("Inference");
        hThread.start();
        Looper looper = hThread.getLooper();
        handler = new Handler(looper);

        output = new float[1][63];
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
            MappedByteBuffer model = fileChannel.map(FileChannel.MapMode.READ_ONLY, startOffset, declaredLength);

            while(tflite == null){
                Log.e(TAG, "tlite is null!");

                if(m_gpuSupport) {
                    GpuDelegate delegate = new GpuDelegate();
                    Interpreter.Options options = (new Interpreter.Options()).addDelegate(delegate);
                    tflite = new Interpreter(model, options);
                }
                else{
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

    /*
        Runs inference using the input and the output.
     */
    public void RunInference(final byte[] inputBuffer){
        handler.post(new Runnable() {
            @Override
            public void run() {
                tfliteInference(inputBuffer);
            }
        });
    }
    private void tfliteInference(byte[] inputBuffer){
        tflite.run(Utils.convertRawByteDataToByteBuffer(inputBuffer), output);
    }

}
