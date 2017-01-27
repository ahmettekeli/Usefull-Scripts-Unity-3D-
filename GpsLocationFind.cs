using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CompanyDistance
{
    public int ID { get; set; }
    public float distance { get; set; }

}

public class GpsLocationFind : MonoBehaviour {

    public static int sClosestCompanyID;

    private float latitude, longitude, myLatitude, myLongitude;

    public Text debugLogText, distanceText;

    List<CompanyDistance> myCompanyList = new List<CompanyDistance>();
    public List<Locations> companyList = new List<Locations>();

    private LocationInfo startLocation, lastLocation;

    public string companyJsonUrl = "http://mywebsite.com.tr/myProject.aspx?type=company";


    void Start()
    {
        //getting all the companies from the given url deserializing the json text into a string
        jsonConnection();
    }

    public void GpsButtonClick()
    {
        GpsStop();
        GpsStart();
    }

    public void jsonConnection()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(companyJsonUrl);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string data = "";

        Stream receiveStream = response.GetResponseStream();
        StreamReader readStream = null;

        if (response.CharacterSet == null)
        {
            readStream = new StreamReader(receiveStream);
        }
        else
        {
            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
        }

        data = readStream.ReadToEnd();

        companyList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Locations>>(data);
        GpsButtonClick();
    }

    public void GpsStart()
    {
        if (Input.location.isEnabledByUser)
        {
            if (Input.location.status == LocationServiceStatus.Initializing || Input.location.status == LocationServiceStatus.Running)
            {
                Input.location.Start(100); // 100 eski degeri

                if (Input.location.status == LocationServiceStatus.Running)
                {
                    StopCoroutine(findMyLocation());       //stop process
                    Debug.Log("GPS ALREADY RUNNING");
                }
                StartCoroutine(findMyLocation());        //start process
            }
            else
            {
                Input.location.Start(100);                // 100 eski degeri
                StartCoroutine(findMyLocation());        //start process bunu kapatınca konum servisleri aktif olunca farketmiyor

            }
        }
        else
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                PcLocation();
            }
            else
            {
                GpsDisabled();
            }
        }
    }

    public void jsonSerialize(float myLocationLatitude, float myLocationLongitude)
    {
        if (companyList.Count != 0)
        {
            foreach (var Company in companyList)
            {

                CompanyDistance myCompanyDistance = new CompanyDistance();
                if (!string.IsNullOrEmpty(Company.Coordinate))
                {
                    float locationLatitude = float.Parse(Company.Coordinate.Split(';')[0]);
                    float locationLongitude = float.Parse(Company.Coordinate.Split(';')[1]);
                    float distance = CalculateDistance(myLocationLatitude, myLocationLongitude, locationLatitude, locationLongitude);
                    myCompanyDistance.ID = Company.ID;
                    myCompanyDistance.distance = distance;
                    myCompanyList.Add(myCompanyDistance);

                    Debug.Log("Company ID: " + Company.ID + "       Company Distance: " + DistanceDisplay(distance));
                }
                else if (string.IsNullOrEmpty(Company.Coordinate))
                {
                    Debug.Log("Company ID: " + Company.ID + "Coordinates Value NULL!!!");
                }
            }
            findMinimumDistance();
        }
        else
        {
            Debug.Log("Error: Json Company List Empty!");
        }
    }

    public int findMinimumDistance()
    {
        CompanyDistance smallestDistance = new CompanyDistance();
        smallestDistance.distance = 10000f;
        foreach (var myDistance in myCompanyList)
        {
            if (smallestDistance.distance > myDistance.distance)
            {
                smallestDistance = myDistance;
            }
        }
        //GameObject.Find("Distance").GetComponent<Text>().text = "My Smallest Distance ID: " + smallestDistance.ID + "\nMy Smallest Distance: " + DistanceDisplay(smallestDistance.distance);
        sClosestCompanyID = smallestDistance.ID;
        GpsStop();// after finding minimum distance stop location works
        return sClosestCompanyID;
    }


    public float CalculateDistance(float lat1, float lon1, float lat2, float lon2)
    {
        //calculate distance between two point with earth radius 
        float R = 6378.137f; // Radius of earth in KM
        float dLat = lat2 * Mathf.PI / 180 - lat1 * Mathf.PI / 180;
        float dLon = lon2 * Mathf.PI / 180 - lon1 * Mathf.PI / 180;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
        Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) *
        Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float d = R * c;
        return d;
    }

    public string DistanceDisplay(float d)
    {
        string distance;
        if (d < 1 && d > 0)
        {
            d = d * 1000;
            distance = d.ToString("F0") + " meter";    //meter
        }
        else
        {
            distance = d.ToString("F0") + " km";    //km
        }
        return distance;
    }
    public void GpsStop()
    {
        Input.location.Stop();
    }


    IEnumerator findMyLocation()
    {

        while (true)
        {

            if (Input.location.status == LocationServiceStatus.Stopped)
            {
                Debug.Log("STOPPED?");
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("FAILED");
            }

            if (Input.location.status == LocationServiceStatus.Initializing)
            {
                Debug.Log("INITIALISING");
            }

            if (Input.location.status == LocationServiceStatus.Running)
            {
                Debug.Log("RUNNING SUCCESS");
                startLocation = Input.location.lastData;
                while (Input.location.status == LocationServiceStatus.Running)
                {
                    if (UnityEngine.Input.location.lastData.longitude <= 180.0f
                         && UnityEngine.Input.location.lastData.longitude >= -180.0f
                         && UnityEngine.Input.location.lastData.latitude <= 90.0f
                         && UnityEngine.Input.location.lastData.latitude >= -90.0f)
                    {
                        Debug.Log("INSIDE LOOP");
                        lastLocation = Input.location.lastData;

                        string gpsString = "Enlem: " + lastLocation.latitude + "; Boylam:" + lastLocation.longitude;
                        Debug.Log(gpsString);
                        GameObject.Find("Info").GetComponent<Text>().text = gpsString;

                        jsonSerialize(lastLocation.latitude, lastLocation.longitude);

                        yield return new WaitForSeconds(0.5f);


                    }
                    else
                    {
                        Debug.Log("WELCOME TO MARS");
                    }

                }
            }
            else
            {
                Debug.Log("Your Phone Gps Disabled!\nActivate Location Services");
            }
            Debug.Log("Checking");
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void PcLocation()
    {
        Debug.Log("GPS IS DISABLED Work on PC!");
        myLatitude = 36.998458f;
        myLongitude = 35.32503f;

        string gpsString = "Enlem: " + myLatitude + "; Boylam:" + myLongitude;

        jsonSerialize(myLatitude, myLongitude);
    }

    public virtual void GpsDisabled()
    {
        Debug.Log("Activate Location Services!\n Settings-Applications-Application Manager-BunD-Permissions-Enable Location");
    }

    public virtual void GpsFailed()
    {
        Debug.Log("GPS FAILED TO START");
    }
}

