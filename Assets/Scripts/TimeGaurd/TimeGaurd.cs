using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Gets and Sets the current Frame number
/// </summary>
public static class  TimeGaurd 
{
    [Header("Internals")]
    public static List<float> timePassed;
    public static List<int> frameCount;
    public static  List<int> rederedFrameCount;
    private static int frameNumber;
    
    public static int getCurrentFrame()
    {
        return frameNumber;
    }
    public static void setCurrentFrame(int newFrameNumber)
    {
        frameNumber = newFrameNumber;
    }
}
