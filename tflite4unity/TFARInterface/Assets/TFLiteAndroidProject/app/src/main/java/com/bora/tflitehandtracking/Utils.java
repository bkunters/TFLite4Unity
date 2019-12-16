package com.bora.tflitehandtracking;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Log;

import java.lang.reflect.Type;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

/**
 * Helper class for subtasks.
 */
public class Utils {

    private static final int BATCH_SIZE = 1;
    private static final int inputSize = 256;
    private static final int CHANNEL_SIZE = 3;

    private static final String TAG = "Utils";

    /**
     * Converts a bitmap image to a byte buffer.
     * @param bitmap Input image.
     * @return The byte buffer representing the image.
     */
    public static ByteBuffer convertBitmapToByteBuffer(Bitmap bitmap) {
        ByteBuffer byteBuffer;
        boolean quant = false;

        // New scaled Bitmap for inference.
        Bitmap data = Bitmap.createScaledBitmap(bitmap, 256, 256, false);

        if(quant) {
            byteBuffer = ByteBuffer.allocateDirect(BATCH_SIZE * inputSize * inputSize * CHANNEL_SIZE);
        } else {
            // Float buffer.
            byteBuffer = ByteBuffer.allocateDirect(4 * BATCH_SIZE * inputSize * inputSize * CHANNEL_SIZE);
        }

        byteBuffer.order(ByteOrder.nativeOrder());


        int[] colorValues = new int[inputSize * inputSize];
        data.getPixels(colorValues, 0, data.getWidth(), 0, 0, data.getWidth(), data.getHeight());

        byteBuffer.rewind();
        int pixel = 0;

        // Fill the byte buffer.
        for (int i = 0; i < inputSize; ++i) {
            for (int j = 0; j < inputSize; ++j) {
                final int val = colorValues[pixel++];
                if(quant){
                    byteBuffer.put((byte) ((val >> 16) & 0xFF));    // Red
                    byteBuffer.put((byte) ((val >> 8) & 0xFF));     // Green
                    byteBuffer.put((byte) (val & 0xFF));            // Blue
                } else {
                    byteBuffer.putFloat((((val >> 16) & 0xFF)) / 255.0f);
                    byteBuffer.putFloat((((val >> 8) & 0xFF)) / 255.0f);
                    byteBuffer.putFloat((((val) & 0xFF)) / 255.0f);
                }

            }
        }

        return byteBuffer;
    }

    /**
     * Converts a texture into a byte buffer.
     * @param textureID The pointer address of the texture.
     * @return Byte buffer representing the texture.
     */
    public static ByteBuffer convert2DTextureToByteBuffer(long textureID) throws UnsupportedOperationException{
        // TODO: Use the texture ID  to convert the texture into a byte buffer.
        // TODO: https://forum.unity.com/threads/passing-texture2d-pointer-to-android-plugin-to-write-on.166126/.
        throw new UnsupportedOperationException("Texture conversions are currently not supported.");
    }

    /**
     * Converts a raw byte array to a byte buffer.
     * @param rawData raw input data.
     * @return Byte buffer representing the raw data.
     */
    public static ByteBuffer convertRawByteDataToByteBuffer(byte[] rawData){
        return convertBitmapToByteBuffer(BitmapFactory.decodeByteArray(rawData, 0, rawData.length));
    }

}
