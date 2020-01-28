using System;
using System.Runtime.InteropServices;

#if DEBUG
using UnityEngine;
#endif

namespace TFLite{

    #region Aliases
    using Model              = InterpreterWrapper.TfLiteModel;
    using QuantizationParams = InterpreterWrapper.TfLiteQuantizationParams;
    using InterpreterStr     = InterpreterWrapper.TfLiteInterpreter;
    using Options            = InterpreterWrapper.TfLiteInterpreterOptions;
    using Status             = InterpreterWrapper.TfLiteStatus;
    using Tensor             = GPUDelegateWrapper.TfLiteTensor;
    using GPUDelegate        = GPUDelegateWrapper.TfLiteDelegate;
    using GPUOptions         = GPUDelegateWrapper.TfLiteGpuDelegateOptionsV2;
    #endregion

    /// <summary>
    /// Tensor operations
    /// </summary>
    public class TensorOps{

        public static Tensor GetInputTensor(IntPtr TFInterpreter, int inputIndex){
            return Marshal.PtrToStructure<Tensor>(InterpreterWrapper.TfLiteInterpreterGetInputTensor(TFInterpreter, inputIndex));
        }

        public static Tensor GetOutputTensor(IntPtr TFInterpreter, int inputIndex){
            return Marshal.PtrToStructure<Tensor>(InterpreterWrapper.TfLiteInterpreterGetOutputTensor(TFInterpreter, inputIndex));
        }

        public static void ResizeInputTensor(IntPtr interpreter, int inputIndex, IntPtr inputDims, int inputDimsSize){
            int status = InterpreterWrapper.TfLiteInterpreterResizeInputTensor(interpreter, inputIndex, inputDims, inputDimsSize);
            #if DEBUG
            if(status == (int)Status.kTfLiteError){
                Debug.LogError("TensorOps.ResizeInputTensor(): Resize failed.");
            }
            #endif
        }

        public static int GetInputTensorCount(IntPtr interpreter){
            return InterpreterWrapper.TfLiteInterpreterGetInputTensorCount(interpreter);
        }

        public static void AllocateTensors(IntPtr interpreter){
            int status = InterpreterWrapper.TfLiteInterpreterAllocateTensors(interpreter);
            #if DEBUG
            if(status == (int)Status.kTfLiteError){
                Debug.LogError("TensorOps.AllocateTensors(): Tensor allocation failed.");
            }
            #endif
        }

        public static string GetTensorName(IntPtr tensor){
            return Marshal.PtrToStringAnsi(InterpreterWrapper.TfLiteTensorName(tensor));
        }

        public static void CreateTensorFromBuffer(IntPtr tensor, IntPtr inputData, int inputDataSize){
            int status = InterpreterWrapper.TfLiteTensorCopyFromBuffer(tensor, inputData, inputDataSize);
            #if DEBUG
            if(status == (int)Status.kTfLiteError){
                Debug.LogError("TensorOps.CreateTensorFromBuffer(): Tensor creation from buffer failed.");
            }
            #endif
        }

        public static void CopyTensorToBuffer(IntPtr tensor, IntPtr outputData, int outputDataSize){
            int status = InterpreterWrapper.TfLiteTensorCopyToBuffer(tensor, outputData, outputDataSize);
            #if DEBUG
            if(status == (int)Status.kTfLiteError){
                Debug.LogError("TensorOps.CreateTensorFromBuffer(): Tensor copying to buffer failed.");
            }
            #endif
        }

        public static QuantizationParams GetTensorQuantizationParams(IntPtr tensor){
            return Marshal.PtrToStructure<QuantizationParams>(InterpreterWrapper.TfLiteTensorQuantizationParams(tensor));
        }

        public static int GetTensorType(IntPtr tensor){
            return InterpreterWrapper.TfLiteTensorType(tensor);
        }

        public static int GetTensorDimensions(IntPtr tensor){
            return InterpreterWrapper.TfLiteTensorNumDims(tensor);
        }

        public static int GetTensorIndexDimension(IntPtr tensor, int dimIndex){
            return InterpreterWrapper.TfLiteTensorDim(tensor, dimIndex);
        }

        public static uint GetTensorInByteSize(IntPtr tensor){
            return InterpreterWrapper.TfLiteTensorByteSize(tensor);
        }

        public static IntPtr GetTensorData(IntPtr tensor){
            return InterpreterWrapper.TfLiteTensorData(tensor);
        }

    }

}