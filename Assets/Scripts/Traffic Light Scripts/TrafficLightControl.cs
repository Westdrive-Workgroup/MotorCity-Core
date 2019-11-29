using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* <summary>
 * Controls traffic light state and duration of the light on and off,
 * Sets the schedule among lights,
 * Defines relation between straight and turn lights. 
 * </summary>
 */
public class TrafficLightControl : MonoBehaviour {

    // Define both side of thraffic.
    [Header("Side Controls")]
    public TrafficLight enterOne;
    public TrafficLight enterOneInfront;
    public TrafficLight enterTwo;
    public TrafficLight enterTwoInfront;
    [Space]
    
    // Define duration between change of lights.
    [Header("Light Controls")]
    public float straightGreenTimer = 10f;
    public float yellowLightTimer = 2f;
    public float turnLeftGreenTimer = 5f;
    private bool systemLock = false;
    
    //Initialize traffic states
    public void Init()
    {
        enterOne.straighGreen = false;
        enterOne.straighYellow = false;
        enterOne.turnLeftGreen = false;
        enterOneInfront.straighGreen = false;
        enterOneInfront.straighYellow = false;
        enterOneInfront.turnLeftGreen = false;
        enterTwo.straighGreen = false;
        enterTwo.straighYellow = false;
        enterTwo.turnLeftGreen = false;
        enterTwoInfront.straighGreen = false;
        enterTwoInfront.straighYellow = false;
        enterTwoInfront.turnLeftGreen = false;
    }
    private void OnEnable()
    {
        Init();
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        enterOne.straighGreen = false;
        enterOne.straighYellow = false;
        enterOne.turnLeftGreen = false;
        enterOneInfront.straighGreen = false;
        enterOneInfront.straighYellow = false;
        enterOneInfront.turnLeftGreen = false;
        enterTwo.straighGreen = false;
        enterTwo.straighYellow = false;
        enterTwo.turnLeftGreen = false;
        enterTwoInfront.straighGreen = false;
        enterTwoInfront.straighYellow = false;
        enterTwoInfront.turnLeftGreen = false;
    }
    //Green light for both side,
    //yellow light scehduled later.
    private void SideOneStraightGreen()
    {
        systemLock = true;
        enterOne.straighGreen = true;
        enterOneInfront.straighGreen = true;
        Invoke("SideOneStraightYellow", straightGreenTimer);
    }
    //Green deactivated,
    //yellow lights are invoked, turing to red scheduled.
    private void SideOneStraightYellow()
    {
        enterOne.straighGreen = false;
        enterOneInfront.straighGreen = false;
        enterOne.straighYellow = true;
        enterOneInfront.straighYellow = true;
        Invoke("SideOneStraightRed", yellowLightTimer);
    }
    // Yellow deactivated,
    // Side one to turn left activated.
    private void SideOneStraightRed()
    {
        
        enterOne.straighYellow = false;
        enterOneInfront.straighYellow = false;
        SideOneTrueLeftGreen();
    }
    // Turn left from both side enabled.(green)
    // Yellow light is scheduled later.
    private void SideOneTrueLeftGreen()
    {
        enterOne.turnLeftGreen = true;
        enterOneInfront.turnLeftGreen = true;
        Invoke("SideOneTrueLeftYellow", turnLeftGreenTimer);
    }
    // Green light deactivated
    // Turn left lights are yellow
    // Turn left red is scheduled.
    private void SideOneTrueLeftYellow()
    {
        enterOne.turnLeftGreen = false;
        enterOneInfront.turnLeftGreen = false;
        enterOne.turnLeftYellow = true;
        enterOneInfront.turnLeftYellow = true;
        Invoke("SideOneTrueLeftRed", yellowLightTimer);
    }
    // Yellow lights deactivated,
    // Orthogonal road lights of the intersection activated
    // to green
    private void SideOneTrueLeftRed()
    {

        enterOne.turnLeftYellow = false;
        enterOneInfront.turnLeftYellow = false;
        SideTwoStraightGreen();
    }
    // Green light for both side,
    // Yellow light scehduled later.
    private void SideTwoStraightGreen()
    {
        enterTwo.straighGreen = true;
        enterTwoInfront.straighGreen = true;
        Invoke("SideTwoStraightYellow", straightGreenTimer);
    }
    // Green deactivated, 
    // Yellow light is on,
    // Red light scheduled later.
    private void SideTwoStraightYellow()
    {
        enterTwo.straighGreen = false;
        enterTwoInfront.straighGreen = false;
        enterTwo.straighYellow = true;
        enterTwoInfront.straighYellow = true;
        Invoke("SideTwoStraightRed", yellowLightTimer);
    }
    // Yellow deactivated,
    // Side one to turn left activated.
    private void SideTwoStraightRed()
    {

        enterTwo.straighYellow = false;
        enterTwoInfront.straighYellow = false;
        SideTwoTrueLeftGreen();
    }
    
    //Turn left from both side enabled.(green)
    // Yellow light is scheduled later.
    private void SideTwoTrueLeftGreen()
    {
        enterTwo.turnLeftGreen = true;
        enterTwoInfront.turnLeftGreen = true;
        Invoke("SideTwoTrueLeftYellow", turnLeftGreenTimer);
    }
    
    // Green light deactivated
    // Turn left lights are yellow
    // Turn left red is scheduled.
    private void SideTwoTrueLeftYellow()
    {
        enterTwo.turnLeftGreen = false;
        enterTwoInfront.turnLeftGreen = false;
        enterTwo.turnLeftYellow = true;
        enterTwoInfront.turnLeftYellow = true;
        Invoke("SideTwoTrueLeftRed", yellowLightTimer);
    }
    // Yellow lights deactivated,
    // Cycle done.
    private void SideTwoTrueLeftRed()
    {

        enterTwo.turnLeftYellow = false;
        enterTwoInfront.turnLeftYellow = false;
        systemLock = false;
    }
    
   // Update is called once per frame
    void FixedUpdate () {
        if(!systemLock)
            SideOneStraightGreen();
    }
}
