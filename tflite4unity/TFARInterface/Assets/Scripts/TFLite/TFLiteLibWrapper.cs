using System;
using System.Runtime.InteropServices;

// TODO: Define type aliases later after having moved the structs.
namespace TFLite{

public static const string LIB_NAME = "libtensorflowlite_c";

public class InterpreterWrapper{

    #region Structs/Enums

    [StructLayout(LayoutKind.Sequential)]
    struct TfLiteQuantizationParams{
        public float scale;
        public int zeroPoint;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct TfLiteModel{
        IntPtr impl;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct TfLiteInterpreter{
        IntPtr model;
        IntPtr optional_error_reporter;
        IntPtr impl;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct TfLiteInterpreterOptions{
        enum Threads{
            kDefaultNumThreads = -1
        };

        int num_threads = Threads.kDefaultNumThreads;
        MutableOpResolver op_resolver;
        IntPtr error_reporter = IntPtr.Zero;
        IntPtr error_reporter_user_data = IntPtr.Zero;
        IntPtr delegates;
    };

    enum TfLiteStatus{
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
    public static extern unsafe TfLiteInterpreter TfLiteInterpreterCreate(
        IntPtr model,
        IntPtr optional_options);

    [DllImport(LIB_NAME)]
    public static extern unsafe void TfLiteInterpreterDelete(IntPtr interpreter);

    [DllImport(LIB_NAME)]
    public static extern unsafe int TfLiteInterpreterGetInputTensorCount(IntPtr interpreter);

    [DllImport(LIB_NAME)]
    public static extern unsafe TfLiteTensor TfLiteInterpreterGetInputTensor(
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
    public static extern unsafe TfLiteTensor TfLiteInterpreterGetOutputTensor(
        IntPtr interpreter,
        int output_index);

    [DllImport(LIB_NAME)]
    public static extern unsafe TfLiteType TfLiteTensorType(IntPtr tensor);

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


