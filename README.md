# TensorflowLite4Unity

## Project Description
TFLite models are normally not supported by the ml-agents framework. This causes the problem that quantized models can not be currently run by the Unity Inference Engine whereas performance drawbacks are inevitable during the runtime. Currently, tflite models' performances are backed by the GPU delegates which have been implemented in the mean time and are being developed further. Especially, with the development of frameworks like MediaPipe, Unity could need more power on the Android side due to the unsupport of the TFLite models. This plugin helps to integrate TFLite inference into the Unity Engine and also to the R&D teams for preparing quick prototypes as the Unity Engine has an easy environment to accomplish this purpose.

## Roadmap
1.  Simple TFLite interpreter integration(in development).
2.  Reusable plug&play API on the C# side to reduce development time without rewriting the same components again(in development).
3.  Easy integration into the AR Foundations environment to use computer vision neural nets for several tasks for AR scenes(in development).
4.  Performance optimizations.

## Project Structure

[TFARInterface](tflite4unity/TFARInterface) : A Unity interface to use the tflite plugin.

[TFLite Android](tflite4unity/TFLitePlugin) : A plugin to support tflite models on the Android side.
TODO: iOS plugin will be added.

## Contributions
Contributions are welcomed and encouraged!

1.  TFLite plugin will be ported to C++ for better performance.
2.  You can add your .tflite models into the /assets folder to share them for reuse(pull request with model details is encouraged!). 

## Acknowledgments

Thanks to the [MediaPipe Team](https://github.com/google/mediapipe) for having published their useful models for essential AR interactions!

## Tech Stack

1.  Tensorflow Lite 2.0.0
2.  Unity 2019.2(recommended)
3.  Android Studio
