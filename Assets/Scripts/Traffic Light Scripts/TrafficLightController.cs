using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour {
    [Header("traffic light Control")]
    public TrafficLight relatedTrafficSide;
    public bool gostraightLight = true;
    [Space]
    [Header("Traffic Light settings")]
    public bool use_real_lights = true;
    public bool traffic_light_type_single = true;
    public float real_lights_range = 10.0f;
    public bool cast_shadows = true;
    [Space]
    [Header("Animation Settings")]
    public bool scriptedAnimation = true;
    public bool setAlwaysGreen = true;
    //-------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------
    int traffic_lights_mode;
    float traffic_lights_counter;
    int traffic_lights_last_frame_mode;

    Renderer traffic_light_renderer;

    int direction1_light_red_mat_index;
    int direction1_light_yellow_mat_index;
    int direction1_light_green_mat_index;

    Light lightsource_red1;
    Light lightsource_yellow1;
    Light lightsource_green1;
    Light lightsource_red2;
    Light lightsource_yellow2;
    Light lightsource_green2;
    // Use this for initialization,
    // Get light color to be changed.
    private void Awake()
    {
        traffic_light_renderer = gameObject.GetComponent<Renderer>();

        lightsource_red1 = transform.Find("LightRed1").GetComponent<Light>();
        lightsource_yellow1 = transform.Find("LightYellow1").GetComponent<Light>();
        lightsource_green1 = transform.Find("LightGreen1").GetComponent<Light>();
        if (!traffic_light_type_single)
        {
            lightsource_red2 = transform.Find("LightRed2").GetComponent<Light>();
            lightsource_yellow2 = transform.Find("LightYellow2").GetComponent<Light>();
            lightsource_green2 = transform.Find("LightGreen2").GetComponent<Light>();
        }
        if (use_real_lights == false)
        {
            Destroy(lightsource_red1);
            Destroy(lightsource_yellow1);
            Destroy(lightsource_green1);

            if (traffic_light_type_single == false)
            {
                Destroy(lightsource_red2);
                Destroy(lightsource_yellow2);
                Destroy(lightsource_green2);
            }
        }

        //-- global light parameters if real lights are enabled
        // currently not used
        if (use_real_lights == true)
        {
            lightsource_red1.GetComponent<Light>().range = real_lights_range;
            lightsource_yellow1.GetComponent<Light>().range = real_lights_range;
            lightsource_green1.GetComponent<Light>().range = real_lights_range;

            if (cast_shadows == false)
            {
                lightsource_red1.GetComponent<Light>().shadows = LightShadows.None;
                lightsource_yellow1.GetComponent<Light>().shadows = LightShadows.None;
                lightsource_green1.GetComponent<Light>().shadows = LightShadows.None;
            }

            if (traffic_light_type_single == false)
            {
                lightsource_red2.GetComponent<Light>().range = real_lights_range;
                lightsource_yellow2.GetComponent<Light>().range = real_lights_range;
                lightsource_green2.GetComponent<Light>().range = real_lights_range;

                if (cast_shadows == false)
                {
                    lightsource_red1.GetComponent<Light>().shadows = LightShadows.None;
                    lightsource_yellow1.GetComponent<Light>().shadows = LightShadows.None;
                    lightsource_green1.GetComponent<Light>().shadows = LightShadows.None;
                }
            }
        }
    }
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (scriptedAnimation)
        {
            if (gostraightLight)
            {
                if (relatedTrafficSide.straighGreen && !relatedTrafficSide.straighYellow)
                {
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[4].SetColor("_EmissionColor", Color.red);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", Color.black);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[3].SetColor("_EmissionColor", Color.black);
                    if (use_real_lights == true)
                    {
                        lightsource_green1.GetComponent<Light>().enabled = true;
                        lightsource_red1.GetComponent<Light>().enabled = false;
                        lightsource_yellow1.GetComponent<Light>().enabled = false;
                        if (!traffic_light_type_single)
                        {
                            lightsource_green1.GetComponent<Light>().enabled = true;
                            lightsource_red1.GetComponent<Light>().enabled = false;
                            lightsource_yellow1.GetComponent<Light>().enabled = false;
                        }
                    }
                }
                else if (!relatedTrafficSide.straighGreen && !relatedTrafficSide.straighYellow)
                {
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[4].SetColor("_EmissionColor", Color.black);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", Color.green);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[3].SetColor("_EmissionColor", Color.black);
                    if (use_real_lights == true)
                    {
                        lightsource_green1.GetComponent<Light>().enabled = false;
                        lightsource_red1.GetComponent<Light>().enabled = true;
                        lightsource_yellow1.GetComponent<Light>().enabled = false;
                        if (!traffic_light_type_single)
                        {
                            lightsource_green1.GetComponent<Light>().enabled = false;
                            lightsource_red1.GetComponent<Light>().enabled = true;
                            lightsource_yellow1.GetComponent<Light>().enabled = false;
                        }
                    }
                }
                else if (!relatedTrafficSide.straighGreen && relatedTrafficSide.straighYellow)
                {
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[4].SetColor("_EmissionColor", Color.black);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", Color.black);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[3].SetColor("_EmissionColor", Color.yellow);
                    if (use_real_lights == true)
                    {
                        lightsource_green1.GetComponent<Light>().enabled = false;
                        lightsource_red1.GetComponent<Light>().enabled = false;
                        lightsource_yellow1.GetComponent<Light>().enabled = true;
                        if (!traffic_light_type_single)
                        {
                            lightsource_green1.GetComponent<Light>().enabled = false;
                            lightsource_red1.GetComponent<Light>().enabled = true;
                            lightsource_yellow1.GetComponent<Light>().enabled = true;
                        }
                    }
                }
            }
            else
            {
                if (relatedTrafficSide.turnLeftGreen && !relatedTrafficSide.turnLeftYellow)
                {
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[4].SetColor("_EmissionColor", Color.red);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", Color.black);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[3].SetColor("_EmissionColor", Color.black);
                    if (use_real_lights == true)
                    {
                        lightsource_green1.GetComponent<Light>().enabled = true;
                        lightsource_red1.GetComponent<Light>().enabled = false;
                        lightsource_yellow1.GetComponent<Light>().enabled = false;
                        if (!traffic_light_type_single)
                        {
                            lightsource_green1.GetComponent<Light>().enabled = true;
                            lightsource_red1.GetComponent<Light>().enabled = false;
                            lightsource_yellow1.GetComponent<Light>().enabled = false;
                        }
                    }
                }
                else if (!relatedTrafficSide.turnLeftGreen && !relatedTrafficSide.turnLeftYellow)
                {
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[4].SetColor("_EmissionColor", Color.black);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", Color.green);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[3].SetColor("_EmissionColor", Color.black);
                    if (use_real_lights == true)
                    {
                        lightsource_green1.GetComponent<Light>().enabled = false;
                        lightsource_red1.GetComponent<Light>().enabled = true;
                        lightsource_yellow1.GetComponent<Light>().enabled = false;
                        if (!traffic_light_type_single)
                        {
                            lightsource_green1.GetComponent<Light>().enabled = false;
                            lightsource_red1.GetComponent<Light>().enabled = true;
                            lightsource_yellow1.GetComponent<Light>().enabled = false;
                        }
                    }
                }
                else if (!relatedTrafficSide.turnLeftGreen && relatedTrafficSide.turnLeftYellow)
                {
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[4].SetColor("_EmissionColor", Color.black);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", Color.black);
                    traffic_light_renderer.transform.GetComponent<Renderer>().materials[3].SetColor("_EmissionColor", Color.yellow);
                    if (use_real_lights == true)
                    {
                        lightsource_green1.GetComponent<Light>().enabled = true;
                        lightsource_red1.GetComponent<Light>().enabled = false;
                        lightsource_yellow1.GetComponent<Light>().enabled = true;
                        if (!traffic_light_type_single)
                        {
                            lightsource_green1.GetComponent<Light>().enabled = false;
                            lightsource_red1.GetComponent<Light>().enabled = true;
                            lightsource_yellow1.GetComponent<Light>().enabled = true;
                        }
                    }
                }
            }

        }
        else
        {
            if (setAlwaysGreen)
            {
                traffic_light_renderer.transform.GetComponent<Renderer>().materials[4].SetColor("_EmissionColor", Color.green);
                traffic_light_renderer.transform.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", Color.black);
                traffic_light_renderer.transform.GetComponent<Renderer>().materials[3].SetColor("_EmissionColor", Color.black);
                if (use_real_lights == true)
                {
                    lightsource_green1.GetComponent<Light>().enabled = true;
                    lightsource_red1.GetComponent<Light>().enabled = false;
                    lightsource_yellow1.GetComponent<Light>().enabled = false;
                    if (!traffic_light_type_single)
                    {
                        lightsource_green1.GetComponent<Light>().enabled = true;
                        lightsource_red1.GetComponent<Light>().enabled = false;
                        lightsource_yellow1.GetComponent<Light>().enabled = false;
                    }
                }
            }
        }
    }
}
