package com.bora.tflitehandtracking;

import android.graphics.Bitmap;

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
    private static final float IMAGE_MEAN = 128.0f;
    private static final float IMAGE_STD = 128.0f;

    /**
     * Converts a bitmap image to a byte buffer.
     * @param bitmap Input image.
     * @return The byte buffer representing the image.
     */
    public static ByteBuffer convertBitmapToByteBuffer(Bitmap bitmap) {
        ByteBuffer byteBuffer;
        boolean quant = false;

        if(quant) {
            byteBuffer = ByteBuffer.allocateDirect(BATCH_SIZE * inputSize * inputSize * CHANNEL_SIZE);
        } else {
            // Float buffer.
            byteBuffer = ByteBuffer.allocateDirect(4 * BATCH_SIZE * inputSize * inputSize * CHANNEL_SIZE);
        }

        byteBuffer.order(ByteOrder.nativeOrder());
        byteBuffer.rewind();

        int[] colorValues = new int[inputSize * inputSize];
        bitmap.getPixels(colorValues, 0, bitmap.getWidth(), 0, 0, bitmap.getWidth(), bitmap.getHeight());
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
                    byteBuffer.putFloat((((val >> 16) & 0xFF)-IMAGE_MEAN)/IMAGE_STD);
                    byteBuffer.putFloat((((val >> 8) & 0xFF)-IMAGE_MEAN)/IMAGE_STD);
                    byteBuffer.putFloat((((val) & 0xFF)-IMAGE_MEAN)/IMAGE_STD);
                }

            }
        }

        return byteBuffer;
    }

    /**
     * Converts a texture into a byte buffer.
     * @param textureID
     * @return Byte buffer representing the texture.
     */
    public static ByteBuffer convert2DTextureToByteBuffer(int textureID) throws UnsupportedOperationException{
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
        ByteBuffer byteBuffer;
        boolean quant = false;

        if(quant) {
            byteBuffer = ByteBuffer.allocateDirect(BATCH_SIZE * inputSize * inputSize * CHANNEL_SIZE);
        } else {
            // Float buffer.
            byteBuffer = ByteBuffer.allocateDirect(4 * BATCH_SIZE * inputSize * inputSize * CHANNEL_SIZE);
        }

        byteBuffer.order(ByteOrder.nativeOrder());
        byteBuffer.rewind();

        int pixel = 0;
        // Fill the byte buffer.
        for (int i = 0; i < inputSize; ++i) {
            for (int j = 0; j < inputSize; ++j) {
                final byte val = rawData[pixel++];

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

}
