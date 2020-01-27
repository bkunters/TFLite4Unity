using System;
using System.Runtime.InteropServices;

// TODO: define pointer aliases later and move the structs and enums to another class. 
namespace TFLite{
    
    public class GPUDelegateWrapper{

        private const string LIB_NAME = "libtensorflowlite_gpu_delegate";

        #region Structs/Enums

        // Encapsulated precision/compilation/runtime tradeoffs.
        public enum TfLiteGpuInferencePreference {
            // Delegate will be used only once, therefore, bootstrap/init time should
            // be taken into account.
            TFLITE_GPU_INFERENCE_PREFERENCE_FAST_SINGLE_ANSWER = 0,

            // Prefer maximizing the throughput. Same delegate will be used repeatedly on
            // multiple inputs.
            TFLITE_GPU_INFERENCE_PREFERENCE_SUSTAINED_SPEED = 1,
        };

        public enum TfLiteAllocationType{
            kTfLiteMemNone = 0,
            kTfLiteMmapRo,
            kTfLiteArenaRw,
            kTfLiteArenaRwPersistent,
            kTfLiteDynamic,
        };

        // Types supported by tensor
        public enum TfLiteType{
            kTfLiteNoType = 0,
            kTfLiteFloat32 = 1,
            kTfLiteInt32 = 2,
            kTfLiteUInt8 = 3,
            kTfLiteInt64 = 4,
            kTfLiteString = 5,
            kTfLiteBool = 6,
            kTfLiteInt16 = 7,
            kTfLiteComplex64 = 8,
            kTfLiteInt8 = 9,
            kTfLiteFloat16 = 10,
        };

        public enum TfLiteQuantizationType{
            // No Quantization.
            kTfLiteNoQuantization = 0,
            // Affine quantization (with support for per-channel quantization).
            // Corresponds to TfLiteAffineQuantization.
            kTfLiteAffineQuantization = 1,
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLitePtrUnion{
            public IntPtr i32;
            public IntPtr i64;
            public IntPtr f;
            public IntPtr raw;
            public IntPtr raw_const;
            public IntPtr uint8;
            public IntPtr b;
            public IntPtr i16;
            public IntPtr c64;
            public IntPtr int8;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteIntArray{
            public int size;
            public int[] data;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteFloatArray{
            // TODO.
        };
        
        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteQuantizationParams{
            public float scale;
            public int zero_point;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteTensor{
            // The data type specification for data stored in `data`. This affects
            // what member of `data` union should be used.
            public TfLiteType type;
            // A union of data pointers. The appropriate type should be used for a typed
            // tensor based on `type`.
            public TfLitePtrUnion data;
            // A pointer to a structure representing the dimensionality interpretation
            // that the buffer should have. NOTE: the product of elements of `dims`
            // and the element datatype size should be equal to `bytes` below.
            public TfLiteIntArray dims;

            // Quantization information.
            public TfLiteQuantizationParams _params;
            // How memory is mapped
            //  kTfLiteMmapRo: Memory mapped read only.
            //  i.e. weights
            //  kTfLiteArenaRw: Arena allocated read write memory
            //  (i.e. temporaries, outputs).
            public TfLiteAllocationType allocation_type;
            // The number of bytes required to store the data of this Tensor. I.e.
            // (bytes of each element) * dims[0] * ... * dims[n-1].  For example, if
            // type is kTfLiteFloat32 and dims = {3, 2} then
            // bytes = sizeof(float) * 3 * 2 = 4 * 3 * 2 = 24.
            public uint bytes;

            // An opaque pointer to a tflite::MMapAllocation
            public IntPtr allocation;

            // Null-terminated name of this tensor.
            public string name;

            // The delegate which knows how to handle `buffer_handle`.
            // WARNING: This is an experimental interface that is subject to change.
            public TfLiteDelegate _delegate;

            // An integer buffer handle that can be handled by `delegate`.
            // The value is valid only when delegate is not null.
            // WARNING: This is an experimental interface that is subject to change.
            public int buffer_handle;

            // If the delegate uses its own buffer (e.g. GPU memory), the delegate is
            // responsible to set data_is_stale to true.
            // `delegate->CopyFromBufferHandle` can be called to copy the data from
            // delegate buffer.
            // WARNING: This is an // experimental interface that is subject to change.
            public bool data_is_stale;

            // True if the tensor is a variable.
            public bool is_variable;

            // Quantization information. Replaces params field above.
            public TfLiteQuantization quantization;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteQuantization{
            // The type of quantization held by params.
            public TfLiteQuantizationType type;
            
            // Holds a reference to one of the quantization param structures specified
            // below.
            public IntPtr _params;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteDelegate{
            public IntPtr data;
            public IntPtr Prepare;
            public IntPtr CopyFromBufferHandle;
            public IntPtr CopyToBufferHandle;
            public IntPtr FreeBufferHandle;
            public long flags;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteGpuDelegateOptionsV2{
            // When set to zero, computations are carried out in maximal possible
            // precision. Otherwise, the GPU may quantify tensors, downcast values,
            // process in FP16 to increase performance. For most models precision loss is
            // warranted.
            public int is_precision_loss_allowed;
            
            // Preference is defined in TfLiteGpuInferencePreference.
            public int inference_preference;
        };
        #endregion

        #region Native Methods.
        [DllImport(LIB_NAME)]
        public static extern unsafe TfLiteDelegate TfLiteDelegateCreate();

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteFloatArrayCreate(int size);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteFloatArrayFree(IntPtr a);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteFloatArrayGetSizeInBytes(int size);

        [DllImport(LIB_NAME)]
        public static extern unsafe TfLiteGpuDelegateOptionsV2 TfLiteGpuDelegateOptionsV2Default();

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteGpuDelegateV2Create(IntPtr options);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteGpuDelegateV2Delete(IntPtr _delegate);

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteIntArrayCopy(IntPtr a);

        [DllImport(LIB_NAME)] 
        public static extern unsafe IntPtr TfLiteIntArrayCreate(int size);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteIntArrayEqual(IntPtr array, IntPtr b);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteIntArrayEqualsArray(IntPtr a, int b_size, int[] b_data);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteIntArrayFree(IntPtr a);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteIntArrayGetSizeInBytes(int size);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteQuantizationFree(IntPtr quantization);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteTensorDataFree(IntPtr t);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteTensorFree(IntPtr t);

        [DllImport(LIB_NAME)]   
        public static extern unsafe void TfLiteTensorRealloc(uint num_bytes, IntPtr tensor);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteTensorReset(TfLiteType type, string name, IntPtr dims,
                                                           TfLiteQuantizationParams quantization, IntPtr buffer,
                                                           uint size, TfLiteAllocationType allocation_type,
                                                           IntPtr allocation, bool is_variable,
                                                           IntPtr tensor);

        [DllImport(LIB_NAME)]  
        public static extern unsafe string TfLiteTypeGetName(TfLiteType type);
        #endregion

    }

}