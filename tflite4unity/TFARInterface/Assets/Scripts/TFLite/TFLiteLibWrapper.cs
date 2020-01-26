using System;
using System.Runtime.InteropServices;

/// Links:
/// 1) See the c_api.cc file
/// 2) https://docs.microsoft.com/en-us/dotnet/framework/interop/marshaling-classes-structures-and-unions
namespace TFLite{

// Type aliases.
using TfLiteModel               = IntPtr;
using TfLiteInterpreterOptions  = IntPtr;
using TfLiteDelegate            = IntPtr;
using TfLiteTensorData          = IntPtr;

private static const string LIB_NAME = "libtensorflowlite_c";

public class InterpreterWrapper{

    #region Structs/Enums

    [StructLayout(LayoutKind.Sequential)]
    struct TfLiteQuantizationParams{
        public float scale;
        public int zeroPoint;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct TfLiteModel{
        // TODO.
    };

    [StructLayout(LayoutKind.Sequential)]
    struct TfLiteInterpreter{
        // TODO.
    };

    [StructLayout(LayoutKind.Sequential)]
    struct TfLiteInterpreterOptions{
        // TODO.
    };

    enum TfLiteStatus{
        kTfLiteOk = 0,
        kTfLiteError = 1
    };

    #endregion

    #region Native Methods
    
    [DllImport(LIB_NAME)]
    private static extern unsafe string TfLiteVersion();

    [DllImport(LIB_NAME)]
    private static extern unsafe TfLiteInterpreter TfLiteModelCreate(IntPtr model_data, uint model_size);

    [DllImport(LIB_NAME)]
    private static extern unsafe TfLiteModel TfLiteModelCreateFromFile(string model_path);

    [DllImport(LIB_NAME)]
    private static extern unsafe TfLiteInterpreter TfLiteModelDelete(TfLiteModel model);

    [DllImport(LIB_NAME)]
    private static extern unsafe TfLiteInterpreterOptions TfLiteInterpreterOptionsCreate();

    [DllImport(LIB_NAME)]
    private static extern unsafe void TfLiteInterpreterOptionsDelete(TfLiteInterpreterOptions options);

    [DllImport(LIB_NAME)]
    private static extern unsafe void TfLiteInterpreterOptionsSetNumThreads(
        TfLiteInterpreterOptions options,
        int num_threads
    );

    [DllImport(LIB_NAME)]
    private static extern unsafe void TfLiteInterpreterOptionsAddDelegate(
        TfLiteInterpreterOptions options,
        TfLiteDelegate _delegate);

    [DllImport(LIB_NAME)]
    private static extern unsafe void TfLiteInterpreterOptionsSetErrorReporter(TfLiteInterpreterOptions options, IntPtr reporter, IntPtr user_data);

    [DllImport(LIB_NAME)]
    private static extern unsafe TfLiteInterpreter TfLiteInterpreterCreate(
        TfLiteModel model,
        TfLiteInterpreterOptions optional_options);

    [DllImport(LIB_NAME)]
    private static extern unsafe void TfLiteInterpreterDelete(TfLiteInterpreter interpreter);

    [DllImport(LIB_NAME)]
    private static extern unsafe int TfLiteInterpreterGetInputTensorCount(
        TfLiteInterpreter interpreter);

    [DllImport(LIB_NAME)]
    private static extern unsafe TfLiteTensor TfLiteInterpreterGetInputTensor(
        TfLiteInterpreter interpreter,
        int input_index);

    [DllImport(LIB_NAME)]
    private static extern unsafe int TfLiteInterpreterResizeInputTensor(
        TfLiteInterpreter interpreter,
        int input_index,
        int[] input_dims,
        int input_dims_size);

    [DllImport(LIB_NAME)]
    private static extern unsafe int TfLiteInterpreterAllocateTensors(
        TfLiteInterpreter interpreter);

    [DllImport(LIB_NAME)]
    private static extern unsafe int TfLiteInterpreterInvoke(TfLiteInterpreter interpreter);

    [DllImport(LIB_NAME)]
    private static extern unsafe int TfLiteInterpreterGetOutputTensorCount(
        TfLiteInterpreter interpreter);

    [DllImport(LIB_NAME)]
    private static extern unsafe TfLiteTensor TfLiteInterpreterGetOutputTensor(
        TfLiteInterpreter interpreter,
        int output_index);

    [DllImport(LIB_NAME)]
    private static extern unsafe DataType TfLiteTensorType(TfLiteTensor tensor);

    [DllImport(LIB_NAME)]
    private static extern unsafe int TfLiteTensorNumDims(TfLiteTensor tensor);

    [DllImport(LIB_NAME)]
    private static extern unsafe int TfLiteTensorDim(TfLiteTensor tensor, int dim_index);

    [DllImport(LIB_NAME)]
    private static extern unsafe uint TfLiteTensorByteSize(TfLiteTensor tensor);

    [DllImport(LIB_NAME)]
    private static extern unsafe IntPtr TfLiteTensorData(TfLiteTensorData tensor);

    [DllImport(LIB_NAME)]
    private static extern unsafe IntPtr TfLiteTensorName(TfLiteTensor tensor);

    [DllImport(LIB_NAME)]
    private static extern unsafe TfLiteQuantizationParams TfLiteTensorQuantizationParams(TfLiteTensor tensor);

    [DllImport(LIB_NAME)]
    private static extern unsafe TfLiteStatus TfLiteTensorCopyFromBuffer(
        TfLiteTensor tensor,
        IntPtr input_data,
        int input_data_size);

    [DllImport(LIB_NAME)]
    private static extern unsafe TfLiteStatus TfLiteTensorCopyToBuffer(
        TfLiteTensor tensor,
        IntPtr output_data,
        int output_data_size);

    #endregion

}

}


