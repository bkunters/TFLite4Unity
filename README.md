# TensorflowLite4Unity

## Project Description
TFLite models are normally not supported by the ml-agents framework. This causes the problem that quantized models can not be currently supported by the Unity Inference Engine whereas performance drawbacks are inevitable during the runtime. Currently, tflite models' performances are backed by the GPU delegates which have been implemented in the mean time. Especially, with the development of frameworks like MediaPipe, Unity could need more power on the Android side due to the unsupport of the TFLite models. This plugin helps to integrate TFLite inference into the Unity Engine and also to the R&D teams for preparing quick prototypes as the Unity Engine has an easy environment to accomplish this aim.

## Roadmap
1.  Simple TFLite interpreter integration.
2.  Reusable plug&play API on the C# side to reduce development time without rewriting the same components again.
3.  Integration into the AR Foundations package to use computer vision neural nets for several tasks.
4.  Performance optimizations.

## Project Structure

## Contributions
Contributions are welcomed and encouraged!

1.  TFLite plugin can be ported to C++ for better performance.
2.  You can add your .tflite models into the /assets folder to share them for reuse. 

