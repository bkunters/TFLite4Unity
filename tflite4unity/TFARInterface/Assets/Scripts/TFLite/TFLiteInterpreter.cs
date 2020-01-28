using System;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

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
    /// This class represents a tflite interpreter to run inference in the runtime
    /// The most basic pattern for running a model is:
    /// 1) Load the model(preferably in a Start() function)
    /// 2) Allocation of the tensors after a resize if needed
    /// 3) Setting the input tensor
    /// 4) Invoking the interpreter
    /// 5) Setting the output tensor
    ///
    /// Example:
    /// <code>
    /// Interpreter interpreter = new Interpreter("model.tflite");
    /// TensorOps.AllocateTensors(interpreter.TFInterpreter);
    /// Tensor inputTensor = Marshal.PtrToStructure<Tensor>(TensorOps.GetInputTensor(interpreter.TFInterpreter, 0));
    /// // Fill the input tensor...
    /// interpreter.InvokeModel();
    /// Tensor outputTensor = Marshal.PtrToStructure<Tensor>(TensorOps.GetOutputTensor(interpreter.TFInterpreter, 0));
    /// // Do what you want with the output tensor...
    /// </code>
    /// </summary>
    public class Interpreter : IDisposable{

        #region Fields
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
        #endregion

        #region Constructors
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

        /// <summary>
        /// Create the interpreter over the model name
        /// Default: gpu-ready model
        /// </summary>
        public Interpreter(string model_name) : this(Helpers.GetModelDataFromModelName(model_name)){}
        #endregion

        #region Public Methods
        public static string GetTFLiteVersion(){
            return InterpreterWrapper.TfLiteVersion();
        }

        /// <summary>
        /// Runs the model on allocated tensors
        /// </summary>
        public void InvokeModel(){
            int status = InterpreterWrapper.TfLiteInterpreterInvoke(TFInterpreter);
            if(status == (int)Status.kTfLiteError){
                throw new Exception("Model inference failed.");
            }
        }

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
        #endregion

    }

}