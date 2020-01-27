using System;
using System.Runtime.InteropServices;

namespace TFLite{

    #region Aliases
    using Model              = InterpreterWrapper.TfLiteModel;
    using QuantizationParams = InterpreterWrapper.TfLiteQuantizationParams;
    using Interpreter        = InterpreterWrapper.TfLiteInterpreter;
    using Options            = InterpreterWrapper.TfLiteInterpreterOptions;
    using Status             = InterpreterWrapper.TfLiteStatus;
    using Tensor             = GPUDelegateWrapper.TfLiteTensor;
    using GPUDelegate        = GPUDelegateWrapper.TfLiteDelegate;
    using GPUOptions         = GPUDelegateWrapper.TfLiteGpuDelegateOptionsV2;
    #endregion

    public class Interpreter : IDisposable{

        /// TODO: Implement the functions from <see cref="InterpreterWrapper"/>

        /// <summary>
        /// The deep learning model to be written into memory
        /// </summary>
        public Model TFModel{get; private set;}

        /// <summary>
        /// The interpreter for managing the inference
        /// </summary>
        public Interpreter TFInterpreter{get; private set;}

        /// <summary>
        /// The gpu delegate to be used with the interpreter
        /// </summary>
        public GPUDelegate gpuDelegate{get; private set;}

        /// <summary>
        /// Creates the model from its name
        /// Default: gpu-ready model
        /// </summary>
        public Interpreter(string filename){
            // Initialize the model.
            TFModel       = Marshal.PtrToStructure<Model>(InterpreterWrapper.TfLiteModelCreateFromFile(filename));
            if(TFModel == null){
                throw new Exception("Model could not be initialized...");
            }

            // Setup the GPU delegate.
            GPUOptions  = GPUDelegateWrapper.TfLiteGpuDelegateOptionsV2Default();
            gpuDelegate = GPUDelegateWrapper.TfLiteGpuDelegateV2Create(GPUOptions);
            if(gpuDelegate == null){
                throw new Exception("GPU delegate could not be initialized...");
            }

            // Initialize the interpreter.
            Options options = InterpreterWrapper.TfLiteInterpreterOptionsCreate();
            InterpreterWrapper.TfLiteInterpreterOptionsAddDelegate(options, gpuDelegate);
            TFInterpreter = InterpreterWrapper.TfLiteInterpreterCreate(TFModel, options);
            if(TFInterpreter == null){
                throw new Exception("Interpreter could not be initialized...");
            }
        }

        public override void Dispose(){
            // Delete the interpreter.
            InterpreterWrapper.TfLiteInterpreterDelete(TFInterpreter);
            // Delete model from memory.
            InterpreterWrapper.TfLiteModelDelete(TFModel);
        }

    }

}