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

    public class Interpreter : IDisposable{

        /// <summary>
        /// The deep learning model to be written into memory
        /// </summary>
        public IntPtr TFModel{get; private set;}

        /// <summary>
        /// The interpreter for managing the inference
        /// </summary>
        public IntPtr TFInterpreter{get; private set;}

        /// <summary>
        /// The gpu delegate to be used with the interpreter
        /// </summary>
        public IntPtr gpuDelegate{get; private set;}

        /// <summary>
        /// Creates the model from its content
        /// Default: gpu-ready model
        /// </summary>
        public Interpreter(byte[] modelData){

            TFInterpreter = IntPtr.Zero;
            TFModel       = IntPtr.Zero;
            gpuDelegate   = IntPtr.Zero;

            // Initialize the model.
            GCHandle modelDataHandle = GCHandle.Alloc(modelData, GCHandleType.Pinned);
            IntPtr modelDataPtr = modelDataHandle.AddrOfPinnedObject();
            TFModel = InterpreterWrapper.TfLiteModelCreate(modelDataPtr, (uint)modelData.Length);
            if(TFModel == IntPtr.Zero){
                throw new Exception("Model could not be initialized...");
            }
            #if DEBUG
            else{
                Debug.Log("Model initialized.");
            }
            #endif

            // Setup the GPU delegate.
            IntPtr gpuOptions  = GPUDelegateWrapper.TfLiteGpuDelegateOptionsV2Default();
            gpuDelegate = GPUDelegateWrapper.TfLiteGpuDelegateV2Create(gpuOptions);
            if(gpuDelegate == IntPtr.Zero){
                throw new Exception("GPU delegate could not be initialized...");
            }
            #if DEBUG
            else{
                Debug.Log("GPU Delegate initialized.");
            }
            #endif

            // Initialize the interpreter.
            IntPtr options = InterpreterWrapper.TfLiteInterpreterOptionsCreate();
            InterpreterWrapper.TfLiteInterpreterOptionsAddDelegate(options, gpuDelegate);
            TFInterpreter = InterpreterWrapper.TfLiteInterpreterCreate(TFModel, options);
            if(TFInterpreter == IntPtr.Zero){
                throw new Exception("Interpreter could not be initialized...");
            }
            #if DEBUG
            else{
                Debug.Log("Interpreter initialized.");
            }
            #endif
        }

        // TODO: Implement other functionalities for runtime inference.

        public void Dispose(){
            // Delete the interpreter.
            InterpreterWrapper.TfLiteInterpreterDelete(TFInterpreter);
            // Delete model from memory.
            InterpreterWrapper.TfLiteModelDelete(TFModel);
            // Delete the gpu delegate.
            GPUDelegateWrapper.TfLiteGpuDelegateV2Delete(gpuDelegate);
        
            // Reset the pointers.
            TFInterpreter = IntPtr.Zero;
            TFModel       = IntPtr.Zero;
            gpuDelegate   = IntPtr.Zero;
        }

    }

}