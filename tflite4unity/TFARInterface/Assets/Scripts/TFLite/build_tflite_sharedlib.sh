# Branch: r2.1
# Setup: NDK r21, Bazel 0.29.1
#
# This script builds tflite shared libraries for Android.
# Start this script from the root tensorflow folder.

# Start configuration.
./configure

# Build the tflite lib.
bazel build //tensorflow/lite:libtensorflowlite.so --config android_arm64 --cxxopt='--std=c++11' -c opt

# Build the gpu delegate.
bazel build -c opt --config android_arm64 --copt -Os --copt -DTFLITE_GPU_BINARY_RELEASE --copt -Xlinker -s --strip always //tensorflow/lite/delegates/gpu:libtensorflowlite_gpu_delegate.so

