using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Inherit this class and customize the <c>RunInference</c>.
/// </summary>
public interface IInference
{
    float[] RunInference(byte[] input);
}
