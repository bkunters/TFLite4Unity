using System;
using System.Runtime.InteropServices;

// TODO: Define type aliases later after having moved the structs.
namespace TFLite{

    public class InterpreterWrapper{

        private const string LIB_NAME = "libtensorflowlite_c";

        #region Structs/Enums

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteQuantizationParams{
            public float scale;
            public int zeroPoint;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteModel{
            IntPtr impl;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteInterpreter{
            public IntPtr model;
            public IntPtr optional_error_reporter;
            public IntPtr impl;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TfLiteInterpreterOptions{
            public enum Threads{
                kDefaultNumThreads = -1
            };

            public int num_threads;
            //public MutableOpResolver op_resolver;
            public IntPtr error_reporter;
            public IntPtr error_reporter_user_data;
            public IntPtr delegates;
        };

        public enum TfLiteStatus{
            kTfLiteOk = 0,
            kTfLiteError = 1
        };

        #endregion

        #region Native Methods

        [DllImport(LIB_NAME)]
        public static extern unsafe string TfLiteVersion();

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteModelCreate(IntPtr model_data, uint model_size);

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteModelCreateFromFile(string model_path);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteModelDelete(IntPtr model);

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteInterpreterOptionsCreate();

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteInterpreterOptionsDelete(IntPtr options);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteInterpreterOptionsSetNumThreads(
            IntPtr options,
            int num_threads
        );

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteInterpreterOptionsAddDelegate(
            IntPtr options,
            IntPtr _delegate);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteInterpreterOptionsSetErrorReporter(IntPtr options, IntPtr reporter, IntPtr user_data);

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteInterpreterCreate(
            IntPtr model,
            IntPtr optional_options);

        [DllImport(LIB_NAME)]
        public static extern unsafe void TfLiteInterpreterDelete(IntPtr interpreter);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteInterpreterGetInputTensorCount(IntPtr interpreter);

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteInterpreterGetInputTensor(
            IntPtr interpreter,
            int input_index);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteInterpreterResizeInputTensor(
            IntPtr interpreter,
            int input_index,
            IntPtr input_dims,
            int input_dims_size);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteInterpreterAllocateTensors(
            IntPtr interpreter);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteInterpreterInvoke(IntPtr interpreter);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteInterpreterGetOutputTensorCount(
            IntPtr interpreter);

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteInterpreterGetOutputTensor(
            IntPtr interpreter,
            int output_index);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteTensorType(IntPtr tensor);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteTensorNumDims(IntPtr tensor);

        [DllImport(LIB_NAME)]
        public static extern unsafe int TfLiteTensorDim(IntPtr tensor, int dim_index);

        [DllImport(LIB_NAME)]
        public static extern unsafe uint TfLiteTensorByteSize(IntPtr tensor);

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteTensorData(IntPtr tensor);

        [DllImport(LIB_NAME)]
        public static extern unsafe IntPtr TfLiteTensorName(IntPtr tensor);

        [DllImport(LIB_NAME)]
        public static extern unsafe TfLiteQuantizationParams TfLiteTensorQuantizationParams(IntPtr tensor);

        [DllImport(LIB_NAME)]
        public static extern unsafe TfLiteStatus TfLiteTensorCopyFromBuffer(
            IntPtr tensor,
            IntPtr input_data,
            int input_data_size);

        [DllImport(LIB_NAME)]
        public static extern unsafe TfLiteStatus TfLiteTensorCopyToBuffer(
            IntPtr tensor,
            IntPtr output_data,
            int output_data_size);

        #endregion

    }

}


