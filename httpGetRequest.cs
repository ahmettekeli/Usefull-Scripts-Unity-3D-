using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System;
using UnityEngine;

public class httpGetRequest : MonoBehaviour {


    //getting all of the text content(usually json text) 
    //from the given website (from url) and returning it with a single string 

    public string getJson(string queryType, string valueType, int value)
    {
        string urlAddress = "http://myWebsite.com.tr/myproject.aspx?type=" + queryType + "&" + valueType + "=" + value;
        
        try
        {


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
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
            response.Close();
            readStream.Close();
            Debug.Log("closing all connections");

            return data;
        }

        catch (Exception e)
        {
            Debug.Log("Error:\n" + e);
            return "Error: "+ e.ToString();
        }
    }

    //converting each json element into a specified class object list.
    public List<myClass> jsonProductDeserialize(string jsonData)
    {
        List<myClass> cList = JsonConvert.DeserializeObject<List<myClass>>(jsonData);
        return cList;
    }

    //converting json into a specified class object.
    public myClass jsonProductDeserialize(string jsonData)
    {
        myClass cm = JsonConvert.DeserializeObject<myClass>(jsonData);
        return cm;
    }




}
