<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var result = Parallel.For(0, 10000, (ctr) =>
	{
        var request = new MyWebRequest("http://localhost/Sample/WsdlSample.svc?wsdl");
        var wsdl = request.GetResponse();
        Console.WriteLine("Iteration {0,2}: {1}", ctr, wsdl.Length);
      });
	Console.WriteLine("Result: {0}", result.IsCompleted ? "Completed Normally" : 
	                                                     String.Format("Completed to {0}", result.LowestBreakIteration));
}

public class MyWebRequest
{
  private HttpWebRequest request;
  private Stream dataStream;

  private string status;

  public String Status
  {
      get { return status; }
      set { status = value; }
  }

  public MyWebRequest(string url)
  {
      // Create a request using a URL that can receive a post.
      request = (HttpWebRequest)WebRequest.Create(url);
  }

  public MyWebRequest(string url, string method)
      : this(url)
  {
      if (method.Equals("GET") || method.Equals("POST"))
      {
          // Set the Method property of the request to POST.
          request.Method = method;
      }
      else
      {
          throw new Exception("Invalid Method Type");
      }
  }

  public MyWebRequest(string url, string method, string data)
      : this(url, method)
  {
      // Create POST data and convert it to a byte array.
      string postData = data;
      byte[] byteArray = Encoding.UTF8.GetBytes(postData);

      // Set the ContentType property of the WebRequest.
      request.ContentType = "application/x-www-form-urlencoded";

      // Set the ContentLength property of the WebRequest.
      request.ContentLength = byteArray.Length;

      // Get the request stream.
      dataStream = request.GetRequestStream();

      // Write the data to the request stream.
      dataStream.Write(byteArray, 0, byteArray.Length);

      // Close the Stream object.
      dataStream.Close();
  }

  public string GetResponse()
  {
      // Get the original response.
      WebResponse response = request.GetResponse();

      this.Status = ((HttpWebResponse)response).StatusDescription;

      // Get the stream containing all content returned by the requested server.
      dataStream = response.GetResponseStream();

      // Open the stream using a StreamReader for easy access.
      StreamReader reader = new StreamReader(dataStream);

      // Read the content fully up to the end.
      string responseFromServer = reader.ReadToEnd();

      // Clean up the streams.
      reader.Close();
      dataStream.Close();
      response.Close();

      return responseFromServer;
  }

	private void SetBasicAuthHeader(WebRequest req, String userName, String userPassword)
	{
		string authInfo = userName + ":" + userPassword;
		authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
		req.Headers["Authorization"] = "Basic " + authInfo;
	}	
}